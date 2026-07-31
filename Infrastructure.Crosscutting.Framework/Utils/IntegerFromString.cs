using System;
using System.Linq;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public static class IntegerFromString
    {
        public static int GetInteger(string incomingString)
        {
            var integerInString = string.Join("", incomingString.ToCharArray().Where(Char.IsDigit));

            return int.Parse(integerInString);
        }
    }
}
