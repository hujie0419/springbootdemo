using System;
using System.Collections.Generic;
using System.Linq;
using TheBiz.Common.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class RewardApplicationManager
    {
        public static IEnumerable<RewardApplicationModel> SelectRewardApplicationModels(int pageIndex, int pageSize, int applicationState, string applicationName, string phone, DateTime? createDateTime)
        {
            var dbResult = DalRewardApplication.SelectRewardApplicationModels(pageIndex, pageSize, applicationState, applicationName, phone, createDateTime).ToList();
            dbResult.ForEach(r =>
            {
                r.Phones = dbResult.Select(p => p.Phone).ToList();
            });
            return dbResult;
        }
        public static bool SaveRewardApplicationModels(string phone, int state, string user)
        {
            return DalRewardApplication.SaveRewardApplicationModels(phone, state, user);
        }
        public static RewardApplicationModel FetchRewardApplicationModel(string phone, string[] phones,string applicationName, DateTime? createDateTime)
        {
            return DalRewardApplication.FetchRewardApplicationModel(phone, phones, applicationName, createDateTime);
        }

        public static RewardApplicationModel FetchNextOrPreRewardApplicationModel(string phone)
        {
            return DalRewardApplication.FetchNextOrPreRewardApplicationModel(phone);
        }
    }
}
