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
    public class InsumoController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public InsumoController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("Insertar")]
        public IActionResult Insert([FromBody] Insumo categoria)
        {
            try
            {
                var servicio = new InsumoService(unitOfWork);
                categoria.FechaCreacion = Fecha.Hoy;
                categoria.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Add(categoria);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Modificar")]
        public IActionResult Update([FromBody] Insumo categoria)
        {
            try
            {
                var servicio = new InsumoService(unitOfWork);
                categoria.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Update(categoria);
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
                var servicio = new InsumoService(unitOfWork);
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
                var servicio = new InsumoService(unitOfWork);
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
                var servicio = new InsumoService(unitOfWork);
                return Ok(servicio.ObtenerCombo());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}