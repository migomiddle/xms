using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xms.Core.Context;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.WebResource;
using Xms.WebResource.Abstractions;

namespace Xms.Web.Api
{
    /// <summary>
    /// Web资源加载控制器
    /// </summary>
    [Route("{org}/api/webresource")]
    public class WebResourceController : ApiControllerBase
    {
        private readonly IWebResourceFinder _webResourceFinder;
        private readonly IWebResourceContentCoder _webResourceContentCoder;

        public WebResourceController(IWebAppContext appContext
            , IWebResourceFinder webResourceFinder
            , IWebResourceContentCoder webResourceContentCoder)
            : base(appContext)
        {
            _webResourceFinder = webResourceFinder;
            _webResourceContentCoder = webResourceContentCoder;
        }

        [Description("加载Web资源")]
        [HttpGet]
        public IActionResult Get(string ids)
        {
            if (ids.IsNotEmpty())
            {
                var idList = new List<Guid>();
                if (ids.IndexOf(",") > 0)
                {
                    foreach (var item in ids.Split(','))
                    {
                        if (Guid.TryParse(item, out Guid id))
                        {
                            idList.Add(id);
                        }
                    }
                }
                else
                {
                    if (Guid.TryParse(ids, out Guid id))
                    {
                        idList.Add(id);
                    }
                }
                StringBuilder content = new StringBuilder();
                var result = _webResourceFinder.FindByIds(idList.ToArray());
                foreach (var item in result)
                {
                    content.Append(_webResourceContentCoder.CodeDecode(item.Content));
                }
                return Content(content.ToString());
            }
            return Content(T["notfound_record"]);
        }

        [Description("加载图片资源")]
        [HttpGet("Picture")]
        public IActionResult Picture(Guid id)
        {
            var entity = _webResourceFinder.FindById(id);
            if (entity != null && entity.WebResourceType == WebResourceType.Picture)
            {
                return new ImageResult(_webResourceContentCoder.DecodeToByte(entity.Content), "image/jpeg");
            }
            return Content("");
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _webResourceFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.WebResourceId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = WebResourceDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }
    }
}