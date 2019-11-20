using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xms.RibbonButton.Abstractions;

namespace Xms.RibbonButton
{
    public interface IRibbonButtonImporter
    {
        bool Import(Guid solutionId, List<RibbonButtonXmlInfo> ribbonButtons);
    }

    public class RibbonButtonXmlInfo : Domain.RibbonButton
    {
        [XmlIgnore]
        public new RibbonButtonArea ShowArea { get; set; }

        [XmlAttribute("ShowArea")]
        public int ShowAreaValue
        {
            get { return (int)ShowArea; }
            set { ShowArea = (RibbonButtonArea)value; }
        }
    }
}