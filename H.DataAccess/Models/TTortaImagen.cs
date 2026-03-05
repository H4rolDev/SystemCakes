using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TTortaImagen
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador de categoria de la torta.
        /// </summary>
        public decimal IdTorta { get; set; }
        /// <summary>
        /// Url de imagen.
        /// </summary>
        public string ImagenUrl { get; set; } = null!;
        /// <summary>
        /// Imagen public Id.
        /// </summary>
        public string ImagenPublicId { get; set; } = null!;
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
