using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface IUnidadMedidaService
    {
        int Add(UnidadMedida entidad);
        int Update(UnidadMedida entidad);
        int Delete(int id, string usuario);
        UnidadMedida GetById(int id);
        IEnumerable<UnidadMedidaListadoDTO> ObtenerCombo();
    }
}
