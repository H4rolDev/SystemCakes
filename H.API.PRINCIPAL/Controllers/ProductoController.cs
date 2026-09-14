using H.DataAccess.Entidades;
using H.DataAccess.Helpers;
using H.Services;
using H.DataAccess.Extension;
using H.DataAccess.UnitofWork;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public ProductoController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            try
            {
                var productos = unitOfWork.ProductoRepository.GetAll().ToList();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("Insertar")]
        public IActionResult Insert([FromBody] Producto producto)
        {
            try
            {
                var servicio = new ProductoService(unitOfWork);
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
        public IActionResult Update([FromBody] Producto producto)
        {
            try
            {
                var servicio = new ProductoService(unitOfWork);
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
                var servicio = new ProductoService(unitOfWork);
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
                var servicio = new ProductoService(unitOfWork);
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
                var servicio = new ProductoService(unitOfWork);
                return Ok(servicio.ObtenerCombo());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}