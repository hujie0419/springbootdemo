using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using System.Data.SqlClient;
using System.Data;

namespace Tuhu.Service.Activity.DataAccess
{
   public class DalFightGroupsPacketsLog
    {
        /// <summary>
        /// 获取分享红包组列表
        /// </summary>
        /// <param name="fightGroupsIdentity"></param>
        /// <returns></returns>
        public static async Task<List<FightGroupsPacketsLogModel>> GetFightGroupsPacketsList(Guid fightGroupsIdentity)
        {
            string sql = "SELECT * FROM Tuhu_log.dbo.FightGroupsPacketsLog WITH (NOLOCK) WHERE FightGroupsIdentity=@FightGroupsIdentity";
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FightGroupsIdentity",fightGroupsIdentity);
                return (await db.ExecuteSelectAsync<FightGroupsPacketsLogModel>(cmd))?.ToList();
            }
        }

        /// <summary>
        /// 插入分享红包组信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static async Task<bool> InsertFightGroupsPacket(List<FightGroupsPacketsLogModel> list)
        {
            string sql = @"
                           INSERT INTO Tuhu_log.dbo.FightGroupsPacketsLog
                            ( CreateDateTime ,
                              LastUpdateDateTime ,
                              FightGroupsIdentity ,
                              UserId ,
                              GetRuleGuid ,
                              IsLeader ,
                              PromotionPKID ,
                              OrderBy
                            )
                    OUTPUT Inserted.PKID
                    VALUES  ( GETDATE() , -- CreateDateTime - datetime
                              GETDATE() , -- LastUpdateDateTime - datetime
                              @FightGroupsIdentity , -- FightGroupsIdentity - uniqueidentifier
                              @UserId , -- UserId - uniqueidentifier
                              @GetRuleGuid , -- GetRuleGuid - uniqueidentifier
                              @IsLeader , -- IsLeader - bit
                              @PromotionPKID , -- PromotionPKID - int
                              @OrderBy  -- OrderBy - int
                            )	 ";

            using (var db = DbHelper.CreateLogDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var item in list)
                    {
                        SqlCommand cmd = new SqlCommand(sql);
                        cmd.Parameters.AddWithValue("@FightGroupsIdentity", item.FightGroupsIdentity);
                        cmd.Parameters.AddWithValue("@UserId", item.UserId);
                        cmd.Parameters.AddWithValue("@GetRuleGuid", item.GetRuleGuid);
                        cmd.Parameters.AddWithValue("@IsLeader", item.IsLeader);
                        cmd.Parameters.AddWithValue("@PromotionPKID", item.PromotionPKID);
                        cmd.Parameters.AddWithValue("@OrderBy", item.OrderBy);
                        object obj = await db.ExecuteScalarAsync(cmd);
                        item.PKID = Convert.ToInt32(obj);
                    }
                    db.Commit();
                    return true;
                }
                catch (Exception em)
                {
                    db.Rollback();
                    throw em;
                }
            }
        }

        /// <summary>
        /// 更新分享红包用户应有的优惠券
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<int> UpdateFightGroupsPacketByUserId(Guid fightGroupsIdentity, Guid userId)
        {
            string sql = @"UPDATE Tuhu_log.dbo.FightGroupsPacketsLog SET UserId=@UserId, LastUpdateDateTime=GETDATE() 
            WHERE PKID=(SELECT MIN(PKID) FROM Tuhu_log.dbo.FightGroupsPacketsLog WHERE FightGroupsIdentity=@FightGroupsIdentity AND UserId IS NULL);
            SELECT PKID FROM Tuhu_log.dbo.FightGroupsPacketsLog WITH(NOLOCK) WHERE FightGroupsIdentity=@FightGroupsIdentity AND UserId=@UserId";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FightGroupsIdentity", fightGroupsIdentity);
                var obj = await db.ExecuteScalarAsync(cmd);
                if (obj == null) return 0;
                return (int) obj;
            }
        }

        /// <summary>
        /// 记录发放优惠券的promotionPKID
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="promotionPKID"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateFightGroupsPacketByPromotionPKID(int pkid, int promotionPKID)
        {
            string sql = "UPDATE Tuhu_log.dbo.FightGroupsPacketsLog SET PromotionPKID=@PromotionPKID WHERE PKID=@PKID";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@PromotionPKID", promotionPKID);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return (await db.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        /// <summary>
        /// 查询用户是否有正在拼的团
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<List<FightGroupsPacketsLogModel>> SelectFightGroupsPacketByUser(Guid userId)
        {
            string sql = @"SELECT * FROM Tuhu_log.dbo.FightGroupsPacketsLog WITH (NOLOCK) WHERE FightGroupsIdentity IN (

                SELECT TOP 1 FightGroupsIdentity FROM Tuhu_log.dbo.FightGroupsPacketsLog WITH (NOLOCK)  WHERE UserId=@UserId AND IsLeader=1 ORDER BY CreateDateTime DESC
                )";
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", userId);
                return (await db.ExecuteSelectAsync<FightGroupsPacketsLogModel>(cmd))?.ToList();
            }
        }

        /// <summary>
        /// 记录创建优惠券的信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateCreatePromotionPKID(List<FightGroupsPacketsLogModel> list)
        {
            string sql = " UPDATE Tuhu_log.dbo.FightGroupsPacketsLog SET PromotionPKID=@PromotionPKID WHERE PKID=@PKID ";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var item in list)
                    {
                        SqlCommand cmd = new SqlCommand(sql);
                        cmd.Parameters.AddWithValue("@PromotionPKID", item.PromotionPKID);
                        cmd.Parameters.AddWithValue("@PKID", item.PKID);
                        await db.ExecuteNonQueryAsync(cmd);
                    }
                    db.Commit();
                    return true;
                }
                catch (Exception em)
                {
                    db.Rollback();
                    throw em;
                }
            }
        }

    }
}
