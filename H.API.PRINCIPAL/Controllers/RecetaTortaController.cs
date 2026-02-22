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
    public class RecetaTortaController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public RecetaTortaController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("Insertar")]
        public IActionResult Insert([FromBody] RecetaTorta categoria)
        {
            try
            {
                var servicio = new RecetaTortaService(unitOfWork);
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
        public IActionResult Update([FromBody] RecetaTorta categoria)
        {
            try
            {
                var servicio = new RecetaTortaService(unitOfWork);
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
                var servicio = new RecetaTortaService(unitOfWork);
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
                var servicio = new RecetaTortaService(unitOfWork);
                return Ok(servicio.GetById(id));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        /*[HttpGet("ObtenerCombo")]
        public IActionResult ObtenerCombo()
        {
            try
            {
                var servicio = new RecetaTortaService(unitOfWork);
                return Ok(servicio.ObtenerCombo());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }*/

        [HttpPost("InsertarMultipleTabla")]
        public IActionResult InsertarMultipleTabla(InsertarMultipleRecetaTortaDTO dto)
        {
            try
            {
                var servicio = new RecetaTortaService(unitOfWork);
                var respuesta = servicio.AddMultipleTabla(dto);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}