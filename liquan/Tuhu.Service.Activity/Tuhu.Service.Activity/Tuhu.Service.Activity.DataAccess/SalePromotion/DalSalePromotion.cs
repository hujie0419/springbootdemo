using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalSalePromotion
    {
        /// <summary>
        /// 根据产品id和活动Id查询产品
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SalePromotionActivityProduct>> SelectSalePromotionActivityProducts(IEnumerable<string> pids)
        {
            var sql = @"SELECT  A.PKID ,
        A.ActivityId ,
        A.Pid ,
        A.ProductName ,
        A.TotalStock ,
        A.CostPrice ,
        A.SalePrice ,
        A.LimitQuantity ,
        A.SoldQuantity ,
        A.ImageUrl ,
        A.CreateDateTime ,
        A.LastUpdateDateTime ,
        A.Is_Deleted
FROM    Activity..SalePromotionActivityProduct AS A WITH ( NOLOCK )
        JOIN Activity..SplitString(@Pids, ',', 1) AS B ON A.Pid = B.Item
        JOIN Activity..SalePromotionActivity AS C WITH ( NOLOCK ) ON A.ActivityId = C.ActivityId
WHERE   A.Is_Deleted = 0
        AND C.Is_Deleted = 0
        AND C.AuditStatus = 2
        AND C.Is_UnShelveAuto=0
        AND C.Is_UnShelve = 0
        AND C.StartTime < GETDATE()
        AND C.EndTime > GETDATE()
ORDER BY PKID DESC;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Pids", string.Join(",", pids)));
                return await DbHelper.ExecuteSelectAsync<SalePromotionActivityProduct>(true, cmd);
            }
        }
        /// <summary>
        /// 实时查询活动是否有效
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public static async Task<bool> CheckActivityIsEffictive(string ActivityId)
        {
            var sql = @"SELECT  A.PKID ,
        A.ActivityId ,
        A.Name ,
        A.Description ,
        A.Banner ,
        A.PromotionType ,
        A.Is_DefaultLabel ,
        A.Label ,
        A.Is_PurchaseLimit ,
        A.LimitQuantity ,
        A.PaymentMethod ,
        A.InstallMethod ,
        A.StartTime ,
        A.EndTime ,
        A.AuditStatus ,
        A.AuditUserName ,
        A.Is_UnShelve ,
        A.CreateDateTime ,
        A.CreateUserName ,
        A.LastUpdateDateTime ,
        A.LastUpdateUserName ,
        A.Is_Deleted ,
        A.AuditRemark ,
        A.AuditDateTime
FROM    Activity..SalePromotionActivity AS A WITH ( NOLOCK )
WHERE   A.Is_Deleted = 0
        AND A.AuditStatus = 2
        AND A.Is_UnShelve = 0
        AND A.Is_UnShelveAuto=0
        AND A.StartTime < GETDATE()
        AND A.EndTime > GETDATE()
        AND A.ActivityId = @ActivityId;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@ActivityId", ActivityId));
                return (await DbHelper.ExecuteFetchAsync<SalePromotionActivityModel>(true, cmd)) != null;
            }
        }
        /// <summary>
        /// 查询有效的活动信息
        /// </summary>
        /// <param name="activityIds"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SalePromotionActivityModel>> SelectSalePromotionActivity(IEnumerable<string> activityIds)
        {
            var sql = @"SELECT  A.PKID ,
        A.ActivityId ,
        A.Name ,
        A.Description ,
        A.Banner ,
        A.PromotionType ,
        A.Is_DefaultLabel ,
        A.Label ,
        A.Is_PurchaseLimit ,
        A.LimitQuantity ,
        A.PaymentMethod ,
        A.InstallMethod ,
        A.StartTime ,
        A.EndTime ,
        A.AuditStatus ,
        A.AuditUserName ,
        A.Is_UnShelve ,
        A.CreateDateTime ,
        A.CreateUserName ,
        A.LastUpdateDateTime ,
        A.LastUpdateUserName ,
        A.Is_Deleted ,
        A.AuditRemark ,
        A.AuditDateTime
FROM    Activity..SalePromotionActivity AS A WITH ( NOLOCK )
        JOIN Activity..SplitString(@ActivityIds, ',', 1) AS B ON A.ActivityId = B.Item
WHERE   A.Is_Deleted = 0
        AND A.AuditStatus = 2
        AND A.Is_UnShelve = 0
        AND A.Is_UnShelveAuto=0
        AND A.StartTime < GETDATE()
        AND A.EndTime > GETDATE()
ORDER BY PKID DESC; 
";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@ActivityIds", string.Join(",", activityIds)));
                return await DbHelper.ExecuteSelectAsync<SalePromotionActivityModel>(true, cmd);
            }
        }
        /// <summary>
        /// 查询活动打折信息
        /// </summary>
        /// <param name="activityIds"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SalePromotionActivityDiscount>> SelectSalePromotionActivityDiscounts(IEnumerable<string> activityIds)
        {
            var sql = @"SELECT  A.PKID ,
        A.ActivityId ,
        A.DiscountMethod ,
        A.Condition ,
        A.DiscountRate ,
        A.CreateDateTime ,
        A.Is_Deleted
FROM    Activity..SalePromotionActivityDiscount AS A WITH ( NOLOCK )
        JOIN Activity..SplitString(@ActivityIds, ',', 1) AS B ON A.ActivityId = B.Item
WHERE   A.Is_Deleted = 0;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@ActivityIds", string.Join(",", activityIds)));
                return await DbHelper.ExecuteSelectAsync<SalePromotionActivityDiscount>(true, cmd);
            }
        }
    }
}
