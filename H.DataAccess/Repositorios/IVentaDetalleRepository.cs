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
    public interface IVentaDetalleRepository : IGenericRepository<TVentaDetalle>
    {
        TVentaDetalle Add(VentaDetalle entidad);
        int Delete(int id, string usuario);
        TVentaDetalle Update(VentaDetalle entidad);
        VentaDetalle GetById(int id);
        //IEnumerable<VentaDetalleListadoDTO> ObtenerCombo();
    }
}
