using System;
using System.Collections.Generic;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class OptionSetModel : BasePaged<Schema.Domain.OptionSet>
    {
        public Guid OptionSetId { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }

        public List<Schema.Domain.OptionSetDetail> Details { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditOptionSetModel
    {
        public Guid OptionSetId { get; set; }
        public string Name { get; set; }
        public List<string> OptionSetName { set; get; }
        public List<int> OptionSetValue { set; get; }
        public List<bool> IsSelectedOption { set; get; }
        public List<Guid> DetailId { get; set; }
        public List<Schema.Domain.OptionSetDetail> Details { get; set; }
        public Guid SolutionId { get; set; }
    }
}