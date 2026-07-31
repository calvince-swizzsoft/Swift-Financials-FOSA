using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public class Converters
    {
        #region Public Methods

        public static List<T> ConvertStringToEnum<T>(string stringArray)
        {
            var permissions = stringArray.Split('|').Select(permission => Convert.ToInt32(permission));

            return (List<T>)permissions.Select(r => (T)Enum.Parse(typeof(T), r.ToString(CultureInfo.InvariantCulture)));
        }

        #endregion
    }
}
