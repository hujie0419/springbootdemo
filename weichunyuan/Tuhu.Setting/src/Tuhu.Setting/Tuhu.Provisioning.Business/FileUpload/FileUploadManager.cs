using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Business.FileUpload
{
    public class FileUploadManager
    {
        private static readonly Common.Logging.ILog Logger;
        static FileUploadManager()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(FileUploadManager));

        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public string UploadImg(byte[] contents)
        {
            var result = "";
            try
            {
                using (var client = new Tuhu.Service.Utility.FileUploadClient())
                {
                    ImageUploadRequest image = new ImageUploadRequest($"activity/image//{DateTime.Now.ToString("yyyyMMdd")}/", contents);
                    var serviceResult = client.UploadImage(image);
                    if (serviceResult.Success)
                    {
                        return serviceResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UploadImg", ex);
                result = "";
            }
            return result;
        }
    }
}
