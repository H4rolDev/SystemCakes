//using HPP.DataAccess.Repositorios;

using H.DataAccess.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace H.DataAccess.UnitofWork
{
    public interface IUnitOfWork
    {
        DbContext Context { get; }
        IProductoRepository ProductoRepository { get; }     
        ITortaLoteRepository TortaLoteRepository { get; }
        IClienteRepository ClienteRepository { get; }
        IRolRepository RolRepository { get; }
        IPersonaRepository PersonaRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        IUsuarioRolRepository UsuarioRolRepository { get; }
        ITortaRepository TortaRepository { get; }
        ITipoDocumentoRepository TipoDocumentoRepository { get; }
        IInsumoRepository InsumoRepository { get; }
        ICompraRepository CompraRepository { get; }
        ICompraDetalleRepository CompraDetalleRepository { get; }
        IRecetaTortaRepository RecetaTortaRepository { get; }
        IProduccionRepository ProduccionRepository { get; }
        ICategoriaTortaRepository CategoriaTortaRepository { get; }
        IUnidadMedidaRepository UnidadMedidaRepository { get; }
        IInsumoLoteRepository InsumoLoteRepository { get; }
        ITipoMovimientoRepository TipoMovimientoRepository { get; }
        IMovimientoInsumoRepository MovimientoInsumoRepository { get; }
        IMovimientoTortaRepository MovimientoTortaRepository { get; }
        IProduccionDetalleInsumoRepository ProduccionDetalleInsumoRepository { get; }
        IVentaRepository VentaRepository { get; }
        IVentaDetalleRepository VentaDetalleRepository { get; }
        ICancelacionVentaRepository CancelacionVentaRepository { get; }
        IPagoVentaRepository PagoVentaRepository { get; }
        IEntregaDeliveryRepository EntregaDeliveryRepository { get; }
        IComprobanteVentaRepository ComprobanteVentaRepository { get; }
        IMetodoPagoRepository MetodoPagoRepository { get; }
        IEstadoEntregaRepository EstadoEntregaRepository { get; }
        ITipoEntregaRepository TipoEntregaRepository { get; }
        IEstadoVentaRepository EstadoVentaRepository { get; }
        IProveedorRepository ProveedorRepository { get; }
        IEntradaInsumoRepository EntradaInsumoRepository { get; }
        IEntradaInsumoDetalleRepository EntradaInsumoDetalleRepository { get; }
        IMetaVentaRepository MetaVentaRepository { get; }

        void Commit();
        void Rollback();
        void Dispose();
    }
}
