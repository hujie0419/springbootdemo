using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class OrderAreaHandler
    {
        private readonly IDBScopeManager dbManager;

        public OrderAreaHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// 根据ID获取配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<OrderArea> GetOrderAreaById(int id)
        {
            Func<SqlConnection, List<OrderArea>> action = (connection) => DALOrderArea.GetOrderAreaById(connection, id);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 根据parentid获取配置信息
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public List<OrderArea> GetOrderAreaByParentId(int parentid)
        {
            Func<SqlConnection, List<OrderArea>> action = (connection) => DALOrderArea.GetOrderAreaByParentId(connection, parentid);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取全部配置信息
        /// </summary>
        /// <returns></returns>
        public List<OrderArea> GetALLOrderArea()
        {
            Func<SqlConnection, List<OrderArea>> action = (connection) => DALOrderArea.GetALLOrderArea(connection);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 修改大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateOrderArea(OrderArea model)
        {
            Func<SqlConnection, bool> action = (connection) => DALOrderArea.UpdateOrderArea(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 添加大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOrderArea(OrderArea model)
        {
            Func<SqlConnection, bool> action = (connection) => DALOrderArea.AddOrderArea(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddChidAreaData(OrderArea model)
        {
            Func<SqlConnection, bool> action = (connection) => DALOrderArea.AddChidAreaData(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateChidAreaData(OrderArea model)
        {
            Func<SqlConnection, bool> action = (connection) => DALOrderArea.UpdateChidAreaData(connection, model);
            return dbManager.Execute(action);
        }
    }
}
