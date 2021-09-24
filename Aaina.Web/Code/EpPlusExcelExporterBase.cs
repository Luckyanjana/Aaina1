using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Aaina.Web.Code
{
    public class EpPlusExcelExporterBase
    {
        protected static void AddHeader(ExcelWorksheet sheet, params string[] headerTexts)
        {

            for (var i = 0; i < headerTexts.Length; i++)
            {
                AddHeader(sheet, i + 1, headerTexts[i]);
            }
        }

        protected static void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }

        protected static void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        protected static void AddObjectsFromDataTable(ExcelWorksheet sheet, int startRowIndex, string[] headerTexts, DataTable dt)
        {
            for (var i = 1; i <= dt.Rows.Count; i++)
            {
                for (var j = 0; j < headerTexts.Length; j++)
                {
                    if (headerTexts[j] == "Emotion")
                    {
                        var value = dt.Rows[i - 1][headerTexts[j]];
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            System.Drawing.Image myImage = System.Drawing.Image.FromFile(value.ToString());
                            var pic = sheet.Drawings.AddPicture(DateTime.Now.Ticks.ToString(), myImage);
                            pic.SetPosition((i + startRowIndex)-2, (i + startRowIndex)-2, j, j + 1);
                        }
                        else
                        {
                            sheet.Cells[i + startRowIndex, j + 1].Value = "";
                        }
                        
                    }
                    else
                    {
                        var value = dt.Rows[i - 1][headerTexts[j]];
                        sheet.Cells[(i + startRowIndex)-1, j + 1].Value = value.ToString();
                    }
                       
                }
            }
        }

        public static FileInfo ExportExcel(string sheetName, DataTable dt)
        {
            List<string> headerTexts = new List<string>();
            foreach (DataColumn item in dt.Columns)
            {
                headerTexts.Add(item.ColumnName);
            }
            var newFile = FileOutputUtil.CreateFile(DateTime.Now.Ticks + ".xlsx");
            using (var package = new ExcelPackage(newFile))
            {
                var sheet = package.Workbook.Worksheets.Add(sheetName);
                AddHeader(sheet, headerTexts.ToArray());
                AddObjectsFromDataTable(sheet, 2, headerTexts.ToArray(), dt);
                package.Save();
                return newFile;
            }
        }
    }
}
