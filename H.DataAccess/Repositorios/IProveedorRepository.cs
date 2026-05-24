using H.DataAccess.Models;
using H.DataAccess.Entidades;
using H.DTOs;
using System.Collections.Generic;

namespace H.DataAccess.Repositorios
{
    public interface IProveedorRepository
    {
        TProveedor Add(Proveedor entidad);
        TProveedor Update(Proveedor entidad);
        TProveedor? GetById(int id);
        IEnumerable<ProveedorListadoDTO> ObtenerCombo();
        void Desactivar(int id);
        void Activar(int id);
    }
}