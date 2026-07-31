using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public static class UberUtil
    {
        public static string NumberToCurrencyText(decimal number)
        {
            // Round the value just in case the decimal value is longer than two digits
            number = decimal.Round(number, 2);

            string currencyName = "Shilling";
            string wordNumber = string.Empty;

            // Divide the number into the whole and fractional part strings
            string[] arrNumber = number.ToString().Split('.');

            // Get the whole number text
            long wholePart = long.Parse(arrNumber[0]);
            string strWholePart = NumberToText(wholePart);

            // For amounts of zero dollars show 'No Dollars...' instead of 'Zero Dollars...'
            wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? string.Format(" {0} and ", currencyName) : string.Format(" {0}s and ", currencyName));

            // If the array has more than one element then there is a fractional part otherwise there isn't
            // just add 'No Cents' to the end
            if (arrNumber.Length > 1)
            {
                // If the length of the fractional element is only 1, add a 0 so that the text returned isn't,
                // 'One', 'Two', etc but 'Ten', 'Twenty', etc.
                long fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));
                string strFarctionPart = NumberToText(fractionPart);

                wordNumber += (fractionPart == 0 ? "Zero" : strFarctionPart) + (fractionPart == 1 ? " Cent" : " Cents");
            }
            else wordNumber += "Zero Cents";

            return string.Format("{0} Only", wordNumber);
        }

#if ! SILVERLIGHT

        public static string NumberToCurrencyText(decimal number, MidpointRounding midpointRounding)
        {
            // Round the value just in case the decimal value is longer than two digits
            number = decimal.Round(number, 2, midpointRounding);

            string currencyName = "Shilling";
            string wordNumber = string.Empty;

            // Divide the number into the whole and fractional part strings
            string[] arrNumber = number.ToString().Split('.');

            // Get the whole number text
            long wholePart = long.Parse(arrNumber[0]);
            string strWholePart = NumberToText(wholePart);

            // For amounts of zero dollars show 'No Dollars...' instead of 'Zero Dollars...'
            wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? string.Format(" {0} and ", currencyName) : string.Format(" {0}s and ", currencyName));

            // If the array has more than one element then there is a fractional part otherwise there isn't
            // just add 'No Cents' to the end
            if (arrNumber.Length > 1)
            {
                // If the length of the fractional element is only 1, add a 0 so that the text returned isn't,
                // 'One', 'Two', etc but 'Ten', 'Twenty', etc.
                long fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));
                string strFarctionPart = NumberToText(fractionPart);

                wordNumber += (fractionPart == 0 ? "Zero" : strFarctionPart) + (fractionPart == 1 ? " Cent" : " Cents");
            }
            else wordNumber += "Zero Cents";

            return string.Format("{0} Only", wordNumber);
        }

