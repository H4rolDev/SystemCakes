// H.DataAccess/Repositorios/AuthRepository.cs
using Dapper;
using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using H.DataAccess.Infrastructure;
using H.DataAccess.Helpers;

namespace H.DataAccess.Repositorios
{
    public class AuthRepository : IAuthRepository
    {
        private readonly sistemContext _context;
        private readonly IConnectionFactory _connectionFactory;

        public AuthRepository(sistemContext context, IConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        public async Task<Usuario?> GetUsuarioByUsername(string username)
        {
            try
            {
                var usuarioDb = await _context.TUsuario
                    .FirstOrDefaultAsync(u => u.Username == username && u.Activo);
                    
                if (usuarioDb == null) return null;

                return new Usuario
                {
                    Id = usuarioDb.Id,
                    IdPersona = usuarioDb.IdPersona,
                    Username = usuarioDb.Username,
                    PasswordHash = usuarioDb.PasswordHash,
                    PasswordSalt = usuarioDb.PasswordSalt,
                    Activo = usuarioDb.Activo,
                    UsuarioCreacion = usuarioDb.UsuarioCreacion,
                    UsuarioModificacion = usuarioDb.UsuarioModificacion,
                    FechaCreacion = usuarioDb.FechaCreacion,
                    FechaModificacion = usuarioDb.FechaModificacion
                };
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthRepository.GetUsuarioByUsername: " + ex.Message,
                    Exception = ex,
                    Operation = "GetUsuarioByUsername",
                    Code = TiposError.NoEncontrado,
                    Objeto = JsonConvert.SerializeObject(new { username })
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public async Task<bool> ExisteUsername(string username)
        {
            try
            {
                return await _context.TUsuario.AnyAsync(u => u.Username == username);
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
        }

        public async Task<bool> ExisteNumeroDocumento(string numeroDocumento)
        {
            try
            {
                return await _context.TPersona.AnyAsync(p => p.NumeroDocumento == numeroDocumento);
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthRepository.ExisteNumeroDocumento: " + ex.Message,
                    Exception = ex,
                    Operation = "ExisteNumeroDocumento",
                    Code = TiposError.ErrorGeneral,
                    Objeto = JsonConvert.SerializeObject(new { numeroDocumento })
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        /*public async Task<int> RegistrarCliente(Usuario usuario, Persona persona)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Crear Usuario
                    var nuevoUsuario = new TUsuario
                    {
                        //IdTipoUsuario = 2,
                        Username = usuario.Username,
                        PasswordHash = usuario.PasswordHash,
                        PasswordSalt = usuario.PasswordSalt,
                        Activo = true,
                        UsuarioCreacion = usuario.Username,
                        FechaCreacion = Fecha.Hoy,
                        FechaModificacion = Fecha.Hoy
                    };

                    _context.TUsuario.Add(nuevoUsuario);
                    await _context.SaveChangesAsync();

                    // 2. Asignar Rol de Cliente
                    var rolCliente = new TUsuarioRol
                    {
                        IdUsuario = nuevoUsuario.Id,
                        IdRol = 2,
                        Activo = true,
                        UsuarioCreacion = usuario.Username,
                        FechaCreacion = Fecha.Hoy
                    };

                    _context.TUsuarioRol.Add(rolCliente);
                    await _context.SaveChangesAsync();

                    // 3. Crear Persona
                    var nuevaPersona = new TPersona
                    {
                        IdUsuario = nuevoUsuario.Id,
                        TipoDocumento = persona.TipoDocumento,
                        NumeroDocumento = persona.NumeroDocumento,
                        Nombres = persona.Nombres,
                        ApellidoPaterno = persona.ApellidoPaterno,
                        ApellidoMaterno = persona.ApellidoMaterno,
                        Telefono = persona.Telefono,
                        Direccion = persona.Direccion,
                        RazonSocial = persona.RazonSocial,
                        Estado = true,
                        UsuarioCreacion = usuario.Username,
                        FechaCreacion = Fecha.Hoy,
                        FechaModificacion = Fecha.Hoy
                    };

                    _context.TPersona.Add(nuevaPersona);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return nuevoUsuario.Id;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var error = new Error
                    {
                        Message = "AuthRepository.RegistrarCliente: " + ex.Message,
                        Exception = ex,
                        Operation = "RegistrarCliente",
                        Code = TiposError.NoInsertado,
                        Objeto = JsonConvert.SerializeObject(new { usuario, persona })
                    };
                    LogErp.EscribirBaseDatos(error);
                    throw;
                }
            });
        }
        */
        
        public async Task<List<string>> ObtenerRolesPorUsuario(int idUsuario)
        {
            try
            {
                var roles = await (from ur in _context.TUsuarioRol
                                   join r in _context.TRol on ur.IdRol equals r.Id
                                   where ur.IdUsuario == idUsuario && ur.Activo && r.Activo
                                   select r.Nombre).ToListAsync();

                return roles;
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthRepository.ObtenerRolesPorUsuario: " + ex.Message,
                    Exception = ex,
                    Operation = "ObtenerRolesPorUsuario",
                    Code = TiposError.NoEncontrado,
                    Objeto = JsonConvert.SerializeObject(new { idUsuario })
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        /*public async Task<Persona?> ObtenerPersonaPorIdUsuario(int idUsuario)
        {
            try
            {
                var personaDb = await _context.TPersona
                    .FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.Activo);

                if (personaDb == null) return null;

                return new Persona
                {
                    Id = personaDb.Id,
                    IdUsuario = personaDb.IdUsuario,
                    IdTipoDocumento = personaDb.IdTipoDocumento,
                    NumeroDocumento = personaDb.NumeroDocumento,
                    Nombres = personaDb.Nombres,
                    ApellidoPaterno = personaDb.ApellidoPaterno,
                    ApellidoMaterno = personaDb.ApellidoMaterno,
                    Telefono = personaDb.Telefono,
                    Direccion = personaDb.Direccion,
                    RazonSocial = personaDb.RazonSocial
                };
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthRepository.ObtenerPersonaPorIdUsuario: " + ex.Message,
                    Exception = ex,
                    Operation = "ObtenerPersonaPorIdUsuario",
                    Code = TiposError.NoEncontrado,
                    Objeto = JsonConvert.SerializeObject(new { idUsuario })
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }*/

        /*public async Task<string?> ObtenerTipoUsuario(int idTipoUsuario)
        {
            try
            {
                var tipoUsuario = await _context.Set<TTipoUsuario>()
                    .FirstOrDefaultAsync(t => t.Id == idTipoUsuario);

                return tipoUsuario?.Nombre;
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "AuthRepository.ObtenerTipoUsuario: " + ex.Message,
                    Exception = ex,
                    Operation = "ObtenerTipoUsuario",
                    Code = TiposError.NoEncontrado,
                    Objeto = JsonConvert.SerializeObject(new { idTipoUsuario })
                };
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }
*/

        /*public async Task<int> RegistrarAdministrador(Usuario usuario, Persona persona)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Crear Usuario Admin
                    var nuevoUsuario = new TUsuario
                    {
                        IdTipoUsuario = 1, // 1 = Administrador
                        Username = usuario.Username,
                        PasswordHash = usuario.PasswordHash,
                        PasswordSalt = usuario.PasswordSalt,
                        Estado = true,
                        UsuarioCreacion = "SYSTEM",
                        FechaCreacion = Fecha.Hoy,
                        FechaModificacion = Fecha.Hoy
                    };

                    _context.TUsuario.Add(nuevoUsuario);
                    await _context.SaveChangesAsync();

                    // 2. Asignar Rol Administrador
                    var rolAdmin = new TUsuarioRol
                    {
                        IdUsuario = nuevoUsuario.Id,
                        IdRol = 1, // Rol Administrador
                        Estado = true,
                        UsuarioCreacion = "SYSTEM",
                        FechaCreacion = Fecha.Hoy
                    };

                    _context.TUsuarioRol.Add(rolAdmin);
                    await _context.SaveChangesAsync();

                    // 3. Crear Persona
                    var nuevaPersona = new TPersona
                    {
                        IdUsuario = nuevoUsuario.Id,
                        TipoDocumento = persona.TipoDocumento,
                        NumeroDocumento = persona.NumeroDocumento,
                        Nombres = persona.Nombres,
                        ApellidoPaterno = persona.ApellidoPaterno,
                        ApellidoMaterno = persona.ApellidoMaterno,
                        Telefono = persona.Telefono,
                        Direccion = persona.Direccion,
                        RazonSocial = persona.RazonSocial,
                        Estado = true,
                        UsuarioCreacion = "SYSTEM",
                        FechaCreacion = Fecha.Hoy,
                        FechaModificacion = Fecha.Hoy
                    };

                    _context.TPersona.Add(nuevaPersona);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return nuevoUsuario.Id;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var error = new Error
                    {
                        Message = "AuthRepository.RegistrarAdministrador: " + ex.Message,
                        Exception = ex,
                        Operation = "RegistrarAdministrador",
                        Code = TiposError.NoInsertado,
                        Objeto = JsonConvert.SerializeObject(new { usuario, persona })
                    };
                    LogErp.EscribirBaseDatos(error);
                    throw;
                }
            });
        }
*/
    
        /*public async Task<int> RegistrarAdministrador(Usuario usuario, Persona persona)
        {
            // Usar CreateExecutionStrategy para compatibilidad con EnableRetryOnFailure
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Crear Usuario Administrador
                    var nuevoUsuario = new TUsuario
                    {
                        //IdTipoUsuario = 1, // 1 = Administrador
                        Username = usuario.Username,
                        PasswordHash = usuario.PasswordHash,
                        PasswordSalt = usuario.PasswordSalt,
                        Activo = true,
                        UsuarioCreacion = usuario.UsuarioCreacion,
                        FechaCreacion = DateTime.Now,
                        FechaModificacion = DateTime.Now
                    };

                    _context.TUsuario.Add(nuevoUsuario);
                    await _context.SaveChangesAsync();

                    // 2. Asignar Rol de Administrador
                    var rolAdmin = new TUsuarioRol
                    {
                        IdUsuario = nuevoUsuario.Id,
                        IdRol = 1, // Rol Administrador
                        Activo = true,
                        UsuarioCreacion = usuario.UsuarioCreacion,
                        FechaCreacion = DateTime.Now,
                        FechaModificacion = null,
                        UsuarioModificacion = null
                    };

                    _context.TUsuarioRol.Add(rolAdmin);
                    await _context.SaveChangesAsync();

                    // 3. Crear Persona
                    var nuevaPersona = new TPersona
                    {
                        IdUsuario = nuevoUsuario.Id,
                        TipoDocumento = persona.TipoDocumento,
                        NumeroDocumento = persona.NumeroDocumento,
                        Nombres = persona.Nombres,
                        ApellidoPaterno = persona.ApellidoPaterno,
                        ApellidoMaterno = persona.ApellidoMaterno,
                        Telefono = persona.Telefono,
                        Direccion = persona.Direccion,
                        RazonSocial = persona.RazonSocial,
                        Estado = true,
                        UsuarioCreacion = usuario.UsuarioCreacion,
                        FechaCreacion = DateTime.Now,
                        FechaModificacion = DateTime.Now
                    };

                    _context.TPersona.Add(nuevaPersona);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return nuevoUsuario.Id;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var error = new Error
                    {
                        Message = "AuthRepository.RegistrarAdministrador: " + ex.Message,
                        Exception = ex,
                        Operation = "RegistrarAdministrador",
                        Code = TiposError.NoInsertado,
                        Objeto = JsonConvert.SerializeObject(new { usuario, persona })
                    };
                    LogErp.EscribirBaseDatos(error);
                    throw;
                }
            });
        }
    */
    }
}