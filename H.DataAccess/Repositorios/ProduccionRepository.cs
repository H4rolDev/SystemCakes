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
using System.Data;
using H.DTOs;
using Dapper;

namespace H.DataAccess.Repositorios
{
    public class ProduccionRepository : GenericRepository<TProduccion>, IProduccionRepository
    {
        private Mapper mapper;
        public ProduccionRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Produccion, TProduccion>(MemberList.None).ReverseMap());
            mapper = new Mapper(config);
        }

        public TProduccion Add(Produccion entidad)
        {
            try
            {
                var modelo = mapper.Map<TProduccion>(entidad);
                base.Add(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public TProduccion Update(Produccion entidad)
        {
            try
            {
                var modelo = mapper.Map<TProduccion>(entidad);
                base.Update(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionRepository" + ex.Message;
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
                error.Message = "ProduccionRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public Produccion GetById(int id)
        {
            try
            {
                var modelo = base.GetById(id);
                return mapper.Map<Produccion>(modelo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public IEnumerable<ProduccionDetalleDTO> ObtenerDetalle(int idProduccion)
        {
            try
            {
                var query = "SP_ObtenerDetalleProduccion";

                using (var conn = connectionFactory.GetConnection)
                {
                    return SqlMapper.Query<ProduccionDetalleDTO>(
                        conn,
                        query,
                        new { IdProduccion = idProduccion },
                        commandType: CommandType.StoredProcedure
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionDetalleInsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(idProduccion);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public IEnumerable<ProduccionCabeceraDTO> ObtenerProducciones()
        {
            try
            {
                var query = "SP_ObtenerProduccionCabecera";

                using (var conn = connectionFactory.GetConnection)
                {
                    return SqlMapper.Query<ProduccionCabeceraDTO>(
                        conn,
                        query,
                        commandType: CommandType.StoredProcedure
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionDetalleInsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }
    }
}
