using H.DataAccess.Infraestructure;
using H.DataAccess.Models;

namespace H.DataAccess.Repositorios
{
    public class EstadoEntregaRepository : GenericRepository<TEstadoEntrega>, IEstadoEntregaRepository
    {
        public EstadoEntregaRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
        }
    }
}
