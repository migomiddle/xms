using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Xms.Authorization.Abstractions;
using Xms.Business.DataAnalyse.Data;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Dependency;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Module.Core;
using Xms.Schema.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;
using Xms.Solution;

namespace Xms.Business.DataAnalyse.Report
{
    /// <summary>
    /// 报表服务
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IDependencyService _dependencyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAppContext _appContext;

        public ReportService(IAppContext appContext
            , IReportRepository reportRepository
            , ILocalizedLabelService localizedLabelService
            , ISolutionComponentService solutionComponentService
            , IRoleObjectAccessService roleObjectAccessService
            , IDependencyService dependencyService
            , IEventPublisher eventPublisher)
        {
            _appContext = appContext;
            _reportRepository = reportRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _roleObjectAccessService = roleObjectAccessService;
            _dependencyService = dependencyService;
            _eventPublisher = eventPublisher;
        }

        public bool Create(Domain.Report entity)
        {
            entity.OrganizationId = _appContext.OrganizationId;
            var result = true;
            using (UnitOfWork.Build(_reportRepository.DbContext))
            {
                result = _reportRepository.Create(entity);
                //solution component
                _solutionComponentService.Create(entity.SolutionId, entity.ReportId, ReportDefaults.ModuleName);
                //依赖于实体
                _dependencyService.Create(ReportDefaults.ModuleName, entity.ReportId, EntityDefaults.ModuleName, entity.EntityId, entity.RelatedEntityId);
                //本地化标签
                _localizedLabelService.Create(entity.SolutionId, entity.Name.IfEmpty(""), ReportDefaults.ModuleName, "LocalizedName", entity.ReportId, this._appContext.BaseLanguage);
                _localizedLabelService.Create(entity.SolutionId, entity.Description.IfEmpty(""), ReportDefaults.ModuleName, "Description", entity.ReportId, this._appContext.BaseLanguage);
            }
            return result;
        }

        public bool Update(Domain.Report entity)
        {
            var original = this.FindById(entity.ReportId);
            var result = true;
            using (UnitOfWork.Build(_reportRepository.DbContext))
            {
                result = _reportRepository.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.ReportId, this._appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.ReportId, this._appContext.BaseLanguage);
                //assigning roles
                if (original.IsAuthorization || !entity.IsAuthorization)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = new List<Guid> { entity.ReportId }
                        ,
                        State = false
                        ,
                        ResourceName = ReportDefaults.ModuleName
                    });
                }
            }
            return result;
        }

        public bool UpdateAuthorization(IEnumerable<Guid> ids, bool isAuthorization)
        {
            var context = UpdateContextBuilder.Build<Domain.Report>();
            context.Set(f => f.IsAuthorization, isAuthorization);
            context.Where(f => f.ReportId.In(ids));
            var result = true;
            using (UnitOfWork.Build(_reportRepository.DbContext))
            {
                result = _reportRepository.Update(context);
                if (result && !isAuthorization)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = ids.ToList()
                        ,
                        State = isAuthorization
                        ,
                        ResourceName = ReportDefaults.ModuleName
                    });
                }
                return result;
            }
        }

        public Domain.Report FindById(Guid id)
        {
            var data = _reportRepository.FindById(id);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public Domain.Report Find(Expression<Func<Domain.Report, bool>> predicate)
        {
            var data = _reportRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public bool DeleteById(Guid id)
        {
            var entity = _reportRepository.FindById(id);
            if (entity == null)
            {
                return false;
            }
            var result = true;
            using (UnitOfWork.Build(_reportRepository.DbContext))
            {
                result = _reportRepository.DeleteById(id);
                //delete assigned roles
                _roleObjectAccessService.DeleteByObjectId(id, ReportDefaults.ModuleName);
                //solution component
                _solutionComponentService.DeleteObject(entity.SolutionId, entity.ReportId, ReportDefaults.ModuleName);
                //localization
                _localizedLabelService.DeleteByObject(id);
            }
            return result;
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            bool flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
        }

        public PagedList<Domain.Report> QueryPaged(Func<QueryDescriptor<Domain.Report>, QueryDescriptor<Domain.Report>> container)
        {
            QueryDescriptor<Domain.Report> q = container(QueryDescriptorBuilder.Build<Domain.Report>());
            var datas = _reportRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.Report> QueryPaged(Func<QueryDescriptor<Domain.Report>, QueryDescriptor<Domain.Report>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Domain.Report> q = container(QueryDescriptorBuilder.Build<Domain.Report>());
            var datas = _reportRepository.QueryPaged(q, ModuleCollection.GetIdentity(ReportDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.Report> QueryAuthorized(Func<QueryDescriptor<Domain.Report>, QueryDescriptor<Domain.Report>> container)
        {
            QueryDescriptor<Domain.Report> q = container(QueryDescriptorBuilder.Build<Domain.Report>());
            var datas = _reportRepository.Query(q)?.ToList();
            if (datas.NotEmpty())
            {
                var authIds = datas.Where(x => x.IsAuthorization).Select(x => x.ReportId).ToArray();
                if (authIds.NotEmpty())
                {
                    var authorizedItems = _roleObjectAccessService.Authorized(ReportDefaults.ModuleName, authIds);
                    datas.RemoveAll(x => x.IsAuthorization && !authorizedItems.Contains(x.ReportId));
                }
            }

            WrapLocalizedLabel(datas);
            return datas;
        }

        private void WrapLocalizedLabel(IList<Domain.Report> datas)
        {
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.ReportId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this._appContext.BaseLanguage && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.ReportId, "LocalizedName", d.Name);
                    d.Description = _localizedLabelService.GetLabelText(labels, d.ReportId, "Description", d.Description);
                }
            }
        }

        private void WrapLocalizedLabel(Domain.Report entity)
        {
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this._appContext.BaseLanguage && f.ObjectId == entity.ReportId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.ReportId, "LocalizedName", entity.Name);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.ReportId, "Description", entity.Description);
        }

        public string GetFieldValueName(IQueryResolver queryTranslator, string field, Schema.Domain.Attribute attr = null)
        {
            return _reportRepository.GetFieldValueName(queryTranslator, field, attr);
        }

        public string GetGroupingName(ReportDescriptor report, IQueryResolver queryTranslator, string field, bool includeAlias = false)
        {
            return _reportRepository.GetGroupingName(report, queryTranslator, field, includeAlias);
        }

        public DataTable GetChartData(ReportDescriptor report, IQueryResolver queryTranslator)
        {
            return _reportRepository.GetChartData(report, queryTranslator);
        }

        public DataTable GetData(ReportDescriptor report, IQueryResolver queryTranslator, FilterExpression filter = null)
        {
            return _reportRepository.GetData(report, queryTranslator, filter);
        }
    }
}