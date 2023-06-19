using System;
using System.Globalization;

namespace CCL
{
    /// <summary>
    /// Useful functions that has to do with Date and Time stuff.
    /// </summary>
    public static class DateAndTime
    {

        public static string GetDateAndTimeStringForFileName()
        {
            DateTime now = DateTime.Now;
            return string.Format("{0}.{1}.{2}@{3}_{4}_{5}-{6}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
        }

        public static string GetMonthNameByNumber(int month)
        {
            switch (month) {
                case 1:     return "January";
                case 2:     return "February";
                case 3:     return "March";
                case 4:     return "April";
                case 5:     return "May";
                case 6:     return "June";
                case 7:     return "July";
                case 8:     return "August";
                case 9:     return "September";
                case 10:    return "October";
                case 11:    return "November";
                case 12:    return "December";
                default:    return "Unknown";
            }
        }

        public static int GetWeekNumberOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

    }
}
