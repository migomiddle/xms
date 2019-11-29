using Xms.Localization.Abstractions;

namespace Xms.Web.Api.Models
{
    public class UpdateLocalizedTextModel
    {
        public LanguageCode Language { get; set; }

        public LocalizedTextLabel[] Labels { get; set; }
    }
}