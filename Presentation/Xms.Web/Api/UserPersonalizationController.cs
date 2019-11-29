using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Infrastructure.Utility;
using Xms.UserPersonalization;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{  /// <summary>
   /// 用户个性化
   /// </summary>
    [Route("{org}/api/userpersonalization")]
    public class UserPersonalizationController : ApiControllerBase
    {
        private readonly IUserPersonalizationService _userPersonalizationService;

        public UserPersonalizationController(IWebAppContext appContext
            , IUserPersonalizationService userPersonalizationService)
            : base(appContext)
        {
            _userPersonalizationService = userPersonalizationService;
        }

        [Description("获取用户个性化")]
        [HttpGet("{ownerId}")]
        public IActionResult Get(Guid ownerId)
        {
            var result = _userPersonalizationService.Get(ownerId);
            if (result.IsEmpty())
            {
                return NotFound();
            }
            return JOk(result);
        }

        [Description("根据名称获取用户个性化")]
        [HttpGet("getByName")]
        public IActionResult GetByName(Guid ownerId, string name)
        {
            var result = _userPersonalizationService.GetByName(ownerId, name);
            if (result == null)
            {
                return NotFound();
            }
            return JOk(result);
        }

        [Description("设置用户个性化")]
        [HttpPost("set")]
        public IActionResult Set(SetUserPersonalizationModel model)
        {
            var userPersonalization = new UserPersonalization.Domain.UserPersonalization()
            {
                Id = model.Id,
                Name = model.Name,
                OwnerId = model.OwnerId,
                Value = model.Value
            };

            return _userPersonalizationService.Set(userPersonalization) ? SaveSuccess() : SaveFailure();
        }

        [Description("设置用户个性化")]
        [HttpPost("delete")]
        public IActionResult Delete(Guid ownerId)
        {
            return _userPersonalizationService.Delete(ownerId) ? SaveSuccess() : SaveFailure();
        }

        [Description("设置用户个性化")]
        [HttpPost("deleteByName")]
        public IActionResult DeleteByName(Guid ownerId, string name)
        {
            return _userPersonalizationService.Delete(ownerId, name) ? SaveSuccess() : SaveFailure();
        }

        [Description("设置用户个性化")]
        [HttpPost("deleteById")]
        public IActionResult DeleteById(Guid id)
        {
            return _userPersonalizationService.DeleteById(id) ? SaveSuccess() : SaveFailure();
        }
    }
}