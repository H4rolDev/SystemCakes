namespace H.DTOs
{
    public class InsumoListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal CostoUnitario { get; set; }
    }
}