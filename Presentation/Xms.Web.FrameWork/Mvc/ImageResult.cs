using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Xms.Web.Framework
{
    /// <summary>
    /// 图片结果视图
    /// </summary>
    public class ImageResult : ActionResult
    {
        public byte[] Image { get; set; }

        public string ContentType { get; set; }

        public ImageResult(byte[] image, string contenttype)
        {
            Image = image ?? throw new ArgumentNullException("image");
            ContentType = contenttype;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            var response = context.HttpContext.Response;
            response.Clear();
            if (!string.IsNullOrWhiteSpace(ContentType)) response.ContentType = ContentType;
            return response.Body.WriteAsync(Image, 0, Image.Length);
        }
    }
}