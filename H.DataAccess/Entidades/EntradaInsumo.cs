using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class EntradaInsumo : BaseEntity
    {
        public int? IdProveedor { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string? ImagenDocumento { get; set; }
        public string? Observaciones { get; set; }
        public int IdEstado { get; set; }
        public string? MotivoRechazo { get; set; }
    }
}