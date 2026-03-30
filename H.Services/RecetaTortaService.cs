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

        public bool Delete(int idTorta, string usuario)
        {
            try
            {
                var fechaActual = Fecha.Hoy;

                var registros = _unitOfWork.RecetaTortaRepository.GetBy(x => x.IdTorta == idTorta && x.Activo).ToList();

                if (registros.Count() == 0) throw new Exception("No se tiene registros para eliminar.");

                foreach (var item in registros)
                {
                    item.Activo = false;
                    item.UsuarioModificacion = usuario;
                    item.FechaModificacion = fechaActual;

                    _unitOfWork.RecetaTortaRepository.Update(item);
                }

                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RecetaTortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(idTorta);

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

        public IEnumerable<RecetaTortaListadoDTO> ObtenerCombo()
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
        }

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
                        CantidadRequerida = item.CantidadRequerida,
                        Activo = true,
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

        public bool UpdateMultipleTabla(InsertarMultipleRecetaTortaDTO dto)
        {
            try
            {
                if (dto == null || dto.Detalles == null || !dto.Detalles.Any())
                {
                    throw new Exception("No se enviaron detalles para actualizar.");
                }

                var tortaActual = _unitOfWork.TortaRepository.GetBy(x => x.Id == dto.IdTorta && x.Activo).FirstOrDefault();

                if (tortaActual == null)
                {
                    throw new Exception("La torta no existe.");
                }

                var tortasMismoNombre = _unitOfWork.TortaRepository.GetBy(x => x.Nombre.Trim().ToLower() == tortaActual.Nombre.Trim().ToLower() 
                    && x.Id != dto.IdTorta && x.Activo).ToList();

                foreach (var torta in tortasMismoNombre)
                {
                    var tieneReceta = _unitOfWork.RecetaTortaRepository.GetBy(r => r.IdTorta == torta.Id && r.Activo).Any();

                    if (tieneReceta)
                    {
                        throw new Exception($"Ya existe una receta registrada para la torta '{tortaActual.Nombre}'.");
                    }
                }

                var fechaActual = Fecha.Hoy;

                var actuales = _unitOfWork.RecetaTortaRepository.GetBy(x => x.IdTorta == dto.IdTorta && x.Activo).ToList();

                var actualesDict = actuales.ToDictionary(x => x.IdInsumo);

                foreach (var item in dto.Detalles)
                {
                    if (item.IdInsumo <= 0)
                        continue;

                    if (actualesDict.TryGetValue(item.IdInsumo, out var existente))
                    {
                        existente.CantidadRequerida = item.CantidadRequerida;
                        existente.UsuarioModificacion = dto.UsuarioCreacion;
                        existente.FechaModificacion = fechaActual;

                        _unitOfWork.RecetaTortaRepository.Update(existente);

                        actualesDict.Remove(item.IdInsumo);
                    }
                    else
                    {
                        var nuevo = new TRecetaTorta
                        {
                            IdTorta = dto.IdTorta,
                            IdInsumo = item.IdInsumo,
                            CantidadRequerida = item.CantidadRequerida,
                            Activo = true,
                            UsuarioCreacion = dto.UsuarioCreacion,
                            UsuarioModificacion = dto.UsuarioCreacion,
                            FechaCreacion = fechaActual,
                            FechaModificacion = fechaActual
                        };

                        _unitOfWork.RecetaTortaRepository.Add(nuevo);
                    }
                }

                foreach (var item in actualesDict.Values)
                {
                    item.Activo = false;
                    item.UsuarioModificacion = dto.UsuarioCreacion;
                    item.FechaModificacion = fechaActual;

                    _unitOfWork.RecetaTortaRepository.Update(item);
                }

                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "RecetaTortaService UpdateMultipleTabla: " + ex.Message;
                error.Exception = ex;
                error.Operation = "UpdateMultiple";
                error.Code = TiposError.NoActualizado;
                error.Objeto = JsonConvert.SerializeObject(dto);

                LogErp.EscribirBaseDatos(error);

                throw;
            }
        }
    }
}
