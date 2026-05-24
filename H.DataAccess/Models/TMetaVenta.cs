using System;

namespace H.DataAccess.Models
{
    public partial class TMetaVenta : H.DataAccess.BaseEntity
    {
        public int Anio { get; set; }
        public decimal MetaAnual { get; set; }
    }
}