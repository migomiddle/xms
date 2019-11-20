using System;
using System.Collections.Generic;

namespace Xms.Core.Context
{
    [Serializable]
    /// <summary>
    /// 查询描述
    /// </summary>
    public class ExecuteContext<T> : IExecuteContext<T> where T : class
    {
        /// <summary>
        /// 创建一个实例
        /// </summary>
        public ExecuteContext()
        {
        }

        public ExecuteContext(object executeContainer)
        {
            this.ExecuteContainer = executeContainer;
        }

        /// <summary>
        /// 查询前N条记录
        /// </summary>
        public int TopCount { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageDescriptor PagingInfo { get; set; }

        /// <summary>
        /// 排序信息
        /// </summary>
        public List<SortDescriptor<T>> SortingInfo { get; set; }

        /// <summary>
        /// 其它数据
        /// </summary>
        public List<Dictionary<string, object>> Extras { get; set; }

        //public override string ToString()
        //{
        //    StringBuilder s = new StringBuilder();
        //    s.AppendFormat("[Paging={0}]", PagingInfo.Output());
        //    s.AppendFormat("[Sorting={0}]", SortingInfo.Output());
        //    return s.ToString();
        //}

        public object ExecuteContainer { get; set; }
    }
}