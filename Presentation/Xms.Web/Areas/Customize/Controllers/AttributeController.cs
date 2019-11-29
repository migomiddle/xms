using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Schema.StringMap;
using Xms.Sdk.Abstractions.Query;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 字段管理控制器
    /// </summary>
    public class AttributeController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IAttributeCreater _attributeCreater;
        private readonly IAttributeDeleter _attributeDeleter;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IAttributeUpdater _attributeUpdater;

        public AttributeController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailFinder optionSetDetailFinder
            , IStringMapFinder stringMapFinder
            , IAttributeCreater attributeCreater
            , IAttributeDeleter attributeDeleter
            , IAttributeFinder attributeFinder
            , IAttributeUpdater attributeUpdater)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _optionSetFinder = optionSetFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
            _stringMapFinder = stringMapFinder;
            _attributeCreater = attributeCreater;
            _attributeDeleter = attributeDeleter;
            _attributeFinder = attributeFinder;
            _attributeUpdater = attributeUpdater;
        }

        [Description("字段列表")]
        public IActionResult Index(AttributeModel model)
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
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }

            FilterContainer<Schema.Domain.Attribute> container = FilterContainerBuilder.Build<Schema.Domain.Attribute>();
            container.And(n => n.EntityId == model.EntityId);
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name.Like(model.Name));
            }
            if (model.AttributeTypeName != null && model.AttributeTypeName.Length > 0)
            {
                container.And(n => n.AttributeTypeName.In(model.AttributeTypeName));
            }
            if (model.FilterSysAttribute)
            {
                container.And(n => n.Name.NotIn(AttributeDefaults.SystemAttributes));
                container.And(n => n.AttributeTypeName != AttributeTypeIds.PRIMARYKEY);
            }
            if (!model.IsSortBySeted)
            {
                model.SortBy = "name";
                model.SortDirection = (int)SortDirection.Asc;
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
            PagedList<Schema.Domain.Attribute> result = _attributeFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(container)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [Description("检查字段是否已存在")]
        public IActionResult Exists(Guid entityid, string name)
        {
            var isExists = _attributeFinder.IsExists(entityid, name);
            if (isExists)
            {
                return JError(T["ATTRIBUTE_NAME_EXISTS"]);
            }
            return JOk("");
        }

        [HttpGet]
        [Description("新建字段")]
        public IActionResult CreateAttribute(Guid entityid)
        {
            if (entityid.Equals(Guid.Empty))
            {
                return NotFound();
            }
            CreateAttributeModel model = new CreateAttributeModel();
            model.SolutionId = SolutionId.Value;
            model.Entity = _entityFinder.FindById(entityid);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建字段")]
        public IActionResult CreateAttribute(CreateAttributeModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _entityFinder.FindById(model.EntityId);
                if (entity == null)
                {
                    return NotFound();
                }
                var attr = _attributeFinder.Find(model.EntityId, model.Name);
                if (attr != null)
                {
                    return JError(T["attribute_name_exists"]);
                }
                var attrInfo = new Schema.Domain.Attribute();
                //model.CopyTo(entity);
                attrInfo.EntityId = entity.EntityId;
                attrInfo.EntityName = entity.Name;
                attrInfo.Name = model.Name.Trim();
                attrInfo.LocalizedName = model.LocalizedName;
                attrInfo.AttributeId = Guid.NewGuid();
                attrInfo.IsNullable = model.IsNullable;
                attrInfo.IsRequired = model.IsRequired;
                attrInfo.LogEnabled = model.LogEnabled;
                attrInfo.IsCustomizable = true;
                attrInfo.IsCustomField = true;
                attrInfo.AuthorizationEnabled = model.AuthorizationEnabled;
                attrInfo.CreatedBy = CurrentUser.SystemUserId;
                attrInfo.Description = model.Description;
                attrInfo.ValueType = model.ValueType;
                switch (model.AttributeType)
                {
                    case AttributeTypeIds.NVARCHAR:
                        attrInfo.MaxLength = model.MaxLength.Value;
                        attrInfo.DataFormat = model.TextFormat;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        break;

                    case AttributeTypeIds.NTEXT:
                        attrInfo.DataFormat = model.NTextFormat;
                        break;

                    case AttributeTypeIds.INT:
                        attrInfo.MinValue = model.IntMinValue.Value <= int.MinValue ? int.MinValue + 2 : model.IntMinValue.Value;
                        attrInfo.MaxValue = model.IntMaxValue.Value >= int.MaxValue ? int.MaxValue - 2 : model.IntMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.FLOAT:
                        attrInfo.Precision = model.FloatPrecision.Value;
                        attrInfo.MinValue = model.FloatMinValue.Value <= float.MinValue ? float.MinValue + 2 : model.FloatMinValue.Value;
                        attrInfo.MaxValue = model.FloatMaxValue.Value >= float.MaxValue ? float.MaxValue - 2 : model.FloatMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.MONEY:
                        attrInfo.Precision = model.MoneyPrecision.Value;
                        attrInfo.MinValue = model.MoneyMinValue.Value <= float.MinValue ? float.MinValue + 2 : model.MoneyMinValue.Value;
                        attrInfo.MaxValue = model.MoneyMaxValue.Value >= float.MaxValue ? float.MaxValue - 2 : model.MoneyMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.PICKLIST:
                        attrInfo.DisplayStyle = model.OptionSetType;
                        if (model.IsCommonOptionSet)
                        {
                            attrInfo.OptionSetId = model.CommonOptionSet.Value;
                        }
                        else
                        {
                            if (model.OptionSetName.IsEmpty())
                            {
                                return JError(T["attribute_options_empty"]);
                            }
                            //新建选项集
                            Schema.Domain.OptionSet os = new Schema.Domain.OptionSet();
                            os.OptionSetId = Guid.NewGuid();
                            os.Name = model.Name;
                            os.IsPublic = false;
                            List<Schema.Domain.OptionSetDetail> details = new List<Schema.Domain.OptionSetDetail>();
                            int i = 0;
                            foreach (var item in model.OptionSetName)
                            {
                                if (item.IsEmpty())
                                {
                                    continue;
                                }

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
                            attrInfo.OptionSetId = os.OptionSetId;
                            os.Items = details;
                            attrInfo.OptionSet = os;
                        }
                        break;

                    case AttributeTypeIds.BIT:
                        if (model.BitOptionName.IsEmpty())
                        {
                            return JError(T["attribute_options_empty"]);
                        }
                        //新建选项集
                        List<Schema.Domain.StringMap> pickListItems = new List<Schema.Domain.StringMap>();
                        int j = 0;
                        foreach (var item in model.BitOptionName)
                        {
                            Schema.Domain.StringMap s = new Schema.Domain.StringMap();
                            s.StringMapId = Guid.NewGuid();
                            s.Name = item;
                            s.Value = j == 0 ? 1 : 0;//第一项为true选项
                            s.DisplayOrder = j;
                            s.AttributeId = attrInfo.AttributeId;
                            s.EntityName = attrInfo.EntityName;
                            s.AttributeName = attrInfo.Name;
                            j++;
                            pickListItems.Add(s);
                        }
                        attrInfo.PickLists = pickListItems;
                        break;

                    case AttributeTypeIds.DATETIME:
                        attrInfo.DataFormat = model.DateTimeFormat;
                        break;

                    case AttributeTypeIds.LOOKUP:
                        attrInfo.ReferencedEntityId = model.LookupEntity.Value;
                        attrInfo.DisplayStyle = model.LookupType;
                        break;

                    case AttributeTypeIds.PARTYLIST:
                        attrInfo.DataFormat = model.PartyListFormat;
                        break;
                }
                attrInfo.AttributeTypeName = model.AttributeType;
                _attributeCreater.Create(attrInfo);
                return CreateSuccess(new { id = attrInfo.AttributeId });
            }
            return JModelError(T["created_error"]);
        }

        [HttpGet]
        [Description("字段编辑")]
        public IActionResult EditAttribute(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }
            EditAttributeModel model = new EditAttributeModel();
            var entity = _attributeFinder.FindById(id);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = _entityFinder.FindById(entity.EntityId);
            if (model.Entity != null)
            {
                entity.CopyTo(model);
                if (entity.OptionSetId.HasValue)
                {
                    entity.OptionSet = _optionSetFinder.FindById(entity.OptionSetId.Value);
                    entity.OptionSet.Items = _optionSetDetailFinder.Query(n => n.Where(w => w.OptionSetId == entity.OptionSetId.Value).Sort(s => s.SortAscending(f => f.DisplayOrder)));

                    model.IsCommonOptionSet = entity.OptionSet.IsPublic;
                }
                if (entity.TypeIsBit() || entity.TypeIsState())
                {
                    entity.PickLists = _stringMapFinder.Query(n => n.Where(w => w.AttributeId == entity.AttributeId));
                }
                model.Attribute = entity;
                if (model.SummaryExpression.IsNotEmpty())
                {
                    model.AaExp = new AttributeAggregateExpression().DeserializeFromJson(model.SummaryExpression);
                }
                else
                {
                    model.AaExp = new AttributeAggregateExpression();
                }

                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("字段编辑")]
        public IActionResult EditAttribute(EditAttributeModel model)
        {
            if (ModelState.IsValid)
            {
                var attrInfo = _attributeFinder.FindById(model.AttributeId.Value);
                if (attrInfo == null)
                {
                    return NotFound();
                }
                //model.CopyTo(attrInfo);
                //attrInfo.IsCustomizable = true;
                attrInfo.LocalizedName = model.LocalizedName;
                attrInfo.LogEnabled = model.LogEnabled;
                attrInfo.IsRequired = model.IsRequired;
                attrInfo.AuthorizationEnabled = model.AuthorizationEnabled;
                attrInfo.Description = model.Description;
                attrInfo.ValueType = model.ValueType;
                var attrTypeName = attrInfo.AttributeTypeName;
                if (attrTypeName == "state")
                {
                    attrTypeName = "bit";
                }
                else if (attrTypeName == "status")
                {
                    attrTypeName = "picklist";
                }

                switch (attrTypeName)
                {
                    case AttributeTypeIds.NVARCHAR:
                        attrInfo.MaxLength = model.MaxLength.Value;
                        attrInfo.DataFormat = model.TextFormat;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        break;

                    case AttributeTypeIds.NTEXT:
                        attrInfo.DataFormat = model.NTextFormat;
                        break;

                    case AttributeTypeIds.INT:
                        attrInfo.MinValue = model.IntMinValue.Value <= int.MinValue ? int.MinValue + 2 : model.IntMinValue.Value;
                        attrInfo.MaxValue = model.IntMaxValue.Value >= int.MaxValue ? int.MaxValue - 2 : model.IntMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.FLOAT:
                        attrInfo.Precision = model.FloatPrecision.Value;
                        attrInfo.MinValue = model.FloatMinValue.Value <= float.MinValue ? float.MinValue + 2 : model.FloatMinValue.Value;
                        attrInfo.MaxValue = model.FloatMaxValue.Value >= float.MaxValue ? float.MaxValue - 2 : model.FloatMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.MONEY:
                        attrInfo.Precision = model.MoneyPrecision.Value;
                        attrInfo.MinValue = model.MoneyMinValue.Value <= float.MinValue ? float.MinValue + 2 : model.MoneyMinValue.Value;
                        attrInfo.MaxValue = model.MoneyMaxValue.Value >= float.MaxValue ? float.MaxValue - 2 : model.MoneyMaxValue.Value;
                        attrInfo.DefaultValue = model.DefaultValue;
                        if (model.ValueType == 2)
                        {
                            attrInfo.FormulaExpression = model.FormulaExpression;
                        }
                        else if (model.ValueType == 3)
                        {
                            attrInfo.SummaryEntityId = model.SummaryEntityId;
                            attrInfo.SummaryExpression = model.SummaryExpression;
                        }
                        break;

                    case AttributeTypeIds.PICKLIST:
                        attrInfo.DisplayStyle = model.OptionSetType;
                        attrInfo.OptionSet = _optionSetFinder.FindById(attrInfo.OptionSetId.Value);
                        if (!attrInfo.OptionSet.IsPublic)
                        {
                            if (model.OptionSetName.IsEmpty())
                            {
                                return JError(T["attribute_options_empty"]);
                            }
                            //选项集
                            List<Schema.Domain.OptionSetDetail> details = new List<Schema.Domain.OptionSetDetail>();
                            int i = 0;
                            foreach (var item in model.OptionSetName)
                            {
                                if (item.IsEmpty())
                                {
                                    continue;
                                }

                                Schema.Domain.OptionSetDetail osd = new Schema.Domain.OptionSetDetail();
                                osd.OptionSetDetailId = model.OptionSetDetailId[i];
                                osd.OptionSetId = attrInfo.OptionSetId.Value;
                                osd.Name = item;
                                osd.Value = model.OptionSetValue[i];
                                osd.IsSelected = model.IsSelectedOption[i];
                                osd.DisplayOrder = i;
                                details.Add(osd);

                                i++;
                            }
                            attrInfo.OptionSet.Items = details;
                        }
                        break;

                    case AttributeTypeIds.BIT:
                        //新建选项集
                        List<Schema.Domain.StringMap> pickListItems = new List<Schema.Domain.StringMap>();
                        int j = 0;
                        foreach (var item in model.BitOptionName)
                        {
                            Schema.Domain.StringMap s = new Schema.Domain.StringMap();
                            s.StringMapId = model.BitDetailId[j];
                            s.Name = item;
                            s.Value = j == 0 ? 1 : 0;//第一项为true选项
                            s.DisplayOrder = j;
                            s.AttributeId = attrInfo.AttributeId;
                            s.AttributeName = attrInfo.Name;
                            s.EntityName = attrInfo.EntityName;
                            j++;
                            pickListItems.Add(s);
                        }
                        attrInfo.PickLists = pickListItems;
                        break;

                    case AttributeTypeIds.DATETIME:
                        attrInfo.DataFormat = model.DateTimeFormat;
                        break;

                    case AttributeTypeIds.LOOKUP:
                        attrInfo.DisplayStyle = model.LookupType;
                        //attrInfo.ReferencedEntityId = model.LookupEntity.Value;
                        break;

                    case AttributeTypeIds.PARTYLIST:
                        attrInfo.DataFormat = model.PartyListFormat;
                        break;
                }

                _attributeUpdater.Update(attrInfo);
                return UpdateSuccess(new { id = attrInfo.AttributeId });
            }
            return JModelError(T["saved_error"]);
        }

        [Description("删除字段")]
        [HttpPost]
        public IActionResult DeleteAttribute([FromBody]DeleteManyModel model)
        {
            return _attributeDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}