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
    public interface IUsuarioRolRepository : IGenericRepository<TUsuarioRol>
    {
        TUsuarioRol Add(UsuarioRol entidad);
        int Delete(int id, string usuario);
        TUsuarioRol Update(UsuarioRol entidad);
        UsuarioRol GetById(int id);
        /*IEnumerable<UsuarioRolListadoDTO> ObtenerCombo();*/
    }
}
