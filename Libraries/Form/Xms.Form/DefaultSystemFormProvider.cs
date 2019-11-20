using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Core;
using Xms.Form.Abstractions;
using Xms.Form.Abstractions.Component;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Schema.Attribute;

namespace Xms.Form
{
    /// <summary>
    /// 默认表单提供者
    /// </summary>
    public class DefaultSystemFormProvider : IDefaultSystemFormProvider
    {
        private readonly IDefaultAttributeProvider _defaultAttributeProvider;
        private readonly IAppContext _appContext;
        private readonly ILocalizedTextProvider _loc;

        public DefaultSystemFormProvider(IAppContext appContext
            , IDefaultAttributeProvider defaultAttributeProvider)
        {
            _defaultAttributeProvider = defaultAttributeProvider;
            _appContext = appContext;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
        }

        /// <summary>
        /// 生成默认表单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (Domain.SystemForm DefaultForm, List<Dependency.Domain.Dependency> Dependents) Get(Schema.Domain.Entity entity)
        {
            Domain.SystemForm form = new Domain.SystemForm
            {
                CanBeDeleted = false,
                CreatedBy = entity.CreatedBy,
                CreatedOn = DateTime.Now,
                Description = _loc["form_default"],
                EntityId = entity.EntityId,
                FormType = (int)FormType.Main,
                Name = entity.LocalizedName,
                StateCode = RecordState.Enabled,
                SystemFormId = Guid.NewGuid(),
                IsDefault = true
            };

            var nameAttribute = _defaultAttributeProvider.GetSysAttributes(entity).Find(x => x.Name.IsCaseInsensitiveEqual("name"));
            FormDescriptor formObj = new FormDescriptor();
            formObj.Name = entity.LocalizedName;
            formObj.IsShowNav = false;
            formObj.Panels = new List<PanelDescriptor>
            {
                new PanelDescriptor()
                {
                    Name = _loc["form_information"]
                ,
                    Label = _loc["form_information"]
                ,
                    IsExpanded = true
                ,
                    IsShowLabel = true
                ,
                    IsVisible = true
                ,
                    Sections = new List<SectionDescriptor>()
                {
                    new SectionDescriptor()
                    {
                        IsShowLabel = false
                        ,IsVisible = true
                        ,Label = _loc["form_information"]
                        ,Columns = 2
                        ,Rows = new List<RowDescriptor>()
                        {
                            new RowDescriptor()
                            {
                                IsVisible = true
                                ,Cells = new List<CellDescriptor>()
                                {
                                    new CellDescriptor() {IsShowLabel = true, IsVisible = true, Label = nameAttribute.LocalizedName, Control = new ControlDescriptor() {EntityName = entity.Name, Name = "name", ControlType = FormControlType.Standard } }
                                    ,new CellDescriptor() {Control = new ControlDescriptor() { ControlType = FormControlType.None } }
                                }
                            }
                        }
                    }
                }
                }
            };
            form.FormConfig = formObj.SerializeToJson(false);
            var dp = new Dependency.Domain.Dependency();
            //dp.DependentComponentType = DependencyComponentTypes.Get(FormDefaults.ModuleName);
            dp.DependentObjectId = form.SystemFormId;
            //dp.RequiredComponentType = DependencyComponentTypes.Get(AttributeDefaults.ModuleName);
            dp.RequiredObjectId = nameAttribute.AttributeId;
            var dependents = new List<Dependency.Domain.Dependency>() { dp };
            return (form, dependents);
        }
    }
}