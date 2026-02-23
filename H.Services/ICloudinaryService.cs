using H.DataAccess.Entidades;
using H.DTOs;
using Microsoft.AspNetCore.Http;

namespace H.Services
{
    public interface ICloudinaryService
    {
        Task<string> SubirImagenAsync(IFormFile archivo, string carpeta = "tortas");
        Task<bool> EliminarImagenAsync(string publicId);
    }
}
