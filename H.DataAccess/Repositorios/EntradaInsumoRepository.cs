using AutoMapper;
using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Infraestructure;
using H.DataAccess.Models;
using H.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace H.DataAccess.Repositorios
{
    public class EntradaInsumoRepository : GenericRepository<TEntradaInsumo>, IEntradaInsumoRepository
    {
        protected IMapper mapper;
        public EntradaInsumoRepository(sistemContext context, IConnectionFactory connectionFactory)
            : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TEntradaInsumo, TEntradaInsumo>(MemberList.None).ReverseMap());
            mapper = config.CreateMapper();
        }

        public new TEntradaInsumo Add(TEntradaInsumo entidad)
        {
            try
            {
                context.TEntradaInsumo.Add(entidad);
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoRepository - Add: " + ex.Message, ex);
            }
        }

        public new TEntradaInsumo Update(TEntradaInsumo entidad)
        {
            try
            {
                context.TEntradaInsumo.Update(entidad);
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoRepository - Update: " + ex.Message, ex);
            }
        }

        public IEnumerable<EntradaInsumoListadoDTO> ObtenerPendientes()
        {
            try
            {
                using var conn = connectionFactory.GetConnection;
                var query = @"
                    SELECT 
                        e.Id,
                        p.Nombre AS Proveedor,
                        e.NumeroDocumento,
                        e.TipoDocumento,
                        e.FechaDocumento,
                        e.ImagenDocumento,
                        e.Observaciones,
                        e.IdEstado,
                        e.UsuarioCreacion,
                        e.FechaCreacion
                    FROM TEntradaInsumo e
                    LEFT JOIN TProveedor p ON e.IdProveedor = p.Id
                    WHERE e.IdEstado = @IdEstado AND e.Activo = 1
                    ORDER BY e.FechaCreacion DESC";

                var resultados = conn.Query<EntradaInsumoListadoDTO>(query, new { IdEstado = (int)EstadoEntradaInsumoEnum.Pendiente });

                foreach (var item in resultados)
                {
                    item.Estado = "Pendiente";
                    var detallesQuery = @"
                        SELECT 
                            d.Id,
                            d.IdInsumo,
                            i.Nombre AS Insumo,
                            d.Cantidad,
                            d.PrecioUnitario
                        FROM TEntradaInsumoDetalle d
                        INNER JOIN TInsumo i ON d.IdInsumo = i.Id
                        WHERE d.IdEntradaInsumo = @IdEntrada AND d.Activo = 1";
                    item.Detalles = conn.Query<EntradaInsumoDetalleDTO>(detallesQuery, new { IdEntrada = item.Id }).AsList();
                }

                return resultados;
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoRepository - ObtenerPendientes: " + ex.Message, ex);
            }
        }

        public EntradaInsumoListadoDTO ObtenerPorId(int id)
        {
            try
            {
                using var conn = connectionFactory.GetConnection;
                var query = @"
                    SELECT 
                        e.Id,
                        p.Nombre AS Proveedor,
                        e.NumeroDocumento,
                        e.TipoDocumento,
                        e.FechaDocumento,
                        e.ImagenDocumento,
                        e.Observaciones,
                        e.IdEstado,
                        e.UsuarioCreacion,
                        e.FechaCreacion
                    FROM TEntradaInsumo e
                    LEFT JOIN TProveedor p ON e.IdProveedor = p.Id
                    WHERE e.Id = @Id AND e.Activo = 1";

                var item = conn.QueryFirstOrDefault<EntradaInsumoListadoDTO>(query, new { Id = id });

                if (item != null)
                {
                    var detallesQuery = @"
                        SELECT 
                            d.Id,
                            d.IdInsumo,
                            i.Nombre AS Insumo,
                            d.Cantidad,
                            d.PrecioUnitario
                        FROM TEntradaInsumoDetalle d
                        INNER JOIN TInsumo i ON d.IdInsumo = i.Id
                        WHERE d.IdEntradaInsumo = @IdEntrada AND d.Activo = 1";
                    item.Detalles = conn.Query<EntradaInsumoDetalleDTO>(detallesQuery, new { IdEntrada = id }).AsList();
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoRepository - ObtenerPorId: " + ex.Message, ex);
            }
        }

        public EntradaInsumoPaginacionDTO ObtenerTodos(EntradaInsumoFiltroDTO filtro)
        {
            try
            {
                using var conn = connectionFactory.GetConnection;

                var where = "WHERE e.Activo = 1";
                if (filtro.IdEstado.HasValue)
                    where += " AND e.IdEstado = @IdEstado";
                if (filtro.IdProveedor.HasValue)
                    where += " AND e.IdProveedor = @IdProveedor";
                if (!string.IsNullOrWhiteSpace(filtro.NumeroDocumento))
                    where += " AND e.NumeroDocumento LIKE @NumDoc";
                if (filtro.FechaInicio.HasValue)
                    where += " AND e.FechaCreacion >= @FechaInicio";
                if (filtro.FechaFin.HasValue)
                    where += " AND e.FechaCreacion <= @FechaFin";
                if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
                    where += " AND (p.Nombre LIKE @Busqueda OR e.NumeroDocumento LIKE @Busqueda OR e.Observaciones LIKE @Busqueda)";

                var countQuery = @"SELECT COUNT(*) FROM TEntradaInsumo e LEFT JOIN TProveedor p ON e.IdProveedor = p.Id " + where;
                var totalRegistros = conn.ExecuteScalar<int>(countQuery, new
                {
                    filtro.IdEstado,
                    filtro.IdProveedor,
                    NumDoc = filtro.NumeroDocumento != null ? "%" + filtro.NumeroDocumento + "%" : null,
                    filtro.FechaInicio,
                    filtro.FechaFin,
                    Busqueda = filtro.Busqueda != null ? "%" + filtro.Busqueda + "%" : null
                });

                var offset = (filtro.Pagina - 1) * filtro.TamanioPagina;
                var query = $@"
                    SELECT 
                        e.Id,
                        p.Nombre AS Proveedor,
                        e.NumeroDocumento,
                        e.TipoDocumento,
                        e.FechaDocumento,
                        e.ImagenDocumento,
                        e.Observaciones,
                        e.IdEstado,
                        e.UsuarioCreacion,
                        e.FechaCreacion,
                        e.UsuarioAprobacion,
                        e.FechaAprobacion,
                        e.UsuarioRechazo,
                        e.FechaRechazo
                    FROM TEntradaInsumo e
                    LEFT JOIN TProveedor p ON e.IdProveedor = p.Id
                    {where}
                    ORDER BY e.FechaCreacion DESC
                    OFFSET {offset} ROWS FETCH NEXT {filtro.TamanioPagina} ROWS ONLY";

                var resultados = conn.Query<EntradaInsumoListadoDTO>(query, new
                {
                    filtro.IdEstado,
                    filtro.IdProveedor,
                    NumDoc = filtro.NumeroDocumento != null ? "%" + filtro.NumeroDocumento + "%" : null,
                    filtro.FechaInicio,
                    filtro.FechaFin,
                    Busqueda = filtro.Busqueda != null ? "%" + filtro.Busqueda + "%" : null
                });

                foreach (var item in resultados)
                {
                    item.Estado = item.IdEstado switch
                    {
                        1 => "Pendiente",
                        2 => "Aprobado",
                        3 => "Rechazado",
                        _ => "Desconocido"
                    };
                    var detallesQuery = @"
                        SELECT d.Id, d.IdInsumo, i.Nombre AS Insumo, d.Cantidad, d.PrecioUnitario
                        FROM TEntradaInsumoDetalle d
                        INNER JOIN TInsumo i ON d.IdInsumo = i.Id
                        WHERE d.IdEntradaInsumo = @IdEntrada AND d.Activo = 1";
                    item.Detalles = conn.Query<EntradaInsumoDetalleDTO>(detallesQuery, new { IdEntrada = item.Id }).AsList();
                }

                return new EntradaInsumoPaginacionDTO
                {
                    Items = resultados.AsList(),
                    TotalRegistros = totalRegistros,
                    TotalPaginas = (int)Math.Ceiling((double)totalRegistros / filtro.TamanioPagina),
                    PaginaActual = filtro.Pagina
                };
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoRepository - ObtenerTodos: " + ex.Message, ex);
            }
        }

        public IEnumerable<EntradaInsumoHistorialDTO> ObtenerHistorial(int idEntradaInsumo)
        {
            try
            {
                using var conn = connectionFactory.GetConnection;
                var query = @"
                    SELECT Id, IdEntradaInsumo, IdEstadoAnterior, IdEstadoNuevo, Accion, Observacion, Usuario, Fecha
                    FROM TEntradaInsumoHistorial
                    WHERE IdEntradaInsumo = @IdEntradaInsumo
                    ORDER BY Fecha ASC";
                return conn.Query<EntradaInsumoHistorialDTO>(query, new { IdEntradaInsumo = idEntradaInsumo }).AsList();
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoRepository - ObtenerHistorial: " + ex.Message, ex);
            }
        }

        public void AgregarHistorial(EntradaInsumoHistorialDTO historial)
        {
            try
            {
                using var conn = connectionFactory.GetConnection;
                var query = @"
                    INSERT INTO TEntradaInsumoHistorial (IdEntradaInsumo, IdEstadoAnterior, IdEstadoNuevo, Accion, Observacion, Usuario, Fecha)
                    VALUES (@IdEntradaInsumo, @IdEstadoAnterior, @IdEstadoNuevo, @Accion, @Observacion, @Usuario, @Fecha)";
                conn.Execute(query, new
                {
                    historial.IdEntradaInsumo,
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
                throw new Exception("EntradaInsumoRepository - AgregarHistorial: " + ex.Message, ex);
            }
        }
    }
}