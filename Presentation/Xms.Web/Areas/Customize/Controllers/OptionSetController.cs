using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.OptionSet;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 选项集管理控制器
    /// </summary>
    public class OptionSetController : CustomizeBaseController
    {
        private readonly IOptionSetCreater _optionSetCreater;
        private readonly IOptionSetDeleter _optionSetDeleter;
        private readonly IOptionSetUpdater _optionSetUpdater;
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDetailCreater _optionSetDetailCreater;
        private readonly IOptionSetDetailDeleter _optionSetDetailDeleter;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IOptionSetDetailUpdater _optionSetDetailUpdater;

        public OptionSetController(IWebAppContext appContext
            , ISolutionService solutionService
            , IOptionSetCreater optionSetCreater
            , IOptionSetDeleter optionSetDeleter
            , IOptionSetUpdater optionSetUpdater
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailCreater optionSetDetailCreater
            , IOptionSetDetailDeleter optionSetDetailDeleter
            , IOptionSetDetailFinder optionSetDetailFinder
            , IOptionSetDetailUpdater optionSetDetailUpdater)
            : base(appContext, solutionService)
        {
            _optionSetCreater = optionSetCreater;
            _optionSetDeleter = optionSetDeleter;
            _optionSetUpdater = optionSetUpdater;
            _optionSetFinder = optionSetFinder;
            _optionSetDetailCreater = optionSetDetailCreater;
            _optionSetDetailDeleter = optionSetDetailDeleter;
            _optionSetDetailFinder = optionSetDetailFinder;
            _optionSetDetailUpdater = optionSetDetailUpdater;
        }

        [Description("选项集列表")]
        public IActionResult Index(OptionSetModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<Schema.Domain.OptionSet> filter = FilterContainerBuilder.Build<Schema.Domain.OptionSet>();
            filter.And(n => n.IsPublic == true);
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

            PagedList<Schema.Domain.OptionSet> result = _optionSetFinder.QueryPaged(x => x
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
        [Description("新建选项集")]
        public IActionResult CreateOptionSet()
        {
            EditOptionSetModel model = new EditOptionSetModel
            {
                SolutionId = SolutionId.Value
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建选项集-保存")]
        public IActionResult CreateOptionSet(EditOptionSetModel model)
        {
            if (ModelState.IsValid)
            {
                bool hasDupName = model.OptionSetName.GroupBy(x => x).Where(g => g.Count() > 1).Count() > 0;
                if (hasDupName)
                {
                    return JError(T["validation_name_duplicated"]);
                }
                bool hasDupVal = model.OptionSetValue.GroupBy(x => x).Where(g => g.Count() > 1).Count() > 0;
                if (hasDupVal)
                {
                    return JError(T["validation_value_duplicated"]);
                }
                Schema.Domain.OptionSet os = new Schema.Domain.OptionSet();
                os.OptionSetId = Guid.NewGuid();
                os.Name = model.Name;
                os.IsPublic = true;
                os.SolutionId = model.SolutionId;
                os.CreatedBy = CurrentUser.SystemUserId;
                List<Schema.Domain.OptionSetDetail> details = new List<Schema.Domain.OptionSetDetail>();
                int i = 0;
                foreach (var item in model.OptionSetName)
                {
                    if (item.IsEmpty()) continue;
                    Schema.Domain.OptionSetDetail osd = new Schema.Domain.OptionSetDetail();
                    osd.OptionSetDetailId = Guid.NewGuid();
                    osd.OptionSetId = os.OptionSetId;
                    osd.Name = item;
                    osd.Value = model.OptionSetValue[i];
                    osd.IsSelected = model.IsSelectedOption[i];
                    osd.DisplayOrder = i;
                    details.Add(osd);
                    i++;
                }
                os.Items = details;
                _optionSetCreater.Create(os);
                return CreateSuccess(new { id = os.OptionSetId });
            }
            return CreateFailure(GetModelErrors());
        }

        [HttpGet]
        [Description("选项集编辑")]
        public IActionResult EditOptionSet(Guid id)
        {
            EditOptionSetModel model = new EditOptionSetModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _optionSetFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.Details = entity.Items.OrderBy(x => x.DisplayOrder).ToList();
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("选项集信息保存")]
        public IActionResult EditOptionSet(EditOptionSetModel model)
        {
            if (ModelState.IsValid)
            {
                bool hasDupName = model.OptionSetName.GroupBy(x => x).Where(g => g.Count() > 1).Count() > 0;
                if (hasDupName)
                {
                    return JError(T["validation_name_duplicated"]);
                }
                bool hasDupVal = model.OptionSetValue.GroupBy(x => x).Where(g => g.Count() > 1).Count() > 0;
                if (hasDupVal)
                {
                    return JError(T["validation_value_duplicated"]);
                }
                var entity = _optionSetFinder.FindById(model.OptionSetId);
                var details = entity.Items;
                model.CopyTo(entity);
                entity.IsPublic = true;
                _optionSetUpdater.Update(entity);
                int i = 0;
                foreach (var item in model.OptionSetName)
                {
                    if (item.IsEmpty()) continue;
                    Guid detailid = model.DetailId[i];
                    Schema.Domain.OptionSetDetail osd = new Schema.Domain.OptionSetDetail();
                    osd.OptionSetId = entity.OptionSetId;
                    osd.Name = item;
                    osd.Value = model.OptionSetValue[i];
                    osd.IsSelected = model.IsSelectedOption[i];
                    osd.DisplayOrder = i;
                    if (detailid.Equals(Guid.Empty))
                    {
                        osd.OptionSetDetailId = Guid.NewGuid();
                        _optionSetDetailCreater.Create(osd);
                    }
                    else
                    {
                        osd.OptionSetDetailId = detailid;
                        _optionSetDetailUpdater.Update(osd);
                        details.Remove(details.Find(n => n.OptionSetDetailId == detailid));
                    }

                    i++;
                }
                //delete lost detail
                if (details.NotEmpty())
                {
                    var lostid = details.Select(n => n.OptionSetDetailId).ToList();
                    _optionSetDetailDeleter.DeleteById(lostid.ToArray());
                }

                return UpdateSuccess(new { id = entity.OptionSetId });
            }
            return UpdateFailure(GetModelErrors());
        }

        [Description("删除选项集")]
        [HttpPost]
        public IActionResult DeleteOptionSet([FromBody]DeleteManyModel model)
        {
            return _optionSetDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}