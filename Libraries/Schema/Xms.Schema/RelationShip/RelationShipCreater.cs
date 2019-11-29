using System.Collections.Generic;
using Xms.Context;
using Xms.Schema.Data;
using Xms.Solution.Abstractions;

namespace Xms.Schema.RelationShip
{
    /// <summary>
    /// 关系创建服务
    /// </summary>
    public class RelationShipCreater : IRelationShipCreater
    {
        private readonly IRelationShipRepository _relationShipRepository;
        private readonly Caching.CacheManager<Domain.RelationShip> _cacheService;
        private readonly IAppContext _appContext;

        public RelationShipCreater(IAppContext appContext
            , IRelationShipRepository relationShipRepository)
        {
            _appContext = appContext;
            _relationShipRepository = relationShipRepository;
            _cacheService = new Caching.CacheManager<Domain.RelationShip>(_appContext.OrganizationUniqueName + ":relationships", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(Domain.RelationShip entity)
        {
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            bool result = _relationShipRepository.Create(entity);
            if (result)
            {
                //this.WrapLocalizedLabel(entity);
                //这里缓存对象会不完整
            }
            return result;
        }

        public bool CreateMany(List<Domain.RelationShip> entities)
        {
            var result = _relationShipRepository.CreateMany(entities);
            if (result)
            {
                //this.WrapLocalizedLabel(entities);
                //this.AllRelationShips.AddRange(entities);
            }
            return result;
        }
    }
}