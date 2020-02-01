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
    [Route("{org}/api/serialnumber")]    
    public class SerialNumberController : ApiCustomizeControllerBase
    {
        private readonly ISerialNumberRuleCreater _serialNumberRuleCreater;
        private readonly ISerialNumberRuleUpdater _serialNumberRuleUpdater;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly ISerialNumberRuleDeleter _serialNumberRuleDeleter;
        public SerialNumberController(IWebAppContext appContext
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

        [Description("编号规则列表")]
        public IActionResult Get(SerialNumberModel model)
        {
            FilterContainer<SerialNumberRule> filter = FilterContainerBuilder.Build<SerialNumberRule>();
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name == model.Name);
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = 25000;
            }
            else if (CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            PagedList<SerialNumberRule> result = _serialNumberRuleFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                , SolutionId.Value, true);

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;            
            model.SolutionId = SolutionId.Value;
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("编辑编号规则")]
        public IActionResult Get(Guid id)
        {
            EditSerialNumberModel model = new EditSerialNumberModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _serialNumberRuleFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.SerialNumberRuleId = id;
                    return View(model);
                }
            }
            return NotFound();
        }
    }
}
