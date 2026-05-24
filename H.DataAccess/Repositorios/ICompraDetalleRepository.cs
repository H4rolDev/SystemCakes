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
    public interface ICompraDetalleRepository : IGenericRepository<TCompraDetalle>
    {
        TCompraDetalle Add(CompraDetalle entidad);
        int Delete(int id, string usuario);
        TCompraDetalle Update(CompraDetalle entidad);
        CompraDetalle GetById(int id);
        IEnumerable<CompraDetalleListadoDTO> ObtenerCombo();
    }
}
