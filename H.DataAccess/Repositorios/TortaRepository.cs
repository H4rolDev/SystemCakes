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
using Dapper;
using System.Data;
using H.DTOs;

namespace H.DataAccess.Repositorios
{
    public class TortaRepository : GenericRepository<TTorta>, ITortaRepository
    {
        private Mapper mapper;
        public TortaRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Torta, TTorta>(MemberList.None).ReverseMap());
            mapper = new Mapper(config);
        }

        public TTorta Add(Torta entidad)
        {
            try
            {
                var modelo = mapper.Map<TTorta>(entidad);
                base.Add(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "CtegoriaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public TTorta Update(Torta entidad)
        {
            try
            {
                var modelo = mapper.Map<TTorta>(entidad);
                base.Update(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "CtegoriaRepository" + ex.Message;
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
                error.Message = "CtegoriaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public Torta GetById(int id)
        {
            try
            {
                var modelo = base.GetById(id);
                return mapper.Map<Torta>(modelo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "CtegoriaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

public IEnumerable<TortaListadoDTO> ObtenerCombo()
        {
            try
            {
                var connection = connectionFactory.GetConnection;
                var sql = @"SELECT 
                    t.Id, 
                    t.IdCategoriaTorta,
                    c.Nombre AS NombreCategoriaTorta,
                    ISNULL(t.Nombre, '') AS Nombre, 
                    ISNULL(t.Descripcion, '') AS Descripcion, 
                    ISNULL(t.Cantidades, '') AS Cantidades, 
                    ISNULL(t.PrecioVenta, 0) AS PrecioVenta, 
                    ISNULL(t.ImagenUrl, '') AS ImagenUrl, 
                    ISNULL(t.ImagenPublicId, '') AS ImagenPublicId, 
                    ISNULL(t.EsPersonalizable, 0) AS EsPersonalizable, 
                    t.StockDisponible, 
                    CASE WHEN t.Activo = 1 THEN 'S' ELSE 'N' END AS Activo,
                    t.FechaCreacion, 
                    ISNULL(t.UsuarioCreacion, '') AS UsuarioCreacion, 
                    t.FechaModificacion, 
                    ISNULL(t.UsuarioModificacion, '') AS UsuarioModificacion
                    FROM TTorta t
                    INNER JOIN TCategoriaTorta c ON t.IdCategoriaTorta = c.Id
                    WHERE t.Activo = 1";

                var resultados = connection.Query<TortaListadoDTO>(sql);
                return resultados;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "TortaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerCombo";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }
    }
}
