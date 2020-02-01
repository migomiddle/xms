using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.EntityData.Api.Models;
using Xms.Schema.Attribute;
using Xms.Schema.Extensions;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Web.Api
{
    /// <summary>
    /// 实体数据合并控制器
    /// </summary>
    [Route("{org}/api/data/merge")]
    public class DataMergeController : ApiControllerBase
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataMerger _dataMerger;

        public DataMergeController(IWebAppContext appContext
            , IAttributeFinder attributeFinder
            , IDataMerger dataMerger)
            : base(appContext)
        {
            _attributeFinder = attributeFinder;
            _dataMerger = dataMerger;
        }

        [Description("合并记录")]
        [HttpPost]
        public IActionResult Post([FromForm]MergeModel model)
        {
            if (!Arguments.HasValue(model.EntityId, model.RecordId1, model.RecordId2))
            {
                return JError(T["parameter_error"]);
            }
            var attributes = _attributeFinder.FindByEntityId(model.EntityId);
            Dictionary<string, Guid> attributeMaps = new Dictionary<string, Guid>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var attr in attributes)
            {
                if (!Request.Form.ContainsKey(attr.Name))
                {
                    continue;
                }

                if (attr.IsSystemControl())
                {
                    continue;
                }

                var mainAttr = Request.Form[attr.Name].ToString();
                attributeMaps.Add(attr.Name, Guid.Parse(mainAttr));
            }
            _dataMerger.Merge(model.EntityId, model.MainRecordId, model.MainRecordId.Equals(model.RecordId1) ? model.RecordId2 : model.RecordId1, attributeMaps);

            return SaveSuccess();
        }
    }
}