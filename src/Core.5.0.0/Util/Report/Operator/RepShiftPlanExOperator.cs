using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Entity;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.VIEW;
using NPOI.SS.UserModel;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepShiftPlanExOperator : RepTemplate1
    {
        public RepShiftPlanExOperator()
        {

        }

        /**
         * 填充报表        
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list, int sheetIndex)
        {
            try
            {
                if (sheetIndex == 0)
                {
                    //MrpPlanView dailyPlanView = (MrpPlanView)(list[0]);

                    //int headColumnIndex = 5;
                    //int bodyRowIndex = 1;
                    //foreach (var columnCell in dailyPlanView.PlanHead.ColumnCellList)
                    //{
                    //    this.SetRowCell(0, headColumnIndex, columnCell.PlanDate.Date);
                    //    headColumnIndex++;
                    //}
                    //foreach (var planBody in dailyPlanView.PlanBodyList)
                    //{
                    //    int bodyColumnIndex = 5;
                    //    this.SetRowCell(bodyRowIndex, 0, planBody.Flow);
                    //    this.SetRowCell(bodyRowIndex, 1, planBody.Item);
                    //    this.SetRowCell(bodyRowIndex, 2, planBody.ItemDescription);
                    //    this.SetRowCell(bodyRowIndex, 3, planBody.Uom);
                    //    this.SetRowCell(bodyRowIndex, 4, planBody.ShiftQuota);
                    //    foreach (var rowCell in planBody.RowCellList)
                    //    {
                    //        this.SetRowCell(bodyRowIndex, bodyColumnIndex, rowCell.PlanQty);
                    //        bodyColumnIndex++;
                    //    }
                    //    bodyRowIndex++;
                    //}
                }
                else
                {
                    IList<ShiftPlanView> shiftPlanViews = (IList<ShiftPlanView>)(list[1]);

                    for (int i = 1; i < shiftPlanViews.Count(); i++)
                    {
                        ISheet newSheet = workbook.CreateSheet(shiftPlanViews.ElementAt(i).ProductLine);
                        newSheet = workbook.CloneSheet(1);
                        this.sheet = newSheet;
                    }

                    foreach (var shiftPlanView in shiftPlanViews)
                    {
                        int headColumnIndex = 4;
                        int bodyRowIndex = 1;
                        foreach (var columnCell in shiftPlanView.PlanHead.ColumnCellList)
                        {
                            if ((headColumnIndex - 4) % 3 == 0)
                            {
                                this.SetRowCell(0, headColumnIndex, columnCell.PlanDate);
                            }
                            this.SetRowCell(1, headColumnIndex, columnCell.Shift);
                            headColumnIndex++;
                        }
                        foreach (var planBody in shiftPlanView.PlanBodyList)
                        {
                            int bodyColumnIndex = 4;
                            this.SetRowCell(bodyRowIndex, 0, planBody.Item);
                            this.SetRowCell(bodyRowIndex, 1, planBody.ItemDescription);
                            this.SetRowCell(bodyRowIndex, 2, planBody.Uom);
                            this.SetRowCell(bodyRowIndex, 3, planBody.ShiftQuota);
                            foreach (var rowCell in planBody.RowCellList)
                            {
                                this.SetRowCell(bodyRowIndex, bodyColumnIndex, rowCell.Qty);
                                bodyColumnIndex++;
                            }
                            bodyRowIndex++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues( int pageIndex)
        {

        }

    }
}
