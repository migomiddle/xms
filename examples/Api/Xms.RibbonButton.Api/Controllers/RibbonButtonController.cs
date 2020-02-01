using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xms.Api.Core.Controller;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.RibbonButton.Api.Models;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.WebResource;
using Xms.Infrastructure.Utility;
using Xms.Core.Data;
using System.ComponentModel;

namespace Xms.RibbonButton.Api.Controllers
{
    /// <summary>
    /// 按钮查询接口
    /// </summary>
    [Route("{org}/api/ribbonbutton")]
    public class RibbonButtonController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IRibbonButtonCreater _ribbonButtonCreater;
        private readonly IRibbonButtonUpdater _ribbonButtonUpdater;
        private readonly IRibbonButtonFinder _ribbonButtonFinder;
        private readonly IRibbonButtonDeleter _ribbonButtonDeleter;
        private readonly IWebResourceFinder _webResourceFinder;

        public RibbonButtonController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IRibbonButtonCreater ribbonButtonCreater
            , IRibbonButtonFinder ribbonButtonFinder
            , IRibbonButtonUpdater ribbonButtonUpdater
            , IRibbonButtonDeleter ribbonButtonDeleter
            , IWebResourceFinder webResourceFinder)
          : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _ribbonButtonCreater = ribbonButtonCreater;
            _ribbonButtonFinder = ribbonButtonFinder;
            _ribbonButtonUpdater = ribbonButtonUpdater;
            _ribbonButtonDeleter = ribbonButtonDeleter;
            _webResourceFinder = webResourceFinder;
        }

        [HttpGet]
        [Description("按钮列表")]
        public IActionResult Get([FromQuery]RibbonButtonModel model)
        {
            if (model.EntityId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = entity;
            FilterContainer<RibbonButton.Domain.RibbonButton> filter = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>();
            filter.And(n => n.EntityId == model.EntityId);
            if (model.ShowArea.HasValue)
            {
                filter.And(n => n.ShowArea == model.ShowArea.Value);
            }
            if (model.Label.IsNotEmpty())
            {
                filter.And(n => n.Label.Like(model.Label));
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = 25000;
            }
            else if (CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            PagedList<RibbonButton.Domain.RibbonButton> result = _ribbonButtonFinder.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;            
            model.SolutionId = SolutionId.Value;
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("单个按钮")]
        public IActionResult Get(Guid id)
        {
            EditRibbonButtonModel model = new EditRibbonButtonModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _ribbonButtonFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.WebResourceName = entity.JsLibrary;
                    if (entity.JsLibrary.IsNotEmpty() && entity.JsLibrary.StartsWith("$webresource:"))
                    {
                        var wr = _webResourceFinder.FindById(Guid.Parse(entity.JsLibrary.Replace("$webresource:", "")));
                        if (wr != null)
                        {
                            model.WebResourceName = wr.Name;
                        }
                        else
                        {
                            model.JsLibrary = string.Empty;
                            model.JsAction = string.Empty;
                        }
                    }
                    return JOk(model);
                }
            }
            return NotFound();
        }


    }
}