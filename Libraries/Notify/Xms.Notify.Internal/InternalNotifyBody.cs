using System;
using Xms.Notify.Abstractions;

namespace Xms.Notify.Internal
{
    /// <summary>
    /// 内部信息提示体
    /// </summary>
    public class InternalNotifyBody : NotifyBody
    {
        public Guid ToUserId { get; set; }
        public string LinkTo { get; set; }
        public int TypeCode { get; set; }
    }
}