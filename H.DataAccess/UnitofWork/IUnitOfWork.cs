//using HPP.DataAccess.Repositorios;

using H.DataAccess.Repositorios;

namespace H.DataAccess.UnitofWork
{
    public interface IUnitOfWork
    {
        IProductoRepository ProductoRepository { get; }     
        ICategoriaRepository CategoriaRepository { get; }
        IClienteRepository ClienteRepository { get; }
        IRolRepository RolRepository { get; }
        IPersonaRepository PersonaRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        IUsuarioRolRepository UsuarioRolRepository { get; }
        ITortaRepository TortaRepository { get; }
        ITipoDocumentoRepository TipoDocumentoRepository { get; }
        IInsumoRepository InsumoRepository { get; }

        void Commit();
        void Rollback();
        void Dispose();
    }
}
