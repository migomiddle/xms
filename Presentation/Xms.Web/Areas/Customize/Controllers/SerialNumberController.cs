using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Business.SerialNumber;
using Xms.Business.SerialNumber.Domain;
using Xms.Core.Context;
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
    /// 自动编号规则管理控制器
    /// </summary>
    public class SerialNumberController : CustomizeBaseController
    {
        private readonly ISerialNumberRuleCreater _serialNumberRuleCreater;
        private readonly ISerialNumberRuleUpdater _serialNumberRuleUpdater;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly ISerialNumberRuleDeleter _serialNumberRuleDeleter;

        public SerialNumberController(IWebAppContext appContext
            , ISerialNumberRuleCreater serialNumberRuleCreater
            , ISerialNumberRuleUpdater serialNumberRuleUpdater
            , ISerialNumberRuleFinder serialNumberRuleFinder
            , ISerialNumberRuleDeleter serialNumberRuleDeleter
            , ISolutionService solutionService) : base(appContext, solutionService)
        {
            _serialNumberRuleCreater = serialNumberRuleCreater;
            _serialNumberRuleUpdater = serialNumberRuleUpdater;
            _serialNumberRuleFinder = serialNumberRuleFinder;
            _serialNumberRuleDeleter = serialNumberRuleDeleter;
        }

        [Description("编号规则列表")]
        public IActionResult Index(SerialNumberModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<SerialNumberRule> filter = FilterContainerBuilder.Build<SerialNumberRule>();
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.EntityId == model.EntityId);
            }
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name == model.Name);
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
            PagedList<SerialNumberRule> result = _serialNumberRuleFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                , SolutionId.Value, true);

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [Description("新建编号规则")]
        public IActionResult CreateSerialNumber()
        {
            CreateSerialNumberModel model = new CreateSerialNumberModel
            {
                DateFormatType = -1
            };
            return View(model);
        }

        [Description("新建编号规则")]
        [HttpPost]
        public IActionResult CreateSerialNumber(CreateSerialNumberModel model)
        {
            if (ModelState.IsValid)
            {
                var exists = _serialNumberRuleFinder.FindByEntityId(model.EntityId);
                if (exists != null)
                {
                    return JError(T["serial_number_duplicated"] + ": " + exists.Name);
                }
                var entity = new SerialNumberRule();
                model.CopyTo(entity);
                entity.SerialNumberRuleId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.CreatedOn = DateTime.Now;
                entity.SolutionId = SolutionId.Value;
                entity.ComponentState = 0;
                return _serialNumberRuleCreater.Create(entity).CreateResult(T, new { id = entity.SerialNumberRuleId });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑编号规则")]
        public IActionResult EditSerialNumber(Guid id)
        {
            EditSerialNumberModel model = new EditSerialNumberModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _serialNumberRuleFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.SerialNumberRuleId = id;
                    return View(model);
                }
            }
            return NotFound();
        }

        [Description("编辑编号规则")]
        [HttpPost]
        public IActionResult EditSerialNumber(EditSerialNumberModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _serialNumberRuleFinder.FindById(model.SerialNumberRuleId);
                entity.DateFormatType = model.DateFormatType;
                entity.Description = model.Description;
                entity.Increment = model.Increment;
                entity.IncrementLength = model.IncrementLength;
                entity.Name = model.Name;
                entity.Prefix = model.Prefix;
                entity.Seprator = model.Seprator;
                entity.ModifiedBy = CurrentUser.SystemUserId;
                entity.ModifiedOn = DateTime.Now;
                return _serialNumberRuleUpdater.Update(entity).UpdateResult(T);
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("删除编号规则")]
        [HttpPost]
        public IActionResult DeleteSerialNumber([FromBody]DeleteManyModel model)
        {
            return _serialNumberRuleDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置编号规则可用状态")]
        [HttpPost]
        public IActionResult SetSerialNumberRuleState([FromBody]SetRecordStateModel model)
        {
            return _serialNumberRuleUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }
    }
}