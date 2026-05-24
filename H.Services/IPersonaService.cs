using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface IPersonaService
    {
        int Add(Persona entidad);
        int Update(Persona entidad);
        int Delete(int id, string usuario);
        Persona GetById(int id);
        IEnumerable<PersonaListadoDTO> ObtenerCombo();
        IEnumerable<PersonalDTO> ObtenerComboPersonal();
    }
}
