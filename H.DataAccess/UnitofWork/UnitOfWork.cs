using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess;
using H.DataAccess.Infraestructure;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.Repositorios;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.DataAccess.Repositorios;

namespace H.DataAccess.UnitofWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {        
        private IConnectionFactory _connectionFactory;
        private sistemContext _context;

        public UnitOfWork(sistemContext context, IConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        public DbContext Context => _context;

        private IProductoRepository _ProductoRepository;
        private ITortaLoteRepository _TortaLoteRepository;
        private IClienteRepository _ClienteRepository;
        private IRolRepository _RolRepository;
        private IPersonaRepository _PersonaRepository;
        private IUsuarioRepository _UsuarioRepository;
        private IUsuarioRolRepository _UsuarioRolRepository;
        private ITortaRepository _TortaRepository;
        private ITipoDocumentoRepository _TipoDocumentoRepository;
        private IInsumoRepository _InsumoRepository;
        private ICompraRepository _CompraRepository;
        private ICompraDetalleRepository _CompraDetalleRepository;
        private IRecetaTortaRepository _RecetaTortaRepository;
        private IProduccionRepository _ProduccionRepository;
        private ICategoriaTortaRepository _CategoriaTortaRepository;
        private IUnidadMedidaRepository _UnidadMedidaRepository;
        private IInsumoLoteRepository _InsumoLoteRepository;
        private ITipoMovimientoRepository _TipoMovimientoRepository;
        private IMovimientoInsumoRepository _MovimientoInsumoRepository;
        private IMovimientoTortaRepository _MovimientoTortaRepository;
        private IProduccionDetalleInsumoRepository _ProduccionDetalleInsumoRepository;
        private IVentaRepository _VentaRepository;
        private IVentaDetalleRepository _VentaDetalleRepository;
        private ICancelacionVentaRepository _CancelacionVentaRepository;
        private IPagoVentaRepository _PagoVentaRepository;
        private IEntregaDeliveryRepository _EntregaDeliveryRepository;
        private IComprobanteVentaRepository _ComprobanteVentaRepository;
        private IMetodoPagoRepository _MetodoPagoRepository;
        private IEstadoEntregaRepository _EstadoEntregaRepository;
        private ITipoEntregaRepository _TipoEntregaRepository;
        private IEstadoVentaRepository _EstadoVentaRepository;
        private IProveedorRepository _ProveedorRepository;
        private IEntradaInsumoRepository _EntradaInsumoRepository;
        private IEntradaInsumoDetalleRepository _EntradaInsumoDetalleRepository;
        private IMetaVentaRepository _MetaVentaRepository;

        IProductoRepository IUnitOfWork.ProductoRepository
        {
            get
            {
                return _ProductoRepository ?? new ProductoRepository(_context, _connectionFactory);
            }
        }
        ITortaLoteRepository IUnitOfWork.TortaLoteRepository
        {
            get
            {
                return _TortaLoteRepository ?? new TortaLoteRepository(_context, _connectionFactory);
            }
        }
        IClienteRepository IUnitOfWork.ClienteRepository
        {
            get
            {
                return _ClienteRepository ?? new ClienteRepository(_context, _connectionFactory);
            }
        }
        IRolRepository IUnitOfWork.RolRepository
        {
            get
            {
                return _RolRepository ?? new RolRepository(_context, _connectionFactory);
            }
        }
        IPersonaRepository IUnitOfWork.PersonaRepository
        {
            get
            {
                return _PersonaRepository ?? new PersonaRepository(_context, _connectionFactory);
            }
        }
        IUsuarioRepository IUnitOfWork.UsuarioRepository
        {
            get
            {
                return _UsuarioRepository ?? new UsuarioRepository(_context, _connectionFactory);
            }   
        }
        IUsuarioRolRepository IUnitOfWork.UsuarioRolRepository
        {
            get
            {
                return _UsuarioRolRepository ?? new UsuarioRolRepository(_context, _connectionFactory);
            }
        }
        ITortaRepository IUnitOfWork.TortaRepository
        {
            get
            {
                return _TortaRepository ?? new TortaRepository(_context, _connectionFactory);
            }
        }
        ITipoDocumentoRepository IUnitOfWork.TipoDocumentoRepository
        {
            get
            {
                return _TipoDocumentoRepository ?? new TipoDocumentoRepository(_context, _connectionFactory);
            }
        }
        IInsumoRepository IUnitOfWork.InsumoRepository
        {
            get
            {
                return _InsumoRepository ?? new InsumoRepository(_context, _connectionFactory);
            }
        }
        ICompraRepository IUnitOfWork.CompraRepository
        {
            get
            {
                return _CompraRepository ?? new CompraRepository(_context, _connectionFactory);
            }
        }
        ICompraDetalleRepository IUnitOfWork.CompraDetalleRepository
        {
            get
            {
                return _CompraDetalleRepository ?? new CompraDetalleRepository(_context, _connectionFactory);
            }
        }
        IRecetaTortaRepository IUnitOfWork.RecetaTortaRepository
        {
            get
            {
                return _RecetaTortaRepository ?? new RecetaTortaRepository(_context, _connectionFactory);
            }
        }
        IProduccionRepository IUnitOfWork.ProduccionRepository
        {
            get
            {
                return _ProduccionRepository ?? new ProduccionRepository(_context, _connectionFactory);
            }
        }
        ICategoriaTortaRepository IUnitOfWork.CategoriaTortaRepository
        {
            get
            {
                return _CategoriaTortaRepository ?? new CategoriaTortaRepository(_context, _connectionFactory);
            }
        }
        IUnidadMedidaRepository IUnitOfWork.UnidadMedidaRepository
        {
            get
            {
                return _UnidadMedidaRepository ?? new UnidadMedidaRepository(_context, _connectionFactory);
            }
        }
        IInsumoLoteRepository IUnitOfWork.InsumoLoteRepository
        {
            get
            {
                return _InsumoLoteRepository ?? new InsumoLoteRepository(_context, _connectionFactory);
            }
        }
        ITipoMovimientoRepository IUnitOfWork.TipoMovimientoRepository
        {
            get
            {
                return _TipoMovimientoRepository ?? new TipoMovimientoRepository(_context, _connectionFactory);
            }
        }
        IMovimientoInsumoRepository IUnitOfWork.MovimientoInsumoRepository
        {
            get
            {
                return _MovimientoInsumoRepository ?? new MovimientoInsumoRepository(_context, _connectionFactory);
            }
        }
        IMovimientoTortaRepository IUnitOfWork.MovimientoTortaRepository
        {
            get
            {
                return _MovimientoTortaRepository ?? new MovimientoTortaRepository(_context, _connectionFactory);
            }
        }
        IProduccionDetalleInsumoRepository IUnitOfWork.ProduccionDetalleInsumoRepository
        {
            get
            {
                return _ProduccionDetalleInsumoRepository ?? new ProduccionDetalleInsumoRepository(_context, _connectionFactory);
            }
        }
        IVentaRepository IUnitOfWork.VentaRepository
        {
            get
            {
                return _VentaRepository ?? new VentaRepository(_context, _connectionFactory);
            }
        }
        IVentaDetalleRepository IUnitOfWork.VentaDetalleRepository
        {
            get
            {
                return _VentaDetalleRepository ?? new VentaDetalleRepository(_context, _connectionFactory);
            }
        }
        ICancelacionVentaRepository IUnitOfWork.CancelacionVentaRepository
        {
            get
            {
                return _CancelacionVentaRepository ?? new CancelacionVentaRepository(_context, _connectionFactory);
            }
        }
        IPagoVentaRepository IUnitOfWork.PagoVentaRepository
        {
            get
            {
                return _PagoVentaRepository ?? new PagoVentaRepository(_context, _connectionFactory);
            }
        }
        IEntregaDeliveryRepository IUnitOfWork.EntregaDeliveryRepository
        {
            get
            {
                return _EntregaDeliveryRepository ?? new EntregaDeliveryRepository(_context, _connectionFactory);
            }
        }
        IComprobanteVentaRepository IUnitOfWork.ComprobanteVentaRepository
        {
            get
            {
                return _ComprobanteVentaRepository ?? new ComprobanteVentaRepository(_context, _connectionFactory);
            }
        }
        IMetodoPagoRepository IUnitOfWork.MetodoPagoRepository
        {
            get
            {
                return _MetodoPagoRepository ?? new MetodoPagoRepository(_context);
            }
        }
        IEstadoEntregaRepository IUnitOfWork.EstadoEntregaRepository
        {
            get
            {
                return _EstadoEntregaRepository ?? new EstadoEntregaRepository(_context, _connectionFactory);
            }
        }
        ITipoEntregaRepository IUnitOfWork.TipoEntregaRepository
        {
            get
            {
                return _TipoEntregaRepository ?? new TipoEntregaRepository(_context, _connectionFactory);
            }
        }
IEstadoVentaRepository IUnitOfWork.EstadoVentaRepository
        {
            get
            {
                return _EstadoVentaRepository ?? new EstadoVentaRepository(_context);
            }
        }
        IProveedorRepository IUnitOfWork.ProveedorRepository
        {
            get
            {
                return _ProveedorRepository ?? new ProveedorRepository(_context, _connectionFactory);
            }
        }
        IEntradaInsumoRepository IUnitOfWork.EntradaInsumoRepository
        {
            get
            {
                return _EntradaInsumoRepository ?? new EntradaInsumoRepository(_context, _connectionFactory);
            }
        }
        IEntradaInsumoDetalleRepository IUnitOfWork.EntradaInsumoDetalleRepository
        {
            get
            {
                return _EntradaInsumoDetalleRepository ?? new EntradaInsumoDetalleRepository(_context, _connectionFactory);
            }
        }
        IMetaVentaRepository IUnitOfWork.MetaVentaRepository
        {
            get
            {
                return _MetaVentaRepository ?? new MetaVentaRepository(_context, _connectionFactory);
            }
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UnitOfWork" + ex.Message;
                error.Exception = ex;
                error.Operation = "Commit";
                error.Code = TiposError.NoInsertado;
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }

        public void Rollback()
        {
            try
            {
     
                _context.Dispose();
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = "UnitOfWork" + ex.Message;
                error.Exception = ex;
                error.Operation = "Rollback";
                error.Code = TiposError.NoInsertado;
                LogErp.EscribirBaseDatos(error);

                throw ex;
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
