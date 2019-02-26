using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
  public class DalUserDailyCheckInConfig
    {
        /// <summary>
        /// 根据任务类型ID获取任务奖励列表
        /// </summary>
        public static List<UserDailyCheckInConfigModel> SearchAllList()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
SELECT [PKID] ,
       [ContinuousDays] ,
       [RewardIntegral] ,
       [CreateBy] ,
       [CreateDateTime] ,
       [LastUpdateBy] ,
       [LastUpdateDateTime] ,
       [IsDeleted]
FROM   [Tuhu_profiles].[dbo].[UserDailyCheckInConfig]
WHERE  IsDeleted = 0
");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<UserDailyCheckInConfigModel>().ToList();
            }
        }

        /// <summary>
        /// 创建签到配置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Create(UserDailyCheckInConfigModel model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
INSERT INTO [Tuhu_profiles].[dbo].[UserDailyCheckInConfig] (   [ContinuousDays] ,
                                                               [RewardIntegral] ,
                                                               [CreateBy] ,
                                                               [CreateDateTime] ,
                                                               [LastUpdateBy] ,
                                                               [LastUpdateDateTime] ,
                                                               [IsDeleted]
                                                           )
VALUES ( @ContinuousDays ,
         @RewardIntegral ,
         @LastUpdateBy ,
         GETDATE(),
         @LastUpdateBy ,
         GETDATE(),
         0
       );
");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.Add(new SqlParameter("@ContinuousDays", model.ContinuousDays));
                cmd.Parameters.Add(new SqlParameter("@RewardIntegral", model.RewardIntegral));
                cmd.Parameters.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        /// 逻辑删除原有7天规则配置
        /// </summary>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public static bool Delete(string lastUpdateBy)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
UPDATE [Tuhu_profiles].[dbo].[UserDailyCheckInConfig] WITH ( ROWLOCK )
SET    IsDeleted = 1 ,
       [LastUpdateBy] = @LastUpdateBy ,
       [LastUpdateDateTime] = GETDATE()
WHERE  IsDeleted = 0;
            ");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.Add(new SqlParameter("@LastUpdateBy", lastUpdateBy));
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
    }
}
