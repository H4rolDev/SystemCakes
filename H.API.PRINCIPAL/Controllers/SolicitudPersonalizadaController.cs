using H.DataAccess.Models;
using H.DataAccess;
using H.DTOs;
using H.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace H.API.PRINCIPAL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SolicitudPersonalizadaController : ControllerBase
{
    private readonly sistemContext _db;
    private readonly ICloudinaryService _cloudinary;

    public SolicitudPersonalizadaController(sistemContext db, ICloudinaryService cloudinary) { _db = db; _cloudinary = cloudinary; }

    [HttpPost("imagen")]
    [Authorize]
    public async Task<IActionResult> Imagen([FromBody] SubirImagenDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ImagenBase64)) return BadRequest(new { mensaje = "La imagen es requerida." });
        var raw = dto.ImagenBase64.Contains(',') ? dto.ImagenBase64.Split(',')[1] : dto.ImagenBase64;
        var bytes = Convert.FromBase64String(raw);
        await using var stream = new MemoryStream(bytes);
        var file = new FormFile(stream, 0, stream.Length, "file", $"referencia_{DateTime.UtcNow.Ticks}.jpg");
        return Ok(new { url = await _cloudinary.SubirImagenAsync(file, "solicitudes-personalizadas") });
    }

    [HttpGet("estimado")]
    [AllowAnonymous]
    public IActionResult Estimado([FromQuery] int? porciones, [FromQuery] int pisos = 1, [FromQuery] string? cobertura = null)
    {
        var portions = Math.Clamp(porciones ?? 15, 6, 300);
        var basePrice = 55m + Math.Max(0, portions - 10) * 3.5m;
        var floorExtra = Math.Max(0, pisos - 1) * 35m;
        var decorationExtra = cobertura?.ToLowerInvariant() switch
        {
            "fondant" => 55m,
            "modelado" => 85m,
            "buttercream" => 20m,
            _ => 0m
        };
        var total = basePrice + floorExtra + decorationExtra;
        return Ok(new { minimo = Math.Round(total, 2), maximo = Math.Round(total * 1.35m, 2), mensaje = "Estimado referencial. El precio final se confirma tras revisar el diseño." });
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Crear([FromBody] CrearSolicitudPersonalizadaDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Descripcion)) return BadRequest(new { mensaje = "Describe el diseño que deseas." });
        var personaId = PersonaId();
        if (personaId <= 0 && (string.IsNullOrWhiteSpace(dto.NombreCliente) || string.IsNullOrWhiteSpace(dto.EmailCliente) || string.IsNullOrWhiteSpace(dto.TelefonoCliente)))
            return BadRequest(new { mensaje = "Para continuar como invitado indica nombre, correo y teléfono." });
        var now = DateTime.UtcNow.AddHours(-5);
        var estimate = Estimate(dto);
        var request = new TSolicitudPersonalizada
        {
            Codigo = $"SP-{now:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            IdPersona = personaId,
            NombreCliente = dto.NombreCliente?.Trim(), EmailCliente = dto.EmailCliente?.Trim(), TelefonoCliente = dto.TelefonoCliente?.Trim(),
            TokenAcceso = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(24)),
            Descripcion = dto.Descripcion.Trim(), ImagenReferencia = dto.ImagenReferencia,
            Evento = dto.Evento, Sabor = dto.Sabor, Relleno = dto.Relleno, Tamano = dto.Tamano,
            Porciones = dto.Porciones, Pisos = Math.Clamp(dto.Pisos, 1, 5), Cobertura = dto.Cobertura,
            Colores = dto.Colores, TextoDecorativo = dto.TextoDecorativo,
            FechaEntregaSolicitada = dto.FechaEntregaSolicitada, PresupuestoMinimo = dto.PresupuestoMinimo,
            PresupuestoMaximo = dto.PresupuestoMaximo, EstimadoMinimo = estimate.minimo, EstimadoMaximo = estimate.maximo,
            Estado = "Pendiente", Activo = true, UsuarioCreacion = User.Identity?.Name ?? "invitado", FechaCreacion = now
        };
        _db.TSolicitudPersonalizada.Add(request);
        _db.TSolicitudHistorial.Add(History(request, null, "Pendiente", "Solicitud enviada. Nuestro equipo revisará tu diseño."));
        _db.SaveChanges();
        return Ok(Detalle(request.Id));
    }

    [HttpGet("mis-solicitudes")]
    [Authorize]
    public IActionResult MisSolicitudes()
    {
        var id = PersonaId();
        var items = _db.TSolicitudPersonalizada
            .Where(x => x.Activo && x.IdPersona == id)
            .OrderByDescending(x => x.FechaCreacion)
            .ToList()
            .Select(Resumen)
            .ToList();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult Obtener(int id)
    {
        var request = _db.TSolicitudPersonalizada.FirstOrDefault(x => x.Id == id && x.Activo);
        if (request == null || (!EsAdmin() && request.IdPersona != PersonaId())) return NotFound();
        return Ok(Detalle(id));
    }

    [HttpGet("publica/{codigo}/{token}")]
    [AllowAnonymous]
    public IActionResult ObtenerPublica(string codigo, string token)
    {
        var request = _db.TSolicitudPersonalizada.FirstOrDefault(x => x.Activo && x.Codigo == codigo && x.TokenAcceso == token);
        return request == null ? NotFound() : Ok(Detalle(request.Id));
    }

    [HttpGet("admin/listado")]
    [Authorize]
    public IActionResult AdminListado([FromQuery] string? estado = null)
    {
        if (!EsAdmin()) return Forbid();
        var query = _db.TSolicitudPersonalizada.Where(x => x.Activo);
        if (!string.IsNullOrWhiteSpace(estado)) query = query.Where(x => x.Estado == estado);
        var items = query.OrderByDescending(x => x.FechaCreacion).ToList().Select(Resumen).ToList();
        return Ok(items);
    }

    [HttpPost("{id:int}/cotizar")]
    [Authorize]
    public IActionResult Cotizar(int id, [FromBody] CotizarSolicitudDTO dto)
    {
        if (!EsAdmin()) return Forbid();
        if (dto.PrecioFinal <= 0 || dto.FechaEntrega.Date < DateTime.Today) return BadRequest(new { mensaje = "Indica un precio válido y una fecha futura." });
        var request = _db.TSolicitudPersonalizada.FirstOrDefault(x => x.Id == id && x.Activo);
        if (request == null) return NotFound();
        var version = _db.TCotizacionPersonalizada.Count(x => x.IdSolicitud == id) + 1;
        var now = DateTime.UtcNow.AddHours(-5);
        _db.TCotizacionPersonalizada.Where(x => x.IdSolicitud == id && x.Activo).ToList().ForEach(x => { x.Activo = false; x.Estado = "Reemplazada"; });
        _db.TCotizacionPersonalizada.Add(new TCotizacionPersonalizada
        {
            IdSolicitud = id, Version = version, PrecioFinal = dto.PrecioFinal, Adelanto = Math.Clamp(dto.Adelanto, 0, dto.PrecioFinal),
            CostoDelivery = Math.Max(0, dto.CostoDelivery), FechaEntrega = dto.FechaEntrega, HoraEntrega = dto.HoraEntrega,
            FechaVencimiento = now.AddHours(Math.Clamp(dto.VigenciaHoras, 1, 168)), Observaciones = dto.Observaciones,
            Estado = "Enviada", Activo = true, UsuarioCreacion = User.Identity?.Name ?? "administrador", FechaCreacion = now
        });
        CambiarEstado(request, "Cotizada", $"Cotización v{version} enviada al cliente.");
        _db.SaveChanges();
        return Ok(Detalle(id));
    }

    [HttpPost("{id:int}/estado")]
    [Authorize]
    public IActionResult Estado(int id, [FromBody] CambiarEstadoSolicitudDTO dto)
    {
        var request = _db.TSolicitudPersonalizada.FirstOrDefault(x => x.Id == id && x.Activo);
        if (request == null || (!EsAdmin() && request.IdPersona != PersonaId())) return NotFound();
        var allowed = new[] { "Pendiente", "En revisión", "Falta información", "Cambios solicitados", "Aceptada", "Rechazada", "Vencida", "En producción", "Lista" };
        if (!allowed.Contains(dto.Estado)) return BadRequest(new { mensaje = "Estado no válido." });
        if (!EsAdmin() && dto.Estado is not ("Aceptada" or "Cambios solicitados" or "Rechazada")) return Forbid();
        CambiarEstado(request, dto.Estado, dto.Comentario); _db.SaveChanges();
        return Ok(Detalle(id));
    }

    private int PersonaId()
    {
        if (!int.TryParse(User.FindFirstValue("idPersona") ?? User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return 0;
        return _db.TUsuario.Where(x => x.Id == userId).Select(x => x.IdPersona).FirstOrDefault();
    }
    private bool EsAdmin() => User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value.ToLowerInvariant()).Any(x => x is "administrador" or "atención" or "atencion" or "producción" or "produccion");
    private void CambiarEstado(TSolicitudPersonalizada item, string state, string? comment)
    {
        _db.TSolicitudHistorial.Add(History(item, item.Estado, state, comment)); item.Estado = state; item.FechaModificacion = DateTime.UtcNow.AddHours(-5); item.UsuarioModificacion = User.Identity?.Name ?? "sistema";
    }
    private object Detalle(int id)
    {
        var item = _db.TSolicitudPersonalizada.First(x => x.Id == id);
        var quote = _db.TCotizacionPersonalizada.Where(x => x.IdSolicitud == id).OrderByDescending(x => x.Version).FirstOrDefault();
        var history = _db.TSolicitudHistorial.Where(x => x.IdSolicitud == id).OrderBy(x => x.FechaCreacion).ToList();
        return new { solicitud = Resumen(item), cotizacion = quote, historial = history };
    }
    private object Resumen(TSolicitudPersonalizada x) => new { x.Id, x.Codigo, x.IdPersona, x.NombreCliente, x.EmailCliente, x.TelefonoCliente, x.TokenAcceso, x.Descripcion, x.ImagenReferencia, x.Evento, x.Sabor, x.Relleno, x.Tamano, x.Porciones, x.Pisos, x.Cobertura, x.Colores, x.TextoDecorativo, x.FechaEntregaSolicitada, x.EstimadoMinimo, x.EstimadoMaximo, x.Estado, x.Observaciones, x.FechaCreacion };
    private static TSolicitudHistorial History(TSolicitudPersonalizada item, string? previous, string next, string? comment) => new() { IdSolicitud = item.Id, EstadoAnterior = previous, EstadoNuevo = next, Comentario = comment, Activo = true, UsuarioCreacion = "sistema", FechaCreacion = DateTime.UtcNow.AddHours(-5) };
    private static (decimal minimo, decimal maximo) Estimate(CrearSolicitudPersonalizadaDTO dto)
    {
        var basePrice = 55m + Math.Max(0, (dto.Porciones ?? 15) - 10) * 3.5m + Math.Max(0, dto.Pisos - 1) * 35m;
        if (dto.Cobertura?.ToLowerInvariant() == "fondant") basePrice += 55;
        return (Math.Round(basePrice, 2), Math.Round(basePrice * 1.35m, 2));
    }
}
