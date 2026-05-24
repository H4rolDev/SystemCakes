using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DTOs
{
    public class EntradaInsumoListadoDTO
    {
        public int Id { get; set; }
        public string? Proveedor { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string? ImagenDocumento { get; set; }
        public string? Observaciones { get; set; }
        public int IdEstado { get; set; }
        public string? Estado { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? UsuarioAprobacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string? UsuarioRechazo { get; set; }
        public DateTime? FechaRechazo { get; set; }
        public List<EntradaInsumoDetalleDTO> Detalles { get; set; }
    }

    public class EntradaInsumoDetalleDTO
    {
        public int Id { get; set; }
        public int IdInsumo { get; set; }
        public string? Insumo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }

    public class EntradaInsumoRequestDTO
    {
        public int? IdProveedor { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string? ImagenBase64 { get; set; }
        public string? Observaciones { get; set; }
        public string? Usuario { get; set; }
        public List<EntradaInsumoDetalleRequestDTO>? Detalles { get; set; }
    }

    public class EntradaInsumoDetalleRequestDTO
    {
        public int IdInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }

    public class EntradaInsumoAprobarDTO
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string? Rol { get; set; }
    }

    public class EntradaInsumoRechazarDTO
    {
        public int Id { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
    }

    public class SubirImagenEntradaInsumoDTO
    {
        public string ImagenBase64 { get; set; }
    }

    // ============================================
    // DTOs para Filtros e Historial
    // ============================================

    public class EntradaInsumoFiltroDTO
    {
        public int? IdEstado { get; set; }
        public int? IdProveedor { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? Busqueda { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanioPagina { get; set; } = 20;
    }

    public class EntradaInsumoPaginacionDTO
    {
        public List<EntradaInsumoListadoDTO> Items { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
    }

    public class EntradaInsumoHistorialDTO
    {
        public int Id { get; set; }
        public int IdEntradaInsumo { get; set; }
        public int? IdEstadoAnterior { get; set; }
        public int IdEstadoNuevo { get; set; }
        public string Accion { get; set; }
        public string? Observacion { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }
}