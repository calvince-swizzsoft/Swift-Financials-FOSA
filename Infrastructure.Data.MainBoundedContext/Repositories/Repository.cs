using Application.MainBoundedContext.DTO;
using AutoMapper.QueryableExtensions;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.ExpressionTreeSerialization;
using Infrastructure.Crosscutting.Framework.Utils;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.MainBoundedContext.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public Repository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null)
                throw new ArgumentNullException(nameof(ambientDbContextLocator));

            _ambientDbContextLocator = ambientDbContextLocator;
        }

        public TEntity Get(Guid entityId, ServiceHeader serviceHeader)
        {
            return GetDbContext(serviceHeader).Set<TEntity>().Find(entityId);
        }

        public TTarget Get<TTarget>(Guid entityId, ServiceHeader serviceHeader) where TTarget : class
        {
            var dbSet = GetDbContext(serviceHeader).Set<TEntity>().Where(x => x.Id == entityId);

            var projection = dbSet.ProjectTo<TTarget>().ToList();

            return projection.FirstOrDefault();
        }

        public async Task<TEntity> GetAsync(Guid entityId, ServiceHeader serviceHeader)
        {
            return await GetDbContext(serviceHeader).Set<TEntity>().FindAsync(entityId);
        }

        public async Task<TTarget> GetAsync<TTarget>(Guid entityId, ServiceHeader serviceHeader) where TTarget : class
        {
            var dbSet = GetDbContext(serviceHeader).Set<TEntity>().Where(x => x.Id == entityId);

            var projection = await dbSet.ProjectTo<TTarget>().ToListAsync();

            return projection.FirstOrDefault();
        }

        public void Add(TEntity entity, ServiceHeader serviceHeader)
        {
            GetDbContext(serviceHeader).Set<TEntity>().Add(entity);
        }

        public void Merge(TEntity original, TEntity current, ServiceHeader serviceHeader)
        {
            GetDbContext(serviceHeader).Entry(original).CurrentValues.SetValues(current);
        }

        public void Remove(TEntity entity, ServiceHeader serviceHeader)
        {
            GetDbContext(serviceHeader).Set<TEntity>().Remove(entity);
        }

        public List<TEntity> GetAll(ServiceHeader serviceHeader)
        {
            return GetDbContext(serviceHeader).Set<TEntity>().ToList();
        }

        public List<TTarget> GetAll<TTarget>(ServiceHeader serviceHeader) where TTarget : class
        {
            var dbSet = GetDbContext(serviceHeader).Set<TEntity>();

            var projection = dbSet.ProjectTo<TTarget>().ToList();

            return projection;
        }

        public async Task<List<TEntity>> GetAllAsync(ServiceHeader serviceHeader)
        {
            return await GetDbContext(serviceHeader).Set<TEntity>().ToListAsync();
        }

        public async Task<List<TTarget>> GetAllAsync<TTarget>(ServiceHeader serviceHeader) where TTarget : class
        {
            var dbSet = GetDbContext(serviceHeader).Set<TEntity>();

            var projection = await dbSet.ProjectTo<TTarget>().ToListAsync();

            return projection;
        }

        public List<TEntity> AllMatching(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths)
        {
            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            return items.ToList();
        }

        public List<TTarget> AllMatching<TTarget>(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class
        {
            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            return items.ProjectTo<TTarget>().ToList();
        }

        public async Task<List<TEntity>> AllMatchingAsync(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths)
        {
            List<TEntity> source = null;

            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            if (items != null)
            {
                source = await items.ToListAsync();
            }

            return source;
        }

        public async Task<List<TTarget>> AllMatchingAsync<TTarget>(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class
        {
            List<TTarget> source = null;

            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            if (items != null)
            {
                source = await items.ProjectTo<TTarget>().ToListAsync();
            }

            return source;
        }

        public PageCollectionInfo<TEntity> AllMatchingPaged(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths)
        {
            PageCollectionInfo<TEntity> pageCollection = null;

            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            if (items != null)
            {
                var totalItems = items.Count();

                if (pageSize != 0)
                {
                    if (sortFields != null && sortFields.Any())
                    {
                        sortFields.ForEach(s => items = ExpressionTreeSerializationUtils.CallOrderBy(items, s, ascending));

                        items = items.Skip(pageSize * pageIndex); // NB: orderby must be called before skip(..)
                    }

                    items = items.Take(pageSize);
                }

                pageCollection = new PageCollectionInfo<TEntity>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    PageCollection = items.ToList(),
                    ItemsCount = totalItems
                };
            }

            return pageCollection;
        }

        public PageCollectionInfo<TTarget> AllMatchingPaged<TTarget>(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class
        {
            PageCollectionInfo<TTarget> pageCollection = null;

            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            if (items != null)
            {
                var totalItems = items.Count();

                if (pageSize != 0)
                {
                    if (sortFields != null && sortFields.Any())
                    {
                        sortFields.ForEach(s => items = ExpressionTreeSerializationUtils.CallOrderBy(items, s, ascending));

                        items = items.Skip(pageSize * pageIndex); // NB: orderby must be called before skip(..)
                    }

                    items = items.Take(pageSize);
                }

                pageCollection = new PageCollectionInfo<TTarget>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    PageCollection = items.ProjectTo<TTarget>().ToList(),
                    ItemsCount = totalItems
                };
            }

            return pageCollection;
        }

        public async Task<PageCollectionInfo<TEntity>> AllMatchingPagedAsync(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths)
        {
            PageCollectionInfo<TEntity> source = null;

            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            if (items != null)
            {
                var totalItems = await items.CountAsync();

                if (pageSize != 0)
                {
                    if (sortFields != null && sortFields.Any())
                    {
                        sortFields.ForEach(s => items = ExpressionTreeSerializationUtils.CallOrderBy(items, s, ascending));

                        items = items.Skip(pageSize * pageIndex); // NB: orderby must be called before skip(..)
                    }

                    items = items.Take(pageSize);
                }

                var pageItems = await items.ToListAsync();

                source = new PageCollectionInfo<TEntity>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    PageCollection = pageItems,
                    ItemsCount = totalItems
                };
            }

            return source;
        }

        public async Task<PageCollectionInfo<TTarget>> AllMatchingPagedAsync<TTarget>(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class
        {
            PageCollectionInfo<TTarget> source = null;

            IQueryable<TEntity> items = null;

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            if (paths != null && paths.Any())
            {
                IQueryable<TEntity> entitySet = null;

                Array.ForEach(paths, path =>
                {
                    entitySet = (entitySet ?? objectSet).Include(path);
                });

                items = entitySet.Where(specification.SatisfiedBy());
            }
            else items = objectSet.Where(specification.SatisfiedBy());

            if (items != null)
            {
                var totalItems = await items.CountAsync();

                if (pageSize != 0)
                {
                    if (sortFields != null && sortFields.Any())
                    {
                        sortFields.ForEach(s => items = ExpressionTreeSerializationUtils.CallOrderBy(items, s, ascending));

                        items = items.Skip(pageSize * pageIndex); // NB: orderby must be called before skip(..)
                    }

                    items = items.Take(pageSize);
                }

                var pageItems = await items.ProjectTo<TTarget>().ToListAsync();

                source = new PageCollectionInfo<TTarget>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    PageCollection = pageItems,
                    ItemsCount = totalItems
                };
            }

            return source;
        }

        public int AllMatchingCount(ISpecification<TEntity> specification, ServiceHeader serviceHeader)
        {
            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            var items = objectSet.Where(specification.SatisfiedBy());

            return items != null ? items.Count() : 0;
        }

        public async Task<int> AllMatchingCountAsync(ISpecification<TEntity> specification, ServiceHeader serviceHeader)
        {
            var result = default(int);

            var objectSet = GetDbContext(serviceHeader).Set<TEntity>();

            var items = objectSet.Where(specification.SatisfiedBy());

            if (items != null)
            {
                result = await items.CountAsync();
            }

            return result;
        }

        public IEnumerable<TEntity> DbSetSqlQuery(string sql, ServiceHeader serviceHeader, params object[] parameters)
        {
            return GetDbContext(serviceHeader).Set<TEntity>().SqlQuery(sql, parameters);
        }

        public async Task<List<TEntity>> DbSetSqlQueryAsync(string sql, ServiceHeader serviceHeader, params object[] parameters)
        {
            return await GetDbContext(serviceHeader).Set<TEntity>().SqlQuery(sql, parameters).ToListAsync();
        }

        public IEnumerable<TElement> DatabaseSqlQuery<TElement>(string sql, ServiceHeader serviceHeader, params object[] parameters)
        {
            return GetDbContext(serviceHeader).Database.SqlQuery<TElement>(sql, parameters);
        }

        public async Task<List<TElement>> DatabaseSqlQueryAsync<TElement>(string sql, ServiceHeader serviceHeader, params object[] parameters)
        {
            return await GetDbContext(serviceHeader).Database.SqlQuery<TElement>(sql, parameters).ToListAsync();
        }

        public int DatabaseExecuteSqlCommand(string sql, ServiceHeader serviceHeader, params object[] parameters)
        {
            return GetDbContext(serviceHeader).Database.ExecuteSqlCommand(sql, parameters);
        }

        public async Task<int> DatabaseExecuteSqlCommandAsync(string sql, ServiceHeader serviceHeader, params object[] parameters)
        {
            return await Task.Run(() =>
            {
                var result = GetDbContext(serviceHeader).Database.ExecuteSqlCommand(sql, parameters);

                return result;
            });
        }

        public string Pluralize()
        {
            return (typeof(TEntity).Name.Pluralize()).Camelize();
        }

        public string Pluralize<TTarget>() where TTarget : class
        {
            return (typeof(TTarget).Name.Pluralize()).Camelize();
        }

        private BoundedContextUnitOfWork GetDbContext(ServiceHeader serviceHeader)
        {
            var dbContext = _ambientDbContextLocator.Get<BoundedContextUnitOfWork>(serviceHeader);

            if (dbContext == null)
                throw new InvalidOperationException("No ambient DbContext of type BoundedContextUnitOfWork found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");

            return dbContext;
        }

        public async Task<int> CountAllAsync(ServiceHeader serviceHeader)
        {
            return await GetDbContext(serviceHeader).Set<TEntity>().CountAsync();
        }
    }
}
