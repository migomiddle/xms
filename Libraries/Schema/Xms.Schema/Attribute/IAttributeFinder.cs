using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Schema.Attribute
{
    public interface IAttributeFinder
    {
        Domain.Attribute Find(Expression<Func<Domain.Attribute, bool>> predicate);

        Domain.Attribute Find(Guid entityId, string name);

        Domain.Attribute Find(string entityName, string name);

        List<Domain.Attribute> FindByName(Guid entityId, params string[] name);

        List<Domain.Attribute> FindByName(string entityName, params string[] name);

        List<Domain.Attribute> FindByEntityId(Guid entityId);

        List<Domain.Attribute> FindByEntityName(string entityName);

        Domain.Attribute FindById(Guid id);

        List<Domain.Attribute> FindAll();

        List<Domain.Attribute> Query(Func<QueryDescriptor<Domain.Attribute>, QueryDescriptor<Domain.Attribute>> container);

        PagedList<Domain.Attribute> QueryPaged(Func<QueryDescriptor<Domain.Attribute>, QueryDescriptor<Domain.Attribute>> container);

        bool IsExists(Guid entityId, string name);

        bool IsSysAttribute(string name);
    }
}