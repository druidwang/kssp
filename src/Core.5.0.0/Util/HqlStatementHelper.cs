using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Web.Mvc;
using System;

namespace com.Sconit.Utility
{
    public static class HqlStatementHelper
    {
        public enum LikeMatchMode
        {
            Anywhere,
            Start,
            End,
        }


        public static void AddNotEqStatement(string fieldName, object fieldValue, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValue == null)
            {
                return;
            }

            if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
            {
                if (!string.IsNullOrWhiteSpace(fieldAlias))
                {
                    fieldName = fieldAlias + "." + fieldName;
                }

                if (whereStatement == string.Empty)
                {
                    whereStatement = " where " + fieldName + " != ?";
                }
                else
                {
                    whereStatement += " and " + fieldName + " != ?";
                }
                if (fieldValue.GetType() == typeof(string))
                {
                    param.Add(fieldValue.ToString().Trim());
                }
                else
                {
                    param.Add(fieldValue);
                }
            }
        }


        public static void AddEqStatement(string fieldName, object fieldValue, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValue == null)
            {
                return;
            }

            if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
            {
                if (!string.IsNullOrWhiteSpace(fieldAlias))
                {
                    fieldName = fieldAlias + "." + fieldName;
                }

                if (whereStatement == string.Empty)
                {
                    whereStatement = " where " + fieldName + " = ?";
                }
                else
                {
                    whereStatement += " and " + fieldName + " = ?";
                }

                if (fieldValue.GetType() == typeof(string))
                {
                    param.Add(fieldValue.ToString().Trim());
                }
                else
                {
                    param.Add(fieldValue);
                }
            }
        }

        public static void AddGeStatement(string fieldName, object fieldValue, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValue == null)
            {
                return;
            }

            if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
            {
                if (!string.IsNullOrWhiteSpace(fieldAlias))
                {
                    fieldName = fieldAlias + "." + fieldName;
                }

                if (whereStatement == string.Empty)
                {
                    whereStatement = " where " + fieldName + " >= ?";
                }
                else
                {
                    whereStatement += " and " + fieldName + " >= ?";
                }

                if (fieldValue.GetType() == typeof(string))
                {
                    param.Add(fieldValue.ToString().Trim());
                }
                else
                {
                    param.Add(fieldValue);
                }
            }
        }

        public static void AddGtStatement(string fieldName, object fieldValue, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValue == null)
            {
                return;
            }

            if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
            {
                if (!string.IsNullOrWhiteSpace(fieldAlias))
                {
                    fieldName = fieldAlias + "." + fieldName;
                }

                if (whereStatement == string.Empty)
                {
                    whereStatement = " where " + fieldName + " > ?";
                }
                else
                {
                    whereStatement += " and " + fieldName + " > ?";
                }

                if (fieldValue.GetType() == typeof(string))
                {
                    param.Add(fieldValue.ToString().Trim());
                }
                else
                {
                    param.Add(fieldValue);
                }
            }
        }

        public static void AddLeStatement(string fieldName, object fieldValue, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValue == null)
            {
                return;
            }

            if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
            {
                if (!string.IsNullOrWhiteSpace(fieldAlias))
                {
                    fieldName = fieldAlias + "." + fieldName;
                }

                if (whereStatement == string.Empty)
                {
                    whereStatement = " where " + fieldName + " <= ?";
                }
                else
                {
                    whereStatement += " and " + fieldName + " <= ?";
                }

                if (fieldValue.GetType() == typeof(string))
                {
                    param.Add(fieldValue.ToString().Trim());
                }
                else
                {
                    param.Add(fieldValue);
                }
            }
        }

        public static void AddLtStatement(string fieldName, object fieldValue, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValue == null)
            {
                return;
            }

            if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
            {
                if (!string.IsNullOrWhiteSpace(fieldAlias))
                {
                    fieldName = fieldAlias + "." + fieldName;
                }

                if (whereStatement == string.Empty)
                {
                    whereStatement = " where " + fieldName + " < ?";
                }
                else
                {
                    whereStatement += " and " + fieldName + " < ?";
                }

                if (fieldValue.GetType() == typeof(string))
                {
                    param.Add(fieldValue.ToString().Trim());
                }
                else
                {
                    param.Add(fieldValue);
                }
            }
        }

        public static void AddLikeStatement(string fieldName, object fieldValue, LikeMatchMode matchMode, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValue == null)
            {
                return;
            }

            if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
            {
                if (!string.IsNullOrWhiteSpace(fieldAlias))
                {
                    fieldName = fieldAlias + "." + fieldName;
                }

                if (whereStatement == string.Empty)
                {
                    whereStatement = " where " + fieldName + " like ?";
                }
                else
                {
                    whereStatement += " and " + fieldName + " like ?";
                }

                if (fieldValue.GetType() == typeof(string))
                {
                    if (matchMode == LikeMatchMode.Anywhere)
                    {
                        param.Add("%" + fieldValue.ToString().Trim() + "%");
                    }
                    else if (matchMode == LikeMatchMode.Start)
                    {
                        param.Add(fieldValue.ToString().Trim() + "%");
                    }
                    else if (matchMode == LikeMatchMode.End)
                    {
                        param.Add("%" + fieldValue.ToString().Trim());
                    }
                }
                else
                {
                    if (matchMode == LikeMatchMode.Anywhere)
                    {
                        param.Add("%" + fieldValue + "%");
                    }
                    else if (matchMode == LikeMatchMode.Start)
                    {
                        param.Add(fieldValue + "%");
                    }
                    else if (matchMode == LikeMatchMode.End)
                    {
                        param.Add("%" + fieldValue);
                    }
                }
            }
        }

        public static void AddBetweenStatement(string fieldName, object fieldValue1, object fieldValue2, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (!string.IsNullOrWhiteSpace(fieldAlias))
            {
                fieldName = fieldAlias + "." + fieldName;
            }

            if (whereStatement == string.Empty)
            {
                whereStatement = " where " + fieldName + " between ? and ?";
            }
            else
            {
                whereStatement += " and " + fieldName + " between ? and ?";
            }

            if (fieldValue1.GetType() == typeof(string) && fieldValue2.GetType() == typeof(string))
            {
                param.Add(fieldValue1.ToString().Trim());
                param.Add(fieldValue2.ToString().Trim());
            }
            else
            {
                param.Add(fieldValue1);
                param.Add(fieldValue2);
            }
        }

        public static string GetSortingStatement(IList<SortDescriptor> SortDescriptors)
        {
            if (SortDescriptors != null && SortDescriptors.Count > 0)
            {
                string sortingStatement = " order by";
                foreach (SortDescriptor sort in SortDescriptors)
                {
                    sortingStatement += " " + sort.Member + " " + (sort.SortDirection == ListSortDirection.Descending ? "desc" : "asc");
                }

                return sortingStatement;
            }

            return string.Empty;
        }

        public static void AddInStatement(string fieldName, object[] fieldValues, string fieldAlias, ref string whereStatement, IList<object> param)
        {
            if (fieldValues == null)
            {
                return;
            }
            int i = 0;
            foreach (object fieldValue in fieldValues)
            {
                if (fieldValue.GetType() != typeof(string) || !string.IsNullOrWhiteSpace((string)fieldValue))
                {
                    if (i == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(fieldAlias))
                        {
                            fieldName = fieldAlias + "." + fieldName;
                        }
                        if (whereStatement == string.Empty)
                        {
                            whereStatement = " where " + fieldName + " in (?";
                        }
                        else
                        {
                            whereStatement += " and " + fieldName + " in (?";
                        }
                    }
                    else
                    {
                        whereStatement += ",?";
                    }

                    if (fieldValue.GetType() == typeof(string))
                    {
                        param.Add(fieldValue.ToString().Trim());
                    }
                    else
                    {
                        param.Add(fieldValue);
                    }
                }
                ++i;
            }
            if (!string.IsNullOrEmpty(whereStatement))
            {
                whereStatement += ")";
            }
        }

    }
}
