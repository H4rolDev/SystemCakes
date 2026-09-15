namespace H.DataAccess.Models;

public class TSolicitudHistorial : BaseEntity
{
    public int IdSolicitud { get; set; }
    public string? EstadoAnterior { get; set; }
    public string EstadoNuevo { get; set; } = string.Empty;
    public string? Comentario { get; set; }
}
