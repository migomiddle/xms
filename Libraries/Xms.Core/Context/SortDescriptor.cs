using System;
using System.Linq.Expressions;
using Xms.Infrastructure.Utility;

namespace Xms.Core.Context
{
    public class SortDescriptor<T> where T : class
    {
        public SortDescriptor()
        {
        }

        public SortDescriptor(string field, SortDirection direction)
        {
            this.Field = field;
            this.Direction = direction;
        }

        public SortDescriptor(string field, int direction)
        {
            this.Field = field;
            this.Direction = direction == 1 ? SortDirection.Desc : SortDirection.Asc;
        }

        #region 对外方法

        public SortDescriptor<T> OnFile(string field)
        {
            if (field.IsNotEmpty())
            {
                this.Field = field;
            }
            return this;
        }

        public SortDescriptor<T> OnFile(Expression<Func<T, object>> objectPath)
        {
            var name = ExpressionHelper.GetPropertyName<T>(objectPath);
            this.Field = name;
            return this;
        }

        public SortDescriptor<T> SortAscending(Expression<Func<T, object>> objectPath)
        {
            var name = ExpressionHelper.GetPropertyName<T>(objectPath);
            this.Field = name;
            this.Direction = SortDirection.Asc;
            return this;
        }

        public SortDescriptor<T> SortDescending(Expression<Func<T, object>> objectPath)
        {
            var name = ExpressionHelper.GetPropertyName<T>(objectPath);
            this.Field = name;
            this.Direction = SortDirection.Desc;
            return this;
        }

        public SortDescriptor<T> Asc()
        {
            this.Direction = SortDirection.Asc;
            return this;
        }

        public SortDescriptor<T> Desc()
        {
            this.Direction = SortDirection.Desc;
            return this;
        }

        public SortDescriptor<T> ByDirection(SortDirection direction)
        {
            this.Direction = direction;
            return this;
        }

        public SortDescriptor<T> ByDirection(int direction)
        {
            this.Direction = direction == (int)SortDirection.Desc ? SortDirection.Desc : SortDirection.Asc;
            return this;
        }

        #endregion 对外方法

        public string Field { get; set; }

        public SortDirection Direction { get; set; }

        /// <summary>
        /// 获取排序类型名称
        /// </summary>
        /// <param name="sd"></param>
        /// <returns></returns>
        public string GetDbDirectionName()
        {
            if (this.Direction == SortDirection.Desc)
            {
                return "DESC";
            }
            return string.Empty;
        }
    }

    public enum SortDirection
    {
        Desc = 1
        , Asc = 0
    }
}