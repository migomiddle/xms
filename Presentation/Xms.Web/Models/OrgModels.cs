using System;
using System.Collections.Generic;
using Xms.Localization.Abstractions;
using Xms.Localization.Domain;
using Xms.Organization.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Models
{
    public class OrganizationModel : BasePaged<Organization.Domain.Organization>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? State { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
    }

    public class EditOrganizationModel
    {
        public Guid? OrganizationId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public LanguageCode LanguageId { get; set; }
        public Guid BaseCurrencyId { get; set; }
        public Guid ManagerId { get; set; }

        public int State { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
        public List<Language> LanguageList { get; set; }

        public Core.Data.Entity Datas { get; set; }
    }

    public class BusinessUnitModel : BasePaged<BusinessUnit>
    {
        public Guid? BusinessUnitId { get; set; }

        public string Name { get; set; }
        public Guid? ParentBusinessUnitId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string Description { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }

        public int? State { get; set; }

        public Schema.Domain.Entity EntityMeta { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
    }

    public class EditBusinessUnitModel
    {
        public Guid? BusinessUnitId { get; set; }

        public string Name { get; set; }
        public Guid? ParentBusinessUnitId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string ParentBusinessUnitName { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }

        public int State { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
    }
}