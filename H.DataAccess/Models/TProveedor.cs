using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TProveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Ruc { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Contacto { get; set; }
        public bool Activo { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}