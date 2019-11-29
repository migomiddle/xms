using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Logging.AppLog;
using Xms.Logging.AppLog.Domain;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 平台管理控制器
    /// </summary>
    public class PlatformController : AuthorizedControllerBase
    {
        private readonly ILogService _logService;

        public PlatformController(IWebAppContext appContext
            , ILogService logService)
            : base(appContext)
        {
            _logService = logService;
        }

        [Description("系统日志列表")]
        public IActionResult Log(LogModel m)
        {
            int page = m.Page, pageSize = m.PageSize;
            if (m.SortBy.IsEmpty())
            {
                m.SortBy = ExpressionHelper.GetPropertyName<VisitedLog>(n => n.CreatedOn);
                m.SortDirection = (int)SortDirection.Desc;
            }
            FilterContainer<VisitedLog> container = FilterContainerBuilder.Build<VisitedLog>();
            container.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            if (m.ClientIp.IsNotEmpty())
            {
                container.And(n => n.ClientIP == m.ClientIp);
            }
            if (m.Url.IsNotEmpty())
            {
                container.And(n => n.Url.Like(m.Url));
            }
            if (m.BeginTime.HasValue)
            {
                container.And(n => n.CreatedOn >= m.BeginTime);
            }
            if (m.EndTime.HasValue)
            {
                container.And(n => n.CreatedOn <= m.EndTime);
            }
            if (m.StatusCode > 0)
            {
                container.And(n => n.StatusCode == m.StatusCode);
            }

            PagedList<VisitedLog> result = _logService.Query(x => x
                .Page(page, pageSize)
                .Select(c => c.Title, c => c.CreatedOn, c => c.ClientIP, c => c.UserName, c => c.Url, c => c.StatusCode)
                .Where(container)
                .Sort(n => n.OnFile(m.SortBy).ByDirection(m.SortDirection))
                );

            m.Items = result.Items;
            m.TotalItems = result.TotalItems;

            return DynamicResult(m);
        }

        [Description("日志详情")]
        public IActionResult LogDetail(Guid logid)
        {
            LogDetailModel model = new LogDetailModel();
            model.LogDetail = _logService.FindById(logid);

            return DynamicResult(model);
        }
    }
}