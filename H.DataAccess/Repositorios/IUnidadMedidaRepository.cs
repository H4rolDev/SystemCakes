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
    public interface IUnidadMedidaRepository : IGenericRepository<TUnidadMedida>
    {
        TUnidadMedida Add(UnidadMedida entidad);
        int Delete(int id, string usuario);
        TUnidadMedida Update(UnidadMedida entidad);
        UnidadMedida GetById(int id);
        IEnumerable<UnidadMedidaListadoDTO> ObtenerCombo();
    }
}
