using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ConfigLog
{
    public class CommonConfigLogManager
    {
        private DALCommonConfigLog dal = null;
        public CommonConfigLogManager()
        {
            dal = new DALCommonConfigLog();
        }
        /// <summary>
        /// 获取通用日志列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<CommonConfigLogModel> GetCommonConfigLogList(Pagination pagination, string objectId, string objectType, DateTime startTime, DateTime endTime)
        {
            var connString = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            using (var conn = new SqlConnection(connString))
            {
                return dal.GetCommonConfigLogList(conn, pagination, objectId, objectType, startTime, endTime);
            }
        }
        /// <summary>
        /// 获取通用日志列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<CommonConfigLogModel> GetCommonConfigLogList(Pagination pagination, string objectId, string objectType)
        {
            var connString = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            using (var conn = new SqlConnection(connString))
            {
                return dal.GetCommonConfigLogList(conn, pagination, objectId, objectType);
            }
        }
        /// <summary>
        /// 获取通用日志信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public CommonConfigLogModel GetCommonConfigLogInfo(int id)
        {
            var connString = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            using (var conn = new SqlConnection(connString))
            {
                return dal.GetCommonConfigLogInfo(conn, id);
            }
        }

        /// <summary>
        /// 新的 日志 记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ADDCommonConfigLogInfo(CommonConfigLogModel model)
        {
            var connString = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            using (var conn = new SqlConnection(connString))
            {
                return dal.ADDCommonConfigLogInfo(conn, model);
            }
        }


    }
}

