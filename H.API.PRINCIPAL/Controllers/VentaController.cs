using H.DataAccess.Entidades;
using H.DataAccess.Extension;
using H.DataAccess.Helpers;
using H.DataAccess.UnitofWork;
using H.DTOs;
using H.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult ListadoDeliveries()
        {
            try
            {
                var service = new VentaService(unitOfWork);
                return Ok(service.ObtenerListadoDeliveries());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("ActualizarEstadoDelivery")]
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

        [HttpGet("ComboDrivers")]
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
        public IActionResult CompletarEntrega(int idDelivery, string usuario)
        {
            try
            {
                var service = new VentaService(unitOfWork);
                service.CompletarEntrega(idDelivery, usuario);
                return Ok(true);
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
    }
}