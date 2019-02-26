using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Tuhu.Provisioning.Business.EmployeeManagement;
using Tuhu.Provisioning.Business.Purchase;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning
{
    public static class SiteUtility
    {
        //public static readonly List<string> EmailAddresses = GetEmailAddress("采购,物流,网络服务部");

        public static string SaveFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0 && string.IsNullOrWhiteSpace(file.FileName))
                return null;

            var imgPath = string.Join("", "/Images/", DateTime.Now.Year, "/", DateTime.Now.Month, "/");
            var path = HostingEnvironment.MapPath("~" + imgPath);
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch { }

            var fileName = Guid.NewGuid().ToString("b") + Path.GetExtension(file.FileName);
            imgPath += fileName;
            file.SaveAs(path + fileName);

            return imgPath;
        }

        public static List<string> GetEmailAddress(string hrGroups)
        {
            using (var gdc = new GungnirDataContext())
            {
                try
                {
                    var emailAddress = gdc.Order_Permissions_SelectEmailAddress(hrGroups).Select(e => e.EmailAddress).ToList();
                    return emailAddress;
                }
                catch
                {
                    return new List<string>();
                }
            }
        }
        public static bool IsActiveByGroups(string hrGroups, string userName)
        {
            return new EmployeeManager().IsActiveByGroups(hrGroups, userName);
        }
        public static bool IsActiveByGroups(string hrGroups, string email, bool isCache)
        {
            isCache = false;
            if (isCache)
            {
                System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;
                if (cache["allUsers"] == null)
                    cache["allUsers"] = new EmployeeManager().GetAllUserGroups();
                List<UserGroups> allGroupUsers = cache["allUsers"] as List<UserGroups>;
                if (allGroupUsers.Where(m => hrGroups.Contains((m.GroupName == null ? "" : m.GroupName) + ",") & m.EmailAddress == email).Count() > 0)
                    return true;
                return false;
            }
            else
                return IsActiveByGroups(hrGroups, email);

        }
        public static readonly List<string> TelMarketingAdmin = new List<string>() { "wanghuijie@tuhu.cn", "gejun@tuhu.cn", "muxiaoling@tuhu.cn", "zhaoyuetong@tuhu.cn", "brantgu@tuhu.cn", "wangminyou@tuhu.cn", "cuirongqing@tuhu.cn", "yangpeipei@tuhu.cn", "sundengjia@tuhu.cn", "baoli@tuhu.cn", "huxiaodong@tuhu.cn", "chenmin@tuhu.cn", "lujialing@tuhu.cn", "chenyue@tuhu.cn", " lixiuzhi@tuhu.cn", "chenxiaoyuan@tuhu.cn ", "renyutao@tuhu.cn", "shenxiaowen@tuhu.cn", "renyingqiang@tuhu.cn" };

        public static List<string> GetPurchaseAuditor(string auditType)
        {
            return new PurchaseManager().GetBatchPurchaseAuditor(auditType);
        }

    }
}