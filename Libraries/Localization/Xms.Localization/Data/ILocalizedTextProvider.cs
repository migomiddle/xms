using System.Collections.Generic;

namespace Xms.Localization.Data
{
    public interface ILocalizedTextProvider
    {
        string this[string key] { get; }
        List<LocalizedTextLabel> Labels { get; set; }

        void ReFresh();
    }

    public class LocalizedTextLabel
    {
        public string Name { get; set; }

        public string Text { get; set; }
    }
}