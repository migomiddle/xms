using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Xms.Configuration;
using Xms.Context;
using Xms.File.Extensions;
using Xms.Infrastructure.Utility;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    /// <summary>
    /// 实体插件文件处理
    /// </summary>
    public class EntityPluginFileProvider : IEntityPluginFileProvider
    {
        private readonly IAppContext _appContext;
        private readonly IWebHelper _webHelper;
        private readonly ISettingFinder _settingFinder;

        public EntityPluginFileProvider(IAppContext appContext
            , IWebHelper webHelper
            , ISettingFinder settingFinder)
        {
            _appContext = appContext;
            _webHelper = webHelper;
            _settingFinder = settingFinder;
        }

        public string GetBaseDirectory()
        {
            return _webHelper.MapPath("~/entityplugins/" + _appContext.OrganizationUniqueName + "/", true);
        }

        public string GetTempDirectory()
        {
            return _webHelper.MapPath("~/tempuploadfiles/" + _appContext.OrganizationUniqueName + "/", true);
        }

        public async Task<string> Save(IFormFile file)
        {
            if (file != null)
            {
                string dir = GetBaseDirectory();
                string savePath = dir + file.FileName;
                var result = await file.SaveAs(savePath, _settingFinder, _webHelper).ConfigureAwait(false);
                return result.FilePath;
            }
            return string.Empty;
        }

        public string Save(string fileName)
        {
            string tempDir = GetTempDirectory();
            var tempFilePath = tempDir + fileName;
            string dir = GetBaseDirectory();
            var savePath = dir + fileName;
            FileInfo tempFile = new FileInfo(tempFilePath);
            if (tempFile.Exists)
            {
                tempFile.CopyTo(savePath, true);
                return savePath;
            }
            return string.Empty;
        }

        public bool LoadAssembly(EntityPlugin entity)
        {
            try
            {
                var dllPath = GetBaseDirectory() + entity.AssemblyName;
                if (!System.IO.File.Exists(dllPath))
                {
                    dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, entity.AssemblyName);
                }
                if (System.IO.File.Exists(dllPath))
                {
                    //Assembly.LoadFile(dllPath);
                    //byte[] b = System.IO.File.ReadAllBytes(dllPath);
                    //Assembly.Load(b);
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public async Task<List<PluginAnalysis>> BeforehandLoad(IFormFile file)
        {
            if (file != null)
            {
                string dir = GetTempDirectory();
                string savePath = dir + file.FileName;
                var result = await file.SaveAs(savePath, _settingFinder, _webHelper).ConfigureAwait(false);
                while (System.IO.File.Exists(result.FilePath))
                {
                    var pluginAnalysis = PluginAnalysisHelper.GetPluginAnalysis(result.FilePath);
                    return pluginAnalysis;
                }
            }
            return null;
        }

        public List<PluginAnalysis> BeforehandLoad(string fileName)
        {
            string dir = GetBaseDirectory();
            string savePath = dir + fileName;

            if (System.IO.File.Exists(savePath))
            {
                var pluginAnalysis = PluginAnalysisHelper.GetPluginAnalysis(savePath);
                return pluginAnalysis;
            }
            return null;
        }
    }
}