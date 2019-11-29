using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;

namespace Xms.File
{
    /// <summary>
    /// 附件服务
    /// </summary>
    public class AttachmentService
    {
        private readonly IAppContext _appContext;

        public AttachmentService(IAppContext appContext)
        {
            _appContext = appContext;
        }

        /// <summary>
        /// 附件实体名称
        /// </summary>
        public virtual string EntityName { get; set; } = "attachment";

        private string _path;

        /// <summary>
        /// 附件保存路径
        /// </summary>
        public virtual string Path
        {
            get
            {
                if (_path.IsEmpty())
                {
                    _path = "/upload/attachment/" + _appContext.GetFeature<ICurrentUser>().OrganizationId + "/";
                }
                return _path;
            }
            set
            {
                _path = value;
            }
        }
    }
}