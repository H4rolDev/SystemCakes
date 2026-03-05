using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class TortaImagen : BaseEntity
    {
        public decimal IdTorta {  get; set; }
        public string ImagenUrl { get; set; }
        public string ImagenPublicId {  get; set; }
        public string EsPrincipal { get; set; }
    }
}
