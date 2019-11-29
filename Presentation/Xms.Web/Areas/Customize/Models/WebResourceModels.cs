using Microsoft.AspNetCore.Http;
using System;
using Xms.Web.Framework.Paging;
using Xms.WebResource.Abstractions;

namespace Xms.Web.Customize.Models
{
    public class WebResourceModel : BasePaged<WebResource.Domain.WebResource>
    {
        public string Name { get; set; }
        public WebResourceType? WebResourceType { get; set; }
        public Guid? SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditWebResourceModel
    {
        public Guid? WebResourceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public WebResourceType WebResourceType { get; set; }
        public Guid SolutionId { get; set; }
        public int Type { get; set; }
        public IFormFile ResourceFile { get; set; }
    }
}