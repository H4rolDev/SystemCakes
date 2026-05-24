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
    public interface ICompraRepository : IGenericRepository<TCompra>
    {
        TCompra Add(Compra entidad);
        int Delete(int id, string usuario);
        TCompra Update(Compra entidad);
        Compra GetById(int id);
        IEnumerable<CompraListadoDTO> ObtenerCombo();
    }
}
