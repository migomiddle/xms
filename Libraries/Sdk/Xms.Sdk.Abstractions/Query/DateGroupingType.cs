namespace Xms.Sdk.Abstractions.Query
{
    public enum DateGroupingType
    {
        Year,
        Month,
        Day,
        Quarter,
        Week
    }

    public sealed class DateGroupingFormatting
    {
        public static string GetFormatting(DateGroupingType dgt)
        {
            var result = "yyyy-MM-dd";
            switch (dgt)
            {
                case DateGroupingType.Year:
                    result = "yyyy";
                    break;

                case DateGroupingType.Month:
                    result = "yyyy-MM";
                    break;

                case DateGroupingType.Day:
                    result = "yyyy-MM-dd";
                    break;

                case DateGroupingType.Week:
                    result = "dddd";
                    break;
            }
            return result;
        }
    }
}