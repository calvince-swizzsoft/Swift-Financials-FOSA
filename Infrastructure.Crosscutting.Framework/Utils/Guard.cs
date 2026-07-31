using System;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public static class Guard
    {
        public static void ArgumentNotNull<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
