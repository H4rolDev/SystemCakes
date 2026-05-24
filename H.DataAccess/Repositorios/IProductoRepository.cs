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
    public interface IProductoRepository : IGenericRepository<TProducto>
    {
        TProducto Add(Producto entidad);
        int Delete(int id, string usuario);
        TProducto Update(Producto entidad);
        Producto GetById(int id);
        IEnumerable<ProductoListadoDTO> ObtenerCombo();
    }
}
