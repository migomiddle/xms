using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 实体数据校验
    /// </summary>
    public class EntityValidator : IEntityValidator
    {
        private readonly IEntityFinder _entityFinder;

        public EntityValidator(IEntityFinder entityFinder)
        {
            _entityFinder = entityFinder;
        }

        /// <summary>
        /// 校验各字段值格式
        /// </summary>
        /// <param name="entity">实体记录</param>
        /// <param name="entityMetadata">实体元数据</param>
        /// <param name="attributeMetadatas">字段元数据</param>
        /// <param name="onError">失败时执行的回调</param>
        public void VerifyValues(Entity entity, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas, Action<string> onError)
        {
            List<string> errors = new List<string>();
            foreach (var item in entity)
            {
                var value = item.Value;
                if (value == null)
                {
                    continue;
                }
                var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(item.Key));
                if (attr != null)
                {
                    if (attr.TypeIsBit() || attr.TypeIsState())
                    {
                        //是否存在该值
                        if (!(value is bool) && !value.ToString().IsInteger())
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'bool' or integer", attr.Name, value.ToString()));
                        }
                    }
                    else if (attr.TypeIsLookUp())
                    {
                        if (value is EntityReference)
                        {
                            var er = value as EntityReference;
                            //是否存在该引用实体
                            if (!_entityFinder.Exists(er.ReferencedEntityName))
                            {
                                errors.Add(string.Format("referenced entity '{0}' is not found by attribute '{1}'", er.ReferencedEntityName, attr.Name));
                            }
                            //是否存在该值
                            //var referencedRecord =
                        }
                        else
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.EntityReference'", attr.Name, value.ToString()));
                        }
                    }
                    else if (attr.TypeIsPickList() || attr.TypeIsStatus())
                    {
                        //是否正确的格式
                        if (!(value is OptionSetValue) && !value.ToString().IsInteger())
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.OptionSetValue' or integer", attr.Name, value.ToString()));
                        }
                        //是否存在该值
                    }
                    else if (attr.TypeIsOwner())
                    {
                        //是否正确的格式
                        if (!(value is OwnerObject))
                        {
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.OwnerObject'", attr.Name, value.ToString()));
                        }
                        //是否存在该值
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
                            errors.Add(string.Format("{0}'s value({1}) is not valid, it's type should be 'SDK.Money'", attr.Name, value.ToString()));
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
                else
                {
                    errors.Add(string.Format("attribute with name'{0}' is not found", item.Key, value.ToString()));
                }
            }
            if (errors.NotEmpty())
            {
                onError(string.Join("\n", errors));
            }
        }
    }
}