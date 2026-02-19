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
    public interface IInsumoRepository : IGenericRepository<TInsumo>
    {
        TInsumo Add(Insumo entidad);
        int Delete(int id, string usuario);
        TInsumo Update(Insumo entidad);
        Insumo GetById(int id);
        IEnumerable<InsumoListadoDTO> ObtenerCombo();
    }
}
