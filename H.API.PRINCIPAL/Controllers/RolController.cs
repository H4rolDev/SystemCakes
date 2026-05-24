using H.DataAccess.Entidades;
using H.DataAccess.Helpers;
using H.Services;
using H.DataAccess.Extension;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public RolController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("ObtenerListado")]
        public IActionResult ObtenerListado()
        {
            try
            {
                var servicio = new RolService(unitOfWork);
                return Ok(servicio.ObtenerListado());
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
                var servicio = new RolService(unitOfWork);
                return Ok(servicio.ObtenerCombo());
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
                var servicio = new RolService(unitOfWork);
                return Ok(servicio.GetById(id));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("Insertar")]
        public IActionResult Insertar([FromBody] CrearRolRequestDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest(new { success = false, message = "El nombre del rol es requerido" });

                var existeRol = unitOfWork.RolRepository.GetAll()
                    .FirstOrDefault(r => r.Nombre.ToLower() == dto.Nombre.ToLower() && r.Activo);
                if (existeRol != null)
                    return BadRequest(new { success = false, message = "Ya existe un rol con ese nombre" });

                var rol = new Rol
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Activo = true,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy,
                    UsuarioCreacion = dto.UsuarioRegistra
                };

                var rolCreado = unitOfWork.RolRepository.Add(rol);
                unitOfWork.Commit();
                return Ok(new { success = true, message = "Rol creado correctamente", id = rolCreado.Id });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Actualizar")]
        public IActionResult Actualizar([FromBody] ActualizarRolRequestDTO dto)
        {
            try
            {
                if (dto.Id <= 0)
                    return BadRequest(new { success = false, message = "ID de rol inválido" });
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest(new { success = false, message = "El nombre del rol es requerido" });

                var rol = unitOfWork.RolRepository.GetById(dto.Id);
                if (rol == null)
                    return NotFound(new { success = false, message = "Rol no encontrado" });

                var existeOtro = unitOfWork.RolRepository.GetAll()
                    .FirstOrDefault(r => r.Id != dto.Id && r.Nombre.ToLower() == dto.Nombre.ToLower() && r.Activo);
                if (existeOtro != null)
                    return BadRequest(new { success = false, message = "Ya existe otro rol con ese nombre" });

                rol.Nombre = dto.Nombre;
                rol.Descripcion = dto.Descripcion;
                rol.FechaModificacion = Fecha.Hoy;
                rol.UsuarioModificacion = dto.UsuarioModifica;

                unitOfWork.RolRepository.Update(rol);
                unitOfWork.Commit();
                return Ok(new { success = true, message = "Rol actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpDelete("Eliminar")]
        public IActionResult Eliminar(int id, string usuario)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "ID inválido" });
                if (string.IsNullOrWhiteSpace(usuario))
                    return BadRequest(new { success = false, message = "Usuario requerido" });

                var tieneUsuarios = unitOfWork.UsuarioRolRepository.GetAll()
                    .Any(ur => ur.IdRol == id && ur.Activo);
                if (tieneUsuarios)
                    return BadRequest(new { success = false, message = "No se puede eliminar el rol porque tiene usuarios asociados" });

                var rol = unitOfWork.RolRepository.GetById(id);
                if (rol == null)
                    return NotFound(new { success = false, message = "Rol no encontrado" });

                rol.Activo = false;
                rol.FechaModificacion = Fecha.Hoy;
                rol.UsuarioModificacion = usuario;

                unitOfWork.RolRepository.Update(rol);
                unitOfWork.Commit();
                return Ok(new { success = true, message = "Rol eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}