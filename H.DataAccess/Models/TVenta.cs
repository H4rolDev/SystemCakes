using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Models
{
    public partial class TVenta
    {
        /// <summary>
        /// Identificador de registro.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public int IdPersona { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public int IdEstadoVenta { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public int IdTipoEntrega { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public DateTime? FechaVenta { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public decimal? SubTotal { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public decimal? Descuento { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public decimal? Total { get; set; }
        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public string? Observacion { get; set; }
        /// <summary>
        /// Imagen del comprobante de pago (URL de Cloudinary).
        /// </summary>
        public string? ImagenComprobante { get; set; }
        /// <summary>
        /// Número de operación de pago.
        /// </summary>
        public string? NumeroOperacion { get; set; }
        /// <summary>
        /// Motivo del rechazo del comprobante.
        /// </summary>
        public string? MotivoRechazo { get; set; }
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
