using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Activity
{
    public class ActivityHandler
    {
        public Tuple<int, List<CouponActivity>> GetActivityList(SqlConnection conn, string type, int pageSize, int pageIndex)
        {
            type = type?.ToLower();
            switch (type)
            {
                case "app":
                    return DALActivity.GetActivityListForApp(conn, pageSize, pageIndex);
                case "web":
                    return DALActivity.GetActivityListForWeb(conn, pageSize, pageIndex);
                default:
                    return Tuple.Create(0, new List<CouponActivity>());
            }
        }

        public bool DeleteActivityConfig(SqlConnection conn,string type,string id, string userName)
        {
            return  DALActivity.DeleteActivityConfig(conn, type, id, userName);
        }
    }
}
