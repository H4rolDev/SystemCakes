using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Enums
{
    public enum EstadoVentaEnum
    {
        Pendiente = 1,
        EsperandoValidacion = 2,
        Aprobada = 3,
        Rechazada = 4,
        Pagada = 5,
        Cancelada = 6
    }
}
