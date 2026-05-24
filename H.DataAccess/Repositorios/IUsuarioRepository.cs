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
    public interface IUsuarioRepository : IGenericRepository<TUsuario>
    {
        TUsuario Add(Usuario entidad);
        int Delete(int id, string usuario);
        TUsuario Update(Usuario entidad);
        Usuario GetById(int id);
        IEnumerable<ComboDTO> ObtenerComboClientes();
        IEnumerable<ComboDTO> ObtenerComboDrivers();
    }
}
