using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xms.Context;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;
using Xms.Solution.Data;

namespace Xms.Solution
{
    /// <summary>
    /// 解决方案导出服务
    /// </summary>
    public class SolutionExporter : ISolutionExporter
    {
        private readonly ISolutionRepository _solutionRepository;
        private readonly IAppContext _appContext;
        private readonly IWebHelper _webHelper;
        private readonly IEnumerable<ISolutionComponentExporter> _exporters;

        public SolutionExporter(IAppContext appContext
            , ISolutionRepository solutionRepository
            , IWebHelper webHelper
            , IEnumerable<ISolutionComponentExporter> exporters)
        {
            _appContext = appContext;
            _solutionRepository = solutionRepository;
            _webHelper = webHelper;
            _exporters = exporters;
        }

        /// <summary>
        /// 导出解决方案
        /// </summary>
        /// <param name="solutionId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool Export(Guid solutionId, out string file)
        {
            var solution = _solutionRepository.FindById(solutionId);
            StringBuilder xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\"?>");
            xml.Append("<ImportExportXml xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            //var exporters = ServiceLocator.GetAll<ISolutionComponentExporter>();
            foreach (var expt in _exporters)
            {
                xml.Append(expt.GetXml(solutionId));
            }
            //xml.Append(_solutionRepository.GetEntitiesXml(solutionId));
            //xml.Append(_solutionRepository.GetChartsXml(solutionId));
            //xml.Append(_solutionRepository.GetDuplicateRulesXml(solutionId));
            //xml.Append(_solutionRepository.GetEntityPluginsXml(solutionId));
            //xml.Append(_solutionRepository.GetOptionSetsXml(solutionId));
            //xml.Append(_solutionRepository.GetQueryViewsXml(solutionId));
            //xml.Append(_solutionRepository.GetRelationShipsXml(solutionId));
            //xml.Append(_solutionRepository.GetRibbonButtonsXml(solutionId));
            //xml.Append(_solutionRepository.GetSystemFormsXml(solutionId));
            //xml.Append(_solutionRepository.GetEntityMapsXml(solutionId));
            //xml.Append(_solutionRepository.GetSerialNumberRulesXml(solutionId));
            //xml.Append(_solutionRepository.GetWebResourcesXml(solutionId));
            //xml.Append(_solutionRepository.GetWorkFlowsXml(solutionId));
            //xml.Append(_solutionRepository.GetReportsXml(solutionId));
            //xml.Append(_solutionRepository.GetDashboardsXml(solutionId));
            //xml.Append(_solutionRepository.GetLocalizedLabelXml(solutionId));
            //xml.Append(_solutionRepository.GetPrivilegesXml(solutionId));
            //xml.Append(_solutionRepository.GetRolesXml(solutionId));
            //xml.Append(_solutionRepository.GetFilterRuleXml(solutionId));
            xml.Append("</ImportExportXml>");
            var randomName = DateTime.Now.ToString("yyMMddhhmmss");
            var dir = _webHelper.MapPath("~/solution/export/" + randomName);
            var d = Directory.CreateDirectory(dir);
            //save customizations
            var customizationsFilePath = dir + "/customizations.xml";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.ToString());
            doc.Save(customizationsFilePath);
            //save solution
            var solutionFilePath = dir + "/solution.xml";
            solution.SerializeToXml(solutionFilePath);

            file = _webHelper.MapPath("~/solution/export/" + solution.Name + "_" + solution.Version.Replace(".", "_") + "_" + randomName + ".zip");
            IOHelper.CreateZip(dir, file);
            d.Delete(true);

            return true;
        }
    }
}