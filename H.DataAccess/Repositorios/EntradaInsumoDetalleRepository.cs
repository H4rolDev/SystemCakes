using AutoMapper;
using H.DataAccess.Infraestructure;
using H.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.DataAccess.Repositorios
{
    public class EntradaInsumoDetalleRepository : GenericRepository<TEntradaInsumoDetalle>, IEntradaInsumoDetalleRepository
    {
        protected IMapper mapper;
        public EntradaInsumoDetalleRepository(sistemContext context, IConnectionFactory connectionFactory)
            : base(context, connectionFactory)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TEntradaInsumoDetalle, TEntradaInsumoDetalle>(MemberList.None).ReverseMap());
            mapper = config.CreateMapper();
        }

        public new TEntradaInsumoDetalle Add(TEntradaInsumoDetalle entidad)
        {
            try
            {
                context.TEntradaInsumoDetalle.Add(entidad);
                return entidad;
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoDetalleRepository - Add: " + ex.Message, ex);
            }
        }

        public new IEnumerable<TEntradaInsumoDetalle> AddRange(IEnumerable<TEntradaInsumoDetalle> entidades)
        {
            try
            {
                context.TEntradaInsumoDetalle.AddRange(entidades);
                return entidades;
            }
            catch (Exception ex)
            {
                throw new Exception("EntradaInsumoDetalleRepository - AddRange: " + ex.Message, ex);
            }
        }
    }
}