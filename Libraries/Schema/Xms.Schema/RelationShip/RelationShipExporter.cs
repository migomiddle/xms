using System;
using Xms.Solution.Abstractions;

namespace Xms.Schema.RelationShip
{
    /// <summary>
    /// 关系导出XML
    /// </summary>
    public class RelationShipExporter : ISolutionComponentExporter
    {
        public RelationShipExporter()
        {
        }

        public string GetXml(Guid solutionId)
        {
            return string.Empty;
        }
    }
}