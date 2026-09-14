using H.DataAccess.Entidades;
using H.DataAccess.UnitofWork;

namespace H.Services;

public class TortaOpcionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TortaOpcionService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public IEnumerable<TortaOpcion> ObtenerPorTorta(int idTorta, bool soloActivos) =>
        _unitOfWork.TortaOpcionRepository.ObtenerPorTorta(idTorta, soloActivos);

    public TortaOpcion Upsert(TortaOpcion entidad)
    {
        var existente = entidad.Id > 0
            ? _unitOfWork.TortaOpcionRepository.ObtenerPorId(entidad.Id)
            : _unitOfWork.TortaOpcionRepository.ObtenerPorClave(entidad.IdTorta, entidad.Tipo, entidad.Valor);

        if (existente == null)
        {
            var creado = _unitOfWork.TortaOpcionRepository.Add(entidad);
            _unitOfWork.Commit();
            entidad.Id = creado.Id;
            return entidad;
        }

        entidad.Id = existente.Id;
        entidad.FechaCreacion = existente.FechaCreacion;
        entidad.UsuarioCreacion = existente.UsuarioCreacion;
        _unitOfWork.TortaOpcionRepository.Update(entidad);
        _unitOfWork.Commit();
        return entidad;
    }

    public int Delete(int id, string usuario)
    {
        var resultado = _unitOfWork.TortaOpcionRepository.Delete(id, usuario);
        _unitOfWork.Commit();
        return resultado;
    }
}
