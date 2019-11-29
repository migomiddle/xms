using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 导入按钮服务
    /// </summary>
    public class RibbonButtonImporter : IRibbonButtonImporter
    {
        private readonly IRibbonButtonCreater _ribbonButtonCreater;
        private readonly IRibbonButtonUpdater _ribbonButtonUpdater;
        private readonly IRibbonButtonFinder _ribbonButtonFinder;
        private readonly IAppContext _appContext;

        public RibbonButtonImporter(IAppContext appContext
            , IRibbonButtonCreater ribbonButtonCreater
            , IRibbonButtonUpdater ribbonButtonUpdater
            , IRibbonButtonFinder ribbonButtonFinder)
        {
            _appContext = appContext;
            _ribbonButtonCreater = ribbonButtonCreater;
            _ribbonButtonUpdater = ribbonButtonUpdater;
            _ribbonButtonFinder = ribbonButtonFinder;
        }

        public bool Import(Guid solutionId, List<RibbonButtonXmlInfo> ribbonButtons)
        {
            if (ribbonButtons.NotEmpty())
            {
                foreach (var item in ribbonButtons)
                {
                    var entity = _ribbonButtonFinder.FindById(item.RibbonButtonId);
                    if (entity != null)
                    {
                        entity.CssClass = item.CssClass;
                        entity.DisplayOrder = item.DisplayOrder;
                        entity.Icon = item.Icon;
                        entity.JsAction = item.JsAction;
                        entity.JsLibrary = item.JsLibrary;
                        entity.Label = item.Label;
                        entity.ShowArea = item.ShowArea;
                        entity.StateCode = item.StateCode;
                        _ribbonButtonUpdater.Update(entity);
                    }
                    else
                    {
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.CreatedOn = DateTime.Now;
                        _ribbonButtonCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}