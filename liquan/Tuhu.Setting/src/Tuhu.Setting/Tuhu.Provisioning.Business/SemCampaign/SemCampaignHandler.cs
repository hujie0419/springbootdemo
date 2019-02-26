using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SemCampaign
{
    public class SemCampaignHandler
    {
        private readonly IDBScopeManager dbManager;
        public SemCampaignHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;

        } 
        public int CreateApp(AppDownloadCount adc)
        {
            return dbManager.Execute(connection => DALSemCampaign.CreateApp(connection, adc));

        }
        public List<AppDownloadCount> AppCampaignList()
        {
            return dbManager.Execute(connection => DALSemCampaign.AppCampaignList(connection));

        }

        public List<AppDownloadHistory> AppDownloadActionList(int id)
        {
            return dbManager.Execute(connection => DALSemCampaign.AppDownloadActionList(connection, id));

        }

        public int DeleteApp(string deletor, int pkid)
        {
            return dbManager.Execute(connection => DALSemCampaign.DeleteApp(connection, deletor, pkid));

        }

        public int CreateOrDeleteApp(string url, int pkid)
        {
            return dbManager.Execute(connection => DALSemCampaign.CreateOrDeleteApp(connection, url, pkid));


        }

    }
}
