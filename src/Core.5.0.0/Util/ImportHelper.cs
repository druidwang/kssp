﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Utility
{
    public static class ImportHelper
    {
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


        public static bool CheckValidDataRow(HSSFRow row, int startColIndex, int endColIndex)
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

        public static void ThrowCommonError(int rowIndex, int colIndex, ICell cell)
        {
            string errorValue = string.Empty;
            if (cell != null)
            {
                if (cell.CellType == NPOI.SS.UserModel.CellType.STRING)
                {
                    errorValue = cell.StringCellValue;
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.NUMERIC)
                {
                    errorValue = cell.NumericCellValue.ToString("0.########");
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.BOOLEAN)
                {
                    errorValue = cell.NumericCellValue.ToString();
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.BLANK)
                {
                    errorValue = "Null";
                }
                else
                {
                    errorValue = "Unknow value";
                }
            }
            throw new BusinessException("Import.Read.CommonError", (rowIndex + 1).ToString(), (colIndex + 1).ToString(), errorValue);
        }
    }
}
