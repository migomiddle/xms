using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 消息接口
    /// </summary>
    [Route("{org}/api/notice")]
    public class NoticeController : ApiControllerBase
    {
        private readonly IDataFinder _dataFinder;
        private readonly IDataUpdater _dataUpdater;

        public NoticeController(IWebAppContext appContext
            , IDataFinder dataFinder
            , IDataUpdater dataUpdater)
            : base(appContext)
        {
            _dataFinder = dataFinder;
            _dataUpdater = dataUpdater;
        }

        [Description("消息通知")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var entity = _dataFinder.RetrieveById("notice", id);
            if (entity.NotEmpty())
            {
                //如果所有者等于当前用户，则设为已读
                if (entity.GetGuidValue("ownerid").Equals(CurrentUser.SystemUserId))
                {
                    var updEntity = new Entity("notice");
                    updEntity.SetIdValue(id);
                    updEntity.SetAttributeValue("isread", true);
                    _dataUpdater.Update(updEntity);
                }

                //if (entity.GetStringValue("linkto").IsNotEmpty())
                //{
                //    return Redirect("/" + XmsWebContext.OrganizationUniqueName + entity.GetStringValue("linkto"));
                //}
                //else
                //{
                //    return Redirect("/" + XmsWebContext.OrganizationUniqueName + "/entity/create?entityname=notice&recordid=" + id);
                //}
                return JOk(entity);
            }
            return NotFound();
        }

        [Description("更新消息通知为已读")]
        [HttpPost("allread")]
        public IActionResult AllRead()
        {
            var entity = new Entity("notice");
            entity.SetAttributeValue("isread", true);
            entity.SetAttributeValue("readtime", DateTime.Now);
            entity.SetAttributeValue("organizationid", CurrentUser.OrganizationId);
            entity.SetAttributeValue("ownerid", CurrentUser.SystemUserId);
            entity.SetAttributeValue("owningbusinessunit", CurrentUser.BusinessUnitId);
            var query = new QueryExpression("notice", CurrentUser.UserSettings.LanguageId);
            query.Criteria.AddCondition("ownerid", ConditionOperator.EqualUserId);
            query.Criteria.AddCondition("isread", ConditionOperator.Equal, false);
            _dataUpdater.Update(entity, query);
            return JOk(T["updated_success"]);
        }
    }
}