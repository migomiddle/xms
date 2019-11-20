using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xms.File;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    [Route("{org}/file/[action]")]
    public class FileController : WebControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IAttachmentFinder _attachmentFinder;
        private readonly IWebHelper _webHelper;

        public FileController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IAttachmentFinder attachmentFinder
            , IWebHelper webHelper)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _attachmentFinder = attachmentFinder;
            _webHelper = webHelper;
        }

        //[Description("文件列表")]
        //public IActionResult Index(AttachmentsModel model)
        //{
        //    return View();
        //}

        [Description("附件列表")]
        public IActionResult AttachmentsDialog(AttachmentsModel model, DialogModel dm)
        {
            if (!Arguments.HasValue(model.EntityId, model.ObjectId))
            {
                return JError(T["parameter_error"]);
            }
            model.EntityMetaData = _entityFinder.FindByName("attachment");
            model.AttributeMetaDatas = _attributeFinder.FindByEntityId(model.EntityMetaData.EntityId);
            var result = _attachmentFinder.QueryPaged(model.Page, model.PageSize, model.EntityId, model.ObjectId);
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            ViewData["DialogModel"] = dm;
            return View(model);
        }

        [Description("下载附件")]
        public IActionResult Download(Guid id, string sid, bool preview = false)
        {
            if (!sid.IsCaseInsensitiveEqual(CurrentUser.SessionId))
            {
                return NotFound();
            }
            var result = _attachmentFinder.FindById(id);
            if (result.IsEmpty())
            {
                return NotFound();
            }
            var filePath = result.GetStringValue("cdnpath");
            if (preview)
            {
                if (filePath.IsNotEmpty())
                {
                    return new FileStreamResult(System.IO.File.OpenRead(_webHelper.MapPath(filePath)), result.GetStringValue("mimetype"));
                }
            }
            if (filePath.IsNotEmpty())
            {
                return PhysicalFile(_webHelper.MapPath(filePath), result.GetStringValue("mimetype"), result.GetStringValue("name"));
            }
            else if (result.GetStringValue("body").IsNotEmpty())
            {
                byte[] bt = Convert.FromBase64String(result.GetStringValue("body"));
                //System.IO.MemoryStream stream = new System.IO.MemoryStream(bt, 0, bt.Length);
                return File(bt, result.GetStringValue("mimetype"), result.GetStringValue("name").UrlEncode());
            }
            return NotFound();
        }
    }

    [Route("{org}/file/[action]")]
    public class FileCreaterController : WebControllerBase
    {
        private readonly IAttachmentCreater _attachmentCreater;

        public FileCreaterController(IWebAppContext appContext
            , IAttachmentCreater attachmentCreater)
            : base(appContext)
        {
            _attachmentCreater = attachmentCreater;
        }

        [Description("新建附件")]
        public IActionResult Create()
        {
            CreateAttachmentModel model = new CreateAttachmentModel();
            return View(model);
        }

        [Description("新建附件")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAttachmentModel model)
        {
            if (model.EntityId.Equals(Guid.Empty) || model.ObjectId.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            if (model.Attachments.NotEmpty())
            {
                var result = await _attachmentCreater.CreateManyAsync(model.EntityId, model.ObjectId, model.Attachments).ConfigureAwait(false);

                if (result.NotEmpty())
                {
                    return JOk(T["saved_success"], result);
                }
            }
            return SaveFailure();
        }
    }

    [Route("{org}/file/[action]")]
    public class FileDeleterController : WebControllerBase
    {
        private readonly IAttachmentDeleter _attachmentDeleter;

        public FileDeleterController(IWebAppContext appContext
            , IAttachmentDeleter attachmentDeleter)
            : base(appContext)
        {
            _attachmentDeleter = attachmentDeleter;
        }

        [Description("删除附件")]
        public IActionResult Delete([FromBody]DeleteAttachmentModel model)
        {
            if (model.EntityId.Equals(Guid.Empty) || model.ObjectId.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            var flag = _attachmentDeleter.DeleteById(model.EntityId, model.ObjectId);
            return flag.DeleteResult(T);
        }
    }
}