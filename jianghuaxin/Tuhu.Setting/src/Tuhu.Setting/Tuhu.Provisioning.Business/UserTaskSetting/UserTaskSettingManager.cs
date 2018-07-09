using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.UserTaskSetting
{
    public class UserTaskSettingManager
    {
        public static List<UserTaskSettingModel> GetUserTaskSettingList(int pageIndex, int pageSize)
        {
            return DalUserTaskSetting.GetUserTaskSettingList(pageIndex, pageSize);

        }
        public static UserTaskSettingModel GetUserTaskSetting(int pkid)
        {
            return DalUserTaskSetting.GetUserTaskSetting(pkid);
        }
        public static List<TaskCompleteCondition> GetTaskCompleteCondition()
        {
            return DalUserTaskSetting.GetTaskCompleteCondition();
        }
        public static Tuple<bool, string> SaveTaskSetting(UserTaskSettingModel model)
        {
            if (model.PKID > 0)
            {
                var temp = GetUserTaskSetting(model.PKID);
                if (temp != null)
                {
                    if (temp.StartTime < DateTime.Now && model.StartTime.ToString()!=temp.StartTime.ToString())
                    {
                        return Tuple.Create(false, "已经开始的任务开始时间不能修改！");
                    }

                    return Tuple.Create(DalUserTaskSetting.UpdateTaskSetting(model), "");
                }
                else
                {
                    return Tuple.Create(false, "不存在该任务!");
                }

            }
            else
            {
                return Tuple.Create(DalUserTaskSetting.CreateTaskSetting(model), "");
            }

        }
    }
}
