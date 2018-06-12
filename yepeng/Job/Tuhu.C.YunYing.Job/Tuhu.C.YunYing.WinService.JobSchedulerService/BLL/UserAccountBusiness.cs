using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    class UserAccountBusiness
    {
        /// <summary>
        /// 查询大客户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static  SYS_CompanyUser GetCompanyUserInfo(Guid userId)
        {
            SYS_CompanyUser result = null;

            using (var client = new UserAccountClient())
            {
                var serviceResult = client.SelectCompanyUserInfo(userId);
                serviceResult.ThrowIfException(true);
                result = serviceResult.Result;
            }

            return result;
        }
    }
}
