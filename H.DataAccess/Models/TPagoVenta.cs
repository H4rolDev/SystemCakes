using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TPagoVenta
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public int IdVenta { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public int IdMetodoPago { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public decimal Monto { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public DateTime FechaPago { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public string? NumeroOperacion { get; set; }
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
