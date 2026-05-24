using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class CancelacionVenta : BaseEntity
    {
        public int IdVenta {  get; set; }
        public string Motivo { get; set; }
        public DateTime FechaCancelacion { get; set; }
    }
}
