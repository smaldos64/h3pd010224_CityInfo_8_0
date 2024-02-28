using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Entities;
using Contracts;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    { 
        protected DatabaseContext RepositoryContext { get; set; }
        internal DbSet<T> dbSet;

        public RepositoryBase(DatabaseContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
            this.dbSet = this.RepositoryContext.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> FindAll()
        {
#if (ENABLED_FOR_LAZY_LOADING_USAGE)
            return await RepositoryContext.Set<T>().ToListAsync();
#else
            return await this.RepositoryContext.Set<T>().AsNoTracking().ToListAsync();
#endif
        }

        public virtual async Task<T> FindOne(int id)
        {
#if (ENABLED_FOR_LAZY_LOADING_USAGE)
            return await this.RepositoryContext.Set<T>().FindAsync(id);
#else
            var entity = await this.RepositoryContext.Set<T>().FindAsync(id);
            this.RepositoryContext.Entry(entity).State = EntityState.Detached;
            return entity;
#endif
        }

    public virtual async Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> expression,
                                                              bool UseIQueryable = false)
    {
      ParameterExpression s = Expression.Parameter(typeof(T));

      if (false == UseIQueryable)
      {
#if (ENABLED_FOR_LAZY_LOADING_USAGE)
        return await this.RepositoryContext.Set<T>().Where(expression).ToListAsync();
#else
        return await this.RepositoryContext.Set<T>().Where(expression).AsNoTracking().ToListAsync();
#endif
      }
      else
      {
        IQueryable<T> Query = dbSet;
#if (ENABLED_FOR_LAZY_LOADING_USAGE)
        return await Query.Where(expression).ToListAsync();
#else
        return await Query.Where(expression).AsNoTracking().ToListAsync();
#endif
      }
    }

    public virtual async Task<IQueryable<T>> FindByConditionReturnIQueryable(Expression<Func<T, bool>> expression)
    {
      IQueryable<T> Query = dbSet;
#if (ENABLED_FOR_LAZY_LOADING_USAGE)
      //return await Query.Where(expression).AsQueryable<T>();
      var Data = await Query.Where(expression).ToListAsync();
      return (Data.AsQueryable<T>());

#else
      var Data = await Query.Where(expression).AsNoTracking().ToListAsync();
      return (Data.AsQueryable<T>());
      //return await Query.Where(expression).AsNoTracking().ToListAsync();
#endif
    }

    //public virtual List<T> FindByCondition(Expression<Func<T, bool>> expression)
    //{
    //    //ParameterExpression s = Expression.Parameter(typeof(T));
    //    return this.RepositoryContext.Set<T>().Where(expression).AsNoTracking().ToList();
    //}

    public virtual async Task Create(T entity)
    {
        await this.RepositoryContext.Set<T>().AddAsync(entity);
        //await this.Save();
    }

    public virtual async Task Update(T entity)
    {
        // Skal laves asynkron i linjen herunder. Men UpdateAsync findes ikke !!!
        try
        {
            this.RepositoryContext.Set<T>().Update(entity);
            //await this.Save();
        }
        catch (Exception Error)
        {
            string ErrorString = Error.ToString();
        }
    }

    public virtual async Task Delete(T entity)
    {
        this.RepositoryContext.Set<T>().Remove(entity);
        //await this.Save();
    }

    public virtual async Task<int> Save()
    {
        int NumberOfObjectsChanged = -1;
        NumberOfObjectsChanged = await this.RepositoryContext.SaveChangesAsync();
        return NumberOfObjectsChanged;
    }

    public virtual void EnableLazyLoading()
    {
        this.RepositoryContext.ChangeTracker.LazyLoadingEnabled = true;
    }

    public virtual void DisableLazyLoading()
    {
        this.RepositoryContext.ChangeTracker.LazyLoadingEnabled = false;
    }
  }
}
