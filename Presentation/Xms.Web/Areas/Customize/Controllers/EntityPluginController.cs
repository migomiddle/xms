using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Form;
using Xms.Infrastructure.Utility;
using Xms.Plugin;
using Xms.Plugin.Abstractions;
using Xms.Plugin.Domain;
using Xms.QueryView;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 实体插件管理控制器
    /// </summary>
    public class EntityPluginController : CustomizeBaseController
    {
        private readonly IEntityPluginCreater _entityPluginCreater;
        private readonly IEntityPluginUpdater _entityPluginUpdater;
        private readonly IEntityPluginFinder _entityPluginFinder;
        private readonly IEntityPluginDeleter _entityPluginDeleter;
        private readonly IWebHelper _webHelper;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly IQueryViewFinder _queryViewFinder;

        public EntityPluginController(IWebAppContext appContext
            , IEntityPluginCreater entityPluginCreater
            , IEntityPluginUpdater entityPluginUpdater
            , IEntityPluginFinder entityPluginFinder
            , IEntityPluginDeleter entityPluginDeleter
            , ISolutionService solutionService
            , IWebHelper webHelper
            , ISystemFormFinder systemFormFinder
            , IQueryViewFinder queryViewFinder)
            : base(appContext, solutionService)
        {
            _entityPluginCreater = entityPluginCreater;
            _entityPluginUpdater = entityPluginUpdater;
            _entityPluginFinder = entityPluginFinder;
            _entityPluginDeleter = entityPluginDeleter;
            _webHelper = webHelper;
            _systemFormFinder = systemFormFinder;
            _queryViewFinder = queryViewFinder;
        }

        [Description("插件列表")]
        public IActionResult Index(EntityPluginModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<EntityPlugin> filter = FilterContainerBuilder.Build<EntityPlugin>();
            filter.And(n => n.IsVisibled == true);
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.EntityId == model.EntityId);
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

            var result = _entityPluginFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                , SolutionId.Value, true);

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [Description("新建插件")]
        public IActionResult CreateEntityPlugin()
        {
            EditEntityPluginModel model = new EditEntityPluginModel
            {
                SolutionId = SolutionId.Value
            };
            return View(model);
        }

        [Description("预加载DLL")]
        [HttpPost]
        public async Task<IActionResult> BeforehandLoadPlugin(BeforehandLoadPluginModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PluginFile != null && model.PluginFile.Length > 0)
                {
                    if (!model.PluginFile.FileName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return JError("上传的文件类型错误");
                    }
                }
                var pluginAnalyses = await _entityPluginCreater.BeforehandLoad(model.PluginFile).ConfigureAwait(false);
                return JOk(pluginAnalyses);
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("修改插件")]
        [HttpPost]
        public IActionResult EditPlugin([FromBody]EditEntityPluginListModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.EntityPlugins != null)
                {
                    model.EntityPlugins.ForEach(x =>
                    {
                        if (x.EntityPluginId.IsEmpty())
                        {
                            x.EntityPluginId = Guid.NewGuid();
                            x.CreatedBy = CurrentUser.SystemUserId;
                            x.CreatedOn = DateTime.Now;
                            x.SolutionId = SolutionId.Value;
                            x.ComponentState = 0;
                            x.StateCode = RecordState.Enabled;
                            x.IsVisibled = true;
                            _entityPluginCreater.Create(x, x.AssemblyName);
                        }
                    });
                }
                if (model.DeleteEntityPluginIds != null)
                {
                    _entityPluginDeleter.DeleteById(model.DeleteEntityPluginIds.ToArray());
                }
                return CreateSuccess();
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("新建插件")]
        [HttpPost]
        public async Task<IActionResult> CreateEntityPlugin(EditEntityPluginModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PluginFile != null && model.PluginFile.Length > 0)
                {
                    if (!model.PluginFile.FileName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return JError("上传的文件类型错误");
                    }
                }
                var entity = new EntityPlugin();
                model.CopyTo(entity);
                entity.EntityPluginId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.CreatedOn = DateTime.Now;
                entity.SolutionId = SolutionId.Value;
                entity.ComponentState = 0;
                entity.StateCode = RecordState.Enabled;
                entity.ProcessOrder = model.ProcessOrder;
                entity.TypeCode = model.TypeCode;
                await _entityPluginCreater.Create(entity, model.PluginFile).ConfigureAwait(false);

                return CreateSuccess(new { id = entity.EntityPluginId });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑插件")]
        public IActionResult EditEntityPlugin(Guid id)
        {
            var entity = _entityPluginFinder.FindById(id);
            FilterContainer<EntityPlugin> filter = FilterContainerBuilder.Build<EntityPlugin>();
            filter.And(n => n.IsVisibled == true);
            filter.And(n => n.AssemblyName == entity.AssemblyName);
            EditPluginListModel model = new EditPluginListModel();
            var entityPlugins = _entityPluginFinder.Query(x => x.Where(filter));
            Dictionary<Guid, List<QueryView.Domain.QueryView>> queryViews = new Dictionary<Guid, List<QueryView.Domain.QueryView>>();
            Dictionary<Guid, List<Form.Domain.SystemForm>> systemForms = new Dictionary<Guid, List<Form.Domain.SystemForm>>();
            if (entityPlugins.NotEmpty())
            {
                entityPlugins.Where(x => x.TypeCode != (int)PlugInType.Entity && x.TypeCode != (int)PlugInType.WorkFlow).ToList().ForEach(x =>
                 {
                     if (x.TypeCode == (int)PlugInType.List)
                     {
                         var queryView = _queryViewFinder.FindById(x.EntityId);
                         if (queryView != null && !queryViews.Keys.Contains(queryView.EntityId))
                         {
                             queryViews.Add(queryView.EntityId, _queryViewFinder.FindByEntityId(queryView.EntityId));
                         }
                     }
                     else if (x.TypeCode == (int)PlugInType.Form)
                     {
                         var systemForm = _systemFormFinder.FindById(x.EntityId);
                         if (systemForm != null && !systemForms.Keys.Contains(systemForm.EntityId))
                         {
                             systemForms.Add(systemForm.EntityId, _systemFormFinder.FindByEntityId(systemForm.EntityId));
                         }
                     }
                 });
            }
            model.EntityPlugins = _entityPluginFinder.Query(x => x.Where(filter));
            model.PluginAnalysis = _entityPluginCreater.BeforehandLoad(entity.AssemblyName);
            model.QueryViews = queryViews;
            model.SystemForms = systemForms;
            model.SolutionId = SolutionId.Value;
            return View(model);
        }

        [Description("编辑插件")]
        [HttpPost]
        public async Task<IActionResult> EditEntityPlugin(EditEntityPluginModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PluginFile != null && model.PluginFile.Length > 0)
                {
                    if (!model.PluginFile.FileName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return JError("上传的文件类型错误");
                    }
                }
                var entity = _entityPluginFinder.FindById(model.EntityPluginId.Value);
                model.CopyTo(entity);
                await _entityPluginUpdater.Update(entity, model.PluginFile).ConfigureAwait(false);

                return UpdateSuccess(new { id = entity.EntityPluginId });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("删除插件")]
        [HttpPost]
        public IActionResult DeleteEntityPlugin([FromBody]DeleteManyModel model)
        {
            return _entityPluginDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置插件可用状态")]
        [HttpPost]
        public IActionResult SetEntityPluginState([FromBody]SetRecordStateModel model)
        {
            return _entityPluginUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }
    }
}