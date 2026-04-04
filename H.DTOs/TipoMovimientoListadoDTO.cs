namespace H.DTOs
{
    public class TipoMovimientoListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}