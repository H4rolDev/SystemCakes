using H.DataAccess.Entidades;
using H.DataAccess.Extension;
using H.DataAccess.Helpers;
using H.DataAccess.UnitofWork;
using H.DTOs;
using H.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class VentaController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public VentaController(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            this.unitOfWork = unitOfWork;
            this._cloudinaryService = cloudinaryService;
        }

        [HttpPost("SubirImagen")]
        public async Task<IActionResult> SubirImagen([FromBody] SubirImagenDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.ImagenBase64))
                    return BadRequest(new { message = "La imagen es requerida" });

                var base64Data = dto.ImagenBase64;
                var mimeType = "image/jpeg";
                if (base64Data.Contains(","))
                {
                    var parts = base64Data.Split(',');
                    if (parts.Length >= 2)
                    {
                        mimeType = parts[0].Contains("image/png") ? "image/png" : "image/jpeg";
                        base64Data = parts[1];
                    }
                }

                var bytes = Convert.FromBase64String(base64Data);
                var stream = new MemoryStream(bytes);
                var fileName = $"comprobante_{DateTime.Now.Ticks}.jpg";
                
                var formFile = new FormFile(stream, 0, stream.Length, "file", fileName);

                var url = await _cloudinaryService.SubirImagenAsync(formFile, "comprobantes");
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("Insertar")]
        public IActionResult Insert([FromBody] Venta categoria)
        {
            try
            {
                var servicio = new VentaService(unitOfWork);
                categoria.FechaCreacion = Fecha.Hoy;
                categoria.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Add(categoria);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Modificar")]
        public IActionResult Update([FromBody] Venta categoria)
        {
            try
            {
                var servicio = new VentaService(unitOfWork);
                categoria.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Update(categoria);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpDelete("Eliminar")]
        public IActionResult Delete(int id, string usuario)
        {
            try
            {
                var servicio = new VentaService(unitOfWork);
                var respuesta = servicio.Delete(id, usuario);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerListadoPorId")]
        public IActionResult ObtenerListadoPorId(int id)
        {
            try
            {
                var servicio = new VentaService(unitOfWork);
                return Ok(servicio.GetById(id));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerComboMetodoPago")]
        public IActionResult ObtenerComboMetodoPago()
        {
            try
            {
                var servicio = new VentaService(unitOfWork);
                return Ok(servicio.ObtenerComboMetodoPago());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerComboTipoComprobante")]
        public IActionResult ObtenerComboTipoComprobante()
        {
            try
            {
                var servicio = new VentaService(unitOfWork);
                return Ok(servicio.ObtenerComboTipoComprobante());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerComboPersonal")]
        public IActionResult ObtenerComboPersonal()
        {
            try
            {
                var servicio = new VentaService(unitOfWork);
                return Ok(servicio.ObtenerComboPersonal());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("Registrar")]
        public IActionResult RegistrarVenta([FromBody] InsertarVentaDTO dto)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                var id = service.RegistrarVenta(dto);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("Listado")]
        ////[Authorize(Roles = "Administrador,Atención")]
        public IActionResult Listado()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.Listado());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("Comprobante")]
        public IActionResult Comprobante(int idVenta)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerComprobante(idVenta));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("Detalle")]
        public IActionResult Detalle(int idVenta)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.Detalle(idVenta));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Cancelar")]
        public IActionResult Cancelar(int idVenta, string motivo, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.CancelarVenta(idVenta, motivo, usuario);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ListadoDeliveries")]
        //[Authorize(Roles = "Administrador,Atención,Repartidor")]
        public IActionResult ListadoDeliveries(int pagina = 1, int tamanioPagina = 6, int idEstadoEntrega = 0)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerListadoDeliveries(pagina, tamanioPagina, idEstadoEntrega));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("ActualizarEstadoDelivery")]
        //[Authorize(Roles = "Administrador,Atención,Repartidor")]
        public IActionResult ActualizarEstadoDelivery(int idDelivery, int idEstadoEntrega, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.ActualizarEstadoDelivery(idDelivery, idEstadoEntrega, usuario);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ComboEstadoEntrega")]
        public IActionResult ComboEstadoEntrega()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerComboEstadoEntrega());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ComboTipoEntrega")]
        public IActionResult ComboTipoEntrega()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerComboTipoEntrega());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("MisPedidos")]
        public IActionResult MisPedidos(int idPersona)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerMisPedidos(idPersona));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ListadoRecojos")]
        public IActionResult ListadoRecojos()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerListadoRecojos());
            }
            catch (Exception ex) { return new ErrorResult(ex, User); }
        }

        [HttpPost("CompletarRecojo")]
        public IActionResult CompletarRecojo([FromBody] CompletarRecojoDTO dto)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.CompletarRecojo(dto);
                return Ok(new { success = true, message = "Pedido recogido correctamente." });
            }
            catch (Exception ex) { return new ErrorResult(ex, User); }
        }

        [HttpPost("ValidarCodigoDelivery")]
        public IActionResult ValidarCodigoDelivery(int idDelivery, string codigo, string documento)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.ValidarCodigoDelivery(idDelivery, codigo, documento);
                return Ok(new { success = true, message = "Delivery validado correctamente." });
            }
            catch (Exception ex) { return new ErrorResult(ex, User); }
        }

        [HttpPost("SubirImagenReferencia")]
        public async Task<IActionResult> SubirImagenReferencia([FromBody] SubirImagenDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.ImagenBase64))
                    return BadRequest(new { message = "La imagen de referencia es requerida." });
                var base64Data = dto.ImagenBase64;
                var mimeType = "image/jpeg";
                if (base64Data.Contains(','))
                {
                    var parts = base64Data.Split(',', 2);
                    mimeType = parts[0].Contains("png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg";
                    base64Data = parts[1];
                }
                var bytes = Convert.FromBase64String(base64Data);
                await using var stream = new MemoryStream(bytes);
                var formFile = new FormFile(stream, 0, stream.Length, "file", $"referencia_{DateTime.UtcNow.Ticks}.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = mimeType
                };
                var url = await _cloudinaryService.SubirImagenAsync(formFile, "referencias-tortas");
                return Ok(new { url });
            }
            catch (Exception ex) { return new ErrorResult(ex, User); }
        }

        [HttpGet("MisPedidosPaginado")]
        public IActionResult MisPedidosPaginado(int idPersona, int pagina = 1, int tamanioPagina = 6)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerMisPedidosPaginado(idPersona, pagina, tamanioPagina));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ComboDrivers")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult ComboDrivers()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerComboDrivers());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ComboClientes")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult ComboClientes()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerComboClientes());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("AsignarDriver")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult AsignarDriver(int idDelivery, int idDriver, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.AsignarDriver(idDelivery, idDriver, usuario);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("CancelarEntrega")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult CancelarEntrega(int idDelivery, string motivo, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.CancelarEntrega(idDelivery, motivo, usuario);
                return Ok(new { success = true, message = "Entrega cancelada correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("AsignarRepartidor")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult AsignarRepartidor(int idDelivery, int idPersonalRepartidor, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.AsignarRepartidor(idDelivery, idPersonalRepartidor, usuario);
                return Ok(new { success = true, message = "Repartidor asignado correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerPendientesValidacion")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult ObtenerPendientesValidacion()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                var result = service.ObtenerVentasPendientesValidacion();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("AprobarVenta")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult AprobarVenta([FromBody] AprobarRechazarVentaDTO dto)
        {
            try
            {
                if (dto.IdVenta <= 0)
                    return BadRequest(new { success = false, message = "ID de venta inválido" });

                var service = new VentaService(unitOfWork);
                service.AprobarVenta(dto.IdVenta, dto.Usuario);
                return Ok(new { success = true, message = "Venta aprobada correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

[HttpPost("RechazarVenta")]
        //[Authorize(Roles = "Administrador,Atención")]
        public IActionResult RechazarVenta([FromBody] AprobarRechazarVentaDTO dto)
        {
            try
            {
                if (dto.IdVenta <= 0)
                    return BadRequest(new { success = false, message = "ID de venta inválido" });
                if (string.IsNullOrWhiteSpace(dto.MotivoRechazo))
                    return BadRequest(new { success = false, message = "El motivo de rechazo es requerido" });

                var service = new VentaService(unitOfWork);
                service.RechazarVenta(dto.IdVenta, dto.MotivoRechazo, dto.Usuario);
                return Ok(new { success = true, message = "Venta rechazada correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("EmitirComprobante")]
        public IActionResult EmitirComprobante([FromBody] EmitirComprobanteDTO dto)
        {
            try
            {
                if (dto?.IdVenta <= 0)
                    return BadRequest(new { success = false, message = "ID de venta inválido" });
                if (dto.IdTipoComprobante <= 0)
                    return BadRequest(new { success = false, message = "Tipo de comprobante requerido" });

                var service = new VentaService(unitOfWork);
                var comprobante = service.EmitirComprobante(dto.IdVenta, dto.IdTipoComprobante, dto.Usuario ?? "admin");
                return Ok(new { success = true, message = "Comprobante emitido correctamente", comprobante });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerTodos")]
        public IActionResult ObtenerTodos([FromQuery] VentaFiltroDTO filtro)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                var resultado = service.ObtenerTodos(filtro);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerHistorial")]
        public IActionResult ObtenerHistorial(int id)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                var historial = service.ObtenerHistorial(id);
                return Ok(historial);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        // ===================== NUEVOS ENDPOINTS PARA REPARTIDORES =====================

        [HttpGet("MisPedidosRepartidor")]
        public IActionResult MisPedidosRepartidor(int idPersona)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerMisPedidosRepartidor(idPersona));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("AceptarPedido")]
        public IActionResult AceptarPedido(int idDelivery, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.AceptarPedidoDelivery(idDelivery, usuario);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("IniciarDelivery")]
        public IActionResult IniciarDelivery(int idDelivery, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.IniciarDelivery(idDelivery, usuario);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("CompletarEntrega")]
        public IActionResult CompletarEntrega(int idDelivery, string usuario, decimal? montoCobrado = null, int idMetodoPago = 1)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.CompletarEntrega(idDelivery, usuario, montoCobrado, idMetodoPago);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("MarcarEntregado")]
        public IActionResult MarcarEntregado(int idVenta, string usuario = "admin")
        {
            try
            {
                new VentaService(unitOfWork).MarcarEntregado(idVenta, usuario);
                return Ok(new { success = true, message = "Pedido entregado y comprobante generado correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPost("DesasignarPedido")]
        public IActionResult DesasignarPedido(int idDelivery, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.DesasignarPedido(idDelivery, usuario);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("GestionRepartidores")]
        public IActionResult GestionRepartidores()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerGestionRepartidores());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("HistorialRepartidor")]
        public IActionResult HistorialRepartidor(int idPersona, int pagina = 1, int tamanioPagina = 8)
        {
            try
            {
                return Ok(new VentaService(unitOfWork).ObtenerHistorialRepartidor(idPersona, pagina, tamanioPagina));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("GananciasRepartidor")]
        public IActionResult GananciasRepartidor(int idPersona)
        {
            try
            {
                return Ok(new VentaService(unitOfWork).ObtenerGananciasRepartidor(idPersona));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}
