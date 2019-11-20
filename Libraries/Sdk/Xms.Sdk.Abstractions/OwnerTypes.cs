using System.Xml.Serialization;

namespace Xms.Sdk.Abstractions
{
    public enum OwnerTypes
    {
        [XmlEnum("1")]
        SystemUser = 1

        ,

        [XmlEnum("2")]
        BusinessUnit = 2

        ,

        [XmlEnum("3")]
        Team = 3
    }
}