using AuthServer.Domain.Interfaces;
using AuthServer.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Infrastructure.Repository
{
    public class Repository<T> : IAsyncRepository<T>, IDisposable where T : class
    {
        protected readonly AppDBContext _context;

        public Repository(AppDBContext context)
        {
            _context = context;
        }

        public virtual T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        //public T GetSingleBySpec(ISpecification<T> spec)
        //{
        //    return List(spec).FirstOrDefault();
        //}

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _context.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteManyAsync(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public async Task<List<T>> WhereAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>()
                            .Where(expression)
                            .ToListAsync();
        }

        public IQueryable<T> AsQueryable(Expression<Func<T, bool>> expression)
        {
            if (expression != null)
            {
                return _context.Set<T>()
                                .Where(expression);
            }
            else
            {
                return _context.Set<T>().AsQueryable();
            }
        }

        public IQueryable<T> AsQueryableAsync(Expression<Func<T, bool>> expression)
        {
            if (expression != null)
            {
                return _context.Set<T>()
                                .Where(expression);
            }
            else
            {
                return _context.Set<T>().AsQueryable();
            }
        }

        public async Task<T> FistOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>()
                            .FirstOrDefaultAsync(expression);
        }

        public T FistOrDefault(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>()
                            .FirstOrDefault(expression);
        }

        public async Task UpdateManyAsync(List<T> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                _context.Entry(entities[i]).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }
    }
}
