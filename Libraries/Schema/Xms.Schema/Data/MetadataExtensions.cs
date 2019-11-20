using Xms.Schema.Abstractions;

namespace Xms.Schema.Data
{
    public static class MetadataExtensions
    {
        public static string GetDbType(this Domain.Attribute attr)
        {
            string result = string.Empty;
            switch (attr.AttributeTypeName)
            {
                case AttributeTypeIds.FLOAT:
                    result = string.Format("{0}(23,10)", AttributeTypeIds.DECIMAL);
                    break;

                case AttributeTypeIds.NVARCHAR:
                    result = string.Format("{0}({1})", AttributeTypeIds.NVARCHAR, attr.MaxLength);
                    break;

                case AttributeTypeIds.NTEXT:
                    result = AttributeTypeIds.NVARCHAR + "(MAX)";//AttributeTypeIds.NTEXT;
                    break;

                case AttributeTypeIds.VARCHAR:
                    result = string.Format("{0}({1})", AttributeTypeIds.VARCHAR, attr.MaxLength);
                    break;

                case AttributeTypeIds.CHAR:
                    result = string.Format("{0}({1})", AttributeTypeIds.CHAR, attr.MaxLength);
                    break;

                case AttributeTypeIds.NCHAR:
                    result = string.Format("{0}({1})", AttributeTypeIds.NCHAR, attr.MaxLength);
                    break;

                case AttributeTypeIds.PICKLIST:
                    result = AttributeTypeIds.INT;
                    break;

                case AttributeTypeIds.STATE:
                    result = AttributeTypeIds.INT;
                    break;

                case AttributeTypeIds.STATUS:
                    result = AttributeTypeIds.INT;
                    break;

                case AttributeTypeIds.CUSTOMER:
                case AttributeTypeIds.OWNER:
                case AttributeTypeIds.PRIMARYKEY:
                case AttributeTypeIds.LOOKUP:
                    result = AttributeTypeIds.UNIQUEIDENTIFIER;
                    break;

                default:
                    result = attr.AttributeTypeName;
                    break;
            }
            return result;
        }
    }
}