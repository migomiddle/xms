using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Caching;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Localization.Data;
using Xms.Localization.Domain;

namespace Xms.Localization
{
    /// <summary>
    /// 语言选项服务
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly CacheManager<Language> _cache;

        public LanguageService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
            _cache = new CacheManager<Language>("$language$", (Language l) => { return "$language$"; });
        }

        public bool Create(Language entity)
        {
            var result = _languageRepository.Create(entity);
            if (result)
            {
                _cache.Remove();
            }
            return result;
        }

        public bool CreateMany(List<Language> entities)
        {
            var result = _languageRepository.CreateMany(entities);
            if (result)
            {
                _cache.Remove();
            }
            return result;
        }

        public bool Update(Language entity)
        {
            var result = _languageRepository.Update(entity);
            if (result)
            {
                _cache.Remove();
            }
            return result;
        }

        public bool Update(Func<UpdateContext<Language>, UpdateContext<Language>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Language>());
            var result = _languageRepository.Update(ctx);
            if (result)
            {
                _cache.Remove();
            }
            return result;
        }

        public Language FindById(Guid id)
        {
            return _cache.GetItem(() =>
            {
                return _languageRepository.FindById(id);
            }, id.ToString());
        }

        public Language Find(Expression<Func<Language, bool>> predicate)
        {
            return _languageRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            var result = _languageRepository.DeleteById(id);
            if (result)
            {
                _cache.Remove();
            }
            return result;
        }

        public bool DeleteById(List<Guid> ids)
        {
            var result = _languageRepository.DeleteMany(ids);
            if (result)
            {
                _cache.Remove();
            }
            return result;
        }

        public List<Language> FindAll()
        {
            return _languageRepository.FindAll()?.ToList();
        }

        public PagedList<Language> QueryPaged(Func<QueryDescriptor<Language>, QueryDescriptor<Language>> container)
        {
            QueryDescriptor<Language> q = container(QueryDescriptorBuilder.Build<Language>());

            return _languageRepository.QueryPaged(q);
        }

        public List<Language> Query(Func<QueryDescriptor<Language>, QueryDescriptor<Language>> container)
        {
            QueryDescriptor<Language> q = container(QueryDescriptorBuilder.Build<Language>());

            return _languageRepository.Query(q)?.ToList();
        }
    }
}