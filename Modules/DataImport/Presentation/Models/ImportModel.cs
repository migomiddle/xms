using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xms.Data.Import.Domain;

namespace Xms.Module.DataImport.Web.Models
{
    public class DialogModel
    {
        public string InputId { get; set; }
        public bool SingleMode { get; set; }

        public string CallBack { get; set; } = "function(){}";
    }
    public class ImportModel
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }

        public List<Schema.Domain.Attribute> Attributes { get; set; }

        public Dictionary<string, Schema.Domain.Attribute> MapData { get; set; }

        public IFormFile DataFile { get; set; }

        public string DataFileName { get; set; }

        public string MapCustomizations { get; set; }

        public string Name { get; set; }
        public int DuplicateDetection { get; set; }
        public Guid ImportMapId { get; set; }
        public Guid ImportFileId { get; set; }
        public List<ImportMap> ImportMaps { get; set; }
    }
}
