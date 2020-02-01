using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.EntityData.Api.Models;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;

namespace Xms.EntityData.Api
{
    /// <summary>
    /// 实体数据分派接口
    /// </summary>
    [Route("{org}/api/data/assign")]
    public class DataAssignController : ApiControllerBase
    {
        private readonly IDataAssigner _dataAssigner;
        public DataAssignController(IWebAppContext appContext
            , IDataAssigner dataAssigner)
            : base(appContext)
        {
            _dataAssigner = dataAssigner;
        }

        [Description("分派记录")]
        [HttpPost]
        public IActionResult Post(AssignedModel model)
        {
            if (model == null || !Arguments.HasValue(model.ObjectId))
            {
                return NotSpecifiedRecord();
            }
            foreach (var item in model.ObjectId)
            {
                OwnerObject owner = null;
                if (model.OwnerIdType == 1) //assign to me
                {
                    owner = new OwnerObject(OwnerTypes.SystemUser, CurrentUser.SystemUserId);
                }
                else if (model.OwnerIdType == 2)
                {
                    owner = new OwnerObject(OwnerTypes.SystemUser, model.OwnerId);
                }
                else if (model.OwnerIdType == 3)
                {
                    owner = new OwnerObject(OwnerTypes.Team, model.OwnerId);
                }
                _dataAssigner.Assign(model.EntityId, item, owner);
            }
            return JOk(T["assign_success"]);
        }
    }
}