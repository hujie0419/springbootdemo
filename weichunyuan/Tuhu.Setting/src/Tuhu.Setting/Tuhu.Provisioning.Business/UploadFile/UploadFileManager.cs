using Common.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.UploadFile
{
    public class UploadFileManager
    {
        private static readonly Common.Logging.ILog logger;
        private static readonly IConnectionManager tuhuLogConn =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerTuhulog;

        static UploadFileManager()
        {
            logger = LogManager.GetLogger(typeof(UploadFileManager));
            dbScopeManagerTuhulog = new DBScopeManager(tuhuLogConn);
        }

        public static Tuple<string, string> UploadFile(HttpPostedFileBase file, FileType type, string user)
        {
            var result = string.Empty;
            var uploadDomain = "/UploadFile/File";
            var batchCode = string.Empty;
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var buffer = new byte[file.ContentLength];
                    file.InputStream.Read(buffer, 0, buffer.Length);
                    var extension = Path.GetExtension(file.FileName);
                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        var getResult = client.UploadFile(new Service.Utility.Request.FileUploadRequest()
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
                            batchCode = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            LoggerManager.InserUploadFiletLog(result, type, file.FileName, batchCode, user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Tuple.Create(result, batchCode);
        }

        public Tuple<string, string> UploadFile(byte[] buffer, FileType type, string extension, string fileName, string user)
        {
            var result = string.Empty;
            var uploadDomain = "/UploadFile/File";
            var batchCode = string.Empty;
            try
            {
                if (buffer.Count() > 0)
                {
                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        var getResult = client.UploadFile(new Service.Utility.Request.FileUploadRequest()
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
                            batchCode = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            LoggerManager.InserUploadFiletLog(result, type, fileName, batchCode, user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Tuple.Create(result, batchCode);
        }

        /// <summary>
        /// 获取上传文件状态
        /// </summary>
        /// <param name="batchcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetFileTaskStatus(string batchcode, FileType type)
        {
            var result = string.Empty;
            try
            {
                result = dbScopeManagerTuhulog.Execute(conn => DalUploadFile.GetFileTaskStatus(conn, batchcode, type));
            }
            catch (Exception ex)
            {
                logger.Error("GetFileTaskStatus", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新上传文件状态
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateFileTaskStatus(string batchCode, FileType type, FileStatus status, FileStatus originStatus)
        {
            var result = false;
            try
            {
                result = dbScopeManagerTuhulog.Execute(conn => DalUploadFile.UpdateFileStatus(conn, batchCode,
                    type, status, originStatus)) > 0;
            }
            catch (Exception ex)
            {
                logger.Error("UpdateFileTaskStatus", ex);
            }
            return result;
        }
    }
}
