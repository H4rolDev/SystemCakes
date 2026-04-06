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
    public class ProduccionDetalleInsumoRepository : GenericRepository<TProduccionDetalleInsumo>, IProduccionDetalleInsumoRepository
    {
        private Mapper mapper;
        public ProduccionDetalleInsumoRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProduccionDetalleInsumo, TProduccionDetalleInsumo>(MemberList.None).ReverseMap());
            mapper = new Mapper(config);
        }

        public TProduccionDetalleInsumo Add(ProduccionDetalleInsumo entidad)
        {
            try
            {
                var modelo = mapper.Map<TProduccionDetalleInsumo>(entidad);
                base.Add(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionDetalleInsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public TProduccionDetalleInsumo Update(ProduccionDetalleInsumo entidad)
        {
            try
            {
                var modelo = mapper.Map<TProduccionDetalleInsumo>(entidad);
                base.Update(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionDetalleInsumoRepository" + ex.Message;
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
                error.Message = "ProduccionDetalleInsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public ProduccionDetalleInsumo GetById(int id)
        {
            try
            {
                var modelo = base.GetById(id);
                return mapper.Map<ProduccionDetalleInsumo>(modelo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionDetalleInsumoRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }
    }
}
