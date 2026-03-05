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
    public interface ITortaImagenRepository : IGenericRepository<TTortaImagen>
    {
        TTortaImagen Add(TortaImagen entidad);
        int Delete(int id, string usuario);
        TTortaImagen Update(TortaImagen entidad);
        TortaImagen GetById(int id);
        IEnumerable<TortaImagenListadoDTO> ObtenerCombo();
    }
}
