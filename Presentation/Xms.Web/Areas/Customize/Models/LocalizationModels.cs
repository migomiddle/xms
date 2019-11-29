using System;
using System.Collections.Generic;
using Xms.Localization.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class LocalizationLabelsModel : BasePaged<LocalizedLabel>
    {
        public int? TypeCode { get; set; }
        public Guid? SolutionId { get; set; }
    }

    public class UpdateLocalizationLabelModel
    {
        public Guid ObjectId { get; set; }
        public string ObjectColumnName { get; set; }

        public string[] Label { get; set; }
        public int[] LanguageId { get; set; }

        public List<LocalizedLabel> ObjectLabels { get; set; }

        public List<Language> Languages { get; set; }
    }
}