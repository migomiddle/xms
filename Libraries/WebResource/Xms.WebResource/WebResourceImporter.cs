using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.WebResource
{
    /// <summary>
    /// web资源导入服务
    /// </summary>
    [SolutionImportNode("webresources")]
    public class WebResourceImporter : ISolutionComponentImporter<Domain.WebResource>
    {
        private readonly IWebResourceCreater _webResourceCreater;
        private readonly IWebResourceUpdater _webResourceUpdater;
        private readonly IWebResourceFinder _webResourceFinder;
        private readonly IAppContext _appContext;

        public WebResourceImporter(IAppContext appContext
            , IWebResourceCreater webResourceCreater
            , IWebResourceUpdater webResourceUpdater
            , IWebResourceFinder webResourceFinder)
        {
            _appContext = appContext;
            _webResourceCreater = webResourceCreater;
            _webResourceUpdater = webResourceUpdater;
            _webResourceFinder = webResourceFinder;
        }

        public bool Import(Guid solutionId, IList<Domain.WebResource> webResources)
        {
            if (webResources.NotEmpty())
            {
                foreach (var item in webResources)
                {
                    var entity = _webResourceFinder.FindById(item.WebResourceId, false);
                    if (entity != null)
                    {
                        entity.Content = item.Content;
                        entity.Description = item.Description;
                        entity.Name = item.Name;
                        _webResourceUpdater.Update(entity);
                    }
                    else
                    {
                        item.ComponentState = 0;
                        item.SolutionId = solutionId;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.OrganizationId = _appContext.OrganizationId;
                        _webResourceCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}