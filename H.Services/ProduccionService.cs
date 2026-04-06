using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Helpers;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Newtonsoft.Json;

namespace H.Services
{
    public class ProduccionService: IProduccionService
    {
        private IUnitOfWork _unitOfWork;
        public ProduccionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(Produccion entidad)
        {
            try
            {
                var modelo = _unitOfWork.ProduccionRepository.Add(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public int Update(Produccion entidad)
        {
            try
            {
                var modelo = _unitOfWork.ProduccionRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public int Delete(int id, string usuario)
        {
            try
            {
                var rpta = _unitOfWork.ProduccionRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionService" + ex.Message;
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
                return _unitOfWork.ProduccionRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public int AddMultipleTabla(InsertarProduccionDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new Exception("DTO nulo.");

                if (dto.CantidadProducida <= 0)
                    throw new Exception("Cantidad inválida.");

                if (string.IsNullOrWhiteSpace(dto.UsuarioCreacion))
                    throw new Exception("Usuario requerido.");

                var fechaActual = Fecha.Hoy;

                var receta = _unitOfWork.RecetaTortaRepository.GetBy(x => x.IdTorta == dto.IdTorta).ToList();

                if (!receta.Any())
                    throw new Exception("La torta no tiene receta.");

                // VALIDAR STOCK POR LOTES
                foreach (var item in receta)
                {
                    var cantidadNecesaria = item.CantidadRequerida * dto.CantidadProducida;

                    var hoy = fechaActual.Date;

                    var lotes = _unitOfWork.InsumoLoteRepository
                        .GetBy(x => x.IdInsumo == item.IdInsumo
                                 && x.Activo
                                 && x.CantidadDisponible > 0
                                 && (x.FechaVencimiento == null || x.FechaVencimiento.Value.Date >= hoy))
                        .OrderBy(x => x.FechaVencimiento)
                        .ToList();

                    var stockTotal = lotes.Sum(x => x.CantidadDisponible);

                    var insumo = _unitOfWork.InsumoRepository.GetById(item.IdInsumo);

                    if (stockTotal < cantidadNecesaria)
                        throw new Exception($"Stock insuficiente para insumo {insumo.Nombre}");
                }

                var produccion = new TProduccion
                {
                    IdTorta = dto.IdTorta,
                    FechaProduccion = fechaActual,
                    CantidadProducida = dto.CantidadProducida,
                    Observacion = dto.Observacion?.Trim(),
                    Activo = true,
                    UsuarioCreacion = dto.UsuarioCreacion,
                    UsuarioModificacion = dto.UsuarioCreacion,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                _unitOfWork.ProduccionRepository.Add(produccion);
                _unitOfWork.Commit();

                // PROCESAR INSUMOS
                foreach (var item in receta)
                {
                    var cantidadRestante = item.CantidadRequerida * dto.CantidadProducida;

                    var hoy = fechaActual.Date;

                    var lotes = _unitOfWork.InsumoLoteRepository
                        .GetBy(x => x.IdInsumo == item.IdInsumo
                                 && x.Activo
                                 && x.CantidadDisponible > 0
                                 && (x.FechaVencimiento == null
                                     || x.FechaVencimiento.Value.Date >= hoy))
                        .ToList()
                        .OrderBy(x => x.FechaVencimiento ?? DateTime.MaxValue)
                        .ToList();

                    foreach (var lote in lotes)
                    {
                        if (cantidadRestante <= 0)
                            break;

                        var cantidadUsada = Math.Min(lote.CantidadDisponible, cantidadRestante);

                        lote.CantidadDisponible -= cantidadUsada;

                        _unitOfWork.InsumoLoteRepository.Update(lote);

                        var movimiento = new TMovimientoInsumo
                        {
                            IdTipoMovimiento = (int)TipoMovimientoEnum.Salida,
                            IdInsumoLote = lote.Id,
                            Cantidad = cantidadUsada,
                            FechaMovimiento = fechaActual,
                            Referencia = "Producción",
                            Activo = true,
                            UsuarioCreacion = dto.UsuarioCreacion,
                            UsuarioModificacion = dto.UsuarioCreacion,
                            FechaCreacion = fechaActual,
                            FechaModificacion = fechaActual
                        };

                        _unitOfWork.MovimientoInsumoRepository.Add(movimiento);

                        var detalle = new TProduccionDetalleInsumo
                        {
                            IdProduccion = produccion.Id,
                            IdInsumo = item.IdInsumo,
                            CantidadUsada = cantidadUsada,
                            Activo = true,
                            UsuarioCreacion = dto.UsuarioCreacion,
                            UsuarioModificacion = dto.UsuarioCreacion,
                            FechaCreacion = fechaActual,
                            FechaModificacion = fechaActual
                        };

                        _unitOfWork.ProduccionDetalleInsumoRepository.Add(detalle);

                        cantidadRestante -= cantidadUsada;
                    }
                }

                var movimientoTorta = new TMovimientoTorta
                {
                    IdTorta = dto.IdTorta,
                    Cantidad = dto.CantidadProducida,
                    IdTipoMovimiento = (int)TipoMovimientoEnum.Produccion,
                    FechaMovimiento = fechaActual,
                    Activo = true,
                    UsuarioCreacion = dto.UsuarioCreacion,
                    UsuarioModificacion = dto.UsuarioCreacion,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                _unitOfWork.MovimientoTortaRepository.Add(movimientoTorta);

                var torta = _unitOfWork.TortaRepository.GetById(dto.IdTorta);

                if (torta == null)
                    throw new Exception("Torta no existe.");

                torta.StockDisponible += (int)dto.CantidadProducida;

                _unitOfWork.TortaRepository.Update(torta);

                _unitOfWork.Commit();

                return produccion.Id;
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = "ProduccionService - Create: " + ex.Message,
                    Exception = ex,
                    Operation = "Create",
                    Code = TiposError.NoActualizado,
                    Objeto = JsonConvert.SerializeObject(dto)
                };

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public bool AjustarInsumo(AjusteInsumoDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new Exception("DTO requerido.");

                if (dto.IdInsumo <= 0)
                    throw new Exception("Insumo inválido.");

                if (dto.Cantidad <= 0)
                    throw new Exception("Cantidad inválida.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    throw new Exception("Usuario requerido.");

                var fechaActual = Fecha.Hoy;

                var insumo = _unitOfWork.InsumoRepository.GetById(dto.IdInsumo);

                if (insumo == null)
                    throw new Exception("Insumo no existe.");

                if (!insumo.Activo)
                    throw new Exception("No se puede ajustar un insumo inactivo.");

                // traer lotes válidos
                var lotes = _unitOfWork.InsumoLoteRepository
                    .GetBy(x => x.IdInsumo == dto.IdInsumo && x.Activo)
                    .OrderBy(x => x.FechaVencimiento ?? DateTime.MaxValue)
                    .ToList();

                if (!lotes.Any())
                    throw new Exception("El insumo no tiene lotes.");

                var lote = lotes.First();

                if (!dto.EsEntrada && lote.CantidadDisponible < dto.Cantidad)
                    throw new Exception("El lote no tiene suficiente stock.");

                if (dto.EsEntrada)
                    lote.CantidadDisponible += dto.Cantidad;
                else
                    lote.CantidadDisponible -= dto.Cantidad;

                lote.UsuarioModificacion = dto.Usuario;
                lote.FechaModificacion = fechaActual;

                _unitOfWork.InsumoLoteRepository.Update(lote);

                var movimiento = new TMovimientoInsumo
                {
                    IdTipoMovimiento = dto.EsEntrada
                        ? (int)TipoMovimientoEnum.AjusteEntrada
                        : (int)TipoMovimientoEnum.AjusteSalida,
                    IdInsumoLote = lote.Id,
                    Cantidad = dto.Cantidad,
                    FechaMovimiento = fechaActual,
                    Referencia = dto.Observacion ?? "Ajuste manual",
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    UsuarioModificacion = dto.Usuario,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                _unitOfWork.MovimientoInsumoRepository.Add(movimiento);

                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService - AjustarInsumo: " + ex.Message;
                error.Exception = ex;
                error.Operation = "AjustarInsumo";
                error.Code = TiposError.NoActualizado;
                error.Objeto = JsonConvert.SerializeObject(dto);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public bool AjustarTorta(AjusteTortaDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new Exception("DTO requerido.");

                if (dto.IdTorta <= 0)
                    throw new Exception("Torta inválida.");

                if (dto.Cantidad <= 0)
                    throw new Exception("Cantidad inválida.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    throw new Exception("Usuario requerido.");

                var fechaActual = Fecha.Hoy;

                var torta = _unitOfWork.TortaRepository.GetById(dto.IdTorta);

                if (torta == null)
                    throw new Exception("Torta no existe.");

                if (!torta.Activo)
                    throw new Exception("No se puede ajustar una torta inactiva.");

                if (!dto.EsEntrada && torta.StockDisponible < dto.Cantidad)
                    throw new Exception("Stock insuficiente.");

                if (dto.EsEntrada)
                    torta.StockDisponible += (int)dto.Cantidad;
                else
                    torta.StockDisponible -= (int)dto.Cantidad;

                torta.UsuarioModificacion = dto.Usuario;
                torta.FechaModificacion = fechaActual;

                _unitOfWork.TortaRepository.Update(torta);

                var movimiento = new TMovimientoTorta
                {
                    IdTipoMovimiento = dto.EsEntrada
                        ? (int)TipoMovimientoEnum.AjusteEntrada
                        : (int)TipoMovimientoEnum.AjusteSalida,
                    IdTorta = dto.IdTorta,
                    Cantidad = dto.Cantidad,
                    FechaMovimiento = fechaActual,
                    Referencia = dto.Observacion ?? "Ajuste manual",
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    UsuarioModificacion = dto.Usuario,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                _unitOfWork.MovimientoTortaRepository.Add(movimiento);

                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionService - AjustarTorta: " + ex.Message;
                error.Exception = ex;
                error.Operation = "AjustarTorta";
                error.Code = TiposError.NoActualizado;
                error.Objeto = JsonConvert.SerializeObject(dto);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public IEnumerable<ProduccionDetalleDTO> ObtenerDetalleProduccion(int id)
        {
            if (id <= 0)
                throw new Exception("Id inválido.");

            return _unitOfWork.ProduccionRepository.ObtenerDetalle(id);
        }

        public IEnumerable<ProduccionCabeceraDTO> ObtenerProducciones()
        {
            return _unitOfWork.ProduccionRepository.ObtenerProducciones();
        }
    }
}