using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.DataAccess.Entidades;
using H.DataAccess.Models;
using H.DataAccess.Repositorios;
using H.DTOs;

namespace H.DataAccess.Repositorios
{
    public interface ITipoDocumentoRepository : IGenericRepository<TTipoDocumento>
    {
        TTipoDocumento Add(TipoDocumento entidad);
        int Delete(int id, string usuario);
        TTipoDocumento Update(TipoDocumento entidad);
        TipoDocumento GetById(int id);
        IEnumerable<TipoDocumentoListadoDTO> ObtenerCombo();
    }
}
