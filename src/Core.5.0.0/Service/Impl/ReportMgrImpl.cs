using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using com.Sconit.Persistence;
using com.Sconit.Entity.VIEW;
using Castle.Windsor;
using com.Sconit.Entity.INV;

namespace com.Sconit.Service.Impl
{
    public class ReportMgrImpl : IReportMgr
    {
        public ISqlQueryDao sqlQueryDao { get; set; }

        public List<object> GetRealTimeLocationDetail(string procedureName, SqlParameter[] parameters)
        {
            List<Object> result = new List<object>();
            DataSet ds = sqlQueryDao.GetDatasetByStoredProcedure(procedureName, parameters);
            result.Add((int)ds.Tables[0].Rows[0].ItemArray[0]);
            var locationDetailList = (from t in ds.Tables[1].AsEnumerable()
                                      select new LocationDetailView
                                      {
                                          Location = t.Field<string>("Location"),
                                          Item = t.Field<string>("Item"),
                                          ManufactureParty = t.Field<string>("ManufactureParty"),
                                          Qty = t.Field<decimal>("Qty"),
                                          LotNo = t.Field<string>("LotNo"),
                                          CsQty = t.Field<decimal>("CsQty"),
                                          QualifyQty = t.Field<decimal>("QualifyQty"),
                                          InspectQty = t.Field<decimal>("InspectQty"),
                                          RejectQty = t.Field<decimal>("RejectQty"),
                                          ATPQty = t.Field<decimal>("ATPQty"),
                                          FreezeQty = t.Field<decimal>("FreezeQty"),
                                          ItemDescription = t.Field<string>("ItemDescription"),
                                          Uom = t.Field<string>("Uom"),
                                          LocationName = t.Field<string>("LocationName"),
                                          MaterialsGroup = t.Field<string>("MaterialsGroup"),
                                          MaterialsGroupDesc = t.Field<string>("MaterialsGroupDesc"),
                                          TransQty = t.Field<decimal>("TransQty"),
                                          SalesTransQty = t.Field<decimal>("SalesTransQty"),
                                          TransQualifyQty = t.Field<decimal>("TransQualifyQty"),
                                          TransRejectQty = t.Field<decimal>("TransRejectQty"),
                                      }).ToList();
            result.Add(locationDetailList);

            return result;
        }

        public List<object> GetHistoryInvAjaxPageData(string procedureName, SqlParameter[] parameters)
        {
            List<Object> result = new List<object>();
            DataSet ds = sqlQueryDao.GetDatasetByStoredProcedure(procedureName, parameters);
            result.Add((int)ds.Tables[0].Rows[0].ItemArray[0]);
            var HistoryInvList = (from t in ds.Tables[1].AsEnumerable()
                                  select new HistoryInventory
                                 {
                                     Location = t.Field<string>("Location"),
                                     Item = t.Field<string>("Item"),
                                     ManufactureParty = t.Field<string>("ManufactureParty"),
                                     LotNo = t.Field<string>("LotNo"),
                                     CsQty = t.Field<decimal>("CsQty"),
                                     QualifyQty = t.Field<decimal>("QualifyQty"),
                                     InspectQty = t.Field<decimal>("InspectQty"),
                                     RejectQty = t.Field<decimal>("RejectQty"),
                                     TobeQualifyQty = t.Field<decimal>("TobeQualifyQty"),
                                     TobeInspectQty = t.Field<decimal>("TobeInspectQty"),
                                     TobeRejectQty = t.Field<decimal>("TobeRejectQty")

                                 }).ToList();
            result.Add(HistoryInvList);

            return result;
        }

        public List<object> GetInventoryAgeAjaxPageData(string procedureName, SqlParameter[] parameters)
        {
            List<Object> result = new List<object>();
            DataSet ds = sqlQueryDao.GetDatasetByStoredProcedure(procedureName, parameters);
            result.Add((int)ds.Tables[0].Rows[0].ItemArray[0]);
            var HistoryInvList = (from t in ds.Tables[1].AsEnumerable()
                                  select new InventoryAge
                                  {
                                      Location = t.Field<string>("Location"),
                                      Item = t.Field<string>("Item"),
                                      Range0 = t.Field<object>("Range0") == null ? "" : Convert.ToDouble(t.Field<object>("Range0")).ToString(),
                                      Range1 = t.Field<object>("Range1") == null ? "" : Convert.ToDouble(t.Field<object>("Range1")).ToString(),
                                      Range2 = t.Field<object>("Range2") == null ? "" : Convert.ToDouble(t.Field<object>("Range2")).ToString(),
                                      Range3 = t.Field<object>("Range3") == null ? "" : Convert.ToDouble(t.Field<object>("Range3")).ToString(),
                                      Range4 = t.Field<object>("Range4") == null ? "" : Convert.ToDouble(t.Field<object>("Range4")).ToString(),
                                      Range5 = t.Field<object>("Range5") == null ? "" : Convert.ToDouble(t.Field<object>("Range5")).ToString(),
                                      Range6 = t.Field<object>("Range6") == null ? "" : Convert.ToDouble(t.Field<object>("Range6")).ToString(),
                                      Range7 = t.Field<object>("Range7") == null ? "" : Convert.ToDouble(t.Field<object>("Range7")).ToString(),
                                      Range8 = t.Field<object>("Range8") == null ? "" : Convert.ToDouble(t.Field<object>("Range8")).ToString(),
                                      Range9 = t.Field<object>("Range9") == null ? "" : Convert.ToDouble(t.Field<object>("Range9")).ToString(),
                                      Range10 = t.Field<object>("Range10") == null ? "" : Convert.ToDouble(t.Field<object>("Range10")).ToString(),
                                      Range11 = t.Field<object>("Range11") == null ? "" : Convert.ToDouble(t.Field<object>("Range11")).ToString()

                                  }).ToList();
            result.Add(HistoryInvList);

            return result;
        }

