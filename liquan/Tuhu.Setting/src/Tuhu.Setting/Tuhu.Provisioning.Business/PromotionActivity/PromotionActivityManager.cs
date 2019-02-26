using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Component.Common;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Service.ConfigLog;

namespace Tuhu.Provisioning.Business
{
   public class PromotionActivityManager
    {
        public static DALGetPromotionActivityConfig dal = null;

        public PromotionActivityManager()
        {
            dal = new DALGetPromotionActivityConfig();
        }

        public IEnumerable<SE_GetPromotionActivityConfig> GetList()
        {
            var dt = dal.GetList();
            if (dt == null || dt.Rows.Count == null)
                return null;
            var list = dt.ConvertTo<SE_GetPromotionActivityConfig>();
            foreach (var item in list)
            {
                item.GetCouponNumbers = GetCouponHad(item.ID.Value);
                var items = GetEntity(item.ID.Value).CouponItems;
                if (items != null &&items.Count() > 0)
                {
                    item.GetCouponTotal = items.FirstOrDefault().Quantity;
                }
               
            }
            return list;
        }

        public int GetCouponHad(Guid ID)
        {
            return dal.GetCouponHad(ID);
        }

        public SE_GetPromotionActivityConfig GetEntity(Guid ID)
        {
            SE_GetPromotionActivityConfig model = dal.GetEntity(ID).ConvertTo<SE_GetPromotionActivityConfig>().FirstOrDefault();
            if (model != null)
            {
                model.CouponItems = dal.GetCouponInfo(ID).ConvertTo<SE_GetPromotionActivityCouponInfoConfig>();
            }
            return model;
        }

        public bool Save(SE_GetPromotionActivityConfig model)
        {
            bool result = false;
            string operate = "";
            if (model.ID == null)
            {
                result = dal.Add(model);
                operate = "新增";
            }
            else
            {
                result = dal.Update(model);
                operate = "修改";
            }
            if (result)
            {
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.ID?.ToString().ToLower(),
                        ObjectType = "PromotionActivity",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Operate = operate,
                        Author = ThreadIdentity.Operator.Name
                    }));
                }
            }

            return result;

        }

        public bool Delete(Guid ID)
        {
            using (var client = new ConfigLogClient())
            {
                client.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                {
                    ObjectId = ID.ToString().ToLower(),
                    ObjectType = "PromotionActivity",
                    BeforeValue = "",
                    AfterValue = "",
                    Operate = "删除",
                    Author = ThreadIdentity.Operator.Name
                }));
            }
            return dal.Delete(ID);
        }

        public SE_GetPromotionActivityCouponInfoConfig GetCouponValidate(Guid ID)
        {
            return dal.GetCouponInfoValidate(ID).ConvertTo<SE_GetPromotionActivityCouponInfoConfig>().FirstOrDefault();
        }

        public int GetPromotionActivityCountByID(Guid ID)
        {
            return dal.GetPromotionActivityCountByID(ID);
        }


        ~PromotionActivityManager()
        {
          
        }

    }
}
