using System;
using System.Collections.Generic;
using Xms.Business.SerialNumber.Domain;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则导入服务
    /// </summary>
    [SolutionImportNode("serialnumberrules")]
    public class SerialNumberRuleImporter : ISolutionComponentImporter<SerialNumberRule>
    {
        private readonly ISerialNumberRuleCreater _serialNumberRuleCreater;
        private readonly ISerialNumberRuleUpdater _serialNumberRuleUpdater;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly IAppContext _appContext;

        public SerialNumberRuleImporter(IAppContext appContext
            , ISerialNumberRuleCreater serialNumberRuleCreater
            , ISerialNumberRuleUpdater serialNumberRuleUpdater
            , ISerialNumberRuleFinder serialNumberRuleFinder)
        {
            _appContext = appContext;
            _serialNumberRuleCreater = serialNumberRuleCreater;
            _serialNumberRuleUpdater = serialNumberRuleUpdater;
            _serialNumberRuleFinder = serialNumberRuleFinder;
        }

        public bool Import(Guid solutionId, IList<SerialNumberRule> serialNumberRules)
        {
            if (serialNumberRules.NotEmpty())
            {
                foreach (var item in serialNumberRules)
                {
                    var entity = _serialNumberRuleFinder.FindById(item.SerialNumberRuleId);
                    if (entity != null)
                    {
                        entity.DateFormatType = item.DateFormatType;
                        entity.Description = item.Description;
                        entity.Increment = item.Increment;
                        entity.IncrementLength = item.IncrementLength;
                        entity.Name = item.Name;
                        entity.Prefix = item.Prefix;
                        entity.Seprator = item.Seprator;
                        entity.StateCode = item.StateCode;
                        _serialNumberRuleUpdater.Update(entity);
                    }
                    else
                    {
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedOn = DateTime.Now;
                        item.OrganizationId = _appContext.OrganizationId;
                        _serialNumberRuleCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}