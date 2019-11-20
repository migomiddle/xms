using System.Xml.Serialization;

namespace Xms.Core
{
    public enum RecordState
    {
        [XmlEnum("1")]
        Enabled = 1

        ,

        [XmlEnum("0")]
        Disabled = 0
    }
}