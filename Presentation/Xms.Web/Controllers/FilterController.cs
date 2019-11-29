using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.OptionSet;
using Xms.Schema.RelationShip;
using Xms.Schema.StringMap;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 过滤条件控制器
    /// </summary>
    public class FilterController : WebControllerBase
    {
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IStringMapFinder _stringMapFinder;

        public FilterController(Framework.Context.IWebAppContext appContext
            , IRelationShipFinder relationShipService
            , IAttributeFinder attributeFinder
            , IOptionSetFinder optionSetFinder
            , IStringMapFinder stringMapFinder)
            : base(appContext)
        {
            _relationShipFinder = relationShipService;
            _attributeFinder = attributeFinder;
            _optionSetFinder = optionSetFinder;
            _stringMapFinder = stringMapFinder;
        }

        /// <summary>
        /// 过滤条件对话框
        /// </summary>
        /// <returns></returns>
        public IActionResult FilterDialog([FromBody]FilterModel model, DialogModel dm)
        {
            ViewData["DialogModel"] = dm;
            if (model.Field.IsNotEmpty())
            {
                //如果是关联表字段
                if (model.Field.IndexOf('.') > 0)
                {
                    var relationshipName = model.Field.Split('.')[0];
                    var field = model.Field.Split('.')[1];
                    //根据关系查询目标实体ID
                    var relationship = _relationShipFinder.FindByName(relationshipName);
                    model.AttributeMeta = _attributeFinder.Find(relationship.ReferencedEntityId, field);
                    model.RelationShipMeta = relationship;
                }
                else
                {
                    model.AttributeMeta = _attributeFinder.Find(model.EntityId, model.Field);
                }
                if (model.AttributeMeta == null)
                {
                    return NotFound();
                }
                if (model.AttributeMeta.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.PICKLIST))
                {
                    model.AttributeMeta.OptionSet = _optionSetFinder.FindById(model.AttributeMeta.OptionSetId.Value);
                }
                if (model.AttributeMeta.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.BIT)
                    || model.AttributeMeta.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.STATE))
                {
                    model.AttributeMeta.PickLists = _stringMapFinder.Query(n => n.Where(w => w.AttributeId == model.AttributeMeta.AttributeId));
                }
            }

            return View(model);
        }

        /// <summary>
        /// 筛选条件部分页
        /// </summary>
        /// <returns></returns>
        public IActionResult ScreenConditions()
        {
            return View();
        }

        [Description("简单过滤条件设置")]
        [HttpPost]
        public IActionResult SimpleFilterSection([FromBody]SimpleFilterModel model)
        {
            return View(model);
        }
    }
}