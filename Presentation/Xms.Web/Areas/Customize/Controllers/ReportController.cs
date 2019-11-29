using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Xms.Business.DataAnalyse.Domain;
using Xms.Business.DataAnalyse.Report;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 报表管理控制器
    /// </summary>
    public class ReportController : CustomizeBaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IWebAppContext appContext
            , IReportService reportService
            , ISolutionService solutionService)
            : base(appContext, solutionService)
        {
            _reportService = reportService;
        }

        [Description("报表列表")]
        public IActionResult Index(ReportModel model)
        {
            return ToBePerfected();
            FilterContainer<Report> filter = FilterContainerBuilder.Build<Report>();
            filter.And(n => n.SolutionId == SolutionId.Value);
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }

            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<Report> result = _reportService.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                , SolutionId.Value, true);
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;

            return DynamicResult(model);
        }

        [Description("新建报表")]
        public IActionResult CreateReport()
        {
            EditReportModel model = new EditReportModel
            {
                SolutionId = SolutionId.Value
            };
            return View(model);
        }

        [Description("新建报表")]
        [HttpPost]
        public async Task<IActionResult> CreateReport(EditReportModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Report();
                model.CopyTo(entity);
                entity.ReportId = Guid.NewGuid();
                entity.SolutionId = SolutionId.Value;
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.EntityId = model.EntityId;
                entity.RelatedEntityId = model.RelatedEntityId;
                if (model.ReportFile != null && model.ReportFile.Length > 0)
                {
                    entity.FileName = model.ReportFile.FileName;
                    int fsLen = (int)model.ReportFile.Length;
                    byte[] heByte = new byte[fsLen];
                    using (Stream s = new MemoryStream())
                    {
                        await model.ReportFile.CopyToAsync(s).ConfigureAwait(false);
                        await s.ReadAsync(heByte, 0, heByte.Length).ConfigureAwait(false);
                        string bodytext = System.Text.Encoding.UTF8.GetString(heByte);
                        entity.BodyText = bodytext;
                    }
                }
                _reportService.Create(entity);
                return CreateSuccess(new { id = entity.ReportId });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑报表")]
        public IActionResult EditReport(Guid id)
        {
            EditReportModel model = new EditReportModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _reportService.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.ReportId = id;

                    return View(model);
                }
            }
            return NotFound();
        }

        [Description("编辑报表")]
        [HttpPost]
        public async Task<IActionResult> EditReport(EditReportModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _reportService.FindById(model.ReportId.Value);
                model.SolutionId = entity.SolutionId;
                model.CopyTo(entity);
                entity.ModifiedBy = CurrentUser.SystemUserId;
                entity.ModifiedOn = DateTime.Now;
                if (model.ReportFile != null && model.ReportFile.Length > 0)
                {
                    entity.FileName = model.ReportFile.FileName;
                    int fsLen = (int)model.ReportFile.Length;
                    byte[] heByte = new byte[fsLen];
                    using (Stream s = new MemoryStream())
                    {
                        await model.ReportFile.CopyToAsync(s).ConfigureAwait(false);
                        await s.ReadAsync(heByte, 0, heByte.Length).ConfigureAwait(false);
                        string bodytext = System.Text.Encoding.UTF8.GetString(heByte);
                        entity.BodyText = bodytext;
                    }
                }
                _reportService.Update(entity);
                return UpdateSuccess();
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("报表向导")]
        public IActionResult ReportPilot()
        {
            return View();
        }

        [Description("删除报表")]
        [HttpPost]
        public IActionResult DeleteReport([FromBody]DeleteManyModel model)
        {
            return _reportService.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置报表权限启用状态")]
        [HttpPost]
        public IActionResult SetReportAuthorizationState(SetReportAuthorizationStateModel model)
        {
            return _reportService.UpdateAuthorization(model.RecordId, model.IsAuthorization).UpdateResult(T);
        }
    }
}