using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 团队控制器
    /// </summary>
    [Route("{org}/api/team")]
    public class TeamController : ApiControllerBase
    {
        private readonly IDataFinder _dataFinder;
        private readonly IDataCreater _dataCreater;
        private readonly IDataDeleter _dataDeleter;

        public TeamController(IWebAppContext appContext
            , IDataFinder dataFinder
            , IDataCreater dataCreater
            , IDataDeleter dataDeleter)
            : base(appContext)
        {
            _dataFinder = dataFinder;
            _dataCreater = dataCreater;
            _dataDeleter = dataDeleter;
        }

        [Description("添加团队成员")]
        [HttpPost("AddMembers")]
        public IActionResult AddMembers(Guid teamId, Guid[] userid)
        {
            if (userid.IsEmpty())
            {
                return JError(T["notspecified_record"]);
            }
            var query = new QueryExpression("TeamMembership", CurrentUser.UserSettings.LanguageId);
            query.ColumnSet.AddColumns("systemuserid");
            query.Criteria.AddCondition("teamid", ConditionOperator.Equal, teamId);
            var members = _dataFinder.RetrieveAll(query);
            var addEntities = new List<Entity>();
            foreach (var item in userid)
            {
                if (!members.Any(n => n.GetGuidValue("systemuserid") == item))
                {
                    Entity entity = new Entity("TeamMembership");
                    entity.SetAttributeValue("teamid", teamId);
                    entity.SetAttributeValue("systemuserid", item);
                    addEntities.Add(entity);
                }
            }
            if (addEntities.NotEmpty())
            {
                _dataCreater.CreateMany(addEntities);
            }
            return JOk(T["added_success"]);
        }

        [Description("移除团队成员")]
        [HttpPost("RemoveMembers")]
        public IActionResult RemoveMembers(Guid teamId, Guid[] userid)
        {
            if (userid.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            var query = new QueryExpression("TeamMembership", CurrentUser.UserSettings.LanguageId);
            query.ColumnSet.AddColumns("systemuserid");
            query.Criteria.AddCondition("teamid", ConditionOperator.Equal, teamId);
            query.Criteria.AddCondition("systemuserid", ConditionOperator.In, userid.Select(n => (object)n));
            var members = _dataFinder.RetrieveAll(query);
            foreach (var item in members)
            {
                _dataDeleter.Delete(item);
            }
            return DeleteSuccess();
        }
    }
}