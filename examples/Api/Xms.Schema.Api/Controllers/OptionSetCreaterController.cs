using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Schema.Api.Models;
using Xms.Schema.OptionSet;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Infrastructure.Utility;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 选项集接口新增
    /// </summary>
    [Route("{org}/api/schema/optionset/create")]
    public class OptionSetCreaterController : ApiControllerBase
    {
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IOptionSetCreater _optionSetCreater;

        public OptionSetCreaterController(IWebAppContext appContext
            , IOptionSetCreater optionSetCreater
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailFinder optionSetDetailFinder) : base(appContext)
        {
            _optionSetCreater = optionSetCreater;
            _optionSetFinder = optionSetFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
        }

        [HttpPost]        
        [Description("新建选项集-保存")]
        public IActionResult Post(EditOptionSetModel model)
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

    }
}