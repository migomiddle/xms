using System.Xml.Serialization;

namespace Xms.RibbonButton.Abstractions
{
    /// <summary>
    /// 按钮显示位置
    /// </summary>
    public enum RibbonButtonArea
    {
        [XmlEnum("1")]
        Form = 1

            ,

        [XmlEnum("2")]
        ListHead = 2

            ,

        [XmlEnum("3")]
        ListRow = 3

            ,

        [XmlEnum("4")]
        SubGrid = 4
    }
}