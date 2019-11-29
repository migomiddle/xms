using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xms.Sdk.Abstractions.Query;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class AttributeModel : BasePaged<Schema.Domain.Attribute>
    {
        public string[] AttributeTypeName { get; set; }
        public Guid? AttributeTypeId { get; set; }
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public int Length { get; set; }
        public bool IsNullable { get; set; }
        public Guid EntityId { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public bool IsLoged { get; set; }
        public int? ReferencedEntityObjectTypeCode { get; set; }
        public Guid? OptionSetId { get; set; }

        public bool FilterSysAttribute { get; set; }

        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

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
        public bool LogEnabled { get; set; }
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

        public string LookupType { get; set; }

        //part list setting
        public string PartyListFormat { get; set; }

        public Schema.Domain.Entity Entity { get; set; }

        public Schema.Domain.Attribute Attribute { get; set; }
        public Guid SolutionId { get; set; }
        public bool AuthorizationEnabled { get; set; }
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
        public bool LogEnabled { get; set; }
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

        public string LookupType { get; set; }

        //part list setting
        public string PartyListFormat { get; set; }

        public Schema.Domain.Entity Entity { get; set; }

        public Schema.Domain.Attribute Attribute { get; set; }
        public Guid SolutionId { get; set; }
        public bool AuthorizationEnabled { get; set; }

        public string Description { get; set; }
        public string FormulaExpression { get; set; }
        public int ValueType { get; set; }
        public Guid SummaryEntityId { get; set; }
        public string SummaryExpression { get; set; }
        public AttributeAggregateExpression AaExp { get; set; }
    }
}