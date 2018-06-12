using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.QRCodeManageConfig
{
    public class QRCodeManageConfigManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("QRCodeManageConfig");
        private QRCodeManageConfigHandler handler = null;
        #endregion

        public QRCodeManageConfigManager()
        {
            handler = new QRCodeManageConfigHandler(DbScopeManager);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(QRCodeManageModel model)
        {
            try
            {
                return handler.Add(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(QRCodeManageModel model)
        {
            try
            {
                return handler.Update(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int Id)
        {
            try
            {
                return handler.Delete(Id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        public QRCodeManageModel GetModel(int Id)
        {
            try
            {
                return handler.GetModel(Id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 检查是否存在TraceId
        /// </summary>
        /// <param name="traceId"></param>
        /// <returns></returns>
        public bool CheckedTraceId(int traceId, int? id)
        {
            try
            {
                return handler.CheckedTraceId(traceId, id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public IEnumerable<QRCodeManageModel> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            try
            {
                return handler.GetListByPage(strWhere, orderby, startIndex, endIndex);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }
    }
}
