namespace H.DTOs
{
    public class InsertarVentaDTO
    {
        public int IdPersona { get; set; }
        public int IdTipoEntrega { get; set; }
        public string Usuario { get; set; }

        public string? ImagenComprobante { get; set; }
        public string? NumeroOperacion { get; set; }

        public List<DetalleVentaDTO> Detalles { get; set; }
        public List<PagoVentaDTO> Pagos { get; set; }
        public EntregaDTO? Entrega { get; set; }
        public ComprobanteDTO? Comprobante { get; set; }
    }

    public class DetalleVentaDTO
    {
        public int IdTorta { get; set; }
        public int Cantidad { get; set; }

        public decimal PrecioBase { get; set; }
        public decimal PrecioPersonalizacion { get; set; }

        public string? Mensaje { get; set; }
        public string? Tamanio { get; set; }
        public string? Sabor { get; set; }
        public string? Relleno { get; set; }
        public int? Pisos { get; set; }
        public string? ColorDecoracion { get; set; }
        public string? Decoracion { get; set; }
        public string? Cobertura { get; set; }
        public int? Porciones { get; set; }
        public string? Evento { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? Observaciones { get; set; }
        public string? ImagenReferencia { get; set; }
    }

    public class PagoVentaDTO
    {
        public int IdMetodoPago { get; set; }
        public decimal Monto { get; set; }
        public string? NumeroOperacion { get; set; }
    }

    public class EntregaDTO
    {
        public int? IdPersonalRepartidor { get; set; }
        public string Direccion { get; set; }
        public string? Referencia { get; set; }
        public string Telefono { get; set; }
        public string? NombreContacto { get; set; }
        public decimal CostoDelivery { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
    }

    public class CompletarEntregaDTO
    {
        public int IdDelivery { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public decimal? MontoCobrado { get; set; }
        public int IdMetodoPago { get; set; } = 1;
    }

    public class HistorialRepartidorPaginacionDTO
    {
        public List<object> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
        public int TamanioPagina { get; set; }
    }

    public class DeliveryPaginacionDTO
    {
        public List<object> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
        public int TamanioPagina { get; set; }
    }

    public class ComprobanteDTO
    {
        public int IdTipoComprobante { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
    }
public class EmitirComprobanteDTO
    {
        public int IdVenta { get; set; }
        public int IdTipoComprobante { get; set; }
        public string Usuario { get; set; }
    }

    public class VentaComprobanteDTO
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }

        public string TipoComprobante { get; set; }
        public string SerieNumero { get; set; }

        public List<DetalleComprobanteDTO> Detalles { get; set; }

        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }

        public string TipoEntrega { get; set; }
        public string? Direccion { get; set; }

        public List<PagoComprobanteDTO> Pagos { get; set; }
        public EmpresaComprobanteDTO Empresa { get; set; }
    }

    public class EmpresaComprobanteDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Ruc { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }

    public class DetalleComprobanteDTO
    {
        public string Torta { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? SubTotal { get; set; }
        public string? Tamanio { get; set; }
        public string? Sabor { get; set; }
        public string? Relleno { get; set; }
        public int? Pisos { get; set; }
        public string? ColorDecoracion { get; set; }
        public string? Mensaje { get; set; }
        public string? Decoracion { get; set; }
        public string? Cobertura { get; set; }
        public int? Porciones { get; set; }
        public string? Evento { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? Observaciones { get; set; }
        public string? ImagenReferencia { get; set; }
    }

    public class PagoComprobanteDTO
    {
        public string Metodo { get; set; }
        public decimal Monto { get; set; }
    }

    public class MisPedidosDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public bool RequiereAnticipo { get; set; }
        public bool TienePersonalizacion { get; set; }
        public int CantidadPersonalizadas { get; set; }
        public DateTime? FechaEntregaSolicitada { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string? ClienteTelefono { get; set; }
        public string EstadoPago { get; set; } = string.Empty;
        public string Productos { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public string? CodigoEntrega { get; set; }
        public int IdEstadoVenta { get; set; }
        public int? IdEstadoEntrega { get; set; }
        public string? DeliveryEstado { get; set; }
        public string? DeliveryDireccion { get; set; }
        public string? DeliveryTelefono { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? TipoComprobante { get; set; }
        public string? SerieNumeroComprobante { get; set; }
    }

    public class MisPedidosPaginacionDTO
    {
        public List<MisPedidosDTO> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
        public int TamanioPagina { get; set; }
    }

    public class ActualizarPersonaDTO
    {
        public int Id { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
    }

    public class VentaValidacionDTO
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public string Cliente { get; set; } = null!;
        public string ClienteTelefono { get; set; } = null!;
        public decimal Total { get; set; }
        public string? NumeroOperacion { get; set; }
        public string? ImagenComprobante { get; set; }
        public string Estado { get; set; } = null!;
        public int IdEstadoVenta { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public bool RequiereAnticipo { get; set; }
        public List<VentaDetalleValidacionDTO> Detalles { get; set; } = new();
    }

    public class VentaDetalleValidacionDTO
    {
        public int IdTorta { get; set; }
        public string Torta { get; set; } = null!;
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
    }

    public class AprobarRechazarVentaDTO
    {
        public int IdVenta { get; set; }
        public string Usuario { get; set; } = null!;
        public string? MotivoRechazo { get; set; }
    }

    public class CompletarRecojoDTO
    {
        public int IdVenta { get; set; }
        public string CodigoEntrega { get; set; } = string.Empty;
        public string DocumentoCliente { get; set; } = string.Empty;
        public decimal MontoCobrado { get; set; }
        public int IdMetodoPago { get; set; } = 1;
        public string Usuario { get; set; } = string.Empty;
    }

    public class SubirImagenDTO
    {
        public string ImagenBase64 { get; set; } = null!;
    }

    // ============================================
    // DTOs para Filtros e Historial de Ventas
    // ============================================

    public class VentaFiltroDTO
    {
        public int? IdEstado { get; set; }
        public int? IdPersona { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NumeroOperacion { get; set; }
        public string? Busqueda { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanioPagina { get; set; } = 20;
    }

    public class VentaPaginacionDTO
    {
        public List<VentaListadoDTO> Items { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
    }

    public class VentaListadoDTO
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public string Cliente { get; set; }
        public decimal? Total { get; set; }
        public string? NumeroOperacion { get; set; }
        public int IdEstadoVenta { get; set; }
        public string Estado { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public bool RequiereAnticipo { get; set; }
    }

    public class VentaHistorialDTO
    {
        public int Id { get; set; }
        public int IdVenta { get; set; }
        public int? IdEstadoAnterior { get; set; }
        public int IdEstadoNuevo { get; set; }
        public string Accion { get; set; }
        public string? Observacion { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }
}
