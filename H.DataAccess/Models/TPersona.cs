using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TPersona
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public int IdUsuario { get; set; }
        /// <summary>
        /// Identificador del tipo de documento.
        /// </summary>
        public string TipoDocumento { get; set; }
        /// <summary>
        /// Numero de documento del usuario.
        /// </summary>
        public string NumeroDocumento { get; set; }
        /// <summary>   
        /// Razon social de la persona.
        /// </summary>
        public string RazonSocial { get; set; }
        /// <summary>   
        /// Nombres de la persona.
        /// </summary>
        public string Nombres { get; set; }
        /// <summary>
        /// Apellido paterno del usuario.
        /// </summary>
        public string ApellidoPaterno { get; set; }
        /// <summary>
        /// Apellido materno del usuario.
        /// </summary>
        public string ApellidoMaterno { get; set; }
        /// <summary>
        /// Telefono del usuario.
        /// </summary>
        public string? Telefono { get; set; }
        /// <summary>
        /// Direccion del usuario.
        /// </summary>
        public string? Direccion { get; set; }
        /// <summary>
        /// Estado del registro.
        /// </summary>
        public bool Estado { get; set; }
        /// <summary>
        /// Usuario de creación del registro
        /// </summary>
        public string UsuarioCreacion { get; set; } = null!;
        /// <summary>
        /// Usuario de modificación del registro
        /// </summary>
        public string? UsuarioModificacion { get; set; } = null!;
        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        /// <summary>
        /// Fecha de modificación del registro
        /// </summary>
        public DateTime? FechaModificacion { get; set; }
    }
}
