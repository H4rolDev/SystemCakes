namespace H.DTOs
{
    public class MovimientoTortaListadoDTO
    {
        public int Id { get; set; }
        public int IdTipoMovimiento { get; set; }
        public string TipoMovimiento { get; set; }
        public int IdTorta { get; set; }
        public string Torta { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Referencia { get; set; }
        public string? Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}