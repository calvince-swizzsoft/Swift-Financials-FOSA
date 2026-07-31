using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.ExpressionTreeSerialization
{
    public static class ExpressionTreeSerializationUtils
    {
        public static string CustomAttributeName
        {
            get { return "Key"; }
        }

        public static Expression<Func<TEntity, bool>> CreateExpressionByKey<TEntity, TKeyAttribute>(TEntity filtered)
        {
            var entityType = typeof(TEntity);
            var pe = Expression.Parameter(typeof(TEntity), "ba");
            var notFirst = false;
            Expression predicate = null;

            foreach (var pi in entityType.GetProperties().Where(p => p.GetCustomAttributes(typeof(TKeyAttribute), true).Count() != 0).ToList())
            {
                Expression left = Expression.Property(pe, typeof(TEntity).GetProperty(pi.Name));
                Expression right = Expression.Constant(pi.GetValue(filtered, new object[] { }), pi.PropertyType);
                Expression expre = Expression.Equal(left, right);

                if (notFirst)
                    predicate = Expression.And(predicate, expre);
                else
                {
                    notFirst = true;
                    predicate = expre;
                }
            }
            return Expression.Lambda<Func<TEntity, bool>>(predicate, true, new[] { pe });
        }

        public static Expression<Func<TEntity, bool>> CreateExpressionByFilter<TEntity, TKeyAttribute>(TEntity filtered)
        {
            var entityType = typeof(TEntity);
            var pe = Expression.Parameter(typeof(TEntity), "ba");
            var notFirst = false;
            Expression predicate = null;

            foreach (var expre in from pi in entityType.GetProperties().Where(p => !p.GetCustomAttributes(typeof(TKeyAttribute), true).Any()).ToList() where pi.PropertyType.IsValueType && (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(long) || pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(bool)) let left = Expression.Property(pe, typeof(TEntity).GetProperty(pi.Name)) let right = Expression.Constant(pi.GetValue(filtered, new object[] { }), pi.PropertyType) select Expression.Equal(left, right))
            {
                if (notFirst)
                    predicate = Expression.And(predicate, expre);
                else
                {
                    notFirst = true;
                    predicate = expre;
                }
            }
            return Expression.Lambda<Func<TEntity, bool>>(predicate, true, new[] { pe });
        }

        #region Type Utils

        /// <summary>
        /// Check if is Nullable
        /// </summary>
        public static bool IsNullable(Type type)
        {
            return type == null || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Check if is IEnumerable<>
        /// </summary>
        public static bool IsGenericEnumerable(Type type)
        {
            var genArgs = type.GetGenericArguments();
            if (genArgs.Length == 1 && typeof(IEnumerable<>).MakeGenericType(genArgs).IsAssignableFrom(type))
                return true;
            else
                return type.BaseType != null && IsGenericEnumerable(type.BaseType);
        }

        #endregion

        #region OrderBy Reflection

        public static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods().Where(method => method.Name == "OrderBy").Single(method => method.GetParameters().Length == 2);
        public static readonly MethodInfo OrderByDescending = typeof(Queryable).GetMethods().Where(method => method.Name == "OrderByDescending").Single(method => method.GetParameters().Length == 2);

        public static IQueryable<TSource> CallOrderBy<TSource>(IQueryable<TSource> source, string propertyName, bool ascending)
        {
            var orderMethod = ascending ? OrderByMethod : OrderByDescending;
            var parameter = Expression.Parameter(typeof(TSource));
            Expression orderByProperty = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(orderByProperty, new[] { parameter });

            var genericMethod = orderMethod.MakeGenericMethod(new[] { typeof(TSource), orderByProperty.Type });
            var ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<TSource>)ret;
        }

        #endregion
    }
}
