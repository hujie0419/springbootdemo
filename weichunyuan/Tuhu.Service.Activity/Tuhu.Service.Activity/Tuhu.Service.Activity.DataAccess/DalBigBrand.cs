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
   public class DalBigBrand
    {

        /// <summary>
        /// 获取大翻牌配置列表数据信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public async static Task<BigBrandRewardListModel> GetBigBrandRewardListModel(string keyValue)
        {
            string sql = @"SELECT * FROM Configuration.dbo.BigBrandRewardList WITH (NOLOCK) WHERE HashKeyValue=@HashKeyValue";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                var result = await db.ExecuteSelectAsync<BigBrandRewardListModel>(sql, System.Data.CommandType.Text, new SqlParameter("@HashKeyValue", keyValue));
                return result?.FirstOrDefault() ?? new BigBrandRewardListModel();
            }
        }

        /// <summary>
        /// 获取奖励池信息
        /// </summary>
        /// <param name="fkPKID"></param>
        /// <returns></returns>
        public async static Task<List<BigBrandRewardPoolModel>> GetBigBrandRewardPoolList(int fkPKID)
        {
            string sql = @"SELECT * FROM Configuration.dbo.BigBrandRewardPool WITH (NOLOCK) WHERE FKPKID=@FKPKID ";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                var result = await db.ExecuteSelectAsync<BigBrandRewardPoolModel>(sql, System.Data.CommandType.Text, new SqlParameter("@FKPKID", fkPKID));
                return result?.ToList();
            }
        }

        /// <summary>
        /// 获取页面样式配置
        /// </summary>
        /// <param name="fkPKID"></param>
        /// <returns></returns>
        public async static Task<List<BigBrandPageStyleModel>> GetBigBrandPageStyleList(int fkPKID)
        {
            string sql = @"SELECT * FROM Configuration.dbo.BigBrandPageStyle WITH (NOLOCK) WHERE FKPKID=@FKPKID";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                var result = await db.ExecuteSelectAsync<BigBrandPageStyleModel>(sql, System.Data.CommandType.Text, new SqlParameter("@FKPKID", fkPKID));
                return result?.ToList();
            }
        }

        /// <summary>
        /// 获取抽奖次数对应的奖励池信息
        /// </summary>
        /// <param name="fkPKID"></param>
        /// <returns></returns>
        public async static Task<List<BigBrandWheelModel>> GetBigBrandWheelList(int fkPKID)
        {
            string sql = @"SELECT * FROM Configuration.dbo.BigBrandWheel WITH (NOLOCK) WHERE FKBigBrand=@FKBigBrand";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                var result = await db.ExecuteSelectAsync<BigBrandWheelModel>(sql, System.Data.CommandType.Text, new SqlParameter("@FKBigBrand", fkPKID));
                return result?.ToList();
            }
        }


        /// <summary>
        /// 添加领取日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public async static Task<bool> AddBigBrandLog(BigBrandRewardLogModel model)
        {
            string sql = @"INSERT INTO SystemLog.dbo.BigBrandRewardLog
        ( CreateDateTime ,
          LastUpdateDateTime ,
          Refer ,
          FKPKID ,
          ChanceType ,
          UserId ,
          Phone ,
          Status ,
          Channel ,
          DeviceSerialNumber ,
          Remark,FKBigBrandPkid,PromotionCodePKIDs,UnionId
        )
VALUES  ( GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- LastUpdateDateTime - datetime
          @Refer , -- Refer - varchar(3000)
          @FKPKID , -- FKPKID - int
          @ChanceType , -- ChanceType - int
          @UserId , -- UserId - uniqueidentifier
          @Phone , -- Phone - varchar(20)
          @Status , -- Status - bit
          @Channel , -- Channel - varchar(20)
          @DeviceSerialNumber , -- DeviceSerialNumber - varchar(100)
          @Remark,  -- Remark - varchar(100)
          @FKBigBrandPkid,@PromotionCodePKIDs,@UnionId
        )";

            using (var db = DbHelper.CreateDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Refer", model.Refer);
                cmd.Parameters.AddWithValue("@FKPKID", model.FKPKID);
                cmd.Parameters.AddWithValue("@ChanceType", model.ChanceType);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@Status", model.Status);
                cmd.Parameters.AddWithValue("@Channel", model.Channel);
                cmd.Parameters.AddWithValue("@DeviceSerialNumber", model.DeviceSerialNumber);
                cmd.Parameters.AddWithValue("@Remark", model.Remark);
                cmd.Parameters.AddWithValue("@FKBigBrandPkid", model.FKBigBrandPkid);
                cmd.Parameters.AddWithValue("@PromotionCodePKIDs",model.PromotionCodePKIDs);
                cmd.Parameters.AddWithValue("@UnionId", model.UnionId);
                return await db.ExecuteNonQueryAsync(cmd) > 0;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hashKey"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public  static bool IsExistShareLog(Guid userId, string hashKey,string channel)
        {
            string sql = @"SELECT COUNT(*) FROM SystemLog.dbo.BigBrandRewardLog WITH (NOLOCK) WHERE ChanceType=2 AND UserId=@UserId  
                AND Channel=@Channel AND FKBigBrandPkid IN (SELECT  TOP 1 PKID  FROM Configuration.dbo.BigBrandRewardList WITH (NOLOCK) WHERE HashKeyValue=@HashKeyValue)";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Channel", channel);
                cmd.Parameters.AddWithValue("@HashKeyValue",hashKey);
                var obj = db.ExecuteScalar(cmd);
                if (obj == null)
                    return false;
                return (Convert.ToInt32(obj)) > 0;
            }
        }

        /// <summary>
        /// 获取领取日志
        /// </summary>
        /// <param name="fkpkid"></param>
        /// <param name="period">周期数</param>
        /// <param name="periodType"> 1：小时 2：天 3： 月 （从创建日期开始）</param>
        /// <returns></returns>
        public static  int GetBigBrandLogList(int fkBigBrandPkid, int fkPKID, DateTime startDT,DateTime endDT)
        {
            string sql = @"SELECT ISNULL(COUNT(*),0)  AS Number FROM SystemLog.dbo.BigBrandRewardLog WITH (NOLOCK) WHERE FKBigBrandPkid=@FKBigBrandPkid AND FKPKID=@FKPKID AND ChanceType=1 AND CreateDateTime >=@StartDT AND CreateDateTime<@EndDT ";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FKBigBrandPkid", fkBigBrandPkid);
                cmd.Parameters.AddWithValue("@StartDT", startDT);
                cmd.Parameters.AddWithValue("@EndDT", endDT);
                cmd.Parameters.AddWithValue("@FKPKID",fkPKID);
                var obj =  db.ExecuteScalar(cmd);
                if (obj == null)
                    return 0;
                else
                    return Convert.ToInt32(obj);
            }
        }

        public static Dictionary<int, int> GetBatchBigBrandLogList(int fkBigBrandPkid, IEnumerable<int> fkPkids,
            DateTime startDT, DateTime endDT)
        {
            string sql =
                @"SELECT FKPKID,ISNULL(COUNT(1),0)  AS Number FROM SystemLog.dbo.BigBrandRewardLog WITH (NOLOCK) WHERE FKBigBrandPkid=@FKBigBrandPkid AND FKPKID IN(SELECT Item FROM SystemLog..SplitString(@FKPKID,',',1)) AND ChanceType=1 AND CreateDateTime >=@StartDT AND CreateDateTime<@EndDT GROUP BY FKPKID";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FKBigBrandPkid", fkBigBrandPkid);
                cmd.Parameters.AddWithValue("@StartDT", startDT);
                cmd.Parameters.AddWithValue("@EndDT", endDT);
                cmd.Parameters.AddWithValue("@FKPKID", string.Join(",", fkPkids));
                var obj = db.ExecuteQuery(cmd, dt => dt);
                if (obj.Rows.Count > 0)
                {
                    var result = new Dictionary<int, int>(obj.Rows.Count);
                    obj.Rows.OfType<DataRow>().ForEach(row =>
                    {
                        result[row.GetValue<int>("FKPKID")] = row.GetValue<int>("Number");
                    });
                    return result;
                }
                else
                {
                    return new Dictionary<int, int>();
                }
            }
        }



        public static async Task<List<BigBrandRewardLogModel>> GetBigBrandLogListByUserId(int fkBigBrandPkid, DateTime startDT,DateTime endDT,  Guid userId,string unionId)
        {
            string sql = @"SELECT PKID,CreateDateTime,LastUpdateDateTime,Refer,FKPKID,ChanceType,UserId,Phone,Status,Channel,DeviceSerialNumber,Remark,FKBigBrandPkid,PromotionCodePKIDs,UnionId FROM SystemLog.dbo.BigBrandRewardLog WITH (NOLOCK) WHERE FKBigBrandPkid=@FKBigBrandPkid AND CreateDateTime >=@StartDT AND CreateDateTime<@EndDT ";
            var userSql = new List<string>();
            if (userId != Guid.Empty)
            {
                userSql.Add("  UserId=@UserId ");
            }
            if (!string.IsNullOrEmpty(unionId))
            {
                userSql.Add(" UnionId=@UnionId ");
            }
            if (userSql.Count > 0)
            {
                sql += $"AND ({string.Join(" OR ", userSql)})";
            }
            using (var db = DbHelper.CreateDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FKBigBrandPkid", fkBigBrandPkid);
                cmd.Parameters.AddWithValue("@StartDT", startDT);
                cmd.Parameters.AddWithValue("@EndDT", endDT);
                cmd.Parameters.AddWithValue("@UnionId", unionId);
                return (await db.ExecuteSelectAsync<BigBrandRewardLogModel>(cmd))?.ToList();
            }
        }

        /// <summary>
        /// 获取领取包
        /// </summary>
        /// <param name="fkpkid"></param>
        /// <returns></returns>
        public static async Task<List<BigBrandPackerModel>> GetSelectPackageDataTable(int fkBigBrandPkid)
        {
            string sql = @"SELECT BBRP.Name, SUBSTRING(BBRL.Phone,0,4)+'****'+SUBSTRING(BBRL.Phone,8,4) AS  Phone FROM 
 (SELECT TOP 100  FKPKID, Phone FROM SystemLog.dbo.BigBrandRewardLog WITH (NOLOCK) WHERE   FKBigBrandPkid=@FKBigBrandPkid AND ChanceType=1 
  ORDER BY pkid DESC) AS BBRL  LEFT JOIN Configuration.dbo.BigBrandRewardPool  BBRP WITH (NOLOCK) 
 ON BBRL.FKPKID =BBRP.PKID";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@FKBigBrandPkid", fkBigBrandPkid);
                return (await db.ExecuteSelectAsync<BigBrandPackerModel>(cmd))?.ToList();

            }
        }

        /// <summary>
        /// 获取最近领取的优惠礼包信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bigBrandPKID"></param>
        /// <returns></returns>
        public static Tuhu.Service.Activity.Models.BigBrandRewardPoolModel GetRewardInfoLast(Guid userId, int bigBrandPKID)
        {
            string sql = @"SELECT  RP.*, M.CreateDateTime AS DateTimeLog
        FROM    Configuration.dbo.BigBrandRewardPool RP WITH ( NOLOCK )
                INNER JOIN ( SELECT TOP 1
                                    *
                             FROM   SystemLog.dbo.BigBrandRewardLog BRL WITH ( NOLOCK )
                             WHERE  BRL.UserId = @UserId AND BRL.FKBigBrandPkid=@FKBigBrandPkid AND BRL.[Status]=1
                             ORDER BY BRL.CreateDateTime DESC
                           ) M ON RP.PKID = M.FKPKID; ";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FKBigBrandPkid", bigBrandPKID);
                return db.ExecuteSelect<Tuhu.Service.Activity.Models.BigBrandRewardPoolModel>(cmd)?.FirstOrDefault();
            }
        }

        /// <summary>
        /// 添加实物奖励记录
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool AddBigBrandRealLog(IEnumerable<BigBrandRealLogModel> list)
        {
            string sql = @"INSERT  INTO Tuhu_log.dbo.BigBrandRealLog
        ( CreateDateTime ,
          LastUpdateDateTime ,
          UserId ,
          Prize ,
          FKBigBrandPoolID ,
          FKBigBrandID,
          Tip
        )
VALUES  ( @CreateDateTime ,
          @LastUpdateDateTime ,
          @UserId ,
          @Prize ,
          @FKBigBrandPoolID ,
          @FKBigBrandID,
          @Tip
        );";

            using (var db = DbHelper.CreateLogDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var item in list)
                    {
                        SqlCommand cmd = new SqlCommand(sql);
                        cmd.Parameters.AddWithValue("@CreateDateTime",item.CreateDateTime);
                        cmd.Parameters.AddWithValue("@LastUpdateDateTime", item.LastUpdateDateTime);
                        cmd.Parameters.AddWithValue("@UserId",item.UserId);
                        cmd.Parameters.AddWithValue("@Prize", item.Prize);
                        cmd.Parameters.AddWithValue("@FKBigBrandPoolID", item.FKBigBrandPoolID);
                        cmd.Parameters.AddWithValue("@FKBigBrandID",item.FKBigBrandID);
                        cmd.Parameters.AddWithValue("@Tip",item.Tip);
                        db.ExecuteNonQuery(cmd);
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
        /// 更新实物抽奖记录的地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="userId"></param>
        /// <param name="tip"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateBigBrandRealLog(string address, Guid userId, Guid tip,string userName,string phone)
        {
            string sql = @" UPDATE Tuhu_log.dbo.BigBrandRealLog SET [Address]=@Address ,UserName=@UserName ,Phone=@Phone WHERE UserId=@UserId AND Tip=@Tip";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@userId",userId);
                cmd.Parameters.AddWithValue("@tip",tip);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@Phone", phone);
                await db.ExecuteNonQueryAsync(cmd);
                return true;
            }
        }

        /// <summary>
        /// 查询没有填写地址的实物抽奖
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task< IEnumerable<BigBrandRealLogModel>> IsNULLBigBrandRealByAddress(Guid userId,int bigBrandPKID)
        {
            string sql = " SELECT * FROM Tuhu_log.dbo.BigBrandRealLog  WITH (NOLOCK) WHERE UserId=@UserId and FKBigBrandID=@FKBigBrandID AND ([Address] is null or [Address] = '' ) ";
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FKBigBrandID", bigBrandPKID);
                return (await db.ExecuteSelectAsync<BigBrandRealLogModel>(cmd))?.ToList();
            }
        }


        public static async Task<BigBrandAnsQuesModel> GetAnsQuesEntity(int fkPKID)
        {
            string sql = @"SELECT * FROM Configuration.dbo.BigBrandAnsQues WITH (NOLOCK) WHERE BigBrandPKID=@BigBrandPKID";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                var result = await db.ExecuteSelectAsync<BigBrandAnsQuesModel>(sql, System.Data.CommandType.Text, new SqlParameter("@BigBrandPKID", fkPKID));
                return result?.ToList()?.FirstOrDefault();
            }
        }



         /// <summary>
        /// 获取 不同页面 配置 
        /// </summary>
        /// <param name="FKPKID"></param>
        /// <param name="ActivityType"></param>
        /// <returns></returns>
        public static async Task<BigBrandPageConfigModel> GetBigBrandPageConfigModel(int FKPKID, int ActivityType)
        {
            string sql = @"SELECT [PKID]
                      ,[CreateDateTime]
                      ,[LastUpdateDateTime]
                      ,[FKPKID]
                      ,[IsShare]
                      ,[ActivityType]
                      ,[HomeBgImgUri]
                      ,[HomeBgImgUri2]
                      ,[ResultImgUri]
                      ,[DrawMachineImgUri]
                      ,[MarqueeLampIsOn]
                      FROM  Configuration.[dbo].[BigBrandPageConfig] with (nolock)
                      WHERE FKPKID=@FKPKID AND ActivityType=@ActivityType";
            using (var db = DbHelper.CreateDbHelper(true))
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("FKPKID", FKPKID);
                cmd.Parameters.AddWithValue("ActivityType", ActivityType);
                var result = await db.ExecuteSelectAsync<BigBrandPageConfigModel>(cmd);
                return result?.ToList()?.FirstOrDefault() ?? new BigBrandPageConfigModel();
            }
        }

        
    }
}
