using System;
using System.Linq;
using Xms.Dependency;
using Xms.Infrastructure.Utility;
using Xms.RibbonButton.Abstractions;
using Xms.WebResource.Abstractions;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮依赖服务
    /// </summary>
    public class RibbonButtonDependency : IRibbonButtonDependency
    {
        private readonly IDependencyService _dependencyService;
        private readonly IDependencyBatchBuilder _dependencyBatchBuilder;

        public RibbonButtonDependency(IDependencyService dependencyService
            , IDependencyBatchBuilder dependencyBatchBuilder)
        {
            _dependencyService = dependencyService;
            _dependencyBatchBuilder = dependencyBatchBuilder;
        }

        public bool Create(params Domain.RibbonButton[] entities)
        {
            var wsIds = entities.Where(x => x.JsLibrary.IsNotEmpty() && x.JsLibrary.StartsWith("$webresource:"))?.Select(x => new { x.RibbonButtonId, WebResourceId = new Guid(x.JsLibrary.Replace("$webresource:", "")) })?.ToList();

            if (wsIds.NotEmpty())
            {
                //依赖于web资源
                foreach (var item in wsIds)
                {
                    _dependencyBatchBuilder.Append(RibbonButtonDefaults.ModuleName, item.RibbonButtonId, WebResourceDefaults.ModuleName, item.WebResourceId);
                }
                _dependencyBatchBuilder.Save();
            }
            return true;
        }

        public bool Update(Domain.RibbonButton entity)
        {
            if (entity.JsLibrary.IsNotEmpty() && entity.JsLibrary.StartsWith("$webresource:"))
            {
                //依赖于web资源
                _dependencyService.Update(RibbonButtonDefaults.ModuleName, entity.RibbonButtonId, WebResourceDefaults.ModuleName, new Guid(entity.JsLibrary.Replace("$webresource:", "")));
            }

            return true;
        }

        public bool Delete(params Guid[] id)
        {
            return _dependencyService.DeleteByDependentId(RibbonButtonDefaults.ModuleName, id); ;
        }
    }
}