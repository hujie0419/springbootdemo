using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class AirlinesManager : IAirlinesManager
    {
        #region Private Fields

        static string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        private static readonly ILog logger = LoggerFactory.GetLogger("Airlines");

        private AirlinesHandler handler = null;
        #endregion 

        public AirlinesManager()
        {
            handler = new AirlinesHandler(DbScopeManager);
        }
        //获取所有的客服信息
        public List<Airlines> GetAllAirlines()
        {
            return handler.GetAllAirlines();
        }
        //添加客服信息
        public void AddAirlines(Airlines airlines)
        {
            handler.AddAirlines(airlines);
        }
        //修改客服信息
        public void UpdateAirlines(Airlines airlines)
        {
            handler.UpdateAirlines(airlines);
        }
        //删除客服信息
        public void DeleteAirlines(string id)
        {
            handler.DeleteAirlines(id);
        }
        //获取客服信息，BY  id
        public Airlines GetCouponCategoryByID(string id)
        {
            return handler.GetAirlinesByID(id);
        }



    }
}
