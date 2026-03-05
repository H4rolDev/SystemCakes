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
    public interface ICategoriaTortaRepository : IGenericRepository<TCategoriaTorta>
    {
        TCategoriaTorta Add(CategoriaTorta entidad);
        int Delete(int id, string usuario);
        TCategoriaTorta Update(CategoriaTorta entidad);
        CategoriaTorta GetById(int id);
        IEnumerable<CategoriaTortaListadoDTO> ObtenerCombo();
    }
}
