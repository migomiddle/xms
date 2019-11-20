using Xms.Business.Filter.Domain;

namespace Xms.Business.Filter
{
    public interface IFilterRuleCreater
    {
        bool Create(FilterRule entity);
    }
}