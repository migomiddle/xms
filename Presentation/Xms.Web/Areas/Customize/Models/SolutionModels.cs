using System;
using Xms.Solution.Abstractions;
using Xms.Solution.Domain;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Paging;
using Xms.Web.Models;

namespace Xms.Web.Customize.Models
{
    public class SolutionModel : BasePaged<Solution.Domain.Solution>
    {
        public string Name { get; set; }
    }

    public class CreateSolutionModel
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Version { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime InstalledOn { get; set; }

        public Guid PublisherId { get; set; }
    }

    public class EditSolutionModel
    {
        public Guid SolutionId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Version { get; set; }
    }

    public class SolutionComponentModel : BasePaged<SolutionComponent>
    {
        public Guid SolutionId { get; set; }
        public string ComponentType { get; set; }

        public SolutionComponentDescriptor ComponentDescriptor { get; set; }
    }

    public class SolutionComponentDialogModel : DialogModel
    {
        public Guid SolutionId { get; set; }
        public string ComponentType { get; set; }

        public SolutionComponentDescriptor ComponentDescriptor { get; set; }
    }

    public class CreateSolutionComponentModel
    {
        public Guid SolutionId { get; set; }
        public Guid[] ObjectId { get; set; }
        public string ComponentType { get; set; }
    }

    public class DeleteSolutionComponentModel : DeleteManyModel
    {
        public Guid SolutionId { get; set; }
        public string ComponentType { get; set; }
    }
}