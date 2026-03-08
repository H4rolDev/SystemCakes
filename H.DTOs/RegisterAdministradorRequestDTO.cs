using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DTOs
{
    public class RegisterAdministradorRequestDTO
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
}
