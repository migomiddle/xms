using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xms.Configuration;
using Xms.Context;
using Xms.File.Extensions;
using Xms.Identity;
using Xms.Infrastructure.Inject;
using Xms.Infrastructure.Utility;
using Xms.Infrastructure.Utility.Serialize;
using Xms.Solution.Abstractions;

namespace Xms.Solution
{
    /// <summary>
    /// 解决方案导入服务
    /// </summary>
    public class SolutionImporter : ISolutionImporter
    {
        private readonly ISolutionService _solutionService;
        private readonly IAppContext _appContext;
        private readonly IWebHelper _webHelper;
        private readonly ISettingFinder _settingFinder;
        private readonly ICurrentUser _currentUser;
        private readonly IServiceResolver _serviceResolver;

        public SolutionImporter(IAppContext appContext
            , ISolutionService solutionService
            , IWebHelper webHelper
            , ISettingFinder settingFinder
            , IServiceResolver serviceResolver)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _solutionService = solutionService;
            _webHelper = webHelper;
            _settingFinder = settingFinder;
            _serviceResolver = serviceResolver;
        }

        /// <summary>
        /// 导入解决方案
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> ImportAsync(IFormFile file)
        {
            var dir = _webHelper.MapPath("~/solution/import/" + DateTime.Now.ToString("yyMMddhhmmss"));
            var d = Directory.CreateDirectory(dir);
            var filePath = dir + "/" + file.FileName;
            await file.SaveAs(filePath, _settingFinder, _webHelper).ConfigureAwait(false);
            IOHelper.UnZip(filePath);
            //解决方案信息
            var solution = new Domain.Solution();
            solution = solution.DeserializeFromXMLFile(dir + "/solution.xml");
            var existSolution = _solutionService.FindById(solution.SolutionId);
            if (existSolution != null)
            {
                existSolution.Name = solution.Name;
                existSolution.Description = solution.Description;
                existSolution.Version = solution.Version;
                existSolution.ModifiedBy = _currentUser.SystemUserId;
                existSolution.ModifiedOn = DateTime.Now;
                _solutionService.Update(existSolution);
            }
            else
            {
                solution.CreatedBy = _currentUser.SystemUserId;
                solution.InstalledOn = DateTime.Now;
                solution.PublisherId = _currentUser.SystemUserId;
                _solutionService.Create(solution);
            }
            //自定义内容
            XDocument doc = XDocument.Load(dir + "/customizations.xml");
            IEnumerable<XElement> elements = from e in doc.Element("ImportExportXml").Elements()
                                             select e;
            foreach (var node in elements)
            {
                var importerTypeName = ImporterTypes[node.Name.ToString()];
                if (importerTypeName.IsNotEmpty())
                {
                    var importerType = ImporterNodeTypeMapper.Get(node.Name.ToString());//Type.GetType(importerTypeName);
                    if (importerType != null)
                    {
                        var componentArg = importerType.GetInterface(typeof(ISolutionComponentImporter<>).Name).GenericTypeArguments[0];
                        var importerService = _serviceResolver.Get(typeof(ISolutionComponentImporter<>).MakeGenericType(componentArg));
                        var listType = typeof(List<>).MakeGenericType(componentArg);
                        var listObj = Activator.CreateInstance(listType);
                        foreach (var item in node.Elements())
                        {
                            var component = Activator.CreateInstance(componentArg);
                            component = Serializer.FromXml(componentArg, item.ToString());
                            listType.GetMethod("Add").Invoke(listObj, new[] { component });
                        }
                        //invoke import method
                        var importMethod = importerType.GetMethod("Import");
                        var importResult = importMethod.Invoke(importerService, new[] { solution.SolutionId, listObj });
                    }
                }
            }
            //delete files
            Directory.Delete(dir, true);

            return true;
        }

        private readonly Dictionary<string, string> ImporterTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "entities", "Xms.Schema.Entity.EntityImporter,Xms.Schema.Entity" }
            ,{ "charts", "Xms.Business.DataAnalyse.Visualization.ChartImporter,Xms.Business.DataAnalyse" }
        };
    }
}