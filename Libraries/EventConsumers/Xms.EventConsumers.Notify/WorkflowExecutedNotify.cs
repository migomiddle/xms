using System.Collections.Generic;
using Xms.Context;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Flow.Abstractions;
using Xms.Flow.Core.Events;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Notify.Abstractions;
using Xms.Notify.Internal;
using Xms.Sdk.Client;

namespace Xms.EventConsumers.Notify
{
    /// <summary>
    /// 工作流执行后发送通知
    /// </summary>
    public class WorkflowExecutedNotify : IConsumer<WorkFlowExecutedEvent>
    {
        private readonly IDataFinder _dataFinder;
        private readonly IAppContext _appContext;
        private readonly ILocalizedTextProvider _loc;
        private readonly IEnumerable<INotify> _notifies;

        public WorkflowExecutedNotify(IAppContext appContext
            , IDataFinder dataFinder
            , IEnumerable<INotify> notifies)
        {
            _appContext = appContext;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _dataFinder = dataFinder;
            _notifies = notifies;
        }

        public void HandleEvent(WorkFlowExecutedEvent eventMessage)
        {
            Entity user;
            var msg = string.Empty;
            if (eventMessage.Result.NextHandlerId.NotEmpty())
            {
                foreach (var hid in eventMessage.Result.NextHandlerId)
                {
                    //通知下一个处理人
                    user = _dataFinder.RetrieveById("systemuser", hid, ignorePermissions: true);
                    //生成审批消息，文本或网页链接
                    msg = string.Format(_loc["workflow_newtasknotify"], eventMessage.Context.EntityMetaData.LocalizedName);
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
                            ToUserId = hid
                            ,
                            LinkTo = "/entity/create?entityid=" + eventMessage.Context.EntityMetaData.EntityId + "&recordid=" + eventMessage.Context.InstanceInfo.ObjectId
                        });
                    }
                }
            }
            else
            {
                //通知申请人
                user = _dataFinder.RetrieveById("systemuser", eventMessage.Context.InstanceInfo.ApplicantId, ignorePermissions: true);

                if (eventMessage.Context.ProcessState == WorkFlowProcessState.Passed)
                {
                    msg = string.Format(_loc["workflow_passednotify"], eventMessage.Context.EntityMetaData.LocalizedName);
                }
                else if (eventMessage.Context.ProcessState == WorkFlowProcessState.UnPassed)
                {
                    msg = string.Format(_loc["workflow_unpassednotify"], eventMessage.Context.EntityMetaData.LocalizedName);
                }
                //发送消息
                foreach (var notifier in _notifies)
                {
                    notifier.Send(new InternalNotifyBody()
                    {
                        TypeCode = 1
                        ,
                        Subject = string.Format("您提交的【{0}】审批{1}", eventMessage.Context.EntityMetaData.LocalizedName, (eventMessage.Context.ProcessState == WorkFlowProcessState.Passed ? "通过" : "不通过"))
                        ,
                        Content = msg
                        ,
                        ToUserId = user.GetIdValue()
                        ,
                        LinkTo = "/entity/create?entityid=" + eventMessage.Context.EntityMetaData.EntityId + "&recordid=" + eventMessage.Context.InstanceInfo.ObjectId
                    });
                }
            }
        }
    }
}