using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class TortaLote: BaseEntity
    {
        public string IdTorta { get; set; }
        public string NumeroLote { get; set; }
        public DateTime FechaProduccion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal CantidadInicial { get; set; }
        public decimal CantidaddDisponible { get; set; }
    }
}
