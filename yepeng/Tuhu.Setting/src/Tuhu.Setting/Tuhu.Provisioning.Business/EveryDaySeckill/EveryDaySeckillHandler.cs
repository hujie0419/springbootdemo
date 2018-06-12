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
using System.Security.Principal;

namespace Tuhu.Provisioning.Business
{
    public class EveryDaySeckillHandler
    {
        public bool Submit(SE_EveryDaySeckill model, IPrincipal user)
        {
            DalSE_EveryDaySeckill dal = new DalSE_EveryDaySeckill();


            if (model.ActivityGuid == null)
            {
                model.ActivityGuid = Guid.NewGuid();
                model.EveryDaySeckillInfo.FK_EveryDaySeckill = model.ActivityGuid;
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;
                model.EveryDaySeckillInfo.UpdateDate = DateTime.Now;
                model.EveryDaySeckillInfo.CreateDate = DateTime.Now;

                if (dal.Exits(model.StartDate, model.EndDate))
                {
                    return false;
                }

                LoggerManager.InsertOplog(new ConfigHistory()
                {
                    BeforeValue = "",
                    ChangeDatetime = DateTime.Now,
                    AfterValue = JsonConvert.SerializeObject(model),
                    Author = user.Identity.Name,
                    ObjectType = "EDSKill",
                    Operation = "新增天天秒杀" + model.ActivityName
                });




                return dal.Add(model);
            }
            else
            {
                model.UpdateDate = DateTime.Now;
                model.EveryDaySeckillInfo.CreateDate = DateTime.Now;



                LoggerManager.InsertOplog(new ConfigHistory()
                {
                    ChangeDatetime = DateTime.Now,
                    BeforeValue = JsonConvert.SerializeObject(GetEntity(model.ActivityGuid.Value)),
                    AfterValue = JsonConvert.SerializeObject(model),
                    // ObjectID= model.ID.Value.ToString(),
                    Author = user.Identity.Name,
                    ObjectType = "EDSKill",
                    Operation = "修改天天秒杀" + model.ActivityName
                });

                return dal.Update(model);
            }
        }


        public SE_EveryDaySeckill GetEntity(Guid? activityGuid)
        {
            if (activityGuid.HasValue)
            {
                DalSE_EveryDaySeckill dal = new DalSE_EveryDaySeckill();
                DataTable dt = dal.GetEntity(activityGuid.Value);
                if (dt == null || dt.Rows.Count <= 0)
                    return null;
                SE_EveryDaySeckill model = dt.ConvertTo<SE_EveryDaySeckill>().FirstOrDefault();
                DataTable infoDataTable = dal.GetEntityDataTable(activityGuid.Value);
                if (infoDataTable == null || infoDataTable.Rows.Count <= 0)
                    return null;

                model.EveryDaySeckillInfo = infoDataTable.ConvertTo<SE_EveryDaySeckillInfo>().FirstOrDefault();
                return model;

            }
            return null;
        }

        public IEnumerable<SE_EveryDaySeckill> GetList()
        {
            DalSE_EveryDaySeckill dal = new DalSE_EveryDaySeckill();
            DataTable dt = dal.GetList();
            if (dt == null || dt.Rows.Count <= 0)
                return null;

            return dt.ConvertTo<SE_EveryDaySeckill>();

        }

        public bool Delete(Guid activityGuid)
        {

            DalSE_EveryDaySeckill dal = new DalSE_EveryDaySeckill();
            return dal.Delete(activityGuid);
        }

        public IEnumerable<SE_EveryDaySeckill> GetListByWhere(string activityGuid, DateTime? startDate, DateTime? endDate)
        {
            StringBuilder whereString = new StringBuilder();
            List<System.Data.SqlClient.SqlParameter> collection = new List<System.Data.SqlClient.SqlParameter>();
            if (!string.IsNullOrWhiteSpace(activityGuid))
            {
                whereString.Append(" and ActivityGuid=@ActivityGuid ");
                collection.Add(new System.Data.SqlClient.SqlParameter("@ActivityGuid", activityGuid));
            }

            if (startDate != null && endDate != null)
            {
                whereString.Append(" and StartDate>=@StartDate and  EndDate <=@EndDate ");
                collection.Add(new System.Data.SqlClient.SqlParameter("@StartDate", startDate));
                collection.Add(new System.Data.SqlClient.SqlParameter("@endDate", endDate));
            }
            DalSE_EveryDaySeckill dal = new DalSE_EveryDaySeckill();
            DataTable dt = dal.GetListByWhere(whereString.ToString(), collection.ToArray());

            if (dt == null)
                return null;
            List<SE_EveryDaySeckill> list = new List<SE_EveryDaySeckill>();
            foreach (DataRow dr in dt.Rows)
            {
                Guid activity = Guid.Parse(dr["ActivityGuid"].ToString());
                list.Add(GetEntity(activity));
            }
            return list;
        }


    }
}
