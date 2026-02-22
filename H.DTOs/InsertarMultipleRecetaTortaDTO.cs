namespace H.DTOs
{
    public class InsertarMultipleRecetaTortaDTO
    {
        public string UsuarioCreacion { get; set; }
        public int IdTorta { get; set; }
        public List<InsumoDetalleInsertDTO> Detalles { get; set; }
    }

    public class InsumoDetalleInsertDTO
    {
        public int IdInsumo { get; set; }
        public decimal CantidadNecesaria { get; set; }
    }
}