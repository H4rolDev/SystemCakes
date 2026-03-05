using H.DataAccess.Entidades;
using H.DTOs;
using Microsoft.AspNetCore.Http;

namespace H.Services
{
    public interface ITortaImagenService
    {
        int Add(TortaImagen entidad);
        int Update(TortaImagen entidad);
        int Delete(int id, string usuario);
        TortaImagen GetById(int id);
        IEnumerable<TortaImagenListadoDTO> ObtenerCombo();
        Task<int> UpdateAsync(TortaImagen entidad, IFormFile? imagen);
    }
}
