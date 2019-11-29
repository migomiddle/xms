using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Infrastructure.Utility;
using Xms.Localization.Domain;
using Xms.Module.Core;

namespace Xms.Localization
{
    /// <summary>
    /// 多语言标签批量构建服务
    /// </summary>
    public class LocalizedLabelBatchBuilder : ILocalizedLabelBatchBuilder
    {
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly List<LocalizedLabel> _entities;
        private readonly IAppContext _appContext;

        public LocalizedLabelBatchBuilder(IAppContext appContext,
            ILocalizedLabelService localizedLabelService)
        {
            _localizedLabelService = localizedLabelService;
            _entities = new List<LocalizedLabel>();
            _appContext = appContext;
        }

        public virtual ILocalizedLabelBatchBuilder Append(Guid solutionId, string label, string labelTypeName, string columnName, Guid objectId)
        {
            var entity = new LocalizedLabel
            {
                ComponentState = 0,
                SolutionId = solutionId,
                Label = label,
                LabelTypeCode = ModuleCollection.GetIdentity(labelTypeName),
                LanguageId = _appContext.GetFeature<Xms.Organization.Domain.Organization>().LanguageId,
                ObjectColumnName = columnName,
                ObjectId = objectId,
                LocalizedLabelId = Guid.NewGuid()
            };
            _entities.Add(entity);
            return this;
        }

        public virtual bool Save()
        {
            if (_entities.NotEmpty())
            {
                var result = _localizedLabelService.CreateMany(_entities);
                if (result)
                {
                    Clear();
                }
                return result;
            }
            return false;
        }

        public virtual void Clear()
        {
            _entities.Clear();
        }
    }
}