using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class FileUploadService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(FileUploadService));

        public static VideoUploadResponse UploadVideo(byte[] buffer, string extension, string fileName,string uploadDomain)
        {
            VideoUploadResponse result = null;          
            try
            {
                if (buffer != null && !string.IsNullOrEmpty(extension) 
                    && !string.IsNullOrEmpty(fileName) 
                    && !string.IsNullOrEmpty(uploadDomain))
                {
                    using (var client = new FileUploadClient())
                    {
                        var getResult = client.UploadVideo(new FileUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = uploadDomain,
                            Extension = extension
                        });
                        getResult.ThrowIfException(true);
                        if (getResult.Success && getResult.Exception == null)
                        {
                            result = getResult.Result;
                            buffer = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static string UploadFile(byte[] buffer, string extension, string fileName, string uploadDomain)
        {
            var result = string.Empty;
            try
            {
                if (buffer != null && !string.IsNullOrEmpty(extension)
                    && !string.IsNullOrEmpty(fileName)
                    && !string.IsNullOrEmpty(uploadDomain))
                {
                    using (var client = new FileUploadClient())
                    {
                        var getResult = client.UploadFile(new FileUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = uploadDomain,
                            Extension = extension
                        });
                        getResult.ThrowIfException(true);
                        if (getResult.Success && getResult.Exception == null)
                        {
                            result = getResult.Result;
                            buffer = null;
                        }
                    }
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
