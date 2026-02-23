using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TTorta
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string Nombre { get; set; } = null!;
        /// <summary>
        /// Descripcion de la categoria.
        /// </summary>
        public string Descripcion { get; set; } = null!;
        /// <summary>
        /// Descripcion de la categoria.
        /// </summary>
        public decimal PrecioVenta { get; set; }
        /// <summary>
        /// Descripcion de la categoria.
        /// </summary>
        public decimal StockDisponible { get; set; }
        public string? ImagenUrl { get; set; }
        public string? ImagenPublicId { get; set; }
        /// <summary>
        /// Estado del registro
        /// </summary>
        public bool Estado { get; set; }
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
