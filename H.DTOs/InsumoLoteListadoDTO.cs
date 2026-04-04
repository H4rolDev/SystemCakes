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
}