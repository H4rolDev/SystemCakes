// H.API.PRINCIPAL/Controllers/AuthController.cs
using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DataAccess.Extension;
using H.DataAccess.Helpers;
using H.DTOs;
using H.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login para clientes y administrativos
        /// </summary>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { mensaje = "Usuario y contraseña son requeridos" });
                }

                var response = await _authService.Login(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        /// <summary>
        /// Registro de nuevos clientes
        /// </summary>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterClienteRequestDTO request)
        {
            try
            {
                var response = await _authService.RegisterCliente(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        /// <summary>
        /// Validar token de autenticación
        /// </summary>
        [HttpPost("ValidarToken")]
        [Authorize]
        public async Task<IActionResult> ValidarToken()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var esValido = await _authService.ValidarToken(token);

                return Ok(new { valido = esValido });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        /// <summary>
        /// Logout (invalidar token del lado del cliente)
        /// </summary>
        [HttpPost("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                return Ok(new { mensaje = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        /// <summary>
        /// Obtener información del usuario autenticado
        /// </summary>
        [HttpGet("UsuarioActual")]
        [Authorize]
        public IActionResult ObtenerUsuarioActual()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

                return Ok(new
                {
                    idUsuario = userId,
                    username = username,
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        /// <summary>
        /// Registro de nuevos administradores (solo admin puede registrar)
        /// </summary>
        [HttpPost("RegisterAdministrador")]
        //[Authorize(Roles = "Administrador")] // ← Solo admins pueden acceder
        public async Task<IActionResult> RegisterAdministrador([FromBody] RegisterAdministradorRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    return BadRequest(new { mensaje = "El nombre de usuario es requerido" });

                if (string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { mensaje = "La contraseña es requerida" });

                if (request.Password.Length < 4)
                    return BadRequest(new { mensaje = "La contraseña debe tener al menos 4 caracteres" });

                if (string.IsNullOrWhiteSpace(request.NumeroDocumento))
                    return BadRequest(new { mensaje = "El número de documento es requerido" });

                if (string.IsNullOrWhiteSpace(request.Nombres))
                    return BadRequest(new { mensaje = "Los nombres son requeridos" });

                if (string.IsNullOrWhiteSpace(request.ApellidoPaterno))
                    return BadRequest(new { mensaje = "El apellido paterno es requerido" });

                if (string.IsNullOrWhiteSpace(request.UsuarioRegistra))
                    return BadRequest(new { mensaje = "El usuario que registra es requerido" });

                var usernameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                var response = await _authService.RegisterAdministrador(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}