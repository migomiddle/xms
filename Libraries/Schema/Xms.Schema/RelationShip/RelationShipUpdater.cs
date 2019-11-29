using System;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Schema.Data;

namespace Xms.Schema.RelationShip
{
    /// <summary>
    /// 关系更新服务
    /// </summary>
    public class RelationShipUpdater : IRelationShipUpdater
    {
        private readonly IRelationShipRepository _relationShipRepository;
        private readonly Caching.CacheManager<Domain.RelationShip> _cacheService;
        private readonly IAppContext _appContext;

        public RelationShipUpdater(IAppContext appContext
            , IRelationShipRepository relationShipRepository)
        {
            _appContext = appContext;
            _relationShipRepository = relationShipRepository;
            _cacheService = new Caching.CacheManager<Domain.RelationShip>(_appContext.OrganizationUniqueName + ":relationships", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(Domain.RelationShip entity)
        {
            bool flag = _relationShipRepository.Update(entity);
            if (flag)
            {
                //this.WrapLocalizedLabel(entity);
                //set to cache
                _cacheService.SetEntity(entity);
            }
            return flag;
        }

        public bool Update(Func<UpdateContext<Domain.RelationShip>, UpdateContext<Domain.RelationShip>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.RelationShip>());
            var flag = _relationShipRepository.Update(ctx);
            if (flag)
            {
                _cacheService.Remove();
            }
            return flag;
        }
    }
}