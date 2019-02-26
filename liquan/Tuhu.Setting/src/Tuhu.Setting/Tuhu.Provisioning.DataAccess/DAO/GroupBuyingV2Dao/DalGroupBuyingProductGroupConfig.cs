using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Extensions;
using Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Response;

namespace Tuhu.Provisioning.DataAccess.DAO.GroupBuyingV2Dao
{
    public class DalGroupBuyingProductGroupConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="searchType">关键字搜索方式：1.创建人 3.商品名称 4.商品PID 5.groupId </param>
        /// <param name="groupCategory">拼团类别：-1.全部 0.普通拼团；1.拼团抽奖；2.优惠券拼团</param>
        /// <param name="groupType">团类型:-1.全部 0.普通团；1.新人团；2.团长特价</param>
        /// <param name="groupLabel"></param>
        /// <param name="isFinishGroup">拼团是否结束-1.全部 1.进行中 2.已结束 </param>
        /// <returns></returns>
        public static async Task<DataTable> SearchGroupBuyingSettingByPage(int pageIndex, int pageSize, string keyWord,
            int searchType, int groupCategory, int groupType, string groupLabel, int isFinishGroup)
        {
            string condition = string.Empty;

            #region searchType
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                if (searchType == 1 || searchType == 2) //创建人，时间
                {
                    condition = searchType == 1
                      ? $" and GBPGC.Creator like '%{keyWord}%' AND GBPGC.IsDelete=0"
                      : $" and GBPGC.BeginTime>{keyWord.Split('-').FirstOrDefault()} AND GBPGC.EndTime<{keyWord.Split('-').LastOrDefault()} AND GBPGC.IsDelete=0";

                }
                else if (searchType == 3 || searchType == 4) //商品名称，商品pid
                {
                    condition = searchType == 3
                      ? $" and GBPC.ProductName like N'%{keyWord}%' AND GBPGC.IsDelete=0"
                      : $" and GBPC.PID like '%{keyWord}%' AND GBPGC.IsDelete=0";
                }
                else if (searchType == 5)
                {
                    condition = $" and GBPGC.ProductGroupId='{keyWord}' ";
                }
                else
                {
                    condition = " and GBPGC.IsDelete=0";
                }
            }
            #endregion

