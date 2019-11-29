using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Schema.StringMap;

namespace Xms.Data.Import
{
    /// <summary>
    /// 导入模板提供者
    /// </summary>
    public class FileTemplateProvider : IFileTemplateProvider
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailServiceFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IWebHelper _webHelper;

        public FileTemplateProvider(IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IOptionSetDetailFinder optionSetDetailFinder
            , IStringMapFinder stringMapFinder
            , IWebHelper webHelper)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _optionSetDetailServiceFinder = optionSetDetailFinder;
            _stringMapFinder = stringMapFinder;
            _webHelper = webHelper;
        }

        public string Get(Guid entityId)
        {
            var dir = _webHelper.MapPath("~/excel/");
            var entity = _entityFinder.FindById(entityId);
            var attributes = _attributeFinder.Query(n => n.Where(f => f.EntityId == entityId
            && f.AttributeTypeName != AttributeTypeIds.PRIMARYKEY
            && f.Name.NotIn("createdon", "createdby", "modifiedon", "modifiedby", "versionnumber", "owneridtype", "owningbusinessunit", "organizationid", "workflowid", "processstate", "stageid")).Sort(s => s.SortAscending(a => a.CreatedOn)));
            var filePath = dir + entity.Name + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet(entity.LocalizedName);
            IRow headerRow = sheet.CreateRow(0);
            int columnIndex = 0;
            foreach (var attr in attributes)
            {
                ICell cell;
                ICellStyle style = book.CreateCellStyle();
                if (attr.TypeIsPickList() || attr.TypeIsStatus())
                {
                    var options = _optionSetDetailServiceFinder.Query(n => n.Where(f => f.OptionSetId == attr.OptionSetId).Sort(s => s.SortAscending(f => f.DisplayOrder)));
                    sheet.AddValidationData(book.CreateListConstraint(columnIndex, options.Select(f => f.Name)));
                }
                else if (attr.TypeIsBit() || attr.TypeIsState())
                {
                    var options = _stringMapFinder.Query(n => n.Where(f => f.AttributeId == attr.AttributeId).Sort(s => s.SortAscending(f => f.DisplayOrder)));
                    sheet.AddValidationData(book.CreateListConstraint(columnIndex, options.Select(f => f.Name)));
                }
                else if (attr.TypeIsDateTime())
                {
                    sheet.AddValidationData(book.CreateDateConstraint(columnIndex));
                }
                else if (attr.TypeIsDecimal() || attr.TypeIsMoney())
                {
                    sheet.AddValidationData(book.CreateNumericConstraint(columnIndex, attr.MinValue.ToString(), attr.MaxValue.ToString()));
                }
                else if (attr.TypeIsInt())
                {
                    sheet.AddValidationData(book.CreateNumericConstraint(columnIndex, attr.MinValue.ToString(), attr.MaxValue.ToString(), true));
                }
                cell = headerRow.CreateCell(columnIndex);
                cell.SetCellValue(attr.LocalizedName);
                cell.CellStyle = style;
                columnIndex++;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                book.Write(ms);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
                book = null;
            }

            return filePath;
        }
    }
}