using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess.Log;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace H.Services
{
    public class TortaService: ITortaService
    {
        private IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public TortaService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public int Add(Torta entidad)
        {
            try
            {
                var modelo = _unitOfWork.TortaRepository.Add(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "AlmacenService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Add";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public int Update(Torta entidad)
        {
            try
            {
                var modelo = _unitOfWork.TortaRepository.Update(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "TortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public int Delete(int id, string usuario)
        {
            try
            {
                var rpta = _unitOfWork.TortaRepository.Delete(id, usuario);
                _unitOfWork.Commit();
                return rpta;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "TortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Delete";
                error.Code = TiposError.NoEliminado;
                error.Objeto = JsonConvert.SerializeObject(new { id, usuario });

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public Torta GetById(int id)
        {
            try
            {
                return _unitOfWork.TortaRepository.GetById(id);
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "AlmacenService" + ex.Message;
                error.Exception = ex;
                error.Operation = "Update";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(id);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public IEnumerable<TortaListadoDTO> ObtenerCombo()
        {
            try
            {
                return _unitOfWork.TortaRepository.ObtenerCombo();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "CategoriaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "ObtenerListadoActivos";
                error.Code = TiposError.NoEncontrado;
                error.Objeto = JsonConvert.SerializeObject(null);

                LogErp.EscribirBaseDatos(error);
                throw ex;
            }
        }

        public async Task<int> AddAsync(Torta entidad, IFormFile? imagen)
        {
            try
            {
                if (imagen != null && imagen.Length > 0)
                {
                    var url = await _cloudinaryService.SubirImagenAsync(imagen, "tortas");
                    entidad.ImagenUrl = url;
                    entidad.ImagenPublicId = ExtraerPublicId(url);
                }

                var modelo = _unitOfWork.TortaRepository.Add(entidad);
                _unitOfWork.Commit();
                return modelo.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "TortaService" + ex.Message;
                error.Exception = ex;
                error.Operation = "AddAsync";
                error.Code = TiposError.NoInsertado;
                error.Objeto = JsonConvert.SerializeObject(entidad);
                LogErp.EscribirBaseDatos(error);
                throw;
            }
        }

        private string ExtraerPublicId(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segmentos = uri.AbsolutePath.Split('/');
                // Buscamos el índice después de "upload"
                var idxUpload = Array.IndexOf(segmentos, "upload");
                if (idxUpload < 0) return string.Empty;

                // Saltamos el segmento de versión (v123456) si existe
                var inicio = idxUpload + 1;
                if (inicio < segmentos.Length && segmentos[inicio].StartsWith("v")
                    && int.TryParse(segmentos[inicio][1..], out _))
                    inicio++;

                var publicIdConExtension = string.Join("/", segmentos[inicio..]);
                // Quitamos la extensión
                var puntoIdx = publicIdConExtension.LastIndexOf('.');
                return puntoIdx >= 0 ? publicIdConExtension[..puntoIdx] : publicIdConExtension;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
