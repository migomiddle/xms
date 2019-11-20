using System.Xml.Serialization;

namespace Xms.WebResource.Abstractions
{
    public enum WebResourceType
    {
        [XmlEnum("0")]
        Script

            ,

        [XmlEnum("1")]
        Picture

            ,

        [XmlEnum("2")]
        Html

            ,

        [XmlEnum("3")]
        Css
    }
}