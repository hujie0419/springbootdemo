using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tuhu.Service;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Business.Upload
{
    public static class UpLoadManage
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="VirtualPath"></param>
        /// <param name="file"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static OperationResult<string> UpLoadFile(string VirtualPath, HttpPostedFileBase file, string extension)
        {
            try
            {
                var fileContent = default(byte[]);
                var ism = file.InputStream;
                using (var ms = new MemoryStream())
                {
                    ism.CopyTo(ms);
                    ms.Flush();
                    fileContent = ms.ToArray();
                }

                using (Tuhu.Service.Utility.FileUploadClient fileUploadClient = new Tuhu.Service.Utility.FileUploadClient())
                {
                    var fileUploadRequest = new FileUploadRequest
                    {
                        Contents = fileContent,
                        DirectoryName = VirtualPath,
                        Extension = "." + extension
                    };
                    var result = fileUploadClient.UploadFile(fileUploadRequest);
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
