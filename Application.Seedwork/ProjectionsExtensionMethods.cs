using Application.MainBoundedContext.DTO;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.ExpressionTreeSerialization;
using System.Collections.Generic;
using System.Linq;

namespace Application.Seedwork
{
    public static class ProjectionsExtensionMethods
    {
        /// <summary>
        /// Project a type using a DTO
        /// </summary>
        /// <typeparam name="TProjection">The dto projection</typeparam>
        /// <param name="entity">The source entity to project</param>
        /// <returns>The projected type</returns>
        public static TProjection ProjectedAs<TProjection>(this Entity item)
            where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();

            return adapter.Adapt<TProjection>(item);
        }

        /// <summary>
        /// projected a enumerable collection of items
        /// </summary>
        /// <typeparam name="TProjection">The dtop projection type</typeparam>
        /// <param name="items">the collection of entity items</param>
        /// <returns>Projected collection</returns>
        public static List<TProjection> ProjectedAsCollection<TProjection>(this IEnumerable<Entity> items)
            where TProjection : class, new()
        {

            var adapter = TypeAdapterFactory.CreateAdapter();

            return adapter.Adapt<List<TProjection>>(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectSet"></param>
        /// <param name="specification"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortFields"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public static PageCollectionInfo<T> AllMatchingPaged<T>(IQueryable<T> objectSet, ISpecification<T> specification, int pageIndex, int pageSize, IList<string> sortFields, bool ascending)
              where T : class
        {
            IQueryable<T> items = null;

            var totalItems = 0;

            items = objectSet.Where(specification.SatisfiedBy());

            totalItems = items.Count();

            if (pageSize != 0)
            {
                if (sortFields != null && sortFields.Any())
                {
                    sortFields.ToList().ForEach(s => items = ExpressionTreeSerializationUtils.CallOrderBy(items, s, ascending));

                    items = items.Skip(/*pageSize */ pageIndex); // NB: orderby must be called before skip(..)
                }

                items = items.Take(pageSize);
            }

            return new PageCollectionInfo<T> { PageIndex = pageIndex, PageSize = pageSize, PageCollection = items.ToList(), ItemsCount = totalItems };
        }

        public static int AllMatchingCount<T>(IQueryable<T> objectSet, ISpecification<T> specification)
           where T : class
        {
            IQueryable<T> items = null;

            items = objectSet.Where(specification.SatisfiedBy());

            return items.Count();
        }
    }
}
