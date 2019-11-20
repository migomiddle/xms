using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Business.FormStateRule;
using Xms.Business.SerialNumber;
using Xms.Core;
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
using Xms.Security.Principal;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 表单控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class FormController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly IRibbonButtonFinder _ribbonbuttonFinder;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly ISystemFormStatusSetter _systemFormStatusSetter;
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly IRoleObjectAccessEntityPermissionService _roleObjectAccessEntityPermissionService;
        private readonly ISystemUserPermissionService _systemUserPermissionService;
        private readonly IRibbonButtonStatusSetter _ribbonButtonStatusSetter;
        private readonly IFormService _formService;
        private readonly IDataFinder _dataFinder;

        public FormController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , ISystemFormFinder systemFormFinder
            , IRibbonButtonFinder ribbonbuttonFinder
            , IRibbonButtonStatusSetter ribbonButtonStatusSetter
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , ISystemFormStatusSetter systemFormStatusSetter
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , ISystemUserPermissionService systemUserPermissionService
            , ISerialNumberRuleFinder serialNumberRuleFinder
            , IFormService formService
            , IDataFinder dataFinder
            )
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _systemFormFinder = systemFormFinder;
            _ribbonbuttonFinder = ribbonbuttonFinder;
            _ribbonButtonStatusSetter = ribbonButtonStatusSetter;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _systemFormStatusSetter = systemFormStatusSetter;
            _roleObjectAccessEntityPermissionService = roleObjectAccessEntityPermissionService;
            _systemUserPermissionService = systemUserPermissionService;
            _serialNumberRuleFinder = serialNumberRuleFinder;
            _formService = formService;
            _dataFinder = dataFinder;
        }

        [HttpGet]
        [Description("新建记录")]
        [Route("{entityname?}")]
        public IActionResult Create(EntityFormModel args)
        {
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
                    var instances = _workFlowInstanceService.Top(n => n.Take(1).Where(f => f.EntityId == m.EntityId.Value && f.ObjectId == m.RecordId.Value).Sort(s => s.SortDescending(f => f.CreatedOn)));
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
                return PromptView(T["notfound_defaultform"]);
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
                    ViewData["NonePermissionFields"] = obj.SerializeToJson();
                }
            }
            else
            {
                ViewData["NonePermissionFields"] = "[]";
            }
            var _form = formBuilder.Form;
            m.Form = _form;
            ViewData["form"] = _formService.Form.SerializeToJson(false);
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
            ViewData["record"] = m.Entity.SerializeToJson();
            m.StageId = args.StageId;
            m.BusinessFlowId = args.BusinessFlowId;
            m.BusinessFlowInstanceId = args.BusinessFlowInstanceId;

            return View($"~/Views/Entity/Create.cshtml", m);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Description("新建记录")]
        //public IActionResult Create(EditRecordModel _recordModel, string child = null)
        //{
        //    Guid entityid = _recordModel.EntityId.Value;
        //    Guid formid = _recordModel.FormId.Value;
        //    Guid? recordid = _recordModel.RecordId;
        //    bool isNew = !(recordid.HasValue && !recordid.Value.Equals(Guid.Empty));
        //    if (entityid.Equals(Guid.Empty))
        //    {
        //        return NotFound();
        //    }
        //    var entityMetaData = _entityFinder.FindById(entityid);
        //    if (entityMetaData == null)
        //    {
        //        return NotFound();
        //    }
        //    _recordModel.EntityMetaData = entityMetaData;
        //    _recordModel.EntityId = entityid;
        //    var formEntity = DomainCreator.Get<SystemForm>();
        //    if (!formid.Equals(Guid.Empty))
        //    {
        //        formEntity = _systemFormFinder.FindById(formid);
        //    }
        //    else
        //    {
        //        //获取实体默认表单
        //        formEntity = _systemFormFinder.FindEntityDefaultForm(entityid);
        //    }
        //    if (formEntity == null)
        //    {
        //        return NotFound();
        //    }
        //    _recordModel.FormInfo = formEntity;
        //    //FormBuilder formBuilder = new FormBuilder(formEntity);
        //    _formService.Init(formEntity);
        //    //内容已更改的字段信息
        //    List<string> attributeChangedList = null;
        //    if (!isNew && _recordModel.AttributeChanged.IsNotEmpty())
        //    {
        //        attributeChangedList = new List<string>();
        //        attributeChangedList.AddRange(_recordModel.AttributeChanged.Split(','));
        //    }
        //    var headHasChanged = attributeChangedList.NotEmpty();
        //    Core.Data.Entity entity = new Core.Data.Entity(entityMetaData.Name);
        //    if (isNew || (!isNew && headHasChanged))
        //    {
        //        foreach (var attr in _formService.AttributeMetaDatas)
        //        {
        //            var k = attr.Name;
        //            if (headHasChanged && !attributeChangedList.Exists(n => n.IsCaseInsensitiveEqual(k)))
        //            {
        //                continue;
        //            }
        //            object v = Request.Form[k];
        //            if (v != null)
        //            {
        //                v = entity.WrapAttributeValue(_entityFinder, attr, v);
        //                entity[k] = v;
        //            }
        //        }
        //    }
        //    //包含单据体时，启用事务
        //    if (child.IsNotEmpty())
        //    {
        //        //_organizationServiceProxy.BeginTransaction();
        //    }
        //    var thisId = Guid.Empty;
        //    try
        //    {
        //        if (isNew)
        //        {
        //            if (_recordModel.RelationShipName.IsNotEmpty() && _recordModel.ReferencedRecordId.HasValue)//如果存在关联关系
        //            {
        //                var relationShipMetas = _relationShipFinder.FindByName(_recordModel.RelationShipName);
        //                if (null != relationShipMetas && relationShipMetas.ReferencingEntityId == _recordModel.EntityId && entity.GetStringValue(relationShipMetas.ReferencingAttributeName).IsEmpty())
        //                {
        //                    //设置当前记录关联字段的值
        //                    entity.SetAttributeValue(relationShipMetas.ReferencingAttributeName, new EntityReference(relationShipMetas.ReferencedEntityName, _recordModel.ReferencedRecordId.Value));
        //                }
        //            }
        //            if (!_recordModel.StageId.Equals(Guid.Empty))//业务流程的阶段
        //            {
        //                entity.SetAttributeValue("StageId", _recordModel.StageId);
        //            }
        //            thisId = _dataCreater.Create(entity);
        //            if (!_recordModel.StageId.Equals(Guid.Empty))//业务流程的阶段
        //            {
        //                _businessProcessFlowInstanceUpdater.UpdateForward(_recordModel.BusinessFlowId, _recordModel.BusinessFlowInstanceId, _recordModel.StageId, thisId);
        //            }
        //        }
        //        else
        //        {
        //            thisId = recordid.Value;
        //            entity.SetIdValue(recordid.Value);
        //            if (headHasChanged)
        //            {
        //                _dataUpdater.Update(entity);
        //            }
        //        }
        //        //单据体
        //        if (child.IsNotEmpty())
        //        {
        //            var childs = JArray.Parse(child.UrlDecode());
        //            if (childs.Count > 0)
        //            {
        //                List<Core.Data.Entity> childEntities = new List<Core.Data.Entity>();
        //                List<string> entityNames = new List<string>();
        //                foreach (var c in childs)
        //                {
        //                    dynamic root = JObject.Parse(c.ToString());
        //                    string name = root.name, relationshipname = root.relationshipname, refname = string.Empty;
        //                    if (!entityNames.Exists(n => n.IsCaseInsensitiveEqual(name)))
        //                    {
        //                        entityNames.Add(name);
        //                    }

        //                    var data = root.data;
        //                    var childAttributes = _attributeFinder.FindByEntityName(name);
        //                    if (relationshipname.IsNotEmpty())
        //                    {
        //                        var relationShipMetas = _relationShipFinder.FindByName(relationshipname);
        //                        if (null != relationShipMetas && relationShipMetas.ReferencedEntityId == _recordModel.EntityId)
        //                        {
        //                            refname = relationShipMetas.ReferencingAttributeName;
        //                        }
        //                    }
        //                    Core.Data.Entity detail = new Core.Data.Entity(name);
        //                    foreach (JProperty p in data)
        //                    {
        //                        var attr = childAttributes.Find(n => n.Name.IsCaseInsensitiveEqual(p.Name));
        //                        if (attr != null && p.Value != null)
        //                        {
        //                            detail.SetAttributeValue(p.Name.ToString().ToLower(), detail.WrapAttributeValue(_entityFinder, attr, p.Value.ToString()));
        //                        }
        //                    }
        //                    //关联主记录ID
        //                    if (refname.IsNotEmpty())
        //                    {
        //                        detail.SetAttributeValue(refname, new EntityReference(_recordModel.EntityMetaData.Name, thisId));
        //                    }
        //                    childEntities.Add(detail);
        //                }
        //                //批量创建记录
        //                if (childEntities.NotEmpty())
        //                {
        //                    foreach (var item in entityNames)
        //                    {
        //                        var items = childEntities.Where(n => n.Name.IsCaseInsensitiveEqual(item)).ToList();
        //                        var creatingRecords = items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && n.GetIdValue().Equals(Guid.Empty)).ToList();
        //                        if (creatingRecords.NotEmpty())
        //                        {
        //                            _dataCreater.CreateMany(creatingRecords);
        //                        }
        //                        if (!isNew)
        //                        {
        //                            foreach (var updItem in items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && !n.GetIdValue().Equals(Guid.Empty)))
        //                            {
        //                                _dataUpdater.Update(updItem);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //_organizationServiceProxy.CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        //_organizationServiceProxy.RollBackTransaction();
        //        return JsonError(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //    }
        //    if (isNew)
        //    {
        //        return CreateSuccess(new { id = thisId });
        //    }
        //    return UpdateSuccess(new { id = thisId });
        //}

        [HttpGet]
        [Description("更新记录")]
        [Route("{entityname?}")]
        public IActionResult Edit(Guid entityid, Guid recordid, Guid? formid, Guid? copyid)
        {
            return Create(new EntityFormModel { EntityId = entityid, RecordId = recordid, FormId = formid, CopyId = copyid });
            //return RedirectToAction("create", new { entityid = entityid, recordid = recordid, formid = formid });
        }
    }
}