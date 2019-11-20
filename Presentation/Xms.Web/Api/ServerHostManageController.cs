using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xms.ServerHostManage;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Infrastructure.Utility;
using System.ComponentModel;
namespace Xms.Web.Api
{
    /// <summary>
    /// 服务器主机管理
    /// </summary>
    [Route("{org}/api/serverhostmanage")]
    public class ServerHostManageController : ApiControllerBase
    {
        private readonly IServerHostManageService _serverHostManageService;


        public ServerHostManageController(IWebAppContext appContext
            , IServerHostManageService serverHostManageService
           )
            : base(appContext)
        {
            _serverHostManageService = serverHostManageService;
        }
        [Description("获取用户个性化")]
        [HttpGet("getSystemInfomation")]
        public IActionResult GetSystemInfomation()
        {
            var result = _serverHostManageService.GetSystemInfomation();
            return JOk(result);

        }
    }
}