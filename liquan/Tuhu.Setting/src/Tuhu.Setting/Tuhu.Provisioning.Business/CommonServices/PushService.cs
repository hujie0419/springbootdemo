using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public static class PushService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(PushService));

        public static bool PushWechatInfoByBatchId(int batchId, PushTemplateLog template)
        {
            var result = false;
            try
            {
                var templateResult = SelectTemplateByBatchID(batchId);
                if (templateResult != null && templateResult.Any())
                {
                    var wxTemplate = templateResult.FirstOrDefault(x => x.DeviceType == DeviceType.WeChat);
                    if (wxTemplate != null)
                    {
                        result = PushTemplate(wxTemplate, template);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static List<PushTemplate> SelectTemplateByBatchID(int batchId)
        {
            List<PushTemplate> result = new List<PushTemplate>();
            try
            {
                using (var client = new TemplatePushClient())
                {
                    var getResult = client.SelectTemplateByBatchID(batchId);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static bool PushTemplate(PushTemplate data, PushTemplateLog template)
        {
            var result = false;
            try
            {
                using (var client = new WeiXinPushClient())
                {
                    var getResult = client.PushByTemplate(data, template);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
