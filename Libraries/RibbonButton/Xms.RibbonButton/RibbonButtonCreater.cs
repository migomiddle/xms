using System;
using System.Collections.Generic;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.RibbonButton.Abstractions;
using Xms.RibbonButton.Data;
using Xms.Solution.Abstractions;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮创建服务
    /// </summary>
    public class RibbonButtonCreater : IRibbonButtonCreater
    {
        private readonly IRibbonButtonRepository _ribbonButtonRepository;
        private readonly IDefaultButtonProvider _defaultButtonProvider;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;
        private readonly IRibbonButtonDependency _dependencyService;

        public RibbonButtonCreater(IRibbonButtonRepository ribbonButtonRepository
            , IDefaultButtonProvider defaultButtonProvider
            , ILocalizedLabelBatchBuilder localizedLabelService
            , IRibbonButtonDependency dependencyService)
        {
            _ribbonButtonRepository = ribbonButtonRepository;
            _defaultButtonProvider = defaultButtonProvider;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
        }

        public bool Create(Domain.RibbonButton entity)
        {
            return CreateCore(entity);
        }

        public bool CreateDefaultButtons(Schema.Domain.Entity entity)
        {
            var defaultButtons = _defaultButtonProvider.Get(entity.EntityMask);
            defaultButtons.ForEach((b) =>
            {
                b.RibbonButtonId = Guid.NewGuid();
                b.CreatedBy = entity.CreatedBy;
                b.EntityId = entity.EntityId;
                b.SolutionId = entity.SolutionId;
                b.ComponentState = entity.ComponentState;
            });
            return CreateMany(defaultButtons);
        }

        public bool CreateMany(List<Domain.RibbonButton> entities)
        {
            return CreateCore(entities.ToArray());
        }

        private bool CreateCore(params Domain.RibbonButton[] entities)
        {
            Guard.NotEmpty(entities, nameof(entities));
            foreach (var entity in entities)
            {
                entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            }
            var result = true;
            using (UnitOfWork.Build(_ribbonButtonRepository.DbContext))
            {
                result = _ribbonButtonRepository.CreateMany(entities);
                _dependencyService.Create(entities);
                //本地化标签
                foreach (var entity in entities)
                {
                    _localizedLabelService.Append(entity.SolutionId, entity.Label.IfEmpty(""), RibbonButtonDefaults.ModuleName, "LocalizedName", entity.RibbonButtonId)
                    .Append(entity.SolutionId, entity.Description.IfEmpty(""), RibbonButtonDefaults.ModuleName, "Description", entity.RibbonButtonId);
                }
                _localizedLabelService.Save();
            }
            return result;
        }
    }
}