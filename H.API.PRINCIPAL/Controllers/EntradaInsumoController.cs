using H.DataAccess.Extension;
using H.DataAccess.Helpers;
using H.DTOs;
using H.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    public class EntradaInsumoController : ControllerBase
    {
        private readonly IEntradaInsumoService _servicio;
        private readonly ICloudinaryService _cloudinaryService;

        public EntradaInsumoController(IEntradaInsumoService servicio, ICloudinaryService cloudinaryService)
        {
            _servicio = servicio;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar([FromBody] EntradaInsumoRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("El objeto no puede ser nulo.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    return BadRequest("Usuario requerido.");

                if (dto.Detalles == null || !dto.Detalles.Any())
                    return BadRequest("Debe agregar al menos un insumo.");

                string? imagenUrl = null;
                if (!string.IsNullOrEmpty(dto.ImagenBase64))
                {
                    try
                    {
                        var bytes = Convert.FromBase64String(dto.ImagenBase64);
                        var stream = new MemoryStream(bytes);
                        var formFile = new FormFile(stream, 0, bytes.Length, "imagen", "image.png");
                        imagenUrl = await _cloudinaryService.SubirImagenAsync(formFile, "entradas-insumos");
                    }
                    catch { }
                }
                var id = _servicio.Registrar(dto, imagenUrl);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("SubirImagen")]
        public async Task<IActionResult> SubirImagen([FromBody] SubirImagenEntradaInsumoDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.ImagenBase64))
                    return BadRequest("Debe proporcionar una imagen en Base64.");

                var bytes = Convert.FromBase64String(dto.ImagenBase64);
                var stream = new MemoryStream(bytes);
                var formFile = new FormFile(stream, 0, bytes.Length, "imagen", "image.png");
                var url = await _cloudinaryService.SubirImagenAsync(formFile, "entradas-insumos");
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerPendientes")]
        public IActionResult ObtenerPendientes()
        {
            try
            {
                var lista = _servicio.ObtenerPendientes();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerPorId")]
        public IActionResult ObtenerPorId(int id)
        {
            try
            {
                var item = _servicio.ObtenerPorId(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Aprobar")]
        public IActionResult Aprobar([FromBody] EntradaInsumoAprobarDTO dto)
        {
            try
            {
                var resultado = _servicio.Aprobar(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Rechazar")]
        public IActionResult Rechazar([FromBody] EntradaInsumoRechazarDTO dto)
        {
            try
            {
                var resultado = _servicio.Rechazar(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerTodos")]
        public IActionResult ObtenerTodos([FromQuery] EntradaInsumoFiltroDTO filtro)
        {
            try
            {
                if (filtro == null)
                    filtro = new EntradaInsumoFiltroDTO();
                var resultado = _servicio.ObtenerTodos(filtro);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerHistorial")]
        public IActionResult ObtenerHistorial(int id)
        {
            try
            {
                var historial = _servicio.ObtenerHistorial(id);
                return Ok(historial);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}