using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class Torta : BaseEntity
    {
        public string Nombre { get; set; }
        public string Descripcion {  get; set; }
        public decimal PrecioVenta {  get; set; }
        public decimal StockDisponible {  get; set; }
        public string? ImagenUrl { get; set; }
        public string? ImagenPublicId { get; set; }
    }
}
