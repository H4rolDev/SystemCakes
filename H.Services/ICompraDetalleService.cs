using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface ICompraDetalleService
    {
        int Add(CompraDetalle entidad);
        int Update(CompraDetalle entidad);
        int Delete(int id, string usuario);
        CompraDetalle GetById(int id);
        IEnumerable<CompraDetalleListadoDTO> ObtenerCombo();
        public int AddMultipleTabla(CompraInsertDTO dto);
    }
}
