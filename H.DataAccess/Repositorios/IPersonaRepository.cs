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
    public interface IPersonaRepository : IGenericRepository<TPersona>
    {
        TPersona Add(Persona entidad);
        int Delete(int id, string usuario);
        TPersona Update(Persona entidad);
        Persona GetById(int id);
        IEnumerable<PersonaListadoDTO> ObtenerCombo();
        IEnumerable<PersonalDTO> ObtenerComboPersonal();
    }
}
