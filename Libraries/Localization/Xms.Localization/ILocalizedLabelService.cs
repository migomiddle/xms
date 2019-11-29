using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Localization.Abstractions;
using Xms.Localization.Domain;

namespace Xms.Localization
{
    public interface ILocalizedLabelService
    {
        bool Create(Guid solutionId, string label, string labelTypeName, string columnName, Guid objectId, LanguageCode languageId);

        bool Create(LocalizedLabel entity);

        bool CreateMany(IList<LocalizedLabel> entities);

        bool DeleteById(params Guid[] id);

        bool DeleteByObject(Guid objectId, params string[] name);

        bool DeleteByObject(params Guid[] objectId);

        LocalizedLabel Find(Expression<Func<LocalizedLabel, bool>> predicate);

        LocalizedLabel FindById(Guid id);

        List<LocalizedLabel> GetLabels(Guid objectId, params string[] columnName);

        string GetLabelText(Guid objectId, string columnName);

        string GetLabelText(IList<LocalizedLabel> labels, Guid objectId, string name, string defaultLabel = "");

        List<LocalizedLabel> Query(Func<QueryDescriptor<LocalizedLabel>, QueryDescriptor<LocalizedLabel>> container);

        PagedList<LocalizedLabel> QueryPaged(Func<QueryDescriptor<LocalizedLabel>, QueryDescriptor<LocalizedLabel>> container);

        bool Update(Func<UpdateContext<LocalizedLabel>, UpdateContext<LocalizedLabel>> context);

        bool Update(LocalizedLabel entity);

        bool Update(string label, string columnName, Guid objectId, LanguageCode languageId);
    }
}