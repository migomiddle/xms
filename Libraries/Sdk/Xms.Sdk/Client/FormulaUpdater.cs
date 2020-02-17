using System.Linq;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Extensions;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 公式值更新服务
    /// </summary>
    public class FormulaUpdater : IFormulaUpdater
    {
        private readonly IFieldValueUpdater _fieldValueUpdater;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;

        public FormulaUpdater(
            IFieldValueUpdater fieldValueUpdater
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            )
        {
            _fieldValueUpdater = fieldValueUpdater;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
        }

        public bool Update(Schema.Domain.Entity entityMetadata, Entity data)
        {
            //exists summary field?
            var summaryFields = _attributeFinder.Query(n => n.Where(f => f.SummaryEntityId == entityMetadata.EntityId));
            if (summaryFields.NotEmpty())
            {
                foreach (var item in summaryFields)
                {
                    var ae = new AttributeAggregateExpression();
                    ae = ae.DeserializeFromJson(item.SummaryExpression);
                    if (!data.ContainsKey(ae.Field))
                    {
                        continue;
                    }
                    if (ae.EntityName.IsCaseInsensitiveEqual(data.Name))
                    {
                        var rs = _relationShipFinder.FindByName(ae.RelationshipName);
                        _fieldValueUpdater.UpdateSummaryValue(entityMetadata, data, item, rs, ae);
                        //字段公式计算
                        var formulaAttributes = _attributeFinder.FindByEntityName(rs.ReferencedEntityName).Where(n => n.ValueType == 2).ToList();
                        if (formulaAttributes.NotEmpty())
                        {
                            formulaAttributes.Add(_attributeFinder.FindById(rs.ReferencedAttributeId));
                            _fieldValueUpdater.UpdateFormulaValue(rs.ReferencedEntityName, formulaAttributes, data.GetGuidValue(rs.ReferencedAttributeName));
                        }
                    }
                }
            }
            return true;
        }
    }
}