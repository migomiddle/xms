using System;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Extensions
{
    /// <summary>
    /// 字段元数据扩展方法
    /// </summary>
    public static class AttributeExtensions
    {
        public static bool TypeIsBit(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.BIT);
        }

        public static bool TypeIsInt(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.INT);
        }

        public static bool TypeIsSmallInt(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.SMALLINT);
        }

        public static bool TypeIsFloat(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.FLOAT);
        }

        public static bool TypeIsDecimal(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.DECIMAL);
        }

        public static bool TypeIsMoney(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.MONEY);
        }

        public static bool TypeIsSmallMoney(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.SMALLMONEY);
        }

        public static bool TypeIsNvarchar(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.NVARCHAR);
        }

        public static bool TypeIsVarchar(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.VARCHAR);
        }

        public static bool TypeIsChar(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.CHAR);
        }

        public static bool TypeIsPickList(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.PICKLIST);
        }

        public static bool TypeIsOwner(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.OWNER);
        }

        public static bool TypeIsCustomer(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.CUSTOMER);
        }

        public static bool TypeIsPrimaryKey(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.PRIMARYKEY);
        }

        public static bool TypeIsState(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.STATE);
        }

        public static bool TypeIsStatus(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.STATUS);
        }

        public static bool TypeIsText(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.TEXT);
        }

        public static bool TypeIsNText(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.NTEXT);
        }

        public static bool TypeIsUniqueidentifier(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.UNIQUEIDENTIFIER);
        }

        public static bool TypeIsDateTime(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.DATETIME);
        }

        public static bool TypeIsSmallDateTime(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.SMALLDATETIME);
        }

        public static bool TypeIsLookUp(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.LOOKUP);
        }

        public static bool TypeIsPartyList(this Domain.Attribute attr)
        {
            return attr.AttributeTypeName.IsCaseInsensitiveEqual(AttributeTypeIds.PARTYLIST);
        }

        public static string GetNameField(this Domain.Attribute attr, string alias = "")
        {
            if (attr == null)
            {
                return string.Empty;
            }

            var field = alias.IsNotEmpty() ? alias : attr.Name;
            if (attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer() || attr.TypeIsState() || attr.TypeIsBit() || attr.TypeIsPickList() || attr.TypeIsStatus())
            {
                field += "name";
            }
            return field;
        }

        public static bool IsSystemControl(this Domain.Attribute attr)
        {
            return AttributeDefaults.SystemAttributes.Contains(attr.Name, StringComparer.InvariantCultureIgnoreCase) && !attr.TypeIsPrimaryKey();
        }
    }
}