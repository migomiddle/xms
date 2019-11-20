using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.RibbonButton.Abstractions;
using Xms.RibbonButton.Data;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮删除服务
    /// </summary>
    public class RibbonButtonDeleter : IRibbonButtonDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IRibbonButtonRepository _ribbonButtonRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IRibbonButtonDependency _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;

        public RibbonButtonDeleter(IRibbonButtonRepository ribbonButtonRepository
            , ILocalizedLabelService localizedLabelService
            , IRibbonButtonDependency dependencyService
            , IDependencyChecker dependencyChecker)
        {
            _ribbonButtonRepository = ribbonButtonRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = true;
            var deleteds = _ribbonButtonRepository.Query(x => x.RibbonButtonId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds, (deleted) =>
                {
                    _dependencyChecker.CheckAndThrow<Domain.RibbonButton>(RibbonButtonDefaults.ModuleName, deleted.RibbonButtonId);
                    return true;
                });
            }
            return result;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId);
            var buttons = _ribbonButtonRepository.Query(x => x.EntityId.In(entityIds));
            if (buttons.NotEmpty())
            {
                DeleteCore(buttons, null);
            }
        }

        private bool DeleteCore(IEnumerable<Domain.RibbonButton> deleteds, Func<Domain.RibbonButton, bool> validation)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            foreach (var deleted in deleteds)
            {
                result = validation?.Invoke(deleted) ?? true;
            }
            if (result)
            {
                var ids = deleteds.Select(x => x.RibbonButtonId).ToArray();
                using (UnitOfWork.Build(_ribbonButtonRepository.DbContext))
                {
                    result = _ribbonButtonRepository.DeleteMany(ids);
                    //删除依赖项
                    _dependencyService.Delete(ids);
                    //localization
                    _localizedLabelService.DeleteByObject(ids);
                }
            }
            return result;
        }
    }
}