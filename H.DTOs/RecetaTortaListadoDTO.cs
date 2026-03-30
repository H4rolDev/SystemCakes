namespace H.DTOs
{
    public class RecetaTortaListadoDTO
    {
        public int Id { get; set; }
        public int IdTorta { get; set; }
        public string NombreTorta { get; set; }
        public int IdInsumo { get; set; }
        public string NombreInsumo { get; set; }
        public decimal CantidadRequerida { get; set; }
        public bool Activo { get; set; }
        public string UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}