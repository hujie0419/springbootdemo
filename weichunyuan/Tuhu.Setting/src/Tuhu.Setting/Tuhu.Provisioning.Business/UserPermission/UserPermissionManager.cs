using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.UserPermission
{
    public class UserPermissionManager
    {
        #region 会员特权

        public static IEnumerable<UserPermissionModel> SelectAllUserPermission()
        {
            var dt = DALUserPermission.SelectAllUserPermission();
            if (dt == null || dt.Rows.Count <= 0)
                return new UserPermissionModel[0];
            return dt.Rows.Cast<DataRow>().Select(row => new UserPermissionModel(row));
        }


        public static int GetRowCount()
        {
            return DALUserPermission.GetRowCount();
        }

        public static IEnumerable<UserPermissionModel> SelectUserPermissionByPage(int page, int pageSize)
        {
            var dt = DALUserPermission.SelectUserPermissionByPage(page, pageSize);
            if (dt == null || dt.Rows.Count <= 0)
                return new UserPermissionModel[0];
            return dt.Rows.Cast<DataRow>().Select(row => new UserPermissionModel(row));
        }

        public static int Add(UserPermissionModel model)
        {
            int i = DALUserPermission.AddUserPermission(model);
            return i;
        }


        public static int Update(UserPermissionModel model)
        {
            int i = DALUserPermission.UpdateUserPermission(model);
            return i;
        }

        public static UserPermissionModel GetUserPermission(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            else
                return DALUserPermission.GetUserPermission(id);
        }


        public static int Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return 0;
            return DALUserPermission.Delete(id);
        }
        #endregion


        #region 特价商品

        public static UserPermissionActivityProduct GetActivityProduct(string pkID)
        {
            var dt = DALUserPermission.GetActivityProduct(pkID);
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            return new UserPermissionActivityProduct(dt.Rows[0]);
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddByActivityProduct(UserPermissionActivityProduct model )
        {
            if (model.PKID == 0)
            {
                return DALUserPermission.AddByActivityProduct(model);
            }
            else
            {
                return DALUserPermission.UpdateByActivityProduct(model);
            }
           
        }

        public static bool DeleteByActivityProduct(string activityID, string pkid)
        {
            return DALUserPermission.DeleteByActivityProduct(activityID, pkid);
        }



        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static IEnumerable<UserPermissionActivityProduct> GetActivityProductList(string activityID)
        {
            Guid guid ;
            if (Guid.TryParse(activityID, out guid))
            {
                DataTable dt = DALUserPermission.GetActivityProductList(activityID);
                return dt.Rows.Cast<DataRow>().Select(row => new UserPermissionActivityProduct(row));
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region 会员运费
        public static bool SaveTransMoney(List<tbl_UserTransportation> list)
        {
            try
            {

                foreach (var item in list)
                {
                    if (DALUserPermission.Exist(item.Rank.ToString()))
                    {
                        DALUserPermission.UpdateTrans(item);
                    }
                    else
                    {
                        DALUserPermission.AddUseTrans(item);
                    }
                }
                return true;
            }
            catch (Exception em)
            {
                return false;
            }
        }

        public static List<tbl_UserTransportation> GetUseTransMoney()
        {
            List<tbl_UserTransportation> list = new List<tbl_UserTransportation>();
            DataTable dt = DALUserPermission.GetUseTransMoney();
            if (dt == null || dt.Rows.Count <= 0)
            {

               
                tbl_UserTransportation model1 = new tbl_UserTransportation();
                model1.Rank = 1;
                tbl_UserTransportation model2 = new tbl_UserTransportation();
                model2.Rank = 2;
                tbl_UserTransportation model3 = new tbl_UserTransportation();
                model3.Rank = 3;
                tbl_UserTransportation model4 = new tbl_UserTransportation();
                model4.Rank = 4;
                list.Add(model1);
                list.Add(model2);
                list.Add(model3);
                list.Add(model4);
              
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tbl_UserTransportation item = new tbl_UserTransportation(dr);
                    list.Add(item);
                }
            }
            return list;
        }


        #endregion


        #region 升级任务
        public static IEnumerable<tbl_UserTask> GetTaskList(string appType)
        {
            DataTable dt = DALUserPermission.GetTaskList(appType);
            if (dt == null || dt.Rows.Count <= 0)
                return new List<tbl_UserTask>();
            else
            {
                return dt.Rows.Cast<DataRow>().Select(row => new tbl_UserTask(row));
            }
        }


        public static bool EditTask(tbl_UserTask model)
        {
            if (model.ID <= 0)
            {
                return DALUserPermission.AddTask(model);
            }
            else
            {
                return DALUserPermission.UpdateTask(model);
            }
        }


        public static tbl_UserTask GetTask(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new tbl_UserTask();
            else
            {
                DataTable dt = DALUserPermission.GetTask(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return new tbl_UserTask(dt.Rows[0]);
                }
                else
                {
                    return new tbl_UserTask();
                }
            }
        }


        public static bool DeleteTask(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            else
                return DALUserPermission.DeleteTask(id);
        }

        #endregion


        #region 会员优惠券
        public static IEnumerable<tbl_UserPromotionCode> GetPromotionList(string userRank)
        {
            DataTable dt = DALUserPermission.GetPromotionList(userRank);
            if (dt == null || dt.Rows.Count <= 0)
                return new List<tbl_UserPromotionCode>();
            else
            {
                return dt.Rows.Cast<DataRow>().Select(row => new tbl_UserPromotionCode(row));
            }
        }


        public static bool EditPromotion(tbl_UserPromotionCode model)
        {
            if (model.ID <= 0)
            {
                return DALUserPermission.AddPromotion(model);
            }
            else
            {
                return DALUserPermission.UpdatePromotion(model);
            }
        }


        public static tbl_UserPromotionCode GetPromotion(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new tbl_UserPromotionCode();
            else
            {
                DataTable dt = DALUserPermission.GetPromotion(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return new tbl_UserPromotionCode(dt.Rows[0]);
                }
                else
                {
                    return new tbl_UserPromotionCode();
                }
            }
        }


        public static bool DeletePromotion(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            else
                return DALUserPermission.DeletePromotion(id);
        }

        #endregion


    }
}
