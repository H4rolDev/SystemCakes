namespace H.DTOs
{
    public class PersonaListadoDTO
    {
        public int Id { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
    }
}
