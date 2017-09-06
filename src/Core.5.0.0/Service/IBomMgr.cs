using System;
using System.Collections.Generic;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.PRD;
using System.IO;

namespace com.Sconit.Service
{
    public interface IBomMgr
    {
        IList<BomDetail> GetOnlyNextLevelBomDetail(string bomCode, DateTime? effectiveDate, bool isMrp = true);

        IList<BomDetail> GetFlatBomDetail(string bomCode, DateTime? effectiveDate, bool isMrp = false);

        IList<BomDetail> GetFlatBomDetail(BomMaster bom, DateTime? effectiveDate, bool isMrp = false);

        string FindItemBom(Item item);

        string FindItemBom(string itemCode);

        IList<BomDetail> GetProductLineWeightAverageBomDetail(string flow);

        void ResetBomCache();

        BomMaster GetCacheBomMaster(string bomCode);

        Dictionary<string, List<BomDetail>> GetCacheAllBomDetail();

        void ImportBom(Stream inputStream);

        void ProcessSectionBom();

        string GetSection(string itemCode);
    }
}
