using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.MoveCarQRCode;
using Tuhu.Provisioning.DataAccess.Entity.MoveCarQRCode;
using Newtonsoft.Json;
using System.Data;

namespace Tuhu.Provisioning.Business.MoveCarQRCode
{
    public class MoveCarQRCodeManager
    {
        private DalMoveCarQRCode dal = null;

        public MoveCarQRCodeManager()
        {
            dal = new DalMoveCarQRCode();
        }

        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetLogger(typeof(MoveCarQRCodeManager));

        #region 新增生成

        /// <summary>
        /// 生成途虎挪车二维码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddMoveCarQRCode(MoveCarQRCodeModel model)
        {
            int result = 0;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result= dal.AddMoveCarQRCode(conn, model);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"AddMoveCarQRCode-> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 批量插入途虎挪车二维码
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool BulkSaveMoveCarQRCode(DataTable dt)
        {
            bool result = false;
            try
            {
                dal.BulkSaveMoveCarQRCode(dt);
            }
            catch(Exception e)
            {
                Logger.Error($"BulkSaveMoveCarQRCode", e);
                throw;
            }
            return result;
        }
        /// <summary>
        /// 添加途虎挪车二维码生成记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddMoveCarGenerationRecords(MoveCarGenerationRecordsModel model)
        {
            int result = 0;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result= dal.AddMoveCarGenerationRecords(conn, model);
                }
            }catch(Exception e)
            {
                Logger.Error($"AddMoveCarGenerationRecords-> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 更改生成记录的生成状态为已生成
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool UpdateGenerationRecordsStatus(int pkid)
        {
            bool result = false;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result= dal.UpdateGenerationRecordsStatus(conn, pkid);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateGenerationRecordsStatus-> {pkid}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取总生成下载记录
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public MoveCarTotalRecordsModel GetMoveCarTotalRecord()
        {
            var result = new MoveCarTotalRecordsModel();
            try
            {
                using (var conn = ProcessConnection.OpenGungnirReadOnly)
                {
                    result= dal.GetMoveCarTotalRecord(conn);
                }
            }
            catch(Exception e)
            {
                Logger.Error("GetMoveCarTotalRecord", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 添加或修改途虎挪车二维码总生成下载记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOrUpdateMoveCarTotalRecord(MoveCarTotalRecordsModel model, int totalRecordCount)
        {
            bool result = false;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result= dal.AddOrUpdateMoveCarTotalRecord(conn, model, totalRecordCount);
                }
            }
            catch(Exception e)
            {
                Logger.Error($"AddOrUpdateMoveCarTotalRecord-> {JsonConvert.SerializeObject(model)} -> {totalRecordCount}", e);
                throw;
            }
            return result;
        }

        #endregion

        #region 新增下载

        /// <summary>
        /// 获取途虎挪车二维码列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="downloadNum"></param>
        /// <returns></returns>
        public List<MoveCarQRCodeModel> GetMoveCarQRCodeList(int downloadNum)
        {
            var result = new List<MoveCarQRCodeModel>();
            try
            {
                using (var conn = ProcessConnection.OpenGungnirReadOnly)
                {
                    result= dal.GetMoveCarQRCodeList(conn, downloadNum);
                }
            }
            catch(Exception e)
            {
                Logger.Error($"GetMoveCarQRCodeList-> {downloadNum}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 更新途虎挪车二维码表的下载flag为true
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="downloadNum"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public bool UpdateMoveCarQRCodeDownloadFlag(int downloadNum, string lastUpdateBy)
        {
            bool result = false;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result= dal.UpdateMoveCarQRCodeDownloadFlag(conn, downloadNum, lastUpdateBy);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateMoveCarQRCodeDownloadFlag-> {downloadNum} -> {lastUpdateBy}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 更新途虎挪车二维码表的下载flag为true 并获取更新flag的列表
        /// </summary>
        /// <param name="downloadNum"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public List<MoveCarQRCodeModel> UpdateDownloadFlagAndSelectMoveCarQRCode(int downloadNum, string lastUpdateBy)
        {
            var result = new List<MoveCarQRCodeModel>();
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result = dal.UpdateDownloadFlagAndSelectMoveCarQRCode(conn, downloadNum,lastUpdateBy);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateDownloadFlagAndSelectMoveCarQRCode-> {downloadNum} -> {lastUpdateBy}", e);
                throw;
            }
            return result;
        }
        
            #endregion

            /// <summary>
            /// 获取途虎挪车二维码生成记录列表
            /// </summary>
            /// <param name="conn"></param>
            /// <returns></returns>
            public List<MoveCarGenerationRecordsModel> GetMoveCarGenerationRecordsList(out int recordCount, int pageSize, int pageIndex)
        {
            var result = new List<MoveCarGenerationRecordsModel>();
            recordCount = 0;
            try
            {
                using (var conn = ProcessConnection.OpenGungnirReadOnly)
                {
                    result= dal.GetMoveCarGenerationRecordsList(conn, out recordCount, pageSize, pageIndex);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetMoveCarGenerationRecordsList-> {recordCount} -> {pageSize} -> {pageIndex}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取途虎挪车二维码总生成下载记录
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public MoveCarTotalRecordsModel GetMoveCarTotalRecordsModel()
        {
            var result = new MoveCarTotalRecordsModel();
            try
            {
                using (var conn = ProcessConnection.OpenGungnirReadOnly)
                {
                    result= dal.GetMoveCarTotalRecordsModel(conn);
                }
            }
            catch(Exception e)
            {
                Logger.Error("GetMoveCarTotalRecordsModel", e);
                throw;
            }
            return result;
        }
    }
}
