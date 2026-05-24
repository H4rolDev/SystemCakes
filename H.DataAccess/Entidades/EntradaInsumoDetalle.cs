using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class EntradaInsumoDetalle : BaseEntity
    {
        public int IdEntradaInsumo { get; set; }
        public int IdInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}