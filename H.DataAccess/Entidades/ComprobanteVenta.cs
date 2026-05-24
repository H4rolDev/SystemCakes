using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class ComprobanteVenta : BaseEntity
    {
        public int IdVenta {  get; set; }
        public int IdTipoComprobante { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public DateTime? FechaEmision { get; set; }
    }
}
