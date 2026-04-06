using H.DataAccess.Entidades;
using H.DataAccess.Helpers;
using H.Services;
using H.DataAccess.Extension;
using H.DataAccess.UnitofWork;
using Microsoft.AspNetCore.Mvc;
using H.DTOs;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduccionController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public ProduccionController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("Insertar")]
        public IActionResult Insert([FromBody] Produccion categoria)
        {
            try
            {
                var servicio = new ProduccionService(unitOfWork);
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
        public IActionResult Update([FromBody] Produccion categoria)
        {
            try
            {
                var servicio = new ProduccionService(unitOfWork);
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
                var servicio = new ProduccionService(unitOfWork);
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
                var servicio = new ProduccionService(unitOfWork);
                return Ok(servicio.GetById(id));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("InsertarMultipleTabla")]
        public IActionResult InsertarMultipleTabla(InsertarProduccionDTO dto)
        {
            try
            {
            
                var servicio = new ProduccionService(unitOfWork);
                var respuesta = servicio.AddMultipleTabla(dto);
                return Ok(respuesta);
            }
            catch (Exception ex) { 
                return new ErrorResult(ex, User);
            }
        }

        /*[HttpPut("ActualizarProduccion")]
        public IActionResult ActualizarProduccion(UpdateProduccionDTO dto)
        {
            try
            {
                var servicio = new ProduccionService(unitOfWork);
                var respuesta = servicio.UpdateProduccion(dto);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }*/

        [HttpPost("AjustarInsumo")]
        public IActionResult AjustarInsumo(AjusteInsumoDTO dto)
        {
            try
            {
                var servicio = new ProduccionService(unitOfWork);
                var rpta = servicio.AjustarInsumo(dto);
                return Ok(rpta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("AjustarTorta")]
        public IActionResult AjustarTorta(AjusteTortaDTO dto)
        {
            try
            {
                var servicio = new ProduccionService(unitOfWork);
                var rpta = servicio.AjustarTorta(dto);
                return Ok(rpta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerProducciones")]
        public IActionResult ObtenerProducciones()
        {
            try
            {
                var servicio = new ProduccionService(unitOfWork);
                return Ok(servicio.ObtenerProducciones());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerDetalleProduccion")]
        public IActionResult ObtenerDetalleProduccion(int id)
        {
            try
            {
                var servicio = new ProduccionService(unitOfWork);
                return Ok(servicio.ObtenerDetalleProduccion(id));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}