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
    public interface IProduccionRepository : IGenericRepository<TProduccion>
    {
        TProduccion Add(Produccion entidad);
        int Delete(int id, string usuario);
        TProduccion Update(Produccion entidad);
        Produccion GetById(int id);
        IEnumerable<ProduccionDetalleDTO> ObtenerDetalle(int idProduccion);
        IEnumerable<ProduccionCabeceraDTO> ObtenerProducciones();
    }
}
