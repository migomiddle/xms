using System;
using System.Collections.Generic;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Form;
using Xms.Form.Abstractions.Component;
using Xms.Form.Domain;
using Xms.Identity;
using Xms.Organization;
using Xms.QueryView;
using Xms.Schema.Entity;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;

namespace Xms.Sdk.Client.AggRoot
{
    public class AggFinder : DataProviderBase, IAggFinder
    {
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly IOrganizationDataRetriever _organizationDataRetriever;
        private readonly IDataFinder _dataFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IFetchDataService _fetchService;

        private AggregateRoot _aggregateRoot;

        public ICurrentUser User { get; set; }

        public AggFinder(
            IAppContext appContext
            , IDataFinder dataFinder
            , ISystemFormFinder systemFormFinder
            , IEntityFinder entityFinder
            , IRelationShipFinder relationShipFinder
            , IQueryViewFinder queryViewFinder
            , IFetchDataService fetchDataService

            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IOrganizationDataRetriever organizationDataRetriever
            )
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataRetriever = organizationDataRetriever;
            _systemFormFinder = systemFormFinder;
            _relationShipFinder = relationShipFinder;
            _queryViewFinder = queryViewFinder;
            _fetchService = fetchDataService;

            _dataFinder = dataFinder;
            User = _appContext.GetFeature<ICurrentUser>();

            _aggregateRoot = new AggregateRoot();
        }

        public AggregateRoot Retrieve(QueryBase request, bool ignorePermissions = false)
        {
            //查询单个实体，查询列表

            //var entity = args.EntityId.Equals(Guid.Empty) ? _entityFinder.FindByName(args.EntityName) : _entityFinder.FindById(args.EntityId);

            //var record = _dataFinder.RetrieveById(entity.Name, args.RecordId.Value);

            string entityname = "";
            Guid? recordId = null;
            Guid? formId = null;

            _aggregateRoot.MainEntity = _dataFinder.RetrieveById(entityname, recordId.Value);

            //表单列表
            SystemForm formEntity = null;
            formEntity = _systemFormFinder.FindById(formId.Value);
            FormBuilder formBuilder = new FormBuilder(formEntity.FormConfig);

            List<PanelDescriptor> panelDescriptors = formBuilder.Form.Panels;

            foreach (var panel in panelDescriptors)
            {
                foreach (var section in panel.Sections)
                {
                    foreach (var row in section.Rows)
                    {
                        foreach (var cell in row.Cells)
                        {
                            if (cell.Control.ControlType == FormControlType.SubGrid)
                            {
                                var param = (SubGridParameters)cell.Control.Parameters;

                                //param.ViewId;
                                //param.RelationshipName

                                var queryView = _queryViewFinder.FindById(Guid.Parse(param.ViewId));
                                if (queryView != null)
                                {
                                    //if (!queryView.IsDefault && queryView.IsAuthorization)
                                    {
                                    }

                                    FetchDescriptor fetch = new FetchDescriptor
                                    {
                                        //Page = model.Page,
                                        //PageSize = model.PageSize,
                                        //FetchConfig = queryView.FetchConfig,
                                        //GetAll = !model.PagingEnabled
                                    };

                                    //排序，过滤
                                    var relationship = _relationShipFinder.FindByName(param.RelationshipName);
                                    var filter = new FilterExpression();
                                    var condition = new ConditionExpression(relationship.ReferencingAttributeName, ConditionOperator.Equal, recordId.Value);
                                    filter.AddCondition(condition);
                                    fetch.Filter = filter;
                                    fetch.User = User;
                                    var datas = _fetchService.Execute(fetch);
                                    //_aggregateRoot.grids.Add("", datas.Items);
                                }
                            }
                        }
                    }
                }
            }
            return _aggregateRoot;
        }
    }
}