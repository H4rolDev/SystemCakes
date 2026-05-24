using H.DataAccess.Entidades;
using H.DataAccess.Helpers;
using H.DataAccess.Enums;
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
    public class ClienteController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public ClienteController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("ObtenerCombo")]
        public IActionResult ObtenerCombo()
        {
            try
            {
                var servicio = new UsuarioService(unitOfWork);
                return Ok(servicio.ObtenerComboClientes());
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
                var rolClienteId = (int)RolEnum.Cliente;
                var usuarioIds = unitOfWork.UsuarioRolRepository.GetAll()
                    .Where(ur => ur.IdRol == rolClienteId && ur.Activo)
                    .Select(ur => ur.IdUsuario)
                    .ToList();

                var usuarios = unitOfWork.UsuarioRepository.GetAll()
                    .Where(u => usuarioIds.Contains(u.Id) && u.Activo)
                    .ToList();

                var personas = unitOfWork.PersonaRepository.GetAll()
                    .Where(p => p.Activo)
                    .ToList();

                var tipoDocs = unitOfWork.TipoDocumentoRepository.GetAll().ToList();

                var result = (from u in usuarios
                           join p in personas on u.IdPersona equals p.Id
                           join ur in unitOfWork.UsuarioRolRepository.GetAll() on u.Id equals ur.IdUsuario
                           join td in tipoDocs on p.IdTipoDocumento equals td.Id
                           where ur.IdRol == rolClienteId && ur.Activo
                           select new ClienteDTO
                           {
                               IdUsuario = u.Id,
                               IdPersona = p.Id,
                               Username = u.Username,
                               NumeroDocumento = p.NumeroDocumento,
                               Nombres = p.Nombres,
                               ApellidoPaterno = p.ApellidoPaterno,
                               ApellidoMaterno = p.ApellidoMaterno,
                               Telefono = p.Telefono,
                               Direccion = p.Direccion,
                               Activo = u.Activo
                           }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("Insertar")]
        public IActionResult Insertar([FromBody] CrearClienteRequestDTO dto)
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
                this.unitOfWork.Commit();

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
                this.unitOfWork.Commit();

                var usuarioRol = new UsuarioRol
                {
                    IdUsuario = usuarioCreado.Id,
                    IdRol = (int)RolEnum.Cliente,
                    Activo = true,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy,
                    UsuarioCreacion = dto.UsuarioRegistra
                };
                unitOfWork.UsuarioRolRepository.Add(usuarioRol);

                unitOfWork.Commit();
                return Ok(new { success = true, message = "Cliente creado correctamente", id = personaCreada.Id });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Actualizar")]
        public IActionResult Actualizar([FromBody] ActualizarClienteRequestDTO dto)
        {
            try
            {
                if (dto.IdUsuario <= 0)
                    return BadRequest(new { success = false, message = "ID de cliente inválido" });
                if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
                    return BadRequest(new { success = false, message = "El número de documento es requerido" });
                if (string.IsNullOrWhiteSpace(dto.Nombres))
                    return BadRequest(new { success = false, message = "Los nombres son requeridos" });
                if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                    return BadRequest(new { success = false, message = "El apellido paterno es requerido" });

                var persona = unitOfWork.PersonaRepository.GetById(dto.IdPersona);
                if (persona == null)
                    return NotFound(new { success = false, message = "Cliente no encontrado" });

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
                unitOfWork.Commit();
                return Ok(new { success = true, message = "Cliente actualizado correctamente" });
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

                var usuarioObj = unitOfWork.UsuarioRepository.GetById(id);
                if (usuarioObj == null)
                    return NotFound(new { success = false, message = "Cliente no encontrado" });

                usuarioObj.Activo = false;
                usuarioObj.FechaModificacion = Fecha.Hoy;
                usuarioObj.UsuarioModificacion = usuario;

                unitOfWork.UsuarioRepository.Update(usuarioObj);
                unitOfWork.Commit();
                return Ok(new { success = true, message = "Cliente eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}