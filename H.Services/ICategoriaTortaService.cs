using H.DataAccess.Entidades;
using H.DTOs;
using Microsoft.AspNetCore.Http;

namespace H.Services
{
    public interface ICategoriaTortaService
    {
        int Add(CategoriaTorta entidad);
        int Update(CategoriaTorta entidad);
        int Delete(int id, string usuario);
        CategoriaTorta GetById(int id);
        IEnumerable<CategoriaTortaListadoDTO> ObtenerCombo();
    }
}
