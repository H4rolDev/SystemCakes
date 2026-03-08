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
                if (dto == null || dto.CantidadProducida <= 0)
                    throw new Exception("Cantidad inválida.");

                var fechaActual = Fecha.Hoy;

                var receta = _unitOfWork.RecetaTortaRepository.GetBy(x => x.IdTorta == dto.IdTorta).ToList();

                if (!receta.Any())
                    throw new Exception("La torta no tiene receta registrada.");

                foreach (var item in receta)
                {
                    var cantidadTotal = item.CantidadRequerida * dto.CantidadProducida;
                    var insumo = _unitOfWork.InsumoRepository.GetById(item.IdInsumo);

                    if (insumo == null)
                        throw new Exception($"Insumo {insumo.Nombre} no existe.");

                    if (insumo.StockActual < cantidadTotal)
                        throw new Exception($"Stock insuficiente para el insumo {insumo.Nombre}.");
                }

                var produccion = new TProduccion
                {
                    IdTorta = dto.IdTorta,
                    CantidadProducida = dto.CantidadProducida,
                    FechaProduccion = fechaActual,
                    Observacion = string.IsNullOrWhiteSpace(dto.Observacion) ? null : dto.Observacion.Trim(),
                    Activo = true,
                    UsuarioCreacion = dto.UsuarioCreacion,
                    UsuarioModificacion = dto.UsuarioCreacion,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                _unitOfWork.ProduccionRepository.Add(produccion);

                foreach (var item in receta)
                {
                    var cantidadTotal = item.CantidadRequerida * dto.CantidadProducida;
                    var insumo = _unitOfWork.InsumoRepository.GetById(item.IdInsumo);
                    insumo.StockActual -= cantidadTotal;
                    _unitOfWork.InsumoRepository.Update(insumo);
                }

                var torta = _unitOfWork.TortaRepository.GetById(dto.IdTorta);

                if (torta == null)
                    throw new Exception("La torta no existe.");

                torta.Cantidades += dto.CantidadProducida;

                _unitOfWork.TortaRepository.Update(torta);
                _unitOfWork.Commit();

                return produccion.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "ProduccionService" + ex.Message;
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
