using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.Interfaces
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        IEnumerable<T> ListAll();
        Task<List<T>> ListAllAsync();

        //Task<List<T>> ListAsync(ISpecification<T> spec);
        Task<List<T>> WhereAsync(Expression<Func<T, bool>> expression);
        T FistOrDefault(Expression<Func<T, bool>> expression);
        Task<T> FistOrDefaultAsync(Expression<Func<T, bool>> expression);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);
        Task UpdateManyAsync(List<T> entities);

        Task DeleteAsync(T entity);
        Task DeleteManyAsync(List<T> entities);

        IQueryable<T> AsQueryable(Expression<Func<T, bool>> expression);
        IQueryable<T> AsQueryableAsync(Expression<Func<T, bool>> expression);
    }
}
