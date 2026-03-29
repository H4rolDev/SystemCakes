// H.Services/AuthService.cs
using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Helpers;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.Repositorios;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace H.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private IUnitOfWork _unitOfWork;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _unitOfWork = unitOfWork; 
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            try
            {
                var usuario = await _authRepository.GetUsuarioByUsername(request.Username);
                if (usuario == null)
                {
                    throw new Exception("Usuario o contraseña incorrectos");
                }
                if (!SecurityHelper.VerifyPasswordHash(request.Password, usuario.PasswordHash, usuario.PasswordSalt))
                {
                    throw new Exception("Usuario o contraseña incorrectos");
                }
                if (!usuario.Activo)
                {
                    throw new Exception("El usuario se encuentra inactivo");
                }

                var roles = await _authRepository.ObtenerRolesPorUsuario(usuario.Id);

                var usuarioModel = _unitOfWork.UsuarioRepository.GetBy(p => p.Id == usuario.Id).FirstOrDefault();
                var personaModel = _unitOfWork.PersonaRepository.GetBy(p => p.Id == usuarioModel.IdPersona).FirstOrDefault();
                var tipoDocModel = _unitOfWork.TipoDocumentoRepository.GetBy(p => p.Id == personaModel.IdTipoDocumento).FirstOrDefault();

                var token = GenerarToken(usuario, roles);

                var response = new LoginResponseDTO
                {
                    IdUsuario = usuario.Id,
                    Username = usuario.Username,
                    Token = token,
                    Roles = roles,
                    Persona = personaModel != null ? new PersonaDTO
                    {
                        Id = personaModel.Id,
                        TipoDocumento = tipoDocModel.Nombre,
                        NumeroDocumento = personaModel.NumeroDocumento,
                        Nombres = personaModel.Nombres,
                        ApellidoPaterno = personaModel.ApellidoPaterno,
                        ApellidoMaterno = personaModel.ApellidoMaterno,
                        Telefono = personaModel.Telefono,
                        Direccion = personaModel.Direccion,
                        RazonSocial = personaModel.RazonSocial
                    } : null
                };

                return response;
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthService.Login: " + ex.Message,
                    Exception = ex,
                    Operation = "Login",
                    Code = TiposError.ErrorAutenticacion,
                    Objeto = JsonConvert.SerializeObject(new { username = request.Username })
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public async Task<RegisterResponseDTO> RegisterCliente(RegisterClienteRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    throw new Exception("El nombre de usuario es requerido");

                if (string.IsNullOrWhiteSpace(request.Password))
                    throw new Exception("La contraseña es requerida");

                if (request.Password.Length < 6)
                    throw new Exception("La contraseña debe tener al menos 6 caracteres");

                if (string.IsNullOrWhiteSpace(request.NumeroDocumento))
                    throw new Exception("El número de documento es requerido");

                if (string.IsNullOrWhiteSpace(request.Nombres))
                    throw new Exception("Los nombres son requeridos");

                if (string.IsNullOrWhiteSpace(request.ApellidoPaterno))
                    throw new Exception("El apellido paterno es requerido");

                SecurityHelper.CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);

                var persona = new Persona
                {
                    IdTipoDocumento = request.IdTipoDocumento,
                    NumeroDocumento = request.NumeroDocumento,
                    Nombres = request.Nombres,
                    ApellidoPaterno = request.ApellidoPaterno,
                    ApellidoMaterno = request.ApellidoMaterno,
                    Telefono = request.Telefono,
                    Direccion = request.Direccion,
                    RazonSocial = request.RazonSocial,
                    Activo = true,
                    UsuarioCreacion = request.Username,
                    UsuarioModificacion = request.Username,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy
                };
                var personaModelo = _unitOfWork.PersonaRepository.Add(persona);
                _unitOfWork.Commit();

                var usuario = new Usuario
                {
                    IdPersona = personaModelo.Id,
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Activo = true,
                    UsuarioCreacion = request.Username,
                    UsuarioModificacion = request.Username,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy
                };
                var usuarioModelo = _unitOfWork.UsuarioRepository.Add(usuario);
                _unitOfWork.Commit();

                var rolCliente = new TUsuarioRol
                {
                    IdUsuario = usuarioModelo.Id,
                    IdRol = 3,
                    Activo = true,
                    UsuarioCreacion = request.Username,
                    UsuarioModificacion = request.Username,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy
                };
                _unitOfWork.UsuarioRolRepository.Add(rolCliente);
                _unitOfWork.Commit();

                return new RegisterResponseDTO
                {
                    IdUsuario = usuarioModelo.Id,
                    Username = request.Username,
                    Mensaje = "Cliente registrado exitosamente"
                };
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthService.RegisterCliente: " + ex.Message,
                    Exception = ex,
                    Operation = "RegisterCliente",
                    Code = TiposError.NoInsertado,
                    Objeto = JsonConvert.SerializeObject(request)
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public async Task<bool> ValidarToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "TortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(token);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public string GenerarToken(Usuario usuario, List<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
            };

            // Agregar roles como claims
            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<RegisterResponseDTO> RegisterAdministrador(RegisterAdministradorRequestDTO request)
        {
            try
            {
                if (await _authRepository.ExisteUsername(request.Username))
                {
                    throw new Exception("El nombre de usuario ya existe");
                }

                if (await _authRepository.ExisteNumeroDocumento(request.NumeroDocumento))
                {
                    throw new Exception("El número de documento ya está registrado");
                }

                /*var usuarioRegistra = await _authRepository.GetUsuarioByUsername(request.UsuarioRegistra);
                if (usuarioRegistra == null)
                {
                    throw new Exception("Usuario que registra no encontrado");
                }
                
                if (usuarioRegistra.IdTipoUsuario != 1)
                {
                    throw new Exception("Solo los administradores pueden registrar otros administradores");
                }*/

                SecurityHelper.CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);
                var persona = new Persona
                {
                    IdTipoDocumento = request.IdTipoDocumento,
                    NumeroDocumento = request.NumeroDocumento,
                    Nombres = request.Nombres,
                    ApellidoPaterno = request.ApellidoPaterno,
                    ApellidoMaterno = request.ApellidoMaterno,
                    Telefono = request.Telefono,
                    Direccion = request.Direccion,
                    Activo = true,
                    UsuarioCreacion = request.Username,
                    UsuarioModificacion = request.Username,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy
                };
                var modelPersona = _unitOfWork.PersonaRepository.Add(persona);
                _unitOfWork.Commit();

                var usuario = new Usuario
                {
                    IdPersona = modelPersona.Id,
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Activo = true,
                    UsuarioCreacion = request.Username,
                    UsuarioModificacion = request.Username,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy
                };
                var modelUsuario = _unitOfWork.UsuarioRepository.Add(usuario);
                _unitOfWork.Commit();

                var rolAdmin = new TUsuarioRol
                {
                    IdUsuario = modelUsuario.Id,
                    IdRol = 2,
                    Activo = true,
                    UsuarioCreacion = request.Username,
                    FechaCreacion = Fecha.Hoy,
                    FechaModificacion = Fecha.Hoy,
                    UsuarioModificacion = request.Username
                };
                var modelRol = _unitOfWork.UsuarioRolRepository.Add(rolAdmin);
                _unitOfWork.Commit();

                //var idUsuario = await _authRepository.RegistrarAdministrador(usuario, persona);

                return new RegisterResponseDTO
                {
                    IdUsuario = modelUsuario.Id,
                    Username = request.Username,
                    Mensaje = "Administrador registrado exitosamente"
                };
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthService.RegisterAdministrador: " + ex.Message,
                    Exception = ex,
                    Operation = "RegisterAdministrador",
                    Code = TiposError.NoInsertado,
                    Objeto = JsonConvert.SerializeObject(request)
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        /*public async Task<bool> ExisteUsername(string username)
        {
            try
            {
                return await _context.Usuario.AnyAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthRepository.ExisteUsername: " + ex.Message,
                    Exception = ex,
                    Operation = "ExisteUsername",
                    Code = TiposError.ErrorGeneral,
                    Objeto = JsonConvert.SerializeObject(new { username })
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }*/
    }
}