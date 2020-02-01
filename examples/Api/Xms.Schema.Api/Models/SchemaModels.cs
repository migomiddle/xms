using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xms.Schema.Abstractions;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Schema.Api.Models
{
    public class RetrieveEntityModel
    {
        public string[] Name { get; set; }
        public bool? LogEnabled { get; set; }
        public bool? IsCustomizable { get; set; }
        public bool? AuthorizationEnabled { get; set; }
        public bool? DuplicateEnabled { get; set; }
        public bool? WorkFlowEnabled { get; set; }
        public bool? BusinessFlowEnabled { get; set; }
        public Guid? SolutionId { get; set; }
    }
    public class RetrieveAttributesModel
    {
        public string[] AttributeTypeName { get; set; }
        public Guid EntityId { get; set; }
        public Guid SolutionId { get; set; }
        public bool FilterSysAttribute { get; set; }
    }


    public class CreateEntityModel
    {
        public Guid EntityId { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsLoged { get; set; }
        public bool IsAuthorization { get; set; }
        public bool WorkFlowEnabled { get; set; }
        public EntityMaskEnum EntityMask { get; set; }

        [Required]
        public string LocalizedName { get; set; }

        public bool DuplicateEnabled { get; set; }
        public Guid SolutionId { get; set; }
        public string Description { get; set; }
        public bool BusinessFlowEnabled { get; set; }
    }

    public class EditEntityModel
    {
        public Schema.Domain.Entity Entity { get; set; }
        public Guid EntityId { get; set; }
        public bool IsLoged { get; set; }
        public bool IsAuthorization { get; set; }
        public bool IsCustomizable { get; set; }
        public bool WorkFlowEnabled { get; set; }

        [Required]
        public string LocalizedName { get; set; }

        public bool DuplicateEnabled { get; set; }
        public Guid SolutionId { get; set; }
        public string Description { get; set; }
        public bool BusinessFlowEnabled { get; set; }

        public EntityMaskEnum EntityMask { get; set; }
    }


    #region AttributeModel
    public class CreateAttributeModel
    {
        public Guid? AttributeId { get; set; }

        [Required]
        public string AttributeType { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string LocalizedName { get; set; }

        public int Length { get; set; }
        public bool IsNullable { get; set; }
        public Guid EntityId { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public bool IsLoged { get; set; }
        public Guid? ReferencedEntityId { get; set; }
        //public int? ReferencedEntityObjectTypeCode { get; set; }
        //public Guid? OptionSetId { get; set; }
        //public SelectList AttributeTypes { get; set; }

        //int setting
        [Range(int.MinValue, int.MaxValue)]
        public int? IntMinValue { get; set; }

        [Range(int.MinValue, int.MaxValue)]
        public int? IntMaxValue { get; set; }

        //nvarchar setting
        public string TextFormat { get; set; }

        [Range(10, 4000)]
        public int? MaxLength { get; set; }
        //ntext setting
        public string NTextFormat { get; set; }

        //float setting
        [Range(0, 8)]
        public int? FloatPrecision { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? FloatMinValue { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? FloatMaxValue { get; set; }

        //money setting
        [Range(0, 8)]
        public int? MoneyPrecision { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? MoneyMinValue { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? MoneyMaxValue { get; set; }

        //optionset setting
        public string OptionSetType { get; set; }

        public bool IsCommonOptionSet { set; get; }
        public Guid? CommonOptionSet { set; get; }
        public List<string> OptionSetName { set; get; }
        public List<int> OptionSetValue { set; get; }
        public List<Guid> OptionSetDetailId { set; get; }
        public List<bool> IsSelectedOption { set; get; }

        //bit setting
        public List<string> BitOptionName { get; set; }

        public List<bool> BitIsDefault { get; set; }
        public List<Guid> BitDetailId { set; get; }

        //datetime setting
        public string DateTimeFormat { get; set; }

        //lookup setting
        public Guid? LookupEntity { get; set; }

        //part list setting
        public string PartyListFormat { get; set; }

        public Schema.Domain.Entity Entity { get; set; }

        public Schema.Domain.Attribute Attribute { get; set; }
        public Guid SolutionId { get; set; }
        public bool IsSecured { get; set; }
        public string Description { get; set; }

        public string FormulaExpression { get; set; }
        public int ValueType { get; set; }
        public Guid SummaryEntityId { get; set; }
        public string SummaryExpression { get; set; }
    }

    public class EditAttributeModel
    {
        public Guid? AttributeId { get; set; }

        public string AttributeType { get; set; }

        public string Name { get; set; }

        [Required]
        public string LocalizedName { get; set; }

        public int Length { get; set; }
        public bool IsNullable { get; set; }
        public Guid EntityId { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public bool IsLoged { get; set; }
        public bool IsPrimaryField { get; set; }
        public Guid? ReferencedEntityId { get; set; }

        //int setting
        [Range(int.MinValue, int.MaxValue)]
        public int? IntMinValue { get; set; }

        [Range(int.MinValue, int.MaxValue)]
        public int? IntMaxValue { get; set; }

        //nvarchar setting
        public string TextFormat { get; set; }

        [Range(10, 4000)]
        public int? MaxLength { get; set; }
        //ntext setting
        public string NTextFormat { get; set; }

        //float setting
        [Range(0, 8)]
        public int? FloatPrecision { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? FloatMinValue { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? FloatMaxValue { get; set; }

        //money setting
        [Range(0, 8)]
        public int? MoneyPrecision { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? MoneyMinValue { get; set; }

        [Range(float.MinValue, float.MaxValue)]
        public float? MoneyMaxValue { get; set; }

        //optionset setting
        public string OptionSetType { get; set; }

        public bool IsCommonOptionSet { set; get; }
        public Guid? CommonOptionSet { set; get; }
        public List<string> OptionSetName { set; get; }
        public List<int> OptionSetValue { set; get; }
        public List<Guid> OptionSetDetailId { set; get; }
        public List<bool> IsSelectedOption { set; get; }

        //bit setting
        public List<string> BitOptionName { get; set; }

        public List<bool> BitIsDefault { get; set; }
        public List<Guid> BitDetailId { set; get; }

        //datetime setting
        public string DateTimeFormat { get; set; }

        //lookup setting
        public Guid? LookupEntity { get; set; }

        //part list setting
        public string PartyListFormat { get; set; }

        public Schema.Domain.Entity Entity { get; set; }

        public Schema.Domain.Attribute Attribute { get; set; }
        public Guid SolutionId { get; set; }
        public bool IsSecured { get; set; }

        public string Description { get; set; }
        public string FormulaExpression { get; set; }
        public int ValueType { get; set; }
        public Guid SummaryEntityId { get; set; }
        public string SummaryExpression { get; set; }
        public AttributeAggregateExpression AaExp { get; set; }
    }
    #endregion

    #region OptionSet
    public class EditOptionSetModel
    {
        public Guid OptionSetId { get; set; }
        public string Name { get; set; }
        public List<string> OptionSetName { set; get; }
        public List<int> OptionSetValue { set; get; }
        public List<bool> IsSelectedOption { set; get; }
        public List<Guid> DetailId { get; set; }
        public List<Schema.Domain.OptionSetDetail> Details { get; set; }
        public Guid SolutionId { get; set; }
    }


    #endregion

    #region RelationShip

    public class EditRelationShipModel
    {
        public Guid RelationShipId { get; set; }
        public int CascadeLinkMask { get; set; }
        public int CascadeDelete { get; set; }
        public int CascadeAssign { get; set; }
        public int CascadeShare { get; set; }
        public int CascadeUnShare { get; set; }
        public Guid SolutionId { get; set; }
        public Schema.Domain.RelationShip RelationShipMeta { get; set; }
    }

    #endregion

    #region StringMap
    #endregion


}
