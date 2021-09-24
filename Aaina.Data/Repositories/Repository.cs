using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Aaina.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aaina.Data.Repositories
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        internal ApplicationDbContext Context;
        internal DbSet<TEntity> DbSet;
        private readonly IConfiguration configuration;

        public Repository(ApplicationDbContext context, IConfiguration configuration)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
            this.configuration = configuration;
        }

        public TEntity Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            DbSet.Add(entity);
            Context.SaveChanges();
            return entity;
        }


        //public TPrimaryKey InsertAndGetId(TEntity entity)
        //{
        //    if (entity == null)
        //    {
        //        throw new ArgumentNullException("entity");
        //    }
        //    DbSet.Add(entity);
        //    Context.SaveChanges();
        //    return entity;
        //}

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            DbSet.Add(entity);
            await Context.SaveChangesAsync();

            return entity;
        }


        public List<TEntity> InsertRange(List<TEntity> entities)
        {

            this.DbSet.AddRange(entities);
            this.Context.SaveChanges();
            return entities;

        }


        public async Task<List<TEntity>> InsertRangeAsyn(List<TEntity> entities)
        {

            this.DbSet.AddRange(entities);
            await this.Context.SaveChangesAsync();
            return entities;


        }


        public TEntity Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return entity;
        }

        public List<TEntity> UpdateRange(List<TEntity> entities)
        {
            DbSet.AttachRange(entities);
            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
            
            Context.SaveChanges();
            return entities;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return entity;

        }

        public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities)
        {
            DbSet.AttachRange(entities);
            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }

            await Context.SaveChangesAsync();
            return entities;
        }

        public void Delete(TEntity entity)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
            Context.SaveChanges();

        }
        public void DeleteRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);

            }

            Context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }
        public void Delete(TPrimaryKey id)
        {
            var entity = DbSet.Find(id);
            DbSet.Remove(entity);
            Context.SaveChanges();
        }
        public async Task DeleteAsync(TPrimaryKey id)
        {
            var entity = DbSet.Find(id);
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = GetAllList(predicate);

            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = await GetAllListAsync(predicate);

            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }


        public ApplicationDbContext GetContext()
        {
            return Context;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
        public string GetOpenConnection()
        {
            return configuration["ConnectionStrings:EROCDB"];
        }
        //public void ChangeEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class
        //{
        //    Context.Entry(entity).State = ConvertState(state);
        //}
        //public void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, EntityState state) where T : class
        //{
        //    foreach (T entity in entityCollection.ToList())
        //    {
        //        Context.Entry(entity).State = ConvertState(state);
        //    }
        //}


        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = DbSet.Where(filter);
            return query;
        }
        public IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = DbSet;
            return query;
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> filter)
        {
            return DbSet.Where(filter).ToList();
        }

        public List<TEntity> GetAllList()
        {
            return DbSet.ToList();
        }

        public TEntity Get(TPrimaryKey id)
        {
            return DbSet.Find(id);
        }

        public async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return await DbSet.FindAsync(id);
        }




        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors != null)
            {
                return GetAll();
            }

            var query = GetAll();

            foreach (var propertySelector in propertySelectors)
            {
                //TODO: Test if NHibernate supports multiple fetch.
                //    query = query.Fetch(propertySelector);
            }

            return query;
        }

        public IQueryable<TEntity> GetAllIncluding(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null)
        {
            IQueryable<TEntity> queryable = this.GetAll().Where(match);
            IQueryable<TEntity> result = includeMembers(queryable);
            return result;
        }

        public async Task<List<TEntity>> GetAllIncludingAsyn(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null)
        {
            IQueryable<TEntity> queryable = this.GetAll().Where(match);
            IQueryable<TEntity> result = includeMembers(queryable);
            return await result.ToListAsync();
        }


        public async Task<TEntity> GetIncludingByIdAsyn(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null)
        {
            IQueryable<TEntity> queryable = this.GetAll().Where(match);
            IQueryable<TEntity> result;
            if (includeMembers != null)
            {
                result = includeMembers(queryable);
            }
            else
            {
                result = queryable;
            }
            return await result.FirstOrDefaultAsync();
        }

        public TEntity GetIncludingById(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null)
        {
            IQueryable<TEntity> queryable = this.GetAll().Where(match);
            IQueryable<TEntity> result;
            if (includeMembers != null)
            {
                result = includeMembers(queryable);
            }
            else
            {
                result = queryable;
            }
            return result.FirstOrDefault();
        }

        public TEntity GetIncludingLast(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null)
        {
            IQueryable<TEntity> queryable = this.GetAll().Where(match);
            IQueryable<TEntity> result;
            if (includeMembers != null)
            {
                result = includeMembers(queryable);
            }
            else
            {
                result = queryable;
            }
            return result.LastOrDefault();
        }

        public Task<List<TEntity>> GetAllListAsync()
        {
            return GetAll().ToListAsync();
        }

        public Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToListAsync();
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).SingleAsync();
        }
        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).Single();
        }

        public TEntity FirstOrDefault(TPrimaryKey id)
        {
            return DbSet.Where(CreateEqualityExpressionForId(id)).FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return DbSet.Where(CreateEqualityExpressionForId(id)).FirstOrDefaultAsync();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).FirstOrDefault();
        }

        public TEntity FirstOrDefault()
        {
            return DbSet.FirstOrDefault();
        }

        public TEntity FirstOrDefaultWithWhere(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).FirstOrDefault();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public TEntity Load(TPrimaryKey id)
        {
            return Get(id);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }


        public Task<int> CountAsync()
        {
            return GetAll().CountAsync();
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().CountAsync(predicate);
        }

        public bool Any()
        {
            return GetAll().Any();
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Any(predicate);
        }
        public Task<bool> AnyAsync()
        {
            return GetAll().AnyAsync();
        }
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().AnyAsync(predicate);
        }

        public Task<long> LongCountAsync()
        {
            return GetAll().LongCountAsync();
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().LongCountAsync(predicate);
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().LongCount(predicate);
        }

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var leftExpression = Expression.PropertyOrField(lambdaParam, "Id");

            var idValue = Convert.ChangeType(id, typeof(TPrimaryKey));

            Expression<Func<object>> closure = () => idValue;
            var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

            var lambdaBody = Expression.Equal(leftExpression, rightExpression);

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        private EntityState ConvertState(EntityState state)
        {
            switch (state)
            {
                case EntityState.Added:
                    return EntityState.Added;
                case EntityState.Deleted:
                    return EntityState.Deleted;
                case EntityState.Modified:
                    return EntityState.Modified;
                default:
                    return EntityState.Unchanged;
            }
        }

    }
}
