using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class PagoVenta : BaseEntity
    {
        public int IdVenta {  get; set; }
        public int IdMetodoPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? NumeroOperacion { get; set; }
    }
}
