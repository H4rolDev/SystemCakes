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
    public interface ITortaLoteRepository : IGenericRepository<TTortaLote>
    {
        TTortaLote Add(TortaLote entidad);
        int Delete(int id, string usuario);
        TTortaLote Update(TortaLote entidad);
        TortaLote GetById(int id);
        IEnumerable<TortaLoteListadoDTO> ObtenerCombo();
    }
}
