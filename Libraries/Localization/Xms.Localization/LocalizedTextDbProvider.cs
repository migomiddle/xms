using System.Collections.Generic;
using System.Linq;
using Xms.Caching;
using Xms.Core.Data;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Localization.Data;
using Xms.Localization.Domain;

namespace Xms.Localization
{
    /// <summary>
    /// 系统多语言服务
    /// </summary>
    public class LocalizedTextDbProvider : ILocalizedTextProvider
    {
        private readonly ILocalizedTextRepository _localizedTextRepository;
        private readonly ICurrentUser _user;
        private readonly CacheManager<List<LocalizedTextLabel>> _cache;
        private readonly LanguageCode _language = LocalizationDefaults.DefaultLanguage;
        private readonly string _langName = ((int)LocalizationDefaults.DefaultLanguage).ToString();

        public LocalizedTextDbProvider(ICurrentUser user
            , ILocalizedTextRepository localizedTextRepository)
        {
            _user = user;
            if (_user != null && _user.HasValue())
            {
                _language = _user.UserSettings.LanguageId;
                _langName = ((int)_language).ToString();
            }
            _localizedTextRepository = localizedTextRepository;
            _cache = new CacheManager<List<LocalizedTextLabel>>(LocalizationDefaults.CACHE_KEY, (List<LocalizedTextLabel> l) => { return LocalizationDefaults.CACHE_KEY + _langName; });
        }

        private List<LocalizedTextLabel> LoadAll()
        {
            var result = _cache.Get(() =>
            {
                var datas = _localizedTextRepository.Query(x => x.LanguageId == _language);
                var labels = from d in datas select new LocalizedTextLabel() { Name = d.Name, Text = d.Text, Language = d.LanguageId };
                return labels.ToList();
            });
            return result;
        }

        public string this[string key]
        {
            get
            {
                var a = Labels.FirstOrDefault(x => x.Language == _user.UserSettings.LanguageId && x.Name.IsCaseInsensitiveEqual(key));
                return a != null ? a.Text : string.Empty;
            }
        }

        public IList<LocalizedTextLabel> _labels;

        public IList<LocalizedTextLabel> Labels
        {
            get
            {
                return LoadAll();
            }
            set
            {
                _labels = value;
            }
        }

        public void ReFresh()
        {
            _cache.Remove();
            LoadAll();
        }

        public bool Save(LanguageCode language, params LocalizedTextLabel[] labels)
        {
            if (labels.IsEmpty())
            {
                return false;
            }
            var datas = labels.Select(x =>
            new LocalizedResource()
            {
                LanguageId = language
                ,
                Name = x.Name
                ,
                Text = x.Text
            }
            );
            int batchCount = 50, totalCount = datas.Count(), skip = 0;
            while (totalCount >= 0)
            {
                var names = datas.Skip(skip).Take(batchCount).Select(x => x.Name);
                _localizedTextRepository.DeleteMany(x => x.Name.In(names));
                totalCount -= batchCount;
                skip += batchCount;
            }
            var result = _localizedTextRepository.CreateMany(datas);
            if (result)
            {
                this.ReFresh();
            }
            return result;
        }
    }
}