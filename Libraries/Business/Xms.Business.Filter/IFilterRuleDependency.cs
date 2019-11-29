using System;
using System.Collections.Generic;
using Xms.Business.Filter.Domain;

namespace Xms.Business.Filter
{
    public interface IFilterRuleDependency
    {
        bool Create(FilterRule entity);

        bool Delete(params Guid[] id);

        List<Schema.Domain.Attribute> GetRequireds(FilterRule entity);

        bool Update(FilterRule entity);
    }
}