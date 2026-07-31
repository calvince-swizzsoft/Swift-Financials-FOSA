using System.Linq;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public static class FNV
    {
        public static readonly int OffsetBasis = unchecked((int)2166136261);
        public static readonly int Prime = 16777619;

        /// <summary>
        /// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        /// </summary>
        /// <param name="objs">list of objects</param>
        /// <returns>FNV Hash</returns>
        public static int CreateHash(params object[] objs)
        {
            return objs.Aggregate(OffsetBasis, (r, o) => (r ^ o.GetHashCode()) * Prime);
        }
    }
}
