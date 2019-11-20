using System.Xml.Serialization;

namespace Xms.Core
{
    public enum AccessRightValue
    {
        [XmlEnum("1")]
        Read = 1,

        [XmlEnum("2")]
        Create = 2,

        [XmlEnum("3")]
        Update = 3,

        [XmlEnum("4")]
        Delete = 4,

        [XmlEnum("5")]
        Share = 5,

        [XmlEnum("6")]
        Append = 6,

        [XmlEnum("7")]
        AppendTo = 7,

        [XmlEnum("8")]
        Import = 8,

        [XmlEnum("9")]
        Export = 9,

        [XmlEnum("10")]
        Assign = 10
    }
}