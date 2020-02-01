using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Schema.OptionSet;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 选项集接口
    /// </summary>
    [Route("{org}/api/schema/optionset")]
    public class OptionSetController : ApiControllerBase
    {
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        public OptionSetController(IWebAppContext appContext
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailFinder optionSetDetailFinder) : base(appContext)
        {
            _optionSetFinder = optionSetFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
        }

        [Description("查询选项集")]
        [HttpGet]
        public IActionResult Get(bool isPublic)
        {
            var result = _optionSetFinder.Query(n => n.Where(f => f.IsPublic == isPublic));
            return JOk(result);
        }

        [Description("查询选项集选项")]
        [HttpGet("getitems/{optionsetid}")]
        public IActionResult GetItems(Guid optionsetid)
        {
            var result = _optionSetDetailFinder.Query(n => n.Where(f => f.OptionSetId == optionsetid));
            return JOk(result);
        }
    }
}