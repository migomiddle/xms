using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xms.Form.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Form.Api.Models
{
    public class FormModel : BasePaged<SystemForm>
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public string FormConfig { get; set; }

        public bool CanBeDeleted { get; set; }
        public bool IsCustomizable { get; set; }
        public int StateCode { get; set; }
        public int FormType { get; set; }
        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }
    }

    public class EditFormModel
    {
        public Guid SystemFormId { get; set; }
        public Guid EntityId { get; set; }
        [Required]
        public string Name { get; set; }
        public string FormConfig { get; set; }
        public string CustomButtons { get; set; }
        public bool IsCustomButton { get; set; }

        //public RecordState StateCode { get; set; }
        public Guid SolutionId { get; set; }
        public bool IsAuthorization { get; set; }
        public List<Guid> AssignRoleId { get; set; }
    }

    public class SetFormDefaultModel
    {
        public List<Guid> RecordId { get; set; }
        public Guid EntityId { get; set; }
        public bool IsDefault { get; set; }
    }
    public class SetFormAuthorizationStateModel
    {
        public Guid[] RecordId { get; set; }
        public bool IsAuthorization { get; set; }
        public int ComponentType { get; set; }
    }
    public class SetFormButtonsModel
    {
        public Guid RecordId { get; set; }
        public bool IsCustomButton { get; set; }
        public List<Guid> Buttons { get; set; }
    }
}
