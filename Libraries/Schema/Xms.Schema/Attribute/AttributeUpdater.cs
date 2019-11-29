using System;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Schema.OptionSet;
using Xms.Schema.StringMap;

namespace Xms.Schema.Attribute
{
    /// <summary>
    /// 字段更新服务
    /// </summary>
    public class AttributeUpdater : IAttributeUpdater
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IOptionSetDetailCreater _optionSetDetailCreater;
        private readonly IOptionSetDetailDeleter _optionSetDetailDeleter;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IOptionSetDetailUpdater _optionSetDetailUpdater;
        private readonly IStringMapCreater _stringMapCreater;
        private readonly IStringMapUpdater _stringMapUpdater;
        private readonly IEventPublisher _eventPublisher;
        private readonly Caching.CacheManager<Domain.Attribute> _cacheService;
        private readonly Caching.CacheManager<Domain.OptionSet> _cacheServiceOption;
        private readonly IAppContext _appContext;
        private readonly IOptionSetFinder _optionSetFinder;

        public AttributeUpdater(IAppContext appContext
            , IAttributeRepository attributeRepository
            , IOptionSetDetailCreater optionSetDetailCreater
            , IOptionSetDetailDeleter optionSetDetailDeleter
            , IOptionSetDetailFinder optionSetDetailFinder
            , IOptionSetDetailUpdater optionSetDetailUpdater
            , IOptionSetFinder optionSetFinder
            , IStringMapCreater stringMapCreater
            , IStringMapUpdater stringMapUpdater
            , ILocalizedLabelService localizedLabelService
            , IEventPublisher eventPublisher)
        {
            _appContext = appContext;
            _attributeRepository = attributeRepository;
            _localizedLabelService = localizedLabelService;
            _optionSetDetailCreater = optionSetDetailCreater;
            _optionSetDetailDeleter = optionSetDetailDeleter;
            _optionSetDetailFinder = optionSetDetailFinder;
            _optionSetDetailUpdater = optionSetDetailUpdater;
            _optionSetFinder = optionSetFinder;
            _stringMapCreater = stringMapCreater;
            _stringMapUpdater = stringMapUpdater;
            _eventPublisher = eventPublisher;
            _cacheService = new Caching.CacheManager<Domain.Attribute>(_appContext.OrganizationUniqueName + ":attributes", _appContext.PlatformSettings.CacheEnabled);
            _cacheServiceOption = new Caching.CacheManager<Domain.OptionSet>(_appContext.OrganizationUniqueName + ":optionsets", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(Domain.Attribute entity)
        {
            var result = true;
            using (UnitOfWork.Build(_attributeRepository.DbContext))
            {
                result = _attributeRepository.Update(entity);
                if (entity.OptionSetId.HasValue && entity.OptionSet != null && !entity.OptionSet.IsPublic)
                {
                    var details = _optionSetDetailFinder.Query(n => n.Select(f => f.OptionSetDetailId).Where(f => f.OptionSetId == entity.OptionSetId.Value));
                    foreach (var item in entity.OptionSet.Items)
                    {
                        if (item.OptionSetDetailId.Equals(Guid.Empty))
                        {
                            item.OptionSetDetailId = Guid.NewGuid();
                            result = _optionSetDetailCreater.Create(item);
                        }
                        else
                        {
                            result = _optionSetDetailUpdater.Update(item);
                        }
                    }
                    //delete lost
                    var ids = entity.OptionSet.Items.Select(n => n.OptionSetDetailId);
                    var lostid = details.Select(n => n.OptionSetDetailId).Except(ids).ToList();
                    if (lostid.NotEmpty())
                    {
                        result = _optionSetDetailDeleter.DeleteById(lostid.ToArray());
                    }
                    var optionSetEntity = _optionSetFinder.FindById(entity.OptionSet.OptionSetId);
                    _cacheServiceOption.RemoveEntity(optionSetEntity);
                }
                if (entity.PickLists.NotEmpty())//bit
                {
                    foreach (var item in entity.PickLists)
                    {
                        if (item.StringMapId.Equals(Guid.Empty))
                        {
                            result = _stringMapCreater.Create(item);
                        }
                        else
                        {
                            result = _stringMapUpdater.Update(item);
                        }
                    }
                }
                //localization
                _localizedLabelService.Update(entity.LocalizedName.IfEmpty(""), "LocalizedName", entity.AttributeId, this._appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.AttributeId, this._appContext.BaseLanguage);
                //set to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }

        public bool UpdateAuthorization(bool isAuthorization, params Guid[] id)
        {
            var context = UpdateContextBuilder.Build<Domain.Attribute>();
            context.Set(f => f.AuthorizationEnabled, isAuthorization);
            context.Where(f => f.AttributeId.In(id));
            var result = true;
            using (UnitOfWork.Build(_attributeRepository.DbContext))
            {
                result = _attributeRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = id.ToList()
                    ,
                    State = isAuthorization
                    ,
                    ResourceName = AttributeDefaults.ModuleName
                });
                //set to cache
                var items = _attributeRepository.Query(f => f.AttributeId.In(id)).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }
    }
}