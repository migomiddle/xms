using System;
using System.Linq;
using Xms.Dependency;
using Xms.Form.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Abstractions;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;

namespace Xms.Form
{
    /// <summary>
    /// 表单依赖服务
    /// </summary>
    public class SystemFormDependency : ISystemFormDependency
    {
        private readonly IDependencyService _dependencyService;
        private readonly IFormService _formService;

        public SystemFormDependency(IDependencyService dependencyService
            , IFormService formService)
        {
            _dependencyService = dependencyService;
            _formService = formService;
        }

        public bool Create(Domain.SystemForm entity)
        {
            if (entity.FormType == (int)FormType.Main)
            {
                _formService.Init(entity);
                var attrIds = _formService.AttributeMetaDatas.Select(x => x.AttributeId).ToArray();
                if (attrIds.NotEmpty())
                {
                    //依赖于字段
                    _dependencyService.Create(FormDefaults.ModuleName, entity.SystemFormId, AttributeDefaults.ModuleName, attrIds);
                }
                if (entity.IsCustomButton && entity.CustomButtons.IsNotEmpty())
                {
                    var buttonIds = new Guid[] { }.DeserializeFromJson(entity.CustomButtons);
                    if (buttonIds.NotEmpty())
                    {
                        //依赖于按钮
                        _dependencyService.Create(FormDefaults.ModuleName, entity.SystemFormId, RibbonButtonDefaults.ModuleName, buttonIds);
                    }
                }
                //依赖于视图(单据体)
                var queryViewIds = entity.GetQueryViewIds();
                if (queryViewIds.NotEmpty())
                {
                    _dependencyService.Create(FormDefaults.ModuleName, entity.SystemFormId, QueryViewDefaults.ModuleName, queryViewIds.ToArray());
                }
            }
            return true;
        }

        public bool Update(Domain.SystemForm entity)
        {
            if (entity.FormType == (int)FormType.Main)
            {
                _formService.Init(entity);
                //依赖于字段
                _dependencyService.Update(FormDefaults.ModuleName, entity.SystemFormId, AttributeDefaults.ModuleName, _formService.AttributeMetaDatas.Select(x => x.AttributeId).ToArray());
                //依赖于按钮
                if (entity.IsCustomButton && entity.CustomButtons.IsNotEmpty())
                {
                    var buttonIds = new Guid[] { }.DeserializeFromJson(entity.CustomButtons);
                    if (buttonIds.NotEmpty())
                    {
                        _dependencyService.Update(FormDefaults.ModuleName, entity.SystemFormId, RibbonButtonDefaults.ModuleName, buttonIds);
                    }
                }
                //依赖于视图(单据体)
                var queryViewIds = entity.GetQueryViewIds();
                if (queryViewIds.NotEmpty())
                {
                    _dependencyService.Update(FormDefaults.ModuleName, entity.SystemFormId, QueryViewDefaults.ModuleName, queryViewIds.ToArray());
                }
            }
            return true;
        }

        public bool Delete(params Guid[] id)
        {
            return _dependencyService.DeleteByDependentId(FormDefaults.ModuleName, id); ;
        }
    }
}