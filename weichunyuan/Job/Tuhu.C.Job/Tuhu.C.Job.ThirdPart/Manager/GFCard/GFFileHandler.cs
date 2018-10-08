using Common.Logging;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Dal;
using Tuhu.C.Job.ThirdPart.Utils;

namespace Tuhu.C.Job.ThirdPart.Manager.GFCard
{
    /// <summary>
    /// 文件处理者
    /// </summary>
    class GFFileHandler
    {
        private static readonly int _port = 22; //端口
        private static readonly string _host = ConfigurationManager.AppSettings["TuhuSftpHostIp"]; //sftp地址
        private static readonly string _username = ConfigurationManager.AppSettings["GFSftpUser"]; //用户名
        private static readonly string _password = ConfigurationManager.AppSettings["GFSftpPassword"];//密码
        private static readonly string _remoteRootFilePath = @"/upload";
        private static readonly string _encryptedPath = ".\\TempFile\\Encrypted";
        private static readonly string _decryptedPath = ".\\TempFile\\Decrypted";
        private GFDal _gfDal = GFDal.GetInstance();
        private static readonly int _fileDaysAgo = 3;//从ftp服务器获取近N天的任务
        /// <summary>
        /// 从ftp服务器获取联名卡数据然后创建发券任务
        /// </summary>
        internal void LoadFileThenCreateTasks()
        {
            this.LoadFiles();
            this.DecryptFiles();
            this.CreatePromotionAndRedemptionTask();
            this.ClearTempFile();
        }
        /// <summary>
        /// 从ftp服务器下载联名卡文件
        /// </summary>
        private void LoadFiles()
        {
            ClearTempFile();
            Directory.CreateDirectory(_encryptedPath);
            Directory.CreateDirectory(_decryptedPath);
            using (var client = new SftpClient(_host, _port, _username, _password))
            {
                client.Connect();
                var files = client.ListDirectory(_remoteRootFilePath);//获取文件列表
                if (files != null && files.Any())
                {
                    var startDate = DateTime.Now.AddDays(-_fileDaysAgo).Date;
                    foreach (var file in files)
                    {
                        if (!string.IsNullOrEmpty(file.Name.Replace(".", "")))
                        {
                            var fileDate = GetFileDate(file.Name);
                            if (fileDate != null && fileDate >= startDate)
                            {
                                var filePath = file.FullName;
                                var tmp = client.ReadAllBytes(filePath);
                                var pathFileName = file.Name.StartsWith("\\") ? file.Name : $"\\{file.Name}";
                                File.WriteAllBytes(_encryptedPath + pathFileName, tmp);
                            }
                        }
                    }
                }
                client.Disconnect();
            }
        }
        /// <summary>
        /// 解密文件
        /// </summary>
        private void DecryptFiles()
        {
            var localFiles = Directory.GetFiles(_encryptedPath, "*.*");
            var decryptManager = new GFDecryptHandler();
            foreach (var file in localFiles)
            {
                decryptManager.DecryptFile(file, $"{_decryptedPath}{file.Replace(_encryptedPath, "")}");
            }
        }
        /// <summary>
        /// 根据解密文件创建任务
        /// </summary>
        private void CreatePromotionAndRedemptionTask()
        {
            var decryptedFiles = Directory.GetFiles(_decryptedPath, "*.*");
            foreach (var file in decryptedFiles)
            {
                string line = string.Empty;
                var rows = File.ReadAllLines(file);
                var sourceFileName = file.Replace(@".\TempFile\Decrypted\", "");
                var records = AsyncHelper.RunSync(async () => await _gfDal.SelectGFBankCardRecordsBySourceFile(sourceFileName));
                var tasks = AsyncHelper.RunSync(async () => await _gfDal.SelectGFBankPromotionTasksBySourceFile(sourceFileName));
                var effectiveRows = rows.Where(r => IsEffectiveRow(r)).Distinct();//去除空白行和去重
                if (effectiveRows.Count() <= records.Count() && effectiveRows.Count() <= tasks.Count())//文件已经被写入数据库,并且任务已经创建
                {
                    continue;
                }
                else//文件存到数据库
                {
                    foreach (var row in effectiveRows)
                    {
                        if (IsEffectiveRow(row))
                        {
                            var cardUserArr = row.Split('|');
                            var userName = cardUserArr[0].Trim();
                            var mobile = cardUserArr[1].Trim();
                            var cardLevel = cardUserArr[2].Trim();
                            var businessType = cardUserArr[3].Trim();
                            var gFUserManager = new GFTaskCreator(mobile, userName, cardLevel, businessType, sourceFileName);
                            AsyncHelper.RunSync(async () => await gFUserManager.CreateCardRecordAndTask());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据文件名获取文件日期
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private DateTime? GetFileDate(string fileName)
        {
            DateTime? result = null;
            try
            {
                var fileNameArr = fileName.StartsWith("\\") ? fileName.Replace(@"\CMMS.THYC.JHZXMD.", "").Split('.')
               : fileName.Replace(@"CMMS.THYC.JHZXMD.", "").Split('.');
                var fileDateStr = fileNameArr.Length > 1 ? fileNameArr[0].Replace("S", "") : "";
                if (!string.IsNullOrWhiteSpace(fileDateStr))
                {
                    result = DateTime.ParseExact(fileDateStr, "yyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                JobLogger.GFLogger.Error(ex);
                EmailHelper.SendEmail($"广发联名卡Job异常",ex.ToString());
            }

            return result;
        }
        /// <summary>
        /// 判断广发的一行数据是否有效
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsEffectiveRow(string row)
        {
            return !string.IsNullOrWhiteSpace(row) && row.Split('|').Length == 4;
        }
        /// <summary>
        /// 清理临时文件
        /// </summary>
        private void ClearTempFile()
        {
            if (Directory.Exists(_encryptedPath))
                Directory.Delete(_encryptedPath, true);

            if (Directory.Exists(_decryptedPath))
                Directory.Delete(_decryptedPath, true);
        }
    }
}
