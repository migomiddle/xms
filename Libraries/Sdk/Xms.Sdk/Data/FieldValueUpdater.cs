using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Data;
using Xms.Data;
using Xms.DataMapping.Abstractions;
using Xms.DataMapping.Domain;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Data
{
    /// <summary>
    /// 字段值更新
    /// </summary>
    public class FieldValueUpdater : IFieldValueUpdater
    {
        private readonly DataRepositoryBase<dynamic> _repository;
        private readonly ILocalizedTextProvider _loc;

        public FieldValueUpdater(ILocalizedTextProvider localizedTextProvider
            , IDbContext dbContext)
        {
            _loc = localizedTextProvider;
            _repository = new DataRepositoryBase<dynamic>(dbContext);
        }

        public bool UpdateControlMap(EntityMap entityMap, IEnumerable<AttributeMap> attributeMaps, Guid sourceRecordId, Schema.Domain.Attribute refAttribute, IEnumerable<Schema.Domain.Attribute> sourceAttributeMeta, IEnumerable<Schema.Domain.Attribute> targetAttributeMeta, bool onDelete = false)
        {
            Sql s = Sql.Builder.Append("UPDATE [" + entityMap.SourceEnttiyName + "] SET ");
            foreach (var ca in attributeMaps)
            {
                //查找所有目标单据，钩稽字段的总和
                s.Append(string.Format("[{0}]=(SELECT SUM([{1}]) AS SumValue FROM [{2}] WHERE [{3}] = '{4}')"
                    , ca.RemainAttributeName, ca.TargetAttributeName, entityMap.TargetEnttiyName, refAttribute.Name, sourceRecordId));
                s.Append(string.Format(",[{0}]=(CASE WHEN ([{1}]-(SELECT SUM([{2}]) AS SumValue FROM [{3}] WHERE [{4}] = '{5}'))<=0 THEN 1 ELSE 0 END)"
                    , ca.ClosedAttributeName, ca.SourceAttributeName, ca.TargetAttributeName, entityMap.TargetEnttiyName, refAttribute.Name, sourceRecordId));
                if (!onDelete && entityMap.MapType == MapType.ForceControl)
                {
                    var data = _repository.ExecuteQuery(string.Format("SELECT 1 FROM [{0}] a LEFT JOIN (SELECT SUM([{1}]) AS SumValue,[{3}] FROM [{2}] WHERE [{3}] = '{4}' GROUP BY [{3}]) b ON a.{0}id=b.[{3}] WHERE a.{0}id='{4}' AND b.SumValue>a.[{5}]"
                        , entityMap.SourceEnttiyName, ca.TargetAttributeName, entityMap.TargetEnttiyName, refAttribute.Name, sourceRecordId, ca.SourceAttributeName));
                    if (data.NotEmpty())
                    {
                        var attr = sourceAttributeMeta.First(n => n.AttributeId.Equals(ca.SourceAttributeId));
                        var attr2 = targetAttributeMeta.First(n => n.AttributeId.Equals(ca.TargetAttributeId));
                        _repository.RollBackTransaction();
                        throw new XmsException(string.Format("'{0}' " + _loc["entitymap_greaterthan_error"] + " '{1}'", attr2.LocalizedName, attr.LocalizedName));
                    }
                }
            }
            s.Append(" WHERE " + entityMap.SourceEnttiyName + "id='" + sourceRecordId + "'");
            _repository.Execute(s);
            return true;
        }

        public bool UpdateSummaryValue(Schema.Domain.Entity entityMetadata, Entity data, Schema.Domain.Attribute summaryField, Schema.Domain.RelationShip relationShipMetadata, AttributeAggregateExpression aggExp)
        {
            var sql = string.Format("UPDATE {0} SET {1}=(SELECT {6}({2}) FROM {3} WITH(NOLOCK) WHERE {4}='{5}') WHERE {4} = '{5}'"
                            , summaryField.EntityName, summaryField.Name, aggExp.Field, aggExp.EntityName
                            , relationShipMetadata.ReferencedAttributeName, data[relationShipMetadata.ReferencingAttributeName].ToString()
                            , aggExp.Aggregate);
            _repository.Execute(sql, null);
            return true;
        }

        public bool UpdateFormulaValue(string entityName, IEnumerable<Schema.Domain.Attribute> attributes, Guid recordId)
        {
            var sql = string.Format("UPDATE [{0}] SET ", entityName);
            var sets = new List<string>();
            foreach (var attr in attributes)
            {
                if (attr.FormulaExpression.IsNotEmpty())
                {
                    sets.Add(string.Format("[{0}]={1}", attr.Name, attr.FormulaExpression));
                }
            }
            if (sets.NotEmpty())
            {
                sql += string.Join(",", sets);
                sql += string.Format(" WHERE [{0}]=@0", attributes.First(x => x.TypeIsPrimaryKey()).Name.IfEmpty($"{entityName}id"));
                _repository.Execute(sql, recordId);
            }
            return true;
        }
    }
}