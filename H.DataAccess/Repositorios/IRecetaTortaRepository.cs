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
    public interface IRecetaTortaRepository : IGenericRepository<TRecetaTorta>
    {
        TRecetaTorta Add(RecetaTorta entidad);
        int Delete(int id, string usuario);
        TRecetaTorta Update(RecetaTorta entidad);
        RecetaTorta GetById(int id);
        /*IEnumerable<RecetaTortaListadoDTO> ObtenerCombo();*/
    }
}
