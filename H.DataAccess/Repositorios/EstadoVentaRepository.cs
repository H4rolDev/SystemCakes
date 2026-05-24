using H.DataAccess.Models;

namespace H.DataAccess.Repositorios
{
    public class EstadoVentaRepository : IEstadoVentaRepository
    {
        private readonly sistemContext context;

        public EstadoVentaRepository(sistemContext context)
        {
            this.context = context;
        }

        public TEstadoVenta? GetById(int id)
        {
            return context.TEstadoVenta.FirstOrDefault(x => x.Id == id);
        }
    }
}
