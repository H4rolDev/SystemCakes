using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TTortaLote
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
        /// Numero de lote registrado.
        /// </summary>
        public string NumeroLote { get; set; } = null!;
        /// <summary>
        /// Fecha de vencimiento de torta lote.
        /// </summary>
        public DateTime FechaProduccion { get; set; }
        /// <summary>
        /// Fecha de vencimiento de torta lote.
        /// </summary>
        public DateTime? FechaVencimiento { get; set; }
        /// <summary>
        /// Cantidad inicial de torta lote.
        /// </summary>
        public decimal CantidadInicial { get; set; }
        /// <summary>
        /// Cantidad disponible de torta lote.
        /// </summary>
        public decimal CantidaddDisponible { get; set; }
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