#endif

        public static string NumberToText(long number)
        {
            StringBuilder wordNumber = new StringBuilder();

            string[] powers = new string[] { "Thousand ", "Million ", "Billion " };
            string[] tens = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            string[] ones = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                                   "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

            if (number == 0) { return "Zero"; }
            if (number < 0)
            {
                wordNumber.Append("Negative ");
                number = -number;
            }

            long[] groupedNumber = new long[] { 0, 0, 0, 0 };
            int groupIndex = 0;

            while (number > 0)
            {
                groupedNumber[groupIndex++] = number % 1000;
                number /= 1000;
            }

            for (int i = 3; i >= 0; i--)
            {
                long group = groupedNumber[i];

                if (group >= 100)
                {
                    wordNumber.Append(ones[group / 100 - 1] + " Hundred ");
                    group %= 100;

                    if (group == 0 && i > 0)
                        wordNumber.Append(powers[i - 1]);
                }

                if (group >= 20)
                {
                    if ((group % 10) != 0)
                        wordNumber.Append(tens[group / 10 - 2] + " " + ones[group % 10 - 1] + " ");
                    else
                        wordNumber.Append(tens[group / 10 - 2] + " ");
                }
                else if (group > 0)
                    wordNumber.Append(ones[group - 1] + " ");

                if (group != 0 && i > 0)
                    wordNumber.Append(powers[i - 1]);
            }

            return wordNumber.ToString().Trim();
        }

        /// <summary>
        /// Get the first day of the month for
        /// any full date submitted
        /// </summary>
        /// <param name="dtDate"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(DateTime dtDate)
        {
            // set return value to the first day of the month
            // for any date passed in to the method

            // create a datetime variable set to the passed in date
            DateTime dtFrom = dtDate;

            // remove all of the days in the month
            // except the first day and set the
            // variable to hold that date
            dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));

            // return the first day of the month
            return dtFrom;
        }

        /// <summary>
        /// Get the first day of the month for a
        /// month passed by it's integer value
        /// </summary>
        /// <param name="iMonth"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(int iMonth)
        {
            // set return value to the last day of the month
            // for any date passed in to the method

            // create a datetime variable set to the passed in date
            DateTime dtFrom = new DateTime(DateTime.Now.Year, iMonth, 1);

            // remove all of the days in the month
            // except the first day and set the
            // variable to hold that date
            dtFrom = dtFrom.AddDays(-(dtFrom.Day - 1));

            // return the first day of the month
            return dtFrom;
        }

        /// <summary>
        /// Get the last day of the month for any
        /// full date
        /// </summary>
        /// <param name="dtDate"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(DateTime dtDate)
        {
            // set return value to the last day of the month
            // for any date passed in to the method

            // create a datetime variable set to the passed in date
            DateTime dtTo = dtDate;

            // overshoot the date by a month
            dtTo = dtTo.AddMonths(1);

            // remove all of the days in the next month
            // to get bumped down to the last day of the
            // previous month
            dtTo = dtTo.AddDays(-(dtTo.Day));

            // return the last day of the month
            return dtTo;
        }

        /// <summary>
        /// Get the last day of a month expressed by it's
        /// integer value
        /// </summary>
        /// <param name="iMonth"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(int iMonth, int iYear, bool enforceMonthValueDate, DateTime? defaultDate = null, bool capDate = true)
        {
            if (enforceMonthValueDate)
            {
                // set return value to the last day of the month
                // for any date passed in to the method

                // create a datetime variable set to the passed in date
                DateTime dtTo = new DateTime(iYear, iMonth, 1);

                // overshoot the date by a month
                dtTo = dtTo.AddMonths(1);

                // remove all of the days in the next month
                // to get bumped down to the last day of the
                // previous month
                dtTo = dtTo.AddDays(-(dtTo.Day));

                // return the last day of the month
                if (capDate)
                    return (dtTo > DateTime.Now ? DateTime.Now : dtTo);
                return dtTo;
            }
            else return defaultDate ?? DateTime.Now;
        }

        /// <summary>
        /// Finds the last day of the year for the selected day's year.
        /// </summary>
        public static DateTime LastDayOfYear(DateTime d)
        {
            // 1
            // Get first of next year
            DateTime n = new DateTime(d.Year + 1, 1, 1);
            // 2
            // Subtract 1 from it
            return n.AddDays(-1);
        }

        public static int GetAge(DateTime birthDate)
        {
            int age = default(int);

            DateTime today = DateTime.Today;

            age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            return age;
        }

        public static int GetPeriod(DateTime date1, DateTime date2)
        {
            return ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month;
        }

        /// <summary>
        /// Returns the luhn digit for a given PAN
        /// </summary>
        /// <param name="pan">PAN missing the luhn check digit</param>
        /// <returns>Luhn check digit</returns>
        public static string GetLuhn(string pan)
        {
            int sum = 0;

            pan = string.Format("{0}", pan).Trim();

            bool alternate = true;
            for (int i = pan.Length - 1; i >= 0; i--)
            {
                int num = int.Parse(pan[i].ToString());

                if (alternate)
                {
                    num *= 2;
                    if (num > 9)
                        num = num - 9;
                }

                sum += num;
                alternate = !alternate;
            }

            int luhnDigit = 10 - (sum % 10);
            if (luhnDigit == 10)
                luhnDigit = 0;

            return luhnDigit.ToString();
        }

        /// <summary>
        /// Checks that the luhn check digit is valid
        /// </summary>
        /// <param name="pan">PAN to validate</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool IsValidPAN(String pan)
        {
            pan = string.Format("{0}", pan).Trim();
            string luhn = GetLuhn(pan.Substring(0, pan.Length - 1));
            return luhn == pan.Substring(pan.Length - 1);
        }

        /// <summary>
        /// PCI DSS PAN mask. For strings longer than 10 chars masks characters [6..Length-4] 
        /// by character 'x'; otherwise returns the pan parameter unchanged.
        /// </summary>
        /// <param name="pan">a PAN string</param>
        /// <returns>a masked PAN string</returns>
        public static string MaskPan(string pan, int frontLength = 4, int endLength = 6)
        {
            if (pan == null)
                return null;

            pan = string.Format("{0}", pan).Trim();
            int unmaskedLength = frontLength + endLength;

            var totalLength = pan.Length;

            if (totalLength <= unmaskedLength)
                return pan;

            return
                new StringBuilder(totalLength, totalLength)
                    .Append(pan.Substring(0, frontLength)) // front
                    .Append(new string('x', totalLength - unmaskedLength))  // mask
                    .Append(pan.Substring((totalLength - endLength), endLength)) // end
                    .ToString();
        }

        /// <summary>
        /// Return the previous or next business day of the date specified.
        /// </summary>
        /// <param name="today"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static DateTime GetBusinessDay(DateTime today, int addValue)
        {
            #region Sanity Checks
            if ((addValue != -1) && (addValue != 1))
                throw new ArgumentOutOfRangeException("addValue must be -1 or 1");
            #endregion

            if (addValue > 0)
                return NextBusinessDay(today);
            else
                return PreviousBusinessDay(today);
        }

        /// <summary>
        /// Return the previous or next business day of the date specified.
        /// </summary>
        /// <param name="today"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static DateTime GetBusinessDay(DateTime today, int addValue, ICollection<DateTime> holidays)
        {
            #region Sanity Checks
            if ((addValue != -1) && (addValue != 1))
                throw new ArgumentOutOfRangeException("addValue must be -1 or 1");
            #endregion

            if (addValue > 0)
                return NextBusinessDay(today, holidays);
            else
                return PreviousBusinessDay(today, holidays);
        }

        /// <summary>
        /// return the previous business date of the date specified.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime PreviousBusinessDay(DateTime today)
        {
            DateTime result;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = today.AddDays(-2);
                    break;

                case DayOfWeek.Monday:
                    result = today.AddDays(-3);
                    break;

                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                case DayOfWeek.Friday:
                    result = today.AddDays(-1);
                    break;

                case DayOfWeek.Saturday:
                    result = today.AddDays(-1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("DayOfWeek=" + today.DayOfWeek);
            }
            return ScreenHolidays(result, -1);
        }

        /// <summary>
        /// return the previous business date of the date specified.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime PreviousBusinessDay(DateTime today, ICollection<DateTime> holidays)
        {
            DateTime result;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = today.AddDays(-2);
                    break;

                case DayOfWeek.Monday:
                    result = today.AddDays(-3);
                    break;

                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                case DayOfWeek.Friday:
                    result = today.AddDays(-1);
                    break;

                case DayOfWeek.Saturday:
                    result = today.AddDays(-1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("DayOfWeek=" + today.DayOfWeek);
            }

            return ScreenHolidays(result, -1, holidays);
        }

        /// <summary>
        /// return the next business date of the date specified.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime NextBusinessDay(DateTime today)
        {
            DateTime result;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    result = today.AddDays(1);
                    break;

                case DayOfWeek.Friday:
                    result = today.AddDays(3);
                    break;

                case DayOfWeek.Saturday:
                    result = today.AddDays(2);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("DayOfWeek=" + today.DayOfWeek);
            }
            return ScreenHolidays(result, 1);
        }

        /// <summary>
        /// return the next business date of the date specified.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime NextBusinessDay(DateTime today, ICollection<DateTime> holidays)
        {
            DateTime result;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    result = today.AddDays(1);
                    break;

                case DayOfWeek.Friday:
                    result = today.AddDays(3);
                    break;

                case DayOfWeek.Saturday:
                    result = today.AddDays(2);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("DayOfWeek=" + today.DayOfWeek);
            }
            return ScreenHolidays(result, 1, holidays);
        }

        /// <summary>
        /// return the mm/dd string of the date specified.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string MonthDay(DateTime time)
        {
            return String.Format("{0:00}/{1:00}", time.Month, time.Day);
        }

        /// <summary>
        /// screen for holidays 
        /// (simple mode)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="addValue"></param>
        /// <returns></returns>
        public static DateTime ScreenHolidays(DateTime result, int addValue)
        {
            #region Sanity Checks
            if ((addValue != -1) && (addValue != 1))
                throw new ArgumentOutOfRangeException("addValue must be -1 or 1");
            #endregion

            // holidays on fixed date
            switch (MonthDay(result))
            {
                case "01/01":  // Happy New Year
                case "07/04":  // Independent Day
                case "12/25":  // Christmas
                    return GetBusinessDay(result, addValue);
                default:
                    return result;
            }
        }

        public static DateTime ScreenHolidays(DateTime result, int addValue, ICollection<DateTime> holidays)
        {
            #region Sanity Checks
            if ((addValue != -1) && (addValue != 1))
                throw new ArgumentOutOfRangeException("addValue must be -1 or 1");
            #endregion

            // holidays on fixed date
            var temp = from dt in holidays
                       select MonthDay(dt);

            if (temp.Contains(MonthDay(result)))
                return GetBusinessDay(result, addValue, holidays);
            else return result;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static DateTime AdjustTimeSpan(DateTime value)
        {
            if (value.TimeOfDay == TimeSpan.Zero)
            {
                if (value.Date == DateTime.Today)
                    return DateTime.Now;
                else return value.Add(new TimeSpan(23, 59, 59));
            }
            else return value;
        }
    }
}
