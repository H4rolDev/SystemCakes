using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface IRolService
    {
        int Add(Rol entidad);
        int Update(Rol entidad);
        int Delete(int id, string usuario);
        Rol GetById(int id);
        IEnumerable<RolListadoDTO> ObtenerCombo();
        IEnumerable<RolListadoDTO> ObtenerListado();
    }
}
