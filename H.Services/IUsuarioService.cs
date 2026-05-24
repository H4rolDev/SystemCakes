using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface IUsuarioService
    {
        int Add(Usuario entidad);
        int Update(Usuario entidad);
        int Delete(int id, string usuario);
        Usuario GetById(int id);
        IEnumerable<ComboDTO> ObtenerComboClientes();
        IEnumerable<ComboDTO> ObtenerComboDrivers();
    }
}
