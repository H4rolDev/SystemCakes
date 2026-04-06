namespace H.DTOs
{
    public class ProduccionCabeceraDTO
    {
        public int Id { get; set; }
        public int IdTorta { get; set; }
        public string NombreTorta { get; set; }
        public decimal CantidadProducida { get; set; }
        public DateTime FechaProduccion { get; set; }
        public string Observacion { get; set; }
        public bool Activo { get; set; }
        public string UsuarioCreacion { get; set; }
    }

    public class ProduccionDetalleDTO
    {
        public int Id { get; set; }
        public int IdProduccion { get; set; }
        public int IdInsumo { get; set; }
        public string NombreInsumo { get; set; }
        public decimal CantidadUsada { get; set; }
    }
}