        public List<object> GetReportTransceiversAjaxPageData(string procedureName, SqlParameter[] parameters)
        {
            List<Object> result = new List<object>();
            DataSet ds = sqlQueryDao.GetDatasetByStoredProcedure(procedureName, parameters);
            result.Add((int)ds.Tables[0].Rows[0].ItemArray[0]);
            var locationDetailList = (from t in ds.Tables[1].AsEnumerable()
                                      select new Transceivers
                                      {
                                          Location = t.Field<string>("Location"),
                                          Item = t.Field<string>("Item"),
                                          //SAPLocation = t.Field<string>("SAPLocation"),
                                          BOPQty = t.Field<decimal>("BOPQty"),
                                          EOPQty = t.Field<decimal>("EOPQty"),
                                          InputQty = t.Field<decimal>("InputQty"),
                                          OutputQty = t.Field<decimal>("OutputQty")

                                      }).ToList();
            result.Add(locationDetailList);

            return result;
        }

        public List<object> GetAuditInspectionPageData(string procedureName, SqlParameter[] parameters)
        {
            List<Object> result = new List<object>();
            DataSet ds = sqlQueryDao.GetDatasetByStoredProcedure(procedureName, parameters);

            var locationDetailList = (from t in ds.Tables[0].AsEnumerable()
                                      select new LocationLotDetail
                                      {
                                          Location = t.Field<string>("Location"),
                                          Item = t.Field<string>("Item"),
                                          ManufactureParty = t.Field<string>("ManufactureParty"),
                                          HuQty = t.Field<decimal>("HuQty"),
                                          LotNo = t.Field<string>("LotNo"),
                                          Id = t.Field<int>("Id"),
                                          UnitCount = t.Field<decimal>("UC"),
                                          IsConsignment = t.Field<bool>("IsCS"),
                                          IsFreeze = t.Field<bool>("IsFreeze"),
                                          HuId = t.Field<string>("HuId"),
                                          CreateDate = t.Field<DateTime>("CreateDate")
                                      }).ToList();
            result.Add(locationDetailList);

            return result;
        }
        public List<object> GetHuAjaxPageData(string procedureName, SqlParameter[] parameters)
        {
            List<Object> result = new List<object>();
            DataSet ds = sqlQueryDao.GetDatasetByStoredProcedure(procedureName, parameters);
            //DateTime minDate=
            result.Add((int)ds.Tables[0].Rows[0].ItemArray[0]);
            //HuId	Item  NoNeedAgingQty AgingStartTime AgingEndTime RefItemCode	ItemDesc	LocTo	UC	Uom	Qty	RemindExpireDate	ExpireDate	RowID	LotNo 
            var HuList = (from t in ds.Tables[1].AsEnumerable()
                          select new Hu
                          {
                              Item = t.Field<string>("Item"),
                              ItemDescription = t.Field<string>("ItemDesc"),
                              ReferenceItemCode = t.Field<string>("RefItemCode"),
                              Uom = t.Field<string>("Uom"),
                              HuId = t.Field<string>("HuId"),
                              LotNo = t.Field<string>("LotNo"),
                              Qty = t.Field<Decimal>("Qty"),
                              Qty0 = t.Field<Decimal>("Qty0"),
                              Qty1 = t.Field<Decimal>("Qty1"),
                              Qty2 = t.Field<Decimal>("Qty2"),
                              LocTo = t.Field<string>("LocTo"),
                              Location = t.Field<string>("Location"),
                              RemindExpireDate = t.Field<DateTime?>("RemindExpireDate"),
                              ExpireDate = t.Field<DateTime?>("ExpireDate"),
                              AgingStartTime = t.Field<DateTime?>("AgingStartTime"),
                              AgingEndTime = t.Field<DateTime?>("AgingEndTime"),
                              UnitCount = t.Field<Decimal>("UC"),
                              IsFreeze = t.Field<int>("IsFreeze"),
                              QualityType = t.Field<com.Sconit.CodeMaster.QualityType>("QualityType"),
                              Bin = t.Field<string>("Bin"),
                              TotalQty = t.Field<Decimal>("TotalQty"),
                              UnAgingQty = t.Field<Decimal>("UnAgingQty"),
                              AgedQty = t.Field<Decimal>("AgedQty"),
                              AgingQty = t.Field<Decimal>("AgingQty"),
                              NoNeedAgingQty = t.Field<Decimal>("NoNeedAgingQty"),
                              SQty = t.Field<Decimal>("Qty"),
                              FreezedQty = t.Field<Decimal>("FreezedQty"),
                              NonFreezeQty = t.Field<Decimal>("NonFreezeQty"),
                              QulifiedQty = t.Field<Decimal>("QulifiedQty"),
                              InspectQty = t.Field<Decimal>("InspectQty"),
                              InQulifiedQty = t.Field<Decimal>("InQulifiedQty")
                          }).ToList();
            result.Add(HuList);

            return result;
        }

    }
}
