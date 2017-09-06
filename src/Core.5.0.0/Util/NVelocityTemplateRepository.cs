using System;
using System.Collections.Generic;
using System.IO;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace com.Sconit.Utility
{
    public class NVelocityTemplateRepository
    {
        public enum TemplateEnum
        {
            /// <summary>
            ///   //Edi转Plan失败
            /// </summary>
            EDI_ScheduleMgrImpl_EDI2Plan = 20001,

            /// <summary>
            ///   //精益引擎创建订单失败
            /// </summary>
            LeanEngine_CreateOrder = 20101,


            /// <summary>
            /// 从SAP获取主数据错误
            /// </summary>
            SAPMasterDataErrorTemplate = 50001,

            /// <summary>
            /// 从SAP获取物料主数据错误
            /// </summary>
            SAPItemErrorTemplate = 50001,

            /// <summary>
            /// 从SAP获取BOM主数据错误
            /// </summary>
            SAPBomErrorTemplate = 50002,

            /// <summary>
            /// 从SAP获取单位转换主数据错误
            /// </summary>
            SAPUomConvErrorTemplate = 50003,

            /// <summary>
            /// 从SAP获取价格单主数据错误
            /// </summary>
            SAPPriceListErrorTemplate = 50004,

            /// <summary>
            /// 从SAP获取供应商主数据错误
            /// </summary>
            SAPSupplierErrorTemplate = 50005,

            /// <summary>
            /// 从SAP获取客户主数据错误
            /// </summary>
            SAPCustomertErrorTemplate = 50006,

            /// <summary>
            /// 传输业务订单数据给SAP失败
            /// </summary>
            ExportBusDataToSAPErrorTemplate = 51000,

            /// <summary>
            /// 传输业务订单数据给SAP失败
            /// </summary>
            ExportBusDataAdjustToSAPErrorTemplate = 52000,

        }

        public IDictionary<int, string> templateNameDictionary { get; set; }
        private VelocityEngine engine;

        public NVelocityTemplateRepository(string templateDirectory)
        {
            engine = new VelocityEngine();
            ExtendedProperties props = new ExtendedProperties();
            props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templateDirectory);

            engine.Init(props);
        }

        public string RenderTemplate(TemplateEnum template, IDictionary<string, object> data)
        {
            return RenderTemplate(templateNameDictionary[(int)template], data);
        }

        public string RenderTemplate(string template, IDictionary<string, object> data)
        {
            Template vmTemplate = engine.GetTemplate(template);
            //template.Encoding = Encoding.UTF8.BodyName;
            var context = new VelocityContext();

            IDictionary<string, object> templateData = data ?? new Dictionary<string, object>();
            foreach (var key in templateData.Keys)
            {
                context.Put(key, templateData[key]);
            }

            using (var writer = new StringWriter())
            {
                vmTemplate.Merge(context, writer);
                return writer.GetStringBuilder().ToString();
            }
        }
    }

    public class ErrorMessage
    {
        public NVelocityTemplateRepository.TemplateEnum Template { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
