using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Localization.Domain;

namespace Xms.Localization
{
    public interface ILanguageService
    {
        bool Create(Language entity);

        bool CreateMany(List<Language> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        Language Find(Expression<Func<Language, bool>> predicate);

        Language FindById(Guid id);

        List<Language> FindAll();

        List<Language> Query(Func<QueryDescriptor<Language>, QueryDescriptor<Language>> container);

        PagedList<Language> QueryPaged(Func<QueryDescriptor<Language>, QueryDescriptor<Language>> container);

        bool Update(Func<UpdateContext<Language>, UpdateContext<Language>> context);

        bool Update(Language entity);
    }
}