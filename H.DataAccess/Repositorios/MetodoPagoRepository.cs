using H.DataAccess.Models;

namespace H.DataAccess.Repositorios
{
    public class MetodoPagoRepository : IMetodoPagoRepository
    {
        private readonly sistemContext context;

        public MetodoPagoRepository(sistemContext context)
        {
            this.context = context;
        }

        public TMetodoPago GetById(int id)
        {
            return context.TMetodoPago.FirstOrDefault(x => x.Id == id);
        }
    }
}
