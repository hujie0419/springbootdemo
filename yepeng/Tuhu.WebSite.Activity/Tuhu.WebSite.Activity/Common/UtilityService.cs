using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity.Common
{
    public class UtilityService
    {
        /// <summary>远程保存文件</summary>
        /// <param name="file">文件</param>
        /// <param name="filePath">文件路径。访问路径为http://image.tuhu.test+路径</param>
        /// <param name="maxWidth">图片最大宽，最小值为100。如果不限则为0</param>
        /// <param name="maxHeight">图片最大高，最小值为100。如果不限则为0</param>
        /// <returns>1：保存成功；0：没有保存；-1：没有路径；-2：内容为空；-3：maxWidth和maxHeight最少值为100</returns>
        public static string UploadImage(HttpPostedFileBase file, string filePath, short maxWidth, short maxHeight)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return null;

                if (file == null || file.ContentLength < 1)
                    return null;

                if (maxWidth != 0 && maxHeight != 0 && (maxWidth < 100 || maxHeight < 100))
                    return null;

                using (var client = new FileUploadClient())
                {
                    var buffer = new byte[file.ContentLength];
                    file.InputStream.Read(buffer, 0, buffer.Length);

                    var result = client.UploadImage(new ImageUploadRequest(filePath, buffer, maxWidth, maxHeight));
                    result.ThrowIfException(true);
                    return result.Result;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return null;
        }

        /// <summary>远程保存文件</summary>
        /// <param name="file">文件</param>
        /// <param name="filePath">文件路径。访问路径为http://image.tuhu.test+路径</param>
        /// <param name="maxWidth">图片最大宽，最小值为100。如果不限则为0</param>
        /// <param name="maxHeight">图片最大高，最小值为100。如果不限则为0</param>
        /// <returns>1：保存成功；0：没有保存；-1：没有路径；-2：内容为空；-3：maxWidth和maxHeight最少值为100</returns>
        public static async Task<string> UploadImageAsync(HttpPostedFileBase file, string filePath, short maxWidth, short maxHeight)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return null;

                if (file == null || file.ContentLength < 1)
                    return null;

                if (maxWidth != 0 && maxHeight != 0 && (maxWidth < 100 || maxHeight < 100))
                    return null;

                using (var client = new FileUploadClient())
                {
                    var buffer = new byte[file.ContentLength];
                    file.InputStream.Read(buffer, 0, buffer.Length);

                    var result = await client.UploadImageAsync(new ImageUploadRequest(filePath, buffer, maxWidth, maxHeight));
                    result.ThrowIfException(true);
                    return result.Result;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return null;
        }

        public static async Task<bool> PushArticleMessage(string userId, int batchId,int articleid,string userName)
        {
            try
            {
                using (var client = new Tuhu.Service.Push.TemplatePushClient())
                {
                    var result = await client.PushByUserIDAndBatchIDAsync(new List<string> { userId }, batchId, new Service.Push.Models.Push.PushTemplateLog()
                    {
                        Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string> {
                            { "{{push_title}}",""},
                            { "{{iOS_Nickname}}",userName},
                            { "{{Android_Nickname}}",userName},
                            { "{{App_Nickname}}",userName},
                            { "{{articleid}}",$"{articleid}"},
                        }),
                        DeviceType = Service.Push.Models.Push.DeviceType.MessageBox
                    });
                    result.ThrowIfException(true);
                    return result.Result;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return false;
        }
    }
}