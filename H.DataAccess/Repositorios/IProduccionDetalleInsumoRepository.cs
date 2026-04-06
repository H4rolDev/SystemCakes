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
    public interface IProduccionDetalleInsumoRepository : IGenericRepository<TProduccionDetalleInsumo>
    {
        TProduccionDetalleInsumo Add(ProduccionDetalleInsumo entidad);
        int Delete(int id, string usuario);
        TProduccionDetalleInsumo Update(ProduccionDetalleInsumo entidad);
        ProduccionDetalleInsumo GetById(int id);
    }
}
