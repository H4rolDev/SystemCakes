using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DTOs;

namespace H.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO request);
        Task<RegisterResponseDTO> RegisterCliente(RegisterClienteRequestDTO request);
        Task<RegisterResponseDTO> RegisterAdministrador(RegisterAdministradorRequestDTO request);
        Task<bool> ValidarToken(string token);
        string GenerarToken(Usuario usuario, List<string> roles);
        Task<bool> ExisteUsername(string username);
        Task<int> CrearUsuarioConRol(TPersona persona, TUsuario usuario, int rolId);
    }
}
