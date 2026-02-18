using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface ITortaService
    {
        int Add(Torta entidad);
        int Update(Torta entidad);
        int Delete(int id, string usuario);
        Torta GetById(int id);
        IEnumerable<TortaListadoDTO> ObtenerCombo();
    }
}
