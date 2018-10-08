using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models;

namespace Tuhu.Service.Activity.DataAccess.OARedEnvelope
{
    /// <summary>
    ///     公众号领红包 - 统计
    /// </summary>
    public class DalOARedEnvelopeStatistics
    {
        /// <summary>
        ///     获取公众号领红包统计
        /// </summary>
        /// <returns></returns>
        public static async Task<OARedEnvelopeStatisticsModel> GetOARedEnvelopeStatisticsAsync(int officialAccountType,
            DateTime date)
        {
            var sql = @" select  [PKID]
                                  ,[StatisticsDate]
                                  ,[DayMaxMoney]
                                  ,[UserCount]
                                  ,[RedEnvelopeCount]
                                  ,[RedEnvelopeSumMoney]
                                  ,[RedEnvelopeAvg]
                                  ,[OfficialAccountType]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                         from [Activity].[dbo].[tbl_OARedEnvelopeStatistics] with (nolock)
                         where OfficialAccountType = @officialAccountType and StatisticsDate =@StatisticsDate
                        ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@officialAccountType", officialAccountType);
                cmd.AddParameter("@StatisticsDate", date.Date);

                return await DbHelper.ExecuteFetchAsync<OARedEnvelopeStatisticsModel>(true, cmd);
            }
        }


        /// <summary>
        ///     保存公众号领红包统计
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static async Task<long> InsertOARedEnvelopeStatisticsAsync(BaseDbHelper dbHelper,
            OARedEnvelopeStatisticsModel setting)
        {
            var sql = @"INSERT INTO [Activity].[dbo].[tbl_OARedEnvelopeStatistics]
                            ([StatisticsDate]
                               ,[DayMaxMoney]
                               ,[UserCount]
                               ,[RedEnvelopeCount]
                               ,[RedEnvelopeSumMoney]
                               ,[RedEnvelopeAvg]
                               ,[OfficialAccountType]
                               ,[CreateDatetime]
                               ,[LastUpdateDateTime])
                        VALUES
                            (
                                @StatisticsDate
                               ,@DayMaxMoney
                               ,@UserCount
                               ,@RedEnvelopeCount
                               ,@RedEnvelopeSumMoney
                               ,@RedEnvelopeAvg
                               ,@OfficialAccountType
                               ,getdate()
                               ,getdate()
                            );
                      SELECT SCOPE_IDENTITY();

            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@StatisticsDate", setting.StatisticsDate);
                cmd.AddParameter("@DayMaxMoney", setting.DayMaxMoney);
                cmd.AddParameter("@UserCount", setting.UserCount);
                cmd.AddParameter("@RedEnvelopeCount", setting.RedEnvelopeCount);
                cmd.AddParameter("@RedEnvelopeSumMoney", setting.RedEnvelopeSumMoney);
                cmd.AddParameter("@RedEnvelopeAvg", setting.RedEnvelopeAvg);
                cmd.AddParameter("@OfficialAccountType", setting.OfficialAccountType);


                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     更新公众号领红包统计
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateOARedEnvelopeStatisticsAsync(BaseDbHelper dbHelper,
            OARedEnvelopeStatisticsModel setting)
        {
            var sql = @"

                        UPDATE Activity.[dbo].[tbl_OARedEnvelopeStatistics]
                              SET [StatisticsDate] = @StatisticsDate
                                  ,[DayMaxMoney] = @DayMaxMoney
                                  ,[UserCount] = @UserCount
                                  ,[RedEnvelopeCount] = @RedEnvelopeCount
                                  ,[RedEnvelopeSumMoney] = @RedEnvelopeSumMoney
                                  ,[RedEnvelopeAvg] = @RedEnvelopeAvg
                                  ,[OfficialAccountType] = @OfficialAccountType
                                  ,[LastUpdateDateTime] =  getdate()
                             WHERE  pkid = @pkid
        
                    ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@pkid", setting.PKID);

                cmd.AddParameter("@StatisticsDate", setting.StatisticsDate);
                cmd.AddParameter("@DayMaxMoney", setting.DayMaxMoney);
                cmd.AddParameter("@UserCount", setting.UserCount);
                cmd.AddParameter("@RedEnvelopeCount", setting.RedEnvelopeCount);
                cmd.AddParameter("@RedEnvelopeSumMoney", setting.RedEnvelopeSumMoney);
                cmd.AddParameter("@RedEnvelopeAvg", setting.RedEnvelopeAvg);
                cmd.AddParameter("@OfficialAccountType", setting.OfficialAccountType);

                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
    }
}
