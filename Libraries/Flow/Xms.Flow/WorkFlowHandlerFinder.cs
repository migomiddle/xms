using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;

namespace Xms.Flow
{
    /// <summary>
    /// 流程处理者查找服务
    /// </summary>
    public class WorkFlowHandlerFinder : IWorkFlowHandlerFinder
    {
        private readonly IDataFinder _dataFinder;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public WorkFlowHandlerFinder(IAppContext appContext
            , IDataFinder dataFinder)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _dataFinder = dataFinder;
        }

        /// <summary>
        /// 获取当前处理者列表
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="handlerIdType"></param>
        /// <param name="handlers"></param>
        /// <param name="stepOrder"></param>
        /// <returns></returns>
        public List<Guid> GetCurrentHandlerId(WorkFlowInstance instance, WorkFlowProcess prevStep, int handlerIdType, string handlers)
        {
            var result = new List<Guid>();
            try
            {
                var applicantUser = _dataFinder.RetrieveById("systemuser", instance.ApplicantId, ignorePermissions: true);
                switch (handlerIdType)
                {
                    case 1://所有成员

                        break;

                    case 2://指定成员
                        if (handlers.IsNotEmpty())
                        {
                            List<WorkFlowStepHandler> handlerObjs = new List<WorkFlowStepHandler>().DeserializeFromJson(handlers);
                            foreach (var item in handlerObjs)
                            {
                                if (item.Type == WorkFlowStepHandlerType.SystemUser)
                                {
                                    result.Add(item.Id);
                                }
                                else if (item.Type == WorkFlowStepHandlerType.Team)
                                {
                                    var queryTeam = new QueryExpression("TeamMembership", _currentUser.UserSettings.LanguageId);
                                    queryTeam.ColumnSet.AddColumn("SystemUserId");
                                    queryTeam.Criteria.AddCondition("TeamId", ConditionOperator.Equal, item.Id);
                                    var teamMembers = _dataFinder.RetrieveAll(queryTeam, true);
                                    if (teamMembers.NotEmpty())
                                    {
                                        result.AddRange(teamMembers.Select(n => n.GetGuidValue("SystemUserId")));
                                    }
                                }
                                else if (item.Type == WorkFlowStepHandlerType.Roles)
                                {
                                    QueryExpression query = new QueryExpression("SystemUserRoles");
                                    query.AddColumns("SystemUserId");
                                    query.Criteria.AddCondition("RoleId", ConditionOperator.Equal, item.Id);
                                    var userRolesData = _dataFinder.RetrieveAll(query, true);
                                    if (userRolesData.NotEmpty())
                                    {
                                        result.AddRange(userRolesData.Select(n => n.GetGuidValue("SystemUserId")));
                                    }
                                }
                                else if (item.Type == WorkFlowStepHandlerType.Post)
                                {
                                    var queryTeam = new QueryExpression("SystemUser", _currentUser.UserSettings.LanguageId);
                                    queryTeam.ColumnSet.AddColumn("SystemUserId");
                                    queryTeam.Criteria.AddCondition("PostId", ConditionOperator.Equal, item.Id);
                                    queryTeam.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);
                                    var teamMembers = _dataFinder.RetrieveAll(queryTeam, true);
                                    if (teamMembers.NotEmpty())
                                    {
                                        result.AddRange(teamMembers.Select(n => n.GetGuidValue("SystemUserId")));
                                    }
                                }
                            }
                        }
                        break;

                    case 3://发起人领导
                        var parentUserId = applicantUser.GetGuidValue("parentsystemuserid");
                        if (!parentUserId.Equals(Guid.Empty))
                        {
                            result.Add(parentUserId);
                        }
                        break;

                    case 4://发起人部门负责人
                        var bmanager = _dataFinder.RetrieveById("businessunit", applicantUser.GetGuidValue("businessunitid"));
                        var bmanagerId = bmanager.GetGuidValue("managerid");
                        if (!bmanagerId.Equals(Guid.Empty))
                        {
                            result.Add(bmanagerId);
                        }
                        break;

                    case 5://发起人公司负责人
                        var orgmanager = _dataFinder.RetrieveById("organization", applicantUser.GetGuidValue("organizationid"), ignorePermissions: true);
                        var orgmanagerId = orgmanager.GetGuidValue("managerid");
                        if (!orgmanagerId.Equals(Guid.Empty))
                        {
                            result.Add(orgmanagerId);
                        }
                        break;

                    case 6://上一环节审核人领导
                           //var prevStepOrder = stepOrder - 1;
                           //var prevStep = new WorkFlowStepService(this.User).Find(n => n.WorkFlowId == instance.WorkFlowId && n.StepOrder == prevStepOrder);
                        if (prevStep != null)
                        {
                            var prevHandlers = GetCurrentHandlerId(instance, null, prevStep.HandlerIdType, prevStep.Handlers);
                            foreach (var item in prevHandlers)
                            {
                                var prevHandledUsers = _dataFinder.RetrieveById("systemuser", item, ignorePermissions: true);
                                var parentUserId2 = prevHandledUsers.GetGuidValue("parentsystemuserid");
                                if (!parentUserId2.Equals(Guid.Empty))
                                {
                                    result.Add(parentUserId2);
                                }
                            }
                        }
                        break;

                    case 7://发起人
                        result.Add(instance.ApplicantId);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new XmsException(ex);
            }
            return result.Distinct().ToList();
        }
    }
}