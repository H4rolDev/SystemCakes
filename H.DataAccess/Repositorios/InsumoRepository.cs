using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DataAccess.Enums;
using H.DataAccess.Infrastructure;
using H.DataAccess.Log;
using H.DataAccess.Repositorios;
using Newtonsoft.Json;
using Dapper;
using System.Data;
using H.DTOs;

namespace H.DataAccess.Repositorios
{
    public class InsumoRepository : GenericRepository<TInsumo>, IInsumoRepository
    {
        private Mapper mapper;
        public InsumoRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Insumo, TInsumo>(MemberList.None).ReverseMap());
            mapper = new Mapper(config);
        }

        public TInsumo Add(Insumo entidad)
        {
            try
            {
                var modelo = mapper.Map<TInsumo>(entidad);
                base.Add(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public TInsumo Update(Insumo entidad)
        {
            try
            {
                var modelo = mapper.Map<TInsumo>(entidad);
                base.Update(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoRepository" + ex.Message;
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
                error.Message = "InsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public Insumo GetById(int id)
        {
            try
            {
                var modelo = base.GetById(id);
                return mapper.Map<Insumo>(modelo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public IEnumerable<InsumoListadoDTO> ObtenerCombo()
        {
            try
            {
                var query = "SP_Insumo_ListadoActivo_Combo";
                using (var conn = connectionFactory.GetConnection)
                {
                    var rpta = SqlMapper.Query<InsumoListadoDTO>(conn, query, param: null, commandType: CommandType.StoredProcedure);
                    return rpta.ToList();
                }
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerCombo";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }
    }
}
