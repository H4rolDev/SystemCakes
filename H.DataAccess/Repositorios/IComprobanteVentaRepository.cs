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
    public interface IComprobanteVentaRepository : IGenericRepository<TComprobanteVenta>
    {
        TComprobanteVenta Add(ComprobanteVenta entidad);
        int Delete(int id, string usuario);
        TComprobanteVenta Update(ComprobanteVenta entidad);
        ComprobanteVenta GetById(int id);
        //IEnumerable<ComprobanteVentaListadoDTO> ObtenerCombo();
    }
}
