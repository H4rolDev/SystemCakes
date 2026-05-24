using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using H.DataAccess;

namespace H.DataAccess.Repositorios
{
    // crud
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        TEntity GetById(int id);
        IEnumerable<TEntity> GetById(List<int> listadoId);
        IQueryable<TEntity> GetAll();
        ICollection<TEntity> GetAllByPageIndex(int pageIndex, int pageSize);
        IQueryable<TEntity> GetAllByFiltersByPageIndex(string filter, int pageIndex, int pageSize);
        
        int Add(TEntity entidad);
        int Delete(int id, string usuario);
        int Update(TEntity entidad);

        bool Add(IEnumerable<TEntity> list);
        bool Update(IEnumerable<TEntity> list);
        bool Delete(IEnumerable<int> list, string usuario);

        IEnumerable<TEntity> GetBy(Expression<Func<TEntity, bool>> filter);
        ICollection<TType> GetBy<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select)
            where TType : class;

        bool Exist(int id);
        bool Exist(Expression<Func<TEntity, bool>> filter);

        TEntity FirstBy(Expression<Func<TEntity, bool>> filter);
        TType FirstBy<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select)
            where TType : class;

        public TType FirstByGeneral<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select)
            where TType : class;
	}
}
