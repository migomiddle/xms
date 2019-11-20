using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图导入服务
    /// </summary>
    public class QueryViewImporter : IQueryViewImporter
    {
        private readonly IQueryViewCreater _queryViewCreater;
        private readonly IQueryViewUpdater _queryViewUpdater;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IAppContext _appContext;

        public QueryViewImporter(IAppContext appContext
            , IQueryViewCreater queryViewCreater
            , IQueryViewUpdater queryViewUpdater
            , IQueryViewFinder queryViewFinder)
        {
            _appContext = appContext;
            _queryViewCreater = queryViewCreater;
            _queryViewUpdater = queryViewUpdater;
            _queryViewFinder = queryViewFinder;
        }

        public bool Import(Guid solutionId, List<Domain.QueryView> queryViews)
        {
            if (queryViews.NotEmpty())
            {
                foreach (var item in queryViews)
                {
                    var entity = _queryViewFinder.FindById(item.QueryViewId);
                    if (entity != null)
                    {
                        entity.Description = item.Description;
                        entity.FetchConfig = item.FetchConfig;
                        entity.IsDefault = item.IsDefault;
                        entity.StateCode = item.StateCode;
                        entity.LayoutConfig = item.LayoutConfig;
                        entity.IsCustomButton = item.IsCustomButton;
                        entity.CustomButtons = item.CustomButtons;
                        entity.TargetFormId = item.TargetFormId;
                        entity.ModifiedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        entity.ModifiedOn = DateTime.Now;
                        entity.Name = item.Name;
                        _queryViewUpdater.Update(entity);
                    }
                    else
                    {
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.CreatedOn = DateTime.Now;
                        _queryViewCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}