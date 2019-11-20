using System;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Extensions
{
    public static class ConditionExpressionExtensions
    {
        private static bool ProcessEqual(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.INT:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) == int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.DECIMAL:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (decimal.Parse(cnd.Values[0].ToString()) == decimal.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.FLOAT:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (float.Parse(cnd.Values[0].ToString()) == float.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.MONEY:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (float.Parse(cnd.Values[0].ToString()) == float.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.VARCHAR:
                        result = (cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.NVARCHAR:
                        result = (cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.TEXT:
                        result = (cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.PICKLIST:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) == int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.STATE:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) == int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.STATUS:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) == int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.BIT:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) == int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.LOOKUP:
                        result = (cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.OWNER:
                        result = (cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.PRIMARYKEY:
                        result = (cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.DATETIME:
                        result = ProcessDatetimeCondition(cnd, values);
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessNotEqual(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.INT:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) != int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.DECIMAL:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (decimal.Parse(cnd.Values[0].ToString()) != decimal.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.FLOAT:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (float.Parse(cnd.Values[0].ToString()) != float.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.MONEY:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (float.Parse(cnd.Values[0].ToString()) != float.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.NVARCHAR:
                        result = (!cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.VARCHAR:
                        result = (!cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.TEXT:
                        result = (!cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.PICKLIST:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) != int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.STATE:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) != int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.STATUS:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) != int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.BIT:
                        if (values[0].ToString().IsNotEmpty())
                        {
                            result = (int.Parse(cnd.Values[0].ToString()) != int.Parse(values[0].ToString()));
                        }
                        break;

                    case AttributeTypeIds.LOOKUP:
                        result = (!cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.OWNER:
                        result = (!cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.PRIMARYKEY:
                        result = (!cnd.Values[0].ToString().IsCaseInsensitiveEqual(values[0].ToString()));
                        break;

                    case AttributeTypeIds.DATETIME:
                        result = ProcessDatetimeCondition(cnd, values);
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessGreaterThan(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.IsEmpty() || values[0] == null || values[0].ToString().IsEmpty())
            {
                result = false;
            }
            else
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.INT:
                        result = (int.Parse(values[0].ToString()) > int.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.DECIMAL:
                        result = (decimal.Parse(values[0].ToString()) > decimal.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.FLOAT:
                        result = (float.Parse(values[0].ToString()) > float.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.MONEY:
                        result = (float.Parse(values[0].ToString()) > float.Parse(cnd.Values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessGreaterEqual(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.IsEmpty() || values[0] == null || values[0].ToString().IsEmpty())
            {
                result = false;
            }
            else
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.INT:
                        result = (int.Parse(values[0].ToString()) >= int.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.DECIMAL:
                        result = (decimal.Parse(values[0].ToString()) >= decimal.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.FLOAT:
                        result = (float.Parse(values[0].ToString()) >= float.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.MONEY:
                        result = (float.Parse(values[0].ToString()) >= float.Parse(cnd.Values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessLessThan(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null && values[0].ToString().IsNotEmpty())
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.INT:
                        result = (int.Parse(values[0].ToString()) < int.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.DECIMAL:
                        result = (decimal.Parse(values[0].ToString()) < decimal.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.FLOAT:
                        result = (float.Parse(values[0].ToString()) < float.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.MONEY:
                        result = (float.Parse(values[0].ToString()) < float.Parse(cnd.Values[0].ToString()));
                        break;
                }
            }
            return result;
        }

        private static bool ProcessLessEqual(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null && values[0].ToString().IsNotEmpty())
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.INT:
                        result = (int.Parse(values[0].ToString()) <= int.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.DECIMAL:
                        result = (decimal.Parse(values[0].ToString()) <= decimal.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.FLOAT:
                        result = (float.Parse(values[0].ToString()) <= float.Parse(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.MONEY:
                        result = (float.Parse(values[0].ToString()) <= float.Parse(cnd.Values[0].ToString()));
                        break;
                }
            }
            return result;
        }

        private static bool ProcessBeginsWith(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = (cnd.Values[0].ToString().StartsWith(values[0].ToString(), StringComparison.InvariantCultureIgnoreCase));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessNotBeginWith(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = !(cnd.Values[0].ToString().StartsWith(values[0].ToString(), StringComparison.InvariantCultureIgnoreCase));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessEndsWith(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = (cnd.Values[0].ToString().EndsWith(values[0].ToString(), StringComparison.InvariantCultureIgnoreCase));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessNotEndWith(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = !(cnd.Values[0].ToString().EndsWith(values[0].ToString(), StringComparison.InvariantCultureIgnoreCase));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessNotContain(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = !(cnd.Values[0].ToString().Contains(values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessContains(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = (cnd.Values[0].ToString().Contains(values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessIn(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = (cnd.Values[0].ToString().Contains(values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessNotIn(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = !(cnd.Values[0].ToString().Contains(values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessLike(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = (values[0].ToString().Contains(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.VARCHAR:
                        result = (values[0].ToString().Contains(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.TEXT:
                        result = (values[0].ToString().Contains(cnd.Values[0].ToString()));
                        break;

                    case AttributeTypeIds.NTEXT:
                        result = (values[0].ToString().Contains(cnd.Values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessNotLike(ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null)
            {
                switch (attr.AttributeTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        result = !(cnd.Values[0].ToString().Contains(values[0].ToString()));
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private static bool ProcessDatetimeCondition(ConditionExpression cnd, params object[] values)
        {
            bool result = false;
            if (values.NotEmpty() && values[0] != null && values[0].ToString().IsNotEmpty())
            {
                switch (cnd.Operator)
                {
                    case ConditionOperator.Today:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(cnd.Values[0].ToString()), DateTime.Now) == 0;
                        break;

                    case ConditionOperator.Tomorrow:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(cnd.Values[0].ToString()), DateTime.Now) == -1;
                        break;

                    case ConditionOperator.Between:

                        break;

                    case ConditionOperator.InFiscalPeriod:

                        break;

                    case ConditionOperator.InFiscalPeriodAndYear:

                        break;

                    case ConditionOperator.InFiscalYear:

                        break;

                    case ConditionOperator.InOrAfterFiscalPeriodAndYear:

                        break;

                    case ConditionOperator.InOrBeforeFiscalPeriodAndYear:

                        break;

                    case ConditionOperator.Last7Days:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) <= 7;
                        break;

                    case ConditionOperator.LastFiscalPeriod:

                        break;

                    case ConditionOperator.LastFiscalYear:

                        break;

                    case ConditionOperator.LastMonth:
                        result = DateTimeHelper.DateDiff(DateInterval.Month, DateTime.Parse(values[0].ToString()), DateTime.Now) == 1;
                        break;

                    case ConditionOperator.LastWeek:
                        result = DateTimeHelper.DateDiff(DateInterval.Weekday, DateTime.Parse(values[0].ToString()), DateTime.Now) == 1;
                        break;

                    case ConditionOperator.LastXDays:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.LastXFiscalPeriods:

                        break;

                    case ConditionOperator.LastXFiscalYears:

                        break;

                    case ConditionOperator.LastXHours:
                        result = DateTimeHelper.DateDiff(DateInterval.Hour, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.LastXMonths:
                        result = DateTimeHelper.DateDiff(DateInterval.Month, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.LastXWeeks:
                        result = DateTimeHelper.DateDiff(DateInterval.Weekday, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.LastXYears:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.LastYear:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) == 1;
                        break;

                    case ConditionOperator.Next7Days:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) >= -7;
                        break;

                    case ConditionOperator.NextFiscalPeriod:

                        break;

                    case ConditionOperator.NextFiscalYear:

                        break;

                    case ConditionOperator.NextMonth:
                        result = DateTimeHelper.DateDiff(DateInterval.Month, DateTime.Parse(values[0].ToString()), DateTime.Now) == -1;
                        break;

                    case ConditionOperator.NextWeek:
                        result = DateTimeHelper.DateDiff(DateInterval.Weekday, DateTime.Parse(values[0].ToString()), DateTime.Now) == -1;
                        break;

                    case ConditionOperator.NextXDays:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) >= -int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.NextXFiscalPeriods:

                        break;

                    case ConditionOperator.NextXFiscalYears:

                        break;

                    case ConditionOperator.NextXHours:
                        result = DateTimeHelper.DateDiff(DateInterval.Hour, DateTime.Parse(values[0].ToString()), DateTime.Now) >= -int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.NextXMonths:
                        result = DateTimeHelper.DateDiff(DateInterval.Month, DateTime.Parse(values[0].ToString()), DateTime.Now) < int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.NextXWeeks:
                        result = DateTimeHelper.DateDiff(DateInterval.Weekday, DateTime.Parse(values[0].ToString()), DateTime.Now) >= -int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.NextXYears:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) >= -int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.NextYear:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) == -1;
                        break;

                    case ConditionOperator.NotBetween:

                        break;

                    case ConditionOperator.OlderThanXYears:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.OlderThanXMonths:
                        result = DateTimeHelper.DateDiff(DateInterval.Month, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.OlderThanXDays:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) >= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.AfterXYears:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) <= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.AfterXMonths:
                        result = DateTimeHelper.DateDiff(DateInterval.Month, DateTime.Parse(values[0].ToString()), DateTime.Now) <= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.AfterXDays:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) <= int.Parse(cnd.Values[0].ToString());
                        break;

                    case ConditionOperator.On:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) == 0;
                        break;

                    case ConditionOperator.NotOn:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) != 0;
                        break;

                    case ConditionOperator.OnOrAfter:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) <= 0;
                        break;

                    case ConditionOperator.OnOrBefore:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) >= 0;
                        break;

                    case ConditionOperator.ThisFiscalPeriod:

                        break;

                    case ConditionOperator.ThisFiscalYear:

                        break;

                    case ConditionOperator.ThisMonth:
                        result = DateTimeHelper.DateDiff(DateInterval.Month, DateTime.Parse(values[0].ToString()), DateTime.Now) == 0;
                        break;

                    case ConditionOperator.ThisWeek:
                        result = DateTimeHelper.DateDiff(DateInterval.Weekday, DateTime.Parse(values[0].ToString()), DateTime.Now) == 0;
                        break;

                    case ConditionOperator.ThisYear:
                        result = DateTimeHelper.DateDiff(DateInterval.Year, DateTime.Parse(values[0].ToString()), DateTime.Now) == 0;
                        break;

                    case ConditionOperator.Yesterday:
                        result = DateTimeHelper.DateDiff(DateInterval.Day, DateTime.Parse(values[0].ToString()), DateTime.Now) == 1;
                        break;
                }
            }
            return result;
        }

        public static bool IsTrue(this ConditionExpression cnd, Schema.Domain.Attribute attr, params object[] values)
        {
            bool result = false;
            switch (cnd.Operator)
            {
                case ConditionOperator.Equal:
                    result = ProcessEqual(cnd, attr, values);
                    break;

                case ConditionOperator.EqualUserId:
                    result = ProcessEqual(cnd, attr, values);
                    break;

                case ConditionOperator.NotEqual:
                    result = ProcessNotEqual(cnd, attr, values);
                    break;

                case ConditionOperator.NotEqualUserId:
                    result = ProcessNotEqual(cnd, attr, values);
                    break;

                case ConditionOperator.NotEqualBusinessId:
                    result = ProcessNotEqual(cnd, attr, values);
                    break;

                case ConditionOperator.BeginsWith:
                    result = ProcessBeginsWith(cnd, attr, values);
                    break;

                case ConditionOperator.DoesNotBeginWith:
                    result = ProcessNotBeginWith(cnd, attr, values);
                    break;

                case ConditionOperator.DoesNotContain:
                    result = ProcessNotContain(cnd, attr, values);
                    break;

                case ConditionOperator.DoesNotEndWith:
                    result = ProcessNotEndWith(cnd, attr, values);
                    break;

                case ConditionOperator.EndsWith:
                    result = ProcessEndsWith(cnd, attr, values);
                    break;

                case ConditionOperator.GreaterEqual:
                    result = ProcessGreaterEqual(cnd, attr, values);
                    break;

                case ConditionOperator.GreaterThan:
                    result = ProcessGreaterThan(cnd, attr, values);
                    break;

                case ConditionOperator.LessEqual:
                    result = ProcessLessEqual(cnd, attr, values);
                    break;

                case ConditionOperator.LessThan:
                    result = ProcessLessThan(cnd, attr, values);
                    break;

                case ConditionOperator.Last7Days:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastMonth:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastWeek:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastXDays:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastXHours:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastXMonths:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastXWeeks:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastXYears:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.LastYear:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.Next7Days:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextMonth:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextWeek:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextXDays:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextXHours:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextXMonths:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextXWeeks:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextXYears:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NextYear:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.Today:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NotBetween:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.Between:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.OlderThanXYears:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.OlderThanXMonths:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.OlderThanXDays:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.AfterXYears:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.AfterXMonths:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.AfterXDays:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.On:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NotOn:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.OnOrAfter:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.OnOrBefore:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.ThisMonth:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.ThisWeek:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.ThisYear:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.Tomorrow:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.Yesterday:
                    result = ProcessDatetimeCondition(cnd, values);
                    break;

                case ConditionOperator.NotIn:
                    result = ProcessNotIn(cnd, attr, values);
                    break;

                case ConditionOperator.In:
                    result = ProcessIn(cnd, attr, values);
                    break;

                case ConditionOperator.Like:
                    result = ProcessLike(cnd, attr, values);
                    break;

                case ConditionOperator.Contains:
                    result = ProcessContains(cnd, attr, values);
                    break;

                case ConditionOperator.NotLike:
                    result = ProcessNotLike(cnd, attr, values);
                    break;

                case ConditionOperator.NotNull:
                    result = (values.NotEmpty() && values[0] != null);
                    break;

                case ConditionOperator.Null:
                    result = (values.IsEmpty() || values[0] == null);
                    break;

                case ConditionOperator.EqualBusinessId:
                    result = ProcessEqual(cnd, attr, values);
                    break;

                case ConditionOperator.EqualOrganizationId:
                    result = ProcessEqual(cnd, attr, values);
                    break;

                case ConditionOperator.NotEqualOrganizationId:
                    result = ProcessNotEqual(cnd, attr, values);
                    break;

                default:
                    break;
            }
            return result;
        }
    }
}