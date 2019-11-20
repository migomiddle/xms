using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Form.Abstractions;
using Xms.Form.Abstractions.Component;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Domain;
using Xms.Schema.Attribute;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Schema.StringMap;
using Xms.Solution.Abstractions;

namespace Xms.Form
{
    /// <summary>
    /// 表单服务
    /// </summary>
    public class FormService : IFormService
    {
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelBatchBuilder;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _user;
        private Guid _entityid;

        public FormService(IAppContext appContext
            , IAttributeFinder attributeFinder
            , IOptionSetFinder optionSetFinder
            , IStringMapFinder stringMapFinder
            , ILocalizedLabelService localizedLabelService
            , ILocalizedLabelBatchBuilder localizedLabelBatchBuilder)
        {
            _appContext = appContext;
            _user = appContext.GetFeature<ICurrentUser>();
            _optionSetFinder = optionSetFinder;
            _stringMapFinder = stringMapFinder;
            _attributeFinder = attributeFinder;
            _localizedLabelService = localizedLabelService;
            _localizedLabelBatchBuilder = localizedLabelBatchBuilder;
        }

        public IFormService Init(Domain.SystemForm formEntity)
        {
            Form = Form.DeserializeFromJson(formEntity.FormConfig);
            _entityid = formEntity.EntityId;
            var attributeMetadatas = _attributeFinder.FindByEntityId(_entityid);
            var labels = GetLabels();
            if (Form.Header != null && Form.Header.Rows.NotEmpty())
            {
                //header
                foreach (var row in Form.Header.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (cell.Control.ControlType == FormControlType.Standard
                            || cell.Control.ControlType == FormControlType.Hidden
                            || cell.Control.ControlType == FormControlType.Lookup
                            || cell.Control.ControlType == FormControlType.OptionSet)
                        {
                            _attributes.Add(cell.Control.Name);
                            cell.Control.AttributeMetadata = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Control.Name));
                        }
                        if (cell.Label.IsNotEmpty())
                        {
                            if (labels != null)
                            {
                                var label = labels.Find(n => n.ObjectId == cell.Id);
                                if (label != null)
                                {
                                    cell.Label = label.Label;
                                }
                            }
                        }
                        else if (cell.Control.AttributeMetadata != null)
                        {
                            cell.Label = cell.Control.AttributeMetadata.LocalizedName;
                        }
                    }
                }
            }
            if (Form.Panels.NotEmpty())
            {
                //tabs
                foreach (var tab in Form.Panels)
                {
                    if (labels != null && tab.Label.IsNotEmpty())
                    {
                        var label = labels.Find(n => n.ObjectId == tab.Id);
                        if (label != null)
                        {
                            tab.Label = label.Label;
                        }
                    }
                    foreach (var sec in tab.Sections)
                    {
                        if (labels != null && sec.Label.IsNotEmpty())
                        {
                            var label = labels.Find(n => n.ObjectId == sec.Id);
                            if (label != null)
                            {
                                sec.Label = label.Label;
                            }
                        }
                        foreach (var row in sec.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                if (cell.Control.ControlType == FormControlType.Standard
                                    || cell.Control.ControlType == FormControlType.Hidden
                                    || cell.Control.ControlType == FormControlType.Lookup
                                    || cell.Control.ControlType == FormControlType.OptionSet)
                                {
                                    _attributes.Add(cell.Control.Name);
                                    cell.Control.AttributeMetadata = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Control.Name));
                                }
                                if (cell.Label.IsNotEmpty())
                                {
                                    if (labels != null)
                                    {
                                        var label = labels.Find(n => n.ObjectId == cell.Id);
                                        if (label != null)
                                        {
                                            cell.Label = label.Label;
                                        }
                                    }
                                }
                                else if (cell.Control.AttributeMetadata != null)
                                {
                                    cell.Label = cell.Control.AttributeMetadata.LocalizedName;
                                }
                            }
                        }
                    }
                }
            }
            //if (_form.Sections.NotEmpty())
            //{
            //    //sections
            //    foreach (var sec in _form.Sections)
            //    {
            //        foreach (var row in sec.Rows)
            //        {
            //            foreach (var cell in row.Cells)
            //            {
            //                _attributes.Add(cell.Control.Name);
            //                cell.Control.AttributeMetadata = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Control.Name));
            //            }
            //        }
            //    }
            //}
            if (Form.Footer != null && Form.Footer.Rows.NotEmpty())
            {
                //footer
                foreach (var row in Form.Footer.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (cell.Control.ControlType == FormControlType.Standard
                            || cell.Control.ControlType == FormControlType.Hidden
                            || cell.Control.ControlType == FormControlType.Lookup
                            || cell.Control.ControlType == FormControlType.OptionSet)
                        {
                            _attributes.Add(cell.Control.Name);
                            cell.Control.AttributeMetadata = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Control.Name));
                        }
                        if (cell.Label.IsNotEmpty())
                        {
                            if (labels != null)
                            {
                                var label = labels.Find(n => n.ObjectId == cell.Id);
                                if (label != null)
                                {
                                    cell.Label = label.Label;
                                }
                            }
                        }
                        else if (cell.Control.AttributeMetadata != null)
                        {
                            cell.Label = cell.Control.AttributeMetadata.LocalizedName;
                        }
                    }
                }
            }
            AttributeMetaDatas = attributeMetadatas.Where(n => _attributes.Contains(n.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var attr in AttributeMetaDatas)
            {
                SetAttributeItems(attr);
            }
            return this;
        }

        private List<LocalizedLabel> GetLabels()
        {
            List<Guid> objectIds = new List<Guid>();
            if (Form.Header != null && Form.Header.Rows.NotEmpty())
            {
                //header
                foreach (var row in Form.Header.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                        {
                            objectIds.Add(cell.Id);
                        }
                    }
                }
            }
            if (Form.NavGroups.NotEmpty())
            {
                //navs
                foreach (var navg in Form.NavGroups)
                {
                    if (!navg.Id.Equals(Guid.Empty) && navg.Label.IsNotEmpty())
                    {
                        objectIds.Add(navg.Id);
                    }
                    foreach (var item in navg.NavItems)
                    {
                        if (!item.Id.Equals(Guid.Empty) && item.Label.IsNotEmpty())
                        {
                            objectIds.Add(item.Id);
                        }
                    }
                }
            }
            if (Form.Panels.NotEmpty())
            {
                //tabs
                foreach (var tab in Form.Panels)
                {
                    if (!tab.Id.Equals(Guid.Empty) && tab.Label.IsNotEmpty())
                    {
                        objectIds.Add(tab.Id);
                    }
                    foreach (var sec in tab.Sections)
                    {
                        if (!sec.Id.Equals(Guid.Empty) && sec.Label.IsNotEmpty())
                        {
                            objectIds.Add(sec.Id);
                        }
                        foreach (var row in sec.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                                {
                                    objectIds.Add(cell.Id);
                                }
                            }
                        }
                    }
                }
            }
            if (Form.Footer != null && Form.Footer.Rows.NotEmpty())
            {
                //footer
                foreach (var row in Form.Footer.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                        {
                            objectIds.Add(cell.Id);
                        }
                    }
                }
            }
            if (objectIds.NotEmpty())
            {
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this._user.UserSettings.LanguageId && f.ObjectId.In(objectIds)));
                return labels;
            }
            return null;
        }

        private void SetAttributeItems(Schema.Domain.Attribute attr)
        {
            if ((attr.TypeIsPickList() || attr.TypeIsStatus()))// && attr.OptionSet == null)
            {
                var os = _optionSetFinder.FindById(attr.OptionSetId.Value);
                attr.OptionSet = os;
            }
            else if ((attr.TypeIsBit()
                    || attr.TypeIsState()
                    ))// && attr.PickLists.IsEmpty())
            {
                attr.PickLists = _stringMapFinder.Query(n => n.Where(w => w.AttributeId == attr.AttributeId).Sort(s => s.SortAscending(f => f.DisplayOrder)));
            }
        }

        public List<Schema.Domain.Attribute> AttributeMetaDatas { get; private set; } = new List<Schema.Domain.Attribute>();

        private List<string> _attributes = new List<string>();

        public FormDescriptor Form { get; set; }

        public void DeleteOriginalLabels(Domain.SystemForm original)
        {
            var oldForm = new FormDescriptor().DeserializeFromJson(original.FormConfig);
            List<Guid> objectIds = new List<Guid>();
            if (oldForm.Header != null && oldForm.Header.Rows.NotEmpty())
            {
                //header
                foreach (var row in oldForm.Header.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                        {
                            objectIds.Add(cell.Id);
                        }
                    }
                }
            }
            if (oldForm.NavGroups.NotEmpty())
            {
                //navs
                foreach (var navg in oldForm.NavGroups)
                {
                    if (!navg.Id.Equals(Guid.Empty) && navg.Label.IsNotEmpty())
                    {
                        objectIds.Add(navg.Id);
                    }
                    foreach (var item in navg.NavItems)
                    {
                        if (!item.Id.Equals(Guid.Empty) && item.Label.IsNotEmpty())
                        {
                            objectIds.Add(item.Id);
                        }
                    }
                }
            }
            if (oldForm.Panels.NotEmpty())
            {
                //tabs
                foreach (var tab in oldForm.Panels)
                {
                    if (!tab.Id.Equals(Guid.Empty) && tab.Label.IsNotEmpty())
                    {
                        objectIds.Add(tab.Id);
                    }
                    foreach (var sec in tab.Sections)
                    {
                        if (!sec.Id.Equals(Guid.Empty) && sec.Label.IsNotEmpty())
                        {
                            objectIds.Add(sec.Id);
                        }
                        foreach (var row in sec.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                                {
                                    objectIds.Add(cell.Id);
                                }
                            }
                        }
                    }
                }
            }
            if (oldForm.Footer != null && oldForm.Footer.Rows.NotEmpty())
            {
                //footer
                foreach (var row in oldForm.Footer.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                        {
                            objectIds.Add(cell.Id);
                        }
                    }
                }
            }
            if (objectIds.NotEmpty())
            {
                _localizedLabelService.DeleteById(objectIds.ToArray());
            }
        }

        public void UpdateLocalizedLabel(Domain.SystemForm original)
        {
            //删除原有的
            if (original != null)
            {
                DeleteOriginalLabels(original);
            }
            var attributeMetadatas = _attributeFinder.FindByEntityId(_entityid);
            var solutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            if (Form.Header != null && Form.Header.Rows.NotEmpty())
            {
                //header
                foreach (var row in Form.Header.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                        {
                            if (cell.Control.ControlType == FormControlType.Standard
                                || cell.Control.ControlType == FormControlType.Hidden
                                || cell.Control.ControlType == FormControlType.Lookup
                                || cell.Control.ControlType == FormControlType.OptionSet)
                            {
                                var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Control.Name));
                                if (attr != null && !cell.Label.IsCaseInsensitiveEqual(attr.LocalizedName))//与字段显示名称不一致时
                                {
                                    _localizedLabelBatchBuilder.Append(solutionId, cell.Label, FormDefaults.ModuleName, "LayoutLabel", cell.Id);
                                }
                            }
                            else
                            {
                                _localizedLabelBatchBuilder.Append(solutionId, cell.Label, FormDefaults.ModuleName, "LayoutLabel", cell.Id);
                            }
                        }
                    }
                }
            }
            if (Form.NavGroups.NotEmpty())
            {
                //navs
                foreach (var navg in Form.NavGroups)
                {
                    if (!navg.Id.Equals(Guid.Empty) && navg.Label.IsNotEmpty())
                    {
                        _localizedLabelBatchBuilder.Append(solutionId, navg.Label, FormDefaults.ModuleName, "LayoutLabel", navg.Id);
                    }
                    foreach (var item in navg.NavItems)
                    {
                        _localizedLabelBatchBuilder.Append(solutionId, item.Label, FormDefaults.ModuleName, "LayoutLabel", item.Id);
                    }
                }
            }
            if (Form.Panels.NotEmpty())
            {
                //tabs
                foreach (var tab in Form.Panels)
                {
                    if (!tab.Id.Equals(Guid.Empty) && tab.Label.IsNotEmpty())
                    {
                        _localizedLabelBatchBuilder.Append(solutionId, tab.Label, FormDefaults.ModuleName, "LayoutLabel", tab.Id);
                    }
                    foreach (var sec in tab.Sections)
                    {
                        if (!sec.Id.Equals(Guid.Empty) && sec.Label.IsNotEmpty())
                        {
                            _localizedLabelBatchBuilder.Append(solutionId, sec.Label, FormDefaults.ModuleName, "LayoutLabel", sec.Id);
                        }
                        foreach (var row in sec.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                                {
                                    if (cell.Control.ControlType == FormControlType.Standard
                                        || cell.Control.ControlType == FormControlType.Hidden
                                        || cell.Control.ControlType == FormControlType.Lookup
                                        || cell.Control.ControlType == FormControlType.OptionSet)
                                    {
                                        var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Control.Name));
                                        if (attr != null && !cell.Label.IsCaseInsensitiveEqual(attr.LocalizedName))//与字段显示名称不一致时
                                        {
                                            _localizedLabelBatchBuilder.Append(solutionId, cell.Label, FormDefaults.ModuleName, "LayoutLabel", cell.Id);
                                        }
                                    }
                                    else
                                    {
                                        _localizedLabelBatchBuilder.Append(solutionId, cell.Label, FormDefaults.ModuleName, "LayoutLabel", cell.Id);
                                    }
                                }
                            }
                        }
                    }
                }
                if (Form.Footer != null && Form.Footer.Rows.NotEmpty())
                {
                    //footer
                    foreach (var row in Form.Footer.Rows)
                    {
                        foreach (var cell in row.Cells)
                        {
                            if (!cell.Id.Equals(Guid.Empty) && cell.Label.IsNotEmpty())
                            {
                                if (cell.Control.ControlType == FormControlType.Standard
                                    || cell.Control.ControlType == FormControlType.Hidden
                                    || cell.Control.ControlType == FormControlType.Lookup
                                    || cell.Control.ControlType == FormControlType.OptionSet)
                                {
                                    var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(cell.Control.Name));
                                    if (attr != null && !cell.Label.IsCaseInsensitiveEqual(attr.LocalizedName))//与字段显示名称不一致时
                                    {
                                        _localizedLabelBatchBuilder.Append(solutionId, cell.Label, FormDefaults.ModuleName, "LayoutLabel", cell.Id);
                                    }
                                }
                                else
                                {
                                    _localizedLabelBatchBuilder.Append(solutionId, cell.Label, FormDefaults.ModuleName, "LayoutLabel", cell.Id);
                                }
                            }
                        }
                    }
                }
            }
            _localizedLabelBatchBuilder.Save();
        }
    }

    public static class FormExtensions
    {
        /// <summary>
        /// 获取表单引用的视图
        /// </summary>
        /// <param name="systemForm"></param>
        /// <returns></returns>
        public static List<Guid> GetQueryViewIds(this Domain.SystemForm systemForm)
        {
            var form = new FormDescriptor().DeserializeFromJson(systemForm.FormConfig);
            List<Guid> result = new List<Guid>();
            if (form.Panels.NotEmpty())
            {
                //tabs
                foreach (var tab in form.Panels)
                {
                    foreach (var sec in tab.Sections)
                    {
                        foreach (var row in sec.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                if (cell.Control.ControlType == FormControlType.SubGrid)
                                {
                                    result.Add(new Guid((cell.Control.Parameters as SubGridParameters).ViewId));
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}