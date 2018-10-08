using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.Crm;
using Tuhu.Provisioning.DataAccess.DAO.Discovery;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.Business.Discovery
{
    public class UserBlackListBLL
    {
        //加入黑名单
        public static bool AddBlackList(UserBlackList model)
        {
            try
            {
                return UserBlackListDAL.AddBlackList(ProcessConnection.OpenMarketing, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //删除黑名单
        public static bool DeleteBlackList(string UserId)
        {
            try
            {
                return UserBlackListDAL.DeleteBlackList(ProcessConnection.OpenMarketing, UserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
