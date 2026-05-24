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
    public interface IPagoVentaRepository : IGenericRepository<TPagoVenta>
    {
        TPagoVenta Add(PagoVenta entidad);
        int Delete(int id, string usuario);
        TPagoVenta Update(PagoVenta entidad);
        PagoVenta GetById(int id);
        //IEnumerable<PagoVentaListadoDTO> ObtenerCombo();
    }
}
