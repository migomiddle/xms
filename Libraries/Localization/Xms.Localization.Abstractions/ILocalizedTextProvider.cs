using System.Collections.Generic;

namespace Xms.Localization.Abstractions
{
    public interface ILocalizedTextProvider
    {
        string this[string key] { get; }
        IList<LocalizedTextLabel> Labels { get; set; }

        void ReFresh();

        bool Save(LanguageCode language, params LocalizedTextLabel[] labels);
    }

    public class LocalizedTextLabel
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public LanguageCode Language { get; set; } = LanguageCode.CHS;
    }
}