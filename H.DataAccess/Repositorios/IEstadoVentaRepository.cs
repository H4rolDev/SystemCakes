using H.DataAccess.Models;

namespace H.DataAccess.Repositorios
{
    public interface IEstadoVentaRepository
    {
        TEstadoVenta? GetById(int id);
    }
}
