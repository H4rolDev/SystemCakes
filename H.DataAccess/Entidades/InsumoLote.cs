using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class InsumoLote: BaseEntity
    {
        public int IdInsumo { get; set; }
        public string NumeroLote { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal? CantidadInicial { get; set; }
        public decimal? CntidadDisponible { get; set; }
        public decimal? CostoUnitario { get; set; }
    }
}
