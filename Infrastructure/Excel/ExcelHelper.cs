using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Infrastructure.Logging;
using Npoi.Mapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace Infrastructure.Excel
{
    public static class ExcelHelper
    {
        /// <summary>
        /// excel檔案流轉的某個sheet成對像集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static List<T> ReadExcelSheet<T>(this Stream data, string sheetName = "Sheet1") where T : class
        {
            try
            {
                var importer = new Mapper(data);
                var items = importer.Take<T>(sheetName).Select(r => r.Value).ToList();
                return items;
            }
            catch (Exception ex)
            {
                LogHelper.Warn("ReadExcelSheet", "讀取excel失敗", ex);
                return null;
            }
        }


        /// <summary>
        /// excel檔案流轉的某個sheet成對像集合 並且解析出header
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Tuple<List<string>, List<T>> ReadExcelSheetWithHeader<T>(this Stream data, string sheetName = "Sheet1") where T : class
        {
            try
            {
                var importer = new Mapper(data);

                var items = importer.Take<T>(sheetName).Select(r => r.Value).ToList();

                var sheet = importer.Workbook.GetSheet(sheetName);
                IRow headerRow = sheet.GetRow(sheet.FirstRowNum);
                var headerValueList = new List<string>();
                foreach (ICell cell in headerRow)
                {
                    string stringCellValue = cell.StringCellValue;
                    headerValueList.Add(stringCellValue);
                }

                return Tuple.Create(headerValueList, items);
            }
            catch (Exception ex)
            {
                LogHelper.Warn("ReadExcelSheetWithHeader", "讀取excel失敗", ex);
                return Tuple.Create<List<string>, List<T>>(null, null);
            }
        }

        /// <summary>
        /// 導出excel模板
        /// </summary>
        /// <param name="fieldList">Item1是欄位名稱 Item2是欄位的備註</param>
        /// <param name="isxls"></param>
        /// <returns></returns>
        public static byte[] ToExcel(List<Tuple<string, string>> fieldList, bool isxls = false)
        {
            IWorkbook workbook = CreateWorkbook(isxls);
            ICellStyle headerCellStyle = GetCellStyle(workbook, true, isxls);
            ISheet sheet = workbook.CreateSheet("Sheet1");
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < fieldList.Count; i++)
            {
                var item = fieldList[i];
                ICell headerCell = headerRow.CreateCell(i);
                headerCell.SetCellValue(item.Item1);
                if (!string.IsNullOrEmpty(item.Item2))
                {
                    IDrawing patriarch = sheet.CreateDrawingPatriarch();

                    IComment comment = isxls ? patriarch.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, 2, 1, 4, 4)) : patriarch.CreateCellComment(new XSSFClientAnchor(0, 0, 0, 0, 2, 1, 4, 4));
                    comment.Author = "Admin";
                    if (isxls)
                    {
                        comment.String = new HSSFRichTextString(item.Item2);
                    }
                    else
                    {
                        comment.String = new XSSFRichTextString(item.Item2);
                    }
                    comment.Visible = false;
                    headerCell.CellComment = comment;
                }
                headerCell.CellStyle = headerCellStyle;
                sheet.AutoSizeColumn(headerCell.ColumnIndex);
            }

            byte[] xlsInBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                xlsInBytes = ms.ToArray();
            }

            return xlsInBytes;
        }

        /// <summary>
        /// 由DataSet導出Excel
        /// </summary>
        /// <param name="sourceDs">要導出數據的DataSet</param>
        /// <param name="isxls">是否是xls還是xlsx</param>
        /// <returns></returns>
        public static byte[] ToExcel(this DataSet sourceDs, bool isxls = false)
        {

            IWorkbook workbook = CreateWorkbook(isxls);
            ICellStyle headerCellStyle = GetCellStyle(workbook, true, isxls);

            for (int i = 0; i < sourceDs.Tables.Count; i++)
            {
                DataTable table = sourceDs.Tables[i];
                string sheetName = string.IsNullOrEmpty(table.TableName) ? "result" + i.ToString() : table.TableName;
                ISheet sheet = workbook.CreateSheet(sheetName);
                IRow headerRow = sheet.CreateRow(0);
                Dictionary<int, ICellStyle> colStyles = new Dictionary<int, ICellStyle>();
                // handling header.
                foreach (DataColumn column in table.Columns)
                {
                    ICell headerCell = headerRow.CreateCell(column.Ordinal);
                    headerCell.SetCellValue(column.ColumnName);
                    headerCell.CellStyle = headerCellStyle;
                    sheet.AutoSizeColumn(headerCell.ColumnIndex);
                    colStyles[headerCell.ColumnIndex] = GetCellStyle(workbook);
                }

                // handling value.
                int rowIndex = 1;

                foreach (DataRow row in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    foreach (DataColumn column in table.Columns)
                    {
                        ICell cell = dataRow.CreateCell(column.Ordinal);
                        SetCellValue(cell, (row[column] ?? "").ToString(), column.DataType, colStyles);
                        ReSizeColumnWidth(sheet, cell, column.DataType);
                    }

                    rowIndex++;
                }
                sheet.ForceFormulaRecalculation = true;
            }
            byte[] xlsInBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                xlsInBytes = ms.ToArray();
            }

            return xlsInBytes;
        }


        /// <summary>
        /// 導出Excel
        /// </summary>
        /// <param name="list">導出模型數據</param>
        /// <param name="ExcelSheetName">初始頁簽名稱</param>
        public static byte[] ExportExcel<T>(IEnumerable<T> list, string ExcelSheetName = "Sheet1")
        {
            //建立workbook
            IWorkbook workbook = CreateWorkbook(false);
            //建立worksheet
            ISheet sheet = workbook.CreateSheet(ExcelSheetName);
            //頭樣式
            ICellStyle headerCellStyle = GetCellStyle(workbook, true);
            //儲存頭部樣式
            Dictionary<int, ICellStyle> colStyles = new Dictionary<int, ICellStyle>();
            //獲取導出屬性
            Dictionary<PropertyInfo, ExcelFieldAttribute> _excelInfos = GetPropInfo<T>();

            //設定Excel行
            IRow rowTitle = sheet.CreateRow(0);
            int _cellIndex = 0;
            foreach (var item in _excelInfos)
            {
                ICell celltitle = rowTitle.CreateCell(_cellIndex);
                celltitle.SetCellValue(item.Value.Name);
                celltitle.CellStyle = headerCellStyle;
                sheet.AutoSizeColumn(celltitle.ColumnIndex);//自動列寬
                colStyles[celltitle.ColumnIndex] = GetCellStyle(workbook);
                _cellIndex++;
            }

            //設定Excel內容
            int _rowNum = 1;
            foreach (T rowItem in list)
            {
                int _rowCell = 0;
                IRow _rowValue = sheet.CreateRow(_rowNum);
                foreach (var cellItem in _excelInfos)
                {
                    object _cellItemValue = cellItem.Key.GetValue(rowItem, null);
                    ICell _cell = _rowValue.CreateCell(_rowCell);
                    SetCellValue(_cell, _cellItemValue == null ? "" : _cellItemValue.ToString(), cellItem.Key.PropertyType, colStyles);
                    ReSizeColumnWidth(sheet, _cell, cellItem.Key.PropertyType);
                    _rowCell++;
                }
                _rowNum++;
            }
            sheet.ForceFormulaRecalculation = true;

            //導出
            byte[] xlsInBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                xlsInBytes = ms.ToArray();
            }
            return xlsInBytes;
        }
        #region Private
        /// <summary>
        /// 獲取實體類的公共屬性
        /// </summary>
        /// <typeparam name="T">實體類</typeparam>
        /// <returns>實體類相應集合</returns>
        private static Dictionary<PropertyInfo, ExcelFieldAttribute> GetPropInfo<T>()
        {
            Dictionary<PropertyInfo, ExcelFieldAttribute> _infos = new Dictionary<PropertyInfo, ExcelFieldAttribute>();
            Type _type = typeof(T);
            var classAttr = _type.GetCustomAttributes<ExcelClassAttribute>();

            //獲取所有的Properties
            PropertyInfo[] _propInfos = _type.GetProperties();

            foreach (ExcelClassAttribute classInfo in classAttr)
            {
                ExcelFieldAttribute attr = new ExcelFieldAttribute(classInfo.Name);
                attr.OrderRule = classInfo.OrderRule;
                var p = _propInfos.FirstOrDefault(r => r.Name.Equals(classInfo.Column));
                if (p != null)
                {
                    _infos.Add(p, attr);
                }
            }

            foreach (var propInfo in _propInfos)
            {
                var attr = propInfo.GetCustomAttribute<ExcelFieldAttribute>();
                if (attr != null)
                {
                    _infos.Add(propInfo, attr);
                }
            }
            _infos = _infos.OrderBy(r => r.Value.OrderRule).ToDictionary(r => r.Key, y => y.Value);
            return _infos;
        }

        /// <summary>
        /// 建立工作薄
        /// </summary>
        /// <param name="isCompatible"></param>
        /// <returns></returns>
        private static IWorkbook CreateWorkbook(bool isCompatible)
        {
            if (isCompatible)
            {
                return new HSSFWorkbook(); // 03
            }
            else
            {
                return new XSSFWorkbook(); // 07 以上
            }
        }


        /// <summary>
        /// 建立單元格樣式
        /// </summary>
        /// <param name="workbook">workbook</param>
        /// <param name="isHeaderRow">是否獲取頭部樣式</param>
        /// <returns></returns>
        private static ICellStyle GetCellStyle(IWorkbook workbook, bool isHeaderRow = false, bool isXls = false)
        {
            if (!isXls)
            {
                return GetCellStyleXlsx(workbook, isHeaderRow);
            }
            ICellStyle style = workbook.CreateCellStyle();

            if (isHeaderRow)
            {
                style.FillPattern = FillPattern.SolidForeground;
                style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                IFont f = workbook.CreateFont();
                f.IsBold = true;
                style.SetFont(f);
            }

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            return style;
        }

        private static ICellStyle GetCellStyleXlsx(IWorkbook workbook, bool isHeaderRow = false)
        {
            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();

            if (isHeaderRow)
            {
                style.FillPattern = FillPattern.SolidForeground;
                style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.Alignment = HorizontalAlignment.Center;
                XSSFFont defaultFont = (XSSFFont)workbook.CreateFont();
                defaultFont.FontHeightInPoints = (short)10;
                defaultFont.FontName = "Arial";
                defaultFont.Color = IndexedColors.Black.Index;
                defaultFont.IsItalic = false;
                defaultFont.IsBold = true;
                style.SetFont(defaultFont);
            }

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            return style;
        }


        /// <summary>
        /// 依據值型別為單元格設定值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        /// <param name="colType"></param>
        /// <param name="colStyles"></param>
        private static void SetCellValue(ICell cell, string value, Type colType, IDictionary<int, ICellStyle> colStyles)
        {
            string dataFormatStr = null;
            switch (colType.ToString())
            {
                case "System.String": //字串型別
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(value);
                    break;
                case "System.Nullable`1[System.DateTime]":
                case "System.DateTime": //日期型別
                    DateTime dateV;
                    if (DateTime.TryParse(value, out dateV))
                    {
                        cell.SetCellValue(dateV);
                    }
                    dataFormatStr = "yyyy/mm/dd hh:mm:ss";
                    break;
                case "System.Nullable`1[System.Boolean]":
                case "System.Boolean": //布爾型
                    bool boolV = false;
                    if (bool.TryParse(value, out boolV))
                    {
                        cell.SetCellType(CellType.Boolean);
                        cell.SetCellValue(boolV);
                    }
                    break;
                case "System.Int16": //整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                case "System.Nullable`1[System.Int16]": //整型
                case "System.Nullable`1[System.Int32]":
                case "System.Nullable`1[System.Int64]":
                case "System.Nullable`1[System.Byte]":
                    long intV = 0;
                    if (long.TryParse(value, out intV))
                    {
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(intV);
                    }
                    dataFormatStr = "0";
                    break;
                case "System.Decimal": //浮點型
                case "System.Double":
                case "System.Nullable`1[System.Decimal]": //浮點型
                case "System.Nullable`1[System.Double]":
                    double doubV = 0;
                    if (double.TryParse(value, out doubV))
                    {
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(doubV);
                    }
                    dataFormatStr = "0.00";
                    break;
                case "System.DBNull": //空值處理
                    cell.SetCellType(CellType.Blank);
                    cell.SetCellValue("");
                    break;
                default:
                    cell.SetCellType(CellType.Unknown);
                    cell.SetCellValue(value);
                    break;
            }

            if (!string.IsNullOrEmpty(dataFormatStr) && colStyles[cell.ColumnIndex].DataFormat <= 0) //沒有設定，則採用預設型別格式
            {
                colStyles[cell.ColumnIndex] = GetCellStyleWithDataFormat(cell.Sheet.Workbook, dataFormatStr);
            }
            cell.CellStyle = colStyles[cell.ColumnIndex];
        }

        /// <summary>
        /// 根據單元格內容重新設定列寬
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cell"></param>
        /// <param name="cellType"></param>
        private static void ReSizeColumnWidth(ISheet sheet, ICell cell, Type cellType)
        {
            int cellLength = (Encoding.Default.GetBytes(cell.ToString()).Length + 2) * 256;
            const int maxLength = 60 * 256; //255 * 256;
            if (cellLength > maxLength) //當單元格內容超過30箇中文字元（英語60個字元）寬度，則強制換行
            {
                cellLength = maxLength;
                cell.CellStyle.WrapText = true;
            }
            int colWidth = sheet.GetColumnWidth(cell.ColumnIndex);
            if (colWidth < cellLength)
            {
                sheet.SetColumnWidth(cell.ColumnIndex, cellLength);
            }
            else if (cellType == typeof(DateTime))
            {
                sheet.SetColumnWidth(cell.ColumnIndex, colWidth + 10);
            }
        }

        /// <summary>
        /// 建立單元格樣式並設定數據格式化規則
        /// </summary>
        /// <param name="workbook">workbook</param>
        /// <param name="format">格式化字串</param>
        private static ICellStyle GetCellStyleWithDataFormat(IWorkbook workbook, string format)
        {
            var style = GetCellStyle(workbook);
            var dataFormat = workbook.CreateDataFormat();
            short formatId = -1;
            if (dataFormat is HSSFDataFormat)
            {
                formatId = HSSFDataFormat.GetBuiltinFormat(format);
            }
            if (formatId != -1)
            {
                style.DataFormat = formatId;
            }
            else
            {
                style.DataFormat = dataFormat.GetFormat(format);
            }
            return style;
        }

        #endregion

    }
}
