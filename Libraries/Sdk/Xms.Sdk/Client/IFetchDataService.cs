using System.Collections.Generic;
using Xms.Identity;
using Xms.Localization.Abstractions;
using Xms.Schema.Domain;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;

namespace Xms.Sdk.Client
{
    public interface IFetchDataService
    {
        ILocalizedTextProvider Loc { get; }
        Entity MainEntity { get; }
        QueryExpression QueryExpression { get; set; }
        IQueryResolver QueryResolver { get; set; }
        ICurrentUser User { get; set; }
        List<Attribute> NonePermissionFields { get; }

        Core.Context.PagedList<dynamic> Execute(FetchDescriptor fetch, bool wrapOptionName = false);

        Core.Context.PagedList<dynamic> Execute(QueryExpression query, bool wrapOptionName = false);

        void GetMetaDatas(FetchDescriptor fetch);

        void GetMetaDatas(string fetchConfig);
    }
}