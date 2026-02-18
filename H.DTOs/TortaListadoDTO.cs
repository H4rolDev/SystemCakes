namespace H.DTOs
{
    public class TortaListadoDTO
    {
        public int Id { get; set; }
        public int Nombre { get; set; }
        public string Descripcion { get; set; }
        public string PrecioVenta { get; set; }
        public string? StockDisponible { get; set; }
        public string? Estado { get; set; }
    }
}