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
using Xms.Schema.OptionSet;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 字段元数据接口更新
    /// </summary>
    [Route("{org}/api/schema/attribute/update")]
    public class AttributeUpdaterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IAttributeUpdater _attributeUpdater;
        private readonly IOptionSetFinder _optionSetFinder;

        public AttributeUpdaterController(IWebAppContext appContext
            , IEntityFinder entityService
            , IAttributeFinder attributeService
            , IAttributeUpdater attributeUpdater
            , IOptionSetFinder optionSetFinder
            )
            : base(appContext)
        {
            _entityFinder = entityService;
            _attributeFinder = attributeService;
            _attributeUpdater = attributeUpdater;
            _optionSetFinder = optionSetFinder;
        }


        [HttpPost]        
        [Description("字段编辑")]
        public IActionResult Post(EditAttributeModel model)
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
                attrInfo.LogEnabled = model.IsLoged;
                attrInfo.IsRequired = model.IsRequired;
                attrInfo.AuthorizationEnabled = model.IsSecured;
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
    }
}