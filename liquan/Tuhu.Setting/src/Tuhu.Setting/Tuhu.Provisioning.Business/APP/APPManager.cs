using System;
using System.Collections.Generic;
using System.Data;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class APPManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("CouponActivityConfig");
        /// <summary>
        /// 获取app发布版本信息 分页
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static List<tbl_app_Versions> GetAppVersions(PagerModel pagerModel)
        {
            return APPDAL.GetAppVersions(pagerModel);
        }

        public static tbl_app_Versions load2(int PKID)
        {
            return APPDAL.load2(PKID);
        }
        public static int upload(string Version_Number, string Download, string UpdateConnent, string Size, string versionCode, string mustUpdate)
        {
            return APPDAL.upload(Version_Number, Download, UpdateConnent, Size, versionCode, mustUpdate);


        }
        public static int upDate(tbl_app_Versions tb)
        {

            return APPDAL.upDate(tb);
        }

        public static int InsertTuhuDiTuiLog(string TuhuUserPhone, string UserPhone, string DeviceID, string Channal, string City, string UserType, string Versions)
        {
            return InsertTuhuDiTuiLog(TuhuUserPhone, UserPhone, DeviceID, Channal, City, UserType, Versions);

         }

        public static DataTable SelectInsuranceCompany()
        {
            return APPDAL.SelectInsuranceCompany();
        }
    }
}
