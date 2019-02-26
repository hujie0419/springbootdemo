using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.OprLog;
using Tuhu.Service.OprLog.Models;

namespace Tuhu.Provisioning.Business.WebActivity
{
    public class WebActivityManager
    {
        /// <summary>
        /// 获取每次活动的各种数据
        /// </summary>
        /// <param name="ActiveID"></param>
        /// <returns></returns>
        public static WebActive WebActivityDetail(string ActiveID)
        {
            var dt = DALWebActivityDetail.FetchWebActivityDetail(ActiveID);
            if (dt == null || dt.Rows.Count <= 0)
                return new WebActive();
            var Floors = new List<CommodityFloor>();
            IEnumerable<OtherPart> otherpart = DALWebActivityDetail.FetchOtherPartForFloor(ActiveID).Rows.Cast<DataRow>().Select(row => new OtherPart(row));
            foreach (DataRow dr in dt.Rows)
            {
                CommodityFloor cf = new CommodityFloor();
                cf.FloorID = Convert.ToInt32(dr["FloorID"]);
                cf.FloorLink = dr["FloorLink"].ToString();
                cf.FloorPicture = dr["FloorPicture"].ToString();
                cf.Products = DALWebActivityDetail.FetchProductsForFloor(ActiveID, cf.FloorID).Rows.Cast<DataRow>().Select(row => new Products(row));
                Floors.Add(cf);
            }
            return dt.Rows.Cast<DataRow>().Select(row => new WebActive()
            {
                CommodifyFloor = Floors.Distinct(),
                PKID = Convert.ToInt32(row["PKID"]),
                ActiveDescription = row["ActiveDescription"].ToString(),
                ActiveID = row["ActiveID"].ToString(),
                ActiveLink = row["ActiveLink"].ToString(),
                ActiveName = row["ActiveName"].ToString(),
                backgroundColor = row["backgroundColor"].ToString(),
                Banner = row["Banner"].ToString(),
                CornerMark = row["CornerMark"].ToString(),
                CreateDateTime = Convert.ToDateTime(row["CreateDateTime"]),
                EndDateTime = Convert.ToDateTime(row["EndDateTime"]),
                StartDateTime = Convert.ToDateTime(row["StartDateTime"]),
                OtherPart = otherpart
            }).First();
        }
        /// <summary>
        /// 得到最新的活动期数
        /// </summary>
        /// <returns></returns>
        public static string GetLastActiveID()
        {
            return DALWebActivityDetail.GetLastActiveID();
        }

        /// <summary>
        /// 得到所有活动的基本信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<WebActive> SelectAllWebActivity()
        {
            return DALWebActivityDetail.SelectAllWebActivity().Rows.Cast<DataRow>().Select(row => new WebActive(row));
        }
        /// <summary>
        /// 活动配置
        /// </summary>
        /// <param name="webact"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static int WebActiveConfig(WebActive webact, string action)
        {
            return DALWebActivityDetail.InsertWebActivityDetail(webact, action);
        }
        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="ActiveID"></param>
        /// <returns></returns>
        public static int DeleteWebActivity(string ActiveID, int Type)
        {
            return DALWebActivityDetail.DeleteWebActivity(ActiveID, Type);
        }
        /// <summary>
        /// 活动表的操作历史记录的查看 
        /// </summary>
        /// <param name="activeID"></param>
        /// <returns></returns>
        public static IEnumerable<OprLogModel> HistoryActions(int activeID, string type)
        {
            //return DALWebActivityDetail.HistoryActions(activeID, type).Rows.Cast<DataRow>().Select(row => new ConfigHistory(row));
            using (var client = new OprLogClient())
            {
                var result = client.SelectOrderOprLog(activeID, type);
                if (result.Success)
                {
                    return result.Result;
                }
                else
                {
                    return new List<OprLogModel>();
                }
            }
        }
        /// <summary>
        /// 活动表操作历史进行记录
        /// </summary>
        /// <param name="activeID"></param>
        /// <param name="Operation"></param>
        /// <param name="Author"></param>
        /// <param name="HostName"></param>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        public static int InsertHistoryActions(int activeID, string ObjectType, string AfterValue, string Operation, string Author, string HostName, string IPAddress)
        {
            OprLogManager log = new OprLogManager();

            OprLogModel model = new OprLogModel();
            model.ObjectId = activeID;
            model.ObjectType = ObjectType;
            model.AfterValue = AfterValue;
            model.Operation = Operation;
            //model.Author = Author;
            model.Author = ThreadIdentity.Operator.Name;
            model.HostName = HostName;
            model.IpAddress = IPAddress;

            int flag = 0;
            using (var client = new OprLogClient())
            {
                var result = client.AddOprLog(model);
                if (result.Result > 0)
                    flag = 1;
            }
            return flag;
        }
    }
}
