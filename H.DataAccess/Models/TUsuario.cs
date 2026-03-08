using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TUsuario
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador del persona.
        /// </summary>
        public int IdPersona { get; set; }
        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password hash de usuario.
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// Password salt de usuario.
        /// </summary>
        public string PasswordSalt { get; set; }
        /// <summary>
        /// Estado del registro.
        /// </summary>
        public bool Activo { get; set; }
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
