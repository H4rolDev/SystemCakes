namespace H.DTOs
{
    public class RecetaTortaDTO
    {
        public int Id { get; set; }
        public int IdInsumo { get; set; }
        public string Insumo { get; set; }
        public int IdTorta { get; set; }
        public string Torta { get; set; }
        public decimal CantidadNecesaria { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }

    }
}
