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
    public interface IMovimientoInsumoRepository : IGenericRepository<TMovimientoInsumo>
    {
        TMovimientoInsumo Add(MovimientoInsumo entidad);
        int Delete(int id, string usuario);
        TMovimientoInsumo Update(MovimientoInsumo entidad);
        MovimientoInsumo GetById(int id);
        IEnumerable<MovimientoInsumoListadoDTO> ObtenerCombo();
    }
}
