using H.DataAccess.Entidades;
using H.DataAccess.Enums;
using H.DataAccess;
using H.DataAccess.Infrastructure;
using H.DataAccess.Log;
using H.DataAccess.Models;
using H.DataAccess.Repositorios;
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

        private IProductoRepository _ProductoRepository;
        private ICategoriaRepository _CategoriaRepository;
        private IClienteRepository _ClienteRepository;
        private IRolRepository _RolRepository;
        private IPersonaRepository _PersonaRepository;
        private IUsuarioRepository _UsuarioRepository;
        private IUsuarioRolRepository _UsuarioRolRepository;
        private ITortaRepository _TortaRepository;

        IProductoRepository IUnitOfWork.ProductoRepository
        {
            get
            {
                return _ProductoRepository ?? new ProductoRepository(_context, _connectionFactory);
            }
        }
        ICategoriaRepository IUnitOfWork.CategoriaRepository
        {
            get
            {
                return _CategoriaRepository ?? new CategoriaRepository(_context, _connectionFactory);
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

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _context.Dispose();
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
