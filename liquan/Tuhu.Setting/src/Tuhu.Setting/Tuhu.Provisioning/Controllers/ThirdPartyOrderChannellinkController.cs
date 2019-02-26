using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.Business.ProductInfomation;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.Common.PinYin;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ProductPrice;
using Tuhu.Provisioning.Models.CarProductModel;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Member;
using Tuhu.Service;
using Tuhu.Provisioning.Business.ThirdPartyOrderChannellink;
using Tuhu.Provisioning.Business.DictionariesManagement;
using Tuhu.Provisioning.DataAccess.Entity.ThirdPartyOrderChannellink;

namespace Tuhu.Provisioning.Controllers
{
    public class ThirdPartyOrderChannellinkController : Controller
    {
        private readonly Lazy<ThirdPartyOrderChannellinkManage> lazyChannellinkManager = new Lazy<ThirdPartyOrderChannellinkManage>();
        private ThirdPartyOrderChannellinkManage ChannellinkManager
        {
            get { return this.lazyChannellinkManager.Value; }
        }

        private readonly Lazy<DictionariesManager> lazyDictionariesManager = new Lazy<DictionariesManager>();
        private DictionariesManager DictionariesManager
        {
            get { return this.lazyDictionariesManager.Value; }
        }

        /// <summary>
        /// 获取三方渠道链接申请列表
        /// </summary>
        /// <param name="orderChannel"></param>
        /// <param name="businessType"></param>
        /// <param name="status"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult GetTPOrderChannellinkList( string orderChannel, string businessType, int status=0, int pageSize = 10, int pageIndex = 1)
        {
            int recordCount = 0;
            var list=ChannellinkManager.GetTPOrderChannellinkList(out recordCount, orderChannel, businessType, status, pageSize, pageIndex);
            return Json(new { data = list,dataCount=recordCount, pageSize = pageSize, pageIndex = pageIndex }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 从三方渠道表获取所有三方渠道key
        /// </summary>
        /// <returns></returns>
        public ActionResult GetThirdPartyOrderChannelList()
        {
            var list = ChannellinkManager.GetThirdPartyOrderChannelList();
            var query = from item in list
                        select new { OrderChannel = item.OrderChannel};
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有订单渠道key
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderChanneKeylList()
        {
            var orderChannellist = DictionariesManager.SelectDeliveryType("orderchannel");
            var query = from orderChannel in orderChannellist
                        select new { OrderChannel = orderChannel.DicKey };
            return Json(new { data = query }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 三方渠道链接-状态操作（启用或禁用）
        /// </summary>
        /// <param name="status"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        [HttpPost]
         public ActionResult UpdateTPOrderChannellinkStatus(int status, int PKID)
        {
            if (status == 1)
            {
                status = -1;
            }else if (status == -1)
            {
                status = 1;
            }
            string lastUpdateBy = User.Identity.Name;
            int result=ChannellinkManager.UpdateTPOrderChannellinkStatus(status, lastUpdateBy, PKID);
            return Json(new { data = result}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加渠道链接
        /// </summary>
        /// <param name="channelModel"></param>
        /// <param name="linkList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrderChannellink(ThirdPartyOrderChannelModel channelModel, List<ThirdPartyOrderChannellinkModel> linkList)
        {
            Encoding gb2312 = Encoding.GetEncoding("GB2312");
            string s = Pinyin.ConvertEncoding(channelModel.OrderChannel, Encoding.UTF8, gb2312);
            string orderChannelEng = Pinyin.GetInitials(s, gb2312).Replace("_", "");
            foreach (var item in linkList)
            {
                item.CreateBy = User.Identity.Name;
                item.LastUpdateBy = User.Identity.Name;
                if (!item.AdditionalRequirement.Contains("无"))
                {
                    if (item.AdditionalRequirement.Contains("IsAggregatePage")) item.IsAggregatePage = true;
                    if (item.AdditionalRequirement.Contains("IsAuthorizedLogin")) item.IsAuthorizedLogin = true;
                    if (item.AdditionalRequirement.Contains("IsPartnerReceivSilver")) item.IsPartnerReceivSilver = true;
                    if (item.AdditionalRequirement.Contains("IsOrderBack")) item.IsOrderBack = true;
                    if (item.AdditionalRequirement.Contains("IsViewOrders")) item.IsViewOrders = true;
                    if (item.AdditionalRequirement.Contains("IsViewCoupons")) item.IsViewCoupons = true;
                    if (item.AdditionalRequirement.Contains("IsContactUserService")) item.IsContactUserService = true;
                    if (item.AdditionalRequirement.Contains("IsBackTop")) item.IsBackTop = true;
                }
                else
                {
                    continue;
                }
            }
            bool isSuccess=ChannellinkManager.AddOrderChannellink(channelModel, orderChannelEng, linkList);
            return Json(new { data = isSuccess });
        }
    }
}