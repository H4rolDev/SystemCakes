using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DataAccess.Repositorios;
using H.DTOs;

namespace H.DataAccess.Repositorios
{
    public interface IInsumoLoteRepository : IGenericRepository<TInsumoLote>
    {
        TInsumoLote Add(InsumoLote entidad);
        int Delete(int id, string usuario);
        TInsumoLote Update(InsumoLote entidad);
        InsumoLote GetById(int id);
        IEnumerable<InsumoLoteListadoDTO> ObtenerCombo();
    }
}
