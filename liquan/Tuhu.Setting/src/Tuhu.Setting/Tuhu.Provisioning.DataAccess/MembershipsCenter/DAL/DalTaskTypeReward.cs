using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 会员任务类型奖励
    /// </summary>
    public class DalTaskTypeReward
    {
        /// <summary>
        /// 获取所有任务类型奖励，任务类型不会超过三个，故可以全表数据获取
        /// </summary>
        public static List<TaskTypeRewardModel> SearchAllList()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
SELECT   r.[PKID] ,
         [TaskTypeID] ,
         [DescriptionTitle] ,
         [RewardValue] ,
         [RemainingDay] ,
         [Status] ,
         r.[CreateBy] ,
         r.[CreateDateTime] ,
         r.[LastUpdateBy] ,
         r.[LastUpdateDateTime] ,
         t.TaskTypeCode ,
         t.TaskTypeName
FROM     [Configuration].[dbo].[TaskTypeReward] AS r WITH ( NOLOCK )
         INNER JOIN [Configuration].[dbo].[TaskType] AS t WITH ( NOLOCK ) ON r.TaskTypeID = t.PKID
WHERE    r.IsDeleted = 0
         AND t.IsDeleted = 0
ORDER BY PKID DESC;
");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<TaskTypeRewardModel>().ToList();
            }
        }

        /// <summary>
        /// 根据任务类型ID获取任务奖励列表
        /// </summary>
        public static List<TaskTypeRewardModel> SearchTypeRewardByTypeID(int taskTypeId)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
 SELECT [PKID]
      ,[TaskTypeID]
      ,[DescriptionTitle]
      ,[RewardValue]
      ,[RemainingDay]
      ,[Status]
      ,[CreateBy]
      ,[CreateDateTime]
      ,[LastUpdateBy]
      ,[LastUpdateDateTime]
      ,[IsDeleted]
  FROM [Configuration].[dbo].[TaskTypeReward]  WITH(NOLOCK) WHERE TaskTypeID=@TaskTypeID AND  IsDeleted=0
");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.Add(new SqlParameter("@TaskTypeID", taskTypeId));
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<TaskTypeRewardModel>().ToList();
            }
        }

        /// <summary>
        /// 根据任务奖励获取任务数据明细
        /// </summary>
        public static List<TaskTypeRewardModel> GetModelById(int Id)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
 SELECT [PKID]
      ,[TaskTypeID]
      ,[DescriptionTitle]
      ,[RewardValue]
      ,[RemainingDay]
      ,[Status]
      ,[CreateBy]
      ,[CreateDateTime]
      ,[LastUpdateBy]
      ,[LastUpdateDateTime]
      ,[IsDeleted]
  FROM [Configuration].[dbo].[TaskTypeReward]  WITH(NOLOCK) WHERE PKID=@PKID
");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.Add(new SqlParameter("@PKID", Id));
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<TaskTypeRewardModel>().ToList();
            }
        }

        /// <summary>
        /// 添加任务奖励类型
        /// </summary>
        public static bool Insert(TaskTypeRewardModel model)
        {
            var strSql = new StringBuilder();
            #region sql脚本
            strSql.Append(@"
 INSERT INTO [Configuration].[dbo].[TaskTypeReward]
([TaskTypeID]
,[DescriptionTitle]
,[RewardValue]
,[RemainingDay]
,[Status]
,[CreateBy]
,[LastUpdateBy]
)
VALUES
(@TaskTypeID
,@DescriptionTitle
,@RewardValue
,@RemainingDay
,@Status
,@CreateBy
,@LastUpdateBy
)");
            #endregion
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.Add(new SqlParameter("@TaskTypeID", model.TaskTypeID));
                cmd.Parameters.Add(new SqlParameter("@DescriptionTitle", model.DescriptionTitle));
                cmd.Parameters.Add(new SqlParameter("@RewardValue", model.RewardValue));
                cmd.Parameters.Add(new SqlParameter("@RemainingDay", model.RemainingDay));
                cmd.Parameters.Add(new SqlParameter("@Status", model.Status));
                cmd.Parameters.Add(new SqlParameter("@CreateBy", model.CreateBy));
                cmd.Parameters.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        /// 根据任务奖励获取任务数据明细
        /// </summary>
        public static bool Update(TaskTypeRewardModel model)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"
UPDATE [Configuration].[dbo].[TaskTypeReward]
SET    [TaskTypeID] = @TaskTypeID ,
       [DescriptionTitle] = @DescriptionTitle ,
       [RewardValue] = @RewardValue ,
       [RemainingDay] = @RemainingDay ,
       [Status] = @Status ,
       [LastUpdateBy] = @LastUpdateBy ,
       [LastUpdateDateTime] = GETDATE()
   WHERE PKID=@PKID
");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.Add(new SqlParameter("@TaskTypeID", model.TaskTypeID));
                cmd.Parameters.Add(new SqlParameter("@DescriptionTitle", model.DescriptionTitle));
                cmd.Parameters.Add(new SqlParameter("@RewardValue", model.RewardValue));
                cmd.Parameters.Add(new SqlParameter("@RemainingDay", model.RemainingDay));
                cmd.Parameters.Add(new SqlParameter("@Status", model.Status));
                cmd.Parameters.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        /// 根据ID 进行删除操作
        /// </summary>
        public static bool Delete(int Id)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"UPDATE  [Configuration].[dbo].[TaskTypeReward]  WITH(ROWLOCK) SET IsDeleted=1 WHERE PKID=@PKID");
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.Add(new SqlParameter("@PKID", Id));
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

       
    }
}
