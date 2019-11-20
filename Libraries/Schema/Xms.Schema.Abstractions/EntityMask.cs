using System.Xml.Serialization;

namespace Xms.Schema.Abstractions
{
    public enum EntityMaskEnum
    {
        [XmlEnum("1")]
        User = 1

            ,

        [XmlEnum("4")]
        Organization = 4
    }
}