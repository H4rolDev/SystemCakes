using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class RecetaTorta: BaseEntity
    {
        public int IdTorta { get; set; }
        public int IdInsumo { get; set; }
        public int CantidadNecesaria { get; set; }
    }
}
