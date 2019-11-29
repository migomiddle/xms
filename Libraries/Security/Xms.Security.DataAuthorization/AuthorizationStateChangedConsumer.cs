using Xms.Authorization.Abstractions;
using Xms.Event.Abstractions;

namespace Xms.Security.DataAuthorization
{
    /// <summary>
    /// 权限资源状态更改事件处理
    /// </summary>
    public class AuthorizationStateChangedConsumer : IConsumer<AuthorizationStateChangedEvent>
    {
        private readonly IRoleObjectAccessService _roleObjectAccessService;

        public AuthorizationStateChangedConsumer(IRoleObjectAccessService roleObjectAccessService)
        {
            _roleObjectAccessService = roleObjectAccessService;
        }

        public void HandleEvent(AuthorizationStateChangedEvent eventMessage)
        {
            if (!eventMessage.State)
            {
                _roleObjectAccessService.DeleteByObjectId(eventMessage.ResourceName, eventMessage.ObjectId.ToArray());
            }
        }
    }
}