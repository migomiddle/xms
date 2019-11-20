using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Solution.Abstractions;

namespace Xms.Schema.Entity
{
    /// <summary>
    /// 实体导入服务
    /// </summary>
    [SolutionImportNode("entities")]
    public class EntityImporter : ISolutionComponentImporter<Domain.Entity>
    {
        private readonly IEntityCreater _entityCreater;
        private readonly IEntityUpdater _entityUpdater;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeImporter _attributeImporter;
        private readonly IAppContext _appContext;

        public EntityImporter(IAppContext appContext
            , IEntityCreater entityCreater
            , IEntityUpdater entityUpdater
            , IEntityFinder entityFinder
            , IAttributeImporter attributeImporter)
        {
            _appContext = appContext;
            _entityCreater = entityCreater;
            _entityUpdater = entityUpdater;
            _entityFinder = entityFinder;
            _attributeImporter = attributeImporter;
        }

        public bool Import(Guid solutionId, IList<Domain.Entity> entities)
        {
            if (entities.NotEmpty())
            {
                foreach (var item in entities)
                {
                    var existEntity = _entityFinder.FindById(item.EntityId);
                    if (existEntity != null)
                    {
                        existEntity.DuplicateEnabled = item.DuplicateEnabled;
                        existEntity.AuthorizationEnabled = item.AuthorizationEnabled;
                        existEntity.LogEnabled = item.LogEnabled;
                        existEntity.LocalizedName = item.LocalizedName;
                        existEntity.BusinessFlowEnabled = item.BusinessFlowEnabled;
                        existEntity.WorkFlowEnabled = item.WorkFlowEnabled;
                        _entityUpdater.Update(existEntity);
                    }
                    else
                    {
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.CreatedOn = DateTime.Now;
                        item.IsCustomizable = true;
                        item.OrganizationId = _appContext.OrganizationId;
                        _entityCreater.Create(item, false);
                    }
                    //字段
                    _attributeImporter.Import(solutionId, item, item.Attributes);
                }
            }
            return true;
        }
    }
}