using Xms.Context;
using Xms.Data.Abstractions;
using Xms.DataMapping.Data;
using Xms.DataMapping.Domain;
using Xms.Solution.Abstractions;

namespace Xms.DataMapping
{
    /// <summary>
    /// 实体映射创建服务
    /// </summary>
    public class EntityMapCreater : IEntityMapCreater
    {
        private readonly IEntityMapRepository _entityMapRepository;
        private readonly IEntityMapDependency _dependencyService;
        private readonly Caching.CacheManager<EntityMap> _cacheService;
        private readonly IAppContext _appContext;

        public EntityMapCreater(IAppContext appContext
            , IEntityMapRepository entityMapRepository
            , IEntityMapDependency dependencyService
            )
        {
            _appContext = appContext;
            _entityMapRepository = entityMapRepository;
            _dependencyService = dependencyService;
            _cacheService = new Caching.CacheManager<EntityMap>(EntityMapCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(EntityMap entity)
        {
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            var result = true;
            using (UnitOfWork.Build(_entityMapRepository.DbContext))
            {
                result = _entityMapRepository.Create(entity);
                //依赖于实体
                _dependencyService.Create(entity);
                //内容不全，不能缓存
                //_cacheService.SetEntity(entity);
            }
            return result;
        }
    }
}