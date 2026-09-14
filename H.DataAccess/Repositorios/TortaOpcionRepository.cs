using AutoMapper;
using H.DataAccess.Entidades;
using H.DataAccess.Infraestructure;
using H.DataAccess.Models;
using H.DTOs;
using Microsoft.EntityFrameworkCore;

namespace H.DataAccess.Repositorios;

public class TortaOpcionRepository : GenericRepository<TTortaOpcion>, ITortaOpcionRepository
{
    private readonly Mapper _mapper;

    public TortaOpcionRepository(sistemContext context, IConnectionFactory connectionFactory)
        : base(context, connectionFactory)
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
            cfg.CreateMap<TortaOpcion, TTortaOpcion>(MemberList.None).ReverseMap()));
    }

    public IEnumerable<TortaOpcion> ObtenerPorTorta(int idTorta, bool soloActivos)
    {
        var query = context.TTortaOpcion.AsNoTracking().Where(x => x.IdTorta == idTorta);
        if (soloActivos)
            query = query.Where(x => x.Activo);

        return query.OrderBy(x => x.Tipo).ThenBy(x => x.Orden).ThenBy(x => x.Valor)
            .Select(x => _mapper.Map<TortaOpcion>(x)).ToList();
    }

    public TortaOpcion? ObtenerPorId(int id) =>
        _mapper.Map<TortaOpcion?>(context.TTortaOpcion.AsNoTracking().FirstOrDefault(x => x.Id == id));

    public TortaOpcion? ObtenerPorClave(int idTorta, string tipo, string valor) =>
        _mapper.Map<TortaOpcion?>(context.TTortaOpcion.AsNoTracking().FirstOrDefault(x =>
            x.IdTorta == idTorta && x.Tipo == tipo && x.Valor == valor));

    public TTortaOpcion Add(TortaOpcion entidad)
    {
        var modelo = _mapper.Map<TTortaOpcion>(entidad);
        base.Add(modelo);
        return modelo;
    }

    public TTortaOpcion Update(TortaOpcion entidad)
    {
        var modelo = _mapper.Map<TTortaOpcion>(entidad);
        base.Update(modelo);
        return modelo;
    }
}
