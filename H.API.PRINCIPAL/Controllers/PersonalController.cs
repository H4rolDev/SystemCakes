using H.DataAccess.Entidades;
using H.DataAccess.Helpers;
using H.Services;
using H.DataAccess.Extension;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Mvc;
using H.DataAccess;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public PersonalController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("ObtenerCombo")]
        public IActionResult ObtenerCombo()
        {
            try
            {
                var servicio = new PersonaService(unitOfWork);
                return Ok(servicio.ObtenerComboPersonal());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerListado")]
        public IActionResult ObtenerListado()
        {
            try
            {
                var servicio = new PersonaService(unitOfWork);
                return Ok(servicio.ObtenerComboPersonal());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerComboDrivers")]
        public IActionResult ObtenerComboDrivers()
        {
            try
            {
                var servicio = new UsuarioService(unitOfWork);
                return Ok(servicio.ObtenerComboDrivers());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("Insertar")]
        public IActionResult Insertar([FromBody] CrearPersonalRequestDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Username))
                    return BadRequest(new { success = false, message = "El username es requerido" });
                if (string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest(new { success = false, message = "La contraseña es requerida" });
                if (dto.Password.Length < 6)
                    return BadRequest(new { success = false, message = "La contraseña debe tener al menos 6 caracteres" });
                if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
                    return BadRequest(new { success = false, message = "El número de documento es requerido" });
                if (string.IsNullOrWhiteSpace(dto.Nombres))
                    return BadRequest(new { success = false, message = "Los nombres son requeridos" });
                if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                    return BadRequest(new { success = false, message = "El apellido paterno es requerido" });
                if (dto.IdRol <= 0)
                    return BadRequest(new { success = false, message = "Debe seleccionar un rol" });

                var existeUsuario = unitOfWork.UsuarioRepository.GetAll()
                    .FirstOrDefault(u => u.Username == dto.Username && u.Activo);
                if (existeUsuario != null)
                    return BadRequest(new { success = false, message = "El username ya está en uso" });

                var persona = new Persona
                {
                    IdTipoDocumento = dto.IdTipoDocumento,
                    NumeroDocumento = dto.NumeroDocumento,
                    Nombres = dto.Nombres,
                    ApellidoPaterno = dto.ApellidoPaterno,
                    ApellidoMaterno = dto.ApellidoMaterno,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion,
                    Activo = true,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy,
                    UsuarioCreacion = dto.UsuarioRegistra
                };
                var personaCreada = unitOfWork.PersonaRepository.Add(persona);
                unitOfWork.Commit();

                SecurityHelper.CreatePasswordHash(dto.Password, out string passwordHash, out string passwordSalt);

                var usuario = new Usuario
                {
                    IdPersona = personaCreada.Id,
                    Username = dto.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Activo = true,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy,
                    UsuarioCreacion = dto.UsuarioRegistra
                };
                var usuarioCreado = unitOfWork.UsuarioRepository.Add(usuario);
                unitOfWork.Commit();

                var usuarioRol = new UsuarioRol
                {
                    IdUsuario = usuarioCreado.Id,
                    IdRol = dto.IdRol,
                    Activo = true,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy,
                    UsuarioCreacion = dto.UsuarioRegistra
                };
                unitOfWork.UsuarioRolRepository.Add(usuarioRol);

                unitOfWork.Commit();
                return Ok(new { success = true, message = "Personal creado correctamente", id = personaCreada.Id });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Actualizar")]
        public IActionResult Actualizar([FromBody] ActualizarPersonalRequestDTO dto)
        {
            try
            {
                if (dto.Id <= 0)
                    return BadRequest(new { success = false, message = "ID de personal inválido" });
                if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
                    return BadRequest(new { success = false, message = "El número de documento es requerido" });
                if (string.IsNullOrWhiteSpace(dto.Nombres))
                    return BadRequest(new { success = false, message = "Los nombres son requeridos" });
                if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                    return BadRequest(new { success = false, message = "El apellido paterno es requerido" });

                var persona = unitOfWork.PersonaRepository.GetById(dto.Id);
                if (persona == null)
                    return NotFound(new { success = false, message = "Personal no encontrado" });

                persona.IdTipoDocumento = dto.IdTipoDocumento;
                persona.NumeroDocumento = dto.NumeroDocumento;
                persona.Nombres = dto.Nombres;
                persona.ApellidoPaterno = dto.ApellidoPaterno;
                persona.ApellidoMaterno = dto.ApellidoMaterno;
                persona.Telefono = dto.Telefono;
                persona.Direccion = dto.Direccion;
                persona.FechaModificacion = Fecha.Hoy;
                persona.UsuarioModificacion = dto.UsuarioModifica;

                unitOfWork.PersonaRepository.Update(persona);

                if (dto.IdRol.HasValue && dto.IdRol > 0)
                {
                    var usuario = unitOfWork.UsuarioRepository.GetAll()
                        .FirstOrDefault(u => u.IdPersona == dto.Id && u.Activo);
                    if (usuario != null)
                    {
                        var usuarioRol = unitOfWork.UsuarioRolRepository.GetAll()
                            .FirstOrDefault(ur => ur.IdUsuario == usuario.Id && ur.Activo);
                        if (usuarioRol != null)
                        {
                            usuarioRol.IdRol = dto.IdRol.Value;
                            usuarioRol.FechaModificacion = Fecha.Hoy;
                            usuarioRol.UsuarioModificacion = dto.UsuarioModifica;
                            unitOfWork.UsuarioRolRepository.Update(usuarioRol);
                        }
                    }
                }

                unitOfWork.Commit();
                return Ok(new { success = true, message = "Personal actualizado correctamente" });
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

                var persona = unitOfWork.PersonaRepository.GetById(id);
                if (persona == null)
                    return NotFound(new { success = false, message = "Personal no encontrado" });

                persona.Activo = false;
                persona.FechaModificacion = Fecha.Hoy;
                persona.UsuarioModificacion = usuario;

                unitOfWork.PersonaRepository.Update(persona);
                unitOfWork.Commit();
                return Ok(new { success = true, message = "Personal eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}