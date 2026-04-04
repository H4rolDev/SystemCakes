namespace H.DTOs
{
    public class InsumoListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdUnidadMedida { get; set; }
        public string NombreUnidadMedida { get; set; }
        public string Abreviatura { get; set; }
        public decimal StockDisponible { get; set; }
        public bool Activo { get; set; }
    }
}