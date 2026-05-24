using H.DataAccess.Entidades;
using H.DataAccess.Infraestructure;
using H.DataAccess.Models;
using H.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace H.DataAccess.Repositorios
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly sistemContext _context;
        private readonly IConnectionFactory _connectionFactory;

        public ProveedorRepository(sistemContext context, IConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        private IDbConnection CreateConnection()
        {
            return _connectionFactory.GetConnection;
        }

        public TProveedor Add(Proveedor entidad)
        {
            try
            {
                var modelo = new TProveedor
                {
                    Nombre = entidad.Nombre,
                    Ruc = entidad.Ruc,
                    Telefono = entidad.Telefono,
                    Direccion = entidad.Direccion,
                    Contacto = entidad.Contacto,
                    Activo = true,
                    UsuarioCreacion = "sistema",
                    FechaCreacion = DateTime.Now
                };
                _context.TProveedor.Add(modelo);
                _context.SaveChanges();
                return modelo;
            }
            catch (Exception ex)
            {
                throw new Exception("ProveedorRepository - Add: " + ex.Message, ex);
            }
        }

        public TProveedor Update(Proveedor entidad)
        {
            try
            {
                var modelo = _context.TProveedor.Find(entidad.Id);
                if (modelo == null) throw new Exception("Proveedor no encontrado");
                
                modelo.Nombre = entidad.Nombre;
                modelo.Ruc = entidad.Ruc;
                modelo.Telefono = entidad.Telefono;
                modelo.Direccion = entidad.Direccion;
                modelo.Contacto = entidad.Contacto;
                modelo.UsuarioModificacion = "sistema";
                modelo.FechaModificacion = DateTime.Now;
                
                _context.TProveedor.Update(modelo);
                _context.SaveChanges();
                return modelo;
            }
            catch (Exception ex)
            {
                throw new Exception("ProveedorRepository - Update: " + ex.Message, ex);
            }
        }

        public IEnumerable<ProveedorListadoDTO> ObtenerCombo()
        {
            try
            {
                var query = @"SELECT Id, Nombre, Ruc, Telefono, Direccion, Contacto, Activo 
                              FROM TProveedor 
                              WHERE Activo = 1 
                              ORDER BY Nombre";
                using var conn = CreateConnection();
                var rpta = SqlMapper.Query<ProveedorListadoDTO>(conn, query);
                return rpta;
            }
            catch (Exception ex)
            {
                throw new Exception("ProveedorRepository - ObtenerCombo: " + ex.Message, ex);
            }
        }

        public TProveedor? GetById(int id)
        {
            try
            {
                return _context.TProveedor.Find(id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProveedorRepository - GetById: " + ex.Message, ex);
            }
        }

        public void Desactivar(int id)
        {
            try
            {
                var modelo = _context.TProveedor.Find(id);
                if (modelo == null) throw new Exception("Proveedor no encontrado");
                modelo.Activo = false;
                modelo.UsuarioModificacion = "admin";
                modelo.FechaModificacion = DateTime.Now;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("ProveedorRepository - Desactivar: " + ex.Message, ex);
            }
        }

        public void Activar(int id)
        {
            try
            {
                var modelo = _context.TProveedor.Find(id);
                if (modelo == null) throw new Exception("Proveedor no encontrado");
                modelo.Activo = true;
                modelo.UsuarioModificacion = "admin";
                modelo.FechaModificacion = DateTime.Now;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("ProveedorRepository - Activar: " + ex.Message, ex);
            }
        }
    }
}