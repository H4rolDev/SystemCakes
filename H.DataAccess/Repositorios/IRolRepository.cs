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
    public interface IRolRepository : IGenericRepository<TRol>
    {
        TRol Add(Rol entidad);
        int Delete(int id, string usuario);
        TRol Update(Rol entidad);
        Rol GetById(int id);
        IEnumerable<RolListadoDTO> ObtenerCombo();
        IEnumerable<RolListadoDTO> ObtenerListado();
    }
}
