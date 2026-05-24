namespace H.DTOs
{
    public class RolDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearRolRequestDTO
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string UsuarioRegistra { get; set; } = null!;
    }

    public class ActualizarRolRequestDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string UsuarioModifica { get; set; } = null!;
    }
}