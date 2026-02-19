// H.Services/AuthService.cs
using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Helpers;
using H.DataAccess.Log;
using H.DataAccess.Repositorios;
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

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            try
            {
                // 1. Validar que el usuario existe
                var usuario = await _authRepository.GetUsuarioByUsername(request.Username);

                if (usuario == null)
                {
                    throw new Exception("Usuario o contraseña incorrectos");
                }

                // 2. Verificar contraseña
                if (!SecurityHelper.VerifyPasswordHash(request.Password, usuario.PasswordHash, usuario.PasswordSalt))
                {
                    throw new Exception("Usuario o contraseña incorrectos");
                }

                // 3. Verificar que el usuario esté activo
                if (!usuario.Estado)
                {
                    throw new Exception("El usuario se encuentra inactivo");
                }

                // 4. Obtener roles
                var roles = await _authRepository.ObtenerRolesPorUsuario(usuario.Id);

                // 5. Obtener tipo de usuario
                var tipoUsuario = usuario.IdTipoUsuario.HasValue ? await _authRepository.ObtenerTipoUsuario(usuario.IdTipoUsuario.Value)
                    : "Cliente";

                // 6. Obtener datos de persona
                var persona = await _authRepository.ObtenerPersonaPorIdUsuario(usuario.Id);

                // 7. Generar token
                var token = GenerarToken(usuario, roles);

                // 8. Preparar respuesta
                var response = new LoginResponseDTO
                {
                    IdUsuario = usuario.Id,
                    Username = usuario.Username,
                    Token = token,
                    TipoUsuario = tipoUsuario ?? "Cliente",
                    Roles = roles,
                    Persona = persona != null ? new PersonaDTO
                    {
                        Id = persona.Id,
                        TipoDocumento = persona.TipoDocumento,
                        NumeroDocumento = persona.NumeroDocumento,
                        Nombres = persona.Nombres,
                        ApellidoPaterno = persona.ApellidoPaterno,
                        ApellidoMaterno = persona.ApellidoMaterno,
                        Telefono = persona.Telefono,
                        Direccion = persona.Direccion,
                        RazonSocial = persona.RazonSocial
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
                // 1. Validar que el username no exista
                if (await _authRepository.ExisteUsername(request.Username))
                {
                    throw new Exception("El nombre de usuario ya existe");
                }

                // 2. Validar que el número de documento no exista
                if (await _authRepository.ExisteNumeroDocumento(request.NumeroDocumento))
                {
                    throw new Exception("El número de documento ya está registrado");
                }

                // 3. Crear hash de contraseña
                SecurityHelper.CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);

                // 4. Preparar entidades
                var usuario = new Usuario
                {
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IdTipoUsuario = 2, // Cliente
                    Estado = true,
                    UsuarioCreacion = request.Username,
                    FechaCreacion = DateTime.Now
                };

                var persona = new Persona
                {
                    TipoDocumento = request.TipoDocumento,
                    NumeroDocumento = request.NumeroDocumento,
                    Nombres = request.Nombres,
                    ApellidoPaterno = request.ApellidoPaterno,
                    ApellidoMaterno = request.ApellidoMaterno,
                    Telefono = request.Telefono,
                    Direccion = request.Direccion,
                    RazonSocial = request.RazonSocial,
                    Estado = true,
                    UsuarioCreacion = request.Username,
                    FechaCreacion = DateTime.Now
                };

                // 5. Registrar en base de datos
                var idUsuario = await _authRepository.RegistrarCliente(usuario, persona);

                // 6. Preparar respuesta
                return new RegisterResponseDTO
                {
                    IdUsuario = idUsuario,
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
            catch
            {
                return false;
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

                var usuarioRegistra = await _authRepository.GetUsuarioByUsername(request.UsuarioRegistra);
                if (usuarioRegistra == null)
                {
                    throw new Exception("Usuario que registra no encontrado");
                }

                if (usuarioRegistra.IdTipoUsuario != 1)
                {
                    throw new Exception("Solo los administradores pueden registrar otros administradores");
                }

                SecurityHelper.CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);

                var usuario = new Usuario
                {
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IdTipoUsuario = 1,
                    Estado = true,
                    UsuarioCreacion = request.UsuarioRegistra,
                    FechaCreacion = DateTime.Now
                };

                var persona = new Persona
                {
                    TipoDocumento = request.TipoDocumento,
                    NumeroDocumento = request.NumeroDocumento,
                    Nombres = request.Nombres,
                    ApellidoPaterno = request.ApellidoPaterno,
                    ApellidoMaterno = request.ApellidoMaterno,
                    Telefono = request.Telefono,
                    Direccion = request.Direccion,
                    Estado = true,
                    UsuarioCreacion = request.UsuarioRegistra,
                    FechaCreacion = DateTime.Now
                };

                var idUsuario = await _authRepository.RegistrarAdministrador(usuario, persona);

                return new RegisterResponseDTO
                {
                    IdUsuario = idUsuario,
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