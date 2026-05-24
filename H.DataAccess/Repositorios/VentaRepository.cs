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
    public class VentaRepository : GenericRepository<TVenta>, IVentaRepository
    {
        private Mapper mapper;
        public VentaRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Venta, TVenta>(MemberList.None).ReverseMap());
            mapper = new Mapper(config);
        }

        public TVenta Add(Venta entidad)
        {
            try
            {
                var modelo = mapper.Map<TVenta>(entidad);
                base.Add(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public TVenta Update(Venta entidad)
        {
            try
            {
                var modelo = mapper.Map<TVenta>(entidad);
                base.Update(modelo);
                return modelo;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository" + ex.Message;
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
                error.Message = "VentaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public Venta GetById(int id)
        {
            try
            {
                var modelo = base.GetById(id);
                return mapper.Map<Venta>(modelo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "GetById";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public IEnumerable<ComboDTO> ObtenerComboMetodoPago()
        {
            try
            {
                return context.TMetodoPago
                    .Where(x => x.Activo)
                    .Select(x => new ComboDTO { Id = x.Id, Nombre = x.Nombre })
                    .ToList();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerComboMetodoPago";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<ComboDTO> ObtenerComboTipoComprobante()
        {
            try
            {
                return new List<ComboDTO>
                {
                    new ComboDTO { Id = 1, Nombre = "Boleta" },
                    new ComboDTO { Id = 2, Nombre = "Factura" }
                };
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerComboTipoComprobante";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<ComboDTO> ObtenerComboPersonal()
        {
            try
            {
                return context.TPersona
                    .Where(x => x.Activo)
                    .Select(x => new ComboDTO { Id = x.Id, Nombre = x.ApellidoPaterno + " " + x.Nombres })
                    .ToList();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerComboPersonal";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public VentaPaginacionDTO ObtenerTodos(VentaFiltroDTO filtro)
        {
            try
            {
                using var conn = connectionFactory.GetConnection;

                var where = "WHERE v.Activo = 1";
                if (filtro.IdEstado.HasValue)
                    where += " AND v.IdEstadoVenta = @IdEstado";
                if (filtro.IdPersona.HasValue)
                    where += " AND v.IdPersona = @IdPersona";
                if (!string.IsNullOrWhiteSpace(filtro.NumeroOperacion))
                    where += " AND v.NumeroOperacion LIKE @NumeroOp";
                if (filtro.FechaInicio.HasValue)
                    where += " AND v.FechaVenta >= @FechaInicio";
                if (filtro.FechaFin.HasValue)
                    where += " AND v.FechaVenta <= @FechaFin";
                if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
                    where += " AND (p.Nombres LIKE @Busqueda OR p.ApellidoPaterno LIKE @Busqueda OR v.NumeroOperacion LIKE @Busqueda)";

                var countQuery = @"SELECT COUNT(*) FROM TVenta v INNER JOIN TPersona p ON v.IdPersona = p.Id " + where;
                var totalRegistros = conn.ExecuteScalar<int>(countQuery, new
                {
                    filtro.IdEstado,
                    filtro.IdPersona,
                    NumeroOp = filtro.NumeroOperacion != null ? "%" + filtro.NumeroOperacion + "%" : null,
                    filtro.FechaInicio,
                    filtro.FechaFin,
                    Busqueda = filtro.Busqueda != null ? "%" + filtro.Busqueda + "%" : null
                });

                var offset = (filtro.Pagina - 1) * filtro.TamanioPagina;
                var query = $@"
                    SELECT 
                        v.Id,
                        v.FechaVenta,
                        p.Nombres + ' ' + p.ApellidoPaterno AS Cliente,
                        v.Total,
                        v.NumeroOperacion,
                        v.IdEstadoVenta,
                        e.Nombre AS Estado
                    FROM TVenta v
                    INNER JOIN TPersona p ON v.IdPersona = p.Id
                    INNER JOIN TEstadoVenta e ON v.IdEstadoVenta = e.Id
                    {where}
                    ORDER BY v.FechaVenta DESC
                    OFFSET {offset} ROWS FETCH NEXT {filtro.TamanioPagina} ROWS ONLY";

                var resultados = conn.Query<VentaListadoDTO>(query, new
                {
                    filtro.IdEstado,
                    filtro.IdPersona,
                    NumeroOp = filtro.NumeroOperacion != null ? "%" + filtro.NumeroOperacion + "%" : null,
                    filtro.FechaInicio,
                    filtro.FechaFin,
                    Busqueda = filtro.Busqueda != null ? "%" + filtro.Busqueda + "%" : null
                });

                return new VentaPaginacionDTO
                {
                    Items = resultados.AsList(),
                    TotalRegistros = totalRegistros,
                    TotalPaginas = (int)Math.Ceiling((double)totalRegistros / filtro.TamanioPagina),
                    PaginaActual = filtro.Pagina
                };
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository - ObtenerTodos: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerTodos";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<VentaHistorialDTO> ObtenerHistorial(int idVenta)
        {
            try
            {
                using var conn = connectionFactory.GetConnection;
                var query = @"
                    SELECT Id, IdVenta, IdEstadoAnterior, IdEstadoNuevo, Accion, Observacion, Usuario, Fecha
                    FROM TVentaHistorial
                    WHERE IdVenta = @IdVenta
                    ORDER BY Fecha ASC";
                return conn.Query<VentaHistorialDTO>(query, new { IdVenta = idVenta }).AsList();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository - ObtenerHistorial: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerHistorial";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public void AgregarHistorial(VentaHistorialDTO historial)
        {
            try
            {
                using var conn = connectionFactory.GetConnection;
                var query = @"
                    INSERT INTO TVentaHistorial (IdVenta, IdEstadoAnterior, IdEstadoNuevo, Accion, Observacion, Usuario, Fecha)
                    VALUES (@IdVenta, @IdEstadoAnterior, @IdEstadoNuevo, @Accion, @Observacion, @Usuario, @Fecha)";
                conn.Execute(query, new
                {
                    historial.IdVenta,
                    historial.IdEstadoAnterior,
                    historial.IdEstadoNuevo,
                    historial.Accion,
                    historial.Observacion,
                    historial.Usuario,
                    Fecha = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaRepository - AgregarHistorial: " + ex.Message;
                error.Exception = ex;
                error.Operation = "AgregarHistorial";
                error.Code = TiposError.NoInsertado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }
    }
}
