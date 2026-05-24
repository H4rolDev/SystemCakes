using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class Venta : BaseEntity
    {
        public int IdPersona {  get; set; }
        public int IdEstadoVenta { get; set; }
        public int IdTipoEntrega {  get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Descuenta { get; set; }
        public decimal? Total { get; set; }
        public string? Observacion { get; set; }
        public string? ImagenComprobante { get; set; }
        public string? NumeroOperacion { get; set; }
        public string? MotivoRechazo { get; set; }
    }
}
