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
    public interface ICancelacionVentaRepository : IGenericRepository<TCancelacionVenta>
    {
        TCancelacionVenta Add(CancelacionVenta entidad);
        int Delete(int id, string usuario);
        TCancelacionVenta Update(CancelacionVenta entidad);
        CancelacionVenta GetById(int id);
        //IEnumerable<CancelacionVentaListadoDTO> ObtenerCombo();
    }
}
