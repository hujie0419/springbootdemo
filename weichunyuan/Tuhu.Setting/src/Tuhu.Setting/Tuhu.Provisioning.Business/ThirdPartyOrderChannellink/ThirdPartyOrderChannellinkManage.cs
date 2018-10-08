using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json.Linq;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Provisioning.DataAccess.Entity.ThirdPartyOrderChannellink;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ThirdPartyOrderChannellink
{
    public class ThirdPartyOrderChannellinkManage
    {
        private static readonly Common.Logging.ILog Logger;

        /// <summary>
        /// 获取三方渠道链接申请列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="orderChannel"></param>
        /// <param name="businessType"></param>
        /// <param name="status"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<ThirdPartyOrderChannellinkModel> GetTPOrderChannellinkList(out int recordCount, string orderChannel, string businessType, int status = 0, int pageSize = 10, int pageIndex = 1)
        {
            try
            {
                var result= DataAccess.DAO.DALThirdPartyOrderChannelLink.GetTPOrderChannellinkList(out recordCount, orderChannel, businessType, status , pageSize , pageIndex);
                foreach (var item in result)
                {
                    var addList = new List<string>();
                    if (item.IsAggregatePage)
                    {
                        addList.Add("聚合页");
                    }
                    if (item.IsAuthorizedLogin)
                    {
                        addList.Add("授权登录");
                    }
                    if (item.IsPartnerReceivSilver)
                    {
                        addList.Add("合作方收银");
                    }
                    if (item.IsOrderBack)
                    {
                        addList.Add("订单回传");
                    }
                    if (item.IsViewOrders)
                    {
                        addList.Add("查看订单（浮层）");
                    }
                    if (item.IsViewCoupons)
                    {
                        addList.Add("查看优惠券（浮层）");
                    }
                    if (item.IsContactUserService)
                    {
                        addList.Add("联系客服（浮层）");
                    }
                    if (item.IsBackTop)
                    {
                        addList.Add("返回顶部（浮层）");
                    }
                    var additionalResult = string.Join("、", addList);
                    if (string.IsNullOrWhiteSpace(additionalResult))
                    {
                        additionalResult = "无";
                    }
                    item.AdditionalRequirement = additionalResult;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("GetTPOrderChannellinkList", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 从三方渠道表获取所有三方渠道key
        /// </summary>
        /// <returns></returns>
        public  List<ThirdPartyOrderChannelModel> GetThirdPartyOrderChannelList()
        {
            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.GetTPOrderChannelList();
            }
            catch (Exception ex)
            {
                Logger.Error("GetThirdPartyOrderChannelList", ex);
                throw ex;
            }
        }

        /// <summary>
        ///  三方渠道链接-状态操作（启用或禁用）
        /// </summary>
        /// <param name="status"></param>
        /// <param name="lastUpdateBy"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public int UpdateTPOrderChannellinkStatus(int status, string lastUpdateBy,int PKID)
        {
            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.UpdateTPOrderChannellinkStatus(status, lastUpdateBy,PKID);
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateTPOrderChannellinkStatus", ex);
                throw ex;
            }
        }

        #region 添加渠道链接
        /// <summary>
        /// 根据订单渠道key获取三方订单渠道集合
        /// </summary>
        /// <param name="OrderChannel"></param>
        /// <returns></returns>
        public static ThirdPartyOrderChannelModel GetTPOrderChannelListByOrderChannel(string orderChannel)
        {
            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.GetTPOrderChannelListByOrderChannel(orderChannel);
            }
            catch (Exception ex)
            {
                Logger.Error("GetTPOrderChannelListByOrderChannel", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取某订单渠道key下的所有渠道链接
        /// </summary>
        /// <param name="orderChanneID"></param>
        /// <returns></returns>
        public static List<ThirdPartyOrderChannellinkModel> GetOrderChannelLinkByOrderChanneID(int orderChanneID)
        {
            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.GetOrderChannelLinkByOrderChanneID(orderChanneID);
            }
            catch (Exception ex)
            {
                Logger.Error("GetOrderChannelLinkByOrderChanneID", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取值与orderChannelEngName相同的简称个数 
        /// </summary>
        /// <param name="orderChannelEngName"></param>
        /// <returns></returns>
        public static int GetTPOrderChannelDiffPinYin(string orderChannelEngName)
        {
            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.GetTPOrderChannelDiffPinYin(orderChannelEngName);
            }
            catch (Exception ex)
            {
                Logger.Error("GetTPOrderChannelDiffPinYin", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据PKID获取三方渠道实体
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static ThirdPartyOrderChannelModel GetThirdPartyOrderChannelModel(int PKID)
        {
            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.GetThirdPartyOrderChannelModel(PKID);
            }
            catch (Exception ex)
            {
                Logger.Error("GetThirdPartyOrderChannelModel", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 向三方订单渠道表插入第三方订单渠道key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddThirdPartyOrderChannel(ThirdPartyOrderChannelModel model)
        {
            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.AddThirdPartyOrderChannel(model);
            }
            catch (Exception ex)
            {
                Logger.Error("AddThirdPartyOrderChannel", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 向三方订单渠道链接表添加渠道链接
        /// </summary>
        /// <param name="linkmodel"></param>
        /// <returns></returns>
        public static bool AddThirdPartyOrderChannellink(ThirdPartyOrderChannellinkModel linkmodel)
        {

            try
            {
                return DataAccess.DAO.DALThirdPartyOrderChannelLink.AddThirdPartyOrderChannellink(linkmodel);
            }
            catch (Exception ex)
            {
                Logger.Error("AddThirdPartyOrderChannellink", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 添加渠道链接
        /// </summary>
        /// <param name="channelModel"></param>
        /// <param name="linkList"></param>
        /// <returns></returns>
        public bool AddOrderChannellink(ThirdPartyOrderChannelModel channelModel,string orderChannelEng, List<ThirdPartyOrderChannellinkModel> linkList)
        {
            int orderChanneID = 0;//订单渠道key的PKID
            int linkCount = 0;//某订单渠道key下已拥有的渠道链接条数
            int recordCount = 0;
            string channelEng = string.Empty;//订单渠道key简称
            string channel = channelModel.OrderChannel;
            string source = string.Empty;//第三方id
            bool isSuccess = false;
            //判断是否有与orderChannel相等的订单渠道key
            var orderLinkList = GetTPOrderChannellinkList(out recordCount, channel, "");
            if (orderLinkList.Count>=1)
            {
                orderChanneID = orderLinkList[0].OrderChanneID;
                channelEng = orderLinkList[0].OrderChannelEngName.Split('_')[0] + "_" + orderLinkList[0].OrderChannelEngName.Split('_')[1];
                foreach (var item in linkList)
                {
                    linkCount = GetOrderChannelLinkByOrderChanneID(orderChanneID).Count+1;
                    item.OrderChannelEngName= source=channelEng + "_" + linkCount;
                    item.OrderChanneID = orderChanneID;
                    if (item.InitialPagelink.IndexOf('?') >= 0)
                    {
                        item.FinalPagelink = "https://wx.tuhu.cn/link/" + source + ".html?url=" + Uri.EscapeDataString(item.InitialPagelink + "&source=" + source);
                    }
                    else
                    {
                        item.FinalPagelink = "https://wx.tuhu.cn/link/" + source + ".html?url=" + Uri.EscapeDataString(item.InitialPagelink + "?source=" + source);
                    }
                    isSuccess=AddThirdPartyOrderChannellink(item);
                }
            }
            else
            {
                orderChanneID=AddThirdPartyOrderChannel(channelModel);
                //判断是否有与orderChannelEng相同的拼音
                var diffPinYinCount = GetTPOrderChannelDiffPinYin(orderChannelEng);
                if (diffPinYinCount == 0)
                {
                    channelEng = orderChannelEng + "_1";
                }
                else
                {
                    channelEng = orderChannelEng + "_"+ (diffPinYinCount+1);
                }
                foreach (var item in linkList)
                {
                    linkCount = GetOrderChannelLinkByOrderChanneID(orderChanneID).Count + 1;
                    item.OrderChannelEngName = source = channelEng + "_" + linkCount;
                    item.OrderChanneID = orderChanneID;
                    if (item.InitialPagelink.IndexOf('?') >= 0)
                    {
                        item.FinalPagelink = "https://wx.tuhu.cn/link/" + source + ".html?url=" + Uri.EscapeDataString(item.InitialPagelink + "&source=" + source);
                    }
                    else
                    {
                        item.FinalPagelink = "https://wx.tuhu.cn/link/" + source + ".html?url=" + Uri.EscapeDataString(item.InitialPagelink + "?source=" + source);
                    }
                    isSuccess=AddThirdPartyOrderChannellink(item);
                }
            }
            return isSuccess;
        }
        #endregion
    }
}
