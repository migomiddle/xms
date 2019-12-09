using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Xms.Data.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Organization.Data;
using Xms.Organization.Domain;
using Xms.Web.Framework.Models;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    public class InitializationController : Microsoft.AspNetCore.Mvc.Controller
    {
        public readonly IServiceCollection _services;

        public InitializationController()
        {
        }

        public IActionResult Initialization()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, reloadOnChange: true);
            var config = builder.Build();
            SqlConnectionStringBuilder sqlConnection = new SqlConnectionStringBuilder(config["DataBase:ConnectionString"]);
            DbConfigurationModel model = new DbConfigurationModel()
            {
                DataServerName = sqlConnection.DataSource,
                DataAccountName = sqlConnection.UserID,
                DataPassword = sqlConnection.Password,
                DatabaseName = sqlConnection.InitialCatalog,
                CommandTimeOut = sqlConnection.ConnectTimeout
            };
            return View(model);
        }

        public IActionResult ConnectionTest([FromBody]DbConfigurationModel model)
        {
            string connectionString = DataBaseHelper.GetDbConfiguration(model.DataServerName, model.DataAccountName, model.DataPassword, model.DatabaseName, model.CommandTimeOut);
            bool isConnectionTest = DataBaseHelper.ConnectionTest(connectionString);
            if (isConnectionTest)
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, reloadOnChange: true);
                var config = builder.Build();
                DataBaseOptions options = new DataBaseOptions()
                {
                    ConnectionString = connectionString
                };
                IOrganizationBaseRepository organizationBaseRepository = new OrganizationBaseRepository(options);
                IOrganizationBaseService _organizationBaseService = new Organization.OrganizationBaseService(organizationBaseRepository);
                List<OrganizationBase> orglist = _organizationBaseService.Query(n => n.Where(x => x.State == 1));
                return new JsonResult(new JsonResultObject() { IsSuccess = true, Content = orglist.SerializeToJson() });
            }
            else
            {
                return new JsonResult(new JsonResultObject() { IsSuccess = false });
            }
        }

        public IActionResult Save([FromBody]InitializationModel model)
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, reloadOnChange: true);
                var config = builder.Build();
                config["DataBase:ConnectionString"] = DataBaseHelper.GetDbConfiguration(model.DataServerName, model.DataAccountName, model.DataPassword, model.DatabaseName, model.CommandTimeOut);
                config["Initialization:IsInitialization"] = "true";
                WriteJson( config["DataBase:ConnectionString"], config["Initialization:IsInitialization"]);
                if (model.OrganizationBases != null)
                {
                    DataBaseOptions options = new DataBaseOptions()
                    {
                        ConnectionString = config["DataBase:ConnectionString"]
                    };
                    IOrganizationBaseRepository organizationBaseRepository = new OrganizationBaseRepository(options);
                    IOrganizationBaseService _organizationBaseService = new Organization.OrganizationBaseService(organizationBaseRepository);
                    model.OrganizationBases.ForEach(x =>
                    {
                        _organizationBaseService.Update(x);
                    });
                }

                return new JsonResult(new JsonResultObject() { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new JsonResultObject() { IsSuccess = false, Content = ex.Message });
            }
        }

        /// <summary>
        /// 写入指定section文件
        /// </summary>
        public void WriteJson( string dbConfiguration, string isInitialization)
        {
            string path = "appsettings.json";
            try
            {
                JObject jObj;
                using (StreamReader file = new StreamReader(path))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObj = (JObject)JToken.ReadFrom(reader);
                    jObj["DataBase"]["DataBase"] = dbConfiguration;
                    jObj["Initialization"]["IsInitialization"] = isInitialization;
                }
                using (StreamWriter writer = new StreamWriter(path))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        jObj.WriteTo(jsonWriter);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}