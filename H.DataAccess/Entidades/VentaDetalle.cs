using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class VentaDetalle : BaseEntity
    {
        public int IdVenta {  get; set; }
        public int IdTorta { get; set; }
        public decimal Cantidad {  get; set; }
        public string? MensajePersonalizado { get; set; }
        public string? SaborPersonalizado { get; set; }
        public string? TamanoPersonalizado { get; set; }
        public string? DecoracionPersonalizada { get; set; }
        public string? ObservacionesPersonalizadas { get; set; }
        public string? RellenoPersonalizado { get; set; }
        public int? PisosPersonalizados { get; set; }
        public string? ColorDecoracionPersonalizada { get; set; }
        public string? CoberturaPersonalizada { get; set; }
        public int? PorcionesPersonalizadas { get; set; }
        public string? EventoPersonalizado { get; set; }
        public DateTime? FechaEntregaSolicitada { get; set; }
        public string? ImagenReferencia { get; set; }
        public decimal? PrecioBase { get; set; }
        public decimal? PrecioPersonalizacion { get; set; }
        public decimal? PrecioFinal { get; set; }
        public decimal? SubTotal { get; set; }
    }
}
