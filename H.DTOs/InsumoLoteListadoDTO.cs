namespace H.DTOs
{
    public class InsumoLoteDTO
    {
        public int Id { get; set; }
        public int IdInsumo { get; set; }
        public string NumeroLote { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal? CantidadInicial { get; set; }
        public decimal CantidadDisponible { get; set; }
        public decimal? CostoUnitario { get; set; }
        public bool Activo { get; set; }
    }

    public class InsumoLoteListadoDTO
    {
        public int Id { get; set; }
        public string NumeroLote { get; set; }
        public string Insumo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
    }
}