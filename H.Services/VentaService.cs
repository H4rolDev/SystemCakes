using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Helpers;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace H.Services
{
    public class VentaService: IVentaService
    {
        private IUnitOfWork _unitOfWork;

        public VentaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Add(Venta entidad)
        {
            try
            {
                var modelo = _unitOfWork.VentaRepository.Add(entidad);
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

        public int Update(Venta entidad)
        {
            try
            {
                var modelo = _unitOfWork.VentaRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService" + ex.Message;
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
                var rpta = _unitOfWork.VentaRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public Venta GetById(int id)
        {
            try
            {
                return _unitOfWork.VentaRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "AlmacenService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public VentaPaginacionDTO ObtenerTodos(VentaFiltroDTO filtro)
        {
            try
            {
                return _unitOfWork.VentaRepository.ObtenerTodos(filtro);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService - ObtenerTodos: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerTodos";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<VentaHistorialDTO> ObtenerHistorial(int idVenta)
        {
            try
            {
                return _unitOfWork.VentaRepository.ObtenerHistorial(idVenta);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService - ObtenerHistorial: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerHistorial";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<ComboDTO> ObtenerComboMetodoPago()
        {
            try
            {
                return _unitOfWork.VentaRepository.ObtenerComboMetodoPago();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "CategoriaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<ComboDTO> ObtenerComboTipoComprobante()
        {
            try
            {
                return _unitOfWork.VentaRepository.ObtenerComboTipoComprobante();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "CategoriaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<ComboDTO> ObtenerComboPersonal()
        {
            try
            {
                return _unitOfWork.VentaRepository.ObtenerComboPersonal();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "CategoriaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
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
                var usuariosDriver = _unitOfWork.UsuarioRolRepository.GetBy(x => x.IdRol == (int)RolEnum.Repartidor && x.Activo).ToList();
                var idsUsuario = usuariosDriver.Select(u => u.IdUsuario).ToList();
                var usuarios = _unitOfWork.UsuarioRepository.GetAll()
                    .Where(u => idsUsuario.Contains(u.Id) && u.Activo)
                    .ToList();
                var idsPersona = usuarios.Select(u => u.IdPersona).ToList();
                var personas = _unitOfWork.PersonaRepository.GetAll()
                    .Where(p => idsPersona.Contains(p.Id) && p.Activo)
                    .ToList();
                return personas.Select(p => new ComboDTO { Id = p.Id, Nombre = (p.ApellidoPaterno ?? "") + " " + p.Nombres });
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerComboDrivers";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<ComboDTO> ObtenerComboClientes()
        {
            try
            {
                var usuariosCliente = _unitOfWork.UsuarioRolRepository.GetBy(x => x.IdRol == (int)RolEnum.Cliente && x.Activo).ToList();
                var idsUsuario = usuariosCliente.Select(u => u.IdUsuario).ToList();
                var usuarios = _unitOfWork.UsuarioRepository.GetAll()
                    .Where(u => idsUsuario.Contains(u.Id) && u.Activo)
                    .ToList();
                var idsPersona = usuarios.Select(u => u.IdPersona).ToList();
                var personas = _unitOfWork.PersonaRepository.GetAll()
                    .Where(p => idsPersona.Contains(p.Id) && p.Activo)
                    .ToList();
                return personas.Select(p => new ComboDTO { Id = p.Id, Nombre = (p.ApellidoPaterno ?? "") + " " + p.Nombres, NumeroDocumento = p.NumeroDocumento });
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerComboClientes";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public void AsignarDriver(int idDelivery, int idDriver, string usuario)
        {
            var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
            if (delivery == null)
                throw new Exception("Delivery no encontrado.");

            var persona = _unitOfWork.PersonaRepository.GetById(idDriver);
            if (persona == null)
                throw new Exception("Driver no encontrado.");

            delivery.IdPersonalRepartior = idDriver;
            delivery.FechaModificacion = Fecha.Hoy;
            delivery.UsuarioModificacion = usuario;
            _unitOfWork.EntregaDeliveryRepository.Update(delivery);
            _unitOfWork.Commit();
        }

        public int RegistrarVenta(InsertarVentaDTO dto)
        {
            var fecha = Fecha.Hoy;

            if (dto == null || !dto.Detalles.Any())
                throw new Exception("Venta inválida.");

            if (dto.Comprobante != null)
            {
                var existe = _unitOfWork.ComprobanteVentaRepository.GetAll()
                    .Any(x => x.Serie == dto.Comprobante.Serie
                           && x.Numero == dto.Comprobante.Numero
                           && x.Activo);

                if (existe)
                    throw new Exception("Serie y número ya registrados.");
            }

            decimal subTotal = 0;

            foreach (var d in dto.Detalles)
            {
                var torta = _unitOfWork.TortaRepository.GetById(d.IdTorta);

                if (torta == null)
                    throw new Exception("Torta no existe.");

                if (torta.StockDisponible < d.Cantidad)
                    throw new Exception($"Stock insuficiente para {torta.Nombre}");

                var precio = d.PrecioBase + d.PrecioPersonalizacion;
                subTotal += precio * d.Cantidad;
            }

            decimal costoDelivery = 0;
            if (dto.IdTipoEntrega == (int)TipoEntregaEnum.Delivery && dto.Entrega != null)
                costoDelivery = dto.Entrega.CostoDelivery;

            var totalVenta = subTotal + costoDelivery;
            var totalPagos = dto.Pagos.Sum(x => x.Monto);

            if (totalPagos < totalVenta)
                throw new Exception("Pago insuficiente.");

            var tieneComprobante = !string.IsNullOrEmpty(dto.ImagenComprobante);
            var estadoInicial = tieneComprobante ? (int)EstadoVentaEnum.EsperandoValidacion : (int)EstadoVentaEnum.Pagada;

            var venta = new TVenta
            {
                IdPersona = dto.IdPersona,
                IdEstadoVenta = estadoInicial,
                IdTipoEntrega = dto.IdTipoEntrega,
                FechaVenta = fecha,
                SubTotal = subTotal,
                Total = totalVenta,
                Activo = true,
                UsuarioCreacion = dto.Usuario,
                FechaCreacion = fecha,
                ImagenComprobante = dto.ImagenComprobante,
                NumeroOperacion = dto.NumeroOperacion
            };

            _unitOfWork.VentaRepository.Add(venta);
            _unitOfWork.Commit();

            // DETALLE + MOVIMIENTO TORTA
            foreach (var d in dto.Detalles)
            {
                var precioFinal = d.PrecioBase + d.PrecioPersonalizacion;

                var detalle = new TVentaDetalle
                {
                    IdVenta = venta.Id,
                    IdTorta = d.IdTorta,
                    Cantidad = d.Cantidad,
                    PrecioBase = d.PrecioBase,
                    PrecioPersonalizacion = d.PrecioPersonalizacion,
                    PrecioFinal = precioFinal,
                    SubTotal = precioFinal * d.Cantidad,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = fecha
                };

                _unitOfWork.VentaDetalleRepository.Add(detalle);
                _unitOfWork.Commit();

                // 🔥 MOVIMIENTO TORTA
                var movimiento = new TMovimientoTorta
                {
                    IdTorta = d.IdTorta,
                    IdTipoMovimiento = (int)TipoMovimientoEnum.Venta,
                    Cantidad = d.Cantidad,
                    FechaMovimiento = fecha,
                    Referencia = $"Venta #{venta.Id}",
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = fecha
                };

                _unitOfWork.MovimientoTortaRepository.Add(movimiento);

                // DESCONTAR STOCK
                var torta = _unitOfWork.TortaRepository.GetById(d.IdTorta);
                torta.StockDisponible -= d.Cantidad;

                _unitOfWork.TortaRepository.Update(torta);
            }

            // PAGOS
            foreach (var p in dto.Pagos)
            {
                _unitOfWork.PagoVentaRepository.Add(new TPagoVenta
                {
                    IdVenta = venta.Id,
                    IdMetodoPago = p.IdMetodoPago,
                    Monto = p.Monto,
                    NumeroOperacion = p.NumeroOperacion,
                    FechaPago = fecha,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = fecha
                });
            }

            // DELIVERY
            if (dto.IdTipoEntrega == (int)TipoEntregaEnum.Delivery && dto.Entrega != null)
            {
                _unitOfWork.EntregaDeliveryRepository.Add(new TEntregaDelivery
                {
                    IdVenta = venta.Id,
                    IdEstadoEntrega = (int)EstadoEntregaEnum.Pendiente,
                    IdPersonalRepartidor = dto.Entrega.IdPersonalRepartidor > 0 ? dto.Entrega.IdPersonalRepartidor : null,
                    Direccion = dto.Entrega.Direccion,
                    Referencia = dto.Entrega.Referencia,
                    TelefonoContacto = dto.Entrega.Telefono,
                    NombreContacto = dto.Entrega.NombreContacto,
                    CostoDelivery = dto.Entrega.CostoDelivery,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = fecha
                });
            }

            // COMPROBANTE
            if (dto.Comprobante != null)
            {
                _unitOfWork.ComprobanteVentaRepository.Add(new TComprobanteVenta
                {
                    IdVenta = venta.Id,
                    IdTipoComprobante = dto.Comprobante.IdTipoComprobante,
                    Serie = dto.Comprobante.Serie,
                    Numero = dto.Comprobante.Numero,
                    FechaEmision = fecha,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = fecha
                });
            }

            _unitOfWork.Commit();

            return venta.Id;
        }

        public void CancelarVenta(int idVenta, string motivo, string usuario)
        {
            var venta = _unitOfWork.VentaRepository.GetById(idVenta);

            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Cancelada)
                throw new Exception("Ya cancelada.");

            var detalles = _unitOfWork.VentaDetalleRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            foreach (var d in detalles)
            {
                var torta = _unitOfWork.TortaRepository.GetById(d.IdTorta);

                torta.StockDisponible += (int)d.Cantidad;
                _unitOfWork.TortaRepository.Update(torta);

                // 🔥 MOVIMIENTO REVERSA
                _unitOfWork.MovimientoTortaRepository.Add(new TMovimientoTorta
                {
                    IdTorta = d.IdTorta,
                    IdTipoMovimiento = (int)TipoMovimientoEnum.Anulacion,
                    Cantidad = d.Cantidad,
                    FechaMovimiento = Fecha.Hoy,
                    Referencia = $"Cancelación Venta #{idVenta}",
                    UsuarioCreacion = usuario,
                    Activo = true
                });
            }

            venta.IdEstadoVenta = (int)EstadoVentaEnum.Cancelada;

            _unitOfWork.VentaRepository.Update(venta);

            _unitOfWork.CancelacionVentaRepository.Add(new TCancelacionVenta
            {
                IdVenta = idVenta,
                Motivo = motivo,
                FechaCancelacion = Fecha.Hoy,
                UsuarioCreacion = usuario,
                Activo = true
            });

            _unitOfWork.Commit();
        }

        public IEnumerable<object> Listado()
        {
            return _unitOfWork.VentaRepository.GetAll()
                .Select(x => new
                {
                    x.Id,
                    x.FechaVenta,
                    x.Total,
                    x.IdEstadoVenta,
                    x.IdTipoEntrega
                }).ToList();
        }

        public object Detalle(int idVenta)
        {
            var venta = _unitOfWork.VentaRepository.GetById(idVenta);
            var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);

            var detalles = _unitOfWork.VentaDetalleRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var pagos = _unitOfWork.PagoVentaRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var delivery = _unitOfWork.EntregaDeliveryRepository
                .GetBy(x => x.IdVenta == idVenta).FirstOrDefault();

            var comp = _unitOfWork.ComprobanteVentaRepository
                .GetBy(x => x.IdVenta == idVenta).FirstOrDefault();

            return new
            {
                venta = new
                {
                    venta.Id,
                    venta.FechaVenta,
                    venta.SubTotal,
                    venta.Total,
                    venta.IdEstadoVenta,
                    venta.IdTipoEntrega,
                    venta.UsuarioCreacion
                },
                cliente = new
                {
                    persona.Id,
                    nombre = persona.ApellidoPaterno + " " + persona.Nombres,
                    documento = persona.NumeroDocumento
                },
                detalles = detalles.Select(d => new
                {
                    d.IdTorta,
                    torta = _unitOfWork.TortaRepository.GetById(d.IdTorta).Nombre,
                    d.Cantidad,
                    d.PrecioBase,
                    d.PrecioPersonalizacion,
                    d.PrecioFinal,
                    d.SubTotal
                }).ToList(),
                pagos = pagos.Select(p => new
                {
                    p.IdMetodoPago,
                    nombreMetodo = _unitOfWork.MetodoPagoRepository.GetById(p.IdMetodoPago).Nombre,
                    p.Monto,
                    p.NumeroOperacion
                }).ToList(),
                delivery,
                comprobante = comp != null ? new
                {
                    comp.Serie,
                    comp.Numero,
                    comp.IdTipoComprobante
                } : null
            };
        }

        public VentaComprobanteDTO ObtenerComprobante(int idVenta)
        {
            var venta = _unitOfWork.VentaRepository.GetById(idVenta);
            var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);

            var detalles = _unitOfWork.VentaDetalleRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var pagos = _unitOfWork.PagoVentaRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var comp = _unitOfWork.ComprobanteVentaRepository
                .GetBy(x => x.IdVenta == idVenta).FirstOrDefault();

            var delivery = _unitOfWork.EntregaDeliveryRepository
                .GetBy(x => x.IdVenta == idVenta).FirstOrDefault();

            var nombreCliente = persona.ApellidoPaterno + " " + persona.Nombres;

            return new VentaComprobanteDTO
            {
                IdVenta = venta.Id,
                Fecha = venta.FechaVenta,
                SubTotal = venta.SubTotal,
                Total = venta.Total,
                Cliente = nombreCliente,

                TipoComprobante = comp?.IdTipoComprobante.ToString(),
                SerieNumero = comp != null ? $"{comp.Serie}-{comp.Numero}" : "",

                TipoEntrega = venta.IdTipoEntrega == 1 ? "Tienda" : "Delivery",
                Direccion = delivery?.Direccion,

                Detalles = detalles.Select(x => new DetalleComprobanteDTO
                {
                    Torta = _unitOfWork.TortaRepository.GetById(x.IdTorta).Nombre,
                    Cantidad = (int)x.Cantidad,
                    PrecioUnitario = x.PrecioFinal,
                    SubTotal = x.SubTotal
                }).ToList(),

                Pagos = pagos.Select(x => new PagoComprobanteDTO
                {
                    Metodo = _unitOfWork.MetodoPagoRepository.GetById(x.IdMetodoPago).Nombre,
                    Monto = x.Monto
                }).ToList()
            };
        }

        public IEnumerable<object> ObtenerListadoDeliveries()
        {
            var deliveries = _unitOfWork.EntregaDeliveryRepository.GetAll()
                .Where(x => x.Activo)
                .ToList();

            var result = new List<object>();
            foreach (var d in deliveries)
            {
                var venta = _unitOfWork.VentaRepository.GetById(d.IdVenta);
                
                if (venta.IdEstadoVenta != (int)EstadoVentaEnum.Pagada && 
                    venta.IdEstadoVenta != (int)EstadoVentaEnum.Aprobada)
                {
                    continue;
                }

                var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);
                var estado = ObtenerEstadoEntrega(d.IdEstadoEntrega);

                result.Add(new
                {
                    d.Id,
                    d.IdVenta,
                    venta.FechaVenta,
                    cliente = persona.ApellidoPaterno + " " + persona.Nombres,
                    clienteTelefono = persona.Telefono,
                    d.Direccion,
                    d.Referencia,
                    d.TelefonoContacto,
                    d.NombreContacto,
                    d.CostoDelivery,
                    d.IdEstadoEntrega,
                    estado = estado != null ? estado.Nombre : "Desconocido",
                    d.FechaAsignacion,
                    d.FechaEntrega,
                    d.IdPersonalRepartidor
                });
            }
            return result;
        }

        public void ActualizarEstadoDelivery(int idDelivery, int idEstadoEntrega, string usuario)
        {
            var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
            if (delivery == null)
                throw new Exception("Delivery no encontrado.");

            delivery.IdEstadoEntrega = idEstadoEntrega;
            delivery.FechaModificacion = Fecha.Hoy;
            delivery.UsuarioModificacion = usuario;

            if (idEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
            {
                delivery.FechaEntrega = Fecha.Hoy;
            }

            _unitOfWork.EntregaDeliveryRepository.Update(delivery);
            _unitOfWork.Commit();
        }

        public IEnumerable<ComboDTO> ObtenerComboEstadoEntrega()
        {
            return _unitOfWork.EstadoEntregaRepository.GetAll()
                .Where(x => x.Activo)
                .Select(x => new ComboDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                }).ToList();
        }

        public void AsignarRepartidor(int idDelivery, int idPersonalRepartidor, string usuario)
        {
            var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
            if (delivery == null)
                throw new Exception("Delivery no encontrado.");

            delivery.IdPersonalRepartidor = idPersonalRepartidor;
            delivery.FechaAsignacion = Fecha.Hoy;
            delivery.FechaModificacion = Fecha.Hoy;
            delivery.UsuarioModificacion = usuario;

            _unitOfWork.EntregaDeliveryRepository.Update(delivery);
            _unitOfWork.Commit();
        }

        public IEnumerable<ComboDTO> ObtenerComboTipoEntrega()
        {
            return _unitOfWork.TipoEntregaRepository.GetAll()
                .Where(x => x.Activo)
                .Select(x => new ComboDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                }).ToList();
        }

        private TEstadoEntrega? ObtenerEstadoEntrega(int id)
        {
            return _unitOfWork.EstadoEntregaRepository.GetById(id);
        }

        public IEnumerable<MisPedidosDTO> ObtenerMisPedidos(int idPersona)
        {
            var ventas = _unitOfWork.VentaRepository.GetBy(x => x.IdPersona == idPersona && x.Activo)
                .OrderByDescending(x => x.FechaCreacion)
                .ToList();

            var result = new List<MisPedidosDTO>();

            foreach (var venta in ventas)
            {
                var detalles = _unitOfWork.VentaDetalleRepository.GetBy(x => x.IdVenta == venta.Id).ToList();
                var pagos = _unitOfWork.PagoVentaRepository.GetBy(x => x.IdVenta == venta.Id).ToList();
                var entrega = _unitOfWork.EntregaDeliveryRepository.GetBy(x => x.IdVenta == venta.Id).FirstOrDefault();

                var nombresTortas = string.Join(", ", detalles.Select(d => {
                    var torta = _unitOfWork.TortaRepository.GetById(d.IdTorta);
                    return torta?.Nombre ?? "Torta";
                }));

                var estadoPago = _unitOfWork.EstadoVentaRepository.GetById(venta.IdEstadoVenta);

                var deliveryEstado = entrega != null ? ObtenerEstadoEntrega(entrega.IdEstadoEntrega)?.Nombre ?? "Sin delivery" : null;

                result.Add(new MisPedidosDTO
                {
                    Id = venta.Id,
                    Fecha = venta.FechaCreacion,
                    Total = venta.Total ?? 0,
                    EstadoPago = estadoPago?.Nombre ?? "Desconocido",
                    Productos = nombresTortas,
                    Cantidad = (int)detalles.Sum(x => x.Cantidad),
                    TipoEntrega = venta.IdTipoEntrega == 1 ? "Recojo en tienda" : "Delivery",
                    DeliveryEstado = deliveryEstado,
                    DeliveryDireccion = entrega?.Direccion,
                    DeliveryTelefono = entrega?.TelefonoContacto,
                    MetodoPago = string.Join(", ", pagos.Select(p => {
                        var metodo = _unitOfWork.MetodoPagoRepository.GetById(p.IdMetodoPago);
                        return metodo?.Nombre ?? "N/A";
                    }))
                });
            }

            return result;
        }

        public void CancelarEntrega(int idDelivery, string motivo, string usuario)
        {
            try
            {
                var entrega = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
                if (entrega == null)
                    throw new Exception("Delivery no encontrado.");

                if (entrega.IdEstadoEntrega == 3)
                    throw new Exception("No se puede cancelar un pedido ya entregado.");
                
                if (entrega.IdEstadoEntrega == 4)
                    throw new Exception("El pedido ya está cancelado.");

                entrega.IdEstadoEntrega = 4;
                entrega.FechaModificacion = Fecha.Hoy;
                entrega.UsuarioModificacion = usuario;

                _unitOfWork.EntregaDeliveryRepository.Update(entrega);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.CancelarEntrega" + ex.Message;
                error.Exception = ex;
                error.Operation = "CancelarEntrega";
                error.Code = TiposError.NoActualizado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<VentaValidacionDTO> ObtenerVentasPendientesValidacion()
        {
            try
            {
                var ventas = _unitOfWork.VentaRepository.GetAll()
                    .Where(v => v.IdEstadoVenta == (int)EstadoVentaEnum.EsperandoValidacion && v.Activo)
                    .ToList();

                var resultado = new List<VentaValidacionDTO>();

                foreach (var v in ventas)
                {
                    var persona = _unitOfWork.PersonaRepository.GetById(v.IdPersona);
                    var detalles = _unitOfWork.VentaDetalleRepository.GetAll()
                        .Where(d => d.IdVenta == v.Id && d.Activo)
                        .ToList();

                    var detallesDto = new List<VentaDetalleValidacionDTO>();
                    foreach (var d in detalles)
                    {
                        var tortaNombre = "Torta ID: " + d.IdTorta;
                        try
                        {
                            var torta = _unitOfWork.TortaRepository.GetById(d.IdTorta);
                            tortaNombre = torta?.Nombre ?? tortaNombre;
                        }
                        catch { }

                        detallesDto.Add(new VentaDetalleValidacionDTO
                        {
                            IdTorta = d.IdTorta,
                            Torta = tortaNombre,
                            Cantidad = d.Cantidad,
                            Precio = d.PrecioFinal ?? 0
                        });
                    }

                    resultado.Add(new VentaValidacionDTO
                    {
                        Id = v.Id,
                        FechaVenta = v.FechaVenta ?? Fecha.Hoy,
                        Cliente = $"{persona?.Nombres ?? ""} {persona?.ApellidoPaterno ?? ""} {persona?.ApellidoMaterno ?? ""}".Trim(),
                        ClienteTelefono = persona?.Telefono ?? "",
                        Total = v.Total ?? 0,
                        NumeroOperacion = v.NumeroOperacion ?? "",
                        ImagenComprobante = v.ImagenComprobante ?? "",
                        Estado = "Esperando Validación",
                        IdEstadoVenta = v.IdEstadoVenta,
                        Detalles = detallesDto
                    });
                }

                return resultado.OrderByDescending(v => v.FechaVenta);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.ObtenerVentasPendientesValidacion" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerVentasPendientesValidacion";
                error.Code = TiposError.NoEncontrado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public void AprobarVenta(int idVenta, string usuario)
        {
            try
            {
                var venta = _unitOfWork.VentaRepository.GetById(idVenta);
                if (venta == null)
                    throw new Exception("Venta no encontrada.");

                if (venta.IdEstadoVenta != (int)EstadoVentaEnum.EsperandoValidacion)
                    throw new Exception("La venta no está en estado de validación.");

                var estadoAnterior = venta.IdEstadoVenta;
                venta.IdEstadoVenta = (int)EstadoVentaEnum.Pagada;
                venta.FechaModificacion = Fecha.Hoy;
                venta.UsuarioModificacion = usuario;

                _unitOfWork.VentaRepository.Update(venta);

                var detalles = _unitOfWork.VentaDetalleRepository.GetAll()
                    .Where(d => d.IdVenta == idVenta && d.Activo)
                    .ToList();

                foreach (var d in detalles)
                {
                    var movimiento = new TMovimientoTorta
                    {
                        IdTorta = d.IdTorta,
                        IdTipoMovimiento = (int)TipoMovimientoEnum.Venta,
                        Cantidad = d.Cantidad,
                        FechaMovimiento = Fecha.Hoy,
                        Referencia = $"Venta #{idVenta} - Aprobada",
                        Activo = true,
                        UsuarioCreacion = usuario,
                        FechaCreacion = Fecha.Hoy
                    };
                    _unitOfWork.MovimientoTortaRepository.Add(movimiento);
                }

                _unitOfWork.Commit();

                _unitOfWork.VentaRepository.AgregarHistorial(new VentaHistorialDTO
                {
                    IdVenta = idVenta,
                    IdEstadoAnterior = estadoAnterior,
                    IdEstadoNuevo = (int)EstadoVentaEnum.Pagada,
                    Accion = "Aprobada",
                    Observacion = "Pago validado y stock actualizado",
                    Usuario = usuario
                });
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.AprobarVenta" + ex.Message;
                error.Exception = ex;
                error.Operation = "AprobarVenta";
                error.Code = TiposError.NoActualizado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public void RechazarVenta(int idVenta, string motivoRechazo, string usuario)
        {
            try
            {
                var venta = _unitOfWork.VentaRepository.GetById(idVenta);
                if (venta == null)
                    throw new Exception("Venta no encontrada.");

                if (venta.IdEstadoVenta != (int)EstadoVentaEnum.EsperandoValidacion)
                    throw new Exception("La venta no está en estado de validación.");

                var estadoAnterior = venta.IdEstadoVenta;
                venta.IdEstadoVenta = (int)EstadoVentaEnum.Rechazada;
                venta.MotivoRechazo = motivoRechazo;
                venta.FechaModificacion = Fecha.Hoy;
                venta.UsuarioModificacion = usuario;

                _unitOfWork.VentaRepository.Update(venta);
                _unitOfWork.Commit();

                _unitOfWork.VentaRepository.AgregarHistorial(new VentaHistorialDTO
                {
                    IdVenta = idVenta,
                    IdEstadoAnterior = estadoAnterior,
                    IdEstadoNuevo = (int)EstadoVentaEnum.Rechazada,
                    Accion = "Rechazada",
                    Observacion = motivoRechazo,
                    Usuario = usuario
                });
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.RechazarVenta" + ex.Message;
                error.Exception = ex;
                error.Operation = "RechazarVenta";
                error.Code = TiposError.NoActualizado;
                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        // ===================== NUEVOS MÉTODOS PARA REPARTIDORES =====================

        public IEnumerable<object> ObtenerMisPedidosRepartidor(int idPersona)
        {
            try
            {
                var deliveries = _unitOfWork.EntregaDeliveryRepository.GetAll()
                    .Where(x => x.IdPersonalRepartidor == idPersona && x.Activo)
                    .ToList();

                var result = new List<object>();
                foreach (var d in deliveries)
                {
                    var venta = _unitOfWork.VentaRepository.GetById(d.IdVenta);
                    var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);
                    var estado = ObtenerEstadoEntrega(d.IdEstadoEntrega);

                    result.Add(new
                    {
                        d.Id,
                        d.IdVenta,
                        venta.FechaVenta,
                        cliente = persona.ApellidoPaterno + " " + persona.Nombres,
                        clienteTelefono = persona.Telefono,
                        d.Direccion,
                        d.Referencia,
                        d.TelefonoContacto,
                        d.NombreContacto,
                        d.CostoDelivery,
                        d.IdEstadoEntrega,
                        estado = estado != null ? estado.Nombre : "Desconocido",
                        d.FechaAsignacion,
                        d.FechaEntrega,
                        puedeAceptar = d.IdEstadoEntrega == (int)EstadoEntregaEnum.Asignado,
                        puedeIniciar = d.IdEstadoEntrega == (int)EstadoEntregaEnum.Aceptado,
                        puedeCompletar = d.IdEstadoEntrega == (int)EstadoEntregaEnum.EnCamino
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.ObtenerMisPedidosRepartidor: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerMisPedidosRepartidor";
                LogErp.EscribirBaseDatos(error);
                return new List<object>();
            }
        }

        public void AceptarPedidoDelivery(int idDelivery, string usuario)
        {
            try
            {
                var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
                if (delivery == null)
                    throw new Exception("Delivery no encontrado.");

                if (delivery.IdEstadoEntrega != (int)EstadoEntregaEnum.Asignado)
                    throw new Exception("El pedido debe estar en estado 'Asignado' para poder aceptarlo.");

                delivery.IdEstadoEntrega = (int)EstadoEntregaEnum.Aceptado;
                delivery.FechaModificacion = Fecha.Hoy;
                delivery.UsuarioModificacion = usuario;

                _unitOfWork.EntregaDeliveryRepository.Update(delivery);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.AceptarPedidoDelivery: " + ex.Message;
                error.Exception = ex;
                error.Operation = "AceptarPedidoDelivery";
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public void IniciarDelivery(int idDelivery, string usuario)
        {
            try
            {
                var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
                if (delivery == null)
                    throw new Exception("Delivery no encontrado.");

                if (delivery.IdEstadoEntrega != (int)EstadoEntregaEnum.Aceptado)
                    throw new Exception("El pedido debe estar aceptado para iniciar el delivery.");

                delivery.IdEstadoEntrega = (int)EstadoEntregaEnum.EnCamino;
                delivery.FechaModificacion = Fecha.Hoy;
                delivery.UsuarioModificacion = usuario;

                _unitOfWork.EntregaDeliveryRepository.Update(delivery);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.IniciarDelivery: " + ex.Message;
                error.Exception = ex;
                error.Operation = "IniciarDelivery";
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public void CompletarEntrega(int idDelivery, string usuario)
        {
            try
            {
                var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
                if (delivery == null)
                    throw new Exception("Delivery no encontrado.");

                if (delivery.IdEstadoEntrega != (int)EstadoEntregaEnum.EnCamino)
                    throw new Exception("El pedido debe estar en camino para completarlo.");

                delivery.IdEstadoEntrega = (int)EstadoEntregaEnum.Entregado;
                delivery.FechaEntrega = Fecha.Hoy;
                delivery.FechaModificacion = Fecha.Hoy;
                delivery.UsuarioModificacion = usuario;

                _unitOfWork.EntregaDeliveryRepository.Update(delivery);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.CompletarEntrega: " + ex.Message;
                error.Exception = ex;
                error.Operation = "CompletarEntrega";
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public void DesasignarPedido(int idDelivery, string usuario)
        {
            try
            {
                var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
                if (delivery == null)
                    throw new Exception("Delivery no encontrado.");

                delivery.IdPersonalRepartidor = null;
                delivery.IdEstadoEntrega = (int)EstadoEntregaEnum.Pendiente;
                delivery.FechaAsignacion = null;
                delivery.FechaModificacion = Fecha.Hoy;
                delivery.UsuarioModificacion = usuario;

                _unitOfWork.EntregaDeliveryRepository.Update(delivery);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.DesasignarPedido: " + ex.Message;
                error.Exception = ex;
                error.Operation = "DesasignarPedido";
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        public IEnumerable<object> ObtenerGestionRepartidores()
        {
            try
            {
                var deliveries = _unitOfWork.EntregaDeliveryRepository.GetAll()
                    .Where(x => x.Activo && x.IdPersonalRepartidor != null)
                    .ToList();

                var result = new List<object>();
                foreach (var d in deliveries)
                {
                    var venta = _unitOfWork.VentaRepository.GetById(d.IdVenta);
                    var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);
                    var repartidor = _unitOfWork.PersonaRepository.GetById(d.IdPersonalRepartidor ?? 0);
                    var estado = ObtenerEstadoEntrega(d.IdEstadoEntrega);

                    TimeSpan? tiempoEntrega = null;
                    if (d.FechaAsignacion.HasValue && d.FechaEntrega.HasValue)
                    {
                        tiempoEntrega = d.FechaEntrega.Value - d.FechaAsignacion.Value;
                    }

                    result.Add(new
                    {
                        d.Id,
                        d.IdVenta,
                        venta.FechaVenta,
                        cliente = persona.ApellidoPaterno + " " + persona.Nombres,
                        clienteTelefono = persona.Telefono,
                        d.Direccion,
                        repartidorId = d.IdPersonalRepartidor,
                        repartidorNombre = repartidor != null ? repartidor.ApellidoPaterno + " " + repartidor.Nombres : "Sin asignar",
                        d.IdEstadoEntrega,
                        estado = estado != null ? estado.Nombre : "Desconocido",
                        d.FechaAsignacion,
                        d.FechaEntrega,
                        tiempoEntregaMinutos = tiempoEntrega.HasValue ? (int)tiempoEntrega.Value.TotalMinutes : (int?)null,
                        puedeDesasignar = d.IdEstadoEntrega != (int)EstadoEntregaEnum.Entregado && 
                                         d.IdEstadoEntrega != (int)EstadoEntregaEnum.Cancelado
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "VentaService.ObtenerGestionRepartidores: " + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerGestionRepartidores";
                LogErp.EscribirBaseDatos(error);
                return new List<object>();
            }
        }
    }
}
