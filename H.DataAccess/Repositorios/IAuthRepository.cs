using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DataAccess.Repositorios;
using H.DTOs;

namespace H.DataAccess.Repositorios
{
    public interface IAuthRepository
    {
        Task<Usuario?> GetUsuarioByUsername(string username);
        Task<bool> ExisteUsername(string username);
        Task<bool> ExisteNumeroDocumento(string numeroDocumento);
        //Task<int> RegistrarCliente(Usuario usuario, Persona persona);
        //Task<int> RegistrarAdministrador(Usuario usuario, Persona persona);
        Task<List<string>> ObtenerRolesPorUsuario(int idUsuario);
        //Task<Persona?> ObtenerPersonaPorIdUsuario(int idUsuario);
        //Task<string?> ObtenerTipoUsuario(int idTipoUsuario);
    }
}
