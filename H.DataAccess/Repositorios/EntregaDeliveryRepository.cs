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
    public class EntregaDeliveryRepository : GenericRepository<TEntregaDelivery>, IEntregaDeliveryRepository
    {
        private Mapper mapper;
        public EntregaDeliveryRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<EntregaDelivery, TEntregaDelivery>(MemberList.None).ReverseMap());
            mapper = new Mapper(config);
        }

        public TEntregaDelivery Add(EntregaDelivery entidad)
        {
            try
            {
                var modelo = mapper.Map<TEntregaDelivery>(entidad);
                base.Add(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntregaDeliveryRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public TEntregaDelivery Update(EntregaDelivery entidad)
        {
            try
            {
                var modelo = mapper.Map<TEntregaDelivery>(entidad);
                base.Update(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntregaDeliveryRepository" + ex.Message;
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
                error.Message = "EntregaDeliveryRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public EntregaDelivery GetById(int id)
        {
            try
            {
                var modelo = base.GetById(id);
                return mapper.Map<EntregaDelivery>(modelo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntregaDeliveryRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        /*public IEnumerable<EntregaDeliveryListadoDTO> ObtenerCombo()
        {
            try
            {
                var query = "SP_Obtener_ListadoEntregaDelivery_Combo";
                using (var conn = connectionFactory.GetConnection)
                {
                    var rpta = SqlMapper.Query<EntregaDeliveryListadoDTO>(conn, query, param: null, commandType: CommandType.StoredProcedure);
                    return rpta.ToList();
                }
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntregaDeliveryRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerCombo";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }*/
    }
}
