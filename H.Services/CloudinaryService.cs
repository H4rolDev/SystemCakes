using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.UnitofWork;
using H.DTOs;
using H.Services.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace H.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var settings = config.Value;
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> SubirImagenAsync(IFormFile archivo, string carpeta = "tortas")
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo está vacío o es nulo.");

            using var stream = archivo.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(archivo.FileName, stream),
                Folder = carpeta,
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var resultado = await _cloudinary.UploadAsync(uploadParams);

            if (resultado.Error != null)
                throw new Exception($"Error al subir imagen a Cloudinary: {resultado.Error.Message}");

            return resultado.SecureUrl.ToString();
        }

        public async Task<bool> EliminarImagenAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return false;

            var deleteParams = new DeletionParams(publicId);
            var resultado = await _cloudinary.DestroyAsync(deleteParams);

            return resultado.Result == "ok";
        }
    }
}