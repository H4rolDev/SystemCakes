using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TEntradaInsumo
    {
        public int Id { get; set; }
        public int? IdProveedor { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string? ImagenDocumento { get; set; }
        public string? Observaciones { get; set; }
        public int IdEstado { get; set; }
        public string? MotivoRechazo { get; set; }
        public bool Activo { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioAprobacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string? UsuarioRechazo { get; set; }
        public DateTime? FechaRechazo { get; set; }
    }
}