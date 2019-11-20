using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetDetailFinder
    {
        Domain.OptionSetDetail Find(Expression<Func<Domain.OptionSetDetail, bool>> predicate);

        Domain.OptionSetDetail FindById(Guid id);

        List<Domain.OptionSetDetail> FindByParentId(Guid optionSetId);

        string GetOptionName(Guid optionSetId, int value);

        List<Domain.OptionSetDetail> Query(Func<QueryDescriptor<Domain.OptionSetDetail>, QueryDescriptor<Domain.OptionSetDetail>> container);

        PagedList<Domain.OptionSetDetail> QueryPaged(Func<QueryDescriptor<Domain.OptionSetDetail>, QueryDescriptor<Domain.OptionSetDetail>> container);
    }
}