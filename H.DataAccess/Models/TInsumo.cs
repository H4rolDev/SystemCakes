using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TInsumo
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del insumo.
        /// </summary>
        public string Nombre { get; set; } = null!;
        /// <summary>
        /// Unidad de medida.
        /// </summary>
        public string UnidadMedida { get; set; } = null!;
        /// <summary>
        /// Stock actual de insumo.
        /// </summary>
        public decimal StockActual { get; set; }
        /// <summary>
        /// Stock minimo de insumo.
        /// </summary>
        public decimal StockMinimo { get; set; }
        /// <summary>
        /// Costo unitario de insumo.
        /// </summary>
        public decimal CostoUnitario { get; set; }
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
