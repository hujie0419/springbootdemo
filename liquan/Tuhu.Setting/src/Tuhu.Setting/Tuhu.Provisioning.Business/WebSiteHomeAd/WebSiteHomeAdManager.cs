using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.WebSiteHomeAd
{
    public class WebSiteHomeAdManager
    {
        /// <summary>
        /// 查找相应ID的广告位及广告位下广告的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AdColumnModel SelectAdDetailByID(string id)
        {
            var dt = DALWebSiteHomeAd.SelectAdDetailByID(id);
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            return new AdColumnModel().Parse(dt).FirstOrDefault();
        }
        /// <summary>
        /// 查找相应ID的广告位下产品的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<AdProductModel> SelectAdProductByID(string id)
        {
            var dt = DALWebSiteHomeAd.SelectAdProductByID(id);
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            return dt.Rows.Cast<DataRow>().Select(row => new AdProductModel(row));
        }

        /// <summary>
        /// 查找所有广告位及广告位下广告的信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AdColumnModel> SelectAllAdDetail(string idstart)
        {
            var dt = DALWebSiteHomeAd.SelectAllAdDetail(idstart);
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            return new AdColumnModel().Parse(dt).ToArray();
        }

        public static int UpdateModuleSetting(string id, int isEnabled)
        {
            return DALWebSiteHomeAd.UpdateModuleSetting(id, isEnabled);
        }

      

        /// <summary>
        /// 查找所有广告位及广告位下产品的信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AdProductModel> SelectAllAdProducts()
        {
            var dt = DALWebSiteHomeAd.SelectAllAdProducts();
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            return dt.Rows.Cast<DataRow>().Select(row => new AdProductModel(row));
        }


        /// <summary>
        /// 新增广告位
        /// </summary>
        /// <param name="AdcModel"></param>
        /// <returns></returns>
        public static int InsertAdDetail(AdColumnModel AdcModel)
        {
            return DALWebSiteHomeAd.InsertAdDetail(AdcModel);
        }
        /// <summary>
        /// 新增广告信息
        /// </summary>
        /// <param name="ad"></param>
        /// <returns></returns>
        public static int InsertAdvertiseDetail(AdvertiseModel ad, IEnumerable<AdProductModel> products)
        {
            return DALWebSiteHomeAd.InsertAdvertiseDetail(ad, products);
        }
        /// <summary>
        /// 更新广告位信息   （默认图片和链接）
        /// </summary>
        /// <param name="AdcModel"></param>
        /// <returns></returns>
        public static int UpdateAdColumn(AdColumnModel AdcModel)
        {
            return DALWebSiteHomeAd.UpdateAdColumn(AdcModel);
        }
        /// <summary>
        /// 更新广告信息
        /// </summary>
        /// <param name="AdvertiseModel"></param>
        /// <returns></returns>
        public static int UpdateAdvertise(AdvertiseModel AdvertiseModel, IEnumerable<AdProductModel> products, out int result)
        {
            return DALWebSiteHomeAd.UpdateAdvertise(AdvertiseModel, products, out result);
        }
        /// <summary>
        /// 删除广告
        /// </summary>
        /// <param name="AdColumnID"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static int DeleteAdvertise(int PKID)
        {
            return DALWebSiteHomeAd.DeleteAdvertise(PKID);
        }

        /// <summary>
        /// 删除单个广告产品
        /// </summary>
        /// <param name="AdColumnID"></param>
        /// <param name="AdvertiseID"></param>
        /// <returns></returns>
        public static int DeleteProducts(string AdColumnID, string AdvertiseID)
        {
            return DALWebSiteHomeAd.DeleteProducts(AdColumnID, AdvertiseID);
        }

    }
}
