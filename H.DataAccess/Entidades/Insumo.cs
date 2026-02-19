using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class Insumo: BaseEntity
    {
        public string Nombre { get; set; }
        public string? UnidadMedida { get; set; }
        public decimal? StockActual { get; set; }
        public decimal? StockMinimo { get; set; }
        public decimal? CostoUnitario { get; set; }
    }
}
