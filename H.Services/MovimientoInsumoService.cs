using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Newtonsoft.Json;

namespace H.Services
{
    public class MovimientoInsumoService: IMovimientoInsumoService
    {
        private IUnitOfWork _unitOfWork;
        public MovimientoInsumoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(MovimientoInsumo entidad)
        {
            try
            {
                var lote = _unitOfWork.InsumoLoteRepository.GetById(entidad.IdInsumoLote);
                if (lote == null)
                    throw new Exception("Lote de insumo no encontrado.");

                var insumo = _unitOfWork.InsumoRepository.GetById(lote.IdInsumo);
                if (insumo == null)
                    throw new Exception("Insumo no encontrado.");

                bool esEntrada = EsTipoEntrada(entidad.IdTipoMovimiento);
                bool esSalida = EsTipoSalida(entidad.IdTipoMovimiento);

                if (esSalida && lote.CantidadDisponible < entidad.Cantidad)
                    throw new Exception("Stock insuficiente en el lote.");

                if (esEntrada)
                {
                    lote.CantidadDisponible += entidad.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) + entidad.Cantidad;
                }
                else if (esSalida)
                {
                    lote.CantidadDisponible -= entidad.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) - entidad.Cantidad;
                }

                _unitOfWork.InsumoLoteRepository.Update(lote);
                _unitOfWork.InsumoRepository.Update(insumo);

                var modelo = _unitOfWork.MovimientoInsumoRepository.Add(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoInsumoService - Add: " + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        private bool EsTipoEntrada(int idTipoMovimiento)
        {
            return idTipoMovimiento == (int)TipoMovimientoEnum.Entrada
                || idTipoMovimiento == (int)TipoMovimientoEnum.AjusteEntrada;
        }

        private bool EsTipoSalida(int idTipoMovimiento)
        {
            return idTipoMovimiento == (int)TipoMovimientoEnum.Salida
                || idTipoMovimiento == (int)TipoMovimientoEnum.AjusteSalida
                || idTipoMovimiento == (int)TipoMovimientoEnum.Merma
                || idTipoMovimiento == (int)TipoMovimientoEnum.Regalo
                || idTipoMovimiento == (int)TipoMovimientoEnum.Descarga
                || idTipoMovimiento == (int)TipoMovimientoEnum.Donacion;
        }

        public int Update(MovimientoInsumo entidad)
        {
            try
            {
                var movimientoActual = _unitOfWork.MovimientoInsumoRepository.GetById(entidad.Id);
                if (movimientoActual == null)
                    throw new Exception("Movimiento no encontrado.");

                var lote = _unitOfWork.InsumoLoteRepository.GetById(movimientoActual.IdInsumoLote);
                if (lote == null)
                    throw new Exception("Lote de insumo no encontrado.");

                var insumo = _unitOfWork.InsumoRepository.GetById(lote.IdInsumo);
                if (insumo == null)
                    throw new Exception("Insumo no encontrado.");

                bool eraEntrada = EsTipoEntrada(movimientoActual.IdTipoMovimiento);
                bool eraSalida = EsTipoSalida(movimientoActual.IdTipoMovimiento);

                if (eraEntrada)
                {
                    lote.CantidadDisponible -= movimientoActual.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) - movimientoActual.Cantidad;
                }
                else if (eraSalida)
                {
                    lote.CantidadDisponible += movimientoActual.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) + movimientoActual.Cantidad;
                }

                bool esEntrada = EsTipoEntrada(entidad.IdTipoMovimiento);
                bool esSalida = EsTipoSalida(entidad.IdTipoMovimiento);

                if (esSalida && lote.CantidadDisponible < entidad.Cantidad)
                    throw new Exception("Stock insuficiente en el lote.");

                if (esEntrada)
                {
                    lote.CantidadDisponible += entidad.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) + entidad.Cantidad;
                }
                else if (esSalida)
                {
                    lote.CantidadDisponible -= entidad.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) - entidad.Cantidad;
                }

                _unitOfWork.InsumoLoteRepository.Update(lote);
                _unitOfWork.InsumoRepository.Update(insumo);

                var modelo = _unitOfWork.MovimientoInsumoRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoInsumoService - Update: " + ex.Message;
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
                var movimiento = _unitOfWork.MovimientoInsumoRepository.GetById(id);
                if (movimiento == null)
                    throw new Exception("Movimiento no encontrado.");

                var lote = _unitOfWork.InsumoLoteRepository.GetById(movimiento.IdInsumoLote);
                if (lote == null)
                    throw new Exception("Lote de insumo no encontrado.");

                var insumo = _unitOfWork.InsumoRepository.GetById(lote.IdInsumo);
                if (insumo == null)
                    throw new Exception("Insumo no encontrado.");

                bool eraEntrada = EsTipoEntrada(movimiento.IdTipoMovimiento);
                bool eraSalida = EsTipoSalida(movimiento.IdTipoMovimiento);

                if (eraEntrada)
                {
                    lote.CantidadDisponible -= movimiento.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) - movimiento.Cantidad;
                }
                else if (eraSalida)
                {
                    lote.CantidadDisponible += movimiento.Cantidad;
                    insumo.StockActual = (insumo.StockActual ?? 0) + movimiento.Cantidad;
                }

                _unitOfWork.InsumoLoteRepository.Update(lote);
                _unitOfWork.InsumoRepository.Update(insumo);

                var rpta = _unitOfWork.MovimientoInsumoRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoInsumoService - Delete: " + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public MovimientoInsumo GetById(int id)
        {
            try
            {
                return _unitOfWork.MovimientoInsumoRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoInsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<MovimientoInsumoListadoDTO> ObtenerCombo()
        {
            try
            {
                return _unitOfWork.MovimientoInsumoRepository.ObtenerCombo();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoInsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<InsumoLoteListadoDTO> ObtenerInsumoLote()
        {
            try
            {
                return _unitOfWork.InsumoLoteRepository.ObtenerCombo();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoInsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }
    }
}
