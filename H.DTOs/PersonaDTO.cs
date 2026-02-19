using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DTOs
{
    public class PersonaDTO
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public string Nombres { get; set; } = null!;
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? RazonSocial { get; set; }
    }
}