            using (var cmd = new SqlCommand())
            {
                #region 条件筛选
                StringBuilder strw = new StringBuilder("");
                if (groupCategory >= 0)
                {
                    strw.Append(" and GroupCategory=@GroupCategory ");
                    cmd.Parameters.AddWithValue("@GroupCategory", groupCategory);
                }
                if (groupType >= 0)
                {
                    strw.Append(" AND GroupType=@GroupType ");
                    cmd.Parameters.AddWithValue("@GroupType", groupType);
                }
                if (isFinishGroup > 0)
                {
                    if (isFinishGroup == 1)
                        strw.Append(" AND (BeginTime<=GETDATE() AND EndTime>=GETDATE() AND GBPGC.CurrentGroupCount<GBPGC.TotalGroupCount) ");
                    else if (isFinishGroup == 2)
                        strw.Append(" AND (GBPGC.CurrentGroupCount>=GBPGC.TotalGroupCount OR GETDATE()>GBPGC.EndTime) ");
                }
                if (!string.IsNullOrWhiteSpace(groupLabel))
                {
                    var someChars = groupLabel.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (someChars.Count > 0)
                    {
                        strw.Append($" AND Label in (");
                        for (int i = 0; i < someChars.Count; i++)
                        {
                            strw.Append($"@label{i},");
                            cmd.Parameters.AddWithValue($"@label{i}", someChars[i]);
                        }
                        strw.Remove(strw.Length - 1, 1);
                        strw.Append(")");
                    }
                }

                #endregion

                cmd.Parameters.AddWithValue("@Begin", (pageIndex - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                string sql = $@"
select GBPGC.[Sequence],
       GBPGC.[Image],
       GBPGC.[Label],
       GBPGC.ProductGroupId,
       GroupType,
       BeginTime,
       EndTime,
       GBPGC.Creator,
       GBPGC.ShareId,
       GBPGC.MemberCount,
       GBPGC.TotalGroupCount,
       GBPGC.CurrentGroupCount,
       GBPGC.LastUpdateDateTime,
       GBPC.ProductName,
       GBPC.PID,
       GBPC.FinalPrice,
       GBPGC.IsShow
from [Configuration].[dbo].[GroupBuyingProductGroupConfig] as GBPGC with (nolock)
    inner join [Configuration].[dbo].[GroupBuyingProductConfig] as GBPC
        on GBPGC.ProductGroupId = GBPC.ProductGroupId
           and GBPC.DisPlay = 1
           and GBPC.IsDelete = 0  
where 1=1 {condition} {strw.ToString()} 
order by GBPGC.LastUpdateDateTime desc offset @Begin rows fetch next @PageSize rows only;";
                cmd.CommandText = sql;

                return await DbHelper.ExecuteDataTableAsync(cmd);
            }
        }

        public static async Task<int> SearchGroupBuyingSettingCount(string keyWord, int searchType, int groupCategory, int groupType, string groupLabel, int isFinishGroup)
        {
            string condition = string.Empty;

            #region searchType
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                if (searchType == 1 || searchType == 2) //创建人，时间
                {
                    condition = searchType == 1
                      ? $" and GBPGC.Creator like '%{keyWord}%' AND GBPGC.IsDelete=0"
                      : $" and GBPGC.BeginTime>{keyWord.Split('-').FirstOrDefault()} AND GBPGC.EndTime<{keyWord.Split('-').LastOrDefault()} AND GBPGC.IsDelete=0";

                }
                else if (searchType == 3 || searchType == 4) //商品名称，商品pid
                {
                    condition = searchType == 3
                      ? $" and GBPC.ProductName like N'%{keyWord}%' AND GBPGC.IsDelete=0"
                      : $" and GBPC.PID like '%{keyWord}%' AND GBPGC.IsDelete=0";
                }
                else if (searchType == 5)
                {
                    condition = $" and GBPGC.ProductGroupId='{keyWord}'";
                }
                else
                {
                    condition = " and GBPGC.IsDelete=0";
                }
            }
            #endregion

            using (var cmd = new SqlCommand())
            {
                #region 条件筛选
                StringBuilder strw = new StringBuilder("");
                if (groupCategory >= 0)
                {
                    strw.Append(" and GroupCategory=@GroupCategory ");
                    cmd.Parameters.AddWithValue("@GroupCategory", groupCategory);
                }
                if (groupType >= 0)
                {
                    strw.Append(" AND GroupType=@GroupType ");
                    cmd.Parameters.AddWithValue("@GroupType", groupType);
                }
                if (isFinishGroup > 0)
                {
                    if (isFinishGroup == 1)
                        strw.Append(" AND (BeginTime<=GETDATE() AND EndTime>=GETDATE() AND GBPGC.CurrentGroupCount<GBPGC.TotalGroupCount) ");
                    else if (isFinishGroup == 2)
                        strw.Append(" AND (GBPGC.CurrentGroupCount>=GBPGC.TotalGroupCount OR GETDATE()>GBPGC.EndTime) ");
                }
                if (!string.IsNullOrWhiteSpace(groupLabel))
                {
                    var someChars = groupLabel.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (someChars.Count > 0)
                    {
                        strw.Append($" AND Label in (");
                        for (int i = 0; i < someChars.Count; i++)
                        {
                            strw.Append($"@label{i},");
                            cmd.Parameters.AddWithValue($"@label{i}", someChars[i]);
                        }
                        strw.Remove(strw.Length - 1, 1);
                        strw.Append(")");
                    }
                }

                #endregion

                string sql = $@" Select COUNT(1)
From [Configuration].[dbo].[GroupBuyingProductGroupConfig] AS GBPGC With(nolock)
inner join [Configuration].[dbo].[GroupBuyingProductConfig] AS GBPC
ON GBPGC.ProductGroupId=GBPC.ProductGroupId and GBPC.DisPlay=1 and GBPC.IsDelete=0 
 where 1=1 {condition} {strw.ToString()} ";
                cmd.CommandText = sql;

                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(cmd));
            }
        }

        public static async Task<DataTable> GetProductsByGroupBuyingId(string groupBuyingId)
        {
            const string sql = @"
select ProductName,
       PID,
       ProductGroupId,
       OriginalPrice,
       FinalPrice,
       SpecialPrice,
       DisPlay,
       UseCoupon,
       IsShow,
       UpperLimitPerOrder,
       BuyLimitCount
from Configuration..GroupBuyingProductConfig with (nolock)
where [ProductGroupId] = @ProductGroupId
      and IsDelete = 0;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductGroupId", groupBuyingId);
                return await DbHelper.ExecuteDataTableAsync(cmd);
            }
        }

        public static async Task<bool> DeleteProductsByGroupBuyingId(string groupBuyingId)
        {
            const string sqlGroupConfig = @"Update [Configuration].[dbo].[GroupBuyingProductGroupConfig] with(RowLock)
SET IsDelete=1,LastUpdateDateTime=GetDate() 
Where [ProductGroupId]=@ProductGroupId";
            const string sqlProductConfig = @"Update [Configuration].[dbo].[GroupBuyingProductConfig] with(RowLock)
SET IsDelete=1,LastUpdateDateTime=GetDate() 
Where [ProductGroupId]=@ProductGroupId";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                db.BeginTransaction();
                using (var cmd = new SqlCommand())
                {
                    cmd.CommandText = sqlGroupConfig;
                    cmd.Parameters.AddWithValue("@ProductGroupId", groupBuyingId);
                    if ((await db.ExecuteNonQueryAsync(cmd)) > 0)
                    {
                        cmd.CommandText = sqlProductConfig;
                        if ((await db.ExecuteNonQueryAsync(cmd)) > 0)
                        {
                            db.Commit();
                            return true;
                        }
                    }

                    db.Rollback();
                    return false;
                }
            }
        }

        public static async Task<bool> AddGroupBuyingSetting(GroupBuyingProductGroupConfigEntity GroupConfig,
            List<GroupBuyingProductConfigEntity> ProductConfig)
        {
            #region sql_InsertProductConfig

            const string sql_InsertProductConfig = @"
insert into [Configuration].[dbo].[GroupBuyingProductConfig]
(
    [ProductName],
    [SimpleProductName],
    [PID],
    [ProductGroupId],
    [OriginalPrice],
    [FinalPrice],
    [SpecialPrice],
    [DisPlay],
    [Creator],
    [IsDelete],
    [CreateDateTime],
    [LastUpdateDateTime],
    UseCoupon,
    IsShow,
    UpperLimitPerOrder,
    BuyLimitCount
)
values
(@ProductName, @SimpleProductName, @PID, @ProductGroupId, @OriginalPrice, @FinalPrice, @SpecialPrice, @DisPlay,
 @Creator, 0, GETDATE(), GETDATE(), @useCoupon, @IsShow, @upperLimitPerOrder, @buyLimitCount);";

            #endregion

            #region sql_InsertGroupConfig

            const string sql_InsertGroupConfig = @"
insert into Configuration..GroupBuyingProductGroupConfig
(
    [ProductGroupId],
    [TotalGroupCount],
    [CurrentGroupCount],
    [MemberCount],
    [GroupType],
    [Sequence],
    [Image],
    [ShareId],
    [BeginTime],
    [EndTime],
    [Creator],
    [Label],
    [IsDelete],
    [SpecialUser],
    [CreateDateTime],
    [LastUpdateDateTime],
    GroupCategory,
    GroupDescription,
    IsShow,
    ApplyCoupon,
    ShareImage,
    Channel
)
values
(@ProductGroupId, @TotalGroupCount, @CurrentGroupCount, @MemberCount, @GroupType, @Sequence, @Image, @ShareId,
 @BeginTime, @EndTime, @Creator, @Label, 0, @specialusertag, GETDATE(), GETDATE(), @groupCategory, @groupDescription,
 @isShow, @applyCoupon, @shareImage, @channel);";

            #endregion

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    db.BeginTransaction();
                    using (var cmd = new SqlCommand())
                    {
                        #region sql_InsertGroupConfig

                        cmd.CommandText = sql_InsertGroupConfig;
                        cmd.Parameters.AddWithValue("@ProductGroupId", GroupConfig.ProductGroupId);
                        cmd.Parameters.AddWithValue("@TotalGroupCount", GroupConfig.TotalGroupCount);
                        cmd.Parameters.AddWithValue("@CurrentGroupCount", GroupConfig.CurrentGroupCount);
                        cmd.Parameters.AddWithValue("@MemberCount", GroupConfig.MemberCount);
                        cmd.Parameters.AddWithValue("@GroupType", GroupConfig.GroupType);
                        cmd.Parameters.AddWithValue("@Sequence", GroupConfig.Sequence);
                        cmd.Parameters.AddWithValue("@Image", GroupConfig.Image);
                        cmd.Parameters.AddWithValue("@ShareId", GroupConfig.ShareId);
                        cmd.Parameters.AddWithValue("@BeginTime", GroupConfig.BeginTime);
                        cmd.Parameters.AddWithValue("@EndTime", GroupConfig.EndTime);
                        cmd.Parameters.AddWithValue("@Creator", GroupConfig.Creator);
                        cmd.Parameters.AddWithValue("@Label", GroupConfig.Label);
                        cmd.Parameters.AddWithValue("@specialusertag", GroupConfig.SpecialUserTag);
                        cmd.Parameters.AddWithValue("@isShow", GroupConfig.IsShow);
                        cmd.Parameters.AddWithValue("@groupCategory", GroupConfig.GroupCategory);
                        cmd.Parameters.AddWithValue("@groupDescription", GroupConfig.GroupDescription);
                        cmd.Parameters.AddWithValue("@applyCoupon", GroupConfig.ApplyCoupon);
                        cmd.Parameters.AddWithValue("@shareImage", GroupConfig.ShareImage);
                        cmd.Parameters.AddWithValue("@channel", GroupConfig.Channel);
                        #endregion

                        if ((await db.ExecuteNonQueryAsync(cmd)) > 0)
                        {
                            var commit = true;
                            foreach (var item in ProductConfig)
                            {
                                cmd.Parameters.Clear();

                                #region sql_InsertProductConfig

                                cmd.CommandText = sql_InsertProductConfig;
                                cmd.Parameters.AddWithValue("@ProductName", item.ProductName);
                                cmd.Parameters.AddWithValue("@SimpleProductName", item.SimpleProductName);
                                cmd.Parameters.AddWithValue("@PID", item.PID);
                                cmd.Parameters.AddWithValue("@ProductGroupId", item.ProductGroupId);
                                cmd.Parameters.AddWithValue("@OriginalPrice", item.OriginalPrice);
                                cmd.Parameters.AddWithValue("@FinalPrice", item.FinalPrice);
                                cmd.Parameters.AddWithValue("@SpecialPrice", item.SpecialPrice);
                                cmd.Parameters.AddWithValue("@DisPlay", item.DisPlay);
                                cmd.Parameters.AddWithValue("@Creator", item.Creator);
                                cmd.Parameters.AddWithValue("@useCoupon", item.UseCoupon);
                                cmd.Parameters.AddWithValue("@upperLimitPerOrder", item.UpperLimitPerOrder);
                                cmd.Parameters.AddWithValue("@IsShow", item.IsShow);
                                cmd.Parameters.AddWithValue("@buyLimitCount", item.BuyLimitCount);
                                if ((await db.ExecuteNonQueryAsync(cmd)) <= 0)
                                {
                                    db.Rollback();
                                    commit = false;
                                    break;
                                }

                                #endregion
                            }

                            if (commit)
                                db.Commit();
                            return commit;
                        }

                        db.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    return false;
                }

            }
        }


        public static async Task<bool> UpdateGroupBuyingSetting(GroupBuyingProductGroupConfigEntity GroupConfig)
        {
            const string sqlStr = @"
update Configuration..GroupBuyingProductGroupConfig with (rowlock)
set TotalGroupCount = ISNULL(@TotalGroupCount, TotalGroupCount),
    CurrentGroupCount = ISNULL(@CurrentGroupCount, CurrentGroupCount),
    Sequence = ISNULL(@Sequence, Sequence),
    Image = ISNULL(@Image, Image),
    ShareId = ISNULL(@ShareId, ShareId),
    BeginTime = ISNULL(@BeginTime, BeginTime),
    EndTime = ISNULL(@EndTime, EndTime),
    Label = ISNULL(@Label, Label),
    LastUpdateDateTime = GETDATE(),
    SpecialUser = @specialusertag,
    IsShow = @isShow,
    GroupCategory = @groupCategory,
    GroupDescription = @groupDescription,
    ApplyCoupon = @applyCoupon,
    ShareImage = @shareImage,
    Channel = @channel
where ProductGroupId = @ProductGroupId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@TotalGroupCount", GroupConfig.TotalGroupCount);
                cmd.Parameters.AddWithValue("@CurrentGroupCount", GroupConfig.CurrentGroupCount);
                cmd.Parameters.AddWithValue("@Sequence", GroupConfig.Sequence);
                cmd.Parameters.AddWithValue("@Image", GroupConfig.Image);
                cmd.Parameters.AddWithValue("@ShareId", GroupConfig.ShareId);
                cmd.Parameters.AddWithValue("@BeginTime", GroupConfig.BeginTime);
                cmd.Parameters.AddWithValue("@EndTime", GroupConfig.EndTime);
                cmd.Parameters.AddWithValue("@Label", GroupConfig.Label);
                cmd.Parameters.AddWithValue("@specialusertag", GroupConfig.SpecialUserTag);
                cmd.Parameters.AddWithValue("@isShow", GroupConfig.IsShow);
                cmd.Parameters.AddWithValue("@ProductGroupId", GroupConfig.ProductGroupId);
                cmd.Parameters.AddWithValue("@groupDescription", GroupConfig.GroupDescription);
                cmd.Parameters.AddWithValue("@groupCategory", GroupConfig.GroupCategory);
                cmd.Parameters.AddWithValue("@applyCoupon", GroupConfig.ApplyCoupon);
                cmd.Parameters.AddWithValue("@shareImage", GroupConfig.ShareImage);
                cmd.Parameters.AddWithValue("@channel", GroupConfig.Channel);
                return await DbHelper.ExecuteNonQueryAsync(cmd) > 0;

            }
        }

        public static async Task<bool> UpdateGroupBuyingProductConfig(GroupBuyingProductConfigEntity productConfigEntity)
        {
            const string sqlStr = @"
update Configuration..GroupBuyingProductConfig with (rowlock)
set ProductName = @productName,
    OriginalPrice = @originPrice,
    FinalPrice = @finalPrice,
    SpecialPrice = @specialPrice,
    LastUpdateDateTime = GETDATE(),
    UseCoupon = @useCoupon,
    UpperLimitPerOrder = @upperLimitPerOrder,
    IsShow = @IsShow,
    BuyLimitCount = @buyLimitCount
where PID = @pid
      and ProductGroupId = @productGroupId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productName", productConfigEntity.ProductName);
                cmd.Parameters.AddWithValue("@originPrice", productConfigEntity.OriginalPrice);
                cmd.Parameters.AddWithValue("@finalPrice", productConfigEntity.FinalPrice);
                cmd.Parameters.AddWithValue("@specialPrice", productConfigEntity.SpecialPrice);
                cmd.Parameters.AddWithValue("@pid", productConfigEntity.PID);
                cmd.Parameters.AddWithValue("@productGroupId", productConfigEntity.ProductGroupId);
                cmd.Parameters.AddWithValue("@useCoupon", productConfigEntity.UseCoupon);
                cmd.Parameters.AddWithValue("@upperLimitPerOrder", productConfigEntity.UpperLimitPerOrder);
                cmd.Parameters.AddWithValue("@IsShow", productConfigEntity.IsShow);
                cmd.Parameters.AddWithValue("@buyLimitCount", productConfigEntity.BuyLimitCount);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<DataTable> GetProductsByPID(string pid, bool isLottery)
        {
            const string sql = @"select PID,
       DisplayName,
       cy_list_price as TuhuPrice
from Tuhu_productcatalog..vw_Products with (nolock)
where ProductCode = (   select top 1
                               [ProductCode]
                        from [Tuhu_productcatalog].[dbo].[vw_Products] with (nolock)
                        where PID = @pid)
      and (   @isLottery = 1
              or @isLottery = 0
                 and OnSale = 1
                 and stockout = 0);";
            const string sql2 = @"select PID,
       DisplayName,
       cy_list_price as TuhuPrice
from Tuhu_productcatalog..vw_Products with (nolock)
where PID = @pid
      and (   @isLottery = 1
              or @isLottery = 0
                 and OnSale = 1
                 and stockout = 0);";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@isLottery", isLottery);
                var result = await DbHelper.ExecuteDataTableAsync(cmd);
                if (result == null || result.Rows.Count <= 0)
                {
                    cmd.CommandText = sql2;
                    return await DbHelper.ExecuteDataTableAsync(cmd);
                }

                return result;
            }
        }

        public static async Task<DataTable> GetGroupBuyingSettingByid(string ProductGroupId)
        {
            const string sql = @"
select ShareId,
       MemberCount,
       TotalGroupCount,
       [Sequence],
       [Image],
       [Label],
       ProductGroupId,
       GroupType,
       BeginTime,
       EndTime,
       Creator,
       CurrentGroupCount,
       LastUpdateDateTime,
       ProductGroupId,
       SpecialUser,
       IsShow,
       GroupCategory,
       GroupDescription,
       ApplyCoupon,
       ShareImage,
       Channel
from Configuration..GroupBuyingProductGroupConfig with (nolock)
where ProductGroupId = @ProductGroupId;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductGroupId", ProductGroupId);
                return await DbHelper.ExecuteDataTableAsync(cmd);
            }
        }

        public static bool CheckIsExistProductGroupId(string productGroupId)
        {
            const string sql =
                @"Select top 1 1 From [Configuration].[dbo].[GroupBuyingProductGroupConfig]  WITH(NOLOCK) Where ProductGroupId=@ProductGroupId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId);
                return DbHelper.ExecuteScalar(cmd) != null;
            }
        }

        public static async Task<DataTable> GetGroupBuyingModifyLogByGroupId(string productGroupId)
        {
            const string sql = @"SELECT [Name],Title,[CreateDateTime]
  FROM [Configuration].[dbo].[GroupBuyingModifyLog] With(nolock)
  where [ProductGroupId]=@ProductGroupId Order By CreateDateTime DESC";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId);
                return await DbHelper.ExecuteDataTableAsync(cmd);
            }
        }

        public static async Task<bool> InsertGroupBuyingModifyLog(string productGroupId, string name, string title)
        {
            const string sql = @"Insert into [Configuration].[dbo].[GroupBuyingModifyLog]
  Values(@Name,@ProductGroupId,GETDATE(),GETDATE(),@Title)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static bool CheckPIdWithGroupType(List<string> pids, int groupType)
        {
            const string sql = @"SELECT TOP 1 1
FROM [Configuration].[dbo].[GroupBuyingProductConfig] AS GBPC WITH(NOLOCK)
LEFT JOIN [Configuration].[dbo].GroupBuyingProductGroupConfig AS GBPGC WITH(NOLOCK)
ON GBPC.ProductGroupId=GBPGC.ProductGroupId
WHERE GBPC.PID IN('{0}') AND GBPGC.IsDelete=0 AND GBPGC.GroupType=@GroupType";
            string executeSql = string.Format(sql, string.Join("','", pids));
            using (var cmd = new SqlCommand(executeSql))
            {
                cmd.Parameters.AddWithValue("@GroupType", groupType);
                return DbHelper.ExecuteScalar(cmd) != null;
            }
        }


        public static List<string> GetAllProductGroupId()
        {
            const string sqlStr = @"SELECT  DISTINCT
        ProductGroupId
FROM    Configuration..GroupBuyingProductGroupConfig WITH ( NOLOCK )
WHERE   IsDelete = 0
        AND BeginTime < GETDATE()
        AND EndTime > GETDATE();";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var dt = DbHelper.ExecuteDataTable(cmd);
                var result = new List<string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var value = dt.Rows[i].GetValue<string>("ProductGroupId");
                        if (!string.IsNullOrWhiteSpace(value))
                            result.Add(value);
                    }
                }

                return result;
            }
        }

        #region [拼团抽奖]

        public static List<LotteryGroupInfo> GetLotteryGroup(string productGroupId, bool isActivity, int pageIndex, int pageSize)
        {
            const string sqlStr = @"
select T.ProductGroupId,
       T.GroupType,
       T.GroupCategory,
       S.ProductName,
       S.FinalPrice,
       T.BeginTime,
       T.EndTime,
       T.GroupDescription
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where T.IsDelete = 0
      and T.GroupCategory = 1
      and S.DisPlay = 1
      and T.ProductGroupId = ISNULL(@productGroupId, T.ProductGroupId)
      and (   (   @isActivity = 1
                  and T.EndTime > DATEADD(day, -10, GETDATE()))
              or (   @isActivity = 0
                     and T.EndTime <= DATEADD(day, -10, GETDATE())))
order by T.EndTime desc offset @start rows fetch next @step rows only;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId",
                    string.IsNullOrWhiteSpace(productGroupId) ? null : productGroupId);
                cmd.Parameters.AddWithValue("@start", isActivity ? 0 : pageSize * (pageIndex - 1));
                cmd.Parameters.AddWithValue("@step", isActivity ? 10000 : pageSize);
                cmd.Parameters.AddWithValue("@isActivity", isActivity);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<LotteryGroupInfo>()?.ToList() ??
                       new List<LotteryGroupInfo>();
            }
        }
        public static int GetLotteryGroupCount()
        {
            const string sqlStr = @"select COUNT(1)
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where T.IsDelete = 0
      and T.GroupCategory = 1
      and S.DisPlay = 1
      and T.EndTime <= DATEADD(day, -10, GETDATE());";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var value = DbHelper.ExecuteScalar(cmd);
                int.TryParse(value?.ToString(), out var result);
                return result;
            }
        }
        public static int SetUserLotteryResult(string productGroupId, int level, int orderId, Guid userId)
        {
            const string sqlStr = @"
update Activity..tbl_GroupBuyingLotteryInfo with (rowlock)
set LotteryResult = @level
where ProductGroupId = @productGroupId
      and UserId = @userId
      and OrderId = @orderId";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@level", level);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static int SetOtherLotteryResult(string productGroupId, int level)
        {
            const string sqlStr = @"
update Activity..tbl_GroupBuyingLotteryInfo with (rowlock)
set LotteryResult = @level
where ProductGroupId = @productGroupId
      and LotteryResult = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@level", level);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }


        public static bool LotteryLog(string productGroupId, string operateType, string _operator, string remark)
        {
            const string sqlStr = @"
insert into Tuhu_log..GroupLotteryLog
(
    ProductGroupId,
    OperateType,
    Operator,
    Remark
)
values
(@productGroupId, @operateType, @operator, @remark);";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                    cmd.Parameters.AddWithValue("@operateType", operateType);
                    cmd.Parameters.AddWithValue("@operator", _operator);
                    cmd.Parameters.AddWithValue("@remark", remark);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }

        public static int GetLotteryUserCount(string productGroupId, int tag)
        {
            const string sqlStr = @"
select COUNT(1)
from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
where IsDelete = 0
      and ProductGroupId = @productGroupId
      and (   @level = -1
              and (   LotteryResult = 1
                      or LotteryResult = 2)
              or @level <> 0
                 and @level = LotteryResult);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@level", tag);
                var result = DbHelper.ExecuteScalar(cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

        public static bool AddLotteryCoupon(string productGroupId, Guid couponId, string creator)
        {
            const string sqlStr = @"
insert into Configuration..GroupBuyingCouponConfig
(
    ProductGroupId,
    CouponId,
    Creator,
    IsDelete
)
values
(@productGroupId, @couponId, @creator, 0);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@couponId", couponId);
                cmd.Parameters.AddWithValue("@creator", creator);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool DeleteLotteryCoupon(string productGroupId, Guid couponId)
        {
            const string sqlStr = @"
update Configuration..GroupBuyingCouponConfig with (rowlock)
set IsDelete = 1,
    LastUpdateDateTime = GETDATE()
where ProductGroupId = @productGroupId
      and CouponId = @couponId";
            using (var cmd = new SqlCommand((sqlStr)))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@couponId", couponId);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static List<Tuple<Guid, string>> GetLotteryCouponList(string productGroupId)
        {
            const string sqlStr = @"
select CouponId,
       Creator
from Configuration..GroupBuyingCouponConfig with (nolock)
where ProductGroupId = @productGroupId
      and IsDelete = 0";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                var data = DbHelper.ExecuteDataTable(cmd);
                var result = new List<Tuple<Guid, string>>();
                if (data == null || data.Rows.Count < 1) return result;
                for (var i = 0; i < data.Rows.Count; i++)
                {
                    var couponId = data.Rows[i].GetValue<Guid>("CouponId");
                    var creator = data.Rows[i].GetValue<string>("Creator");
                    if (couponId != Guid.Empty) result.Add(Tuple.Create(couponId, creator));
                }

                return result;
            }
        }

        public static int LotteryInfoCount(string productGroupId, int orderId, int lotteryResult)
        {
            const string sqlStr = @"
select COUNT(1)
from Activity..tbl_GroupBuyingLotteryInfo as T with (nolock)
where T.ProductGroupId = @productGroupId
      and (   @orderId = 0
              or @orderId = T.OrderId)
      and (   @lottery = 0
              or @lottery = -1
                 and T.LotteryResult = 0
              or @lottery <> 0
                 and @lottery = T.LotteryResult);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@lottery", lotteryResult);
                var result = DbHelper.ExecuteScalar(cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

        public static List<LotteryUserModel> LotteryInfoList(string productGroupId, int orderId, int lotteryResult,
            int pageIndex, int pageSize)
        {
            const string sqlStr = @"
select T.ProductGroupId,
       T.OrderId,
       T.UserId,
       S.UserTel as UserPhone,
       T.LotteryResult,
       case
           when S.Status = N'7Canceled' then
               N'已取消'
           when S.Status = N'2shipped' then
               N'已完成'
           else
               N'未完成'
       end as OrderStatus
from Activity..tbl_GroupBuyingLotteryInfo as T with (nolock)
    left join Gungnir..tbl_Order as S with (nolock)
        on T.OrderId = S.PKID
where T.ProductGroupId = @productGroupId
      and (   @orderId = 0
              or @orderId = T.OrderId)
      and (   @lottery = 0
              or @lottery = -1
                 and T.LotteryResult = 0
              or @lottery <> 0
                 and @lottery = T.LotteryResult)
order by T.LotteryResult,
         T.PKID offset @start rows fetch next @step rows only;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@lottery", lotteryResult);
                cmd.Parameters.AddWithValue("@start", (pageIndex - 1) * pageSize);
                cmd.Parameters.AddWithValue("@step", pageSize);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<LotteryUserModel>()?.ToList() ??
                       new List<LotteryUserModel>();
            }
        }


        public static List<LotteryLogInfo> GetLotteryLog(string productGroupId, string type, int size)
        {
            const string sqlStr = @"
select top (@count)
       CreateDateTime,
       OperateType,
       Operator,
       Remark
from Tuhu_log..GroupLotteryLog with (nolock)
where OperateType = @type
      and Operator <> N'System'
      and ProductGroupId = @productGroupId
order by CreateDateTime desc;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@count", size);
                    return dbHelper.ExecuteDataTable(cmd)?.ConvertTo<LotteryLogInfo>()?.ToList() ??
                           new List<LotteryLogInfo>();
                }
            }
        }

        public static List<LotteryUserInfo> GetUserInfoList(string productGroupId, int tag, int maxPkid, int step, bool? awardResult)
        {
            const string sqlStr = @"
select top (@step)
       PKID,
       OrderId,
       UserId,
       ProductGroupId
from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
where ProductGroupId = @productGroupId
      and (   @tag = -1
              and LotteryResult <> 0
              or @tag <> 0
                 and @tag = LotteryResult)
      and (   @awardResult is null
              or @awardResult = AwardResult)
      and PKID > @maxpkid
      and IsDelete = 0
order by PKID;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@step", step);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@tag", tag);
                cmd.Parameters.AddWithValue("@maxpkid", maxPkid);
                cmd.Parameters.AddWithValue("@awardResult", awardResult);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<LotteryUserInfo>()?.ToList() ??
                       new List<LotteryUserInfo>();
            }
        }

        public static List<LotteryUserInfo> GetUserInfoListForPush(string productGroupId, int tag, int maxOrderId, int step, bool? awardResult)
        {
            const string sqlStr = @"
select top (@step)
       T.OrderId,
       T.UserId,
       T.ProductGroupId
from
(   select MAX(OrderId) as OrderId,
           MAX(AwardResult) as AwardResult,
           UserId,
           ProductGroupId
    from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
    where ProductGroupId = @productGroupId
          and (   @tag = -1
                  or @tag <> 0
                     and @tag = LotteryResult)
          and IsDelete = 0
    group by UserId,
             ProductGroupId) as T
where T.OrderId > @maxorderId
      and (   @awardResult is null
              or @awardResult = AwardResult)
order by T.OrderId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@step", step);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@tag", tag);
                cmd.Parameters.AddWithValue("@maxorderId", maxOrderId);
                cmd.Parameters.AddWithValue("@awardResult", awardResult);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<LotteryUserInfo>()?.ToList() ??
       new List<LotteryUserInfo>();
            }
        }


        public static List<LotteryUserInfo> GetUserInfo(int orderId, string productGroupId)
        {
            const string sqlStr = @"
select top 1
       PKID,
       OrderId,
       UserId,
       ProductGroupId
from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
where IsDelete = 0
      and OrderId = @orderId
      and ProductGroupId = @productGroupId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<LotteryUserInfo>()?.ToList() ??
                       new List<LotteryUserInfo>();
            }
        }


        public static List<GroupBuyingExportInfo> GetGroupBuyingExportInfo()
        {
            const string sqlStr = @"
select T.ProductGroupId,
       T.GroupType,
       T.BeginTime,
       T.EndTime,
       T.IsShow,
       T.MemberCount,
       T.TotalGroupCount,
       T.CurrentGroupCount,
       T.GroupCategory,
       T.Creator,
       S.PID,
       S.ProductName,
       S.OriginalPrice,
       S.FinalPrice,
       S.SpecialPrice,
       T.Label
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on S.ProductGroupId = T.ProductGroupId
where S.IsDelete = 0
      and S.DisPlay = 1;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<GroupBuyingExportInfo>()?.ToList() ??
                       new List<GroupBuyingExportInfo>();
            }
        }

        #endregion


        //        public static List<string> GetGroupBuyingStockInfo(string productGroupId)
        //        {
        //            const string sqlStr = @"
        //select PID
        //from Configuration..GroupBuyingProductConfig with (nolock)
        //where IsDelete = 0
        //      and ProductGroupId = @productGroupId;";
        //            using (var cmd = new SqlCommand(sqlStr))
        //            {
        //                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
        //                var dt = DbHelper.ExecuteDataTable(cmd);
        //                var result = new List<string>();
        //                if (dt == null || dt.Rows.Count < 1) return result;
        //                for (var i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    var item = dt.Rows[i].GetValue<string>("PID");
        //                    if (!string.IsNullOrWhiteSpace(item)) result.Add(item);
        //                }

        //                return result;
        //            }
        //        }

        public static List<ProductStockInfo> GetGroupBuyingStockInfo(List<string> pids)
        {
            string sqlStr = @"
select PID,
       WAREHOUSEID,
       TotalAvailableStockQuantity,
       StockCost,
       CaigouZaitu
from Tuhu_bi..dw_ProductAvaibleStockQuantity with (nolock)
where PID  in (N'" + string.Join("',N'", pids) + @"')
      and (   WAREHOUSEID = 8598
              or WAREHOUSEID = 7295);";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    return dbHelper.ExecuteDataTable(cmd)?.ConvertTo<ProductStockInfo>()?.ToList() ??
                           new List<ProductStockInfo>();
                }
            }
        }

        #region 虚拟商品优惠券
        /// <summary>
        /// 查询虚拟商品列表
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="pid">商品pid</param>
        /// <returns></returns>
        public static IEnumerable<VirtualProductCouponConfigModel> SelectProductCouponConfig(SqlConnection conn, int pageIndex, int pageSize, string pid)
        {
            return conn.Query<VirtualProductCouponConfigModel>($@"SELECT [T].[PKID],
       [T].[PID],
       [T].[CouponCount],
       [T].[CreateDateTime],
       [T].[CreatedBy],
       [T].[LastUpdateDateTime],
       [T].[UpdatedBy]
FROM
(
    SELECT [PKID],
           [PID],
           COUNT(1) OVER (PARTITION BY [PID]) AS CouponCount,
           ROW_NUMBER() OVER (PARTITION BY [PID] ORDER BY [CreateDateTime]) AS RowNum,
           [CreateDateTime],
           [CreatedBy],
           [LastUpdateDateTime],
           [UpdatedBy]
	FROM [dbo].[VirtualProductCouponConfig] WITH(NOLOCK)
	WHERE [IsDeleted] = 0 {(string.IsNullOrWhiteSpace(pid) ? "" : "AND [PID] = @pid")}
) AS t
WHERE [t].[RowNum] = 1
ORDER BY [t].[PKID] DESC
OFFSET @offset ROWS
FETCH NEXT @size ROWS ONLY",
                new
                {
                    pid,
                    size = pageSize,
                    offset = (pageIndex - 1) * pageSize
                });
        }

        public static int SelectProductCouponConfigCount(SqlConnection conn, string pid)
        {
            string sql = $@"SELECT count(1)
                                FROM
                                (
                                    SELECT [PKID],
                                           [PID],
                                           COUNT(1) OVER (PARTITION BY [PID]) AS CouponCount,
                                           ROW_NUMBER() OVER (PARTITION BY [PID] ORDER BY [CreateDateTime]) AS RowNum,
                                           [CreateDateTime],
                                           [CreatedBy],
                                           [LastUpdateDateTime],
                                           [UpdatedBy]
	                                FROM [dbo].[VirtualProductCouponConfig] WITH(NOLOCK)
	                                WHERE [IsDeleted] = 0 {(string.IsNullOrWhiteSpace(pid) ? "" : "AND [PID] = @pid")}
                                ) AS t WHERE [t].[RowNum] = 1";
            return (int)conn.ExecuteScalar(sql, new { pid });
        }

        public static int DeleteVirtualProductConfig(SqlConnection conn, string pid, string updateBy)
        {
            return conn.Execute(@"UPDATE [dbo].[VirtualProductCouponConfig] WITH (ROWLOCK)
SET [IsDeleted] = 1,
    [LastUpdateDateTime] = GETDATE(),
    [UpdatedBy] = @updateBy
WHERE [IsDeleted]=0 AND [PID] = @pid;", new { pid, updateBy });
        }

        /// <summary>
        /// 根据PID查询单个优惠券详情
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="pid">pid</param>
        /// <returns></returns>
        public static IEnumerable<VirtualProductCouponConfigModel> FetchProductCouponConfig(SqlConnection conn,
            string pid)
        {
            return conn.Query<VirtualProductCouponConfigModel>(@"SELECT [PKID],
       [PID],
       [CouponId],
       [CouponRate],
       [CouponValue],
       [CreateDateTime],
       [CreatedBy],
       [LastUpdateDateTime],
       [UpdatedBy]
FROM [dbo].[VirtualProductCouponConfig] WITH (NOLOCK)
WHERE [IsDeleted] = 0
      AND [PID] = @pid;", new { pid });
        }

        public static IEnumerable<string> SelectProductCouponConfig(SqlConnection conn, IEnumerable<string> pids)
        {
            var sql = $@"SELECT DISTINCT [PID]
FROM [Configuration].[dbo].[VirtualProductCouponConfig] WITH(NOLOCK)
WHERE [IsDeleted] = 0 AND [PID] IN ({string.Join(",", pids.Select(p => $"N'{p.Replace("'", "''")}'"))})";
            return conn.Query<string>(sql);
        }

        /// <summary>
        /// 添加或更新虚拟商品配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="configDetails">配置详情</param>
        /// <param name="deletedConfigs">已删除配置</param>
        /// <param name="updateBy">更新人</param>
        /// <returns></returns>
        public static int AddOrUpdateProductCouponConfig(SqlConnection conn, IEnumerable<VirtualProductCouponConfigModel> configDetails,
            IEnumerable<VirtualProductCouponConfigModel> deletedConfigs, string updateBy)
        {
            using (var transction = conn.BeginTransaction())
            {
                foreach (var deletedConfig in deletedConfigs)
                {
                    deletedConfig.UpdatedBy = updateBy;
                    conn.Execute(@"UPDATE [dbo].[VirtualProductCouponConfig] WITH (ROWLOCK)
                                SET [IsDeleted] = 1,
                                [UpdatedBy] = @UpdatedBy,
                                [LastUpdateDateTime] = GETDATE()
                                WHERE [PKID] = @PKID;", deletedConfig, transction);
                }

                foreach (var configModel in configDetails)
                {
                    configModel.UpdatedBy = updateBy;
                    conn.Execute(configModel.PKID > 0 ?
                            @"UPDATE [dbo].[VirtualProductCouponConfig] WITH (ROWLOCK)
                            SET [CouponRate] = @CouponRate,
                            [CouponValue] = @CouponValue,
                            [CouponId] = @CouponId,
                            [UpdatedBy] = @UpdatedBy,
                            [LastUpdateDateTime] = GETDATE()
                            WHERE [PKID] = @PKID;" :
                            @"INSERT [dbo].[VirtualProductCouponConfig]
                            (
                                [PID],
                                [CouponId],
                                [CouponRate],
                                [CouponValue],
                                [IsDeleted],
                                [CreateDateTime],
                                [CreatedBy],
                                [LastUpdateDateTime],
                                [UpdatedBy]
                            )
                            VALUES
                            (   @PID,       -- PID - nvarchar(20)
                                @CouponId,      -- CouponId - uniqueidentifier
                                @CouponRate,         -- CouponRate - int
                                @CouponValue,  
                                0,      -- IsDeleted - bit
                                GETDATE(), -- CreateDateTime - datetime
                                @UpdatedBy,       -- CreatedBy - nvarchar(50)
                                GETDATE(), -- LastUpdateDateTime - datetime
                                @UpdatedBy        -- UpdatedBy - nvarchar(50)
                                )",
                        configModel, transction);
                }
                transction.Commit();
                return 1;
            }
        }
        #endregion

        #region 新框架团购
        /// <summary>
        /// 获取团购配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="filter"></param>
        /// <param name="activityStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<GroupBuyingProductGroupConfigEntity> GetGroupBuyingV2Config(SqlConnection conn,
            GroupBuyingProductGroupConfigEntity filter, string activityStatus, List<string> productGroupIds,
            int pageIndex, int pageSize)
        {
            const string sql = @"
            WITH    groupIds
                      AS ( SELECT   *
                           FROM     Configuration..SplitString(@ProductGroupIdStr, ',', 1)
                         )
            SELECT  GBPGC.PKID ,
                    GBPGC.Sequence ,
                    GBPGC.Image ,
                    GBPGC.ShareImage ,
                    GBPGC.GroupDescription ,
                    GBPGC.SpecialUser AS SpecialUserTag ,
                    GBPGC.Label ,
                    GBPGC.Channel ,
                    GBPGC.ProductGroupId ,
                    GBPGC.GroupType ,
		            GBPGC.GroupCategory,
                    GBPGC.BeginTime ,
                    GBPGC.EndTime ,
                    GBPGC.Creator ,
                    GBPGC.ShareId ,
                    GBPGC.MemberCount ,
                    GBPGC.TotalGroupCount ,
                    GBPGC.CurrentGroupCount ,
                    GBPGC.LastUpdateDateTime ,
                    GBPGC.IsShow ,
                    GBPGC.IsAutoFinish ,

                    COUNT(1) OVER ( ) AS Total
            FROM    Configuration.dbo.GroupBuyingProductGroupConfig AS GBPGC WITH ( NOLOCK )
            WHERE   GBPGC.IsDelete = 0
                    AND ( @IsShow = -1
                          OR GBPGC.IsShow = @IsShow
                        )
                    AND ( @Sequence = -1
                          OR ( @Sequence = 0 
                               AND GBPGC.Sequence = 0
                              )
                          OR ( @Sequence = 1 
                               AND GBPGC.Sequence <> 0
                              )
                        )
                    AND ( @Creator = ''
                          OR @Creator IS NULL
                          OR GBPGC.Creator LIKE N'%' + @Creator + '%'
                        )
                    AND ( @ProductGroupIdStr = ''
                          OR @ProductGroupIdStr IS NULL
                          OR EXISTS ( SELECT 1
                                     FROM   groupIds
                                     WHERE  GBPGC.ProductGroupId = groupIds.Item )
                        )
                    AND ( @Label = ''
                          OR @Label IS NULL
                          OR GBPGC.Label LIKE N'%' + @Label + '%'
                        )
                    AND ( @GroupType = -1
                          OR GBPGC.GroupType = @GroupType
                        )
                    AND ( @Channel = 'None'
                          OR GBPGC.Channel LIKE N'%' + @Channel + '%'
                        )
                    AND ( @GroupCategory = -1
                          OR GBPGC.GroupCategory = @GroupCategory
                        )
                    AND ( @AcitivtyStatus = 'All'
                          OR ( @AcitivtyStatus = 'Ongoing'
                               AND GBPGC.BeginTime <= GETDATE()
                               AND GETDATE() <= GBPGC.EndTime
                             )
                          OR ( @AcitivtyStatus = 'Ending'
                               AND GETDATE() > GBPGC.EndTime
                             )
                        )
                    AND ( @IsAutoFinish = -1
                          OR GBPGC.IsAutoFinish = @IsAutoFinish
                        )
            ORDER BY GBPGC.LastUpdateDateTime DESC
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                    ONLY;";
            return conn.Query<GroupBuyingProductGroupConfigEntity>(sql, new
            {
                Creator = filter.Creator,
                ProductGroupIdStr = string.Join(",", productGroupIds),
                Label = filter.Label,
                AcitivtyStatus = activityStatus,
                GroupType = filter.GroupType,
                GroupCategory = filter.GroupCategory,
                IsShow = filter.IsShowPage,
                Sequence = filter.Sequence,
                Channel = filter.Channel,
                filter.IsAutoFinish,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public static GroupBuyingProductGroupConfigEntity GetGroupBuyingV2ConfigByGroupId(SqlConnection conn, string productGroupId, bool ignoreDeleted = false)
        {
            string temp = ignoreDeleted ? ";" : " AND GBPGC.IsDelete = 0;";
            string sql = @"
            SELECT  GBPGC.PKID ,
                    GBPGC.Sequence ,
                    GBPGC.Image ,
                    GBPGC.ShareImage ,
                    GBPGC.GroupDescription ,
                    GBPGC.SpecialUser AS SpecialUserTag ,
                    GBPGC.Label ,
                    GBPGC.Channel ,
                    GBPGC.ProductGroupId ,
                    GBPGC.GroupType ,
		            GBPGC.GroupCategory,
                    GBPGC.BeginTime ,
                    GBPGC.EndTime ,
                    GBPGC.Creator ,
                    GBPGC.ShareId ,
                    GBPGC.MemberCount ,
                    GBPGC.TotalGroupCount ,
                    GBPGC.CurrentGroupCount ,
                    GBPGC.LastUpdateDateTime ,
                    GBPGC.IsShow,
                    GBPGC.IsAutoFinish

            FROM    Configuration.dbo.GroupBuyingProductGroupConfig AS GBPGC WITH ( NOLOCK )
            WHERE   GBPGC.ProductGroupId = @ProductGroupId" + temp;
            return conn.Query<GroupBuyingProductGroupConfigEntity>(sql, new
            {
                ProductGroupId = productGroupId
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public static List<GroupBuyingProductConfigEntity> GetGroupBuyingProductConfig(SqlConnection conn, string pid, string productName)
        {
            const string sql = @"
            SELECT DISTINCT
                    GBPC.ProductGroupId ,
                    COUNT(1) OVER ( ) AS Total
            FROM    Configuration..GroupBuyingProductConfig AS GBPC WITH ( NOLOCK )
            WHERE   GBPC.IsDelete = 0
                    AND ( @PID = ''
                          OR @PID IS NULL
                          OR GBPC.PID LIKE N'%' + @PID + '%'
                        )
                    AND ( @ProductName = ''
                          OR @ProductName IS NULL
                          OR GBPC.ProductName LIKE N'%' + @ProductName + '%'
                        )
            ORDER BY GBPC.ProductGroupId DESC";
            return conn.Query<GroupBuyingProductConfigEntity>(sql, new
            {
                PID = pid,
                ProductName = productName,
            }, commandType: CommandType.Text).ToList();
        }

        public static int IsExistProductGroupId(SqlConnection conn, string productGroupId)
        {
            const string sql =
                @"SELECT  COUNT(1)
                FROM    [Configuration].[dbo].[GroupBuyingProductGroupConfig] WITH ( NOLOCK )
                WHERE   ProductGroupId = @ProductGroupId;";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new { ProductGroupId = productGroupId }, commandType: CommandType.Text));
        }

        /// <summary>
        /// 通过GroupId获取团购信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ProductGroupId"></param>
        /// <returns></returns>
        public static List<GroupBuyingProductConfigEntity> GetGroupBuyingV2ProductConfigByGroupId(SqlConnection conn, List<string> productGroupIds)
        {
            const string sql = @"
            WITH    groupIds
                      AS ( SELECT   *
                           FROM     Configuration..SplitString(@ProductGroupIdStr, ',', 1)
                         )
           SELECT  GBPC.PKID ,
            GBPC.ProductName ,
            GBPC.PID ,
            GBPC.SimpleProductName ,
            GBPC.ProductGroupId ,
            GBPC.OriginalPrice ,
            GBPC.FinalPrice ,
            GBPC.SpecialPrice ,
            GBPC.DisPlay ,
            GBPC.UseCoupon ,
            GBPC.IsShow ,
            GBPC.UpperLimitPerOrder ,
            GBPC.BuyLimitCount ,
            GBPC.TotalStockCount ,
            GBPC.CurrentSoldCount ,
            GBPC.IsAutoStock ,
            GBPC.IsShowApp ,
            GBPC.IsActive
            
    FROM    Configuration..GroupBuyingProductConfig AS GBPC WITH ( NOLOCK )
    WHERE   GBPC.IsDelete = 0
            AND EXISTS ( SELECT 1
                         FROM   groupIds
                         WHERE  GBPC.ProductGroupId = groupIds.Item );";

            return conn.Query<GroupBuyingProductConfigEntity>(sql, new
            {
                ProductGroupIdStr = string.Join(",", productGroupIds)
            }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 添加团购商品信息    
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns>PKID</returns>
        public static int InsertGroupBuyingProductConfig(SqlConnection conn,
            GroupBuyingProductConfigEntity config)
        {
            const string sql = @"
            INSERT  INTO Configuration.dbo.GroupBuyingProductConfig
                    ( ProductName ,
                      SimpleProductName ,
                      PID ,
                      ProductGroupId ,
                      OriginalPrice ,
                      FinalPrice ,
                      SpecialPrice ,
                      DisPlay ,
                      Creator ,
                      IsDelete ,
                      CreateDateTime ,
                      LastUpdateDateTime ,
                      UseCoupon ,
                      IsShow ,
                      BuyLimitCount ,
                      UpperLimitPerOrder ,
                      TotalStockCount ,
                      IsShowApp ,
                      IsAutoStock ,
                      CurrentSoldCount                     
                    )
            OUTPUT Inserted.PKID
            VALUES  ( @ProductName ,
                      @SimpleProductName ,
                      @PID ,
                      @ProductGroupId ,
                      @OriginalPrice ,
                      @FinalPrice ,
                      @SpecialPrice ,
                      @DisPlay ,
                      @Creator ,
                      0 ,
                      GETDATE() ,
                      GETDATE() ,
                      @useCoupon ,
                      @IsShow ,
                      @BuyLimitCount ,
                      @upperLimitPerOrder ,
                      @TotalStockCount ,
                      @IsShowApp ,
                      @IsAutoStock ,
                      @CurrentSoldCount
                    );";
            return conn.Query<int>(sql, new
            {
                ProductName = config.ProductName,
                SimpleProductName = config.SimpleProductName,
                PID = config.PID,
                ProductGroupId = config.ProductGroupId,
                OriginalPrice = config.OriginalPrice,
                FinalPrice = config.FinalPrice,
                SpecialPrice = config.SpecialPrice,
                DisPlay = config.DisPlay,
                Creator = config.Creator,
                useCoupon = config.UseCoupon,
                upperLimitPerOrder = config.UpperLimitPerOrder,
                IsShow = config.IsShow,
                BuyLimitCount = config.BuyLimitCount,
                TotalStockCount = config.TotalStockCount,
                IsAutoStock = config.IsAutoStock,
                IsShowApp = config.IsShowApp,
                config.CurrentSoldCount
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        /// <summary>
        /// 更新团购商品配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="productConfig"></param>
        /// <returns></returns>
        public static int UpdateGroupBuyingProductConfig(SqlConnection conn,
            GroupBuyingProductConfigEntity productConfig)
        {
            const string sql = @"
            UPDATE  Configuration..GroupBuyingProductConfig WITH ( ROWLOCK )
            SET     ProductName = @ProductName ,
                    OriginalPrice = @OriginalPrice ,
                    FinalPrice = @FinalPrice ,
                    SpecialPrice = @SpecialPrice ,
                    LastUpdateDateTime = GETDATE() ,
                    UseCoupon = @UseCoupon ,
                    UpperLimitPerOrder = @UpperLimitPerOrder ,
                    BuyLimitCount = @BuyLimitCount ,
                    TotalStockCount = @TotalStockCount ,
                    IsAutoStock = @IsAutoStock ,
                    IsShowApp = @IsShowApp ,
                    DisPlay = @DisPlay ,
                    IsShow = @IsShow
            WHERE   PID = @PID
                    AND ProductGroupId = @ProductGroupId;";
            return conn.Execute(sql, new
            {
                ProductName = productConfig.ProductName,
                OriginalPrice = productConfig.OriginalPrice,
                FinalPrice = productConfig.FinalPrice,
                SpecialPrice = productConfig.SpecialPrice,
                PID = productConfig.PID,
                ProductGroupId = productConfig.ProductGroupId,
                UseCoupon = productConfig.UseCoupon,
                UpperLimitPerOrder = productConfig.UpperLimitPerOrder,
                BuyLimitCount = productConfig.BuyLimitCount,
                IsShow = productConfig.IsShow,
                DisPlay = productConfig.DisPlay,
                TotalStockCount = productConfig.TotalStockCount,
                IsAutoStock = productConfig.IsAutoStock,
                IsShowApp = productConfig.IsShowApp,
            }, commandType: CommandType.Text);
        }

        public static int DeleteGroupBuyingProductConfig(SqlConnection conn, string groupId)
        {
            const string sql = @"DELETE FROM Configuration..GroupBuyingProductConfig WHERE ProductGroupId=@ProductGroupId";
            return conn.Execute(sql, new
            {
                ProductGroupId = groupId
            }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 添加团购配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupConfig"></param>
        /// <returns></returns>
        public static int InsertGroupBuyingGroupConfig(SqlConnection conn,
            GroupBuyingProductGroupConfigEntity groupConfig)
        {
            const string sql = @"
            INSERT  INTO Configuration..GroupBuyingProductGroupConfig
                    ( ProductGroupId ,
                      TotalGroupCount ,
                      CurrentGroupCount ,
                      MemberCount ,
                      GroupType ,
                      Sequence ,
                      Image ,
                      ShareId ,
                      BeginTime ,
                      EndTime ,
                      Creator ,
                      Label ,
                      IsDelete ,
                      SpecialUser ,
                      CreateDateTime ,
                      LastUpdateDateTime ,
                      GroupCategory ,
                      GroupDescription ,
                      IsShow ,
                      ApplyCoupon ,
                      ShareImage ,
                      Channel ,
                      IsAutoFinish
                    )
            VALUES  ( @ProductGroupId ,
                      @TotalGroupCount ,
                      @CurrentGroupCount ,
                      @MemberCount ,
                      @GroupType ,
                      @Sequence ,
                      @Image ,
                      @ShareId ,
                      @BeginTime ,
                      @EndTime ,
                      @Creator ,
                      @Label ,
                      @IsDelete ,
                      @specialusertag ,
                      GETDATE() ,
                      GETDATE() ,
                      @groupCategory ,
                      @groupDescription ,
                      @isShow ,
                      @applyCoupon ,
                      @shareImage ,
                      @channel ,
                      @IsAutoFinish
                    );";
            return conn.Execute(sql, new
            {
                ProductGroupId = groupConfig.ProductGroupId,
                TotalGroupCount = groupConfig.TotalGroupCount,
                CurrentGroupCount = groupConfig.CurrentGroupCount,
                MemberCount = groupConfig.MemberCount,
                GroupType = groupConfig.GroupType,
                Sequence = groupConfig.Sequence,
                Image = groupConfig.Image ?? string.Empty,
                ShareId = groupConfig.ShareId ?? string.Empty,
                BeginTime = groupConfig.BeginTime,
                EndTime = groupConfig.EndTime,
                Creator = groupConfig.Creator,
                Label = groupConfig.Label,
                specialusertag = groupConfig.SpecialUserTag,
                isShow = groupConfig.IsShow,
                groupCategory = groupConfig.GroupCategory,
                groupDescription = groupConfig.GroupDescription,
                applyCoupon = groupConfig.ApplyCoupon,
                shareImage = groupConfig.ShareImage,
                channel = groupConfig.Channel,
                groupConfig.IsAutoFinish,
                IsDelete = groupConfig.IsDelete
            }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 更新团购配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupConfig"></param>
        /// <returns></returns>
        public static int UpdateGroupBuyingGroupConfig(SqlConnection conn, GroupBuyingProductGroupConfigEntity groupConfig)
        {
            const string sql = @"
            UPDATE  Configuration..GroupBuyingProductGroupConfig WITH ( ROWLOCK )
            SET     TotalGroupCount = ISNULL(@TotalGroupCount, TotalGroupCount) ,
                    CurrentGroupCount = ISNULL(@CurrentGroupCount, CurrentGroupCount) ,
                    Sequence = ISNULL(@Sequence, Sequence) ,
                    Image = ISNULL(@Image, Image) ,
                    ShareId = ISNULL(@ShareId, ShareId) ,
                    BeginTime = ISNULL(@BeginTime, BeginTime) ,
                    EndTime = ISNULL(@EndTime, EndTime) ,
                    Label = ISNULL(@Label, Label) ,
                    LastUpdateDateTime = GETDATE() ,
                    SpecialUser = @specialusertag ,
                    IsShow = @isShow ,
                    GroupCategory = @groupCategory ,
                    GroupDescription = @groupDescription ,
                    ApplyCoupon = @applyCoupon ,
                    ShareImage = @shareImage ,
                    Channel = @channel ,
                    IsAutoFinish = @IsAutoFinish

            WHERE   ProductGroupId = @ProductGroupId;";
            return conn.Execute(sql, new
            {
                TotalGroupCount = groupConfig.TotalGroupCount,
                CurrentGroupCount = groupConfig.CurrentGroupCount,
                Sequence = groupConfig.Sequence,
                Image = groupConfig.Image ?? string.Empty,
                ShareId = groupConfig.ShareId ?? string.Empty,
                BeginTime = groupConfig.BeginTime,
                EndTime = groupConfig.EndTime,
                Label = groupConfig.Label,
                specialusertag = groupConfig.SpecialUserTag,
                isShow = groupConfig.IsShow,
                ProductGroupId = groupConfig.ProductGroupId,
                groupDescription = groupConfig.GroupDescription,
                groupCategory = groupConfig.GroupCategory,
                applyCoupon = groupConfig.ApplyCoupon,
                shareImage = groupConfig.ShareImage,
                channel = groupConfig.Channel,
                groupConfig.IsAutoFinish
            }, commandType: CommandType.Text);
        }

        public static DataTable GetLogByGroupId(string productGroupId)
        {
            const string sql = @"
            SELECT  [Name] ,
                    Title ,
                    [CreateDateTime]
            FROM    [Configuration].[dbo].[GroupBuyingModifyLog] WITH ( NOLOCK )
            WHERE   [ProductGroupId] = @ProductGroupId
            ORDER BY CreateDateTime DESC;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId);
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        public static bool InsertGroupBuyingLog(SqlConnection conn, string productGroupId, string name, string title)
        {
            const string sql = @"
            INSERT  INTO Configuration..GroupBuyingModifyLog
                    ( Name ,
                      ProductGroupId ,
                      CreateDateTime ,
                      LastUpdateDateTime ,
                      Title
                    )
            VALUES  ( @Name ,
                      @ProductGroupId ,
                      GETDATE() ,
                      GETDATE() ,
                      @Title
                    );";
            return conn.Execute(sql, new
            {
                Title = title,
                Name = name,
                ProductGroupId = productGroupId
            }, commandType: CommandType.Text) > 0;
        }

        public static List<VW_ProductsModel> GetProductsByPIDAndIsLottery(SqlConnection conn, string pid, bool isLottery)
        {
            const string sql = @"
            SELECT  PID ,
                    DisplayName ,
                    cy_list_price 
            FROM    Tuhu_productcatalog..vw_Products WITH ( NOLOCK )
            WHERE   ProductCode = ( SELECT TOP 1
                                            [ProductCode]
                                    FROM    [Tuhu_productcatalog].[dbo].[vw_Products] WITH ( NOLOCK )
                                    WHERE   PID = @PID
                                  )
                    AND ( @isLottery = 1
                          OR @isLottery = 0
                          AND OnSale = 1
                          AND stockout = 0
                        );";
            const string sql2 = @"
            SELECT  PID ,
                    DisplayName ,
                    cy_list_price 
            FROM    Tuhu_productcatalog..vw_Products WITH ( NOLOCK )
            WHERE   PID = @PID
                    AND ( @isLottery = 1
                          OR @isLottery = 0
                          AND OnSale = 1
                          AND stockout = 0
                        );";
            var result = conn.Query<VW_ProductsModel>(sql, new { PID = pid, isLottery = isLottery }, commandType: CommandType.Text).ToList();
            if (result == null || !result.Any())
                result = conn.Query<VW_ProductsModel>(sql2, new { PID = pid, isLottery = isLottery }, commandType: CommandType.Text).ToList();
            return result;
        }

        public static List<ProductStockInfo> GetStockInfoByPIDs(List<string> pids)
        {
            string sqlStr = @"
              WITH  pids
                      AS ( SELECT   *
                           FROM     Tuhu_bi..SplitString(@PIDStr, ',', 1)
                         )
                SELECT  PID ,
                        WAREHOUSEID ,
                        TotalAvailableStockQuantity ,
                        StockCost ,
                        CaigouZaitu
                FROM    Tuhu_bi..dw_ProductAvaibleStockQuantity AS sq WITH ( NOLOCK )
                WHERE   sq.WAREHOUSEID IN ( 7295, 8598, 31860, 11410, 28790 )
                        AND EXISTS ( SELECT 1
                                     FROM   pids
                                     WHERE  pids.Item = sq.PID );";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_BI")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("PIDStr", string.Join(",", pids));
                    return dbHelper.ExecuteDataTable(cmd)?.ConvertTo<ProductStockInfo>()?.ToList() ??
                           new List<ProductStockInfo>();
                }
            }
        }

        public static int UpdateProductConfigIsShow(SqlConnection conn, string groupId, bool isShow)
        {
            const string sql = @"UPDATE Configuration..GroupBuyingProductGroupConfig SET IsShow=@IsShow ,LastUpdateDateTime=GETDATE() WHERE ProductGroupId=@ProductGroupId";
            return conn.Execute(sql, new { ProductGroupId = groupId, IsShow = isShow }, commandType: CommandType.Text);
        }

        public static int UpdateProductSimpleData(SqlConnection conn, string grouId, int sequence,
            DateTime beginTime, DateTime endTime, int totalGroupCount)
        {
            const string sql = @"
            UPDATE  Configuration..GroupBuyingProductGroupConfig
            SET     Sequence = @Sequence ,
                    BeginTime = @BeginTime ,
                    EndTime@ = EndTime ,
                    TotalGroupCount = @TotalGroupCount ,
                    LastUpdateDateTime = GETDATE()
            WHERE   ProductGroupId = @ProductGroupId;";
            return conn.Execute(sql, new
            {
                ProductGroupId = grouId,
                Sequence = sequence,
                BeginTime = beginTime,
                EndTime = endTime,
                TotalGroupCount = totalGroupCount
            },
                commandType: CommandType.Text);
        }

        public static List<GroupBuyingConfigLog> GetGroupBuyingLogByIdentityId(string identityId, string source)
        {
            const string sql = @"
            SELECT  gbc.PKID ,
                    gbc.IdentityID ,
                    gbc.Source ,
                    gbc.MethodType ,
                    gbc.Msg ,
                    gbc.BeforeValue ,
                    gbc.AfterValue ,
                    gbc.OperateUser ,
                    gbc.CreateTime
            FROM    Tuhu_log..GroupBuyingConfigLog AS gbc WITH ( NOLOCK )
            WHERE   Source = @Source
                    AND IdentityID = @IdentityId
            ORDER BY PKID DESC;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@IdentityID", identityId);
                    cmd.Parameters.AddWithValue("@Source", source);
                    return dbHelper.ExecuteDataTable(cmd)?.ConvertTo<GroupBuyingConfigLog>()?.ToList() ??
                           new List<GroupBuyingConfigLog>();
                }
            }
        }
        #endregion

        #region 轮胎拼团

        /// <summary>
        /// 添加拼团商品配置    
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns>PKID</returns>
        public static int InsertGroupBuyingProductConfig(BaseDbHelper dbHelper,
            GroupBuyingProductConfigEntity config)
        {
            const string sql = @"
            INSERT  INTO Configuration.dbo.GroupBuyingProductConfig
                    ( ProductName ,
                      SimpleProductName ,
                      PID ,
                      ProductGroupId ,
                      OriginalPrice ,
                      FinalPrice ,
                      SpecialPrice ,
                      DisPlay ,
                      Creator ,
                      IsDelete ,
                      CreateDateTime ,
                      LastUpdateDateTime ,
                      UseCoupon ,
                      IsShow ,
                      BuyLimitCount ,
                      UpperLimitPerOrder ,
                      TotalStockCount ,
                      IsShowApp ,
                      IsAutoStock ,
                      CurrentSoldCount                      
                    )
            OUTPUT Inserted.PKID
            VALUES  ( @ProductName ,
                      @SimpleProductName ,
                      @PID ,
                      @ProductGroupId ,
                      @OriginalPrice ,
                      @FinalPrice ,
                      @SpecialPrice ,
                      @DisPlay ,
                      @Creator ,
                      0 ,
                      GETDATE() ,
                      GETDATE() ,
                      @UseCoupon ,
                      @IsShow ,
                      @BuyLimitCount ,
                      @UpperLimitPerOrder ,
                      @TotalStockCount ,
                      @IsShowApp ,
                      @IsAutoStock ,
                      @CurrentSoldCount
                    );";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductName", config.ProductName);
                cmd.Parameters.AddWithValue("@SimpleProductName", config.SimpleProductName);
                cmd.Parameters.AddWithValue("@PID", config.PID);
                cmd.Parameters.AddWithValue("@ProductGroupId", config.ProductGroupId);
                cmd.Parameters.AddWithValue("@OriginalPrice", config.OriginalPrice);
                cmd.Parameters.AddWithValue("@FinalPrice", config.FinalPrice);
                cmd.Parameters.AddWithValue("@SpecialPrice", config.SpecialPrice);
                cmd.Parameters.AddWithValue("@DisPlay", config.DisPlay);
                cmd.Parameters.AddWithValue("@Creator", config.Creator);
                cmd.Parameters.AddWithValue("@UseCoupon", config.UseCoupon);
                cmd.Parameters.AddWithValue("@IsShow", config.IsShow);
                cmd.Parameters.AddWithValue("@BuyLimitCount", config.BuyLimitCount);
                cmd.Parameters.AddWithValue("@UpperLimitPerOrder", config.UpperLimitPerOrder);
                cmd.Parameters.AddWithValue("@TotalStockCount", config.TotalStockCount);
                cmd.Parameters.AddWithValue("@IsShowApp", config.IsShowApp);
                cmd.Parameters.AddWithValue("@IsAutoStock", config.IsAutoStock);
                cmd.Parameters.AddWithValue("@CurrentSoldCount", config.CurrentSoldCount);
                return (int)dbHelper.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// 添加拼团轮胎商品配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static int InsertGroupBuyingTireProductConfig(BaseDbHelper dbHelper,
            GroupBuyingTireProductConfigEntity config)
        {
            const string sql = @"
            INSERT INTO Configuration.dbo.GroupBuyingTireProductConfig
                (
                    ProductConfigID,
                    TireBrand,
                    TirePattern,
                    TireWidth,
                    TireAspectRatio,
                    TireRim,
                    IsDelete,
                    CreateDateTime,
                    LastUpdateDateTime
                )
                VALUES
                (@ProductConfigID, @TireBrand, @TirePattern, @TireWidth, @TireAspectRatio, @TireRim,
                0, GETDATE(), GETDATE());";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductConfigID", config.ProductConfigID);
                cmd.Parameters.AddWithValue("@TireBrand", config.TireBrand);
                cmd.Parameters.AddWithValue("@TirePattern", config.TirePattern);
                cmd.Parameters.AddWithValue("@TireWidth", config.TireWidth);
                cmd.Parameters.AddWithValue("@TireAspectRatio", config.TireAspectRatio);
                cmd.Parameters.AddWithValue("@TireRim", config.TireRim);
                return dbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 更改拼团轮胎团IsDelete状态
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="productGroupId"></param>
        /// <returns></returns>
        public static int ChangeGroupBuyingTireGroupStatus(BaseDbHelper dbHelper,
            string productGroupId)
        {
            const string sql = @"
            UPDATE Configuration..GroupBuyingProductGroupConfig WITH (ROWLOCK)
            SET IsDelete = 0
            WHERE ProductGroupId = @ProductGroupId;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId);
                return dbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 添加拼团轮胎商品配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static int InsertGroupBuyingTireProductConfig(SqlConnection conn,
            GroupBuyingTireProductConfigEntity config)
        {
            const string sql = @"
            INSERT INTO Configuration.dbo.GroupBuyingTireProductConfig
                (
                    ProductConfigID,
                    TireBrand,
                    TirePattern,
                    TireWidth,
                    TireAspectRatio,
                    TireRim,
                    IsDelete,
                    CreateDateTime,
                    LastUpdateDateTime
                )
                VALUES
                (@ProductConfigID, @TireBrand, @TirePattern, @TireWidth, @TireAspectRatio, @TireRim,
                0, GETDATE(), GETDATE());";

            return conn.Execute(sql, config, commandType: CommandType.Text);
        }

        /// <summary>
        /// 更新拼团轮胎商品配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static int UpdateGroupBuyingTireProductConfig(BaseDbHelper dbHelper,
            GroupBuyingProductConfigEntity config)
        {
            const string sql = @"
            UPDATE Configuration..GroupBuyingProductConfig WITH (ROWLOCK)
            SET ProductName = @ProductName,
                FinalPrice = @FinalPrice,
                SpecialPrice = @SpecialPrice,
	            BuyLimitCount = @BuyLimitCount,
                UpperLimitPerOrder = @UpperLimitPerOrder,
	            TotalStockCount = @TotalStockCount,
                UseCoupon = @UseCoupon,
                LastUpdateDateTime = GETDATE()
            WHERE PKID = @PKID;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", config.PKID);
                cmd.Parameters.AddWithValue("@ProductName", config.ProductName);
                cmd.Parameters.AddWithValue("@FinalPrice", config.FinalPrice);
                cmd.Parameters.AddWithValue("@SpecialPrice", config.SpecialPrice);
                cmd.Parameters.AddWithValue("@BuyLimitCount", config.BuyLimitCount);
                cmd.Parameters.AddWithValue("@UpperLimitPerOrder", config.UpperLimitPerOrder);
                cmd.Parameters.AddWithValue("@TotalStockCount", config.TotalStockCount);
                cmd.Parameters.AddWithValue("@UseCoupon", config.UseCoupon);
                return dbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 获取拼团轮胎商品配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="productGroupId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<GroupBuyingTireProductConfigResponse> GetGroupBuyingTireProductConfigs
            (SqlConnection conn, string productGroupId, string query)
        {
            const string sql = @"
            SELECT PID,
                   B.ProductConfigID,
                   ProductName,
                   B.TireBrand,
                   B.TirePattern,
                   B.TireWidth,
                   B.TireAspectRatio,
                   B.TireRim,
                   A.OriginalPrice,
                   A.FinalPrice,
                   A.SpecialPrice,
                   A.BuyLimitCount,
                   A.UpperLimitPerOrder,
                   A.TotalStockCount,
                   A.CurrentSoldCount,
                   A.UseCoupon,
                   A.DisPlay
            FROM Configuration.dbo.GroupBuyingProductConfig AS A WITH (NOLOCK)
                LEFT JOIN Configuration.dbo.GroupBuyingTireProductConfig AS B WITH (NOLOCK)
                    ON A.PKID = B.ProductConfigID
            WHERE ProductGroupId = @ProductGroupId
            AND A.IsDelete = 0 AND B.IsDelete = 0
            AND
              (
                  A.PID = @PID
                  OR A.ProductName LIKE @ProductName
              )
            ORDER BY A.DisPlay DESC;";

            return conn.Query<GroupBuyingTireProductConfigResponse>(sql, new
            {
                ProductGroupId = productGroupId,
                PID = query,
                ProductName = $"{query}%"
            },
            commandType: CommandType.Text).ToList();
        }

        #endregion
    }
}
