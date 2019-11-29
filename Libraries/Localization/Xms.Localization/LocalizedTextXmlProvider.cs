using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Xms.Caching;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;

namespace Xms.Localization
{
    /// <summary>
    /// 系统多语言服务
    /// </summary>
    public class LocalizedTextXmlProvider : ILocalizedTextProvider
    {
        private readonly string _langName = ((int)LocalizationDefaults.DefaultLanguage).ToString();
        private readonly CacheManager<List<LocalizedTextLabel>> _cache;
        private readonly ICurrentUser _user;
        private readonly IWebHelper _webHelper;
        private readonly LocalizationOptions _localizationOptions;

        public LocalizedTextXmlProvider(ICurrentUser user
            , IWebHelper webHelper
            , IOptions<LocalizationOptions> options)
        {
            _webHelper = webHelper;
            _user = user;
            _localizationOptions = options.Value;
            if (_user != null && _user.HasValue())
            {
                _langName = ((int)_user.UserSettings.LanguageId).ToString();
            }
            _cache = new CacheManager<List<LocalizedTextLabel>>(LocalizationDefaults.CACHE_KEY, (List<LocalizedTextLabel> l) => { return LocalizationDefaults.CACHE_KEY + _langName; });
        }

        private string FilePath
        {
            get
            {
                return Path.Combine(_webHelper.MapPath(_localizationOptions.FilePath), _langName + ".xml");
            }
        }

        private List<LocalizedTextLabel> LoadAll()
        {
            var result = _cache.Get(() =>
            {
                var labels = new List<LocalizedTextLabel>();
                XDocument doc = XDocument.Load(FilePath);
                IEnumerable<XElement> elements = from e in doc.Element("Labels").Elements("Label")
                                                 select e;
                elements.ToList().ForEach(item =>
                {
                    labels.Add(new LocalizedTextLabel() { Name = item.Attribute("name").Value, Text = item.Value, Language = _user.UserSettings.LanguageId });
                });
                return labels;
            });
            return result;
        }

        public void ReFresh()
        {
            _cache.Remove();
            LoadAll();
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

        public string this[string key]
        {
            get
            {
                var a = Labels.FirstOrDefault(x => x.Language == _user.UserSettings.LanguageId && x.Name.IsCaseInsensitiveEqual(key));
                return a != null ? a.Text : string.Empty;
            }
        }

        public bool Save(LanguageCode language, params LocalizedTextLabel[] labels)
        {
            if (labels.IsEmpty())
            {
                return false;
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(FilePath);
            XmlNode root = doc.SelectSingleNode("/Labels");
            labels.ToList().ForEach(item =>
            {
                var original = this.Labels.FirstOrDefault(x => x.Name.IsCaseInsensitiveEqual(item.Name));
                if (original != null)
                {
                    original.Text = item.Text;
                    var node = root.SelectSingleNode($"Label[@name=\"{item.Name.ToLower()}\"]");
                    node.InnerText = item.Text;
                }
                else
                {
                    XmlElement node = doc.CreateElement("Label");
                    node.SetAttribute("name", item.Name.ToLower());
                    node.InnerText = item.Text;
                    root.AppendChild(node);
                }
            });
            doc.Save(FilePath);
            this.ReFresh();
            return true;
        }
    }
}