using System.Collections.Generic;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Abstractions.Component;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;

namespace Xms.QueryView.Abstractions
{
    public static class RowCommandExtensions
    {
        public static bool IsTrue(this RowCommand rowCmd, List<KeyValuePair<string, object>> data, List<Schema.Domain.Attribute> attributes)
        {
            var rowFlag = false;
            var cndFlag = false;
            if (rowCmd.Conditions.NotEmpty())
            {
                foreach (var cmdCond in rowCmd.Conditions)
                {
                    if (data.Exists(n => n.Key.IsCaseInsensitiveEqual(cmdCond.AttributeName)))
                    {
                        var a = data.Find(n => n.Key.IsCaseInsensitiveEqual(cmdCond.AttributeName));
                        var attr = attributes.Find(n => n.Name.IsCaseInsensitiveEqual(cmdCond.AttributeName));

                        //符合条件时
                        if (cmdCond.IsTrue(attr, a.Value))
                        {
                            //或者关系
                            if (rowCmd.LogicalOperator == LogicalOperator.Or)
                            {
                                cndFlag = true;
                                rowFlag = true;
                                break;
                            }
                            else//并且
                            {
                                cndFlag = true;
                                if (rowCmd.Conditions.Count() == 1)
                                {
                                    rowFlag = true;
                                    break;
                                }
                            }
                        }
                        else//不符合条件时
                        {
                            //或者关系
                            if (rowCmd.LogicalOperator == LogicalOperator.Or)
                            {
                                cndFlag = false;
                            }
                            else//并且
                            {
                                cndFlag = false;
                                rowFlag = false;
                                break;
                            }
                        }
                    }
                    if (rowFlag)
                    {
                        break;
                    }
                    else
                    {
                        rowFlag = cndFlag;
                    }
                }
            }
            return rowFlag;
        }
    }
}