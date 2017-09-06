using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Service
{
    public interface ISystemMgr
    {
        IDictionary<com.Sconit.CodeMaster.CodeMaster, IList<CodeDetail>> GetCodeDetailDictionary();
        void LoadEntityPreferenceCache();
        string GetEntityPreferenceValue(EntityPreference.CodeEnum code, bool useCache = true);
        void LoadCodeDetailCache();
        IList<CodeDetail> GetCodeDetails(com.Sconit.CodeMaster.CodeMaster code);
        IList<CodeDetail> GetMultiCodeDetails(IList<string> Multicode, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue);
        CodeDetail GetDefaultCodeDetail(com.Sconit.CodeMaster.CodeMaster code);
        IList<CodeDetail> GetCodeDetails(com.Sconit.CodeMaster.CodeMaster code, bool? includeBlankOption, string blankOptionDescription, string blankOptionValue);
        string GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster code, string value);
        string GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster code, int value);
        string TranslateCodeDetailDescription(string description);
        string TranslaterccpExClassifyDescription(string description);
        string TranslateEntityPreferenceDescription(string description);
        void LoadMenuCache();
        IList<Menu> GetAllMenu();
        User GetMonitorUser();
        void ResetCache();
        IList<CodeDetail> GetCodeDetailDescription(IList<CodeDetail> codeDetailList);
    }
}
