using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;
using Xms.WebResource;
using Xms.WebResource.Abstractions;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// web资源管理控制器
    /// </summary>
    public class WebResourceController : CustomizeBaseController
    {
        private readonly IWebResourceCreater _webResourceCreater;
        private readonly IWebResourceUpdater _webResourceUpdater;
        private readonly IWebResourceFinder _webResourceFinder;
        private readonly IWebResourceDeleter _webResourceDeleter;
        private readonly IWebResourceContentCoder _webResourceContentCoder;

        public WebResourceController(IWebAppContext appContext
            , ISolutionService solutionService
            , IWebResourceCreater webResourceCreater
            , IWebResourceUpdater webResourceUpdater
            , IWebResourceFinder webResourceFinder
            , IWebResourceDeleter webResourceDeleter
            , IWebResourceContentCoder webResourceContentCoder)
            : base(appContext, solutionService)
        {
            _webResourceCreater = webResourceCreater;
            _webResourceUpdater = webResourceUpdater;
            _webResourceFinder = webResourceFinder;
            _webResourceDeleter = webResourceDeleter;
            _webResourceContentCoder = webResourceContentCoder;
        }

        [Description("Web资源列表")]
        public IActionResult Index(WebResourceModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<WebResource.Domain.WebResource> filter = FilterContainerBuilder.Build<WebResource.Domain.WebResource>();
            filter.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }
            if (model.WebResourceType.HasValue)
            {
                filter.And(n => n.WebResourceType == model.WebResourceType.Value);
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
            PagedList<WebResource.Domain.WebResource> result = _webResourceFinder.QueryPaged(x => x
                .Select(s => new { s.WebResourceId, s.Name, s.WebResourceType, s.CreatedOn, s.Description })
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                , SolutionId.Value, true);

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建Web资源")]
        public IActionResult CreateWebResource()
        {
            EditWebResourceModel model = new EditWebResourceModel
            {
                SolutionId = SolutionId.Value,
                WebResourceType = WebResourceType.Script
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建Web资源-保存")]
        public async Task<IActionResult> CreateWebResource(EditWebResourceModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new WebResource.Domain.WebResource();
                model.CopyTo(entity);
                entity.WebResourceId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.CreatedOn = DateTime.Now;
                entity.OrganizationId = CurrentUser.OrganizationId;
                if (model.WebResourceType == WebResourceType.Picture)
                {
                    if (model.ResourceFile != null && model.ResourceFile.Length > 0)
                    {
                        byte[] bytes = new byte[model.ResourceFile.Length];
                        using (Stream s = new MemoryStream())
                        {
                            await model.ResourceFile.CopyToAsync(s).ConfigureAwait(false);
                            await s.ReadAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                            entity.Content = _webResourceContentCoder.CodeEncode(bytes);
                        }
                    }
                    else
                    {
                        return CreateFailure(T["webresource_content_empty"]);
                    }
                }
                else
                {
                    if (model.Type == 0)
                    {
                        if (model.ResourceFile != null && model.ResourceFile.Length > 0)
                        {
                            byte[] bytes = new byte[model.ResourceFile.Length];
                            using (Stream s = new MemoryStream())
                            {
                                await model.ResourceFile.CopyToAsync(s).ConfigureAwait(false);
                                await s.ReadAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                                entity.Content = _webResourceContentCoder.CodeEncode(bytes);
                            }
                        }
                    }
                    else if (model.Type == 1)
                    {
                        if (model.Content.IsNotEmpty())
                        {
                            entity.Content = _webResourceContentCoder.CodeEncode(model.Content);
                        }
                    }
                    if (entity.Content.IsEmpty())
                    {
                        return CreateFailure(T["webresource_content_empty"]);
                    }
                }

                _webResourceCreater.Create(entity);
                return CreateSuccess(new { id = entity.WebResourceId });
            }
            return CreateFailure(GetModelErrors());
        }

        [HttpGet]
        [Description("Web资源编辑")]
        public IActionResult EditWebResource(Guid id)
        {
            EditWebResourceModel model = new EditWebResourceModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _webResourceFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.Content = _webResourceContentCoder.CodeDecode(model.Content);
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("Web资源保存")]
        public async Task<IActionResult> EditWebResource(EditWebResourceModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _webResourceFinder.FindById(model.WebResourceId.Value);
                model.CopyTo(entity);
                if (model.WebResourceType == WebResourceType.Picture)
                {
                    //重新上传文件时更新内容，否则保留原有内容
                    if (model.ResourceFile != null && model.ResourceFile.Length > 0)
                    {
                        byte[] bytes = new byte[model.ResourceFile.Length];
                        using (Stream s = new MemoryStream())
                        {
                            await model.ResourceFile.CopyToAsync(s).ConfigureAwait(false);
                            await s.ReadAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                            entity.Content = _webResourceContentCoder.CodeEncode(bytes);
                        }
                    }
                }
                else
                {
                    if (model.Content.IsNotEmpty())
                    {
                        entity.Content = _webResourceContentCoder.CodeEncode(model.Content);
                    }
                    else
                    {
                        return CreateFailure(T["webresource_content_empty"]);
                    }
                }
                _webResourceUpdater.Update(entity);

                return UpdateSuccess(new { id = entity.WebResourceId });
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("删除Web资源")]
        [HttpPost]
        public IActionResult DeleteWebResource([FromBody]DeleteManyModel model)
        {
            return _webResourceDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        /// <summary>
        /// Web资源对话框
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dm"></param>
        /// <returns></returns>
        [Description("Web资源对话框")]
        //[HttpPost]
        public IActionResult Dialog(WebResourceModel model, DialogModel dm)
        {
            FilterContainer<WebResource.Domain.WebResource> container = FilterContainerBuilder.Build<WebResource.Domain.WebResource>();
            container.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            if (model.SolutionId.HasValue && !model.SolutionId.Value.Equals(Guid.Empty))
            {
                container.And(n => n.SolutionId == model.SolutionId.Value);
            }
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name.Like(model.Name));
            }
            if (model.WebResourceType.HasValue)
            {
                container.And(n => n.WebResourceType == model.WebResourceType.Value);
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
            var result = _webResourceFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Select(s => new { s.WebResourceId, s.WebResourceType, s.Name, s.Description, s.CreatedOn })
                .Where(container)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
            );
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            ViewData["DialogModel"] = dm;

            return View(model);
        }
    }
}