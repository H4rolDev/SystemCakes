using H.DataAccess.Infraestructure;
using H.DataAccess.Models;

namespace H.DataAccess.Repositorios
{
    public class TipoEntregaRepository : GenericRepository<TTipoEntrega>, ITipoEntregaRepository
    {
        public TipoEntregaRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
        }
    }
}
