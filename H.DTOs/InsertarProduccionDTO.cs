namespace H.DTOs
{
    public class InsertarProduccionDTO
    {
        public int IdTorta { get; set; }
        public decimal CantidadProducida { get; set; }
        public string? Observacion { get; set; }
        public string UsuarioCreacion { get; set; }
    }

    public class UpdateProduccionDTO
    {
        public int IdProduccion { get; set; }
        public int CantidadProducida { get; set; }
        public string? Observacion { get; set; }
        public string UsuarioModificacion { get; set; }
    }

    public class AjusteInsumoDTO
    {
        public int IdInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public bool EsEntrada { get; set; }
        public string Usuario { get; set; }
        public string? Observacion { get; set; }
    }

    public class AjusteTortaDTO
    {
        public int IdTorta { get; set; }
        public decimal Cantidad { get; set; }
        public bool EsEntrada { get; set; }
        public string Usuario { get; set; }
        public string? Observacion { get; set; }
    }
}