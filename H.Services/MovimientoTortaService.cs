using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Newtonsoft.Json;

namespace H.Services
{
    public class MovimientoTortaService: IMovimientoTortaService
    {
        private IUnitOfWork _unitOfWork;
        public MovimientoTortaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(MovimientoTorta entidad)
        {
            try
            {
                var torta = _unitOfWork.TortaRepository.GetById(entidad.IdTorta);
                if (torta == null)
                    throw new Exception("Torta no encontrada.");

                bool esEntrada = EsTipoEntrada(entidad.IdTipoMovimiento);
                bool esSalida = EsTipoSalida(entidad.IdTipoMovimiento);

                if (esSalida && torta.StockDisponible < entidad.Cantidad)
                    throw new Exception("Stock insuficiente de torta.");

                if (esEntrada)
                    torta.StockDisponible += (int)entidad.Cantidad;
                else if (esSalida)
                    torta.StockDisponible -= (int)entidad.Cantidad;

                _unitOfWork.TortaRepository.Update(torta);

                var modelo = _unitOfWork.MovimientoTortaRepository.Add(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoTortaService - Add: " + ex.Message;
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
                || idTipoMovimiento == (int)TipoMovimientoEnum.AjusteEntrada
                || idTipoMovimiento == (int)TipoMovimientoEnum.Produccion;
        }

        private bool EsTipoSalida(int idTipoMovimiento)
        {
            return idTipoMovimiento == (int)TipoMovimientoEnum.Salida
                || idTipoMovimiento == (int)TipoMovimientoEnum.AjusteSalida
                || idTipoMovimiento == (int)TipoMovimientoEnum.Venta
                || idTipoMovimiento == (int)TipoMovimientoEnum.Anulacion
                || idTipoMovimiento == (int)TipoMovimientoEnum.Merma
                || idTipoMovimiento == (int)TipoMovimientoEnum.Regalo
                || idTipoMovimiento == (int)TipoMovimientoEnum.Descarga
                || idTipoMovimiento == (int)TipoMovimientoEnum.Donacion;
        }

        public int Update(MovimientoTorta entidad)
        {
            try
            {
                var movimientoActual = _unitOfWork.MovimientoTortaRepository.GetById(entidad.Id);
                if (movimientoActual == null)
                    throw new Exception("Movimiento no encontrado.");

                var torta = _unitOfWork.TortaRepository.GetById(movimientoActual.IdTorta);
                if (torta == null)
                    throw new Exception("Torta no encontrada.");

                bool eraEntrada = EsTipoEntrada(movimientoActual.IdTipoMovimiento);
                bool eraSalida = EsTipoSalida(movimientoActual.IdTipoMovimiento);

                if (eraEntrada)
                    torta.StockDisponible -= (int)movimientoActual.Cantidad;
                else if (eraSalida)
                    torta.StockDisponible += (int)movimientoActual.Cantidad;

                bool esEntrada = EsTipoEntrada(entidad.IdTipoMovimiento);
                bool esSalida = EsTipoSalida(entidad.IdTipoMovimiento);

                if (esSalida && torta.StockDisponible < entidad.Cantidad)
                    throw new Exception("Stock insuficiente de torta.");

                if (esEntrada)
                    torta.StockDisponible += (int)entidad.Cantidad;
                else if (esSalida)
                    torta.StockDisponible -= (int)entidad.Cantidad;

                _unitOfWork.TortaRepository.Update(torta);

                var modelo = _unitOfWork.MovimientoTortaRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoTortaService - Update: " + ex.Message;
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
                var movimiento = _unitOfWork.MovimientoTortaRepository.GetById(id);
                if (movimiento == null)
                    throw new Exception("Movimiento no encontrado.");

                var torta = _unitOfWork.TortaRepository.GetById(movimiento.IdTorta);
                if (torta == null)
                    throw new Exception("Torta no encontrada.");

                bool eraEntrada = EsTipoEntrada(movimiento.IdTipoMovimiento);
                bool eraSalida = EsTipoSalida(movimiento.IdTipoMovimiento);

                if (eraEntrada)
                    torta.StockDisponible -= (int)movimiento.Cantidad;
                else if (eraSalida)
                    torta.StockDisponible += (int)movimiento.Cantidad;

                _unitOfWork.TortaRepository.Update(torta);

                var rpta = _unitOfWork.MovimientoTortaRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoTortaService - Delete: " + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public MovimientoTorta GetById(int id)
        {
            try
            {
                return _unitOfWork.MovimientoTortaRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoTortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<MovimientoTortaListadoDTO> ObtenerCombo()
        {
            try
            {
                return _unitOfWork.MovimientoTortaRepository.ObtenerCombo();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "MovimientoTortaService" + ex.Message;
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
