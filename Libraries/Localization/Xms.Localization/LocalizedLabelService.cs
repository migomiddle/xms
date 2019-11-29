using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Localization.Data;
using Xms.Localization.Domain;
using Xms.Module.Core;

namespace Xms.Localization
{
    /// <summary>
    /// 业务多语言标签服务
    /// </summary>
    public class LocalizedLabelService : ILocalizedLabelService
    {
        private readonly ILocalizedLabelRepository _localizedLabelRepository;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _user;
        private LanguageCode _languageId { get { return _user.UserSettings.LanguageId; } }

        public LocalizedLabelService(IAppContext appContext
            , ILocalizedLabelRepository localizedLabelRepository
            )
        {
            _appContext = appContext;
            _user = _appContext.GetFeature<ICurrentUser>();
            _localizedLabelRepository = localizedLabelRepository;
        }

        public bool Create(LocalizedLabel entity)
        {
            if (entity.Label.IsEmpty())
            {
                return false;
            }

            return _localizedLabelRepository.Create(entity);
        }

        public bool Create(Guid solutionId, string label, string labelTypeName, string columnName, Guid objectId, LanguageCode languageId)
        {
            if (label.IsEmpty())
            {
                return false;
            }

            return _localizedLabelRepository.Create(new LocalizedLabel()
            {
                ComponentState = 0
                    ,
                SolutionId = solutionId
                    ,
                Label = label
                    ,
                LabelTypeCode = ModuleCollection.GetIdentity(labelTypeName)
                    ,
                LanguageId = languageId
                    ,
                ObjectColumnName = columnName
                    ,
                ObjectId = objectId
            });
        }

        public bool CreateMany(IList<LocalizedLabel> entities)
        {
            return _localizedLabelRepository.CreateMany(entities.Where(n => n.Label.IsNotEmpty()).ToList());
        }

        public bool Update(string label, string columnName, Guid objectId, LanguageCode languageId)
        {
            return this.Update(n => n.Set(f => f.Label, label).Where(f => f.ObjectId == objectId && f.ObjectColumnName == columnName && f.LanguageId == languageId));
        }

        public bool Update(LocalizedLabel entity)
        {
            return _localizedLabelRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<LocalizedLabel>, UpdateContext<LocalizedLabel>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<LocalizedLabel>());
            return _localizedLabelRepository.Update(ctx);
        }

        public LocalizedLabel FindById(Guid id)
        {
            return _localizedLabelRepository.FindById(id);
        }

        public LocalizedLabel Find(Expression<Func<LocalizedLabel, bool>> predicate)
        {
            return _localizedLabelRepository.Find(predicate);
        }

        public bool DeleteById(params Guid[] ids)
        {
            return _localizedLabelRepository.DeleteMany(ids);
        }

        public bool DeleteByObject(Guid objectId, params string[] name)
        {
            return _localizedLabelRepository.DeleteByObject(objectId, name);
        }

        public bool DeleteByObject(params Guid[] objectId)
        {
            return _localizedLabelRepository.DeleteByObject(objectId);
        }

        public PagedList<LocalizedLabel> QueryPaged(Func<QueryDescriptor<LocalizedLabel>, QueryDescriptor<LocalizedLabel>> container)
        {
            QueryDescriptor<LocalizedLabel> q = container(QueryDescriptorBuilder.Build<LocalizedLabel>());

            return _localizedLabelRepository.QueryPaged(q);
        }

        public List<LocalizedLabel> Query(Func<QueryDescriptor<LocalizedLabel>, QueryDescriptor<LocalizedLabel>> container)
        {
            QueryDescriptor<LocalizedLabel> q = container(QueryDescriptorBuilder.Build<LocalizedLabel>());

            return _localizedLabelRepository.Query(q)?.ToList();
        }

        public string GetLabelText(Guid objectId, string columnName)
        {
            var result = this.Find(n => n.ObjectId == objectId && n.ObjectColumnName == columnName && n.LanguageId == _languageId);
            if (result != null)
            {
                return result.Label;
            }
            return columnName;
        }

        public string GetLabelText(IList<LocalizedLabel> labels, Guid objectId, string name, string defaultLabel = "")
        {
            if (labels.IsEmpty())
            {
                return defaultLabel ?? string.Empty;
            }
            var lbl = labels.ToList().Find(n => n.ObjectId == objectId && n.ObjectColumnName.IsCaseInsensitiveEqual(name));
            if (null != lbl)
            {
                return lbl.Label;
            }
            else
            {
                return defaultLabel ?? string.Empty;
            }
        }

        public List<LocalizedLabel> GetLabels(Guid objectId, params string[] columnName)
        {
            if (columnName.NotEmpty())
            {
                var result = this.Query(n => n.Where(f => f.ObjectId == objectId && f.ObjectColumnName.In(columnName) && f.LanguageId == _languageId));
                return result;
            }
            else
            {
                var result = this.Query(n => n.Where(f => f.ObjectId == objectId));
                return result;
            }
        }
    }
}