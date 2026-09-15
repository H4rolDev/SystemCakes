namespace H.DataAccess.Models;

public class TSolicitudPersonalizada : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public int IdPersona { get; set; }
    public string? NombreCliente { get; set; }
    public string? EmailCliente { get; set; }
    public string? TelefonoCliente { get; set; }
    public string TokenAcceso { get; set; } = string.Empty;
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
    public decimal? EstimadoMinimo { get; set; }
    public decimal? EstimadoMaximo { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string? Observaciones { get; set; }
}
