using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class Torta : BaseEntity
    {
        public decimal IdCategoriaTorta {  get; set; }
        public string Nombre { get; set; }
        public string Descripcion {  get; set; }
        public string Cantidades { get; set; }
        public decimal PrecioVenta { get; set; }
        public bool EsPersonalizable { get; set; }
    }
}
