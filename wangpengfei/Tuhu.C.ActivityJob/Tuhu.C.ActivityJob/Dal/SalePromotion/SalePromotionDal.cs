using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Models.SalePromotion;

namespace Tuhu.C.ActivityJob.Dal.SalePromotion
{
    public class SalePromotionDal
    {
        public static List<FlashSaleModel> GetNextDayFlashSales(int activeType)
        {
            string sql = @"SELECT ActivityID,
                                   ActivityName,
                                   StartDateTime,
                                   EndDateTime
                            FROM Activity..tbl_FlashSale WITH (NOLOCK)
                            WHERE DATEDIFF(DAY, GETDATE(), StartDateTime) = 1
                                  AND ActiveType = @ActiveType;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ActiveType", activeType);
                return DbHelper.ExecuteSelect<FlashSaleModel>(cmd)?.ToList();
            }
        }

        public static List<FlashSaleModel> GetNextDayFlashSaleTemps(int activeType)
        {
            string sql = @"SELECT ActivityID,
                                   ActivityName,
                                   StartDateTime,
                                   EndDateTime
                            FROM Activity..tbl_FlashSale_Temp WITH (NOLOCK)
                            WHERE DATEDIFF(DAY, GETDATE(), StartDateTime) = 1
                                  AND ActiveType = @ActiveType;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ActiveType", activeType);
                return DbHelper.ExecuteSelect<FlashSaleModel>(cmd)?.ToList();
            }
        }

        public static List<ApprovalStatusModel> GetActivityApprovalStatus(List<string> activityIds)
        {
            string sql = @"SELECT ActivityId,
                                   Status
                            FROM Configuration..ActivityApprovalStatus AS S WITH (NOLOCK)
                                JOIN Configuration..SplitString(@ActivityIds, ',', 1) AS T
                                    ON S.ActivityId = T.Item;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ActivityIds", string.Join(",", activityIds));
                return DbHelper.ExecuteSelect<ApprovalStatusModel>(cmd)?.ToList();
            }
        }

        public static List<GiftModel> GetNextDayGiftActivities()
        {
            string sql = @"SELECT Id,
                                   Name,
                                   ValidTimeBegin,
                                   ValidTimeEnd
                            FROM Configuration..SE_GiftManageConfig WITH (NOLOCK)
                            WHERE DATEDIFF(DAY, GETDATE(), ValidTimeBegin) = 1
                                  AND State = 1
                                  AND ActivityType = 1;";

            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteSelect<GiftModel>(cmd)?.ToList();
            }
        }

        public static List<GroupBuyingModel> GetNextDayGroupBuyings()
        {
            string sql = @"SELECT G.ProductGroupId,
                                   P.ProductName,
                                   G.BeginTime,
                                   G.EndTime
                            FROM Configuration..GroupBuyingProductGroupConfig AS G WITH (NOLOCK)
                                JOIN Configuration..GroupBuyingProductConfig AS P WITH (NOLOCK)
                                    ON G.ProductGroupId = P.ProductGroupId
                            WHERE DATEDIFF(DAY, GETDATE(), G.BeginTime) = 1
                                  AND G.IsShow = 1
                                  AND G.IsDelete = 0
                                  AND P.DisPlay = 1
                                  AND P.IsShow = 1
                                  AND P.IsDelete = 0;";

            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteSelect<GroupBuyingModel>(cmd)?.ToList();
            }
        }

        public static List<SalePromotionModel> GetNextDaySalePromotions()
        {
            string sql = @"SELECT ActivityId,
                                   Name,
	                               AuditStatus,
                                   StartTime,
                                   EndTime
                            FROM Activity..SalePromotionActivity WITH (NOLOCK)
                            WHERE DATEDIFF(DAY, GETDATE(), StartTime) = 1
                                  AND Is_UnShelve = 0
                                  AND
                                  (
                                      AuditStatus = 1
                                      OR AuditStatus = 2
                                  )
                                  AND Is_Deleted = 0;";

            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteSelect<SalePromotionModel>(cmd)?.ToList();
            }
        }
    }
}
