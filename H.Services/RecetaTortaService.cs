using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Helpers;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace H.Services
{
    public class RecetaTortaService: IRecetaTortaService
    {
        private IUnitOfWork _unitOfWork;
        public RecetaTortaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(RecetaTorta entidad)
        {
            try
            {
                var modelo = _unitOfWork.RecetaTortaRepository.Add(entidad);
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

        public int Update(RecetaTorta entidad)
        {
            try
            {
                var modelo = _unitOfWork.RecetaTortaRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RecetaTortaService" + ex.Message;
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
                var rpta = _unitOfWork.RecetaTortaRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RecetaTortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public RecetaTorta GetById(int id)
        {
            try
            {
                return _unitOfWork.RecetaTortaRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RecetaTortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        /*public IEnumerable<RecetaTortaListadoDTO> ObtenerCombo()
		{
			try
			{
				return _unitOfWork.RecetaTortaRepository.ObtenerCombo();
			}
			catch (Exception ex)
			{
				var error = new Error();
				error.Message = "RecetaTortaService" + ex.Message;
				error.Exception = ex;
				error.Operation = "ObtenerListadoActivos";
				error.Code = TiposError.NoEncontrado;
				error.Objeto = JsonConvert.SerializeObject(null);

				LogErp.EscribirBaseDatos(error);
				throw ex;
			}
		}*/

        public bool AddMultipleTabla(InsertarMultipleRecetaTortaDTO dto)
        {
            try
            {
                if (dto == null || dto.Detalles == null || !dto.Detalles.Any())
                {
                    throw new Exception("No se enviaron detalles para registrar.");
                }
                var fechaActual = Fecha.Hoy;
                foreach (var item in dto.Detalles)
                {
                    var detalle = new TRecetaTorta
                    {
                        IdTorta = dto.IdTorta,
                        IdInsumo = item.IdInsumo,
                        CantidadNecesaria = item.CantidadNecesaria,
                        Estado = true,
                        UsuarioCreacion = dto.UsuarioCreacion,
                        UsuarioModificacion = dto.UsuarioCreacion,
                        FechaCreacion = fechaActual,
                        FechaModificacion = fechaActual
                    };

                    _unitOfWork.RecetaTortaRepository.Add(detalle);
                }
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "AlmacenService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(dto);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }
    }
}
