﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class UserAccountService
    {
        public static IEnumerable<SYS_CompanyInfo> SelectCompanyInfoById(int companyId)
        {
            IEnumerable<SYS_CompanyInfo> result = null;
            using (var client = new UserAccountClient())
            {
                var serviceResult = client.SelectCompanyInfoById(companyId);
                serviceResult.ThrowIfException(true);
                result = serviceResult.Result;
                return result;
            }
        }

        public static IEnumerable<SYS_CompanyUser> GetCompanyUsersByCompanyId(int companyId)
        {
            IEnumerable<SYS_CompanyUser> result = null;

            using (var client = new UserAccountClient())
            {
                var serviceResult = client.GetCompanyUsersByCompanyId(companyId);
                serviceResult.ThrowIfException(true);
                result = serviceResult.Result;
                return result;
            }
        }

        public static User GetUserByMobile(string mobile)
        {
            User result = null;
            using (var client = new UserAccountClient())
            {
                var serviceResult = client.GetUserByMobile(mobile);
                serviceResult.ThrowIfException(true);
                result = serviceResult.Result;
                return result;
            }
        }
    }
}
