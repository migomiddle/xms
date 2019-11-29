using System;
using Xms.Form.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class EditDashBoardModel
    {
        public string Name { get; set; }
        public Guid EntityId { get; set; }
        public Guid SolutionId { get; set; }
        public string Description { get; set; }
        public string FormConfig { get; set; }
        public bool IsAuthorization { get; set; }
        public Guid SystemFormId { get; set; }
    }

    public class SetDashBoardStateModel
    {
        public Guid[] RecordId { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class DashBoardModel : BasePaged<SystemForm>
    {
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }
}