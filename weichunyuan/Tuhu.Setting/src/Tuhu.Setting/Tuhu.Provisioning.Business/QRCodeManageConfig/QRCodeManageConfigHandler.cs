using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.QRCodeManageConfig
{
    public class QRCodeManageConfigHandler
    {
        private readonly IDBScopeManager dbManager;
        public QRCodeManageConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(QRCodeManageModel model)
        {
            Func<SqlConnection, bool> action = (connection) => QRCodeManageDAL.Add(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(QRCodeManageModel model)
        {
            Func<SqlConnection, bool> action = (connection) => QRCodeManageDAL.Update(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int Id)
        {
            Func<SqlConnection, bool> action = (connection) => QRCodeManageDAL.Delete(connection, Id);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        public QRCodeManageModel GetModel(int Id)
        {
            Func<SqlConnection, QRCodeManageModel> action = (connection) => QRCodeManageDAL.GetModel(connection, Id);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 检查是否存在TraceId
        /// </summary>
        /// <param name="traceId"></param>
        /// <returns></returns>
        public bool CheckedTraceId(int traceId, int? id)
        {
            Func<SqlConnection, bool> action = (connection) => QRCodeManageDAL.CheckedTraceId(connection, traceId, id);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public IEnumerable<QRCodeManageModel> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            Func<SqlConnection, IEnumerable<QRCodeManageModel>> action = (connection) => QRCodeManageDAL.GetListByPage(connection, strWhere, orderby, startIndex, endIndex);
            return dbManager.Execute(action);
        }
    }
}
