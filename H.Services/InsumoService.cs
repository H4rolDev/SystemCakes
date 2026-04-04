using AutoMapper;
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
    public class InsumoService: IInsumoService
    {
        private IUnitOfWork _unitOfWork;
        public InsumoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Add(Insumo entidad)
        {
            try
            {
                var modelo = _unitOfWork.InsumoRepository.Add(entidad);
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

        public int Update(Insumo entidad)
        {
            try
            {
                if (entidad == null)
                    throw new Exception("El objeto no puede ser nulo.");

                if (entidad.Id <= 0)
                    throw new Exception("Id inválido.");

                if (string.IsNullOrWhiteSpace(entidad.Nombre))
                    throw new Exception("Nombre requerido.");

                if (entidad.IdUnidadMedida <= 0)
                    throw new Exception("Unidad de medida inválida.");

                var existe = _unitOfWork.InsumoRepository.GetAll()
                    .Any(x => x.Nombre.ToLower() == entidad.Nombre.ToLower()
                           && x.Id != entidad.Id
                           && x.Activo);

                if (existe)
                    throw new Exception("Ya existe un insumo con ese nombre.");

                var insumoDb = _unitOfWork.InsumoRepository.GetById(entidad.Id);

                if (insumoDb == null)
                    throw new Exception("Insumo no encontrado.");

                insumoDb.Nombre = entidad.Nombre.Trim();
                insumoDb.IdUnidadMedida = entidad.IdUnidadMedida;
                insumoDb.UsuarioModificacion = entidad.UsuarioModificacion;
                insumoDb.FechaModificacion = Fecha.Hoy;

                _unitOfWork.InsumoRepository.Update(insumoDb);

                _unitOfWork.Commit();

                return insumoDb.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService - Update: " + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoActualizado;
                error.Objeto = JsonConvert.SerializeObject(entidad);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public int Delete(int id, string usuario)
        {
            try
            {
                var rpta = _unitOfWork.InsumoRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public Insumo GetById(int id)
        {
            try
            {
                return _unitOfWork.InsumoRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<InsumoListadoDTO> ObtenerCombo()
        {
            try
            {
                return _unitOfWork.InsumoRepository.ObtenerCombo();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public int InsertarLoteInsumo(InsertarLoteInsumoDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new Exception("El objeto no puede ser nulo.");

                if (dto.CantidadInicial <= 0)
                    throw new Exception("Cantidad inicial inválida.");

                if (dto.CantidadDisponible <= 0)
                    throw new Exception("Cantidad disponible inválida.");

                if (dto.CantidadDisponible > dto.CantidadInicial)
                    throw new Exception("La cantidad disponible no puede ser mayor a la inicial.");

                if (dto.CostoUnitario <= 0)
                    throw new Exception("Costo unitario inválido.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    throw new Exception("Usuario requerido.");

                var nombre = dto.Nombre.Trim().ToLower();

                var existe = _unitOfWork.InsumoRepository.GetAll().Any(x => x.Nombre.ToLower() == nombre && x.Activo);

                if (existe)
                    throw new Exception("El insumo ya está registrado.");

                var fechaActual = Fecha.Hoy;

                int idInsumo;

                if (dto.IdInsumo > 0)
                {
                    idInsumo = dto.IdInsumo;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dto.Nombre))
                        throw new Exception("Nombre del insumo requerido.");

                    var insumo = new TInsumo
                    {
                        Nombre = dto.Nombre.Trim(),
                        IdUnidadMedida = dto.IdUnidadMedida,
                        StockActual = 0,
                        StockMinimo = 0,
                        CostoUnitario = 0,
                        Activo = true,
                        UsuarioCreacion = dto.Usuario,
                        UsuarioModificacion = dto.Usuario,
                        FechaCreacion = fechaActual,
                        FechaModificacion = fechaActual
                    };

                    _unitOfWork.InsumoRepository.Add(insumo);
                    _unitOfWork.Commit();

                    idInsumo = insumo.Id;
                }

                var insumoLote = new TInsumoLote
                {
                    IdInsumo = idInsumo,
                    NumeroLote = null,
                    FechaIngreso = fechaActual,
                    FechaVencimiento = dto.FechaVencimiento,
                    CantidadInicial = dto.CantidadInicial,
                    CantidadDisponible = dto.CantidadDisponible,
                    CostoUnitario = dto.CostoUnitario,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    UsuarioModificacion = dto.Usuario,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                _unitOfWork.InsumoLoteRepository.Add(insumoLote);
                _unitOfWork.Commit();

                var idInsumoLote = insumoLote.Id;

                insumoLote.NumeroLote = $"LOT-{idInsumoLote:D6}";
                _unitOfWork.InsumoLoteRepository.Update(insumoLote);

                var movimiento = new TMovimientoInsumo
                {
                    IdTipoMovimiento = (int)TipoMovimientoEnum.Entrada,
                    IdInsumoLote = idInsumoLote,
                    Cantidad = dto.CantidadDisponible,
                    FechaMovimiento = fechaActual,
                    Referencia = null,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    UsuarioModificacion = dto.Usuario,
                    FechaCreacion = fechaActual,
                    FechaModificacion = fechaActual
                };

                _unitOfWork.MovimientoInsumoRepository.Add(movimiento);

                _unitOfWork.Commit();

                return idInsumoLote;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public bool ActualizarLoteInsumo(InsertarLoteInsumoDTO dto)
        {
            try
            {
                if (dto == null)
                    throw new Exception("El objeto no puede ser nulo.");

                if (dto.CantidadInicial <= 0)
                    throw new Exception("Cantidad inicial inválida.");

                if (dto.CostoUnitario <= 0)
                    throw new Exception("Costo unitario inválido.");

                if (string.IsNullOrWhiteSpace(dto.Usuario))
                    throw new Exception("Usuario requerido.");

                var fechaActual = Fecha.Hoy;

                var lote = _unitOfWork.InsumoLoteRepository.GetById(dto.idLote);

                if (lote == null)
                    throw new Exception("Lote no encontrado.");

                var cantidadAnterior = lote.CantidadDisponible;

                lote.FechaVencimiento = dto.FechaVencimiento;
                lote.CostoUnitario = dto.CostoUnitario;
                lote.CantidadInicial = dto.CantidadInicial;
                lote.CantidadDisponible = dto.CantidadDisponible;
                lote.UsuarioModificacion = dto.Usuario;
                lote.FechaModificacion = fechaActual;

                _unitOfWork.InsumoLoteRepository.Update(lote);

                var diferencia = dto.CantidadDisponible - cantidadAnterior;

                if (diferencia != 0)
                {
                    var movimiento = new TMovimientoInsumo
                    {
                        IdTipoMovimiento = diferencia > 0 ? (int)TipoMovimientoEnum.Entrada : (int)TipoMovimientoEnum.Salida,
                        IdInsumoLote = dto.idLote,
                        Cantidad = Math.Abs(diferencia),
                        FechaMovimiento = fechaActual,
                        Referencia = "Ajuste de lote",
                        Activo = true,
                        UsuarioCreacion = dto.Usuario,
                        UsuarioModificacion = dto.Usuario,
                        FechaCreacion = fechaActual,
                        FechaModificacion = fechaActual
                    };

                    _unitOfWork.MovimientoInsumoRepository.Add(movimiento);
                }

                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public void DesactivarInsumo(int idInsumo, string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Usuario requerido.");

                var insumo = _unitOfWork.InsumoRepository.GetById(idInsumo);

                if (insumo == null)
                    throw new Exception("Insumo no encontrado.");

                if (!insumo.Activo)
                    throw new Exception("El insumo ya está desactivado.");

                var lotes = _unitOfWork.InsumoLoteRepository.GetBy(x => x.IdInsumo == idInsumo && x.Activo);

                if (lotes.Any(x => x.CantidadDisponible > 0))
                    throw new Exception("No se puede desactivar un insumo con stock en sus lotes.");

                var fechaActual = Fecha.Hoy;

                foreach (var lote in lotes)
                {
                    lote.Activo = false;
                    lote.UsuarioModificacion = usuario;
                    lote.FechaModificacion = fechaActual;

                    _unitOfWork.InsumoLoteRepository.Update(lote);
                }

                insumo.Activo = false;
                insumo.UsuarioModificacion = usuario;
                insumo.FechaModificacion = fechaActual;

                _unitOfWork.InsumoRepository.Update(insumo);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public void DesactivarLoteInsumo(int idLote, string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Usuario requerido.");

                var lote = _unitOfWork.InsumoLoteRepository.GetById(idLote);

                if (lote == null)
                    throw new Exception("Lote no encontrado.");

                if (!lote.Activo)
                    throw new Exception("El lote ya está desactivado.");

                if (lote.CantidadDisponible > 0)
                    throw new Exception("No se puede desactivar un lote con stock disponible.");

                lote.Activo = false;
                lote.UsuarioModificacion = usuario;
                lote.FechaModificacion = Fecha.Hoy;

                _unitOfWork.InsumoLoteRepository.Update(lote);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public void ActivarInsumo(int idInsumo, string usuario)
        {
            try
            {
                if (idInsumo <= 0)
                    throw new Exception("Id de insumo inválido.");

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Usuario requerido.");

                var insumo = _unitOfWork.InsumoRepository.GetById(idInsumo);

                if (insumo == null)
                    throw new Exception("Insumo no encontrado.");

                if (insumo.Activo)
                    throw new Exception("El insumo ya está activo.");

                var fechaActual = Fecha.Hoy;

                insumo.Activo = true;
                insumo.UsuarioModificacion = usuario;
                insumo.FechaModificacion = fechaActual;

                _unitOfWork.InsumoRepository.Update(insumo);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService - ActivarInsumo: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ActivarInsumo";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(idInsumo);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public void ActivarLoteInsumo(int idLote, string usuario)
        {
            try
            {
                if (idLote <= 0)
                    throw new Exception("Id de lote inválido.");

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new Exception("Usuario requerido.");

                var lote = _unitOfWork.InsumoLoteRepository.GetById(idLote);

                if (lote == null)
                    throw new Exception("Lote no encontrado.");

                if (lote.Activo)
                    throw new Exception("El lote ya está activo.");

                var insumo = _unitOfWork.InsumoRepository.GetById(lote.IdInsumo);

                if (insumo == null)
                    throw new Exception("El insumo asociado no existe.");

                if (!insumo.Activo)
                    throw new Exception("No se puede activar un lote si el insumo está inactivo.");

                var fechaActual = Fecha.Hoy;

                if (lote.FechaVencimiento.HasValue && lote.FechaVencimiento.Value < fechaActual)
                    throw new Exception("No se puede activar un lote vencido.");

                lote.Activo = true;
                lote.UsuarioModificacion = usuario;
                lote.FechaModificacion = fechaActual;

                _unitOfWork.InsumoLoteRepository.Update(lote);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService - ActivarLoteInsumo: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ActivarLoteInsumo";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(idLote);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public IEnumerable<InsumoLoteDTO> ObtenerLotesPorInsumo(int idInsumo)
        {
            try
            {
                if (idInsumo <= 0)
                    throw new Exception("Id de insumo inválido.");

                var insumo = _unitOfWork.InsumoRepository.GetById(idInsumo);

                if (insumo == null)
                    throw new Exception("Insumo no encontrado.");

                return _unitOfWork.InsumoRepository.ObtenerLotesPorInsumo(idInsumo);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "InsumoService - ObtenerLotesPorInsumo: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerLotesPorInsumo";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(idInsumo);

                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }
    }
}
