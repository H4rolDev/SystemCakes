using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class Cliente: BaseEntity
    {
        public int IdTipoDocumento { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? NombresPersona { get; set; }
        public string? ApellidosPersona { get; set; }
        public string? RazonSocial { get; set; }
    }
}
