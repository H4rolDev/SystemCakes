using H.DataAccess.Models;
using H.DataAccess.Infraestructure;
using System.Linq;

namespace H.DataAccess.Repositorios
{
    public class MetaVentaRepository : GenericRepository<TMetaVenta>, IMetaVentaRepository
    {
        public MetaVentaRepository(sistemContext context, IConnectionFactory connectionFactory) : base(context, connectionFactory)
        {
        }
    }
}