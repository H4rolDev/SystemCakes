namespace H.DTOs
{
    public class PersonalDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = null!;
        public int IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearPersonalRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int IdRol { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string UsuarioRegistra { get; set; } = null!;
    }

    public class ActualizarPersonalRequestDTO
    {
        public int Id { get; set; }
        public int? IdRol { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string UsuarioModifica { get; set; } = null!;
    }
}