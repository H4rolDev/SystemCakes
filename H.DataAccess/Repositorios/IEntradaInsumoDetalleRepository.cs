using H.DataAccess.Models;
using System.Collections.Generic;

namespace H.DataAccess.Repositorios
{
    public interface IEntradaInsumoDetalleRepository : IGenericRepository<TEntradaInsumoDetalle>
    {
        TEntradaInsumoDetalle Add(TEntradaInsumoDetalle entidad);
        IEnumerable<TEntradaInsumoDetalle> AddRange(IEnumerable<TEntradaInsumoDetalle> entidades);
    }
}