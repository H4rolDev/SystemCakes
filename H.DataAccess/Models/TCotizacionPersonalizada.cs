namespace H.DataAccess.Models;

public class TCotizacionPersonalizada : BaseEntity
{
    public int IdSolicitud { get; set; }
    public int Version { get; set; }
    public decimal PrecioFinal { get; set; }
    public decimal Adelanto { get; set; }
    public decimal CostoDelivery { get; set; }
    public DateTime FechaEntrega { get; set; }
    public TimeSpan? HoraEntrega { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string? Observaciones { get; set; }
    public string Estado { get; set; } = "Enviada";
}
