using H.DataAccess.Entidades;
using H.DataAccess.Models;

namespace H.DataAccess.Repositorios;

public interface ITortaOpcionRepository : IGenericRepository<TTortaOpcion>
{
    IEnumerable<TortaOpcion> ObtenerPorTorta(int idTorta, bool soloActivos);
    TortaOpcion? ObtenerPorId(int id);
    TortaOpcion? ObtenerPorClave(int idTorta, string tipo, string valor);
    TTortaOpcion Add(TortaOpcion entidad);
    TTortaOpcion Update(TortaOpcion entidad);
}
