using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 审批流附件控制器
    /// </summary>
    [Route("{org}/flow/[action]")]
    public class WorkflowAttachmentController : AuthenticatedControllerBase
    {
        private readonly IDataFinder _dataFinder;

        public WorkflowAttachmentController(IWebAppContext appContext
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _dataFinder = dataFinder;
        }

        [Description("下载流程附件")]
        public IActionResult WorkFlowAttachments(Guid processId, bool preview = false)
        {
            QueryExpression query = new QueryExpression("attachment", CurrentUser.UserSettings.LanguageId);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, processId);
            var data = _dataFinder.RetrieveAll(query);
            if (data.IsEmpty())
            {
                return NotFound();
            }
            return Redirect("/" + WebContext.OrganizationUniqueName + "/file/download?id=" + data.First().GetIdValue() + "&sid=" + CurrentUser.SessionId + "&preview=" + preview);
        }
    }
}