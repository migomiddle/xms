using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集导入服务
    /// </summary>
    [SolutionImportNode("optionsets")]
    public class OptionSetImporter : ISolutionComponentImporter<Domain.OptionSet>
    {
        private readonly IOptionSetCreater _optionSetCreater;
        private readonly IOptionSetUpdater _optionSetUpdater;
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDetailCreater _optionSetDetailCreater;
        private readonly IOptionSetDetailUpdater _optionSetDetailUpdater;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IAppContext _appContext;

        public OptionSetImporter(IAppContext appContext
            , IOptionSetCreater optionSetCreater
            , IOptionSetUpdater optionSetUpdater
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailCreater optionSetDetailCreater
            , IOptionSetDetailUpdater optionSetDetailUpdater
            , IOptionSetDetailFinder optionSetDetailFinder)
        {
            _appContext = appContext;
            _optionSetCreater = optionSetCreater;
            _optionSetUpdater = optionSetUpdater;
            _optionSetFinder = optionSetFinder;
            _optionSetDetailCreater = optionSetDetailCreater;
            _optionSetDetailUpdater = optionSetDetailUpdater;
            _optionSetDetailFinder = optionSetDetailFinder;
        }

        public bool Import(Guid solutionId, IList<Domain.OptionSet> optionSets)
        {
            if (optionSets.NotEmpty())
            {
                foreach (var item in optionSets)
                {
                    foreach (var d in item.Items)
                    {
                        d.OptionSetId = item.OptionSetId;
                    }
                    var entity = _optionSetFinder.FindById(item.OptionSetId);
                    if (entity != null)
                    {
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.CreatedOn = DateTime.Now;
                        item.SolutionId = solutionId;
                        item.OrganizationId = _appContext.OrganizationId;
                        _optionSetUpdater.Update(item);
                        //_optionSetDetailService.DeleteByParentId(entity.OptionSetId);
                        //_optionSetDetailService.CreateMany(item.Items);
                        foreach (var d in item.Items)
                        {
                            var dd = _optionSetDetailFinder.Find(x => x.OptionSetId == item.OptionSetId && x.OptionSetDetailId == d.OptionSetDetailId);
                            if (dd != null)
                            {
                                dd.Name = d.Name;
                                dd.Value = d.Value;
                                _optionSetDetailUpdater.Update(dd);
                            }
                            else
                            {
                                d.OptionSetId = item.OptionSetId;
                                _optionSetDetailCreater.Create(d);
                            }
                        }
                    }
                    else
                    {
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.CreatedOn = DateTime.Now;
                        item.SolutionId = solutionId;
                        item.OrganizationId = _appContext.GetFeature<ICurrentUser>().OrganizationId;
                        _optionSetCreater.Create(item);
                        _optionSetDetailCreater.CreateMany(item.Items);
                    }
                }
            }
            return true;
        }
    }
}