using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Xms.Data.Import
{
    /// <summary>
    /// excel文件助手
    /// </summary>
    public class ExcelHelper
    {
        public static List<string> GetColumns(string filePath)
        {
            IWorkbook workBook = GetWorkbook(filePath);
            ISheet sheet = workBook.GetSheetAt(0);
            if (null != sheet)
            {
                IRow row = sheet.GetRow(0);
                return row.Cells.Select(n => n.StringCellValue).ToList<string>();
            }
            return new List<string>();
        }

        public static DataTable ToDataTable(string filePath)
        {
            IWorkbook workBook = GetWorkbook(filePath);
            bool isHigher = filePath.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase);
            ISheet sheet = workBook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            var headerRow = sheet.GetRow(0);
            for (int j = 0; j < (headerRow.LastCellNum); j++)
            {
                dt.Columns.Add(headerRow.GetCell(j).ToString());
            }
            rows.MoveNext();//跳过表头
            while (rows.MoveNext())
            {
                IRow row;
                if (isHigher)
                {
                    row = (XSSFRow)rows.Current;
                }
                else
                {
                    row = (HSSFRow)rows.Current;
                }

                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    ICell cell = row.GetCell(i);
                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        dr[i] = cell.GetCellValue();
                    }
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static IWorkbook GetWorkbook(string filePath)
        {
            IWorkbook workBook = null;
            if (filePath.EndsWith(".xls", StringComparison.InvariantCultureIgnoreCase))
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    workBook = new HSSFWorkbook(file);
                }
            }
            else if (filePath.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase))
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    workBook = new XSSFWorkbook(file);
                }
            }
            return workBook;
        }
    }
}