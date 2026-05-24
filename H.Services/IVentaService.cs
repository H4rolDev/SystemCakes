using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface IVentaService
    {
        int Add(Venta entidad);
        int Update(Venta entidad);
        int Delete(int id, string usuario);
        Venta GetById(int id);
        VentaPaginacionDTO ObtenerTodos(VentaFiltroDTO filtro);
        IEnumerable<VentaHistorialDTO> ObtenerHistorial(int idVenta);
    }
}
