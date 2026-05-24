using H.DataAccess.Models;

namespace H.DataAccess.Repositorios
{
    public interface IMetodoPagoRepository
    {
        TMetodoPago GetById(int id);
    }
}
