using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.Collections;

namespace com.Sconit.Utility
{
    public static class ExcelHelper
    {

        /// <summary>
        /// "Code [Description]"
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string GetCodeDescriptionString(string code, string description)
        {
            if (code == null || code.Trim() == string.Empty)
                code = string.Empty;
            if (description == null || description.Trim() == string.Empty)
                description = string.Empty;

            if (description == string.Empty)
                return code;
            else
                return code + " [" + description + "]";
        }

        public static string SubStr(string sString, int nLeng)
        {
            if (sString == null)
            {
                return string.Empty;
            }
            int totalLength = 0;
            int currentIndex = 0;
            while (totalLength < nLeng && currentIndex < sString.Length)
            {
                if (sString[currentIndex] < 0 || sString[currentIndex] > 255)
                    totalLength += 2;
                else
                    totalLength++;

                currentIndex++;
            }

            if (currentIndex < sString.Length)
                return sString.Substring(0, currentIndex) + "...";
            else
                return sString.ToString();
        }

        /// <summary>
        /// Sconit Common String Comparer, ignore case, support Null
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Eq(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static void ProcessPath(ref string path)
        {
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/"))
            {
                path += "/";
            }
        }

        public static string PadString(string code, int length)
        {
            string newCode = code;
            if (newCode != null && newCode.Length > length)
            {
                newCode = newCode.Remove(length);
            }
            else
            {
                newCode = newCode.PadLeft(length, '0');
            }
            return newCode;
        }

        public static string FormatBoxoverString(string str)
        {
            return str.Replace('[', '(').Replace(']', ')').Replace("'", "");
        }

        /// <summary>
        /// 跳过N行
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="count"></param>
        public static void JumpRows(IEnumerator rows, int count)
        {
            for (int i = 0; i < count; i++)
            {
                rows.MoveNext();
            }
        }

        public static string GetLineDataValue(string[] lineData, int colIndex)
        {
            if (lineData.Length < colIndex || colIndex == 0)
            {
                return null;
            }
            else
            {
                string colData = lineData[colIndex - 1];

                return colData == null ? null : colData.Trim();
            }
        }

        public static bool CheckValidDataRow(IRow row, int startColIndex, int endColIndex)
        {
            for (int i = startColIndex; i < endColIndex; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell != null && cell.CellType != NPOI.SS.UserModel.CellType.BLANK)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetCellStringValue(IRow row, int colIndex)
        {
            return GetCellStringValue(GetCell(row, colIndex));
        }

        public static string GetCellStringValue(ICell cell)
        {
            string strValue = null;
            if (cell != null)
            {
                if (cell.CellType == CellType.STRING)
                {
                    strValue = cell.StringCellValue;
                }
                else if (cell.CellType == CellType.NUMERIC)
                {
                    strValue = cell.NumericCellValue.ToString("0.########");
                }
                else if (cell.CellType == CellType.BOOLEAN)
                {
                    strValue = cell.NumericCellValue.ToString();
                }
                else if (cell.CellType == CellType.FORMULA)
                {
                    if (cell.CachedFormulaResultType == CellType.STRING)
                    {
                        strValue = cell.StringCellValue;
                    }
                    else if (cell.CachedFormulaResultType == CellType.NUMERIC)
                    {
                        strValue = cell.NumericCellValue.ToString("0.########");
                    }
                    else if (cell.CachedFormulaResultType == CellType.BOOLEAN)
                    {
                        strValue = cell.NumericCellValue.ToString();
                    }
                }
            }
            if (strValue != null)
            {
                strValue = strValue.Trim();
            }
            strValue = strValue == string.Empty ? null : strValue;
            return strValue;
        }

        public static ICell GetCell(IRow row, int colIndex)
        {
            if (colIndex > 0)
            {
                return row.GetCell(colIndex - 1);
            }
            else
            {
                return null;
            }
        }
    }
}
