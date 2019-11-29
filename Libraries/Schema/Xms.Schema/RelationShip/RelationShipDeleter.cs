using System;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Schema.Data;
using Xms.Schema.Extensions;

namespace Xms.Schema.RelationShip
{
    /// <summary>
    /// 关系删除服务
    /// </summary>
    public class RelationShipDeleter : IRelationShipDeleter, ICascadeDelete<Domain.Attribute>
    {
        private readonly IRelationShipRepository _relationShipRepository;
        private readonly Caching.CacheManager<Domain.RelationShip> _cacheService;
        private readonly IAppContext _appContext;

        public RelationShipDeleter(IAppContext appContext
            , IRelationShipRepository relationShipRepository)
        {
            _appContext = appContext;
            _relationShipRepository = relationShipRepository;
            _cacheService = new Caching.CacheManager<Domain.RelationShip>(_appContext.OrganizationUniqueName + ":relationships", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _relationShipRepository.Query(x => x.RelationshipId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds.ToArray());
            }
            return result;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的字段</param>
        public void CascadeDelete(params Domain.Attribute[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var ids = parent.Where(x => x.TypeIsCustomer() || x.TypeIsOwner() || x.TypeIsLookUp())
                ?.Select(x => x.AttributeId).ToList();
            if (ids.IsEmpty())
            {
                return;
            }
            var deleteds = _relationShipRepository.Query(x => x.ReferencingAttributeId.In(ids));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }

        private bool DeleteCore(params Domain.RelationShip[] deleted)
        {
            Guard.NotEmpty(deleted, nameof(deleted));
            var result = false;
            if (deleted != null)
            {
                using (UnitOfWork.Build(_relationShipRepository.DbContext))
                {
                    result = _relationShipRepository.DeleteMany(deleted.Select(x => x.RelationshipId));
                    foreach (var item in deleted)
                    {
                        _cacheService.RemoveEntity(item);
                    }
                }
            }
            return result;
        }
    }
}