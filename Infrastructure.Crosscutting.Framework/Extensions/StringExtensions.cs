using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Infrastructure.Crosscutting.Framework.Extensions
{
    public static class StringExtensions
    {
        private static readonly HashSet<char> _base64Characters = new HashSet<char>()
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/',
            '='
        };

        public static string ToTitleCase(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
#if !SILVERLIGHT
                var textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

                str = textInfo.ToTitleCase(str.Trim().ToLower());
#endif
                return str;
            }
            else return string.Empty;
        }

        public static string Limit(this string str, int characterCount)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.Length <= characterCount)
                    return str.PadRight(characterCount, ' ');

                else return str.Substring(0, characterCount).PadRight(characterCount, ' ');
            }
            else return string.Empty;
        }

        public static string StripPunctuation(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;
            else
                return new string(s.ToArray().Where(c => !char.IsPunctuation(c)).ToArray()).TrimStart(new char[] { '\r', '\n' }).TrimEnd(new char[] { '\r', '\n' })
                    .Replace('\n', ' ')
                    .Replace('\r', ' ')
                    .Replace('\\', ' ')
                    .Replace('/', ' ')
                    .Replace(':', ' ')
                    .Replace('*', ' ')
                    .Replace('?', ' ')
                    .Replace('<', ' ')
                    .Replace('>', ' ')
                    .Replace('|', ' ')
                    .Replace('%', ' ')
                    .Replace('^', ' ')
                    .Replace('[', ' ')
                    .Replace(']', ' ')
                    .Replace('_', ' ')
                    .Replace('\'', ' ');
        }

        public static string ReplaceNumbers(this string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                return Regex.Replace(input, "[0-9]", "X");
            }
            else return input;
        }

        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }

        public static string ExtractInitials(this string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                text = regex.Replace(text, " "); // ensure single spancing! 

                var result = text.Trim().Split(new char[] { ' ' }).Select(y => y[0]);

                return String.Join<Char>(".", result);
            }
            else return text;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (!string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(toCheck))
            {
                return source.IndexOf(toCheck, comp) >= 0;
            }
            else return false;
        }

        public static string Left(this string str, int length)
        {
            str = (str ?? string.Empty);

            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string Right(this string str, int length)
        {
            str = (str ?? string.Empty);

            return (str.Length >= length) ? str.Substring(str.Length - length, length) : str;
        }

        public static string SanitizePatIndexInput(this string str)
        {
            str = (str ?? string.Empty);

            if (Regex.IsMatch(str, "(^%(.*)%$)", RegexOptions.IgnoreCase))
            {
                return str;
            }
            else if (str.Contains("%") || str.Contains("_") || str.Contains("[") || str.Contains("]") || str.Contains("^"))
            {
                return str;
            }
            else return string.Format("%{0}%", str);
        }

        public static bool IsGuid(this string str)
        {
            str = (str ?? string.Empty);

            Guid x;

            return Guid.TryParse(str, out x);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Encrypts a given password and returns the encrypted data
        /// as a base64 string.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(this string plainText)
        {
            if (!string.IsNullOrWhiteSpace(plainText) && !plainText.IsBase64String())
            {
                //encrypt data
                var data = Encoding.Unicode.GetBytes(plainText);
                byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);

                //return as base64 string
                return Convert.ToBase64String(encrypted);
            }
            else return plainText;
        }

        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns></returns>
        public static string Decrypt(this string cipher)
        {
            if (!string.IsNullOrWhiteSpace(cipher) && cipher.IsBase64String())
            {
                //parse base64 string
                byte[] data = Convert.FromBase64String(cipher);

                //decrypt data
                byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);

                return Encoding.Unicode.GetString(decrypted);
            }
            else return cipher;
        }

        public static bool IsBase64String(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else if (value.Any(c => !_base64Characters.Contains(c)))
            {
                return false;
            }
            else if (value.Length < 32)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(value);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
#endif
    }
}
