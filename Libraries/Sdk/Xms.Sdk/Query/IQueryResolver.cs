using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Schema.Domain;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Query
{
    public interface IQueryResolver
    {
        string AliasJoiner { get; set; }
        List<AttributeAlias> AttributeAliasList { get; set; }
        List<Attribute> AttributeList { get; set; }
        List<Entity> EntityList { get; set; }
        Entity MainEntity { get; set; }
        QueryParameters Parameters { get; set; }
        QueryBase QueryObject { get; set; }
        List<RelationShip> RelationShipList { get; set; }

        IQueryResolver Init(QueryBase query);

        dynamic Find(bool ignorePermissions = false, List<Attribute> noneReadFields = null);

        List<dynamic> Query(bool ignorePermissions = false, List<Attribute> noneReadFields = null);

        PagedList<dynamic> QueryPaged(bool ignorePermissions = false, List<Attribute> noneReadFields = null);

        string ToSqlString(bool includeNameField = true, bool ignorePermissions = false, List<Attribute> noneReadFields = null);
    }
}