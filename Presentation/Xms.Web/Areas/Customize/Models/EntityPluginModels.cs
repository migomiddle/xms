using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xms.Form.Domain;
using Xms.Plugin.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class EntityPluginModel : BasePaged<EntityPlugin>
    {
        public Guid EntityId { get; set; }
        public string AssemblyName { get; set; }

        public string ClassName { get; set; }

        public string EventName { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditEntityPluginModel
    {
        public Guid? EntityPluginId { get; set; }
        public Guid EntityId { get; set; }
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string EventName { get; set; }
        public int ProcessOrder { get; set; }
        public int TypeCode { get; set; }
        public Guid SolutionId { get; set; }

        public IFormFile PluginFile { get; set; }
    }

    public class EditEntityPluginListModel
    {
        public List<EntityPlugin> EntityPlugins { get; set; }
        public List<Guid> DeleteEntityPluginIds { get; set; }
        //public IFormFile PluginFile { get; set; }
    }

    public class EditPluginListModel
    {
        public Guid SolutionId { get; set; }
        public List<EntityPlugin> EntityPlugins { get; set; }
        public List<PluginAnalysis> PluginAnalysis { get; set; }
        public Dictionary<Guid, List<QueryView.Domain.QueryView>> QueryViews { get; set; }
        public Dictionary<Guid, List<SystemForm>> SystemForms { get; set; }
    }

    public class BeforehandLoadPluginModel
    {
        public IFormFile PluginFile { get; set; }
    }

    public class WizardEntityPluginModel
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
    }
}