using H.DataAccess.Entidades;
using H.DataAccess.Helpers;
using H.Services;
using H.DataAccess.Extension;
using H.DataAccess.UnitofWork;
using Microsoft.AspNetCore.Mvc;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaTortaController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public CategoriaTortaController(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            this.unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("Insertar")]
        public IActionResult Insert([FromBody] CategoriaTorta producto)
        {
            try
            {
                var servicio = new CategoriaTortaService(unitOfWork, _cloudinaryService);
                producto.FechaCreacion = Fecha.Hoy;
                producto.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Add(producto);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Modificar")]
        public IActionResult Update([FromBody] CategoriaTorta producto)
        {
            try
            {
                var servicio = new CategoriaTortaService(unitOfWork, _cloudinaryService);
                producto.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Update(producto);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpDelete("Eliminar")]
        public IActionResult Delete(int id, string usuario)
        {
            try
            {
                var servicio = new CategoriaTortaService(unitOfWork, _cloudinaryService);
                var respuesta = servicio.Delete(id, usuario);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerListadoPorId")]
        public IActionResult ObtenerListadoPorId(int id)
        {
            try
            {
                var servicio = new CategoriaTortaService(unitOfWork, _cloudinaryService);
                return Ok(servicio.GetById(id));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerCombo")]
        public IActionResult ObtenerCombo()
        {
            try
            {
                var servicio = new CategoriaTortaService(unitOfWork, _cloudinaryService);
                return Ok(servicio.ObtenerCombo());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}