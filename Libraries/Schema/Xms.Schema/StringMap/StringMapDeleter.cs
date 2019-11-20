using System;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Data;
using Xms.Schema.Extensions;

namespace Xms.Schema.StringMap
{
    /// <summary>
    /// 字段选项删除服务
    /// </summary>
    public class StringMapDeleter : IStringMapDeleter, ICascadeDelete<Domain.Attribute>
    {
        private readonly IStringMapRepository _stringMapRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;

        public StringMapDeleter(IAppContext appContext
            , IStringMapRepository stringMapRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _stringMapRepository = stringMapRepository;
            _localizedLabelService = localizedLabelService;
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _stringMapRepository.Query(x => x.StringMapId.In(id));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
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
            var ids = parent.Where(x => x.TypeIsBit() || x.TypeIsState())
                ?.Select(x => x.AttributeId).ToList();
            if (ids.IsEmpty())
            {
                return;
            }
            var deleteds = _stringMapRepository.Query(x => x.AttributeId.In(ids));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }

        private bool DeleteCore(params Domain.StringMap[] deleted)
        {
            Guard.NotEmpty(deleted, nameof(deleted));
            var result = false;
            var ids = deleted.Select(x => x.StringMapId).ToArray();
            using (UnitOfWork.Build(_stringMapRepository.DbContext))
            {
                result = _stringMapRepository.DeleteMany(ids);
                //localization
                _localizedLabelService.DeleteByObject(ids);
            }
            return result;
        }
    }
}