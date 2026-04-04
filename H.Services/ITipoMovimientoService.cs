using H.DataAccess.Entidades;
using H.DTOs;
using Microsoft.AspNetCore.Http;

namespace H.Services
{
    public interface ITipoMovimientoService
    {
        int Add(TipoMovimiento entidad);
        int Update(TipoMovimiento entidad);
        int Delete(int id, string usuario);
        TipoMovimiento GetById(int id);
        IEnumerable<TipoMovimientoListadoDTO> ObtenerCombo();
    }
}
