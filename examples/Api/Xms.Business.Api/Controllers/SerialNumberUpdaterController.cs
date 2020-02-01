using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xms.Api.Core.Controller;
using Xms.Business.Api.Models;
using Xms.Business.SerialNumber;
using Xms.Business.SerialNumber.Domain;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/serialnumber/update")]    
    public class SerialNumberUpdaterController : ApiCustomizeControllerBase
    {
        private readonly ISerialNumberRuleCreater _serialNumberRuleCreater;
        private readonly ISerialNumberRuleUpdater _serialNumberRuleUpdater;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly ISerialNumberRuleDeleter _serialNumberRuleDeleter;
        public SerialNumberUpdaterController(IWebAppContext appContext
            , ISerialNumberRuleCreater serialNumberRuleCreater
            , ISerialNumberRuleUpdater serialNumberRuleUpdater
            , ISerialNumberRuleFinder serialNumberRuleFinder
            , ISerialNumberRuleDeleter serialNumberRuleDeleter
            , ISolutionService solutionService) : base(appContext, solutionService)
        {
            _serialNumberRuleCreater = serialNumberRuleCreater;
            _serialNumberRuleUpdater = serialNumberRuleUpdater;
            _serialNumberRuleFinder = serialNumberRuleFinder;
            _serialNumberRuleDeleter = serialNumberRuleDeleter;
        }

        [Description("编辑编号规则")]
        [HttpPost]
        public IActionResult Post(EditSerialNumberModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _serialNumberRuleFinder.FindById(model.SerialNumberRuleId);
                entity.DateFormatType = model.DateFormatType;
                entity.Description = model.Description;
                entity.Increment = model.Increment;
                entity.IncrementLength = model.IncrementLength;
                entity.Name = model.Name;
                entity.Prefix = model.Prefix;
                entity.Seprator = model.Seprator;
                entity.ModifiedBy = CurrentUser.SystemUserId;
                entity.ModifiedOn = DateTime.Now;
                return _serialNumberRuleUpdater.Update(entity).UpdateResult(T);
            }
            return UpdateFailure(GetModelErrors());
        }


        [Description("设置编号规则可用状态")]
        [HttpPost("setstate")]
        public IActionResult SetSerialNumberRuleState([FromBody]SetRecordStateModel model)
        {
            return _serialNumberRuleUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }

    }
}
