using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DataAccess.Repositorios;
using H.DTOs;

namespace H.DataAccess.Repositorios
{
    public interface IVentaRepository : IGenericRepository<TVenta>
    {
        TVenta Add(Venta entidad);
        int Delete(int id, string usuario);
        TVenta Update(Venta entidad);
        Venta GetById(int id);
        public IEnumerable<ComboDTO> ObtenerComboMetodoPago();
        public IEnumerable<ComboDTO> ObtenerComboTipoComprobante();
        public IEnumerable<ComboDTO> ObtenerComboPersonal();
        VentaPaginacionDTO ObtenerTodos(VentaFiltroDTO filtro);
        IEnumerable<VentaHistorialDTO> ObtenerHistorial(int idVenta);
        void AgregarHistorial(VentaHistorialDTO historial);
    }
}
