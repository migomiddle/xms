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
    /// 选项集接口更新
    /// </summary>
    [Route("{org}/api/schema/optionset/update")]
    public class OptionSetUpdaterController : ApiControllerBase
    {
        private readonly IOptionSetFinder _optionSetFinder;        
        private readonly IOptionSetUpdater _optionSetUpdater;
        private readonly IOptionSetDetailCreater _optionSetDetailCreater;
        private readonly IOptionSetDetailDeleter _optionSetDetailDeleter;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IOptionSetDetailUpdater _optionSetDetailUpdater;

        public OptionSetUpdaterController(IWebAppContext appContext
            , IOptionSetUpdater optionSetUpdater
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailFinder optionSetDetailFinder
            , IOptionSetDetailCreater optionSetDetailCreater
            , IOptionSetDetailUpdater optionSetDetailUpdater
            ) : base(appContext)
        {
            _optionSetUpdater = optionSetUpdater;
            _optionSetFinder = optionSetFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
            _optionSetDetailCreater = optionSetDetailCreater;
            _optionSetDetailUpdater = optionSetDetailUpdater;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("选项集信息保存")]
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

    }
}