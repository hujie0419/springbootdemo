using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using System.Data;
using Tuhu.Provisioning.Business.Logger;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Business
{
  public  class LuckyWheelManager
    {

        public static bool Save(LuckyWheel model)
        {
            if (string.IsNullOrWhiteSpace(model.ID))
            {
                model.ID = Guid.NewGuid().ToString();
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "LuckyEF", Operation = "新增大翻盘配置" });
            }
            else
            {
                var before = GetEntity(model.ID);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue=JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "LuckyEF", Operation = "修改大翻盘配置" });
            }

            bool result = DALLuckyWheel.Insert(model);
            //if (result)
            //{
            //    DistributedCache.Upsert<LuckyWheel>("LuckWheel/" + model.ID, model, TimeSpan.Zero);
            //}
            return result;
        }


        public static List<LuckyWheel> GetTableList(string selSql)
        {
            return DALLuckyWheel.GetTableList(selSql).ConvertTo<LuckyWheel>().ToList();
        }


        public static LuckyWheel GetEntity(string id)
        {
            return DALLuckyWheel.GetEntity(id);
        }

        public static bool Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return DALLuckyWheel.Delete(id);
            }
            else
                return false;
        }


    }
}
