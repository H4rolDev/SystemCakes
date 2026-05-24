using H.DataAccess.Models;
using H.DTOs;
using System.Collections.Generic;

namespace H.DataAccess.Repositorios
{
    public interface IEntradaInsumoRepository : IGenericRepository<TEntradaInsumo>
    {
        TEntradaInsumo Add(TEntradaInsumo entidad);
        TEntradaInsumo Update(TEntradaInsumo entidad);
        IEnumerable<EntradaInsumoListadoDTO> ObtenerPendientes();
        EntradaInsumoListadoDTO ObtenerPorId(int id);
        EntradaInsumoPaginacionDTO ObtenerTodos(EntradaInsumoFiltroDTO filtro);
        IEnumerable<EntradaInsumoHistorialDTO> ObtenerHistorial(int idEntradaInsumo);
        void AgregarHistorial(EntradaInsumoHistorialDTO historial);
    }
}