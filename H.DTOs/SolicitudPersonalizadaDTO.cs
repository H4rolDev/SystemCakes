namespace H.DTOs;

public class CrearSolicitudPersonalizadaDTO
{
    public string? NombreCliente { get; set; }
    public string? EmailCliente { get; set; }
    public string? TelefonoCliente { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? ImagenReferencia { get; set; }
    public string? Evento { get; set; }
    public string? Sabor { get; set; }
    public string? Relleno { get; set; }
    public string? Tamano { get; set; }
    public int? Porciones { get; set; }
    public int Pisos { get; set; } = 1;
    public string? Cobertura { get; set; }
    public string? Colores { get; set; }
    public string? TextoDecorativo { get; set; }
    public DateTime? FechaEntregaSolicitada { get; set; }
    public decimal? PresupuestoMinimo { get; set; }
    public decimal? PresupuestoMaximo { get; set; }
}

public class CotizarSolicitudDTO
{
    public decimal PrecioFinal { get; set; }
    public decimal Adelanto { get; set; }
    public decimal CostoDelivery { get; set; }
    public DateTime FechaEntrega { get; set; }
    public TimeSpan? HoraEntrega { get; set; }
    public int VigenciaHoras { get; set; } = 48;
    public string? Observaciones { get; set; }
}

public class CambiarEstadoSolicitudDTO
{
    public string Estado { get; set; } = string.Empty;
    public string? Comentario { get; set; }
}
