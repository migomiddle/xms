using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xms.Data.Import
{
    public static class NpoiExtensions
    {
        public static HSSFDataValidation CreateListConstraint(this HSSFWorkbook book, int columnIndex, IEnumerable<string> values)
        {
            var sheetName = "_constraintSheet_";
            ISheet sheet = book.GetSheet(sheetName) ?? book.CreateSheet(sheetName);
            var firstRow = sheet.GetRow(0);
            var conColumnIndex = firstRow == null ? 0 : firstRow.PhysicalNumberOfCells;
            var rowIndex = 0;
            var lastValue = "";

            foreach (var value in values)
            {
                var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
                row.CreateCell(conColumnIndex).SetCellValue(value);
                rowIndex++;
                lastValue = value;
            }
            //如果只有一个可选值，则增加一个相同的选项
            if (values.Count() == 1)
            {
                var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
                row.CreateCell(conColumnIndex).SetCellValue(lastValue);
                rowIndex++;
            }
            IName range = book.CreateName();
            range.RefersToFormula = String.Format("{2}!${0}$1:${0}${1}",
                            (Char)('A' + conColumnIndex),
                            rowIndex.ToString(), sheetName);

            string rangeName = "dicRange" + columnIndex;
            range.NameName = rangeName;
            var cellRegions = new CellRangeAddressList(1, 65535, columnIndex, columnIndex);
            var constraint = DVConstraint.CreateFormulaListConstraint(rangeName);
            book.SetSheetHidden(book.GetSheetIndex(sheet), true);

            return new HSSFDataValidation(cellRegions, constraint);
        }

        public static HSSFDataValidation CreateDateConstraint(this HSSFWorkbook book, int columnIndex)
        {
            ISheet sheet1 = book.GetSheetAt(0);
            CellRangeAddressList cellRegions = new CellRangeAddressList(1, 65535, columnIndex, columnIndex);
            DVConstraint constraint = DVConstraint.CreateDateConstraint(OperatorType.BETWEEN, "1900-01-01", "2999-12-31", "yyyy-MM-dd");
            HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("error", "You must input a date.");
            sheet1.AddValidationData(dataValidate);

            return new HSSFDataValidation(cellRegions, constraint);
        }

        public static HSSFDataValidation CreateNumericConstraint(this HSSFWorkbook book, int columnIndex, string minvalue, string maxvalue, bool isInteger = false)
        {
            ISheet sheet1 = book.GetSheetAt(0);
            CellRangeAddressList cellRegions = new CellRangeAddressList(1, 65535, columnIndex, columnIndex);
            DVConstraint constraint = DVConstraint.CreateNumericConstraint(isInteger ? ValidationType.INTEGER : ValidationType.DECIMAL, OperatorType.BETWEEN, minvalue, maxvalue);
            HSSFDataValidation dataValidate = new HSSFDataValidation(cellRegions, constraint);
            dataValidate.CreateErrorBox("error", "You must input a numeric between " + minvalue + " and " + maxvalue + ".");
            sheet1.AddValidationData(dataValidate);

            return new HSSFDataValidation(cellRegions, constraint);
        }

        public static object GetCellValue(this ICell cell)
        {
            object value = null;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    break;

                case CellType.Boolean:
                    value = cell.BooleanCellValue ? "1" : "0"; break;
                case CellType.Error:
                    value = cell.ErrorCellValue; break;
                case CellType.Formula:
                    value = "=" + cell.CellFormula; break;
                case CellType.Numeric:
                    value = cell.NumericCellValue; break;
                case CellType.String:
                    value = cell.StringCellValue; break;
                case CellType.Unknown:
                    break;
            }
            return value;
        }
    }
}