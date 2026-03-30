namespace H.DTOs
{
    public class TortaListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdCategoriaTorta { get; set; }
        public string NombreCategoriaTorta { get; set; }
        public string Descripcion { get; set; }
        public string Cantidades { get; set; }
        public decimal PrecioVenta { get; set; }
        public string ImagenUrl { get; set; }
        public string ImagenPublicId { get; set; }
        public bool EsPersonalizable { get; set; }
        public int? StockDisponible { get; set; }
        public string? Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}