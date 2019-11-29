using System.Collections.Generic;
using Xms.Context;
using Xms.Event.Abstractions;
using Xms.Flow.Core.Events;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Notify.Abstractions;
using Xms.Notify.Internal;

namespace Xms.EventConsumers.Notify
{
    /// <summary>
    /// 工作流启动审批后发送通知
    /// </summary>
    public class WorkflowStartedNotify : IConsumer<WorkFlowStartedEvent>
    {
        private readonly IAppContext _appContext;
        private readonly ILocalizedTextProvider _loc;
        private readonly IEnumerable<INotify> _notifies;

        public WorkflowStartedNotify(IAppContext appContext
            , IEnumerable<INotify> notifies)
        {
            _appContext = appContext;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _notifies = notifies;
        }

        public void HandleEvent(WorkFlowStartedEvent eventMessage)
        {
            //当前节点处理人
            foreach (var handlerId in eventMessage.Result.NextHandlerId)
            {
                //通知方式：微信、短信、邮件、系统消息
                var msg = _loc["workflow_newtasknotify"].FormatWith(eventMessage.Context.EntityMetaData.LocalizedName);
                //发送消息
                foreach (var notifier in _notifies)
                {
                    notifier.Send(new InternalNotifyBody()
                    {
                        TypeCode = 2
                        ,
                        Subject = msg
                        ,
                        Content = ""
                        ,
                        ToUserId = handlerId
                        ,
                        LinkTo = "/entity/create?entityid=" + eventMessage.Context.EntityMetaData.EntityId + "&recordid=" + eventMessage.Context.ObjectId
                    });
                }
            }
        }
    }
}