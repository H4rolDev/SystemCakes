using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Entidades
{
    public class EntregaDelivery : BaseEntity
    {
        public int IdVenta {  get; set; }
        public int IdEstadoEntrega { get; set; }
        public int IdPersonalRepartior { get; set; }
        public string Direccion { get; set; }
        public string? Referencia { get; set; }
        public string? TelefonoContacto { get; set; }
        public string? NombreContacto { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public decimal? CostoDelivery { get; set; }
        public int? IdPersonalRepartidor { get; set; }
    }
}
