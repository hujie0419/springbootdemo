using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.BizUserIntergal
{
    public class BizUserIntergal
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BizUserIntergal");
        public bool CheckUser(string phone)
        {
            return new DALUserIntergal().CheckUser(phone);
        }

        public List<UserIntergalMessage> PhoneSearch(string phone)
        {
            return new DALUserIntergal().PhoneSearch(phone);
        }

        public List<UserIntergalDetail> GetUserIntegralDetail(string userId)
        {
            var list = new List<UserIntergalDetail>();
            try
            {
                list = new DALUserIntergal().GetUserIntegralDetail(userId);
            }
            catch (Exception ex)
            {

                Logger.Log(Level.Error, ex, "GetUserIntegralDetail");
                throw;
            }
            return list;
        }

        public int InsertIntergal(InsertIntergalModal insertIntergal)
        {
            return new DALUserIntergal().InsertIntergal(insertIntergal);
        }

        public List<UpdateLogModal> SelectUpdateLog(Guid detailId)
        {
            return new DALUserIntergal().SelectUpdateLog(detailId);
        }
    }
}


