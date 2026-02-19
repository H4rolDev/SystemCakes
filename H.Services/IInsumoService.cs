using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface IInsumoService
    {
        int Add(Insumo entidad);
        int Update(Insumo entidad);
        int Delete(int id, string usuario);
        Insumo GetById(int id);
        IEnumerable<InsumoListadoDTO> ObtenerCombo();
    }
}
