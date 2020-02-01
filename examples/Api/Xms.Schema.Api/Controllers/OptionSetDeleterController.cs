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
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 选项集接口删除
    /// </summary>
    [Route("{org}/api/schema/optionset/delete")]
    public class OptionSetDeleterController : ApiControllerBase
    {
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDeleter _optionSetDeleter;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IOptionSetDetailDeleter _optionSetDetailDeleter;
        

        public OptionSetDeleterController(IWebAppContext appContext
             , IOptionSetDeleter optionSetDeleter
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailDeleter optionSetDetailDeleter
            , IOptionSetDetailFinder optionSetDetailFinder) : base(appContext)
        {
            _optionSetDeleter = optionSetDeleter;
            _optionSetFinder = optionSetFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
            _optionSetDetailDeleter = optionSetDetailDeleter;
        }

        [Description("删除选项集")]
        [HttpPost]
        public IActionResult Post(DeleteManyModel model)
        {
            return _optionSetDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

    }
}