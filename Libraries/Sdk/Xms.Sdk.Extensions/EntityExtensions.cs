using System;
using System.Collections;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions;

namespace Xms.Sdk.Extensions
{
    public static class EntityExtensions
    {
        public static T GetValue<T>(this Entity entity, string attributeName, object defaultValue = null)
        {
            if (entity.ContainsKey(attributeName))
            {
                var value = entity[attributeName];
                if (value != null && value.ToString().IsNotEmpty())
                {
                    var newvalue = value.ChangeType(typeof(T));
                    return (T)newvalue;
                }
            }
            if (defaultValue == null)
            {
                return default(T);
            }
            else
            {
                return (T)defaultValue;
            }
        }

        public static string GetStringValue(this Entity entity, string attributeName)
        {
            return entity.ContainsKey(attributeName) && entity[attributeName] != null ? entity[attributeName].ToString() : string.Empty;
        }

        public static int GetIntValue(this Entity entity, string attributeName, int defaultValue = 0)
        {
            return entity.GetValue<int>(attributeName, defaultValue);
        }

        public static bool GetBoolValue(this Entity entity, string attributeName)
        {
            return entity.GetValue<bool>(attributeName);
        }

        public static decimal GetDecimalValue(this Entity entity, string attributeName)
        {
            return entity.GetValue<decimal>(attributeName);
        }

        public static DateTime GetDateValue(this Entity entity, string attributeName)
        {
            return entity.GetValue<DateTime>(attributeName);
        }

        public static Guid GetGuidValue(this Entity entity, string attributeName)
        {
            return entity.GetValue<Guid>(attributeName);
        }

        public static void RemoveKeys(this Entity entity, params string[] name)
        {
            Guard.NotNullOrEmpty(name, "name");
            foreach (var item in name)
            {
                entity.Remove(item);
            }
        }

        public static object WrapAttributeValue(this Entity entity, IEntityFinder entityFinder, Schema.Domain.Attribute attr, object value)
        {
            List<string> errors = new List<string>();

            if (value == null || value.ToString().IsEmpty())
            {
                return null;
            }
            if (attr != null)
            {
                if (attr.TypeIsBit())
                {
                    if (value.ToString().IsInteger())// if value is 0/1
                    {
                        value = value.ToString() == "1";
                    }
                    else
                    {
                        var boolValue = true;
                        if (bool.TryParse(value.ToString(), out boolValue))
                        {
                            value = boolValue;
                        }
                        else//if (!(value is bool))
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'bool'", attr.Name, value.ToString()));
                        }
                    }
                }
                else if (attr.TypeIsLookUp())
                {
                    if (value is EntityReference)
                    {
                        var er = value as EntityReference;
                        //是否存在该引用实体
                        if (!entityFinder.Exists(er.ReferencedEntityName))
                        {
                            errors.Add(string.Format("referenced entity '{0}' is not found by attribute '{1}'", er.ReferencedEntityName, attr.Name));
                        }
                    }
                    else
                    {
                        if (value.ToString().IsGuid())
                        {
                            var referencedEntity = entityFinder.FindById(attr.ReferencedEntityId.Value);
                            value = new EntityReference(referencedEntity.Name, Guid.Parse(value.ToString()));
                        }
                        else
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.EntityReference'", attr.Name, value.ToString()));
                        }
                    }
                }
                else if (attr.TypeIsPickList())
                {
                    //是否正确的格式
                    if (!(value is OptionSetValue))
                    {
                        if (value.ToString().IsInteger())
                        {
                            value = new OptionSetValue(int.Parse(value.ToString()));
                        }
                        else
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.OptionSetValue'", attr.Name, value.ToString()));
                        }
                    }
                    //是否存在该值
                }
                else if (attr.TypeIsOwner())
                {
                    //是否正确的格式
                    if (!(value is OwnerObject))
                    {
                        if (value.ToString().IsGuid())
                        {
                            value = new OwnerObject(OwnerTypes.SystemUser, Guid.Parse(value.ToString()));
                        }
                        else
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.OwnerObject'", attr.Name, value.ToString()));
                        }
                    }
                    //是否存在该值
                }
                else if (attr.TypeIsState())
                {
                    if (value.ToString().IsInteger())// if value is 0/1
                    {
                        value = value.ToString() == "1";
                    }
                    else
                    {
                        var boolValue = true;
                        if (bool.TryParse(value.ToString(), out boolValue))
                        {
                            value = boolValue;
                        }
                        else//if (!(value is bool))
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'bool'", attr.Name, value.ToString()));
                        }
                    }
                }
                else if (attr.TypeIsInt())
                {
                    //是否正确的格式
                    if (!value.ToString().IsInteger())
                    {
                        errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'int'", attr.Name, value.ToString()));
                    }
                }
                else if (attr.TypeIsFloat())
                {
                    //是否正确的格式
                    if (!value.ToString().IsNumeric())
                    {
                        errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'float'", attr.Name, value.ToString()));
                    }
                }
                else if (attr.TypeIsMoney())
                {
                    //是否正确的格式
                    if (!(value is Money))
                    {
                        if (value.ToString().IsNumeric())
                        {
                            value = new Money(decimal.Parse(value.ToString()));
                        }
                        else
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.Money'", attr.Name, value.ToString()));
                        }
                    }
                }
                else if (attr.TypeIsDateTime())
                {
                    //是否正确的格式
                    if (!value.ToString().IsDateTime())
                    {
                        errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'datetime'", attr.Name, value.ToString()));
                    }
                }
            }
            if (errors.NotEmpty())
            {
                throw new XmsException(string.Join("\n", errors));
            }
            return value;
        }

        public static Entity UnWrapAttributeValue(this Entity entity)
        {
            Entity result = new Entity(entity.Name);
            foreach (var item in entity)
            {
                result.Add(item.Key, item.Value);
            }
            ArrayList keys = new ArrayList(result.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                var k = keys[i].ToString();
                object value = result[k];
                if (value is bool)
                {
                    result[k] = ((bool)value) ? 1 : 0;
                }
                else if (value is EntityReference)
                {
                    result[k] = (value as EntityReference).ReferencedValue;
                }
                else if (value is OptionSetValue)
                {
                    result[k] = (value as OptionSetValue).Value;
                }
                else if (value is Money)
                {
                    result[k] = (value as Money).Value;
                }
                else if (value is OwnerObject)
                {
                    var o = (value as OwnerObject);
                    result[k] = o.OwnerId;
                    result["owneridtype"] = o.OwnerType;
                }
            }
            return result;
        }

        public static object UnWrapAttributeValue(this Entity entity, Schema.Domain.Attribute attr)
        {
            object value = null;
            if (entity.ContainsKey(attr.Name))
            {
                var obj = entity[attr.Name];
                if (obj is EntityReference)
                {
                    value = (obj as EntityReference).ReferencedValue;
                }
                else if (obj is Money)
                {
                    value = (obj as Money).Value;
                }
                else if (obj is OptionSetValue)
                {
                    value = (obj as OptionSetValue).Value;
                }
                else if (obj is OwnerObject)
                {
                    value = (obj as OwnerObject).OwnerId;
                }
                else if (obj is bool)
                {
                    value = (bool.Parse(obj.ToString())) ? 1 : 0;
                }
                else
                {
                    value = obj;
                }
            }
            return value;
        }
    }
}