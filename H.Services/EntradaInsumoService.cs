using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Services
{
    public interface IEntradaInsumoService
    {
        int Registrar(EntradaInsumoRequestDTO dto, string? imagenUrl = null);
        IEnumerable<EntradaInsumoListadoDTO> ObtenerPendientes();
        EntradaInsumoListadoDTO ObtenerPorId(int id);
        bool Aprobar(EntradaInsumoAprobarDTO dto);
        bool Rechazar(EntradaInsumoRechazarDTO dto);
        EntradaInsumoPaginacionDTO ObtenerTodos(EntradaInsumoFiltroDTO filtro);
        IEnumerable<EntradaInsumoHistorialDTO> ObtenerHistorial(int id);
    }

    public class EntradaInsumoService : IEntradaInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EntradaInsumoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Registrar(EntradaInsumoRequestDTO dto, string? imagenUrl = null)
        {
            try
            {
                if (dto == null)
                    throw new Exception("El objeto no puede ser nulo.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    throw new Exception("Usuario requerido.");

                if (dto.Detalles == null || !dto.Detalles.Any())
                    throw new Exception("Debe agregar al menos un insumo.");

                var fechaActual = DateTime.Now;

                var entrada = new TEntradaInsumo
                {
                    IdProveedor = dto.IdProveedor,
                    NumeroDocumento = dto.NumeroDocumento?.Trim(),
                    TipoDocumento = dto.TipoDocumento?.Trim(),
                    FechaDocumento = dto.FechaDocumento,
                    ImagenDocumento = imagenUrl,
                    Observaciones = dto.Observaciones?.Trim(),
                    IdEstado = (int)EstadoEntradaInsumoEnum.Pendiente,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    UsuarioModificacion = dto.Usuario,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                var entradaGuardada = _unitOfWork.EntradaInsumoRepository.Add(entrada);
                _unitOfWork.Commit();

                var idEntrada = entradaGuardada.Id;
                if (idEntrada <= 0)
                    throw new Exception("No se pudo registrar la entrada de insumos.");

                foreach (var det in dto.Detalles)
                {
                    var detalle = new TEntradaInsumoDetalle
                    {
                        IdEntradaInsumo = idEntrada,
                        IdInsumo = det.IdInsumo,
                        Cantidad = det.Cantidad,
                        PrecioUnitario = det.PrecioUnitario,
                        FechaVencimiento = det.FechaVencimiento,
                        Activo = true,
                        UsuarioCreacion = dto.Usuario,
                        UsuarioModificacion = dto.Usuario,
                        FechaCreacion = fechaActual,
                        FechaModificacion = fechaActual
                    };
                    _unitOfWork.EntradaInsumoDetalleRepository.Add(detalle);
                }

                _unitOfWork.Commit();

                _unitOfWork.EntradaInsumoRepository.AgregarHistorial(new EntradaInsumoHistorialDTO
                {
                    IdEntradaInsumo = idEntrada,
                    IdEstadoAnterior = null,
                    IdEstadoNuevo = (int)EstadoEntradaInsumoEnum.Pendiente,
                    Accion = "Registrado",
                    Observacion = $"Entrada creada con {dto.Detalles.Count} insumos",
                    Usuario = dto.Usuario
                });
                _unitOfWork.Commit();

                return idEntrada;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntradaInsumoService - Registrar: " + ex.Message;
                error.Exception = ex;
                error.Operation = "Registrar";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(dto);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<EntradaInsumoListadoDTO> ObtenerPendientes()
        {
            try
            {
                return _unitOfWork.EntradaInsumoRepository.ObtenerPendientes();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntradaInsumoService - ObtenerPendientes: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerPendientes";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public EntradaInsumoListadoDTO ObtenerPorId(int id)
        {
            try
            {
                return _unitOfWork.EntradaInsumoRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntradaInsumoService - ObtenerPorId: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerPorId";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public bool Aprobar(EntradaInsumoAprobarDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new Exception("El objeto no puede ser nulo.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    throw new Exception("Usuario requerido.");

                var rolValido = !string.IsNullOrWhiteSpace(dto.Rol) && 
                                dto.Rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
                if (!rolValido)
                    throw new Exception("Solo el Administrador puede aprobar entradas de insumos.");

                var entrada = _unitOfWork.EntradaInsumoRepository.GetById(dto.Id);
                if (entrada == null)
                    throw new Exception("Entrada no encontrada.");

                if (entrada.IdEstado != (int)EstadoEntradaInsumoEnum.Pendiente)
                    throw new Exception("La entrada ya fue procesada.");

                var fechaActual = DateTime.Now;

                var detalles = _unitOfWork.EntradaInsumoRepository.ObtenerPorId(dto.Id)?.Detalles;
                if (detalles == null || !detalles.Any())
                    throw new Exception("No hay detalles para procesar.");

                foreach (var det in detalles)
                {
                    var insumo = _unitOfWork.InsumoRepository.GetById(det.IdInsumo);
                    if (insumo == null)
                        throw new Exception($"Insumo con ID {det.IdInsumo} no encontrado.");

                    var lote = new TInsumoLote
                    {
                        IdInsumo = det.IdInsumo,
                        NumeroLote = null,
                        FechaIngreso = fechaActual,
                        FechaVencimiento = det.FechaVencimiento,
                        CantidadInicial = det.Cantidad,
                        CantidadDisponible = det.Cantidad,
                        CostoUnitario = det.PrecioUnitario,
                        Activo = true,
                        UsuarioCreacion = dto.Usuario,
                        UsuarioModificacion = dto.Usuario,
                        FechaCreacion = fechaActual,
                        FechaModificacion = fechaActual
                    };

                    _unitOfWork.InsumoLoteRepository.Add(lote);
                    _unitOfWork.Commit();

                    lote.NumeroLote = $"LOT-{lote.Id:D6}";
                    _unitOfWork.InsumoLoteRepository.Update(lote);

                    insumo.StockActual = (insumo.StockActual ?? 0) + det.Cantidad;
                    _unitOfWork.InsumoRepository.Update(insumo);

                    var movimiento = new TMovimientoInsumo
                    {
                        IdTipoMovimiento = (int)TipoMovimientoEnum.Entrada,
                        IdInsumoLote = lote.Id,
                        Cantidad = det.Cantidad,
                        FechaMovimiento = fechaActual,
                        Referencia = $"Entrada #{entrada.Id} - {entrada.NumeroDocumento ?? "Sin documento"}",
                        Activo = true,
                        UsuarioCreacion = dto.Usuario,
                        UsuarioModificacion = dto.Usuario,
                        FechaCreacion = fechaActual,
                        FechaModificacion = fechaActual
                    };
                    _unitOfWork.MovimientoInsumoRepository.Add(movimiento);
                }

                entrada.IdEstado = (int)EstadoEntradaInsumoEnum.Aprobado;
                entrada.UsuarioAprobacion = dto.Usuario;
                entrada.FechaAprobacion = fechaActual;
                entrada.UsuarioModificacion = dto.Usuario;
                entrada.FechaModificacion = fechaActual;
                _unitOfWork.EntradaInsumoRepository.Update(entrada);

                _unitOfWork.Commit();

                _unitOfWork.EntradaInsumoRepository.AgregarHistorial(new EntradaInsumoHistorialDTO
                {
                    IdEntradaInsumo = dto.Id,
                    IdEstadoAnterior = (int)EstadoEntradaInsumoEnum.Pendiente,
                    IdEstadoNuevo = (int)EstadoEntradaInsumoEnum.Aprobado,
                    Accion = "Aprobado",
                    Observacion = $"Lotes generados: {string.Join(", ", detalles.Select(d => $"LOT-{d.IdInsumo}"))}",
                    Usuario = dto.Usuario
                });
                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntradaInsumoService - Aprobar: " + ex.Message;
                error.Exception = ex;
                error.Operation = "Aprobar";
                error.Code = TiposError.NoActualizado;
                error.Objeto = JsonConvert.SerializeObject(dto);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public bool Rechazar(EntradaInsumoRechazarDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new Exception("El objeto no puede ser nulo.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    throw new Exception("Usuario requerido.");

                if (string.IsNullOrWhiteSpace(dto.Motivo))
                    throw new Exception("El motivo de rechazo es requerido.");

                var entrada = _unitOfWork.EntradaInsumoRepository.GetById(dto.Id);
                if (entrada == null)
                    throw new Exception("Entrada no encontrada.");

                if (entrada.IdEstado != (int)EstadoEntradaInsumoEnum.Pendiente)
                    throw new Exception("La entrada ya fue procesada.");

                var estadoAnterior = entrada.IdEstado;
                entrada.IdEstado = (int)EstadoEntradaInsumoEnum.Rechazado;
                entrada.MotivoRechazo = dto.Motivo.Trim();
                entrada.UsuarioRechazo = dto.Usuario;
                entrada.FechaRechazo = DateTime.Now;
                entrada.UsuarioModificacion = dto.Usuario;
                entrada.FechaModificacion = DateTime.Now;

                _unitOfWork.EntradaInsumoRepository.Update(entrada);
                _unitOfWork.Commit();

                _unitOfWork.EntradaInsumoRepository.AgregarHistorial(new EntradaInsumoHistorialDTO
                {
                    IdEntradaInsumo = dto.Id,
                    IdEstadoAnterior = estadoAnterior,
                    IdEstadoNuevo = (int)EstadoEntradaInsumoEnum.Rechazado,
                    Accion = "Rechazado",
                    Observacion = dto.Motivo.Trim(),
                    Usuario = dto.Usuario
                });
                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntradaInsumoService - Rechazar: " + ex.Message;
                error.Exception = ex;
                error.Operation = "Rechazar";
                error.Code = TiposError.NoActualizado;
                error.Objeto = JsonConvert.SerializeObject(dto);
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public EntradaInsumoPaginacionDTO ObtenerTodos(EntradaInsumoFiltroDTO filtro)
        {
            try
            {
                return _unitOfWork.EntradaInsumoRepository.ObtenerTodos(filtro);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntradaInsumoService - ObtenerTodos: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerTodos";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<EntradaInsumoHistorialDTO> ObtenerHistorial(int id)
        {
            try
            {
                return _unitOfWork.EntradaInsumoRepository.ObtenerHistorial(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "EntradaInsumoService - ObtenerHistorial: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerHistorial";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }
    }
}