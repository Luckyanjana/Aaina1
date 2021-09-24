using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Aaina.Data.Repositories
{    
    public interface IRepository<TEntity, TPrimaryKey>  where TEntity : class
    {

        int Count();
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync();
                bool Any();
        bool Any(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync();
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
                void Delete(TEntity entity);
        void DeleteRange(List<TEntity> entities);
        void Delete(TPrimaryKey id);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(TPrimaryKey id);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(TEntity entity);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault();
        TEntity FirstOrDefaultWithWhere(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(TPrimaryKey id);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);
        TEntity Get(TPrimaryKey id);
        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter);
        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        IQueryable<TEntity> GetAllIncluding(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null);

        Task<List<TEntity>> GetAllIncludingAsyn(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null);
        Task<TEntity> GetIncludingByIdAsyn(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null);
        TEntity GetIncludingById(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, 
            IQueryable<TEntity>> includeMembers = null);

        TEntity GetIncludingLast(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null);
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> GetAllList();
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllListAsync();
        Task<TEntity> GetAsync(TPrimaryKey id);
        TEntity Insert(TEntity entity);
        // TPrimaryKey InsertAndGetId(TEntity entity);
        // Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);
        List<TEntity> InsertRange(List<TEntity> entities);
                Task<List<TEntity>> InsertRangeAsyn(List<TEntity> entities);
        Task<TEntity> InsertAsync(TEntity entity);
       // TEntity InsertOrUpdate(TEntity entity);
       // TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);
      //  Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);
      //  Task<TEntity> InsertOrUpdateAsync(TEntity entity);
        TEntity Load(TPrimaryKey id);
        long LongCount(Expression<Func<TEntity, bool>> predicate);
        long LongCount();
        Task<long> LongCountAsync();
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Update(TEntity entity);
        // TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);
        // Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction);
        List<TEntity> UpdateRange(List<TEntity> entities);
        Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        ApplicationDbContext GetContext();
        void SaveChanges();
        string GetOpenConnection();

        //void ChangeEntityState<TEntity>(TEntity entity, EntityState state);

        //void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, EntityState state);
    }
}