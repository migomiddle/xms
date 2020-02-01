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

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/serialnumber/create")]    
    public class SerialNumberCreaterController : ApiCustomizeControllerBase
    {
        private readonly ISerialNumberRuleCreater _serialNumberRuleCreater;
        private readonly ISerialNumberRuleUpdater _serialNumberRuleUpdater;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly ISerialNumberRuleDeleter _serialNumberRuleDeleter;
        public SerialNumberCreaterController(IWebAppContext appContext
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


    }
}
