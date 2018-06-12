using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.YLHCouponInit.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.YLHCouponInit.DAL
{
    public class YLHCouponInitDal
    {
        public IEnumerable<YLHResidueProject> GetAllYLHResidueProject()
        {
            string sql = @"SELECT [MemberNumber] as [Display_Card_NBR],
[RemainCount],
[EffectiveDate],
[DefaultPrice],
[ProjectName]
FROM [Activity].[dbo].[YLHResidueService]
";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<YLHResidueProject>(true, cmd);
                return result;
            }
        }

        public List<string> SplitYLHIgnoreServiceProjects()
        {
            List<string> result = new List<string>();
            StringBuilder jsonArrayTextTmp = new StringBuilder();
            using (StreamReader sr = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\IgnoreServiceProject.json"))
            {
                string input = null;
                while ((input = sr.ReadLine()) != null)
                {
                    jsonArrayTextTmp.Append(input);
                }
            }

            JArray ja = (JArray)JsonConvert.DeserializeObject(jsonArrayTextTmp.ToString());
            foreach (var item in ja)
            {
                result.Add(((JObject)item)["ProjectName"].ToString());
            }
            return result;
        }

        public UserObjectModel GetUserIDByDisplayCardNBR(string DisplayCardNBR)
        {
            string sql = @"SELECT TOP 1 * FROM Tuhu_profiles.dbo.UserObject(NOLOCK)
                where UserID in(
                select top 1 [u_user_id]
                from [Tuhu_profiles].[dbo].[YLH_UserInfo] 
                WHERE MemberNumber=@DisplayCardNBR)
ORDER BY ISNULL(dt_date_registered, dt_date_created)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@DisplayCardNBR", DisplayCardNBR);
                var result = DbHelper.ExecuteFetch<UserObjectModel>(true, cmd);
                return result;
            }
        }

        public CouponRuleModel SelectCouponRule(string description, double price)
        {
            string sql = @"SELECT top 1  CR.PKID as RuleID,
                                        CR.[Name] as [Description],
										YLHCT.券类型 AS [PromotionName]
                                FROM    Activity..[tbl_CouponRules] AS CR WITH ( NOLOCK )
								LEFT JOIN Activity..[YLHCouponTemp] AS YLHCT
								ON YLHCT.RuleID = CR.PKID
                                Where CR.PKID in (
                                select top 1 RuleID from Activity..[YLHCouponTemp]
                                WHERE  [项目] = @Description)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Description", "%" + description + "%");
                var result = DbHelper.ExecuteFetch<CouponRuleModel>(true, cmd);
                return result;
            }
        }

        public int CreatePromotion(PromotionCodeModel model)
        {
            try
            {
                using (var cmd = new SqlCommand("[Gungnir].[dbo].[Beautify_CreatePromotionCode]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StartTime", model.StartTime.ToString());
                    cmd.Parameters.AddWithValue("@EndDateTime", model.EndTime.ToString());
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@Discount", model.Discount);
                    cmd.Parameters.AddWithValue("@MinMoney", model.MinMoney);
                    cmd.Parameters.AddWithValue("@Number", model.Number);
                    cmd.Parameters.AddWithValue("@CodeChannel", model.CodeChannel);
                    cmd.Parameters.AddWithValue("@UserID", model.UserId);
                    cmd.Parameters.AddWithValue("@BatchID", model.BatchId == null ? 0 : model.BatchId.Value);
                    cmd.Parameters.AddWithValue("@RuleID", model.RuleId);
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Results",
                        DbType = DbType.Int32,
                        Direction = ParameterDirection.Output,
                        Value = 0
                    });

                    DbHelper.ExecuteNonQuery(cmd);
                    var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                return -99;
            }
        }

        public IEnumerable<CouponDiffModel> GetAllCouponDiff()
        {
            string sql = @"SELECT PKID,
u_user_id,
MemberNumber,
ProjectNumber AS ProjectName,
RuleID,
Diff
FROM Activity.dbo.YLHResidueServiceDiff(NOLOCK)";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<CouponDiffModel>(true, cmd);
                return result;
            }
        }

        public List<PromotionCodeSimplifyModel> GetPromotionPKIDList(CouponDiffModel model)
        {
            string sql = @"SELECT 
PKID,
Code
FROM Gungnir.dbo.tbl_PromotionCode (NOLOCK)
WHERE  CodeChannel='YLH' AND Status=0
AND  RuleID=@ruleID
AND UserId=@userID
ORDER BY EndTime";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ruleID",model.RuleID);
                cmd.Parameters.AddWithValue("@userID", Guid.Parse(model.UserID));
                var result = DbHelper.ExecuteSelect<PromotionCodeSimplifyModel>(true, cmd);
                return result.ToList();
            }
        }

        public int UpdatePromotionStatus(PromotionCodeSimplifyModel model)
        {
            string sql = @"UPDATE Gungnir.dbo.tbl_PromotionCode
SET Status=3
WHERE PKID=@pkid";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@pkid", model.PKID);
                    DbHelper.ExecuteNonQuery(cmd);
                    return 1;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
                return -99;
            }
        }

        public int InsertYewuLogForCleanYLHCoupon(PromotionCodeSimplifyModel promotion, CouponDiffModel model)
        {
            string sql = @"INSERT SystemLog..PromotionOprLog
(PromotionPKID,Author,Operation,Channel,OprDateTime,UserID)
VALUES
(@pkid,'Tuhu.C.YunYing.JobScheduler',N'清理多余YLH优惠券','YLH',getdate(),@userID)";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@pkid", promotion.PKID);
                    cmd.Parameters.AddWithValue("@userID", Guid.Parse(model.UserID));
                    DbHelper.ExecuteNonQuery(cmd);
                    return 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                return -99;
            }
        }
    }
}
