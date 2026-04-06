using H.DataAccess.Entidades;
using H.DTOs;
using Microsoft.AspNetCore.Http;

namespace H.Services
{
    public interface IMovimientoTortaService
    {
        int Add(MovimientoTorta entidad);
        int Update(MovimientoTorta entidad);
        int Delete(int id, string usuario);
        MovimientoTorta GetById(int id);
        IEnumerable<MovimientoTortaListadoDTO> ObtenerCombo();
    }
}
