using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Helpers;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;

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


            if (dto == null || dto.Detalles == null || dto.Detalles.Count == 0)
                throw new Exception("Venta inválida.");

            if (dto.Pagos == null || dto.Pagos.Count == 0)
                throw new Exception("La venta debe incluir al menos un pago.");

            if (dto.IdTipoEntrega != (int)TipoEntregaEnum.RecojoTienda &&
                dto.IdTipoEntrega != (int)TipoEntregaEnum.Delivery)
                throw new Exception("Tipo de entrega inválido.");

            if (dto.IdPersona <= 0 || string.IsNullOrWhiteSpace(dto.Usuario))
                throw new Exception("Cliente y usuario son requeridos.");

            if (dto.IdTipoEntrega == (int)TipoEntregaEnum.Delivery &&
                (dto.Entrega == null || string.IsNullOrWhiteSpace(dto.Entrega.Direccion) ||
                 string.IsNullOrWhiteSpace(dto.Entrega.Telefono)))
                throw new Exception("Dirección y teléfono de delivery son requeridos.");

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
                if (d.IdTorta <= 0 || d.Cantidad <= 0 || d.Pisos is <= 0 || d.Porciones is <= 0)
                    throw new Exception("Detalle de venta inválido.");

                var torta = _unitOfWork.TortaRepository.GetById(d.IdTorta);

                if (torta == null)
                    throw new Exception("Torta no existe.");

                if (torta.StockDisponible < d.Cantidad)
                    throw new Exception($"Stock insuficiente para {torta.Nombre}");

                var precioBase = decimal.Round(torta.PrecioVenta ?? 0, 2);
                var precioPersonalizacion = CalcularPrecioPersonalizacion(torta, d);
                d.PrecioBase = precioBase;
                d.PrecioPersonalizacion = precioPersonalizacion;
                var precio = precioBase + precioPersonalizacion;
                subTotal += precio * d.Cantidad;
            }

            decimal costoDelivery = 0;
            if (dto.IdTipoEntrega == (int)TipoEntregaEnum.Delivery && dto.Entrega != null)
            {
                var deliveryConfig = DeliveryPricingService.GetOrCreate(_unitOfWork);
                costoDelivery = DeliveryPricingService.CalculateCost(
                    deliveryConfig, dto.Entrega.Latitud, dto.Entrega.Longitud);
                dto.Entrega.CostoDelivery = costoDelivery;
            }

            var totalVenta = subTotal + costoDelivery;
            var metodosPago = dto.Pagos
                .Select(p => _unitOfWork.MetodoPagoRepository.GetById(p.IdMetodoPago)?.Nombre ?? string.Empty)
                .ToList();
            if (metodosPago.Any(string.IsNullOrWhiteSpace))
                throw new Exception("Uno de los métodos de pago no es válido.");
            var esDelivery = dto.IdTipoEntrega == (int)TipoEntregaEnum.Delivery;
            if (esDelivery && metodosPago.Any(nombre =>
                    !EsMetodoPagoDeliveryPermitido(nombre)))
                throw new Exception("Para delivery usa Yape, Plin, banca móvil o depósito. No se acepta efectivo ni tarjeta.");

            var pagosDigitales = dto.Pagos
                .Where(p => !EsMetodoPagoEfectivo(_unitOfWork.MetodoPagoRepository.GetById(p.IdMetodoPago)?.Nombre ?? string.Empty))
                .ToList();
            var pagosEfectivo = dto.Pagos
                .Where(p => EsMetodoPagoEfectivo(_unitOfWork.MetodoPagoRepository.GetById(p.IdMetodoPago)?.Nombre ?? string.Empty))
                .ToList();
            var totalPagoDigital = decimal.Round(pagosDigitales.Sum(x => x.Monto), 2);
            var totalPagoEfectivo = decimal.Round(pagosEfectivo.Sum(x => x.Monto), 2);
            var tieneComprobante = !string.IsNullOrEmpty(dto.ImagenComprobante);
            var requiereAnticipo = totalPagoDigital > 0;

            if (esDelivery && totalPagoEfectivo > 0)
                throw new Exception("Delivery solo acepta pagos digitales.");

            if (esDelivery && totalPagoDigital < totalVenta)
                throw new Exception("El delivery debe pagarse completamente mediante banca móvil.");

            if (totalPagoDigital > 0)
            {
                if (totalPagoDigital > totalVenta)
                    throw new Exception("El monto digital no puede superar el total de la venta.");
                if (string.IsNullOrWhiteSpace(dto.ImagenComprobante))
                    throw new Exception("El pago debe incluir un comprobante de pago.");
            }
            if (esDelivery && !tieneComprobante)
                throw new Exception("El delivery debe incluir un comprobante de pago.");
            if (totalPagoEfectivo > 0 && dto.IdTipoEntrega != (int)TipoEntregaEnum.RecojoTienda)
                throw new Exception("El efectivo solo está permitido para recojo en tienda.");
            if (totalPagoDigital + totalPagoEfectivo > totalVenta)
                throw new Exception("El monto pagado no puede superar el total de la venta.");
            if (totalPagoDigital == 0 && totalPagoEfectivo < totalVenta && dto.IdTipoEntrega == (int)TipoEntregaEnum.RecojoTienda)
            {
                // El efectivo declarado representa la intención de pago; se cobra al recoger.
                totalPagoEfectivo = totalVenta;
            }

            // Reserva las unidades de forma atómica mientras el pago está pendiente.
            // La salida definitiva se registra únicamente cuando el pago es aprobado.
            var stockReservado = new List<(int IdTorta, int Cantidad)>();
            try
            {
                foreach (var detalle in dto.Detalles)
                {
                    ReservarStock(detalle.IdTorta, detalle.Cantidad);
                    stockReservado.Add((detalle.IdTorta, detalle.Cantidad));
                }
            }
            catch
            {
                foreach (var reserva in stockReservado)
                    LiberarStock(reserva.IdTorta, reserva.Cantidad);
                throw;
            }

            var estadoInicial = tieneComprobante
                ? (int)EstadoVentaEnum.EsperandoValidacion
                : (int)EstadoVentaEnum.Pendiente;

            var venta = new TVenta
            {
                IdPersona = dto.IdPersona,
                IdEstadoVenta = estadoInicial,
                IdTipoEntrega = dto.IdTipoEntrega,
                FechaVenta = fecha,
                SubTotal = subTotal,
                Total = totalVenta,
                MontoPagado = totalPagoDigital,
                SaldoPendiente = decimal.Round(Math.Max(0, totalVenta - totalPagoDigital), 2),
                RequiereAnticipo = requiereAnticipo,
                CodigoEntrega = GenerarCodigoEntrega(),
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
                    MensajePersonalizado = d.Mensaje,
                    TamanoPersonalizado = d.Tamanio,
                    SaborPersonalizado = d.Sabor,
                    RellenoPersonalizado = d.Relleno,
                    PisosPersonalizados = d.Pisos,
                    ColorDecoracionPersonalizada = d.ColorDecoracion,
                    DecoracionPersonalizada = d.Decoracion,
                    CoberturaPersonalizada = d.Cobertura,
                    PorcionesPersonalizadas = d.Porciones,
                    EventoPersonalizado = d.Evento,
                    FechaEntregaSolicitada = d.FechaEntrega,
                    ImagenReferencia = d.ImagenReferencia,
                    ObservacionesPersonalizacion = d.Observaciones,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = fecha
                };

                _unitOfWork.VentaDetalleRepository.Add(detalle);
                _unitOfWork.Commit();

                // El stock ya fue reservado atómicamente antes de crear la venta.
            }

            // PAGOS
            foreach (var p in pagosDigitales.Where(p => p.Monto > 0))
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
                    Latitud = dto.Entrega.Latitud,
                    Longitud = dto.Entrega.Longitud,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = fecha
                });
            }

            _unitOfWork.Commit();

            return venta.Id;
        }

        private decimal CalcularPrecioPersonalizacion(Torta torta, DetalleVentaDTO detalle)
        {
            if (torta.EsPersonalizable != true)
            {
                if (!string.IsNullOrWhiteSpace(detalle.Tamanio) || !string.IsNullOrWhiteSpace(detalle.Sabor) ||
                    !string.IsNullOrWhiteSpace(detalle.Relleno) || !string.IsNullOrWhiteSpace(detalle.ColorDecoracion) ||
                    !string.IsNullOrWhiteSpace(detalle.Decoracion) || !string.IsNullOrWhiteSpace(detalle.Cobertura) ||
                    detalle.Pisos is > 1 || detalle.Porciones is > 0)
                    throw new Exception($"La torta {torta.Nombre} no admite personalización.");
                return 0;
            }

            var seleccionadas = new[]
            {
                (Tipo: "tamanio", Valor: detalle.Tamanio),
                (Tipo: "sabor", Valor: detalle.Sabor),
                (Tipo: "relleno", Valor: detalle.Relleno),
                (Tipo: "color", Valor: detalle.ColorDecoracion),
                (Tipo: "decoracion", Valor: detalle.Decoracion),
                (Tipo: "cobertura", Valor: detalle.Cobertura),
                (Tipo: "evento", Valor: detalle.Evento),
                (Tipo: "pisos", Valor: detalle.Pisos?.ToString()),
                (Tipo: "porciones", Valor: detalle.Porciones?.ToString())
            };
            var opciones = _unitOfWork.TortaOpcionRepository.ObtenerPorTorta(torta.Id, true);
            var pisos = detalle.Pisos ?? 1;
            var opcionesPisos = opciones.Where(x => x.Tipo.Equals("pisos", StringComparison.OrdinalIgnoreCase)).ToList();
            var maximoPisosConfigurado = opcionesPisos
                .Where(x => x.Maximo.HasValue && x.Maximo.Value > 0)
                .Select(x => x.Maximo!.Value)
                .DefaultIfEmpty(4)
                .Max();
            if (pisos > maximoPisosConfigurado)
                throw new Exception($"Esta torta permite como máximo {maximoPisosConfigurado} pisos.");

            foreach (var grupo in opciones.Where(x => x.Obligatorio).GroupBy(x => x.Tipo, StringComparer.OrdinalIgnoreCase))
            {
                var seleccion = seleccionadas.FirstOrDefault(x => x.Tipo.Equals(grupo.Key, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrWhiteSpace(seleccion.Valor))
                    throw new Exception($"Debes seleccionar una opción de {grupo.Key} para {torta.Nombre}.");
            }
            decimal extra = opcionesPisos.Count == 0 ? Math.Max(0, pisos - 1) * 20m : 0m;
            foreach (var seleccion in seleccionadas.Where(x => !string.IsNullOrWhiteSpace(x.Valor)))
            {
                var grupoConfigurado = opciones.Any(x => x.Tipo.Equals(seleccion.Tipo, StringComparison.OrdinalIgnoreCase));
                if (!grupoConfigurado)
                    continue;
                var opcion = opciones.FirstOrDefault(x => x.Tipo.Equals(seleccion.Tipo, StringComparison.OrdinalIgnoreCase) &&
                    ValoresCoinciden(x.Valor, seleccion.Valor!, seleccion.Tipo));
                if (opcion == null)
                    opcion = opciones.FirstOrDefault(x => x.Tipo.Equals(seleccion.Tipo, StringComparison.OrdinalIgnoreCase) && x.ModoPrecio == "incremental");
                if (opcion == null)
                    throw new Exception($"La opción '{seleccion.Valor}' no está disponible para {torta.Nombre}.");
                if (opcion.Maximo.HasValue && int.TryParse(seleccion.Valor, out var numericValue) && numericValue > opcion.Maximo.Value)
                    throw new Exception($"La opción '{seleccion.Valor}' supera el máximo permitido.");
                if (opcion.ModoPrecio == "incremental" && int.TryParse(seleccion.Valor, out var units))
                    extra += opcion.PrecioPorUnidad * Math.Max(0, units - (opcion.Minimo ?? 1));
                else
                    extra += opcion.PrecioExtra;
            }
            if (!string.IsNullOrWhiteSpace(detalle.Mensaje) && detalle.Mensaje.Length > 100)
                throw new Exception("El mensaje personalizado no puede superar 100 caracteres.");
            if (!string.IsNullOrWhiteSpace(detalle.Observaciones) && detalle.Observaciones.Length > 500)
                throw new Exception("Las observaciones no pueden superar 500 caracteres.");
            return decimal.Round(extra, 2);
        }

        private static bool ValoresCoinciden(string configurado, string seleccionado, string tipo)
        {
            if (configurado.Equals(seleccionado.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
            if (tipo is not ("pisos" or "porciones")) return false;
            var configuredNumber = new string(configurado.Where(char.IsDigit).ToArray());
            var selectedNumber = new string(seleccionado.Where(char.IsDigit).ToArray());
            return configuredNumber.Length > 0 && configuredNumber == selectedNumber;
        }

        private static bool EsMetodoPagoDeliveryPermitido(string nombre)
        {
            var nombreNormalizado = nombre.ToLowerInvariant();
            return nombreNormalizado.Contains("yape") ||
                   nombreNormalizado.Contains("plin") ||
                   nombreNormalizado.Contains("deposito") ||
                   nombreNormalizado.Contains("depósito") ||
                   nombreNormalizado.Contains("transferencia") ||
                   nombreNormalizado.Contains("banca");
        }

        private static bool EsMetodoPagoEfectivo(string nombre)
        {
            return nombre.Trim().ToLowerInvariant().Contains("efectivo");
        }

        private void ReservarStock(int idTorta, int cantidad)
        {
            var filas = _unitOfWork.Context.Database.ExecuteSqlInterpolated($@"
                UPDATE TTorta
                SET StockDisponible = StockDisponible - {cantidad}
                WHERE Id = {idTorta} AND Activo = 1 AND StockDisponible >= {cantidad}");

            if (filas != 1)
                throw new Exception("El stock cambió mientras se registraba el pedido. Actualiza el catálogo e inténtalo nuevamente.");
        }

        private void LiberarStock(int idTorta, int cantidad)
        {
            _unitOfWork.Context.Database.ExecuteSqlInterpolated($@"
                UPDATE TTorta
                SET StockDisponible = StockDisponible + {cantidad}
                WHERE Id = {idTorta} AND Activo = 1");
        }

        public void CancelarVenta(int idVenta, string motivo, string usuario)
        {
            var venta = _unitOfWork.VentaRepository.GetById(idVenta);

            if (venta == null)
                throw new Exception("Venta no encontrada.");

            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Cancelada)
                throw new Exception("Ya cancelada.");
            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Rechazada)
                throw new Exception("Una venta rechazada ya liberó su reserva de stock.");
            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Entregado)
                throw new Exception("No se puede cancelar una venta ya entregada.");

            var detalles = _unitOfWork.VentaDetalleRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var debeLiberarStock = venta.IdEstadoVenta != (int)EstadoVentaEnum.Rechazada &&
                                   venta.IdEstadoVenta != (int)EstadoVentaEnum.Cancelada &&
                                   venta.IdEstadoVenta != (int)EstadoVentaEnum.Entregado;

            foreach (var d in detalles)
            {
                if (debeLiberarStock)
                    LiberarStock(d.IdTorta, (int)d.Cantidad);

                if (debeLiberarStock)
                {
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
            return _unitOfWork.VentaRepository.GetAll().AsEnumerable()
                .Select(x => new
                {
                    x.Id,
                    x.FechaVenta,
                    x.Total,
                    x.IdEstadoVenta,
                    x.IdTipoEntrega,
                    x.UsuarioCreacion,
                    clienteNombre = ObtenerNombreCliente(x.IdPersona),
                    clienteTelefono = _unitOfWork.PersonaRepository.GetById(x.IdPersona)?.Telefono,
                    tienePersonalizacion = _unitOfWork.VentaDetalleRepository.GetBy(d => d.IdVenta == x.Id).Any(d =>
                        d.MensajePersonalizado != null || d.TamanoPersonalizado != null || d.SaborPersonalizado != null ||
                        d.RellenoPersonalizado != null || d.PisosPersonalizados != null || d.ColorDecoracionPersonalizada != null ||
                        d.DecoracionPersonalizada != null || d.CoberturaPersonalizada != null || d.PorcionesPersonalizadas != null || d.ImagenReferencia != null),
                    cantidadPersonalizadas = _unitOfWork.VentaDetalleRepository.GetBy(d => d.IdVenta == x.Id).Count(d =>
                        d.MensajePersonalizado != null || d.TamanoPersonalizado != null || d.SaborPersonalizado != null ||
                        d.RellenoPersonalizado != null || d.PisosPersonalizados != null || d.ColorDecoracionPersonalizada != null ||
                        d.DecoracionPersonalizada != null || d.CoberturaPersonalizada != null || d.PorcionesPersonalizadas != null || d.ImagenReferencia != null),
                    fechaEntregaSolicitada = _unitOfWork.VentaDetalleRepository.GetBy(d => d.IdVenta == x.Id).Where(d => d.FechaEntregaSolicitada != null).Select(d => d.FechaEntregaSolicitada).FirstOrDefault()
                }).ToList();
        }

        private string ObtenerNombreCliente(int idPersona)
        {
            var persona = _unitOfWork.PersonaRepository.GetById(idPersona);
            return persona == null ? "Cliente" : string.Join(" ", new[] { persona.Nombres, persona.ApellidoPaterno }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        public object Detalle(int idVenta)
        {
            var venta = _unitOfWork.VentaRepository.GetById(idVenta);
            if (venta == null)
                throw new Exception("Venta no encontrada.");
            var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);

            var detalles = _unitOfWork.VentaDetalleRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var pagos = _unitOfWork.PagoVentaRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var delivery = _unitOfWork.EntregaDeliveryRepository
                .GetBy(x => x.IdVenta == idVenta).FirstOrDefault();

            var comp = _unitOfWork.ComprobanteVentaRepository
                .GetBy(x => x.IdVenta == idVenta).FirstOrDefault();
            var tortas = detalles.Select(x => x.IdTorta).Distinct()
                .ToDictionary(id => id, id => _unitOfWork.TortaRepository.GetById(id)?.Nombre ?? "Torta");
            var metodos = pagos.Select(x => x.IdMetodoPago).Distinct()
                .ToDictionary(id => id, id => _unitOfWork.MetodoPagoRepository.GetById(id)?.Nombre ?? "Método de pago");

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
                     venta.UsuarioCreacion,
                     venta.ImagenComprobante,
                     venta.MontoPagado,
                     venta.SaldoPendiente,
                     venta.CodigoEntrega
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
                    torta = tortas[d.IdTorta],
                    d.Cantidad,
                    d.PrecioBase,
                    d.PrecioPersonalizacion,
                    d.PrecioFinal,
                    d.SubTotal,
                    tamanio = d.TamanoPersonalizado,
                    sabor = d.SaborPersonalizado,
                    relleno = d.RellenoPersonalizado,
                     pisos = d.PisosPersonalizados,
                     colorDecoracion = d.ColorDecoracionPersonalizada,
                     decoracion = d.DecoracionPersonalizada,
                     cobertura = d.CoberturaPersonalizada,
                     porciones = d.PorcionesPersonalizadas,
                     evento = d.EventoPersonalizado,
                     fechaEntrega = d.FechaEntregaSolicitada,
                     observaciones = d.ObservacionesPersonalizacion,
                     imagenReferencia = d.ImagenReferencia,
                     mensaje = d.MensajePersonalizado,
                 }).ToList(),
                pagos = pagos.Select(p => new
                {
                    p.IdMetodoPago,
                    nombreMetodo = metodos[p.IdMetodoPago],
                    p.Monto,
                    p.NumeroOperacion
                }).ToList(),
                delivery = delivery == null ? null : new
                {
                    delivery.Id,
                    delivery.IdVenta,
                    delivery.IdEstadoEntrega,
                    delivery.IdPersonalRepartidor,
                    nombreRepartidor = delivery.IdPersonalRepartidor.HasValue
                        ? ObtenerNombrePersona(delivery.IdPersonalRepartidor.Value)
                        : null,
                    delivery.Direccion,
                    delivery.Referencia,
                    telefono = delivery.TelefonoContacto,
                    delivery.NombreContacto,
                    delivery.CostoDelivery,
                    delivery.Latitud,
                    delivery.Longitud,
                    delivery.FechaAsignacion,
                    delivery.FechaAceptacion,
                    delivery.FechaInicio,
                    delivery.FechaEntrega,
                    delivery.UsuarioAsignacion
                },
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
            if (venta == null)
                throw new Exception("Venta no encontrada.");

            var comp = _unitOfWork.ComprobanteVentaRepository
                .GetBy(x => x.IdVenta == idVenta).FirstOrDefault();

            if (comp == null)
                throw new Exception("El comprobante todavía no ha sido generado. Emítelo primero.");

            var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);

            var detalles = _unitOfWork.VentaDetalleRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

            var pagos = _unitOfWork.PagoVentaRepository
                .GetBy(x => x.IdVenta == idVenta).ToList();

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

                 TipoComprobante = comp.IdTipoComprobante == 2 ? "Factura" : "Boleta",
                 SerieNumero = $"{comp.Serie}-{comp.Numero}",

                TipoEntrega = venta.IdTipoEntrega == 1 ? "Tienda" : "Delivery",
                Direccion = delivery?.Direccion,

                Detalles = detalles.Select(x => new DetalleComprobanteDTO
                {
                    Torta = _unitOfWork.TortaRepository.GetById(x.IdTorta).Nombre,
                    Cantidad = (int)x.Cantidad,
                    PrecioUnitario = x.PrecioFinal,
                    SubTotal = x.SubTotal
                    ,Tamanio = x.TamanoPersonalizado
                    ,Sabor = x.SaborPersonalizado
                    ,Relleno = x.RellenoPersonalizado
                     ,Pisos = x.PisosPersonalizados
                     ,ColorDecoracion = x.ColorDecoracionPersonalizada
                     ,Decoracion = x.DecoracionPersonalizada
                     ,Cobertura = x.CoberturaPersonalizada
                     ,Porciones = x.PorcionesPersonalizadas
                     ,Evento = x.EventoPersonalizado
                     ,FechaEntrega = x.FechaEntregaSolicitada
                     ,Observaciones = x.ObservacionesPersonalizacion
                     ,ImagenReferencia = x.ImagenReferencia
                     ,Mensaje = x.MensajePersonalizado
                 }).ToList(),

                 Pagos = pagos.Select(x => new PagoComprobanteDTO
                {
                    Metodo = _unitOfWork.MetodoPagoRepository.GetById(x.IdMetodoPago).Nombre,
                    Monto = x.Monto
                 }).ToList(),
                 Empresa = new EmpresaComprobanteDTO
                 {
                     Nombre = "Tortas Yani",
                     Ruc = "10789234567",
                     Direccion = "Av. Los Geranios 456, Lima",
                     Telefono = "987 654 321"
                 }
             };
        }

        public void MarcarEntregado(int idVenta, string usuario)
        {
            var venta = _unitOfWork.VentaRepository.GetById(idVenta);
            if (venta == null)
                throw new Exception("Venta no encontrada.");
            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Entregado)
                return;
            if (venta.IdEstadoVenta != (int)EstadoVentaEnum.Pagada &&
                venta.IdEstadoVenta != (int)EstadoVentaEnum.Aprobada)
                throw new Exception("Solo se puede entregar un pedido con el pago validado.");
            if (venta.MontoPagado < (venta.Total ?? 0))
                throw new Exception("No se puede entregar un pedido con saldo pendiente.");

            var entrega = _unitOfWork.EntregaDeliveryRepository.GetBy(x => x.IdVenta == idVenta).FirstOrDefault();
            if (venta.IdTipoEntrega == (int)TipoEntregaEnum.Delivery &&
                (entrega == null || entrega.IdEstadoEntrega != (int)EstadoEntregaEnum.Entregado))
                throw new Exception("El delivery debe estar marcado como Entregado antes de cerrar el pedido.");

            var estadoAnterior = venta.IdEstadoVenta;
            venta.IdEstadoVenta = (int)EstadoVentaEnum.Entregado;
            venta.FechaModificacion = Fecha.Hoy;
            venta.UsuarioModificacion = usuario;
            _unitOfWork.VentaRepository.Update(venta);

            var comprobante = _unitOfWork.ComprobanteVentaRepository.GetBy(x => x.IdVenta == idVenta).FirstOrDefault();
            if (comprobante == null)
            {
                var ultimoNumero = _unitOfWork.ComprobanteVentaRepository.GetAll()
                    .Where(x => x.Activo && x.Serie == "B001")
                    .Select(x => x.Numero)
                    .AsEnumerable()
                    .Select(x => int.TryParse(x, out var numero) ? numero : 0)
                    .DefaultIfEmpty(0)
                    .Max();

                _unitOfWork.ComprobanteVentaRepository.Add(new TComprobanteVenta
                {
                    IdVenta = idVenta,
                    IdTipoComprobante = 1,
                    Serie = "B001",
                    Numero = (ultimoNumero + 1).ToString("D8"),
                    FechaEmision = Fecha.Hoy,
                    Activo = true,
                    UsuarioCreacion = usuario,
                    FechaCreacion = Fecha.Hoy
                });
            }

_unitOfWork.Commit();
            _unitOfWork.VentaRepository.AgregarHistorial(new VentaHistorialDTO
            {
                IdVenta = idVenta,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = (int)EstadoVentaEnum.Entregado,
                Accion = "Entregado",
                Observacion = "Pedido entregado y comprobante generado",
                Usuario = usuario
            });
            _unitOfWork.Commit();
        }

        public ComprobanteVenta EmitirComprobante(int idVenta, int idTipoComprobante, string usuario)
        {
            var venta = _unitOfWork.VentaRepository.GetById(idVenta);
            if (venta == null)
                throw new Exception("Venta no encontrada.");
            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Cancelada)
                throw new Exception("No se puede emitir comprobante para una venta cancelada.");
            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Rechazada)
                throw new Exception("No se puede emitir comprobante para una venta rechazada.");

            var comprobanteExistente = _unitOfWork.ComprobanteVentaRepository.GetBy(x => x.IdVenta == idVenta).FirstOrDefault();
            if (comprobanteExistente != null)
            {
                return new ComprobanteVenta
                {
                    Id = comprobanteExistente.Id,
                    IdVenta = comprobanteExistente.IdVenta,
                    IdTipoComprobante = comprobanteExistente.IdTipoComprobante,
                    Serie = comprobanteExistente.Serie,
                    Numero = comprobanteExistente.Numero,
                    FechaEmision = comprobanteExistente.FechaEmision,
                    Activo = comprobanteExistente.Activo,
                    UsuarioCreacion = comprobanteExistente.UsuarioCreacion,
                    FechaCreacion = comprobanteExistente.FechaCreacion
                };
            }

            var serie = idTipoComprobante == 2 ? "F001" : "B001";
            var ultimoNumero = _unitOfWork.ComprobanteVentaRepository.GetAll()
                .Where(x => x.Activo && x.Serie == serie)
                .Select(x => x.Numero)
                .AsEnumerable()
                .Select(x => int.TryParse(x, out var numero) ? numero : 0)
                .DefaultIfEmpty(0)
                .Max();

            var comprobante = new TComprobanteVenta
            {
                IdVenta = idVenta,
                IdTipoComprobante = idTipoComprobante,
                Serie = serie,
                Numero = (ultimoNumero + 1).ToString("D8"),
                FechaEmision = Fecha.Hoy,
                Activo = true,
                UsuarioCreacion = usuario,
                FechaCreacion = Fecha.Hoy
            };

            _unitOfWork.ComprobanteVentaRepository.Add(comprobante);
            _unitOfWork.Commit();

            _unitOfWork.VentaRepository.AgregarHistorial(new VentaHistorialDTO
            {
                IdVenta = idVenta,
                IdEstadoAnterior = venta.IdEstadoVenta,
                IdEstadoNuevo = venta.IdEstadoVenta,
                Accion = "ComprobanteEmitido",
                Observacion = $"Comprobante {serie}-{comprobante.Numero} emitido",
                Usuario = usuario
            });
            _unitOfWork.Commit();

            return new ComprobanteVenta
            {
                Id = comprobante.Id,
                IdVenta = comprobante.IdVenta,
                IdTipoComprobante = comprobante.IdTipoComprobante,
                Serie = comprobante.Serie,
                Numero = comprobante.Numero,
                FechaEmision = comprobante.FechaEmision,
                Activo = comprobante.Activo,
                UsuarioCreacion = comprobante.UsuarioCreacion,
                FechaCreacion = comprobante.FechaCreacion
            };
        }

        public DeliveryPaginacionDTO ObtenerListadoDeliveries(int pagina = 1, int tamanioPagina = 6, int idEstadoEntrega = 0)
        {
            pagina = Math.Max(1, pagina);
            tamanioPagina = Math.Clamp(tamanioPagina, 1, 24);

            var deliveries = _unitOfWork.EntregaDeliveryRepository.GetAll()
                .Where(x => x.Activo)
                .ToList();

            var ventasDelivery = deliveries
                .Select(d => new { Delivery = d, Venta = _unitOfWork.VentaRepository.GetById(d.IdVenta) })
                .Where(x => x.Venta != null && x.Venta.Activo &&
                    x.Venta.IdEstadoVenta != (int)EstadoVentaEnum.Rechazada &&
                    x.Venta.IdEstadoVenta != (int)EstadoVentaEnum.Cancelada &&
                    (idEstadoEntrega <= 0 || x.Delivery.IdEstadoEntrega == idEstadoEntrega))
                .OrderByDescending(x => x.Venta.FechaVenta)
                .ThenByDescending(x => x.Venta.Id)
                .ToList();

            var result = new List<object>();
            foreach (var item in ventasDelivery)
            {
                var d = item.Delivery;
                var venta = item.Venta!;

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
                     venta.SubTotal,
                     venta.Total,
                     venta.MontoPagado,
                     venta.SaldoPendiente,
                     venta.RequiereAnticipo,
                     codigoEntrega = venta.CodigoEntrega,
                     idEstadoVenta = venta.IdEstadoVenta,
                     estadoVenta = _unitOfWork.EstadoVentaRepository.GetById(venta.IdEstadoVenta)?.Nombre ?? "Desconocido",
                     productos = ObtenerProductosDelivery(venta.Id),
                    d.IdEstadoEntrega,
                    estado = estado != null ? estado.Nombre : "Desconocido",
                    d.FechaAsignacion,
                    d.FechaAceptacion,
                    d.FechaInicio,
                    d.FechaEntrega,
                    d.IdPersonalRepartidor,
                    repartidorNombre = d.IdPersonalRepartidor.HasValue
                        ? ObtenerNombrePersona(d.IdPersonalRepartidor.Value)
                        : null,
                    d.Latitud,
                    d.Longitud,
                    d.UsuarioAsignacion,
                    fechaUltimaActualizacion = d.FechaModificacion ?? d.FechaCreacion
                 });
            }

            var totalRegistros = result.Count;
            var totalPaginas = Math.Max(1, (int)Math.Ceiling(totalRegistros / (double)tamanioPagina));
            pagina = Math.Min(pagina, totalPaginas);

            return new DeliveryPaginacionDTO
            {
                Items = result.Skip((pagina - 1) * tamanioPagina).Take(tamanioPagina).ToList(),
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                PaginaActual = pagina,
                TamanioPagina = tamanioPagina
            };
        }

        public IEnumerable<object> ObtenerListadoRecojos()
        {
            return _unitOfWork.VentaRepository.GetAll().AsEnumerable()
                .Where(x => x.Activo && x.IdTipoEntrega == (int)TipoEntregaEnum.RecojoTienda)
                .OrderByDescending(x => x.FechaVenta)
                .Select(venta => new
                {
                    id = venta.Id,
                    venta.FechaVenta,
                    venta.Total,
                    venta.MontoPagado,
                    venta.SaldoPendiente,
                    venta.IdEstadoVenta,
                    codigoEntrega = venta.CodigoEntrega,
                    cliente = ObtenerNombreCliente(venta.IdPersona),
                    documento = _unitOfWork.PersonaRepository.GetById(venta.IdPersona)?.NumeroDocumento,
                    detalles = _unitOfWork.VentaDetalleRepository.GetBy(x => x.IdVenta == venta.Id && x.Activo)
                        .Select(d => new
                        {
                            d.IdTorta,
                            torta = _unitOfWork.TortaRepository.GetById(d.IdTorta)?.Nombre ?? "Torta",
                            d.Cantidad,
                            d.SubTotal,
                            d.TamanoPersonalizado,
                            d.SaborPersonalizado,
                            d.RellenoPersonalizado,
                            d.FechaEntregaSolicitada
                        }).ToList()
                 }).ToList();
        }

        public void ActualizarEstadoDelivery(int idDelivery, int idEstadoEntrega, string usuario)
        {
            var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
            if (delivery == null)
                throw new Exception("Delivery no encontrado.");

            if (idEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
            {
                CompletarEntrega(idDelivery, usuario);
                return;
            }

            var transicionValida =
                (delivery.IdEstadoEntrega == (int)EstadoEntregaEnum.Pendiente && idEstadoEntrega == (int)EstadoEntregaEnum.Asignado) ||
                (delivery.IdEstadoEntrega == (int)EstadoEntregaEnum.Asignado && idEstadoEntrega == (int)EstadoEntregaEnum.Aceptado) ||
                (delivery.IdEstadoEntrega == (int)EstadoEntregaEnum.Aceptado && idEstadoEntrega == (int)EstadoEntregaEnum.EnCamino);

            if (!transicionValida)
                throw new Exception("La transición de delivery no es válida.");

            delivery.IdEstadoEntrega = idEstadoEntrega;
            delivery.FechaModificacion = Fecha.Hoy;
            delivery.UsuarioModificacion = usuario;

            if (idEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
            {
                delivery.FechaEntrega = Fecha.Hoy;
            }
            else if (idEstadoEntrega == (int)EstadoEntregaEnum.EnCamino)
            {
                delivery.FechaInicio = Fecha.Hoy;
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

            delivery.IdEstadoEntrega = (int)EstadoEntregaEnum.Asignado;
            delivery.IdPersonalRepartidor = idPersonalRepartidor;
            delivery.FechaAsignacion = Fecha.Hoy;
            delivery.UsuarioAsignacion = usuario;
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
            return MapearMisPedidos(_unitOfWork.VentaRepository.GetBy(x => x.IdPersona == idPersona && x.Activo)
                .OrderByDescending(x => x.FechaCreacion));
        }

        public MisPedidosPaginacionDTO ObtenerMisPedidosPaginado(int idPersona, int pagina = 1, int tamanioPagina = 6)
        {
            pagina = Math.Max(1, pagina);
            tamanioPagina = Math.Clamp(tamanioPagina, 1, 24);

            var ventas = _unitOfWork.VentaRepository.GetBy(x => x.IdPersona == idPersona && x.Activo)
                .OrderByDescending(x => x.FechaCreacion)
                .ToList();
            var totalRegistros = ventas.Count;
            var totalPaginas = Math.Max(1, (int)Math.Ceiling(totalRegistros / (double)tamanioPagina));
            pagina = Math.Min(pagina, totalPaginas);

            return new MisPedidosPaginacionDTO
            {
                Items = MapearMisPedidos(ventas.Skip((pagina - 1) * tamanioPagina).Take(tamanioPagina)),
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                PaginaActual = pagina,
                TamanioPagina = tamanioPagina
            };
        }

        private List<MisPedidosDTO> MapearMisPedidos(IEnumerable<TVenta> ventas)
        {
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
                var comprobante = _unitOfWork.ComprobanteVentaRepository
                    .GetBy(x => x.IdVenta == venta.Id).FirstOrDefault();

                result.Add(new MisPedidosDTO
                {
                    Id = venta.Id,
                    Fecha = venta.FechaCreacion,
                    Total = venta.Total ?? 0,
                    MontoPagado = venta.MontoPagado,
                    SaldoPendiente = venta.SaldoPendiente,
                    RequiereAnticipo = venta.RequiereAnticipo,
                     EstadoPago = estadoPago?.Nombre ?? "Desconocido",
                     CodigoEntrega = venta.CodigoEntrega,
                     IdEstadoVenta = venta.IdEstadoVenta,
                    IdEstadoEntrega = entrega?.IdEstadoEntrega,
                    Productos = nombresTortas,
                    Cantidad = (int)detalles.Sum(x => x.Cantidad),
                    TipoEntrega = venta.IdTipoEntrega == 1 ? "Recojo en tienda" : "Delivery",
                    DeliveryEstado = deliveryEstado,
                    DeliveryDireccion = entrega?.Direccion,
                    DeliveryTelefono = entrega?.TelefonoContacto,
                    MetodoPago = string.Join(", ", pagos.Select(p => {
                        var metodo = _unitOfWork.MetodoPagoRepository.GetById(p.IdMetodoPago);
                        return metodo?.Nombre ?? "N/A";
                    })),
                    TipoComprobante = comprobante?.IdTipoComprobante == 2 ? "Factura" : comprobante == null ? null : "Boleta",
                    SerieNumeroComprobante = comprobante == null ? null : $"{comprobante.Serie}-{comprobante.Numero}"
                });
            }

            return result;
        }

        public void CompletarRecojo(CompletarRecojoDTO dto)
        {
            var venta = _unitOfWork.VentaRepository.GetById(dto.IdVenta)
                ?? throw new Exception("Pedido no encontrado.");
            var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona)
                ?? throw new Exception("Cliente no encontrado.");

            if (venta.IdTipoEntrega != (int)TipoEntregaEnum.RecojoTienda)
                throw new Exception("El pedido no corresponde a recojo en tienda.");
            if (!string.Equals(venta.CodigoEntrega?.Trim(), dto.CodigoEntrega?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new Exception("El código de entrega no coincide.");
            if (!string.Equals(persona.NumeroDocumento?.Trim(), dto.DocumentoCliente?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new Exception("El documento del cliente no coincide.");
            if (venta.IdEstadoVenta == (int)EstadoVentaEnum.Cancelada || venta.IdEstadoVenta == (int)EstadoVentaEnum.Rechazada)
                throw new Exception("El pedido no puede ser entregado.");

            var saldo = decimal.Round(Math.Max(0, (venta.Total ?? 0) - venta.MontoPagado), 2);
            var cobro = decimal.Round(Math.Max(0, dto.MontoCobrado), 2);
            if (cobro != saldo)
                throw new Exception($"Debe cobrar exactamente el saldo pendiente: S/ {saldo:0.00}.");
            if (cobro > 0)
            {
                if (dto.IdMetodoPago != (int)MetodoPagoEnum.Efectivo)
                    throw new Exception("El saldo de un recojo en tienda debe cobrarse en efectivo.");
                _unitOfWork.PagoVentaRepository.Add(new TPagoVenta
                {
                    IdVenta = venta.Id,
                    IdMetodoPago = dto.IdMetodoPago,
                    Monto = cobro,
                    FechaPago = Fecha.Hoy,
                    Activo = true,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = Fecha.Hoy
                });
                venta.MontoPagado = decimal.Round(venta.MontoPagado + cobro, 2);
            }

            if (venta.MontoPagado < (venta.Total ?? 0))
                throw new Exception("El pedido debe estar completamente pagado antes de entregarlo.");

            var estadoAnterior = venta.IdEstadoVenta;
            venta.SaldoPendiente = 0;
            venta.IdEstadoVenta = (int)EstadoVentaEnum.Entregado;
            venta.FechaModificacion = Fecha.Hoy;
            venta.UsuarioModificacion = dto.Usuario;
            _unitOfWork.VentaRepository.Update(venta);

            if (estadoAnterior == (int)EstadoVentaEnum.Pendiente)
            {
                foreach (var detalle in _unitOfWork.VentaDetalleRepository.GetAll().Where(x => x.IdVenta == venta.Id && x.Activo))
                {
                    _unitOfWork.MovimientoTortaRepository.Add(new TMovimientoTorta
                    {
                        IdTorta = detalle.IdTorta,
                        IdTipoMovimiento = (int)TipoMovimientoEnum.Venta,
                        Cantidad = detalle.Cantidad,
                        FechaMovimiento = Fecha.Hoy,
                        Referencia = $"Venta #{venta.Id} - Recojo",
                        Activo = true,
                        UsuarioCreacion = dto.Usuario,
                        FechaCreacion = Fecha.Hoy
                    });
                }
            }
            _unitOfWork.Commit();
            _unitOfWork.VentaRepository.AgregarHistorial(new VentaHistorialDTO
            {
                IdVenta = venta.Id,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = (int)EstadoVentaEnum.Entregado,
                Accion = "Recogido en tienda",
                Observacion = "Pedido validado con código y documento; pago completado",
                Usuario = dto.Usuario
            });
            _unitOfWork.Commit();
        }

        public void ValidarCodigoDelivery(int idDelivery, string codigo, string documento)
        {
            var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery)
                ?? throw new Exception("Delivery no encontrado.");
            var venta = _unitOfWork.VentaRepository.GetById(delivery.IdVenta)
                ?? throw new Exception("Venta asociada no encontrada.");
            var persona = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);
            if (venta.IdTipoEntrega != (int)TipoEntregaEnum.Delivery)
                throw new Exception("El pedido no corresponde a delivery.");
            if (!string.Equals(venta.CodigoEntrega?.Trim(), codigo?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new Exception("El código de entrega no coincide.");
            if (persona == null || !string.Equals(persona.NumeroDocumento?.Trim(), documento?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new Exception("El documento del cliente no coincide.");
        }

        private string GenerarCodigoEntrega()
        {
            const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            for (var intento = 0; intento < 10; intento++)
            {
                var bytes = RandomNumberGenerator.GetBytes(6);
                var codigo = new string(bytes.Select(x => caracteres[x % caracteres.Length]).ToArray());
                if (!_unitOfWork.VentaRepository.GetAll().Any(x => x.CodigoEntrega == codigo))
                    return codigo;
            }
            throw new Exception("No se pudo generar un código de entrega único.");
        }

        public void CancelarEntrega(int idDelivery, string motivo, string usuario)
        {
            try
            {
                var entrega = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
                if (entrega == null)
                    throw new Exception("Delivery no encontrado.");

                if (entrega.IdEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
                    throw new Exception("No se puede cancelar un pedido ya entregado.");

                if (entrega.IdEstadoEntrega == (int)EstadoEntregaEnum.Cancelado)
                    throw new Exception("El pedido ya está cancelado.");

                entrega.IdEstadoEntrega = (int)EstadoEntregaEnum.Cancelado;
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
                        MontoPagado = v.MontoPagado,
                        SaldoPendiente = v.SaldoPendiente,
                        RequiereAnticipo = v.RequiereAnticipo,
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
                var estadoAprobado = venta.MontoPagado >= (venta.Total ?? 0)
                    ? EstadoVentaEnum.Pagada
                    : EstadoVentaEnum.Aprobada;
                venta.IdEstadoVenta = (int)estadoAprobado;
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
                    IdEstadoNuevo = (int)estadoAprobado,
                    Accion = "Aprobada",
                    Observacion = estadoAprobado == EstadoVentaEnum.Pagada
                        ? "Pago validado y pedido pagado completamente"
                        : "Comprobante validado; queda saldo pendiente para completar la entrega",
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

                var detalles = _unitOfWork.VentaDetalleRepository.GetAll()
                    .Where(d => d.IdVenta == idVenta && d.Activo)
                    .ToList();
                foreach (var detalle in detalles)
                {
                    LiberarStock(detalle.IdTorta, (int)detalle.Cantidad);
                    _unitOfWork.MovimientoTortaRepository.Add(new TMovimientoTorta
                    {
                        IdTorta = detalle.IdTorta,
                        IdTipoMovimiento = (int)TipoMovimientoEnum.Anulacion,
                        Cantidad = detalle.Cantidad,
                        FechaMovimiento = Fecha.Hoy,
                        Referencia = $"Rechazo Venta #{idVenta}",
                        UsuarioCreacion = usuario,
                        Activo = true
                    });
                }
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
                        venta.SubTotal,
                        venta.Total,
                        venta.MontoPagado,
                        saldoPorCobrar = venta.SaldoPendiente,
                        venta.SaldoPendiente,
                        productos = ObtenerProductosDelivery(venta.Id),
                        d.IdEstadoEntrega,
                        estado = estado != null ? estado.Nombre : "Desconocido",
                        d.FechaAsignacion,
                        d.FechaAceptacion,
                        d.FechaInicio,
                        d.FechaEntrega,
                        d.Latitud,
                        d.Longitud,
                        d.UsuarioAsignacion,
                        fechaUltimaActualizacion = d.FechaModificacion ?? d.FechaCreacion,
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
                delivery.FechaAceptacion = Fecha.Hoy;
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
                delivery.FechaInicio = Fecha.Hoy;
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

                if (delivery.IdEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
                    return;

                var venta = _unitOfWork.VentaRepository.GetById(delivery.IdVenta);
                if (venta == null)
                    throw new Exception("Venta asociada no encontrada.");
                if (delivery.IdEstadoEntrega != (int)EstadoEntregaEnum.EnCamino)
                    throw new Exception("El pedido debe estar en camino para completarlo.");

                var saldo = Math.Max(0, (venta.SaldoPendiente > 0
                    ? venta.SaldoPendiente
                    : (venta.Total ?? 0) - venta.MontoPagado));
                if (saldo > 0)
                    throw new Exception("Este pedido requiere registrar primero el cobro del saldo pendiente.");

                FinalizarEntrega(delivery, venta, usuario);
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

        public void CompletarEntrega(int idDelivery, string usuario, decimal? montoCobrado, int idMetodoPago)
        {
            if (idMetodoPago == (int)MetodoPagoEnum.Efectivo && (montoCobrado ?? 0) > 0)
                throw new Exception("Delivery solo acepta pagos digitales.");
            var delivery = _unitOfWork.EntregaDeliveryRepository.GetById(idDelivery);
            if (delivery == null)
                throw new Exception("Delivery no encontrado.");
            if (delivery.IdEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
                return;

            var venta = _unitOfWork.VentaRepository.GetById(delivery.IdVenta)
                ?? throw new Exception("Venta asociada no encontrada.");
            var saldo = Math.Max(0, venta.SaldoPendiente > 0
                ? venta.SaldoPendiente
                : (venta.Total ?? 0) - venta.MontoPagado);
            var cobro = decimal.Round(Math.Max(0, montoCobrado ?? 0), 2);
            if (cobro + 0.01m < saldo)
                throw new Exception($"Debe cobrar el saldo pendiente completo: S/ {saldo:0.00}.");
            if (cobro > saldo + 0.01m)
                throw new Exception($"El monto cobrado no puede superar el saldo pendiente: S/ {saldo:0.00}.");
            if (cobro > 0)
            {
                var metodo = _unitOfWork.MetodoPagoRepository.GetById(idMetodoPago);
                if (metodo == null)
                    throw new Exception("El método de pago del cobro no es válido.");
                _unitOfWork.PagoVentaRepository.Add(new PagoVenta
                {
                    IdVenta = venta.Id,
                    IdMetodoPago = idMetodoPago,
                    Monto = cobro,
                    FechaPago = Fecha.Hoy,
                    UsuarioCreacion = usuario,
                    FechaCreacion = Fecha.Hoy,
                    Activo = true
                });
                venta.MontoPagado = decimal.Round(venta.MontoPagado + cobro, 2);
                venta.SaldoPendiente = decimal.Round(Math.Max(0, (venta.Total ?? 0) - venta.MontoPagado), 2);
            }
            if (venta.SaldoPendiente > 0)
                throw new Exception("Este pedido requiere registrar primero el cobro del saldo pendiente.");

            if (delivery.IdEstadoEntrega != (int)EstadoEntregaEnum.EnCamino)
                throw new Exception("El pedido debe estar en camino para completarlo.");

            // Finaliza usando las mismas instancias que se usaron para registrar el cobro.
            // Volver a consultar/adjuntar la venta en este DbContext causa tracking duplicado.
            FinalizarEntrega(delivery, venta, usuario);
        }

        private void FinalizarEntrega(EntregaDelivery delivery, Venta venta, string usuario)
        {
            if (delivery.IdEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
                return;

            var estadoAnterior = venta.IdEstadoVenta;
            venta.IdEstadoVenta = (int)EstadoVentaEnum.Entregado;
            venta.FechaModificacion = Fecha.Hoy;
            venta.UsuarioModificacion = usuario;

            delivery.IdEstadoEntrega = (int)EstadoEntregaEnum.Entregado;
            delivery.FechaEntrega = Fecha.Hoy;
            delivery.FechaModificacion = Fecha.Hoy;
            delivery.UsuarioModificacion = usuario;

            _unitOfWork.VentaRepository.Update(venta);
            _unitOfWork.EntregaDeliveryRepository.Update(delivery);

            var comprobante = _unitOfWork.ComprobanteVentaRepository
                .GetBy(x => x.IdVenta == venta.Id).FirstOrDefault();
            if (comprobante == null)
            {
                var ultimoNumero = _unitOfWork.ComprobanteVentaRepository.GetAll()
                    .Where(x => x.Activo && x.Serie == "B001")
                    .Select(x => x.Numero)
                    .AsEnumerable()
                    .Select(x => int.TryParse(x, out var numero) ? numero : 0)
                    .DefaultIfEmpty(0)
                    .Max();

                _unitOfWork.ComprobanteVentaRepository.Add(new TComprobanteVenta
                {
                    IdVenta = venta.Id,
                    IdTipoComprobante = 1,
                    Serie = "B001",
                    Numero = (ultimoNumero + 1).ToString("D8"),
                    FechaEmision = Fecha.Hoy,
                    Activo = true,
                    UsuarioCreacion = usuario,
                    FechaCreacion = Fecha.Hoy
                });
            }

            _unitOfWork.Commit();
            _unitOfWork.VentaRepository.AgregarHistorial(new VentaHistorialDTO
            {
                IdVenta = venta.Id,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = (int)EstadoVentaEnum.Entregado,
                Accion = "Entregado",
                Observacion = "Pedido entregado y comprobante generado",
                Usuario = usuario
            });
            _unitOfWork.Commit();
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
                        d.CostoDelivery,
                        venta.Total,
                        venta.SaldoPendiente,
                        productos = ObtenerProductosDelivery(venta.Id),
                        repartidorId = d.IdPersonalRepartidor,
                        repartidorNombre = repartidor != null ? repartidor.ApellidoPaterno + " " + repartidor.Nombres : "Sin asignar",
                        d.IdEstadoEntrega,
                        estado = estado != null ? estado.Nombre : "Desconocido",
                        d.FechaAsignacion,
                        d.FechaAceptacion,
                        d.FechaInicio,
                        d.FechaEntrega,
                        d.Latitud,
                        d.Longitud,
                        d.UsuarioAsignacion,
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

        public object ObtenerHistorialRepartidor(int idPersona, int pagina = 1, int tamanioPagina = 8)
        {
            pagina = Math.Max(1, pagina);
            tamanioPagina = Math.Clamp(tamanioPagina, 1, 50);
            var deliveries = _unitOfWork.EntregaDeliveryRepository.GetAll()
                .Where(x => x.IdPersonalRepartidor == idPersona && x.Activo)
                .OrderByDescending(x => x.FechaEntrega ?? x.FechaAsignacion ?? x.FechaCreacion)
                .ToList();
            var total = deliveries.Count;
            var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamanioPagina));
            pagina = Math.Min(pagina, totalPaginas);
            return new
            {
                items = deliveries.Skip((pagina - 1) * tamanioPagina).Take(tamanioPagina)
                    .Select(d => MapearDeliveryRepartidor(d)).ToList(),
                totalRegistros = total,
                totalPaginas,
                paginaActual = pagina,
                tamanioPagina
            };
        }

        public object ObtenerGananciasRepartidor(int idPersona)
        {
            var entregas = _unitOfWork.EntregaDeliveryRepository.GetAll()
                .Where(x => x.IdPersonalRepartidor == idPersona && x.Activo &&
                            x.IdEstadoEntrega == (int)EstadoEntregaEnum.Entregado)
                .ToList();
            var hoy = Fecha.Hoy.Date;
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + 1);
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            decimal Sumar(Func<TEntregaDelivery, bool> filtro) => entregas.Where(filtro).Sum(x => x.CostoDelivery ?? 0);
            return new
            {
                total = Sumar(_ => true),
                hoy = Sumar(x => x.FechaEntrega?.Date == hoy),
                semana = Sumar(x => x.FechaEntrega?.Date >= inicioSemana),
                mes = Sumar(x => x.FechaEntrega?.Date >= inicioMes),
                entregasCompletadas = entregas.Count,
                promedio = entregas.Count == 0 ? 0 : Math.Round(Sumar(_ => true) / entregas.Count, 2)
            };
        }

        private string ObtenerNombrePersona(int idPersona)
        {
            var persona = _unitOfWork.PersonaRepository.GetById(idPersona);
            return persona == null ? "Sin asignar" : $"{persona.ApellidoPaterno} {persona.Nombres}".Trim();
        }

        private List<object> ObtenerProductosDelivery(int idVenta)
        {
            return _unitOfWork.VentaDetalleRepository.GetBy(x => x.IdVenta == idVenta && x.Activo)
                .Select(d => (object)new
                {
                    idTorta = d.IdTorta,
                    producto = _unitOfWork.TortaRepository.GetById(d.IdTorta)?.Nombre ?? "Producto",
                    cantidad = d.Cantidad,
                    precioUnitario = d.PrecioFinal ?? ((d.PrecioBase ?? 0) + (d.PrecioPersonalizacion ?? 0)),
                    subtotal = d.SubTotal ?? 0,
                    tamanio = d.TamanoPersonalizado,
                    sabor = d.SaborPersonalizado,
                    relleno = d.RellenoPersonalizado,
                    pisos = d.PisosPersonalizados,
                    colorDecoracion = d.ColorDecoracionPersonalizada,
                    decoracion = d.DecoracionPersonalizada,
                    cobertura = d.CoberturaPersonalizada,
                    porciones = d.PorcionesPersonalizadas,
                    evento = d.EventoPersonalizado,
                    fechaEntrega = d.FechaEntregaSolicitada,
                    observaciones = d.ObservacionesPersonalizacion,
                    imagenReferencia = d.ImagenReferencia,
                    mensaje = d.MensajePersonalizado
                }).ToList();
        }

        private object MapearDeliveryRepartidor(TEntregaDelivery d)
        {
            var venta = _unitOfWork.VentaRepository.GetById(d.IdVenta)!;
            var cliente = _unitOfWork.PersonaRepository.GetById(venta.IdPersona);
            return new
            {
                d.Id,
                d.IdVenta,
                venta.FechaVenta,
                cliente = cliente == null ? "Cliente" : $"{cliente.ApellidoPaterno} {cliente.Nombres}".Trim(),
                clienteTelefono = cliente?.Telefono,
                d.Direccion,
                d.Referencia,
                d.TelefonoContacto,
                d.NombreContacto,
                d.CostoDelivery,
                venta.SubTotal,
                venta.Total,
                venta.MontoPagado,
                venta.SaldoPendiente,
                codigoEntrega = venta.CodigoEntrega,
                productos = ObtenerProductosDelivery(venta.Id),
                d.IdEstadoEntrega,
                estado = ObtenerEstadoEntrega(d.IdEstadoEntrega)?.Nombre ?? "Desconocido",
                d.FechaAsignacion,
                d.FechaAceptacion,
                d.FechaInicio,
                d.FechaEntrega,
                d.Latitud,
                d.Longitud,
                d.UsuarioAsignacion,
                fechaUltimaActualizacion = d.FechaModificacion ?? d.FechaCreacion
            };
        }
    }
}
