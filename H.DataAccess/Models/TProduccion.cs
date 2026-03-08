using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TProduccion
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador de la torta.
        /// </summary>
        public int IdTorta { get; set; }
        /// <summary>
        /// Fecha de la produccion.
        /// </summary>
        public DateTime FechaProduccion { get; set; }
        /// <summary>
        /// Cantridad producida de torta.
        /// </summary>
        public decimal CantidadProducida { get; set; }
        /// <summary>
        /// Observaciones de produccion.
        /// </summary>
        public string? Observacion { get; set; }
        /// <summary>
        /// Estado del registro
        /// </summary>
        public bool Activo { get; set; }
        /// <summary>
        /// Usuario de creación del registro
        /// </summary>
        public string UsuarioCreacion { get; set; } = null!;
        /// <summary>
        /// Usuario de modificación del registro
        /// </summary>
        public string UsuarioModificacion { get; set; } = null!;
        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        /// <summary>
        /// Fecha de modificación del registro
        /// </summary>
        public DateTime FechaModificacion { get; set; }
    }
}
