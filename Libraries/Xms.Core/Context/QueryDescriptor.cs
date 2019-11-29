using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Core.Context
{
    /// <summary>
    /// 查询对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryDescriptor<T>
        where T : class
    {
        private readonly IExpressionParser _expressionParser;

        /// <summary>
        /// 查询前N条记录
        /// </summary>
        public int TopCount { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageDescriptor PagingDescriptor { get; set; }

        /// <summary>
        /// 排序信息
        /// </summary>
        public List<SortDescriptor<T>> SortingDescriptor { get; set; }

        public QueryDescriptor(IExpressionParser expressionParser)
        {
            _expressionParser = expressionParser;
        }

        /// <summary>
        /// 设置读取记录数
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Take(int take)
        {
            TopCount = take;
            return this;
        }

        /// <summary>
        /// 设置分页信息
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Page(int currentPage, int pageSize)
        {
            if (PagingDescriptor == null)
            {
                PagingDescriptor = new PageDescriptor(currentPage, pageSize);
            }
            else
            {
                PagingDescriptor.PageNumber = currentPage;
                PagingDescriptor.PageSize = pageSize;
            }
            return this;
        }

        /// <summary>
        /// 设置排序信息
        /// </summary>
        /// <param name="objectPath"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Sort(Expression<Func<T, object>> objectPath, SortDirection direction)
        {
            if (this.SortingDescriptor == null)
            {
                this.SortingDescriptor = new List<SortDescriptor<T>>();
            }

            var name = ExpressionHelper.GetPropertyName<T>(objectPath);
            var exists = this.SortingDescriptor.Find(n => n.Field.IsCaseInsensitiveEqual(name));
            if (exists != null)
            {
                this.SortingDescriptor.Remove(exists);
            }
            this.SortingDescriptor.Add(new SortDescriptor<T>(name, direction));

            return this;
        }

        /// <summary>
        /// 设置排序信息
        /// </summary>
        /// <param name="sorts"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Sort(params SortDescriptor<T>[] sorts)
        {
            if (this.SortingDescriptor == null)
            {
                this.SortingDescriptor = new List<SortDescriptor<T>>();
            }
            foreach (var item in sorts)
            {
                if (!this.SortingDescriptor.Contains(item))
                {
                    this.SortingDescriptor.Add(item);
                }
            }
            return this;
        }

        /// <summary>
        /// 设置排序信息
        /// </summary>
        /// <param name="sorts"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Sort(params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts)
        {
            if (this.SortingDescriptor == null)
            {
                this.SortingDescriptor = new List<SortDescriptor<T>>();
            }
            foreach (var item in sorts)
            {
                var sd = item(new SortDescriptor<T>());

                if (!this.SortingDescriptor.Contains(sd))
                {
                    this.SortingDescriptor.Add(sd);
                }
            }
            return this;
        }

        /// <summary>
        /// 设置过滤条件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Where(FilterContainer<T> filter)
        {
            this.Critiria = filter;
            this.QueryText = filter.QueryText;
            this.Parameters = filter.Parameters;
            return this;
        }

        /// <summary>
        /// 设置过滤条件
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Where(Expression<Func<T, bool>> predicate)
        {
            this.Critiria.And(predicate);
            //_expressionParser.ToSql(predicate);
            //QueryText = _expressionParser.QueryText;
            //Parameters = _expressionParser.Arguments;
            this.QueryText = Critiria.QueryText;
            this.Parameters = Critiria.Parameters;

            return this;
        }

        private FilterContainer<T> _critiria;

        public FilterContainer<T> Critiria
        {
            get
            {
                if (_critiria == null)
                {
                    _critiria = new FilterContainer<T>(_expressionParser);
                }
                return _critiria;
            }
            set
            {
                _critiria = value;
            }
        }

        /// <summary>
        /// 生成的sql语句
        /// </summary>
        public string QueryText { get; set; }

        private List<QueryParameter> _parameters;

        /// <summary>
        /// 过滤条件参数
        /// </summary>
        public List<QueryParameter> Parameters
        {
            get
            {
                if (_parameters == null) { _parameters = new List<QueryParameter>(); }
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        private List<string> _columns;

        /// <summary>
        /// 查询字段
        /// </summary>
        public List<string> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new List<string>();
                }
                return _columns;
            }
            private set { _columns = value; }
        }

        /// <summary>
        /// 设置查询字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Select(params Expression<Func<T, object>>[] fields)
        {
            Columns.AddRange(ExpressionHelper.GetPropertyNames<T>(fields));
            return this;
        }

        /// <summary>
        /// 设置查询字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public QueryDescriptor<T> Select(params string[] fields)
        {
            if (fields.NotEmpty())
            {
                Columns.AddRange(fields);
            }
            return this;
        }

        public IUserContext UserContext { get; set; }

        public QueryDescriptor<T> SetUserContext(IUserContext user)
        {
            this.UserContext = user;
            return this;
        }
    }
}