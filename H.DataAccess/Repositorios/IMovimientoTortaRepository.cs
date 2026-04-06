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
    public interface IMovimientoTortaRepository : IGenericRepository<TMovimientoTorta>
    {
        TMovimientoTorta Add(MovimientoTorta entidad);
        int Delete(int id, string usuario);
        TMovimientoTorta Update(MovimientoTorta entidad);
        MovimientoTorta GetById(int id);
        IEnumerable<MovimientoTortaListadoDTO> ObtenerCombo();
    }
}
