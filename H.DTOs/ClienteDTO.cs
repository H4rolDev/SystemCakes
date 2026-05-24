namespace H.DTOs
{
    public class ClienteDTO
    {
        public int IdUsuario { get; set; }
        public int IdPersona { get; set; }
        public string Username { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearClienteRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string UsuarioRegistra { get; set; } = null!;
    }

    public class ActualizarClienteRequestDTO
    {
        public int IdUsuario { get; set; }
        public int IdPersona { get; set; }
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