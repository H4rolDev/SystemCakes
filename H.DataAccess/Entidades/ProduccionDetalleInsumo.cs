using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class ProduccionDetalleInsumo : BaseEntity
    {
        public int IdProduccion {  get; set; }
        public int IdInsumo { get; set; }
        public decimal CantidadUsada {  get; set; } 
    }
}
