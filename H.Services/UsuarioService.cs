using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Newtonsoft.Json;

namespace H.Services
{
    public class UsuarioService: IUsuarioService
    {
        private IUnitOfWork _unitOfWork;
        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(Usuario entidad)
        {
            try
            {
                var modelo = _unitOfWork.UsuarioRepository.Add(entidad);
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

        public int Update(Usuario entidad)
        {
            try
            {
                var modelo = _unitOfWork.UsuarioRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UsuarioService" + ex.Message;
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
                var rpta = _unitOfWork.UsuarioRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UsuarioService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public Usuario GetById(int id)
        {
            try
            {
                return _unitOfWork.UsuarioRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UsuarioService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

public IEnumerable<ComboDTO> ObtenerComboClientes()
 		{
 			try
 			{
 				return _unitOfWork.UsuarioRepository.ObtenerComboClientes();
 			}
 			catch (Exception ex)
 			{
 				var error = new Error();
 				error.Message = "UsuarioService" + ex.Message;
 				error.Exception = ex;
 				error.Operation = "ObtenerComboClientes";
 				error.Code = TiposError.NoEncontrado;
 				error.Objeto = JsonConvert.SerializeObject(null);

 				LogErp.EscribirBaseDatos(error);
 				throw ex;
 			}
 		}

        public IEnumerable<ComboDTO> ObtenerComboDrivers()
 		{
 			try
 			{
 				return _unitOfWork.UsuarioRepository.ObtenerComboDrivers();
 			}
 			catch (Exception ex)
 			{
 				var error = new Error();
 				error.Message = "UsuarioService" + ex.Message;
 				error.Exception = ex;
 				error.Operation = "ObtenerComboDrivers";
 				error.Code = TiposError.NoEncontrado;
 				error.Objeto = JsonConvert.SerializeObject(null);

 				LogErp.EscribirBaseDatos(error);
 				throw ex;
 			}
 		}
    }
}
