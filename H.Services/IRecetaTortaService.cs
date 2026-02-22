using H.DataAccess.Entidades;
using H.DTOs;

namespace H.Services
{
    public interface IRecetaTortaService
    {
        int Add(RecetaTorta entidad);
        int Update(RecetaTorta entidad);
        int Delete(int id, string usuario);
        RecetaTorta GetById(int id);
        /*IEnumerable<InsertarMultipleRecetaTortaDTO> ObtenerCombo();*/
        public bool AddMultipleTabla(InsertarMultipleRecetaTortaDTO dto);
    }
}
