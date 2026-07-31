using System;

namespace Infrastructure.Crosscutting.Framework.Adapter
{
    public static class TypeAdapterExtensions
    {
        public static TProjection ProjectedAs<TProjection>(this object item)
            where TProjection : class, new()
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var adapter = TypeAdapterFactory.CreateAdapter();

            return adapter.Adapt<TProjection>(item);
        }
    }
}
