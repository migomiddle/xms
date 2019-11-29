using Xms.Context;
using Xms.Data.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Data;

namespace Xms.Schema.Entity
{
    /// <summary>
    /// 实体更新服务
    /// </summary>
    public class EntityUpdater : IEntityUpdater
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly Caching.CacheManager<Domain.Entity> _cacheService;

        private readonly IAttributeCreater _attributeCreater;
        private readonly IAppContext _appContext;

        public EntityUpdater(IAppContext appContext
            , IEntityRepository entityRepository
            , ILocalizedLabelService localizedLabelService
            , IAttributeCreater attributeCreater)
        {
            _appContext = appContext;
            _entityRepository = entityRepository;
            _localizedLabelService = localizedLabelService;
            _cacheService = new Caching.CacheManager<Domain.Entity>(_appContext.OrganizationUniqueName + ":entities", _appContext.PlatformSettings.CacheEnabled);
            _attributeCreater = attributeCreater;
        }

        public bool Update(Domain.Entity entity)
        {
            var oldEntity = _entityRepository.FindById(entity.EntityId);
            if (oldEntity == null)
            {
                return false;
            }
            var result = true;
            if (!oldEntity.IsCustomizable)
            {
                entity.AuthorizationEnabled = oldEntity.AuthorizationEnabled;
                entity.WorkFlowEnabled = oldEntity.WorkFlowEnabled;
                entity.BusinessFlowEnabled = oldEntity.BusinessFlowEnabled;
                entity.EntityMask = oldEntity.EntityMask;
            }
            using (UnitOfWork.Build(_entityRepository.DbContext))
            {
                result = _entityRepository.Update(entity);
                //从组织范围改成用户范围
                if (oldEntity.EntityMask != entity.EntityMask && entity.EntityMask == EntityMaskEnum.User)
                {
                    //创建所有者字段
                    _attributeCreater.CreateOwnerAttributes(entity);
                }
                //启用审批流，创建相关字段
                if (!oldEntity.WorkFlowEnabled && entity.WorkFlowEnabled)
                {
                    _attributeCreater.CreateWorkFlowAttributes(entity);
                }
                //启用业务流，创建相关字段
                if (!oldEntity.BusinessFlowEnabled && entity.BusinessFlowEnabled)
                {
                    _attributeCreater.CreateBusinessFlowAttributes(entity);
                }
                //localization
                _localizedLabelService.Update(entity.LocalizedName.IfEmpty(""), "LocalizedName", entity.EntityId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.EntityId, _appContext.BaseLanguage);

                //set to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }
    }
}