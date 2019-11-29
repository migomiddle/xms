using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.DataMapping.Domain;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Data
{
    public interface IFieldValueUpdater
    {
        /// <summary>
        /// 更新单据转换中源记录值
        /// </summary>
        /// <param name="entityMap"></param>
        /// <param name="attributeMaps"></param>
        /// <param name="sourceRecordId"></param>
        /// <param name="refAttribute"></param>
        /// <param name="sourceAttributeMeta"></param>
        /// <param name="targetAttributeMeta"></param>
        /// <param name="onDelete"></param>
        /// <returns></returns>
        bool UpdateControlMap(EntityMap entityMap, IEnumerable<AttributeMap> attributeMaps, Guid sourceRecordId, Schema.Domain.Attribute refAttribute, IEnumerable<Schema.Domain.Attribute> sourceAttributeMeta, IEnumerable<Schema.Domain.Attribute> targetAttributeMeta, bool onDelete = false);

        /// <summary>
        /// 更新汇总值
        /// </summary>
        /// <param name="entityMetadata"></param>
        /// <param name="data"></param>
        /// <param name="summaryField"></param>
        /// <param name="relationShipMetadata"></param>
        /// <param name="aggExp"></param>
        /// <returns></returns>
        bool UpdateSummaryValue(Schema.Domain.Entity entityMetadata, Entity data, Schema.Domain.Attribute summaryField, Schema.Domain.RelationShip relationShipMetadata, AttributeAggregateExpression aggExp);

        /// <summary>
        /// 更新公式值
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="attributes"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        bool UpdateFormulaValue(string entityName, IEnumerable<Schema.Domain.Attribute> attributes, Guid recordId);
    }
}