using System;

namespace Xms.Infrastructure.Utility
{
    public enum DateInterval
    {
        Year,
        Month,
        Weekday,
        Day,
        Hour,
        Minute,
        Second
    }

    public class DateTimeHelper
    {
        public static long DateDiff(DateInterval interval, DateTime date1, DateTime date2)
        {
            TimeSpan ts = date2 - date1;

            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Year - date1.Year;

                case DateInterval.Month:
                    return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));

                case DateInterval.Weekday:
                    return Fix(ts.TotalDays) / 7;

                case DateInterval.Day:
                    return Fix(ts.TotalDays);

                case DateInterval.Hour:
                    return Fix(ts.TotalHours);

                case DateInterval.Minute:
                    return Fix(ts.TotalMinutes);

                default:
                    return Fix(ts.TotalSeconds);
            }
        }

        private static long Fix(double Number)
        {
            if (Number >= 0)
            {
                return (long)Math.Floor(Number);
            }
            return (long)Math.Ceiling(Number);
        }
    }
}