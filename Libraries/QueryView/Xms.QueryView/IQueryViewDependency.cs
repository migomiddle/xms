using System;
using Xms.QueryView.Domain;

namespace Xms.QueryView
{
    public interface IQueryViewDependency
    {
        bool Create(Domain.QueryView entity);
        bool Delete(params Guid[] id);
        bool Update(Domain.QueryView entity);
    }
}