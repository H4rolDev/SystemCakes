using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class UnidadMedida : BaseEntity
    {
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
    }
}
