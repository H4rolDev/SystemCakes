using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TCliente
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Tipo de documento del cliente.
        /// </summary>
        public int IdTipoDocumento { get; set; }
        /// <summary>
        /// Numero de documento del cliente.
        /// </summary>
        public string NumeroDocumento { get; set; } = null!;
        /// <summary>
        /// Nombres del cliente.
        /// </summary>
        public string NombresPersona { get; set; }
        /// <summary>
        /// Apellidos del cliente.
        /// </summary>
        public string ApellidosPersona { get; set; } = null!;
        /// <summary>
        /// RazonSocial del cliente.
        /// </summary>
        public string RazonSocial { get; set; } = null!;
        /// <summary>
        /// Estado del registro.
        /// </summary>
        public bool Estado { get; set; }
        /// <summary>
        /// Usuario de creación del registro.
        /// </summary>
        public string UsuarioCreacion { get; set; } = null!;
        /// <summary>
        /// Usuario de modificación del registro.
        /// </summary>
        public string UsuarioModificacion { get; set; } = null!;
        /// <summary>
        /// Fecha de creación del registro.
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        /// <summary>
        /// Fecha de modificación del registro.
        /// </summary>
        public DateTime FechaModificacion { get; set; }
    }
}
