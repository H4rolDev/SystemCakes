using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DataAccess.Enums;
using H.DataAccess.Infraestructure;
using H.DataAccess.Log;
using H.DataAccess.Repositorios;
using Newtonsoft.Json;
using System.Data;
using H.DTOs;
using Dapper;

namespace H.DataAccess.Repositorios
{
    public class PersonaRepository : GenericRepository<TPersona>, IPersonaRepository
    {
        private Mapper mapper;
        public PersonaRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Persona, TPersona>(MemberList.None).ReverseMap());
            mapper = new Mapper(config);
        }

        public TPersona Add(Persona entidad)
        {
            try
            {
                var modelo = mapper.Map<TPersona>(entidad);
                base.Add(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "PersonaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public TPersona Update(Persona entidad)
        {
            try
            {
                var modelo = mapper.Map<TPersona>(entidad);
                base.Update(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "PersonaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoActualizado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public int Delete(int id, string usuario)
        {
            try
            {
                base.Delete(id, usuario);
                return id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "PersonaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public Persona GetById(int id)
        {
            try
            {
                var modelo = base.GetById(id);
                return mapper.Map<Persona>(modelo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "PersonaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public IEnumerable<PersonaListadoDTO> ObtenerCombo()
        {
            try
            {
                return context.TPersona
                    .Where(x => x.Activo)
                    .Select(x => new PersonaListadoDTO
                    {
                        Id = x.Id,
                        Nombres = x.Nombres,
                        ApellidoPaterno = x.ApellidoPaterno,
                        ApellidoMaterno = x.ApellidoMaterno,
                        NumeroDocumento = x.NumeroDocumento
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "PersonaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerCombo";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<PersonalDTO> ObtenerComboPersonal()
        {
            try
            {
                var clientes = context.TUsuarioRol
                    .Where(ur => ur.IdRol == (int)RolEnum.Cliente)
                    .Select(ur => ur.IdUsuario)
                    .ToList();

                var usuariosNoCliente = context.TUsuario
                    .Where(u => u.Activo && !clientes.Contains(u.Id))
                    .Select(u => u.IdPersona)
                    .ToList();

                var result = from p in context.TPersona
                             join u in context.TUsuario on p.Id equals u.IdPersona
                             join ur in context.TUsuarioRol on u.Id equals ur.IdUsuario
                             join r in context.TRol on ur.IdRol equals r.Id
                             where p.Activo && u.Activo && ur.Activo && usuariosNoCliente.Contains(p.Id)
                             orderby p.Nombres
                             select new PersonalDTO
                             {
                                 Id = p.Id,
                                 Username = u.Username,
                                 IdRol = r.Id,
                                 NombreRol = r.Nombre,
                                 IdTipoDocumento = p.IdTipoDocumento,
                                 TipoDocumento = "",
                                 NumeroDocumento = p.NumeroDocumento,
                                 Nombres = p.Nombres,
                                 ApellidoPaterno = p.ApellidoPaterno,
                                 ApellidoMaterno = p.ApellidoMaterno,
                                 Telefono = p.Telefono,
                                 Direccion = p.Direccion,
                                 Activo = p.Activo
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "PersonaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerComboPersonal";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }
    }
}
