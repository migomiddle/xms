using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;

namespace Xms.Schema.Attribute
{
    /// <summary>
    /// 字段导入服务
    /// </summary>
    public class AttributeImporter : IAttributeImporter
    {
        private readonly IAttributeCreater _attributeCreater;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IAttributeUpdater _attributeUpdater;
        private readonly IAppContext _appContext;

        public AttributeImporter(IAppContext appContext
            , IAttributeCreater attributeCreater
            , IAttributeUpdater attributeUpdater
            , IAttributeFinder attributeFinder)
        {
            _appContext = appContext;
            _attributeCreater = attributeCreater;
            _attributeUpdater = attributeUpdater;
            _attributeFinder = attributeFinder;
        }

        public bool Import(Guid solutionId, Domain.Entity entity, List<Domain.Attribute> attributes)
        {
            if (attributes.NotEmpty())
            {
                foreach (var attr in attributes)
                {
                    attr.EntityId = entity.EntityId;
                    attr.EntityName = entity.Name;
                    var existAttr = _attributeFinder.Find(attr.EntityId, attr.Name);
                    var isCreate = existAttr == null;
                    if (!isCreate)
                    {
                        existAttr.DataFormat = attr.DataFormat;
                        existAttr.DefaultValue = attr.DefaultValue;
                        existAttr.DisplayStyle = attr.DisplayStyle;
                        existAttr.LogEnabled = attr.LogEnabled;
                        existAttr.IsRequired = attr.IsRequired;
                        existAttr.LocalizedName = attr.LocalizedName;
                        existAttr.MaxLength = attr.MaxLength;
                        existAttr.MaxValue = attr.MaxValue;
                        existAttr.MinValue = attr.MinValue;
                        existAttr.Precision = attr.Precision;
                        _attributeUpdater.Update(existAttr);
                    }
                    else
                    {
                        attr.IsCustomField = true;
                        attr.IsCustomizable = true;
                        attr.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        _attributeCreater.Create(attr);
                    }
                    //字段选项
                    //if (attr.TypeIsBit() || attr.TypeIsState())
                    //{
                    //    if (isCreate)
                    //    {
                    //        foreach (var s in attr.PickLists)
                    //        {
                    //            s.AttributeId = attr.AttributeId;
                    //            s.AttributeName = attr.Name;
                    //            s.EntityName = item.Name;
                    //        }
                    //        _stringMapService.CreateMany(attr.PickLists);
                    //    }
                    //    else
                    //    {
                    //        foreach (var s in attr.PickLists)
                    //        {
                    //            var smap = _stringMapService.FindById(s.StringMapId);
                    //            if (smap != null)
                    //            {
                    //                smap.Name = s.Name;
                    //                _stringMapService.Update(smap);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            return true;
        }
    }
}