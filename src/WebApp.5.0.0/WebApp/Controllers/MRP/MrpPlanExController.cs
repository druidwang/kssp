using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Service;
using com.Sconit.Service.MRP;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.PRD;
using com.Sconit.Web.Models.MRP;
using System.Text;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.INV;
using com.Sconit.PrintModel.INV;
using AutoMapper;
using com.Sconit.Utility.Report;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity.MRP.ORD;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MrpPlanExController : WebAppBaseController
    {
        public IMrpMgr mrpMgr { get; set; }
        public IPlanMgr planMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        public IMrpOrderMgr mrpOrderMgr { get; set; }
        public IWorkingCalendarMgr workingCalendarMgr { get; set; }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult Plan()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_PlanView")]
        public ActionResult PlanView()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public ActionResult Shift()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Order")]
        public ActionResult Order()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveOrder")]
        public ActionResult ReceiveOrder()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveUrgentOrder")]
        public ActionResult ReceiveUrgentOrder()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Simulation")]
        public ActionResult Simulation()
        {
            return View();
        }
        #region _ItemPlan
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult _ItemPlan()
        {
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult _MrpItemPlanList(string flow, DateTime planVersion, string dateIndex, string section)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanVersion = planVersion;
            ViewBag.DateIndex = dateIndex;
            ViewBag.Section = section;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _SelectItems(DateTime? planVersion, string flow, string dateIndex, string section)
        {
            IList<MrpExItemPlan> planList = new List<MrpExItemPlan>();
            try
            {
                if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(dateIndex) && planVersion.HasValue)
                {
                    var paramList = new List<object>() { planVersion.Value, flow, dateIndex };
                    string hql = " from MrpExItemPlan where PlanVersion =? and ProductLine = ? and DateIndex =? ";
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        hql += " and Section = ? ";
                        paramList.Add(section);
                    }
                    planList = genericMgr.FindAll<MrpExItemPlan>(hql, paramList.ToArray());
                    foreach (var plan in planList)
                    {
                        //var item = this.itemMgr.GetCacheItem(plan.Item);
                        //plan.ItemDescription = item.Description;
                        plan.CurrentQty = Math.Round(plan.TotalQty, 2);
                    }
                    planList = planList.OrderBy(p => p.Sequence).ToList();
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputVersionTimeWeekProductionLineSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return PartialView(new GridModel(planList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public JsonResult _SaveItems(int[] Ids, int[] Sequences, double[] CurrentQtys)
        {
            try
            {
                if (Ids != null && Ids.Length > 0)
                {
                    var planList = genericMgr.FindAllIn<MrpExItemPlan>
                        ("from MrpExItemPlan where Id in(? ", Ids.Where(p => p > 0).Select(p => (object)p));

                    for (int i = 0; i < Ids.Length; i++)
                    {
                        if (Ids[i] > 0)
                        {
                            var plan = planList.Single(p => p.Id == Ids[i]);
                            plan.Sequence = Sequences[i];
                            plan.CurrentQty = CurrentQtys[i];
                        }
                    }
                    mrpMgr.AdjustMrpExItemPlanList(planList);
                }
                object obj = new { };
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedPlanSuccessfully);
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }
        #endregion

        #region _SectionPlan
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult _SectionPlan()
        {
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult _MrpSectionPlanList(string flow, DateTime planVersion, string dateIndex)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanVersion = planVersion;
            ViewBag.DateIndex = dateIndex;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult _SelectSections(DateTime? planVersion, string flow, string dateIndex)
        {
            ViewBag.PlanVersion = planVersion;
            ViewBag.DateIndex = dateIndex;
            IList<MrpExSectionPlan> planList = new List<MrpExSectionPlan>();
            try
            {
                if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(dateIndex) && planVersion.HasValue)
                {
                    string hql = "from MrpExSectionPlan where PlanVersion =? and ProductLine = ? and DateIndex =? ";
                    planList = genericMgr.FindAll<MrpExSectionPlan>(hql,
                        new object[] { planVersion.Value, flow, dateIndex });
                    foreach (var plan in planList)
                    {
                        //var item = this.itemMgr.GetCacheItem(plan.Section);
                        //plan.SectionDescription = item.Description;
                        plan.CurrentQty = Math.Round(plan.ShiftQty, 2);
                    }
                    planList = planList.OrderBy(p => p.Sequence).ToList();
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputVersionTimeWeekProductionLineSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return PartialView(new GridModel(planList));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public JsonResult _SaveSections(int[] Ids, int[] Sequences, string[] Sections, float[] ShiftQtys, string[] Remarks)
        {
            try
            {
                if (Ids != null && Ids.Length > 0)
                {
                    var planList = genericMgr.FindAllIn<MrpExSectionPlan>
                        ("from MrpExSectionPlan where Id in(? ", Ids.Where(p => p > 0).Select(p => (object)p));

                    for (int i = 0; i < Ids.Length; i++)
                    {
                        if (Ids[i] > 0)
                        {
                            var plan = planList.Single(p => p.Id == Ids[i]);
                            plan.Sequence = Sequences[i];
                            plan.ShiftQty = ShiftQtys[i];
                            plan.Remark = Remarks[i];
                        }
                        else
                        {
                            var plan = planList.FirstOrDefault(p => p.Section == Sections[i]);
                            if (plan != null)
                            {
                                var newPlan = Mapper.Map<MrpExSectionPlan, MrpExSectionPlan>(plan);
                                newPlan.Sequence = Sequences[i];
                                newPlan.ShiftQty = ShiftQtys[i];
                                newPlan.Remark = Remarks[i];
                                planList.Add(newPlan);
                            }
                            else if (Sections[i] == BusinessConstants.VIRTUALSECTION)
                            {
                                var newPlan = Mapper.Map<MrpExSectionPlan, MrpExSectionPlan>(planList.First());
                                var virtualItem = mrpMgr.LoadVirtualProdLineExInstance();
                                newPlan.Section = Sections[i];
                                newPlan.Speed = virtualItem.MrpSpeed;
                                newPlan.SpeedTimes = virtualItem.SpeedTimes;
                                newPlan.ProductType = virtualItem.ProductType;
                                newPlan.ApsPriority = virtualItem.ApsPriority;
                                newPlan.Quota = virtualItem.Quota;
                                newPlan.MinLotSize = virtualItem.MinLotSize;
                                newPlan.EconomicLotSize = virtualItem.EconomicLotSize;
                                newPlan.MaxLotSize = virtualItem.MaxLotSize;
                                newPlan.TurnQty = virtualItem.TurnQty;
                                newPlan.Correction = virtualItem.Correction;
                                newPlan.SwitchTime = virtualItem.SwitchTime;
                                newPlan.ShiftType = virtualItem.ShiftType;
                                //newPlan.ShiftType = CodeMaster.ShiftType.ThreeShiftPerDay;
                                newPlan.TotalAps = 1;
                                newPlan.TotalQuota = 1;
                                newPlan.Qty = 0;
                                newPlan.CorrectionQty = 0;
                                //newPlan.UpTime = 8 * 60;
                                newPlan.LatestStartTime = DateTime.Now;
                                newPlan.Sequence = Sequences[i];
                                newPlan.ShiftQty = ShiftQtys[i];
                                newPlan.Remark = Remarks[i];
                                planList.Add(newPlan);
                            }
                            else
                            {
                                throw new BusinessException(Resources.EXT.ControllerLan.Con_CanNotNewAddedTheSection, Sections[i]);
                            }
                        }
                    }
                    this.mrpMgr.AdjustMrpExSectionPlanList(planList);
                }
                object obj = new { };
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedPlanSuccessfully);
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult _WebMrpExSectionPlan(string flow, string itemCode, string dateIndex, DateTime planVersion)
        {
            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(itemCode))
            {
                var prodLineEx = new ProdLineExInstance();
                if (itemCode == BusinessConstants.VIRTUALSECTION)
                {
                    prodLineEx = mrpMgr.LoadVirtualProdLineExInstance();
                }
                else
                {
                    prodLineEx = this.genericMgr.FindAll<ProdLineExInstance>
                      (" from ProdLineExInstance where ProductLine =? and Item =? and DateIndex=? and DateType=? ",
                      new object[] { flow, itemCode, dateIndex, CodeMaster.TimeUnit.Week })
                      .FirstOrDefault();
                    if (prodLineEx == null)
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheProductionLineLackTheSection, itemCode);
                        return null;
                    }
                }
                var plan = Mapper.Map<ProdLineExInstance, MrpExSectionPlan>(prodLineEx);
                //var item = itemMgr.GetCacheItem(itemCode);
                plan.Section = itemCode;
                //plan.SectionDescription = item.Description;
                plan.ProductLine = flow;
                plan.PlanVersion = planVersion;
                plan.StartTime = DateTime.Now;
                plan.WindowTime = DateTime.Now;
                plan.Remark = plan.ProductType.ToString();
                plan.DateIndex = dateIndex;
                plan.PlanNo = mrpMgr.GetExPlanNo(plan, "x");
                return this.Json(plan);
            }
            return null;
        }

        #endregion

        #region _MrpPlanView
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_PlanView")]
        public ActionResult _MrpPlanView(string flow, DateTime? planVersion, string dateIndex)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanVersion = planVersion;
            ViewBag.DateIndex = dateIndex;
            if (planVersion.HasValue && planVersion.HasValue)
            {
                //var paramList = new List<object> { dateIndex, true };
                //var hql = "from MrpExPlanMaster where DateIndex =? and IsActive=? ";
                //if (!string.IsNullOrEmpty(flow))
                //{
                //    hql += " and ProductLine=? ";
                //    paramList.Add(flow);
                //}
                //var preMrpExPlanMasterList = this.genericMgr.FindAll<MrpExPlanMaster>(hql, paramList.ToArray());
                //if (preMrpExPlanMasterList != null && preMrpExPlanMasterList.Count > 0)
                //{
                //    string flows = string.Empty;
                //    foreach (var productLine in preMrpExPlanMasterList.Select(p => p.ProductLine).Distinct())
                //    {
                //        if (flows == string.Empty)
                //        {
                //            flows = productLine;
                //        }
                //        else
                //        {
                //            flows += "/" + productLine;
                //        }
                //    }
                //    SaveWarningMessage("生产线{0}的班产计划已经释放,请确定是否需要覆盖原计划", flows);
                //}
            }
            else
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputVersionTimePlanSearch);
            }
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReleasePlan")]
        public ActionResult _MrpDailyPlan(string flow, DateTime? planVersion, DateTime? planDate)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanVersion = planVersion;
            ViewBag.PlanDate = planDate;
            if (planVersion.HasValue && planVersion.HasValue)
            {
                var paramList = new List<object> { planDate.Value, true };
                var hql = "from MrpExPlanMaster where PlanDate =? and IsActive=? ";
                if (!string.IsNullOrEmpty(flow))
                {
                    hql += " and ProductLine=? ";
                    paramList.Add(flow);
                }
                var preMrpExPlanMasterList = this.genericMgr.FindAll<MrpExPlanMaster>(hql, paramList.ToArray());
                if (preMrpExPlanMasterList != null && preMrpExPlanMasterList.Count > 0)
                {
                    string flows = string.Empty;
                    foreach (var productLine in preMrpExPlanMasterList.Select(p => p.ProductLine).Distinct())
                    {
                        if (flows == string.Empty)
                        {
                            flows = productLine;
                        }
                        else
                        {
                            flows += "/" + productLine;
                        }
                    }
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_ProductionLineShiftPlanAlreadyReleasedPleaseConfirmIfNeedToCover, flows);
                }
            }
            else
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputVersionTimePlanSearch);
            }
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_PlanView")]
        public ActionResult _MrpExSectionPlanHierarchyAjax(DateTime? planVersion, string flow, string dateIndex)
        {
            IList<MrpExSectionPlan> planList = new List<MrpExSectionPlan>();
            try
            {
                if (planVersion.HasValue)
                {
                    var mrpPlanMaster = this.genericMgr.FindById<MrpPlanMaster>(planVersion.Value);
                    dateIndex = mrpPlanMaster.DateIndex;

                    var paramList = new List<object> { planVersion.Value, dateIndex };
                    string hql = " from MrpExSectionPlan p where p.PlanVersion=? and p.DateIndex=? ";
                    if (!string.IsNullOrEmpty(flow))
                    {
                        hql += " and p.ProductLine=? ";
                        paramList.Add(flow);
                    }
                    hql += " order by p.ProductLine,p.PlanDate,p.Sequence";
                    planList = this.genericMgr.FindAll<MrpExSectionPlan>(hql, paramList.ToArray());
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputVersionTimePlanSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return View(new GridModel(planList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReleasePlan")]
        public ActionResult _MrpExSectionDailyPlanHierarchyAjax(DateTime? planVersion, string flow, DateTime? planDate)
        {
            IList<MrpExSectionPlan> planList = new List<MrpExSectionPlan>();
            try
            {
                if (planVersion.HasValue && planDate.HasValue)
                {
                    var paramList = new List<object> { planVersion.Value, planDate.Value };
                    string hql = " from MrpExSectionPlan p where p.PlanVersion=? and p.PlanDate=? ";
                    if (!string.IsNullOrEmpty(flow))
                    {
                        hql += " and p.ProductLine=? ";
                        paramList.Add(flow);
                    }
                    hql += " order by p.ProductLine,p.PlanDate,p.Sequence";
                    planList = this.genericMgr.FindAll<MrpExSectionPlan>(hql, paramList.ToArray());
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputVersionTimePlanSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return View(new GridModel(planList));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_PlanView")]
        public ActionResult _MrpExItemPlanHierarchyAjax(string Id)
        {
            IList<MrpExItemPlan> planList =
                genericMgr.FindAll<MrpExItemPlan>("from MrpExItemPlan as o where o.SectionId = ?", Id);
            MrpExSectionPlan ExSectionPlan = genericMgr.FindById<MrpExSectionPlan>(planList.FirstOrDefault().SectionId);
            foreach (var plan in planList)
            {
                plan.StartTime = ExSectionPlan.StartTime;
                plan.WindowTime = ExSectionPlan.WindowTime;
                //var item = itemMgr.GetCacheItem(plan.Item);
                //plan.ItemDescritpion = item.Description;
            }
            return View(new GridModel(planList));
        }
        #endregion
        #region 挤出班产计划导出
        [SconitAuthorize(Permissions = "Menu.Inventory.ViewInventory")]
        public void ExportEXPlanXLS(DateTime? planVersion, string flow, string dateIndex, DateTime? planDate, int option)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.MaxRowSizeOnPage));
            IList<MrpExSectionPlan> planList = new List<MrpExSectionPlan>();
            try
            {
                if (planDate.HasValue)
                {
                    dateIndex = Utility.DateTimeHelper.GetWeekOfYear(planDate.Value);
                }

                if (!string.IsNullOrEmpty(dateIndex) && planVersion.HasValue)
                {
                    var paramList = new List<object> { planVersion.Value, dateIndex };
                    string hql = " from MrpExSectionPlan p where p.PlanVersion=? and p.DateIndex=? ";
                    if (!string.IsNullOrEmpty(flow))
                    {
                        hql += " and p.ProductLine=? ";
                        paramList.Add(flow);
                    }
                    if (planDate.HasValue)
                    {
                        hql += " and p.PlanDate=? ";
                        paramList.Add(planDate.Value);
                    }
                    hql += " order by p.ProductLine,p.PlanDate,p.Sequence";
                    planList = this.genericMgr.FindAll<MrpExSectionPlan>(hql, paramList.ToArray());
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputVersionTimeWeekSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            var fileName = string.Format("EXPlanSection.{0}-{1}-{2}.xls", planVersion, dateIndex, flow);
            if (option == 1)
            {
                ExportToXLS<MrpExSectionPlan>(fileName, planList);
            }
            else
            {
                List<object> ids = new List<object>();
                foreach (var plan in planList)
                {
                    ids.Add(plan.Id);
                }
                IList<MrpExItemPlan> itemPlanList =
                genericMgr.FindAllIn<MrpExItemPlan>("from MrpExItemPlan as o where o.SectionId in (?", ids);
                foreach (var itemPlan in itemPlanList)
                {
                    itemPlan.StartTime = planList.Where(p => p.Id == itemPlan.SectionId).FirstOrDefault().StartTime;
                    itemPlan.WindowTime = planList.Where(p => p.Id == itemPlan.SectionId).FirstOrDefault().WindowTime;
                }
                fileName = string.Format("EXPlanItem.{0}-{1}-{2}.xls", planVersion, dateIndex, flow);
                ExportToXLS<MrpExItemPlan>(fileName, itemPlanList);
            }
        }
        #endregion
        #region _MrpShiftList
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public ActionResult _MrpShiftList(string flow, DateTime planDate)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanDate = planDate;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public ActionResult _SelectShifts(string flow, DateTime planDate)
        {
            IList<MrpExShiftPlan> shiftList = new List<MrpExShiftPlan>();
            try
            {
                shiftList = mrpMgr.GetMrpExShiftPlanList(planDate, flow) ?? new List<MrpExShiftPlan>();
                foreach (var plan in shiftList)
                {
                    plan.ShiftQty = Math.Round(plan.ShiftQty, 2);
                    if (!string.IsNullOrWhiteSpace(plan.Shift))
                    {
                        try
                        {
                            plan.Shift = this.genericMgr.FindById<ShiftMaster>(plan.Shift).Name;
                        }
                        catch (Exception)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                throw ex;
            }
            return PartialView(new GridModel(shiftList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public JsonResult _SaveShifts(int[] Ids, int[] Sequences, string[] Items, string[] ItemDescs, double[] Qtys, double[] ShiftQtys, string[] ProductTypes, int[] UnitCounts)
        {
            try
            {
                if (Ids != null && Ids.Length > 0)
                {
                    var plans = this.genericMgr.FindAllIn<MrpExShiftPlan>
                        (" from MrpExShiftPlan where Id in(?", Ids.Select(p => (object)p));

                    var productTypes = genericMgr.FindAll<ProductType>().ToDictionary(d => d.Code, d => d);
                    for (int i = 0; i < Ids.Length; i++)
                    {
                        if (Ids[i] > 0)
                        {
                            var item = this.itemMgr.GetCacheItem(Items[i]);
                            var plan = plans.Single(p => p.Id == Ids[i]);
                            //SaveErrorMessage(Resources.EXT.ControllerLan.Con_CanNotFindTheItemSectionbom, plan.Item);
                            if (plan.Item != Items[i] && plan.Item != "299999")
                            {
                                SaveErrorMessage("Only 299999 Item code can be edit.");
                                return Json(new { });
                            }
                            if (plan.Item != Items[i])
                            {
                                plan.Item = Items[i];
                                plan.ItemDescription = item.Description;
                                plan.Uom = item.Uom;
                                plan.UnitCount = (double)item.UnitCount;
                                plan.Bom = item.Code;
                            }

                            if (ItemDescs != null && ItemDescs.Length == Ids.Length)
                            {
                                if (!string.IsNullOrWhiteSpace(ItemDescs[i]))
                                {
                                    plan.ItemDescription = ItemDescs[i];
                                }
                                else
                                {
                                    plan.ItemDescription = item.Description;
                                }
                            }

                            plan.Sequence = Sequences[i];
                            plan.Qty = Qtys[i];
                            plan.ShiftQty = ShiftQtys[i];
                            plan.ProductType = ProductTypes[i];
                            plan.UnitCount = UnitCounts[i];
                            plan.Remark = productTypes.ValueOrDefault(plan.ProductType).Description;
                            plan.IsFreeze = productTypes.ValueOrDefault(plan.ProductType).NeedFreeze;
                            this.genericMgr.Update(plan);
                        }
                    }
                }
                object obj = new { };
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedPlanSuccessfully);
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(new { });
            }
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Plan")]
        public ActionResult _WebMrpExShiftPlan(string flow, string itemCode, string dateIndex)
        {
            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(itemCode))
            {
                var plan = new MrpExShiftPlan();
                var item = itemMgr.GetCacheItem(itemCode);
                plan.Item = item.Code;
                plan.ItemDescription = item.Description;
                plan.Uom = item.Uom;
                plan.UnitCount = (float)item.UnitCount;
                plan.Remark = "SY01";
                plan.Bom = plan.Item;
                var flowDetails = genericMgr.FindAll<FlowDetail>("from FlowDetail where Flow = ? and Item = ?",
                    new object[] { flow, itemCode });

                if (flowDetails != null && flowDetails.Count() > 0)
                {
                    var flowDetail = flowDetails.First();
                    plan.Bom = flowDetail.Bom == null ? plan.Item : flowDetail.Bom;
                    plan.Uom = flowDetail.Uom;
                    plan.UnitCount = (float)flowDetail.UnitCount;
                }

                var bomDetail = bomMgr.GetOnlyNextLevelBomDetail(plan.Bom, DateTime.Now)
                                        .Where(p => p.Item.StartsWith("29")).FirstOrDefault();
                if (bomDetail != null)
                {
                    plan.Section = bomDetail.Item;
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CanNotFindTheItemSectionbom, plan.Item);
                    return null;
                }

                plan.PlanNo = dateIndex.Substring(2, 2) + dateIndex.Substring(5, 2) + "N" + flow + plan.Section;

                return this.Json(plan);
            }
            return null;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public JsonResult AdjustCalendar(string flow, DateTime planDate)
        {
            try
            {
                mrpMgr.AdjustMrpExShiftPlanWorkingCalendar(planDate, flow);
                object obj = new { };
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_RecalculateTimeSuccessfully);
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(new { });
            }
        }
        #endregion

        #region _MrpOrderList
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Order")]
        public ActionResult _MrpOrderList(string flow, string dateIndex)
        {
            ViewBag.Flow = flow;
            ViewBag.DateIndex = dateIndex;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Order")]
        public ActionResult _SelectOrders(string flow, string dateIndex)
        {
            IList<MrpExOrder> shiftList = new List<MrpExOrder>();
            try
            {
                if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(dateIndex))
                {
                    string hql = "from MrpExOrder where ProductLine = ? and DateIndex =? order by Sequence";
                    shiftList = genericMgr.FindAll<MrpExOrder>(hql,
                        new object[] { flow, dateIndex });
                    foreach (var plan in shiftList)
                    {
                        var section = this.genericMgr.FindById<Item>(plan.Section);
                        plan.SectionDescription = section.Description;
                    }
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputWeekProductionLineToSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            FillCodeDetailDescription(shiftList);
            return PartialView(new GridModel(shiftList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Order")]
        public JsonResult _ProcessOrder(string planNo, DateTime startDate, DateTime? closeDate)
        {
            MrpExOrder mrpExOrder = this.genericMgr.FindById<MrpExOrder>(planNo);
            if (mrpExOrder.Status == CodeMaster.OrderStatus.InProcess)
            {
                mrpExOrder.Status = CodeMaster.OrderStatus.Close;
                mrpExOrder.CloseDate = closeDate;
                this.genericMgr.Update(mrpExOrder);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ProductionPlanOrderNumberOutPutSuccessfully, mrpExOrder.PlanNo);
            }
            else if (mrpExOrder.Status == CodeMaster.OrderStatus.Create)
            {
                var inProcessMrpExOrders = this.genericMgr.FindAll<MrpExOrder>
                    (" from MrpExOrder where Status = ? and ProductLine=? ",
                    new object[] { CodeMaster.OrderStatus.InProcess, mrpExOrder.ProductLine });
                if (inProcessMrpExOrders.Count > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseFirstlyToDoOffLineToProductionPlanOrder, inProcessMrpExOrders.First().PlanNo);
                }
                else
                {
                    mrpExOrder.Status = CodeMaster.OrderStatus.InProcess;
                    mrpExOrder.StartDate = startDate;
                    this.genericMgr.Update(mrpExOrder);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ProductionPlanOrderNumberGoLiveSuccessfully, mrpExOrder.PlanNo);
                }
            }
            return Json(new object { });
        }

        #endregion

        #region _MrpReceiveOrderList
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveOrder")]
        public ActionResult _MrpReceiveOrderList(string flow, DateTime? planDate)
        {
            ViewBag.Flow = flow;
            ViewBag.PlanDate = planDate;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveOrder")]
        public ActionResult _SelectReceiveOrders(string flow, DateTime? planDate)
        {
            IList<MrpExShiftPlan> mrpExShiftPlans = new List<MrpExShiftPlan>();
            try
            {
                if (!string.IsNullOrEmpty(flow) && planDate.HasValue)
                {
                    mrpExShiftPlans = GetExShiftPlan(flow, planDate.Value);
                    foreach (var plan in mrpExShiftPlans)
                    {
                        if (plan.Item == BusinessConstants.VIRTUALSECTION)
                        {
                            //plan.Item = plan.Remark;
                        }
                        if (!string.IsNullOrWhiteSpace(plan.Shift))
                        {
                            try
                            {
                                plan.Shift = this.genericMgr.FindById<ShiftMaster>(plan.Shift).Name;
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputWeekProductionLineToSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return PartialView(new GridModel(mrpExShiftPlans));
        }

        private IList<MrpExShiftPlan> GetExShiftPlan(string flow, DateTime planDate)
        {
            IList<MrpExShiftPlan> mrpExShiftPlan = mrpMgr.GetMrpExShiftPlanList(planDate, flow)
                .Where(p => p.ReceivedQty < p.Qty).ToList();

            var cancelList = genericMgr.FindAllWithNativeSql<object[]>(@"select o.ExtOrderNo,d.RecQty from ORD_OrderMstr_4 o
                            join ORD_RecDet_4 d on d.OrderNo = o.OrderNo
                            join ORD_RecMstr_4 r on r.RecNo = d.RecNo
                            where r.Status = 1 and o.ResourceGroup = 20
                            and d.RecQty>0
                            and o.CreateDate>?
                            and o.ExtOrderNo is not null", DateTime.Now.AddHours(-24));

            var cancelDic = (cancelList ?? new List<object[]>())
                .GroupBy(p => (p[0]).ToString())
                .ToDictionary(d => d.Key, d => d.Sum(p => (decimal)(p[1])));

            foreach (var plan in mrpExShiftPlan)
            {
                plan.CurrentQty = plan.UnitCount;
                plan.ReceivedQty -= (double)cancelDic.ValueOrDefault(plan.Id.ToString());
            }
            return mrpExShiftPlan;
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveOrder")]
        public JsonResult _ReceiveOrder(int? id, double? qty, bool? isFreeze)
        {
            var mrpExShiftPlan = genericMgr.FindById<MrpExShiftPlan>(id.Value);
            mrpExShiftPlan.CurrentQty = qty ?? 0;
            mrpExShiftPlan.IsFreeze = isFreeze ?? false;

            ReceiptMaster receiptMaster = mrpOrderMgr.ReceiveExOrder(mrpExShiftPlan);
            List<Hu> huList = receiptMaster.HuList;

            string printUrl = "";
            if (huList != null && huList.Count > 0)
            {
                string huTemplate = huList.First().HuTemplate;
                if (string.IsNullOrWhiteSpace(huTemplate))
                {
                    huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                }
                printUrl = PrintHuList(huList, huTemplate);
                var hu = huList.First();
                SaveSuccessMessage(string.Format(Resources.EXT.ControllerLan.Con_ItemReceiveDSuccessfullyBarcodeNumber, hu.Item, hu.ItemDescription, string.IsNullOrWhiteSpace(hu.HuId) ? hu.OrderNo : hu.HuId));
            }
            else
            {
                SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_BarcodeReceiveUnsuccessfully));
                //SaveSuccessMessage(string.Format("物料 {0}[{1}] 收货成功,本次收货数:{2}", mrpExShiftPlan.Item, mrpExShiftPlan.ItemDescription, qty));
            }
            object obj = new
            {
                PrintUrl = printUrl
            };
            return Json(obj);
        }

        private string PrintHuList(IList<Hu> huList, string huTemplate)
        {
            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
            IList<object> data = new List<object>();
            data.Add(printHuList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(huTemplate, data);
        }

        #endregion
        #region  _MrpReceiveUrgentOrderList
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveUrgentOrder")]
        public ActionResult _MrpReceiveUrgentOrderList(string flow)
        {
            ViewBag.Flow = flow;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveUrgentOrder")]
        public ActionResult _SelectReceiveUrgentOrders(string flow)
        {
            IList<FlowDetail> mrpFlowDetails = new List<FlowDetail>();
            try
            {
                if (!string.IsNullOrEmpty(flow))
                {
                    mrpFlowDetails = flowMgr.GetFlowDetailList(flow, false, false);
                    var productExDic = this.genericMgr.FindAll<ProdLineEx>(
                        " from ProdLineEx where ProductLine=? and StartDate<=? and EndDate>? ",
                        new object[] { flow, DateTime.Now, DateTime.Now }).GroupBy(p => p.Item)
                        .ToDictionary(d => d.Key, d => d.First().ProductType);

                    foreach (var flowData in mrpFlowDetails)
                    {
                        var bomDetail = (bomMgr.GetFlatBomDetail(flowData.Item, DateTime.Now, true) ?? new List<BomDetail>())
                            .FirstOrDefault(p => p.Item.StartsWith("29"));
                        flowData.ProductType = "A";
                        if (bomDetail != null)
                        {
                            var productType = productExDic.ValueOrDefault(bomDetail.Item);
                            if (string.IsNullOrWhiteSpace(productType))
                            {
                                flowData.ProductType = productType;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(flowData.Item))
                        {
                            flowData.ItemDescription = itemMgr.GetCacheItem(flowData.Item).Description;
                        }
                    }
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputProductionLineSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return PartialView(new GridModel(mrpFlowDetails));
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReceiveUrgentOrder")]
        public JsonResult _ReceiveUrgentOrder(int id, double qty, string remark, bool isFreeze, string shift)
        {
            var flowDetail = this.genericMgr.FindById<FlowDetail>(id);
            flowDetail.CurrentQty = qty;

            flowDetail.IsFreeze = isFreeze;
            flowDetail.Shift = shift;

            var productType = genericMgr.FindById<ProductType>(remark);
            flowDetail.ProductType = productType.Code;
            flowDetail.Remark = productType.Description;

            ReceiptMaster receiptMaster = mrpOrderMgr.ReceiveUrgentExOrder(flowDetail);
            List<Hu> huList = receiptMaster.HuList;

            string printUrl = "";
            if (huList != null && huList.Count > 0)
            {
                //打印
                string huTemplate = huList.First().HuTemplate;
                if (string.IsNullOrWhiteSpace(huTemplate))
                {
                    huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                }
                printUrl = PrintHuList(huList, huTemplate);
                var hu = huList.First();
                SaveSuccessMessage(string.Format(Resources.EXT.ControllerLan.Con_ItemUrgentReceivedSuccessfully, hu.Item, hu.ItemDescription, string.IsNullOrWhiteSpace(hu.HuId) ? hu.OrderNo : hu.HuId));
            }
            else
            {
                SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_BarcodeReceiveUnsuccessfully));
                //var receiptDetail = receiptMaster.ReceiptDetails.First();
                //SaveSuccessMessage(string.Format("物料 {0}[{1}] 收货成功,本次收货数:{2}", receiptDetail.Item, receiptDetail.ItemDescription, qty));
            }
            object obj = new
            {
                PrintUrl = printUrl
            };
            return Json(obj);
        }
        #endregion
        #region ShiftView
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ShiftView")]
        public ActionResult ShiftView()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ShiftView")]
        public string _GetShiftView(DateTime planVersion, string flow)
        {
            StringBuilder str = null;// planMgr.GetShiftPlanView(planVersion, flow);
            return str.ToString();
        }
        #endregion

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_AdjustPlan")]
        public ActionResult _AdjustPlanList(DateTime? planVersion, string dateIndex)
        {
            ViewBag.PlanVersion = planVersion;
            ViewBag.DateIndex = dateIndex;
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_AdjustPlan")]
        public ActionResult AdjustPlan()
        {
            return View();

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_AdjustPlan")]
        public ActionResult _SelectAdjustPlanList(DateTime? planVersion, string dateIndex)
        {
            var exPlanAdjustList = new List<ExPlanAdjust>();
            try
            {
                if (planVersion.HasValue && !string.IsNullOrWhiteSpace(dateIndex))
                {
                    string hql = "from MrpExSectionPlan where PlanVersion =? and DateIndex =? order by Sequence";
                    var planList = genericMgr.FindAll<MrpExSectionPlan>(hql, new object[] { planVersion.Value, dateIndex })
                        .Where(p => Math.Round(p.ShiftQty, 2) > 0).ToList();
                    var flows = planList.Select(p => p.ProductLine).OrderBy(p => p).Distinct().ToList();

                    var startDate = planList.Select(p => p.StartTime.Date).Min().Date;
                    //var endDate = planList.Select(p => p.WindowTime.Date).Max();

                    var endDate = Utility.DateTimeHelper.GetEndTime(CodeMaster.TimeUnit.Week, startDate).Date.AddDays(1);

                    foreach (var flow in flows)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var exPlanAdjust = new ExPlanAdjust();
                            exPlanAdjust.Flow = flow;
                            exPlanAdjustList.Add(exPlanAdjust);
                        }
                    }

                    while (startDate <= endDate)
                    {
                        var winTime1 = startDate;
                        var winTime2 = startDate.AddDays(1);
                        int index = (int)startDate.DayOfWeek;
                        if (startDate == endDate)
                        {
                            winTime2 = planList.Select(p => p.WindowTime.Date).Max().Date.AddDays(1);
                            index = 7;
                        }
                        string qtyIndex = "Qty" + index.ToString();
                        string itemIndex = "Item" + index.ToString();

                        foreach (var flow in flows)
                        {
                            var _plans = planList.Where(p => p.ProductLine == flow && p.StartTime <= winTime2 && p.WindowTime > winTime1).ToList();
                            var _exPlanAdjusts = exPlanAdjustList.Where(p => p.Flow == flow).ToList();
                            for (int i = 0; i < _plans.Count(); i++)
                            {
                                if (i >= 5)
                                {
                                    continue;
                                }
                                var plan = _plans[i];
                                var exPlanAdjust = _exPlanAdjusts[i];
                                var qty = plan.ShiftQty *
                                    ((plan.WindowTime < winTime2 ? plan.WindowTime : winTime2) - (plan.StartTime > winTime1 ? plan.StartTime : winTime1)).TotalMinutes
                                    / (plan.WindowTime - plan.StartTime).TotalMinutes;
                                PropertyInfo[] propertyInfo = typeof(ExPlanAdjust).GetProperties();
                                foreach (PropertyInfo pi in propertyInfo)
                                {
                                    if (pi.Name != null && pi.Name == qtyIndex)
                                    {
                                        pi.SetValue(exPlanAdjust, Math.Round(qty, 2), null);
                                    }
                                    else if (pi.Name != null && pi.Name == itemIndex)
                                    {
                                        pi.SetValue(exPlanAdjust, plan.Section, null);
                                    }
                                }
                            }
                        }
                        startDate = winTime2;
                    }
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputWeekProductionLineToSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            FillCodeDetailDescription(exPlanAdjustList);
            return PartialView(new GridModel(exPlanAdjustList));
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_PlanView")]
        public ActionResult _MrpPlanOverallView(string flow, DateTime? planVersion)
        {
            var planList = new List<MrpExSectionPlan>();
            string weekIndex = _GetWeekIndex(planVersion.Value.ToString()).Substring(0,7);
            if (planVersion.HasValue)
            {
                string hql = "from MrpExSectionPlan where PlanVersion =? and DateIndex =? order by Sequence";
                planList = genericMgr.FindAll<MrpExSectionPlan>(hql, new object[] { planVersion.Value, weekIndex})
                   .Where(p => Math.Round(p.ShiftQty, 2) > 0).ToList();
            }
            return PartialView(planList);
        }

        #region   跟踪
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_TrackView")]
        public ActionResult TrackView()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_TrackView")]
        public string _GetMrpPlanTrackView(DateTime planVersion)
        {
            var planMaster = this.genericMgr.FindById<MrpPlanMaster>(planVersion);
            if (Utility.DateTimeHelper.GetWeekIndexDateTo(planMaster.DateIndex) < DateTime.Now)
            {
                return Resources.EXT.ControllerLan.Con_TheVersionAlreadyOverTimeCanNotTrace;
            }
            string ProcedureName = "USP_Busi_MRP_ExPlanTrack";
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@PlanVersion", planVersion);
            return GetTableHtmlByStoredProcedure(ProcedureName, sqlParams);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_TrackView")]
        public ActionResult _MrpPlanTrackView(DateTime? prevVersion, string dateIndex, DateTime? nextVersion)
        {
            var planList = new List<MrpExSectionPlan>();
            var returnList = new List<MrpExSectionPlan>();
            if (prevVersion.HasValue && nextVersion.HasValue && !string.IsNullOrWhiteSpace(dateIndex))
            {
                if (prevVersion.Value >= nextVersion.Value)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CurrentVersionNumberMustGreaterFormerVersionNumber);
                    return Json(null);
                }
                string hql = "from MrpExSectionPlan where PlanVersion in(?,?) and DateIndex =? order by Sequence";
                planList = genericMgr.FindAll<MrpExSectionPlan>(hql, new object[] { prevVersion.Value, nextVersion.Value, dateIndex })
                   .Where(p => Math.Round(p.ShiftQty, 2) > 0).ToList();
            }
            else
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_SearchConditionCanNotBeEmpty);
                return Json(null);
            }
            if (planList != null && planList.Count > 0)
            {
                var prevVersionList = planList.Where(p => p.PlanVersion == prevVersion.Value).ToList();
                var nextVersionList = planList.Where(p => p.PlanVersion == nextVersion.Value).ToList();
                returnList = (from tak in planList
                              group tak by new
                              {
                                  tak.ProductLine,
                                  tak.Section,
                                  tak.SectionDescription,
                                  tak.PlanDate,
                              } into result
                              select new MrpExSectionPlan
                              {
                                  ProductLine = result.Key.ProductLine,
                                  Section = result.Key.Section,
                                  SectionDescription = result.Key.SectionDescription,
                                  ProductType = nextVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     nextVersionList.FirstOrDefault(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).ProductType : string.Empty,
                                  PrevProductType = prevVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     prevVersionList.FirstOrDefault(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).ProductType : string.Empty,
                                  ShiftQty = nextVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     nextVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Sum(s => s.ShiftQty) : 0,
                                  PrevShiftQty = prevVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     prevVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Sum(s => s.ShiftQty) : 0,
                                  StartTime = nextVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     nextVersionList.FirstOrDefault(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).StartTime :
                                    (prevVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     prevVersionList.FirstOrDefault(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).StartTime : System.DateTime.Now),
                                  PlanDate = nextVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     nextVersionList.FirstOrDefault(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).PlanDate :
                                    (prevVersionList.Where(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).Count() > 0 ?
                                     prevVersionList.FirstOrDefault(n => n.ProductLine == result.Key.ProductLine && n.Section == result.Key.Section && n.PlanDate == result.Key.PlanDate).PlanDate : System.DateTime.Now),

                              }).ToList();


                //var q_bd = (from p in prevVersionList
                //           join n in nextVersionList
                //           on p.ProductLine equals n.ProductLine 
                //           select new MrpExSectionPlan {
                //               ProductLine = n.ProductLine,
                //               Section = n.Section,
                //               SectionDescription = n.SectionDescription,
                //               PrevSection = p.Section,
                //               ProductType = n.ProductType,
                //               PrevProductType = p.ProductType,
                //               ShiftQty = n.ShiftQty,
                //               PrevShiftQty = p.ShiftQty,
                //               StartTime=p.StartTime,
                //           });
                //returnList = (from tak in q_bd
                //          group tak by new
                //          {
                //              tak.ProductLine,
                //              tak.Section,
                //              tak.SectionDescription,
                //              tak.PrevSection,
                //              tak.ProductType,
                //              tak.PrevProductType,
                //              tak.ShiftQty,
                //              tak.PrevShiftQty,
                //              tak.StartTime
                //          } into g
                //          select new MrpExSectionPlan
                //          {
                //              ProductLine = g.Key.ProductLine,
                //              Section = g.Key.Section,
                //              SectionDescription = g.Key.SectionDescription,
                //              PrevSection = g.Key.Section,
                //              ProductType = g.Key.ProductType,
                //              PrevProductType = g.Key.ProductType,
                //              ShiftQty = g.Key.ShiftQty,
                //              PrevShiftQty = g.Key.ShiftQty,
                //              StartTime = g.Key.StartTime,
                //          }).ToList();

                //var rgoupByFlow = (from tak in planList
                //                   group tak by tak.ProductLine
                //                       into result
                //                       select new {
                //                           ProductLine = result.Key,
                //                           MrpExSectionPlanList=result.ToList(),
                //                       }).ToList();
                //var group = (from tak in rgoupByFlow
                //             group tak by tak.ProductLine
                //                 into result
                //                 select new
                //                 {
                //                     ProductLine = result.Key,
                //                     MrpExSectionPlanList = result.ToList(),
                //                 }).ToList();
            }
            return PartialView(returnList);
        }
        #endregion

        #region ReleasePlan
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReleasePlan")]
        public ActionResult ReleasePlan()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_ReleasePlan")]
        public JsonResult _MrpReleasePlan(string flow, DateTime planVersion, DateTime planDate)
        {
            try
            {
                var dateIndex = Utility.DateTimeHelper.GetWeekOfYear(planDate);

                var paramList = new List<object> { planVersion, dateIndex, planDate };
                string hql = " from MrpExSectionPlan p where p.PlanVersion=? and p.DateIndex=? and p.PlanDate=? ";
                if (!string.IsNullOrEmpty(flow))
                {
                    hql += " and p.ProductLine=? ";
                    paramList.Add(flow);
                }
                hql += " order by p.ProductLine,p.PlanDate,p.Sequence";
                var mrpExSectionPlanList = this.genericMgr.FindAll<MrpExSectionPlan>(hql, paramList.ToArray());
                foreach (var prodLine in mrpExSectionPlanList.Where(p=>!string.IsNullOrWhiteSpace(p.ProductLine)).OrderBy(p => p.ProductLine).Select(p => p.ProductLine).Distinct())
                {
                    mrpMgr.ReleaseExPlan(prodLine, planVersion, planDate);
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ReleasedSuccessfully);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(new object[] { });
        }
        #endregion

        #region _NewShift
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public ActionResult _NewShift()
        {
            MrpExShiftPlan mrpExShiftPlan = new MrpExShiftPlan();
            return PartialView(mrpExShiftPlan);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public JsonResult _CreateNewShift(MrpExShiftPlan shiftPlan)
        {
            try
            {
                var mrpExShiftPlanList = mrpMgr.GetMrpExShiftPlanList(shiftPlan.PlanDate, shiftPlan.ProductLine);
                shiftPlan.DateIndex = Utility.DateTimeHelper.GetWeekOfYear(shiftPlan.PlanDate);
                if (!string.IsNullOrWhiteSpace(shiftPlan.Item) && shiftPlan.Item != BusinessConstants.VIRTUALSECTION)
                {
                    var section = bomMgr.GetSection(shiftPlan.Item);
                    if (section != null)
                    {
                        shiftPlan.Section = section;
                        var productExs = this.genericMgr.FindAll<ProdLineEx>
                            (" from ProdLineEx where Item =? and  StartDate <=? and EndDate>?",
                            new object[] { section, shiftPlan.PlanDate, shiftPlan.PlanDate })
                            ?? new List<ProdLineEx>();
                        var productEx = productExs.FirstOrDefault(p => p.ProductLine == shiftPlan.ProductLine);
                        if (productEx == null)
                        {
                            productEx = productExs.FirstOrDefault();
                        }
                        if (productEx != null)
                        {
                            shiftPlan.Speed = productEx.MrpSpeed;
                            shiftPlan.ShiftType = (int)productEx.ShiftType;
                        }
                        else
                        {
                            shiftPlan.Speed = 10;
                            shiftPlan.ShiftType = 3;
                        }
                    }
                    else
                    {

                        if (!itemMgr.GetCacheItem(shiftPlan.Item).Description.Contains(Resources.EXT.ControllerLan.Con_ForceMaterial))
                        {
                            shiftPlan.Item = BusinessConstants.VIRTUALSECTION;
                        }
                        shiftPlan.Section = BusinessConstants.VIRTUALSECTION;
                        shiftPlan.Qty = shiftPlan.ShiftQty;
                        shiftPlan.Speed = 10;
                        shiftPlan.ShiftType = 3;
                    }
                }
                else
                {
                    shiftPlan.Item = BusinessConstants.VIRTUALSECTION;
                    shiftPlan.Section = BusinessConstants.VIRTUALSECTION;
                    shiftPlan.Qty = shiftPlan.ShiftQty;
                    shiftPlan.Speed = 10;
                    shiftPlan.ShiftType = 3;
                }
                shiftPlan.PlanNo = shiftPlan.DateIndex.Substring(2, 2) + shiftPlan.DateIndex.Substring(5, 2) + "N" + shiftPlan.ProductLine + shiftPlan.Section;

                shiftPlan.StartTime = DateTime.Parse(shiftPlan.PlanDate.ToString("yyyy-MM-dd ") + shiftPlan.StartTime.ToString("HH:mm"));
                if (!string.IsNullOrWhiteSpace(shiftPlan.ProductType))
                {
                    var productType = genericMgr.FindById<ProductType>(shiftPlan.ProductType);
                    shiftPlan.Remark = productType.Description;
                    shiftPlan.IsFreeze = productType.NeedFreeze;
                }
                var oldShiftPlan = new MrpExShiftPlan();
                if (mrpExShiftPlanList != null)
                {
                    oldShiftPlan = mrpExShiftPlanList.Where(p => p.Section == shiftPlan.Section && p.Shift == shiftPlan.Shift
                        && p.Item == shiftPlan.Item && shiftPlan.Section != BusinessConstants.VIRTUALSECTION).LastOrDefault() ?? oldShiftPlan;
                }
                if (oldShiftPlan.Item != null)
                {
                    shiftPlan.Bom = shiftPlan.Item; //oldShiftPlan.Bom;
                    shiftPlan.DateIndex = oldShiftPlan.DateIndex;
                    shiftPlan.IsCorrection = oldShiftPlan.IsCorrection;
                    shiftPlan.LocationFrom = oldShiftPlan.LocationFrom;
                    shiftPlan.LocationTo = oldShiftPlan.LocationTo;
                    shiftPlan.PlanDate = oldShiftPlan.PlanDate;
                    shiftPlan.PlanStartTime = oldShiftPlan.PlanStartTime;
                    shiftPlan.PlanVersion = oldShiftPlan.PlanVersion;
                    shiftPlan.PlanWindowTime = oldShiftPlan.PlanWindowTime;
                    shiftPlan.RateQty = oldShiftPlan.RateQty;
                    shiftPlan.ReleaseVersion = oldShiftPlan.ReleaseVersion;
                    //shiftPlan.Section = oldShiftPlan.Section;
                    shiftPlan.ShiftType = oldShiftPlan.ShiftType;
                    shiftPlan.Speed = oldShiftPlan.Speed;
                    //shiftPlan.StartTime = oldShiftPlan.WindowTime;
                    shiftPlan.SwitchTime = oldShiftPlan.SwitchTime;
                    shiftPlan.Uom = oldShiftPlan.Uom;
                    shiftPlan.UnitCount = oldShiftPlan.UnitCount;
                    if (string.IsNullOrWhiteSpace(shiftPlan.ItemDescription))
                    {
                        shiftPlan.ItemDescription = oldShiftPlan.ItemDescription;
                    }
                }
                else
                {
                    oldShiftPlan = mrpExShiftPlanList == null ? oldShiftPlan : mrpExShiftPlanList.Last();
                    if (oldShiftPlan.Item == null)
                    {
                        var mrpExPlanMaster = this.genericMgr.FindAll<MrpExPlanMaster>
                             ("from MrpExPlanMaster where PlanDate =?  and IsActive=? ",
                             new object[] { shiftPlan.PlanDate, true }).FirstOrDefault();
                        oldShiftPlan.PlanVersion = mrpExPlanMaster.PlanVersion;
                        oldShiftPlan.ReleaseVersion = DateTime.Now;
                        MrpExPlanMaster mrpExPlanMasterCreate = new MrpExPlanMaster();
                        mrpExPlanMasterCreate.Shift = shiftPlan.Shift;
                        mrpExPlanMasterCreate.ProductLine = shiftPlan.ProductLine;
                        mrpExPlanMasterCreate.PlanDate = shiftPlan.PlanDate;
                        mrpExPlanMasterCreate.DateIndex = mrpExPlanMaster.DateIndex;
                        mrpExPlanMasterCreate.PlanVersion = oldShiftPlan.PlanVersion;
                        mrpExPlanMasterCreate.ReleaseVersion = oldShiftPlan.ReleaseVersion;
                        mrpExPlanMasterCreate.IsActive = true;
                        this.genericMgr.Create(mrpExPlanMasterCreate);
                    }
                    var flow = this.genericMgr.FindById<FlowMaster>(shiftPlan.ProductLine);
                    //用量取ItemPlan里面的值
                    var itemPlan = genericMgr.FindAllIn<MrpExItemPlan>("from MrpExItemPlan as m where m.PlanVersion=? and Item=? and PlanDate=? ", new object[] { oldShiftPlan.PlanVersion, shiftPlan.Item, shiftPlan.PlanDate }).FirstOrDefault();
                    if (itemPlan != null)
                    {
                        shiftPlan.RateQty = itemPlan.RateQty;
                    }
                    else
                    {
                        shiftPlan.RateQty = 1;
                    }
                      
                    shiftPlan.Bom = shiftPlan.Item;
                    //shiftPlan.DateIndex = oldShiftPlan.DateIndex;
                    shiftPlan.IsCorrection = false;
                    shiftPlan.LocationFrom = flow.LocationFrom;
                    shiftPlan.LocationTo = flow.LocationTo;
                    //shiftPlan.PlanDate = oldShiftPlan.PlanDate;
                    shiftPlan.PlanStartTime = DateTime.Now;
                    shiftPlan.PlanVersion = oldShiftPlan.PlanVersion;
                    shiftPlan.PlanWindowTime = DateTime.Now;
                    //shiftPlan.RateQty = 1;
                    shiftPlan.ReleaseVersion = oldShiftPlan.ReleaseVersion;
                    //shiftPlan.Section = BusinessConstants.VIRTUALSECTION;
                    //shiftPlan.ShiftType = 3;
                    //shiftPlan.Speed = 1;
                    //shiftPlan.StartTime = DateTime.Now;
                    //shiftPlan.SwitchTime = oldShiftPlan.SwitchTime;
                    var item = this.itemMgr.GetCacheItem(shiftPlan.Item);
                    shiftPlan.Uom = item.Uom;
                    shiftPlan.UnitCount = (double)item.UnitCount;
                    if (string.IsNullOrWhiteSpace(shiftPlan.ItemDescription))
                    {
                        shiftPlan.ItemDescription = item.Description;
                    }
                }

                shiftPlan.IsNew = true;
                shiftPlan.WindowTime = shiftPlan.StartTime.AddHours(shiftPlan.ShiftQty * (24 / shiftPlan.ShiftType));

                this.genericMgr.Create(shiftPlan);
                //mrpExShiftPlanList.Add(shiftPlan);
                //mrpMgr.AdjustMrpExShiftPlanWorkingCalendar(mrpExShiftPlanList);
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_NewAddedSuccessfully);
                return Json(shiftPlan);
            }
            catch (Exception)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_NewAddedUnsuccessfully);
                return Json(null);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Shift")]
        public JsonResult _WebLoadItem(string itemCode)
        {
            Item item = new Item();
            try
            {
                item = this.genericMgr.FindById<Item>(itemCode);
            }
            catch (Exception) { }
            return Json(item);
        }

        public string GetLastReleaseVersion(string flow, DateTime planDate)
        {
            var mrpExPlanMasters = this.genericMgr.FindAll<MrpExPlanMaster>
            (" from MrpExPlanMaster where ProductLine =? and PlanDate =? and IsActive = ?",
           new object[] { flow, planDate, true });

            if (mrpExPlanMasters != null && mrpExPlanMasters.Count > 0)
            {
                return mrpExPlanMasters.First().ReleaseVersion.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return null;
        }

        #endregion

        #region Demand
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Demand")]
        public ActionResult Demand()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Demand")]
        public string _GetDemand(DateTime planVersion)
        {
            var planMaster = this.genericMgr.FindById<MrpPlanMaster>(planVersion);

            var exPlanGroup = (this.genericMgr.FindAll<MrpExPlan>(" from MrpExPlan where PlanVersion =? and DateIndex =? order by Section",
                new object[] { planVersion, planMaster.DateIndex })
                ?? new List<MrpExPlan>()).GroupBy(p => p.Section);

            var prodLineExList = this.genericMgr.FindAll<ProdLineEx>
                (" from ProdLineEx where StartDate<=? and EndDate>?", new object[] { DateTime.Now, DateTime.Now });

            var speedDic = prodLineExList.GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.Average(q => q.MrpSpeed));

            var flowDic = prodLineExList.GroupBy(p => p.Item).ToDictionary(d => d.Key, d => string.Join(" ", d.Select(p => p.ProductLine)));
            #region 获取库存 在途
            DateTime snapTime;
            //snapTime = this.genericMgr.FindAll<MrpSnapMaster>
            //                (" from MrpSnapMaster where IsRelease = ? and Type=? Order by SnapTime desc",
            //                new object[] { true, CodeMaster.SnapType.Mrp }, 0, 1).First().SnapTime;
            //取离挤出版本最近的快照
            snapTime = genericMgr.FindAllWithNativeSql<DateTime>(@"Select  dbo.GetStartInvSnapTimeByDate ('" + planVersion + "') As SnapTime").FirstOrDefault();
            var inventoryBalances = this.genericMgr.FindAll<InventoryBalance>
                (@"from InventoryBalance as m where m.SnapTime = ?", new object[] { snapTime });

            var transitOrderList = this.genericMgr.FindAll<TransitOrder>
                ("from TransitOrder as m where m.SnapTime = ?", new object[] { snapTime });
            foreach (var transitOrder in transitOrderList)
            {
                var inventoryBalance = new InventoryBalance();
                inventoryBalance.Item = transitOrder.Item;
                inventoryBalance.Qty = transitOrder.ShippedQty - transitOrder.ReceivedQty;
                inventoryBalances.Add(inventoryBalance);
            }

            inventoryBalances = inventoryBalances.GroupBy(p => new { p.Item, p.Location })
                .Select(p =>
                {
                    var inventoryBalance = new InventoryBalance();
                    inventoryBalance.Item = p.Key.Item;
                    inventoryBalance.Location = p.Key.Location;
                    inventoryBalance.Qty = p.Where(q => q.Qty > 0).Sum(q => q.Qty);
                    inventoryBalance.SafeStock = p.Sum(q => q.SafeStock);
                    inventoryBalance.MaxStock = p.Sum(q => q.MaxStock);
                    return inventoryBalance;
                }).ToList();

            var inventoryBalanceDicOfWorkshop = inventoryBalances.Where(p => !string.IsNullOrWhiteSpace(p.Location) && p.Location.Substring(0, 2) == "92").GroupBy(p => p.Item)
                .ToDictionary(d => d.Key, d => d.Sum(b => b.Qty));

            #endregion
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_Section+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_Description+"</th>");
            str.Append("<th style='max-width: 60px;'>"+Resources.EXT.ControllerLan.Con_ProductionLine+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_RequirementLength+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_RequirementShift+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_LatestStartTo+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_HalfFinishedGood+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_HalfFinishedGoodDescription+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_UomLength+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_GrossRequirement+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_SafeInventory+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_TotalInventory+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_WorkshopInventory+"</th>");
            //str.Append("<th>仓库库存</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_WaitingForReceiving+"</th>");
            str.Append("<th>"+Resources.EXT.ControllerLan.Con_WaitingForShipping+"</th>");
            str.Append("<th>" + Resources.EXT.ControllerLan.Con_NetRequirement + "</th>");
            //str.Append("<th>最晚开始</th>");
            str.Append("</tr></thead><tbody>");
            int l = 0;
            foreach (var exPlans in exPlanGroup)
            {
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                int count = exPlans.Count();
                str.Append(string.Format("<td rowspan='{0}'>{1}</td>", count, exPlans.Key));
                str.Append(string.Format("<td rowspan='{0}'>{1}</td>", count, itemMgr.GetCacheItem(exPlans.Key).FullDescription));
                str.Append(string.Format("<td rowspan='{0}' style='max-width: 60px;'>{1}</td>", count, flowDic.ValueOrDefault(exPlans.Key)));
                str.Append(string.Format("<td rowspan='{0}'>{1}</td>", count, exPlans.Sum(p => p.SectionQty).ToString("0")));
                string shiftQty = "-";
                var speed = speedDic.ValueOrDefault(exPlans.Key);
                if (speed > 0)
                {
                    shiftQty = (exPlans.Sum(p => p.SectionQty) / speed / 8 / 60).ToString("0.#");
                }

                str.Append(string.Format("<td rowspan='{0}'>{1}</td>", count, shiftQty));
                string latestStartTime = "-";
                if (exPlans.Min(p => p.LatestStartTime) < DateTime.Now.AddMonths(1))
                {
                    latestStartTime = exPlans.Min(p => p.LatestStartTime).ToString("yyyy-MM-dd");
                }
                str.Append(string.Format("<td rowspan='{0}'>{1}</td>", count, latestStartTime));

                for (int i = 0; i < exPlans.Count(); i++)
                {
                    if (i > 0)
                    {
                        str.Append("<tr>");
                    }
                    var exPlan = exPlans.ElementAt(i);
                    str.Append(string.Format("<td>{0}</td>", exPlan.Item));
                    str.Append(string.Format("<td>{0}</td>", itemMgr.GetCacheItem(exPlan.Item).FullDescription));
                    str.Append(string.Format("<td>{0}</td>", exPlan.RateQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", exPlan.ItemQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", exPlan.SafeStock.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", inventoryBalances.Where(p => p.Item == exPlan.Item).Sum(p=>p.Qty).ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", inventoryBalanceDicOfWorkshop.ValueOrDefault(exPlan.Item).ToString("0.##")));
                    //str.Append(string.Format("<td>{0}</td>", (exPlan.InvQty - inventoryBalanceDicOfWorkshop.ValueOrDefault(exPlan.Item)).ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", exPlan.PlanInQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", exPlan.PlanOutQty.ToString("0.##")));
                    str.Append(string.Format("<td>{0}</td>", exPlan.NetQty.ToString("0.##")));
                    //str.Append(string.Format("<td>{0}</td>", exPlan.LatestStartTime.ToString("yyyy-MM-dd")));
                    str.Append("</tr>");
                }
            }
            str.Append("</tbody></table>");
            return str.ToString();
        }
        #endregion
        #region Export Ex Demand
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Demand")]
        public ActionResult ExportExDemand(DateTime planVersion)
        {
            var table = _GetDemand(planVersion);
            return new DownloadFileActionResult(table, "ExDemand.xls");
        }
        #endregion
        #region Export Ex Track
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_TrackView")]
        public ActionResult ExportExTrack(DateTime planVersion)
        {
            var table = _GetMrpPlanTrackView(planVersion);
            return new DownloadFileActionResult(table, "Extrack.xls");
        }
        #endregion
        public string _GetWeekIndex(string planversion)
        {
            if (string.IsNullOrWhiteSpace(planversion))
            {
                return "";
            }
            var dateindex = queryMgr.FindAll<MrpPlanMaster>(" from MrpPlanMaster m where m.PlanVersion=? ", new object[] { Convert.ToDateTime(planversion) }).FirstOrDefault().DateIndex;
            var dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateindex);
            var str = new StringBuilder(dateindex);
            str.Append("(");
            str.Append(dateFrom.ToString("MM-dd"));
            str.Append(" -> ");
            dateFrom = dateFrom.AddDays(6);
            str.Append(dateFrom.ToString("MM-dd"));
            str.Append(")");
            return str.ToString();
        }
        #region Export simulation
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Simulation")]
        public ActionResult ExportSimulation(DateTime planVersion)
        {
            var table = _GetSimulationList(planVersion);
            return new DownloadFileActionResult(table, "ExSimulationList.xls");
        }
        #endregion
        #region SimulationList
        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlanEx_Simulation")]
        public string _GetSimulationList(DateTime planVersion)
        {
            int tableColumnCount;
            int settledColumnCount;
            SqlParameter[] sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("@PlanVersion", planVersion);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Busi_MRP_GetPlanSimulation_EX", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0];
            settledColumnCount = (int)ds.Tables[0].Rows[0][1];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            StringBuilder strHead = new StringBuilder("<tr>");
            #region Head
            //拼接固定列
            for (int i = 0; i < settledColumnCount; i++)
            {
                str.Append("<th rowspan=\"2\" style=\"min-width: " + (int)ds.Tables[2].Rows[0][i] + "px;text-align:center\">" + (string)ds.Tables[1].Columns[i].ColumnName.ToString() + "</th>");
            }
            int k = 0;
            //返回结果集格式如下 1到settledColumnCount列是固定列，后面的列数由追踪的天数决定
            //物料	    物料描述	            期初库存 。。。	2014/02/17A收	2014/02/17B发	2014/02/17C存    。。。。
            //301185	2CN发动机盖垫2350	    0	     。。。   2800	        0	            2800             。。。。
            for (int i = settledColumnCount; i < tableColumnCount; i++)
            {
                //SP return each column's length
                if (k == 0)
                {
                    str.Append("<th colspan=\"3\" style=\"text-align:center\"  style=\"text-align:center\" >");
                    str.Append((string)ds.Tables[1].Columns[i].ColumnName.ToString().Substring(5, 5));
                    str.Append("</th>");
                }
                k += 1;
                switch (k)
                {
                    case 1:
                        strHead.Append("<th style=\"text-align:center\" >"+Resources.EXT.ControllerLan.Con_InInventory+"</th>"); break;
                    case 2:
                        strHead.Append("<th style=\"text-align:center\" >"+Resources.EXT.ControllerLan.Con_OutInventory+"</th>"); break;
                    case 3:
                        strHead.Append("<th style=\"text-align:center\" >"+Resources.EXT.ControllerLan.Con_Inventory+"</th>"); break;
                    default:
                        break;
                }
                if (k == 3) k = 0;

            }
            str.Append("</tr>"+strHead.ToString()+ "</tr></thead><tbody>");
            #endregion
            int l = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                l++;
                trcss = "";
                if (l % 2 == 0)
                {
                    trcss = "t-alt";
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");
                for (int j = 0; j < tableColumnCount; j++)
                {
                    if (j > 2 &&  Convert.ToDouble(ds.Tables[1].Rows[i][j]) < 0)
                    {
                        str.Append("<td class =\"WarningColor_Red\">");
                    }
                    else
                    {
                        str.Append("<td>");
                    }
                    str.Append(ds.Tables[1].Rows[i][j]);
                    str.Append("</td>");
                }

                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return str.ToString();


        }
        #endregion
    }

    public class ExPlanAdjust
    {
        public string Flow { get; set; }
        public string Item0 { get; set; }
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public string Item4 { get; set; }
        public string Item5 { get; set; }
        public string Item6 { get; set; }
        public string Item7 { get; set; }
        public double? Qty0 { get; set; }
        public double? Qty1 { get; set; }
        public double? Qty2 { get; set; }
        public double? Qty3 { get; set; }
        public double? Qty4 { get; set; }
        public double? Qty5 { get; set; }
        public double? Qty6 { get; set; }
        public double? Qty7 { get; set; }
    }

}
