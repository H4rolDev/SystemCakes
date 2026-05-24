using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Newtonsoft.Json;

namespace H.Services
{
    public class RolService: IRolService
    {
        private IUnitOfWork _unitOfWork;
        public RolService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(Rol entidad)
        {
            try
            {
                var modelo = _unitOfWork.RolRepository.Add(entidad);
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

        public int Update(Rol entidad)
        {
            try
            {
                var modelo = _unitOfWork.RolRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RolService" + ex.Message;
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
                var rpta = _unitOfWork.RolRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RolService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public Rol GetById(int id)
        {
            try
            {
                return _unitOfWork.RolRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RolService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

public IEnumerable<RolListadoDTO> ObtenerCombo()
 		{
 			try
 			{
 				return _unitOfWork.RolRepository.ObtenerCombo();
 			}
 			catch (Exception ex)
 			{
 				var error = new Error();
 				error.Message = "RolService" + ex.Message;
 				error.Exception = ex;
 				error.Operation = "ObtenerListadoActivos";
 				error.Code = TiposError.NoEncontrado;
 				error.Objeto = JsonConvert.SerializeObject(null);

 				LogErp.EscribirBaseDatos(error);
 				throw ex;
 			}
 		}

        public IEnumerable<RolListadoDTO> ObtenerListado()
 		{
 			try
 			{
 				return _unitOfWork.RolRepository.ObtenerListado();
 			}
 			catch (Exception ex)
 			{
 				var error = new Error();
 				error.Message = "RolService" + ex.Message;
 				error.Exception = ex;
 				error.Operation = "ObtenerListado";
 				error.Code = TiposError.NoEncontrado;
 				error.Objeto = JsonConvert.SerializeObject(null);

 				LogErp.EscribirBaseDatos(error);
 				throw ex;
 			}
 		}
    }
}
