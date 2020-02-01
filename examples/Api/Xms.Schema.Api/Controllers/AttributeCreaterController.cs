using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Api.Models;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 字段元数据接口新增
    /// </summary>
    [Route("{org}/api/schema/attribute/create")]
    public class AttributeCreaterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IAttributeCreater _attributeCreater;

        public AttributeCreaterController(IWebAppContext appContext
            , IEntityFinder entityService
            , IAttributeFinder attributeService
            ,IAttributeCreater attributeCreater)
            : base(appContext)
        {
            _entityFinder = entityService;
            _attributeFinder = attributeService;
            _attributeCreater = attributeCreater;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建字段")]
        public IActionResult Post(CreateAttributeModel model)
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
                attrInfo.LogEnabled = model.IsLoged;
                attrInfo.IsCustomizable = true;
                attrInfo.IsCustomField = true;
                attrInfo.AuthorizationEnabled = model.IsSecured;
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
    }
}