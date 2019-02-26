using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    /// <summary>
    /// 一分钱洗车优惠券 数据层
    /// </summary>
    public class DalWashCarCouponRecord
    {
        /// <summary>
        ///  新增 一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task<int> CreateWashCarCouponAsync(WashCarCouponRecordEntity entity)
        {
            #region sql
            var sql = @" insert into  Activity.[dbo].[WashCarCouponRecord]
                         (  
                            UserID
                           ,PromotionCodeID
                           ,CarID
                           ,CarNo
                           ,VehicleID
                           ,Vehicle
                           ,PaiLiang
                           ,Nian
                           ,Tid
                           ,CreateDateTime
                           ,LastUpdateDateTime
                         )
                         values (
                            @UserID
                           ,@PromotionCodeID
                           ,@CarID
                           ,@CarNo
                           ,@VehicleID
                           ,@Vehicle
                           ,@PaiLiang
                           ,@Nian
                           ,@Tid
                           ,getdate()
                           ,getdate()
                         );
                        SELECT SCOPE_IDENTITY();
                          ";
            #endregion


            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserID", entity.UserID);
                cmd.Parameters.AddWithValue("@PromotionCodeID", entity.PromotionCodeID);
                cmd.Parameters.AddWithValue("@CarID", entity.CarID);
                cmd.Parameters.AddWithValue("@CarNo", entity.CarNo);
                cmd.Parameters.AddWithValue("@VehicleID", entity.VehicleID);
                cmd.Parameters.AddWithValue("@Vehicle", entity.Vehicle);
                cmd.Parameters.AddWithValue("@PaiLiang", entity.PaiLiang);
                cmd.Parameters.AddWithValue("@Nian", entity.Nian);
                cmd.Parameters.AddWithValue("@Tid", entity.Tid);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(cmd));
            }
        }

        /// <summary>
        /// 获取  一分钱洗车优惠券领取记录 通过用户id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<List<WashCarCouponRecordEntity>> GetWashCarCouponListByUseridsAsync(Guid userID)
        {
            #region sql
            var sql = @" SELECT [PKID]
                          ,[UserID]
                          ,[PromotionCodeID]
                          ,[CarID]
                          ,[CarNo]
                          ,[VehicleID]
                          ,[Vehicle]
                          ,[PaiLiang]
                          ,[Nian]
                          ,[Tid]
                          ,[CreateDateTime]
                          ,[LastUpdateDateTime]
                      FROM Activity..WashCarCouponRecord With (nolock)
                      WHERE UserID =@UserID
                          ";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserID", userID);
                return (await DbHelper.ExecuteSelectAsync<WashCarCouponRecordEntity>(true,cmd)).ToList();
            }

        }

        /// <summary>
        /// 根据优惠券id获取  一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="PromotionCodeID"></param>
        /// <returns></returns>
        public static async Task<List<WashCarCouponRecordEntity>> GetWashCarCouponListByPromotionCodeIDAsync(int PromotionCodeID)
        {
            #region sql
            var sql = @" SELECT [PKID]
                          ,[UserID]
                          ,[PromotionCodeID]
                          ,[CarID]
                          ,[CarNo]
                          ,[VehicleID]
                          ,[Vehicle]
                          ,[PaiLiang]
                          ,[Nian]
                          ,[Tid]
                          ,[CreateDateTime]
                          ,[LastUpdateDateTime]
                      FROM Activity..WashCarCouponRecord With (nolock)
                      WHERE PromotionCodeID =@PromotionCodeID
                          ";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PromotionCodeID", PromotionCodeID);
                return (await DbHelper.ExecuteSelectAsync<WashCarCouponRecordEntity>(true, cmd)).ToList();
            }

        }
    }
}
