using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.JobCouponConfig
{
    public class JobCouponConfigHandler
    {
        private readonly IDBScopeManager dbManager;
        public JobCouponConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public IEnumerable<JobCouponConfigModel> Select(int pageIndex, int pageSieze)
        {
            Func<SqlConnection, IEnumerable<JobCouponConfigModel>> action = (connection) => JobCouponConfigDAL.Select(connection, pageIndex, pageSieze);
            return dbManager.Execute(action);
        }

        public JobCouponConfigModel SelectById(int id)
        {
            Func<SqlConnection, JobCouponConfigModel> action = (connection) => JobCouponConfigDAL.SelectById(connection,id);
            return dbManager.Execute(action);

        }

        public bool Insert(JobCouponConfigModel model)
        {
            Func<SqlConnection, bool> action = (connection) => JobCouponConfigDAL.Insert(connection, model);
            return dbManager.Execute(action);
        }

        public bool Update(JobCouponConfigModel model)
        {
            Func<SqlConnection, bool> action = (connection) => JobCouponConfigDAL.Update(connection, model);
            return dbManager.Execute(action);
        }
    }
}