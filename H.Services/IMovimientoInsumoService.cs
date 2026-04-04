using H.DataAccess.Entidades;
using H.DTOs;
using Microsoft.AspNetCore.Http;

namespace H.Services
{
    public interface IMovimientoInsumoService
    {
        int Add(MovimientoInsumo entidad);
        int Update(MovimientoInsumo entidad);
        int Delete(int id, string usuario);
        MovimientoInsumo GetById(int id);
        IEnumerable<MovimientoInsumoListadoDTO> ObtenerCombo();
    }
}
