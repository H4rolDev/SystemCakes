namespace H.DTOs
{
    public class InsertarLoteInsumoDTO
    {
        public string Nombre { get; set; }
        public int idLote { get; set; }
        public int IdUnidadMedida { get; set; }
        public int IdInsumo { get; set; }
        public string? NumeroLote { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal CantidadInicial { get; set; }
        public decimal CantidadDisponible { get; set; }
        public decimal CostoUnitario { get; set; }
        public string Usuario { get; set; }
    }
}