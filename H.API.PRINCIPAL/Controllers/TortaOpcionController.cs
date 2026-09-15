using H.DataAccess.Entidades;
using H.DataAccess.UnitofWork;
using H.DTOs;
using H.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace H.API.PRINCIPAL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TortaOpcionController : ControllerBase
{
    private static readonly string[] TiposPermitidos = ["sabor", "tamanio", "relleno", "color", "pisos", "cobertura", "decoracion", "porciones", "evento"];
    private readonly IUnitOfWork _unitOfWork;

    public TortaOpcionController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    [HttpGet("torta/{idTorta:int}")]
    public IActionResult ObtenerActivas(int idTorta)
    {
        if (idTorta <= 0) return BadRequest(new { message = "La torta no es válida." });
        return Ok(new TortaOpcionService(_unitOfWork).ObtenerPorTorta(idTorta, true).Select(ToDto));
    }

    [HttpGet("admin/torta/{idTorta:int}")]
    [Authorize(Roles = "Administrador")]
    public IActionResult ObtenerTodas(int idTorta)
    {
        if (idTorta <= 0) return BadRequest(new { message = "La torta no es válida." });
        return Ok(new TortaOpcionService(_unitOfWork).ObtenerPorTorta(idTorta, false).Select(ToDto));
    }

    [HttpPost("upsert")]
    [Authorize(Roles = "Administrador")]
    public IActionResult Upsert([FromBody] TortaOpcionDTO dto)
    {
        if (!EsValido(dto))
            return BadRequest(new { message = "Los datos de la opción no son válidos." });

        var ahora = DateTime.UtcNow;
        var opcion = new TortaOpcion
        {
            Id = dto.Id,
            IdTorta = dto.IdTorta,
            Tipo = dto.Tipo.Trim().ToLowerInvariant(),
            Valor = dto.Valor.Trim(),
            PrecioExtra = decimal.Round(dto.PrecioExtra, 2),
            ModoPrecio = dto.ModoPrecio?.Trim().ToLowerInvariant() == "incremental" ? "incremental" : "fijo",
            PrecioPorUnidad = decimal.Round(Math.Max(0, dto.PrecioPorUnidad), 2),
            Obligatorio = dto.Obligatorio,
            Minimo = dto.Minimo,
            Activo = dto.Activo,
            Maximo = dto.Maximo,
            Orden = dto.Orden,
            UsuarioCreacion = User.Identity?.Name ?? "admin",
            UsuarioModificacion = User.Identity?.Name ?? "admin",
            FechaCreacion = ahora,
            FechaModificacion = ahora
        };

        var resultado = new TortaOpcionService(_unitOfWork).Upsert(opcion);
        return Ok(ToDto(resultado));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public IActionResult Eliminar(int id)
    {
        if (id <= 0) return BadRequest(new { message = "La opción no es válida." });
        return Ok(new TortaOpcionService(_unitOfWork).Delete(id, User.Identity?.Name ?? "admin"));
    }

    [HttpPost("{id:int}/estado")]
    [Authorize(Roles = "Administrador")]
    public IActionResult CambiarEstado(int id, [FromBody] EstadoOpcionRequest request)
    {
        if (id <= 0) return BadRequest(new { message = "La opción no es válida." });
        try
        {
            var resultado = new TortaOpcionService(_unitOfWork).CambiarActivo(id, request?.Activo == true, User.Identity?.Name ?? "admin");
            return Ok(ToDto(resultado));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    public sealed class EstadoOpcionRequest
    {
        public bool Activo { get; set; }
    }

    private static bool EsValido(TortaOpcionDTO dto) =>
        dto != null && dto.IdTorta > 0 &&
        TiposPermitidos.Contains(dto.Tipo?.Trim().ToLowerInvariant()) &&
        !string.IsNullOrWhiteSpace(dto.Valor) && dto.Valor.Trim().Length <= 150 &&
        dto.PrecioExtra >= 0 && dto.Maximo is null or > 0 && dto.Orden >= 0;

    private static TortaOpcionDTO ToDto(TortaOpcion opcion) => new()
    {
        Id = opcion.Id,
        IdTorta = opcion.IdTorta,
        Tipo = opcion.Tipo,
        Valor = opcion.Valor,
        PrecioExtra = opcion.PrecioExtra,
        ModoPrecio = opcion.ModoPrecio,
        PrecioPorUnidad = opcion.PrecioPorUnidad,
        Obligatorio = opcion.Obligatorio,
        Minimo = opcion.Minimo,
        Activo = opcion.Activo,
        Maximo = opcion.Maximo,
        Orden = opcion.Orden
    };
}
