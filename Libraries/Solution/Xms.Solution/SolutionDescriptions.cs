using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Xms.Core;
using Xms.Core.Domain.Business;
using Xms.Core.Domain.Dev;
using Xms.Core.Domain.Flow;
using Xms.Core.Domain.Schema;
using Xms.Core.Domain.Security;

namespace Xms.Solution
{
    /// <summary>
    /// 解决方案描述
    /// </summary>
    public class ImportExportXml
    {
        public List<IEntityTable> Entities { get; set; }

        public List<ISystemFormTable> SystemForms { get; set; }

        public List<RelationShipXmlInfo> RelationShips { get; set; }

        public List<IQueryViewTable> QueryViews { get; set; }

        public List<RibbonButtonXmlInfo> RibbonButtons { get; set; }

        public List<IChartTable> Charts { get; set; }

        public List<IOptionSetTable> OptionSets { get; set; }

        public List<IEntityPluginTable> EntityPlugins { get; set; }
        public List<ISerialNumberRuleTable> SerialNumberRules { get; set; }
        public List<EntityMapXmlInfo> EntityMaps { get; set; }

        public List<IWebResourceTable> WebResources { get; set; }

        public List<IDuplicateRuleTable> DuplicateRules { get; set; }
        public List<IWorkFlowTable> WorkFlows { get; set; }

        public List<IReportTable> Reports { get; set; }

        public List<ISystemFormTable> Dashboards { get; set; }

        public List<IRoleTable> Roles { get; set; }

        public List<IPrivilegeTable> Privileges { get; set; }
        public List<IRoleObjectAccessTable> RoleObjectAccesses { get; set; }
        public List<IFilterRuleTable> FilterRules { get; set; }
    }

    //public class RelationShipXmlInfo : Domain.RelationShip
    //{
    //    [XmlIgnore]
    //    public new RelationShipType RelationshipType { get; set; }

    //    [XmlAttribute("RelationshipType")]
    //    public int RelationshipTypeInt
    //    {
    //        get { return (int)RelationshipType; }
    //        set { RelationshipType = (RelationShipType)value; }
    //    }
    //}

    //public class EntityMapXmlInfo : Domain.Business.EntityMap
    //{
    //    [XmlIgnore]
    //    public new MapTypeEnum MapType { get; set; }

    //    [XmlAttribute("MapType")]
    //    public int MapTypeInt
    //    {
    //        get { return (int)MapType; }
    //        set { MapType = (MapTypeEnum)value; }
    //    }
    //}

    //public class RibbonButtonXmlInfo : Domain.Schema.RibbonButton
    //{
    //    [XmlIgnore]
    //    public new RibbonButtonAreaEnum ShowArea { get; set; }

    //    [XmlAttribute("ShowArea")]
    //    public int ShowAreaValue
    //    {
    //        get { return (int)ShowArea; }
    //        set { ShowArea = (RibbonButtonAreaEnum)value; }
    //    }
    //}
}