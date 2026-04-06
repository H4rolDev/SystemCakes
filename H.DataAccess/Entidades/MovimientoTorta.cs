using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class MovimientoTorta : BaseEntity
    {
        public int IdTipoMovimiento { get; set; }
        public int IdTorta { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string? Referencia { get; set; }
    }
}
