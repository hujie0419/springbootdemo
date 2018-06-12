using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Shipping;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.WeChat;

namespace Tuhu.Provisioning.Business.WechatHome
{
    public class WechatHomeManager
    {
        private DALWechatHome dal = null;

        public WechatHomeManager()
        {
            dal = new DALWechatHome();
        }

        public bool AddWechatHomeList(WechatHomeList model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return dal.AddWechatHomeList(connection, model);
            }
        }

        public bool AddHomePageConfig(HomePageConfiguation model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return dal.AddHomePageConfig(connection, model);
            }
        }

        public int AddWechatHomeListToInt(WechatHomeList model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return dal.AddWechatHomeListToInt(connection, model);
            }
        }

        public bool UpdateWechatHomeList(WechatHomeList model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdateWechatHomeList(conn, model);
            }
        }

        public bool UpdateHomePageConfig(HomePageConfiguation model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdateHomePageConfig(conn, model);
            }
        }

        public bool DeleteWechatHomeList(int id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteWechatHomeList(conn, id);
            }
        }

        public bool DeleteHomePageConfig(int id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteHomePageConfig(conn, id);
            }
        }

        public IEnumerable<WechatHomeList> GetWechatHomeList(int homepageconfigId)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetWechatHomeList(conn, homepageconfigId);
            }
        }

        public IEnumerable<HomePageModuleType> GeHomePageModuleTypeList()
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetHomePageModuleTypeList(conn);
            }
        }

        public IEnumerable<HomePageConfiguation> GetHomePageConfigList()
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetHomePageConfigList(conn);
            }
        }

        public WechatHomeList GetWechatHomeEntity(int id)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetWechatHomeEntity(conn, id);
            }
        }

        public HomePageConfiguation GetHomePageConfigEntity(int id)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetHomePageConfigEntity(conn, id);
            }
        }

        public bool AddContent(WechatHomeContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddContent(conn, model);
            }
        }

        public bool AddAreaContent(WechatHomeAreaContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddAreaContent(conn, model);
            }
        }

        public bool AddProductContent(WechatHomeProductContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddProductContent(conn, model);
            }
        }

        public bool UpdateContent(WechatHomeContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdateContent(conn, model);
            }
        }

        public bool UpdateProductContent(WechatHomeProductContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdateProductContent(conn, model);
            }
        }

        public bool UpdateAreaContent(WechatHomeAreaContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.UpdateAreaContent(conn, model);
            }
        }

        public bool DeleteContent(int id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteContent(conn, id);
            }
        }

        public bool DeleteAreaContent(int id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteAreaContent(conn, id);
            }
        }

        public bool DeleteProductContent(int id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteProductContent(conn, id);
            }
        }

        public IEnumerable<WechatHomeContent> GetContentList(int fkid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetContentList(conn, fkid);
            }
        }

        public IEnumerable<WechatHomeProductContent> GetProductContentList(int fkid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetProductContentList(conn, fkid);
            }
        }

        public IEnumerable<WechatHomeAreaContent> GetAreaContentList(int fkid)
        {
            IEnumerable<WechatHomeAreaContent> wechatHomeAreaContents = new List<WechatHomeAreaContent>();
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                wechatHomeAreaContents = dal.GetAreaContentList(conn, fkid);
            }
            if (wechatHomeAreaContents != null && wechatHomeAreaContents.Any())
            {
                foreach (var areacontent in wechatHomeAreaContents)
                {
                    var cityids = areacontent.CityIDs.Replace("||", "|").Trim(new char[] { '|' }).Split(new char[] { '|' }).Select(p => Convert.ToInt32(p)).ToList();
                    areacontent.CityList = ShippingManager.SelectCityNameByCityIDs(cityids);
                }
            }
            return wechatHomeAreaContents;
        }

        public WechatHomeContent GetContentEntity(int id)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetContentEntity(conn, id);
            }
        }

        public WechatHomeProductContent GetProductContentEntity(int id)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetProductContentEntity(conn, id);
            }
        }

        public WechatHomeAreaContent GetAreaContentEntity(int id)
        {
            WechatHomeAreaContent wechatHomeAreaContent = new WechatHomeAreaContent();
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                wechatHomeAreaContent = dal.GetAreaContentEntity(conn, id);
            }
            if (wechatHomeAreaContent != null)
            {
                var cityids = wechatHomeAreaContent.CityIDs.Replace("||", "|").Trim(new char[] { '|' }).Split(new char[] { '|' }).Select(p => Convert.ToInt32(p)).ToList();
                wechatHomeAreaContent.CityList = ShippingManager.SelectCityNameByCityIDs(cityids);
            }
            return wechatHomeAreaContent;
        }

        public string GetRandomString()
        {
            var b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            var r = new Random(BitConverter.ToInt32(b, 0));
            string s = "";
            var str = "0123456789";//"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (var i = 0; i < 8; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        #region WxAppConfig

        public List<WxAppUserEventConfigModel> SelectAppUserEventConfig(int pageIndex = 1, int pageSize = 20)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.SelectWxAppUserEventConfig(conn, pageIndex, pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取微信小程序配置
        /// </summary>
        /// <param name="originId">originId</param>
        /// <param name="eventType">EventType</param>
        /// <param name="userData">userData</param>
        /// <returns></returns>
        public WxAppUserEventConfigModel FetchWxAppUserEventConfig(string originId, string eventType, string userData = null)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.FetchWxAppUserEventConfig(conn, originId, eventType, userData);
            }
        }

        /// <summary>
        /// 获取微信小程序配置
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public WxAppUserEventConfigModel FetchWxAppUserEventConfig(int id)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.FetchWxAppUserEventConfig(conn, id);
            }
        }

        /// <summary>
        /// 保存微信用户事件配置
        /// </summary>
        /// <param name="config">config</param>
        /// <returns></returns>
        public int SaveWxAppUserEventConfig(WxAppUserEventConfigModel config)
        {
            config.CreateBy = ThreadIdentity.Operator.Name;
            config.UpdateBy = ThreadIdentity.Operator.Name;
            if (config.UserData != null)
            {
                config.UserData = config.UserData.Trim().Replace(" ", "");
            }
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.SaveWxAppUserEventConfig(conn, config);
            }
        }

        /// <summary>
        /// 删除配置（逻辑删除
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        public int DeleteWxAppConfig(WxAppUserEventConfigModel model)
        {
            model.UpdateBy = ThreadIdentity.Operator.Name;
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.DeleteWxAppConfig(conn, model);
            }
        }

        #endregion WxAppConfig

        #region 微信社交立减金配置

        /// <summary>
        /// 添加微信社交立减金代金券配置
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        public int AddWechatSocialCardConfig(WechatSocialCardConfigModel model)
        {
            model.CreatedBy = ThreadIdentity.Operator.Name;
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                if (null == model.OtherMerchantNo)
                {
                    model.OtherMerchantNo = "";
                }
                return dal.AddWechatSocialCardConfig(conn, model);
            }
        }

        /// <summary>
        /// 查询微信社交立减金代金券配置
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public IEnumerable<WechatSocialCardConfigModel> SelectWechatSocialCardConfig(int pageIndex, int pageSize)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.SelectWechatSocialCardConfig(conn, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 根据卡券id查询对应配置
        /// </summary>
        /// <param name="cardId">卡券id</param>
        /// <returns>小程序原始id</returns>
        public WechatSocialCardConfigModel FetchWxAppByCardId(string cardId)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.FetchWxAppByCardId(conn, cardId);
            }
        }

        /// <summary>
        /// 添加微信社交立减金活动配置
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        public int AddWechatSocialActivityConfig(WechatSocialActivityConfigModel model)
        {
            model.CreatedBy = ThreadIdentity.Operator.Name;
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return dal.AddWechatSocialActivityConfig(conn, model);
            }
        }

        /// <summary>
        /// 查询微信社交立减金活动配置
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public IEnumerable<WechatSocialActivityConfigModel> SelectWechatSocialActivityConfig(
            int pageIndex, int pageSize)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.SelectWechatSocialActivityConfig(conn, pageIndex, pageSize);
            }
        }

        #endregion 微信社交立减金配置

        /// <summary>
        /// 删除微信自定义菜单
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool DeleteWxMenu(string accessToken, string userName)
        {
            bool flag = false;
            string requestUrl = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", accessToken);
            var request = WebRequest.Create(requestUrl);
            request.Method = "POST";
            var response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();
            response.Close();
            response.Dispose();

            JObject json = JObject.Parse(result);
            if (json["errcode"].ToString() == "0")
            {
                LoggerManager.InsertOplog(new ConfigHistory()
                {
                    AfterValue = "",
                    Author = userName,
                    ObjectType = "WxMenu",
                    Operation = "更新公众号菜单"
                });
                flag = true;
            }
            return flag;
        }
    }
}