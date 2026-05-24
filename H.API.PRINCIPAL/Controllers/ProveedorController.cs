using H.DataAccess.Extension;
using H.DataAccess.Helpers;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Mvc;
using H.DataAccess.Entidades;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProveedorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("ObtenerCombo")]
        public IActionResult ObtenerCombo()
        {
            try
            {
                var lista = _unitOfWork.ProveedorRepository.ObtenerCombo();
                return Ok(lista);
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
                var proveedor = _unitOfWork.ProveedorRepository.GetById(id);
                if (proveedor == null)
                    return NotFound(new { success = false, message = "Proveedor no encontrado" });
                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost]
        public IActionResult Crear([FromBody] ProveedorRequestDTO dto)
        {
            try
            {
                var entidad = new Proveedor
                {
                    Nombre = dto.Nombre,
                    Ruc = dto.Ruc,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion,
                    Contacto = dto.Contacto
                };
                var resultado = _unitOfWork.ProveedorRepository.Add(entidad);
                return Ok(new { success = true, id = resultado.Id });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut]
        public IActionResult Actualizar([FromBody] ProveedorRequestDTO dto)
        {
            try
            {
                var entidad = new Proveedor
                {
                    Id = dto.Id,
                    Nombre = dto.Nombre,
                    Ruc = dto.Ruc,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion,
                    Contacto = dto.Contacto
                };
                _unitOfWork.ProveedorRepository.Update(entidad);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Desactivar/{id}")]
        public IActionResult Desactivar(int id)
        {
            try
            {
                _unitOfWork.ProveedorRepository.Desactivar(id);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Activar/{id}")]
        public IActionResult Activar(int id)
        {
            try
            {
                _unitOfWork.ProveedorRepository.Activar(id);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}