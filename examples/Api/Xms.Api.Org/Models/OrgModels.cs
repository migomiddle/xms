using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xms.Localization.Abstractions;
using Xms.Localization.Domain;
using Xms.Web.Framework.Paging;
using Xms.Core.Data;

namespace Xms.Api.Org.Models
{
    public class OrganizationModel : BasePaged<Organization.Domain.Organization>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? StatusCode { get; set; }
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

        public int StatusCode { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
        public List<Language> LanguageList { get; set; }

        public Entity Datas { get; set; }
    }
}
