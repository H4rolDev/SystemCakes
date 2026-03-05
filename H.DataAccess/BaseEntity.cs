namespace H.DataAccess
{
    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
        public bool Activo { get; set; }
        public string UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

    }
}
