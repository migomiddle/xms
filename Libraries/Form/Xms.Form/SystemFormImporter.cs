using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;

namespace Xms.Form
{
    /// <summary>
    /// 表单导入服务
    /// </summary>
    public class SystemFormImporter : ISystemFormImporter
    {
        private readonly ISystemFormCreater _SystemFormCreater;
        private readonly ISystemFormUpdater _SystemFormUpdater;
        private readonly ISystemFormFinder _SystemFormFinder;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public SystemFormImporter(IAppContext appContext
            , ISystemFormCreater SystemFormCreater
            , ISystemFormUpdater SystemFormUpdater
            , ISystemFormFinder SystemFormFinder)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _SystemFormCreater = SystemFormCreater;
            _SystemFormUpdater = SystemFormUpdater;
            _SystemFormFinder = SystemFormFinder;
        }

        public bool Import(Guid solutionId, List<Domain.SystemForm> systemForms)
        {
            if (systemForms.NotEmpty())
            {
                foreach (var item in systemForms)
                {
                    var entity = _SystemFormFinder.FindById(item.SystemFormId);
                    if (entity != null)
                    {
                        entity.Description = item.Description;
                        entity.FormConfig = item.FormConfig;
                        entity.IsDefault = item.IsDefault;
                        entity.IsCustomButton = item.IsCustomButton;
                        entity.CustomButtons = item.CustomButtons;
                        entity.ModifiedBy = _currentUser.SystemUserId;
                        entity.ModifiedOn = DateTime.Now;
                        entity.Name = item.Name;
                        entity.PublishedOn = DateTime.Now;
                        entity.StateCode = item.StateCode;
                        _SystemFormUpdater.Update(entity, true);
                    }
                    else
                    {
                        item.ComponentState = 0;
                        item.SolutionId = solutionId;
                        item.PublishedOn = DateTime.Now;
                        item.IsCustomizable = true;
                        item.CreatedBy = _currentUser.SystemUserId;
                        _SystemFormCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}