using Xms.Core.Data;
using Xms.Notify.Abstractions;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Client;

namespace Xms.Notify.Internal
{
    /// <summary>
    /// 系统消息处理
    /// </summary>
    public class InternalMessageHandler : INotify
    {
        private readonly IDataCreater _dataCreater;

        public InternalMessageHandler(IDataCreater dataCreater)
        {
            _dataCreater = dataCreater;
        }

        public object Send(NotifyBody body)
        {
            var ibody = body as InternalNotifyBody;
            Entity entity = new Entity("notice");
            entity.SetAttributeValue("name", ibody.Subject);
            entity.SetAttributeValue("content", ibody.Content);
            entity.SetAttributeValue("linkto", ibody.LinkTo);
            entity.SetAttributeValue("isread", false);
            entity.SetAttributeValue("typecode", new OptionSetValue(ibody.TypeCode));
            entity.SetAttributeValue("ownerid", new OwnerObject(OwnerTypes.SystemUser, ibody.ToUserId));
            _dataCreater.Create(entity, true);
            return true;
        }
    }
}