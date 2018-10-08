using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Activity.Models;

namespace Tuhu.C.Job.Activity.Dal
{
    public class DalActivity
    {
        public static IEnumerable<ActivityMessageRemindModel> SelectActivityMessageRemindModel(List<string> pids)
        {
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand($"SELECT  * FROM   Tuhu_Log..ActivityProductUserRemindLog WITH ( NOLOCK ) where Status=1 And  pid in ({string.Join(",", pids.Select(r => "'" + r + "'"))})"))
                {
                    cmd.CommandType = CommandType.Text;
                    //cmd.Parameters.AddWithValue("@Pids", string.Join(",", pids.Select(r => "'" + r + "'")));
                    return db.ExecuteSelect<ActivityMessageRemindModel>(cmd);
                }
            }
        }

        public static IEnumerable<FlashSaleProductModel> SelectFlashSaleProductModels()
        {
            using (var cmd = new SqlCommand(@"	
SELECT	FSP.PID,
FS.StartDateTime,
DATEDIFF(MINUTE,GETDATE(),FS.StartDateTime)
FROM	Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
JOIN	Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK )
		ON FSP.ActivityID = FS.ActivityID
WHERE	DATEDIFF(MINUTE,GETDATE(),FS.StartDateTime)<15
AND FS.StartDateTime>GETDATE()
AND FS.ActiveType in (3,4)"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteSelect<FlashSaleProductModel>(cmd);
            }
        }


        public static int UpdateActivityMessageRemindModel(List<int> pkids)
        {
            using (var db = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"
                        Update Tuhu_Log..ActivityProductUserRemindLog WITH ( ROWLOCK ) set Status=0 where pkid in ( SELECT	Item COLLATE Chinese_PRC_CI_AS AS PKID
		     FROM	Tuhu_log..SplitString(@PKids, ',', 1))"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PKids", string.Join(",", pkids));
                    return db.ExecuteNonQuery(cmd);
                }
            }
        }
        public static string SelectNextScheduleActivity(string schedule)
        {
            var datetime = DateTime.Now;
            var strDate = "";
            var endDate = "";
            switch (schedule)
            {
                case "10点场":
                    strDate = datetime.ToString("yyyy-MM-dd 10:00:0");
                    endDate = datetime.ToString("yyyy-MM-dd 13:00:0");
                    break;
                case "13点场":
                    strDate = datetime.ToString("yyyy-MM-dd 13:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 16:00:0");
                    break;
                case "16点场":
                    strDate = datetime.ToString("yyyy-MM-dd 16:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 20:00:0");
                    break;
                case "20点场":
                    strDate = datetime.ToString("yyyy-MM-dd 20:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case "0点场":
                    strDate = datetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                    endDate = datetime.AddDays(1).ToString("yyyy-MM-dd 10:00:0");
                    break;
            }
            var whereCondition = $"where EndDateTime<='{endDate}' AND StartDatetime>='{strDate}'";
            var sql = $@"SELECT A.ActivityId FROM Activity..tbl_FlashSale AS A WITH ( NOLOCK) 
		JOIN Activity..tbl_FlashSaleProducts AS B WITH(NOLOCK) ON B.ActivityID = A.ActivityID {whereCondition}AND IsDefault=0";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteScalar(true, cmd).ToString();
            }
        }

        public static IEnumerable<string> SelectExpiredScheduleActivity(string schedule)
        {
            var datetime = DateTime.Now;
            var strDate = "";
            var endDate = "";
            switch (schedule)
            {
                case "10点场":
                    strDate = datetime.ToString("yyyy-MM-dd 10:00:0");
                    endDate = datetime.ToString("yyyy-MM-dd 13:00:0");
                    break;
                case "13点场":
                    strDate = datetime.ToString("yyyy-MM-dd 13:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 16:00:0");
                    break;
                case "16点场":
                    strDate = datetime.ToString("yyyy-MM-dd 16:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 20:00:0");
                    break;
                case "20点场":
                    strDate = datetime.ToString("yyyy-MM-dd 20:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case "0点场":
                    strDate = datetime.AddDays(-1).ToString("yyyy-MM-dd 20:00:00");
                    endDate = datetime.AddDays(-1).ToString("yyyy-MM-dd 23:59:59");
                    break;
            }
            var whereCondition = $"where  StartDatetime<'{strDate}'";
            var sql = $@"SELECT A.ActivityId FROM Activity..tbl_FlashSale_temp AS A WITH ( NOLOCK) 
		JOIN Activity..tbl_FlashSaleProducts_temp AS B WITH(NOLOCK) ON B.ActivityID = A.ActivityID {whereCondition}AND IsDefault=0";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteQuery(cmd, dt => dt.ToList<string>());
            }
        }
        public static int DeleteFlashSaleTempByAcid(string acid)
        {
            using (var cmd = new SqlCommand(@"	            
                   DELETE  Activity..tbl_FlashSale_Temp WITH(ROWLOCK)
                    WHERE   ActivityID = @ActivityID;"))
            {
                cmd.Parameters.AddWithValue("@ActivityId", acid);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        public static int DeleteFlashSaleProductsTempByAcid(string acid)
        {
            using (var cmd = new SqlCommand(@"	            
                    DELETE  Activity..tbl_FlashSaleProducts_Temp WITH(ROWLOCK)
                    WHERE   ActivityID = @ActivityID;"))
            {
                cmd.Parameters.AddWithValue("@ActivityId", acid);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        ///     获取今天发布答案的题目
        /// </summary>
        /// <param name="questionnaireID"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static IEnumerable<QuestionModel> SearchTodayReleaseQuestionAnswerList(long questionnaireID)
        {
            var sql = @" select
                            [PKID]
                              ,[QuestionnaireID]
                              ,[QuestionTitle]
                              ,[QuestionType]
                              ,[IsFork]
                              ,[DefaultValue]
                              ,[ScoreStyle]
                              ,[MinScore]
                              ,[MaxScore]
                              ,[IsRequired]
                              ,[IsValidateMinChar]
                              ,[MinChar]
                              ,[IsValidateMaxChar]
                              ,[MaxChar]
                              ,[IsValidateStartDate]
                              ,[StartTime]
                              ,[IsValidateEndDate]
                              ,[EndTime]
                              ,[IsValidatePhone]
                              ,[Sort]
                              ,[CreateDateTime]
                              ,[LastUpdateDateTime]
                              ,[IsDeleted]
                              ,[QuestionTextResult]
                              ,[QuestionConfirm]
                              ,[DeadLineTime]          
                            from Activity.[dbo].[Question] with (nolock)
                            -- 卷子ID
                            where QuestionnaireID = @QuestionnaireID
                            -- 结算上线的题目
                            and  QuestionConfirm = 2
                            -- 不能是已经删除的
                            and IsDeleted = 0
                            -- 已经可以清算的数据（题目下线了
                            and EndTime < getdate()
                            -- 服务只结算今天的题目
                            and convert(date,EndTime) = convert(date,getdate())

                            --and convert(date,EndTime) = convert(date,'2018-06-07')
                            --and pkid in (110)
                                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@QuestionnaireID", questionnaireID);
                return DbHelper.ExecuteSelect<QuestionModel>(cmd);
            }
        }

        /// <summary>
        ///     获取正确的选项
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static IEnumerable<QuestionOptionModel> SearchQuestionOption(long questionId)
        {
            var sql = @" SELECT [PKID]
                                  ,[QuestionnaireID]
                                  ,[QuestionID]
                                  ,[OptionContent]
                                  ,[OptionPicSrc]
                                  ,[NoOptionScore]
                                  ,[YesOptionScore]
                                  ,[IsRightValue]
                                  ,[ForkQuestionID]
                                  ,[Sort]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                                  ,[IsAdditionalInfo]
                                  ,[IsShowAdditionalInfo]
                                  ,[QuestionParentID]
                                  ,[UseIntegral]
                                  ,[WinCouponCount] 
                              FROM Activity.[dbo].[QuestionOption] with (nolock)
                              WHERE QuestionID = @QuestionID 
                                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@QuestionID", questionId);
                return DbHelper.ExecuteSelect<QuestionOptionModel>(cmd);
            }
        }


        /// <summary>
        ///     获取用户尚未清算的数据
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static IEnumerable<QuestionnaireAnswerResultModel> SearchUserAnswerNotClear(long questionId, int pageSize)
        {
            var sql = @" SELECT  top " + pageSize + @"   a.[PKID]
                                  ,a.[QuestionnaireAnswerID]
                                  ,a.[AnswerResultStatus]
                                  ,a.[UseIntegral]
                                  ,a.[WinCouponCount]
                                  ,a.[CreateDatetime]
                                  ,a.[LastUpdateDateTime]
                                  ,b.[AnswerOptionID]
                                  ,b.[UserID]
                                  ,b.AnswerDate
                              FROM Activity.[dbo].[QuestionnaireAnswerResult]
        a with(nolock)
                              inner join Activity.[dbo].[QuestionnaireAnswerRecord]
        b with(nolock)
                              on a.QuestionnaireAnswerID = b.PKID and b.QuestionID = @QuestionID
                              WHERE a.AnswerResultStatus = 0
                              order by a.pkid asc
                            ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@QuestionID", questionId);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);

                return DbHelper.ExecuteSelect<QuestionnaireAnswerResultModel>(cmd);
            }
        }

        /// <summary>
        ///     设置用户答案结果
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="answerId"></param>
        /// <param name="couponCount"></param>
        /// <param name="flag"> 1 胜利  2 失败</param>
        /// <returns></returns>
        public static bool UpdateUserAnswerResult(BaseDbHelper helper, long answerId, int couponCount, int flag)
        {
            var sql = @"
                                   update Activity.dbo.QuestionnaireAnswerResult
                                   set AnswerResultStatus = @AnswerResultStatus,WinCouponCount = @WinCouponCount,LastUpdateDateTime=getdate()
                                   where PKID = @PKID
                            ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@AnswerResultStatus", flag);
                cmd.Parameters.AddWithValue("@WinCouponCount", couponCount);
                cmd.Parameters.AddWithValue("@PKID", answerId);
                var result = helper.ExecuteNonQuery(cmd);
                return result > 0;
            }
        }


        public static IEnumerable<string> GetSystemActivityIds()
        {
            using (var cmd = new SqlCommand(@"	                
			 SELECT SystemActivityId FROM Configuration..ActivePageContent AS apc WITH ( NOLOCK)  WHERE SystemActivityId IS NOT NULL"))
            {
                return DbHelper.ExecuteQuery(cmd, dt => dt.ToList<string>());
            }

        }
    }
}
