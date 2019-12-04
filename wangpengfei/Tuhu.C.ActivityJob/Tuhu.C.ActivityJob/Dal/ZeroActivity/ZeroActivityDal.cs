using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Tuhu.C.ActivityJob.Models.ZeroActivity;

namespace Tuhu.C.ActivityJob.Dal.ZeroActivity
{
    /// <summary>
    /// 【0元众测】
    /// </summary>
    public class ZeroActivityDal
    {
        /// <summary>
        /// 获取要发送评测提醒短信的用户信息:  已中奖、未评测、中奖23天~26天、未发送短信
        /// </summary>
        /// <returns></returns>
        public static List<ZeroActivityApply> GetNeedMessageApplyList()
        {
            string sql = @"Select TOP 2000
                                  PKID,
                                  UserID,
                                  UserMobileNumber
                              FROM Activity.dbo.tbl_ZeroActivity_Apply WITH (NOLOCK)
                              WHERE Status = 1
                                    AND ReportStatus = 0
                                    AND IsSentMessage = 0
                                    AND PrizeDateTime IS NOT NULL
                                    AND DATEDIFF(DAY, PrizeDateTime, @dateNow) > 22
                                    AND DATEDIFF(DAY, PrizeDateTime, @dateNow) < 27;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateNow", DateTime.Now);
                var dnResult = DbHelper.ExecuteSelect<ZeroActivityApply>(cmd);
                return dnResult?.ToList();
            }
        }

        /// <summary>
        /// 发送短信提醒后 修改短信发送状态
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <returns></returns>
        public static int SetMessageSendStatus(List<int> PKIDs)
        {
            string sql = @"UPDATE a
                            SET a.IsSentMessage = 1,
                                a.LastUpdateDateTime = GETDATE()
                            FROM [Activity].[dbo].[tbl_ZeroActivity_Apply] AS a WITH (ROWLOCK)
                                JOIN Activity..SplitString(@PKIDs, ',', 1) AS b
                                    ON a.PKID = b.Item
                            WHERE a.IsSentMessage = 0
                                  OR a.IsSentMessage IS NULL;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKIDs", string.Join(",", PKIDs));
                var dnResult = DbHelper.ExecuteNonQuery(cmd);
                int.TryParse(dnResult.ToString(), out int count);
                return count;
            }
        }

    }
}
