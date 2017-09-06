

namespace com.Sconit.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Entity.Exception;
    using System.IO;
    
    public static class BarcodeHelper
    {
        public static string GetBarcodeStr(string str, string barCodeFontName)
        {
            return GetBarcodeCode128BByStr(str);
            //if (barCodeFontName.Equals("Code 128"))
            //{
            //    return GetBarcodeCode128BByStr(str);
            //}
            //else
            //{
            //    return "*" + str + "*";
            //}
        }

        public static string GetBarcodeCode128BByStr(string str)
        {
            int total = 104;
            int a = 0;
            int endAsc = 0;
            char endChar = new char();
            for (int i = 0; i < str.Length; i++)
            {
                //转换ASCII数值
                a = Convert.ToInt32(Convert.ToChar(str.Substring(i, 1)));

                //Code 128 SET B 字符集
                if (a >= 32)
                {
                    total += (a - 32) * (i + 1);
                }
                else
                {
                    total += (a + 64) * (i + 1);
                }
            }
            endAsc = total % 103;
            //字符集大于95直接赋值，其它转换后获得
            if (endAsc >= 95)
            {
                switch (endAsc)
                {
                    case 95:
                        endChar = Convert.ToChar("Ã");
                        break;
                    case 96:
                        endChar = Convert.ToChar("Ä");
                        break;
                    case 97:
                        endChar = Convert.ToChar("Å");
                        break;
                    case 98:
                        endChar = Convert.ToChar("Æ");
                        break;
                    case 99:
                        endChar = Convert.ToChar("Ç");
                        break;
                    case 100:
                        endChar = Convert.ToChar("È");
                        break;
                    case 101:
                        endChar = Convert.ToChar("É");
                        break;
                    case 102:
                        endChar = Convert.ToChar("Ê");
                        break;
                    default:
                        endChar = Convert.ToChar("");
                        break;
                }
            }
            else
            {
                endAsc += 32;
                endChar = Convert.ToChar(endAsc);
            }
            //生成Code 128B条码字符串
            string result = "Ì" + str + endChar.ToString() + "Î";
            return result;
        }

        public static string[] SplitBarcode(string barcode)
        {
            if (barcode.Length > 12)
            {
                string[] result = new string[5];
                result[0] = barcode.Substring(0, barcode.Length - 12);
                result[1] = barcode.Substring(barcode.Length - 12, 1);
                result[2] = barcode.Substring(barcode.Length - 11, 4);
                result[3] = barcode.Substring(barcode.Length - 7, 4);
                result[4] = barcode.Substring(barcode.Length - 3);

                return result;
            }
            else
            {
                throw new BusinessException("Hu.Error.HuIdNotExist", barcode);
            }
        }

        public static string[] SplitFGBarcode(string barcode)
        {
            if (barcode.Length >= 9)
            {
                string[] result = new string[5];
                result[0] = barcode.Substring(0, barcode.Length - 9);
                result[1] = barcode.Substring(barcode.Length - 9, 1);
                result[2] = barcode.Substring(barcode.Length - 8, 4);
                result[3] = barcode.Substring(barcode.Length - 4, 4);
                result[4] = barcode.Substring(barcode.Length - 3);

                return result;
            }
            else
            {
                throw new BusinessException("Hu.Error.HuIdNotExist", barcode);
            }
        }

        public static string GetBarcodePrefix(string barcode)
        {
            return barcode.Substring(0, barcode.Length - 3);
        }

        public static int GetBarcodeSeq(string barcode)
        {
            return int.Parse(barcode.Substring(barcode.Length - 3));
        }

    

        public static string GetShiftCode(string barCode) 
        {
            return barCode.Substring(barCode.Length - 4, 1);
        }
    }
}
