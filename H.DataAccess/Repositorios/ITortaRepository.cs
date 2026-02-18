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
    public interface ITortaRepository : IGenericRepository<TTorta>
    {
        TTorta Add(Torta entidad);
        int Delete(int id, string usuario);
        TTorta Update(Torta entidad);
        Torta GetById(int id);
        IEnumerable<TortaListadoDTO> ObtenerCombo();
    }
}
