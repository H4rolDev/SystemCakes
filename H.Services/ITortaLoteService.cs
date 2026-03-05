using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface ITortaLoteService
    {
        int Add(TortaLote entidad);
        int Update(TortaLote entidad);
        int Delete(int id, string usuario);
        TortaLote GetById(int id);
        IEnumerable<TortaLoteListadoDTO> ObtenerCombo();
    }
}
