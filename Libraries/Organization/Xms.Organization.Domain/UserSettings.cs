using System;
using Xms.Localization.Abstractions;

namespace Xms.Organization.Domain
{
    public partial class UserSettings
    {
        public Guid DefaultDashboardId { get; set; }
        public string HomePageArea { get; set; }
        public bool EnabledNotification { get; set; }
        public LanguageCode LanguageId { get; set; } = LanguageCode.CHS;
        public int PagingLimit { get; set; }
        public Guid CurrencyId { get; set; }
        public int LayoutType { get; set; }
    }
}