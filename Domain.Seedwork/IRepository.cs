using Application.MainBoundedContext.DTO;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Seedwork
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        TEntity Get(Guid entityId, ServiceHeader serviceHeader);

        TTarget Get<TTarget>(Guid entityId, ServiceHeader serviceHeader) where TTarget : class;

        Task<TEntity> GetAsync(Guid entityId, ServiceHeader serviceHeader);

        Task<TTarget> GetAsync<TTarget>(Guid entityId, ServiceHeader serviceHeader) where TTarget : class;

        void Add(TEntity entity, ServiceHeader serviceHeader);

        void Merge(TEntity original, TEntity current, ServiceHeader serviceHeader);

        void Remove(TEntity entity, ServiceHeader serviceHeader);

        List<TEntity> GetAll(ServiceHeader serviceHeader);

        List<TTarget> GetAll<TTarget>(ServiceHeader serviceHeader) where TTarget : class;

        Task<List<TEntity>> GetAllAsync(ServiceHeader serviceHeader);

        Task<List<TTarget>> GetAllAsync<TTarget>(ServiceHeader serviceHeader) where TTarget : class;

        List<TEntity> AllMatching(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths);

        List<TTarget> AllMatching<TTarget>(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class;

        Task<List<TEntity>> AllMatchingAsync(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths);

        Task<List<TTarget>> AllMatchingAsync<TTarget>(ISpecification<TEntity> specification, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class;

        PageCollectionInfo<TEntity> AllMatchingPaged(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths);

        PageCollectionInfo<TTarget> AllMatchingPaged<TTarget>(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class;

        Task<PageCollectionInfo<TEntity>> AllMatchingPagedAsync(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths);

        Task<PageCollectionInfo<TTarget>> AllMatchingPagedAsync<TTarget>(ISpecification<TEntity> specification, int pageIndex, int pageSize, List<string> sortFields, bool ascending, ServiceHeader serviceHeader, params Expression<Func<TEntity, object>>[] paths) where TTarget : class;

        int AllMatchingCount(ISpecification<TEntity> specification, ServiceHeader serviceHeader);

        Task<int> AllMatchingCountAsync(ISpecification<TEntity> specification, ServiceHeader serviceHeader);

        IEnumerable<TEntity> DbSetSqlQuery(string sql, ServiceHeader serviceHeader, params object[] parameters);

        Task<List<TEntity>> DbSetSqlQueryAsync(string sql, ServiceHeader serviceHeader, params object[] parameters);

        IEnumerable<TElement> DatabaseSqlQuery<TElement>(string sql, ServiceHeader serviceHeader, params object[] parameters);

        Task<List<TElement>> DatabaseSqlQueryAsync<TElement>(string sql, ServiceHeader serviceHeader, params object[] parameters);

        int DatabaseExecuteSqlCommand(string sql, ServiceHeader serviceHeader, params object[] parameters);

        Task<int> DatabaseExecuteSqlCommandAsync(string sql, ServiceHeader serviceHeader, params object[] parameters);

        string Pluralize();

        string Pluralize<TTarget>() where TTarget : class;

        Task<int> CountAllAsync(ServiceHeader serviceHeader);
    }
}
