using Authorizer.Infrastructure.Specification;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hyperion.Infrastructure.Database
{
    public interface IRepository<T>
    {
        Task Save(T entity);
        Task Delete(T entity);
        Task Update(T entity);
        IQueryable<T> Query { get; }
        Task<T> Get(object id);
        Task<IEnumerable<T>> GetAllByCriteria(ISpecification<T> specification);
        Task<T> GetOneByCriteria(ISpecification<T> specification);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllWithOrderBy<Tkey>(Expression<Func<T, Tkey>> order, bool descending = false);
        ITransaction CreateTransaction();
        ITransaction CreateTransaction(System.Data.IsolationLevel isolation = System.Data.IsolationLevel.Serializable);
        void SaveChanges();
        Task Evict(T entity);
        Task Merge(T entity);
    }
}
