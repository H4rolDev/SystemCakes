using Microsoft.EntityFrameworkCore;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.DataAccess.Infrastructure;
using System.Linq.Expressions;
using H.DataAccess;
using H.DataAccess.Log;

namespace H.DataAccess.Repositorios
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        internal readonly IConnectionFactory connectionFactory;
        private sistemContext context;
        internal DbSet<TEntity> entities;
        //Enlazar a EF

        public GenericRepository(sistemContext context, IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            this.context = context;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            entities = context.Set<TEntity>();
        }

        public int Add(TEntity entidad)
        {
            try
            {
                entities.Add(entidad);
                return entidad.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = ex.Message;
                LogErp.EscribirDisco(error);
                throw ex;
            }          
        }

        public int Update(TEntity entidad)
        {
            try
            {
                entities.Update(entidad);
                return entidad.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = ex.Message;
                LogErp.EscribirDisco(error);
                throw ex;
            }
        }

        public int Delete(int id, string usuario)
        {
            try
            {
                var entidad = FirstBy(w => w.Id == id && w.Activo == true);
                if (entidad == null)
                    throw new ArgumentNullException($"La Entidad id: {id}, es nula y/o ya fue eliminada");
                if (string.IsNullOrEmpty(usuario) || (usuario != null && usuario.Trim() == ""))
                    throw new ArgumentNullException("El nombre de usuario es nulo y/o no se proporcionó");

                if ((bool)typeof(TEntity).GetProperty("Activo").GetValue(entidad) == false)
                    throw new ArgumentNullException($"Elemento id: {id}, ya fue eliminado previamente");

                typeof(TEntity).GetProperty("Activo").SetValue(entidad, false);
                typeof(TEntity).GetProperty("UsuarioModificacion").SetValue(entidad, usuario);
                typeof(TEntity).GetProperty("FechaModificacion").SetValue(entidad, DateTime.UtcNow.AddHours(-5));

                entities.Update(entidad);
                return entidad.Id;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = ex.Message;
                LogErp.EscribirDisco(error);
                throw ex;
            }
        }

        public bool Add(IEnumerable<TEntity> list)
        {
            try
            {
                if (list == null)
                    throw new ArgumentNullException("El listado a Insertar es nulo");

                entities.AddRange(list);
                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = ex.Message;
                LogErp.EscribirDisco(error);
                throw ex;
            }
        }

        public bool Update(IEnumerable<TEntity> list)
        {
            try
            {
                if (list == null)
                    throw new ArgumentNullException("El listado a Actualizar es nulo");
                entities.UpdateRange(list);
                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = ex.Message;
                LogErp.EscribirDisco(error);
                throw ex;
            }
        }

        public bool Delete(IEnumerable<int> list, string usuario)
        {
            try
            {
                foreach (var id in list)
                {
                    int resultado = Delete(id, usuario);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                var error = new Error();
                error.Message = ex.Message;
                LogErp.EscribirDisco(error);
                throw ex;
            }
        }


        public IEnumerable<TEntity> GetBy(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return entities.AsNoTracking().Where(w => w.Activo == true).Where(filter).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		public IEnumerable<TEntity> GetBySinActivo(Expression<Func<TEntity, bool>> filter)
		{
			try
			{
				return entities.AsNoTracking().Where(filter).ToList();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public ICollection<TType> GetBy<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select) where TType : class
        {
            try
            {
                return entities.AsNoTracking().Where(w => w.Activo == true).Where(where).Select(select).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Exist(int id)
        {
            try
            {
                return entities.Any(w => w.Id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Exist(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return entities.Where(w => w.Activo == true).Where(filter).Any();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IQueryable<TEntity> GetAll()
        {
			try
			{
				return entities.Where(w => w.Activo == true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public IQueryable<TEntity> GetAllByFiltersByPageIndex(string filter, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> GetAllByPageIndex(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public TEntity GetById(int id)
        {
            try
            {
                if (!Exist(id))
                    throw new Exception($"La entidad con Id {id} de {typeof(TEntity)} no existe");

                TEntity entidad = entities.AsNoTracking().FirstOrDefault(w => w.Id == id);

                return entidad;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<TEntity> GetById(List<int> listadoId)
        {
            try
            {
                if (listadoId == null || listadoId.Count == 0)
                    throw new Exception($"No se envió ids para buscar");

                return entities.AsNoTracking().Where(w => w.Activo == true).Where(w => listadoId.Contains(w.Id)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TEntity FirstBy(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return entities.AsNoTracking().Where(w => w.Activo == true).Where(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TType FirstBy<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select)
            where TType : class
        {
            try
            {
                return entities.AsNoTracking().Where(w => w.Activo == true).Where(where).Select(select).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		public TType FirstByGeneral<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select)
	where TType : class
		{
			try
			{
				return entities.AsNoTracking().Where(where).Select(select).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
