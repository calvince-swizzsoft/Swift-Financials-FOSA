using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Infrastructure.Util
{
    public static class JsonMapper
    {
        public static TTarget MapTo<TTarget>(this object source)
            where TTarget : class
        {
            var json = JsonConvert.SerializeObject(source);

            var target = JsonConvert.DeserializeObject<TTarget>(json);

            return target;
        }

        public static TTarget MapTo<TTarget, TSource>(TSource source) where TTarget : class where TSource : class, new()
        {
            var json = JsonConvert.SerializeObject(source);

            var target = JsonConvert.DeserializeObject<TTarget>(json);

            return target;
        }
    }
}
