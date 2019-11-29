using Xms.Schema.Abstractions;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Client;
using Xms.Sdk.Event;
using Xms.Sdk.Extensions;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 记录所有者改变后事件处理
    /// </summary>
    public class AssignOnEntityUpdated //: IConsumer<EntityUpdatedEvent>
    {
        private readonly IDataAssigner _dataAssigner;

        public AssignOnEntityUpdated(IDataAssigner dataAssigner)
        {
            _dataAssigner = dataAssigner;
        }

        public void HandleEvent(EntityUpdatedEvent eventMessage)
        {
            var ownerObj = eventMessage.EntityMetadata.EntityMask == EntityMaskEnum.User && eventMessage.Updated.ContainsKey("ownerid") ? (OwnerObject)eventMessage.Updated["ownerid"] : null;
            bool ownerChanged = ownerObj != null && !ownerObj.OwnerId.Equals(eventMessage.Origin.GetGuidValue("ownerid"));//是否改变了所有者
            if (ownerChanged)//改变了所有者
            {
                _dataAssigner.Assign(eventMessage.EntityMetadata, eventMessage.Updated, ownerObj);
            }
        }
    }
}