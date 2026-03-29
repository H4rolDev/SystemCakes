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
        /// Identificador de categoria de la torta.
        /// </summary>
        public int IdCategoriaTorta { get; set; }
        /// <summary>
        /// Nombre de la torta.
        /// </summary>
        public string Nombre { get; set; } = null!;
        /// <summary>
        /// Descripcion de la torta.
        /// </summary>
        public string? Descripcion { get; set; } = null!;
        /// <summary>
        /// Cantidades o porciones de la torta.
        /// </summary>
        public string? Cantidades { get; set; }
        /// <summary>
        /// Precio de venta de la torta.
        /// </summary>
        public string? PrecioVenta { get; set; }
        /// <summary>
        /// Indicador de si es perzonalizable o no.
        /// </summary>
        public bool? EsPersonalizable { get; set; }
        /// <summary>
        /// Url de la imagen
        /// </summary>
        public string? ImagenUrl { get; set; }
        /// <summary>
        /// Identificador de la imagen
        /// </summary>
        public string? ImagenPublicId { get; set; }
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
