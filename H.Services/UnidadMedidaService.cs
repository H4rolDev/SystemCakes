using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Newtonsoft.Json;

namespace H.Services
{
    public class UnidadMedidaService: IUnidadMedidaService
    {
        private IUnitOfWork _unitOfWork;
        public UnidadMedidaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(UnidadMedida entidad)
        {
            try
            {
                var modelo = _unitOfWork.UnidadMedidaRepository.Add(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "AlmacenService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public int Update(UnidadMedida entidad)
        {
            try
            {
                var modelo = _unitOfWork.UnidadMedidaRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UnidadMedidaService" + ex.Message;
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
                var rpta = _unitOfWork.UnidadMedidaRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UnidadMedidaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public UnidadMedida GetById(int id)
        {
            try
            {
                return _unitOfWork.UnidadMedidaRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UnidadMedidaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<UnidadMedidaListadoDTO> ObtenerCombo()
        {
            try
            {
                return _unitOfWork.UnidadMedidaRepository.ObtenerCombo();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UnidadMedidaService" + ex.Message;
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
