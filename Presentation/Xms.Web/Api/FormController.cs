using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Business.FormStateRule;
using Xms.Business.SerialNumber;
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Flow;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;
using Xms.Form;
using Xms.Form.Abstractions;
using Xms.Form.Abstractions.Component;
using Xms.Form.Domain;
using Xms.Infrastructure.Utility;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Security.Abstractions;
using Xms.Security.Principal;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Web.Api
{
    /// <summary>
    /// 表单元数据接口
    /// </summary>
    [Route("{org}/api/schema/form")]
    public class FormController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormUpdater _systemFormUpdater;
        private readonly IRibbonButtonFinder _ribbonbuttonFinder;
        private readonly IDataFinder _dataFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly ISystemUserPermissionService _systemUserPermissionService;
        private readonly IFormService _formService;
        private readonly ISystemFormStatusSetter _systemFormStatusSetter;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IRibbonButtonStatusSetter _ribbonButtonStatusSetter;
        private readonly IRoleObjectAccessEntityPermissionService _roleObjectAccessEntityPermissionService;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;

        public FormController(IWebAppContext appContext
            , ISystemFormFinder systemFormFinder
            , ISystemFormUpdater systemFormUpdater
            , IEntityFinder entityService
            , IRibbonButtonFinder ribbonbuttonFinder
            , IDataFinder dataFinder
            , IAttributeFinder attributeFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , ISystemUserPermissionService systemUserPermissionService
            , IFormService formService
            , ISystemFormStatusSetter systemFormStatusSetter
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IRibbonButtonStatusSetter ribbonButtonStatusSetter
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , ISerialNumberRuleFinder serialNumberRuleFinder)
            : base(appContext)
        {
            _systemFormFinder = systemFormFinder;
            _systemFormUpdater = systemFormUpdater;
            _entityFinder = entityService;
            _ribbonbuttonFinder = ribbonbuttonFinder;
            _dataFinder = dataFinder;
            _attributeFinder = attributeFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _systemUserPermissionService = systemUserPermissionService;
            _formService = formService;
            _systemFormStatusSetter = systemFormStatusSetter;
            _workFlowProcessFinder = workFlowProcessFinder;
            _ribbonButtonStatusSetter = ribbonButtonStatusSetter;
            _roleObjectAccessEntityPermissionService = roleObjectAccessEntityPermissionService;
            _serialNumberRuleFinder = serialNumberRuleFinder;
        }

        /// <summary>
        /// 查询表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("查询表单")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _systemFormFinder.FindById(id);
            if (result == null)
            {
                return NotFound();
            }
            return JOk(result);
        }

        /// <summary>
        /// 查询表单
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="authorized"></param>
        /// <returns></returns>
        [Description("查询表单")]
        [HttpGet("GetByEntityId")]
        public IActionResult GetByEntityId(Guid entityId, bool? authorized)
        {
            List<Form.Domain.SystemForm> result = null;
            if (authorized.HasValue && authorized.Value)
            {
                result = _systemFormFinder.QueryAuthorized(n => n.Where(f => f.EntityId == entityId && f.StateCode == Core.RecordState.Enabled)
                    .Sort(s => s.SortAscending(f => f.Name)), FormType.Main);
            }
            else
            {
                result = _systemFormFinder.FindByEntityId(entityId);
            }
            return JOk(result);
        }

        /// <summary>
        /// 查询表单
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="authorized"></param>
        /// <returns></returns>
        [Description("查询表单")]
        [HttpGet("GetByEntityName")]
        public IActionResult GetByEntityName(string entityName, bool? authorized)
        {
            List<Form.Domain.SystemForm> result = null;
            if (authorized.HasValue && authorized.Value)
            {
                result = _systemFormFinder.QueryAuthorized(n => n.Where(f => f.EntityName == entityName && f.StateCode == Core.RecordState.Enabled)
                    .Sort(s => s.SortAscending(f => f.Name)), FormType.Main);
            }
            else
            {
                result = _systemFormFinder.FindByEntityName(entityName);
            }
            return JOk(result);
        }

        /// <summary>
        /// 查询表单按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("查询表单按钮")]
        [HttpGet("GetButtons/{id}")]
        public IActionResult GetButtons(Guid id)
        {
            var form = _systemFormFinder.FindById(id);
            if (form == null)
            {
                return NotFound();
            }
            var buttons = _ribbonbuttonFinder.Find(form.EntityId, RibbonButtonArea.Form);
            if (form.IsCustomButton && form.CustomButtons.IsNotEmpty())
            {
                List<Guid> buttonid = new List<Guid>();
                buttonid = buttonid.DeserializeFromJson(form.CustomButtons);
                buttons.RemoveAll(x => !buttonid.Contains(x.RibbonButtonId));
            }
            if (buttons.NotEmpty())
            {
                buttons = buttons.OrderBy(x => x.DisplayOrder).ToList();
                //_ribbonButtonStatusSetter.Set(buttons);
            }
            return JOk(buttons);
        }

        /// <summary>
        /// 解决方案组件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _systemFormFinder.QueryPaged(x => x.Where(f => f.FormType == (int)FormType.Main).Page(model.Page, model.PageSize), model.SolutionId, model.InSolution, FormType.Main);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.SystemFormId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = FormDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }

        /// <summary>
        /// 查询表单权限资源
        /// </summary>
        /// <returns></returns>
        [Description("查询表单权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource(bool? authorizationEnabled)
        {
            FilterContainer<Form.Domain.SystemForm> filter = FilterContainerBuilder.Build<Form.Domain.SystemForm>();
            filter.And(x => x.StateCode == Core.RecordState.Enabled && x.FormType == (int)FormType.Main && x.IsDefault == false);
            if (authorizationEnabled.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == authorizationEnabled.Value);
            }
            var data = _systemFormFinder.Query(x => x.Select(s => new { s.SystemFormId, s.Name, s.EntityId, s.AuthorizationEnabled })
            .Where(filter));
            if (data.NotEmpty())
            {
                var result = new List<PrivilegeResourceItem>();
                var entities = _entityFinder.FindAll()?.OrderBy(x => x.LocalizedName).ToList();
                foreach (var item in entities)
                {
                    var forms = data.Where(x => x.EntityId == item.EntityId);
                    if (!forms.Any())
                    {
                        continue;
                    }
                    var group1 = new PrivilegeResourceItem
                    {
                        Label = item.LocalizedName,
                        Children = forms.Select(x => (new PrivilegeResourceItem { Id = x.SystemFormId, Label = x.Name, AuthorizationEnabled = x.AuthorizationEnabled })).ToList()
                    };
                    result.Add(group1);
                }
                return JOk(result);
            }
            return JOk();
        }

        [Description("启用表单权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizations = _systemFormFinder.Query(x => x.Where(w => w.AuthorizationEnabled == true && w.FormType == (int)FormType.Main));
            if (authorizations.NotEmpty())
            {
                _systemFormUpdater.UpdateAuthorization(false, authorizations.Select(x => x.SystemFormId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _systemFormUpdater.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }

        [Description("查询表单完整信息")]
        [HttpGet("{entityname?}")]
        public IActionResult GetCompleteInfo(EntityFormModel args)
        {
            string nonePermissionFields = null, form = null, records = null;
            if (args.EntityId.Equals(Guid.Empty) && args.EntityName.IsEmpty())
            {
                return NotFound();
            }
            var entity = args.EntityId.Equals(Guid.Empty) ? _entityFinder.FindByName(args.EntityName) : _entityFinder.FindById(args.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            args.EntityId = entity.EntityId;
            args.EntityName = entity.Name;
            EditRecordModel m = new EditRecordModel
            {
                EntityMetaData = entity,
                EntityId = args.EntityId,
                RelationShipName = args.RelationShipName,
                ReferencedRecordId = args.ReferencedRecordId
            };

            if (args.RecordId.HasValue && !args.RecordId.Value.Equals(Guid.Empty))
            {
                var record = _dataFinder.RetrieveById(entity.Name, args.RecordId.Value);
                if (record == null || record.Count == 0)
                {
                    return NotFound();
                }
                var fileAttributes = _attributeFinder.FindByEntityId(entity.EntityId).Where(n => n.DataFormat.IsCaseInsensitiveEqual("fileupload"));
                foreach (var item in fileAttributes)
                {
                    if (record.GetStringValue(item.Name).IsNotEmpty())
                    {
                        record[item.Name] = string.Empty;
                    }
                    else
                    {
                        record.Remove(item.Name);
                    }
                }
                m.Entity = record;
                m.RecordId = args.RecordId;
                m.FormState = FormState.Update;
                if (m.Entity.GetIntValue("statecode", -1) == 0)
                {
                    m.FormState = FormState.Disabled;
                    //model.ReadOnly = true;
                }
            }
            else if (args.CopyId.HasValue && !args.CopyId.Value.Equals(Guid.Empty))
            {
                var record = _dataFinder.RetrieveById(entity.Name, args.CopyId.Value);
                if (record == null || record.Count == 0)
                {
                    return NotFound();
                }
                var fileAttributes = _attributeFinder.FindByEntityId(entity.EntityId).Where(n => n.DataFormat.IsCaseInsensitiveEqual("fileupload"));
                foreach (var item in fileAttributes)
                {
                    record.Remove(item.Name);
                }
                record.RemoveKeys(AttributeDefaults.SystemAttributes);
                m.Entity = record;
                //m.RecordId = model.RecordId;
                m.FormState = FormState.Create;
            }
            else
            {
                //ViewData["record"] = "{}";
                m.FormState = FormState.Create;
            }
            m.ReadOnly = args.ReadOnly;
            var isCreate = !args.RecordId.HasValue || args.RecordId.Value.Equals(Guid.Empty);
            SystemForm formEntity = null;
            //workflow
            if (!isCreate && m.EntityMetaData.WorkFlowEnabled && m.Entity.GetGuidValue("workflowid").Equals(Guid.Empty))
            {
                var processState = m.Entity.GetIntValue("processstate", -1);
                if (processState == (int)WorkFlowProcessState.Processing)// || processState == (int)WorkFlowProcessState.Passed)
                {
                    m.ReadOnly = true;
                    m.FormState = FormState.ReadOnly;
                    var instances = _workFlowInstanceService.Query(n => n.Take(1).Where(f => f.EntityId == m.EntityId.Value && f.ObjectId == m.RecordId.Value).Sort(s => s.SortDescending(f => f.CreatedOn)));
                    WorkFlowInstance instance = null;
                    if (instances.NotEmpty())
                    {
                        instance = instances.First();
                    }
                    if (instance != null)
                    {
                        var processInfo = _workFlowProcessFinder.GetCurrentStep(instance.WorkFlowInstanceId, CurrentUser.SystemUserId);
                        if (processInfo != null)
                        {
                            if (!processInfo.FormId.Equals(Guid.Empty))
                            {
                                formEntity = _systemFormFinder.FindById(processInfo.FormId);
                            }
                        }
                    }
                }
                m.WorkFlowProcessState = processState;
            }
            if (formEntity == null)
            {
                if (args.FormId.HasValue && !args.FormId.Value.Equals(Guid.Empty))
                {
                    formEntity = _systemFormFinder.FindById(args.FormId.Value);
                    if (formEntity.StateCode != RecordState.Enabled)
                    {
                        formEntity = null;
                    }
                }
                else
                {
                    //获取实体默认表单
                    formEntity = _systemFormFinder.FindEntityDefaultForm(args.EntityId);
                }
            }
            if (formEntity == null)
            {
                return JError(T["notfound_defaultform"]);
            }
            m.FormInfo = formEntity;
            m.FormId = formEntity.SystemFormId;
            FormBuilder formBuilder = new FormBuilder(formEntity.FormConfig);
            _formService.Init(formEntity);
            //表单可用状态
            if (!isCreate && m.FormState != FormState.Disabled && formBuilder.Form.FormRules.NotEmpty())
            {
                if (_systemFormStatusSetter.IsDisabled(formBuilder.Form.FormRules, m.Entity))
                {
                    m.FormState = FormState.Disabled;
                }
            }
            //获取所有字段信息
            m.AttributeList = _formService.AttributeMetaDatas;
            //获取字段权限
            if (!CurrentUser.IsSuperAdmin && m.AttributeList.Count(n => n.AuthorizationEnabled) > 0)
            {
                var securityFields = m.AttributeList.Where(n => n.AuthorizationEnabled).Select(f => f.AttributeId)?.ToList();
                if (securityFields.NotEmpty())
                {
                    //无权限的字段
                    var noneRead = _systemUserPermissionService.GetNoneReadFields(CurrentUser.SystemUserId, securityFields);
                    var noneEdit = _systemUserPermissionService.GetNoneEditFields(CurrentUser.SystemUserId, securityFields);
                    //移除无读取权限的字段内容
                    if (m.Entity.NotEmpty())
                    {
                        foreach (var item in noneRead)
                        {
                            m.Entity.Remove(m.AttributeList.Find(n => n.AttributeId == item).Name);
                        }
                    }
                    var obj = new { noneread = noneRead, noneedit = noneEdit };
                    nonePermissionFields = obj.SerializeToJson();
                }
            }
            else
            {
                nonePermissionFields = "[]";
            }
            var _form = formBuilder.Form;
            m.Form = _form;
            form = _formService.Form.SerializeToJson(false);
            //buttons
            var buttons = _ribbonbuttonFinder.Find(m.EntityId.Value, RibbonButtonArea.Form);
            if (formEntity.IsCustomButton && formEntity.CustomButtons.IsNotEmpty())
            {
                List<Guid> buttonid = new List<Guid>();
                buttonid = buttonid.DeserializeFromJson(formEntity.CustomButtons);
                buttons.RemoveAll(x => !buttonid.Contains(x.RibbonButtonId));
            }
            if (buttons.NotEmpty())
            {
                buttons = buttons.OrderBy(x => x.DisplayOrder).ToList();
                m.RibbonButtons = buttons;
                _ribbonButtonStatusSetter.Set(m.RibbonButtons, m.FormState, m.Entity);
            }
            if (isCreate)
            {
                var rep = _roleObjectAccessEntityPermissionService.FindUserPermission(m.EntityMetaData.Name, CurrentUser.LoginName, AccessRightValue.Create);
                m.HasBasePermission = rep != null && rep.AccessRightsMask != EntityPermissionDepth.None;
            }
            else
            {
                var rep = _roleObjectAccessEntityPermissionService.FindUserPermission(m.EntityMetaData.Name, CurrentUser.LoginName, AccessRightValue.Update);
                m.HasBasePermission = rep != null && rep.AccessRightsMask != EntityPermissionDepth.None;
            }
            m.SnRule = _serialNumberRuleFinder.FindByEntityId(args.EntityId);
            if (m.SnRule != null && m.Entity.NotEmpty() && args.CopyId.HasValue)
            {
                m.Entity.SetAttributeValue(m.SnRule.AttributeName, null);
            }
            records = m.Entity.SerializeToJson();
            m.StageId = args.StageId;
            m.BusinessFlowId = args.BusinessFlowId;
            m.BusinessFlowInstanceId = args.BusinessFlowInstanceId;

            return JOk(new { EditRecord = m, NonePermissionFields = nonePermissionFields, Form = form, Record = records });
        }
    }
}