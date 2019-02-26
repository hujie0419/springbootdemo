using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyYearCard;

namespace Tuhu.Provisioning.DataAccess.DAO.BeautyYearCardDao
{
    public class DalBeautyYearCardConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;
        private static string strConn_tuhulog = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
        private static string strConn_Tuhu_productcatalog = ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString;

        public static Tuple<int, IEnumerable<BeautyYearCardModel>> SelectBeautyYearCardConfigs(int pageIndex, int pageSize, string keyWrod, int nameType, int statusType)
        {
            #region sql
            string sqlCount = @"SELECT  COUNT(1)
  FROM [Tuhu_groupon].[dbo].[ShopBeautyYearCardConfig] AS BYCC  WITH(NOLOCK) 
    {0} 
  WHERE BYCC.[IsDeleted]=0   {1} {2} ";
            string sql = @"SELECT  BYCC.[PKID]
      ,BYCC.[CardName]
      ,BYCC.[CardPrice]
      ,BYCC.[PID]
      ,BYCC.[CardImageUrl]
      ,BYCC.[SalesBeginTime]
      ,BYCC.[SalesEndTime]
      ,BYCC.[CardValidDays]
      ,BYCC.[CardType] 
  FROM [Tuhu_groupon].[dbo].[ShopBeautyYearCardConfig] AS BYCC  WITH(NOLOCK) 
  {0} 
  WHERE BYCC.[IsDeleted]=0  {1} {2}
  Order by BYCC.UpdateTime desc 
  Offset @begin rows fetch next @pageSize rows only";
            #region 关键字
            if (!string.IsNullOrEmpty(keyWrod))
            {
                if (nameType == 1)//年卡名称
                {

                    sql = string.Format(sql, "", "AND BYCC.[CardName] LIKE  @KeyWord ", "{0}");
                    sqlCount = string.Format(sqlCount, "", "AND BYCC.[CardName] LIKE  @KeyWord ", "{0}");
                }
                else if (nameType == 2)//年卡包含服务
                {
                    sql = string.Format(sql, " LEFT JOIN [Tuhu_groupon].[dbo].[ShopBeautyYearCardProductsConfig] AS BYCPC  WITH(NOLOCK) ON BYCC.PKID=BYCPC.CardId "
                       , "AND  BYCPC.IsDeleted=0   AND BYCPC.[ProductName]  LIKE  @KeyWord ", "{0}");
                    sqlCount = string.Format(sqlCount, " LEFT JOIN [Tuhu_groupon].[dbo].[ShopBeautyYearCardProductsConfig] AS BYCPC  WITH(NOLOCK) ON BYCC.PKID=BYCPC.CardId "
                       , "AND  BYCPC.IsDeleted=0   AND  BYCPC.[ProductName]  LIKE  @KeyWord ", "{0}");
                }
            }
            else
            {
                sql = string.Format(sql, "", "", "{0}");
                sqlCount = string.Format(sqlCount, "", "", "{0}");
            }
            #endregion

            #region 年卡状态
            if (statusType == 2)//正常
            {
                sql = string.Format(sql, " AND SalesEndTime>=CONVERT(DATE,getdate()) ");
                sqlCount = string.Format(sqlCount, " AND  SalesEndTime>=CONVERT(DATE,getdate()) ");
            }
            else if (statusType == 3)//过期
            {
                sql = string.Format(sql, " AND  SalesEndTime<CONVERT(DATE,getdate()) ");
                sqlCount = string.Format(sqlCount, " AND   SalesEndTime<CONVERT(DATE,getdate()) ");
            }
            else//所有
            {
                sql = string.Format(sql, "");
                sqlCount = string.Format(sqlCount, "");
            }
            #endregion



            #endregion
            Tuple<int, IEnumerable<BeautyYearCardModel>> result = null;
            using (var conn = new SqlConnection(strConn))
            {
                var count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn,
                    CommandType.Text,
                    sqlCount,
                    new SqlParameter[] {
                      new SqlParameter("@KeyWord", "%"+keyWrod+"%")
                  }));
                if (count > 0)
                {

                    var dt = SqlHelper.ExecuteDataTable(conn,
                  CommandType.Text,
                  sql,
                  new SqlParameter[] { new SqlParameter("@begin", (pageIndex - 1) * pageSize),
                    new SqlParameter("@pageSize", pageSize),
                      new SqlParameter("@KeyWord", "%"+keyWrod+"%")
                  });
                    var temp = dt.Rows.Cast<DataRow>().Select(row => new BeautyYearCardModel
                    {
                        PKID = (int)row["PKID"],
                        CardName = row["CardName"].ToString(),
                        CardPrice = Convert.ToDecimal(row["CardPrice"]),
                        PID = row["PID"].ToString(),
                        SalesBeginTime = Convert.ToDateTime(row["SalesBeginTime"]),
                        SalesEndTime = Convert.ToDateTime(row["SalesEndTime"]),
                        CardValidDays = Convert.ToInt32(row["CardValidDays"]),
                        CardType= Convert.ToInt32(row["CardType"]),
                    });
                    result = Tuple.Create(count, temp);
                }
                return result;
            }

        }
        public static IEnumerable<BeautyYearCardProductModel> SelectBeautyYearCardProductConfigs(int[] cardId)
        {
            const string sql = @"SELECT  [PKID]
      ,[CardId]
      ,[ProductId]
      ,[ProductName]
      ,[ProductNumber]
      ,[ProductDescription]
      ,[ProductPrice]
      ,[Commission]
      ,[UseCycle]
      ,[CycleType]
  FROM [Tuhu_groupon].[dbo].[ShopBeautyYearCardProductsConfig] WITH(NOLOCK)
  WHERE IsDeleted=0 AND CardId IN(SELECT Item FROM [Tuhu_groupon].[dbo].SplitString(@CardId,',',1))";
            using (var conn = new SqlConnection(strConn))
            {
                var dt = SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@CardId", string.Join(",", cardId)) });
                var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyYearCardProductModel
                {
                    PKID = (int)row["PKID"],
                    CardId = Convert.ToInt32(row["CardId"]),
                    ProductId = row["ProductId"].ToString(),
                    ProductName = row["ProductName"].ToString(),
                    ProductNumber = Convert.ToInt32(row["ProductNumber"]),
                    ProductPrice = Convert.ToDecimal(row["ProductPrice"]),
                    ProductDescription = row["ProductDescription"].ToString(),
                    Commission = Convert.ToSingle(row["Commission"]),
                    UseCycle = Convert.ToInt32(row["UseCycle"]),
                    CycleType = Convert.ToInt32(row["CycleType"]),
                });
                return result;
            }

        }

        public static IEnumerable<BeautyYearCardRegionModel> SelectBeautyYearCardRegionConfigs(int cardId)
        {
            const string sql = @"SELECT [CardId]
      ,[ProvinceId]
      ,[CityId]
      ,[IsAllCity]
  FROM [Tuhu_groupon].[dbo].[ShopBeautyYearCardRegionConfig] With(Nolock)
  Where CardId=@CardId";
            using (var conn = new SqlConnection(strConn))
            {
                var dt = SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@CardId", cardId) });
                var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyYearCardRegionModel
                {
                    CardId = Convert.ToInt32(row["CardId"]),
                    ProvinceId = Convert.ToInt32(row["ProvinceId"]),
                    CityId = row["CityId"]?.ToString(),
                    IsAllCity = Convert.ToBoolean(row["IsAllCity"]),
                });
                return result;
            }


        }
        public static BeautyYearCardModel GetBeautyYearCardConfig(int cardId)
        {
            const string sql = @"SELECT [PKID]
      ,[CardName]
      ,[CardPrice]
      ,[PID]
      ,[CardImageUrl]
      ,[AdaptVehicle]
      ,[Remark]
      ,[Rule]
      ,[SalesBeginTime]
      ,[SalesEndTime]
      ,[CardValidDays]
      ,[CardType] 
  FROM [Tuhu_groupon].[dbo].[ShopBeautyYearCardConfig] WITH(NOLOCK)
  WHERE PKID=@PKID AND [IsDeleted]=0";
            using (var conn = new SqlConnection(strConn))
            {
                var dt = SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@PKID", cardId) });
                var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyYearCardModel
                {
                    PKID = Convert.ToInt32(row["PKID"]),
                    CardName = row["CardName"].ToString(),
                    AdaptVehicle = Convert.ToInt32(row["AdaptVehicle"]),
                    CardImageUrl = row["CardImageUrl"].ToString(),
                    CardPrice = Convert.ToDecimal(row["CardPrice"]),
                    CardValidDays = Convert.ToInt32(row["CardValidDays"]),
                    CardType = Convert.ToInt32(row["CardType"]),
                    PID = row["PID"].ToString(),
                    Remark = row["Remark"].ToString(),
                    Rule = row["Rule"].ToString(),
                    SalesBeginTime = Convert.ToDateTime(row["SalesBeginTime"]),
                    SalesEndTime = Convert.ToDateTime(row["SalesEndTime"])
                })?.FirstOrDefault();
                if (result != null)
                {
                    result.BeautyYearCardProducts = SelectBeautyYearCardProductConfigs(new[] { cardId })?.ToArray();
                    result.BeautyYearCardRegions = SelectBeautyYearCardRegionConfigs(cardId)?.ToArray();
                }
                return result;
            }



        }

        public static int CreateBeautyYearCardConfig(BeautyYearCardModel model)
        {
            #region sql
            const string sql = @"Insert Into [Tuhu_groupon].[dbo].[ShopBeautyYearCardConfig]
([CardName]
,[CardPrice]
,[PID]
,[CardImageUrl]
,[AdaptVehicle]
,[Remark]
,[Rule]
,[SalesBeginTime]
,[SalesEndTime]
,[CardValidDays]
,IsDeleted
,CreateTime
,UpdateTime
,CardType)
values(
@CardName,
@CardPrice,
@PID,
@CardImageUrl,
@AdaptVehicle,
@Remark,
@Rule,
@SalesBeginTime,
@SalesEndTime,
@CardValidDays,
0,
GetDate(),
GetDate(),
@CardType
);Select @@identity;";
            const string sqlPro = @"Insert into [Tuhu_groupon].[dbo].[ShopBeautyYearCardProductsConfig]
([CardId]
 ,[ProductId]
 ,[ProductName]
 ,[ProductNumber]
 ,[ProductPrice]
 ,[ProductDescription]
 ,[Commission]
 ,[UseCycle]
 ,[CycleType]
 ,[IsDeleted]
 ,[CreateTime]
 ,[UpdateTime])
  values(
  @CardId,
@ProductId,
@ProductName,
@ProductNumber,
@ProductPrice,
@ProductDescription,
@Commission,
@UseCycle,
@CycleType,
0,
GetDate(),
GetDate()
  ) ";
            const string sqlRegion = @"Insert Into [Tuhu_groupon].[dbo].[ShopBeautyYearCardRegionConfig]
([CardId]
 ,[ProvinceId]
 ,[CityId]
 ,[IsAllCity]
 ,[CreateTime]
 ,[UpdateTime])
 values(
 @CardId,
 @ProvinceId,
 @CityId,
 @IsAllCity,
GetDate(),
GetDate()
 )";
            #endregion
            using (var db = new SqlDbHelper(strConn))
            {
                db.BeginTransaction();
                try
                {
                    #region CardConfig
                    var cardId = Convert.ToInt32(db.ExecuteScalar(sql,
                  CommandType.Text,
                  new SqlParameter[] { new SqlParameter("@CardName", model.CardName),
                   new SqlParameter("@CardPrice", model.CardPrice),
                   new SqlParameter("@PID", model.PID),
                   new SqlParameter("@CardImageUrl", model.CardImageUrl),
                   new SqlParameter("@AdaptVehicle", model.AdaptVehicle),
                   new SqlParameter("@Remark", model.Remark),
                   new SqlParameter("@Rule", model.Rule),
                   new SqlParameter("@SalesBeginTime", model.SalesBeginTime),
                   new SqlParameter("@SalesEndTime", model.SalesEndTime),
                   new SqlParameter("@CardType", model.CardType),
                   new SqlParameter("@CardValidDays", model.CardValidDays)}
                  ));
                    #endregion
                    if (cardId > 0 && model.BeautyYearCardProducts.Any())
                    {
                        #region proConfig
                        var proInsert = true;
                        foreach (var f in model.BeautyYearCardProducts)
                        {
                            var proResult = Convert.ToInt32(db.ExecuteNonQuery(sqlPro,
CommandType.Text,
new SqlParameter[] { new SqlParameter("@CardId", cardId),
                   new SqlParameter("@ProductId", f.ProductId),
                   new SqlParameter("@ProductName", f.ProductName),
                   new SqlParameter("@ProductDescription", f.ProductDescription),
                   new SqlParameter("@ProductNumber", f.ProductNumber),
                   new SqlParameter("@ProductPrice", f.ProductPrice),
                   new SqlParameter("@Commission", f.Commission),
                   new SqlParameter("@UseCycle", f.UseCycle),
                   new SqlParameter("@CycleType", f.CycleType)}));
                            if (proResult <= 0)
                            {
                                db.Rollback();
                                proInsert = false;

                            }
                        }
                        #endregion
                        if (proInsert && model.BeautyYearCardRegions.Any())
                        {
                            #region regConfig

                            var regInsert = true;
                            foreach (var e in model.BeautyYearCardRegions)
                            {
                                var regResult = Convert.ToInt32(db.ExecuteNonQuery(sqlRegion,
CommandType.Text,
new SqlParameter[] { new SqlParameter("@CardId", cardId),
                   new SqlParameter("@ProvinceId", e.ProvinceId),
                   new SqlParameter("@CityId", (e.IsAllCity? null:e.CityId)),
                   new SqlParameter("@IsAllCity", e.IsAllCity)}));
                                if (regResult <= 0)
                                {
                                    db.Rollback();
                                    regInsert = false;

                                }
                            }
                            if (regInsert)
                            {
                                db.Commit();
                                return cardId;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        db.Rollback();
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    throw ex;
                }
            }
        }

        public static bool UpdateBeautyYearCardConfig(BeautyYearCardModel model)
        {
            #region sql
            const string sql = @"Update  [Tuhu_groupon].[dbo].[ShopBeautyYearCardConfig] WITH(ROWLOCK)
 SET 
[CardName]=IsNULL(@CardName,CardName)
,[CardPrice]=IsNULL(@CardPrice,CardPrice)
,[PID]=IsNULL(@PID,PID)
,[CardImageUrl]=IsNULL(@CardImageUrl,CardImageUrl)
,[AdaptVehicle]=IsNULL(@AdaptVehicle,AdaptVehicle)
,[Remark]=IsNULL(@Remark,Remark)
,[Rule]=IsNULL(@Rule,[Rule])
,[SalesBeginTime]=IsNULL(@SalesBeginTime,SalesBeginTime)
,[SalesEndTime]=IsNULL(@SalesEndTime,SalesEndTime)
,[CardValidDays]=IsNULL(@CardValidDays,CardValidDays)
,[CardType]=IsNULL(@CardType,CardType) 
 WHERE PKID=@PKID";
            const string sqlPro = @"UPDATE  [Tuhu_groupon].[dbo].[ShopBeautyYearCardProductsConfig] WITH(ROWLOCK)
 SET 
  [ProductId]=IsNULL(@ProductId,ProductId)
 ,[ProductName]=IsNULL(@ProductName,ProductName)
 ,[ProductNumber]=IsNULL(@ProductNumber,ProductNumber)
 ,[ProductPrice]=IsNULL(@ProductPrice,ProductPrice)
 ,[ProductDescription]=IsNULL(@ProductDescription,ProductDescription)
 ,[Commission]=IsNULL(@Commission,Commission)
 ,[UseCycle]=IsNULL(@UseCycle,UseCycle)
 ,[CycleType]=IsNULL(@CycleType,CycleType)
 ,[UpdateTime]=GETDATE()
 WHERE PKID=@PKID AND CardId=@CardId";
            const string regionDel = @"Delete from [Tuhu_groupon].[dbo].[ShopBeautyYearCardRegionConfig] where CardId=@CardId";

            const string sqlRegion = @"Insert Into [Tuhu_groupon].[dbo].[ShopBeautyYearCardRegionConfig]
([CardId]
 ,[ProvinceId]
 ,[CityId]
 ,[IsAllCity]
 ,[CreateTime]
 ,[UpdateTime])
 values(
 @CardId,
 @ProvinceId,
 @CityId,
 @IsAllCity,
GetDate(),
GetDate()
 )
";
            #endregion
            using (var db = new SqlDbHelper(strConn))
            {
                db.BeginTransaction();
                try
                {
                    #region CardConfig
                    var cardId = Convert.ToInt32(db.ExecuteNonQuery(sql,
                  CommandType.Text,
                  new SqlParameter[] {
                   new SqlParameter("@PKID", model.PKID),
                   new SqlParameter("@CardName", model.CardName),
                   new SqlParameter("@CardPrice", model.CardPrice),
                   new SqlParameter("@PID", model.PID),
                   new SqlParameter("@CardImageUrl", model.CardImageUrl),
                   new SqlParameter("@AdaptVehicle", model.AdaptVehicle),
                   new SqlParameter("@Remark", model.Remark),
                   new SqlParameter("@Rule", model.Rule),
                   new SqlParameter("@SalesBeginTime", model.SalesBeginTime),
                   new SqlParameter("@SalesEndTime", model.SalesEndTime),
                   new SqlParameter("@CardType", model.CardType),
                   new SqlParameter("@CardValidDays", model.CardValidDays)}));
                    #endregion
                    if (cardId > 0 && model.BeautyYearCardProducts.Any())
                    {
                        #region proConfig
                        var proUpdate = true;
                        foreach (var f in model.BeautyYearCardProducts)
                        {
                            var proResult = Convert.ToInt32(db.ExecuteNonQuery(sqlPro,
                            CommandType.Text,
                            new SqlParameter[] {
                   new SqlParameter("@PKID",f.PKID),
                   new SqlParameter("@CardId", model.PKID),
                   new SqlParameter("@ProductId", f.ProductId),
                   new SqlParameter("@ProductName", f.ProductName),
                   new SqlParameter("@ProductDescription", f.ProductDescription),
                   new SqlParameter("@ProductNumber", f.ProductNumber),
                   new SqlParameter("@ProductPrice", f.ProductPrice),
                   new SqlParameter("@Commission", f.Commission),
                   new SqlParameter("@UseCycle", f.UseCycle),
                   new SqlParameter("@CycleType", f.CycleType)}));
                            if (proResult <= 0)
                            {
                                db.Rollback();
                                proUpdate = false;
                                break;
                            }
                        }
                        #endregion
                        if (proUpdate && model.BeautyYearCardRegions.Any())
                        {
                            #region regConfig
                            var regInsert = true;
                            var delResult = Convert.ToInt32(db.ExecuteNonQuery(regionDel,
                                CommandType.Text,
                                new SqlParameter[] {
                                    new SqlParameter("@CardId",model.PKID)
                                })
                                );
                            if (delResult > 0)
                            {
                                foreach (var e in model.BeautyYearCardRegions)
                                {
                                    var regResult = Convert.ToInt32(db.ExecuteNonQuery(sqlRegion,
                                    CommandType.Text,
                                    new SqlParameter[] { new SqlParameter("@CardId", model.PKID),
                                                    new SqlParameter("@ProvinceId", e.ProvinceId),
                                                    new SqlParameter("@CityId",(e.IsAllCity? null: e.CityId)),
                                                    new SqlParameter("@IsAllCity", e.IsAllCity)}));
                                    if (regResult <= 0)
                                    {
                                        db.Rollback();
                                        regInsert = false;
                                        break;
                                    }
                                }
                                if (regInsert)
                                {
                                    db.Commit();
                                    return true;
                                }
                            }
                            #endregion
                        }
                    }
                    db.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    throw ex;
                }
            }

        }

        public static bool DeleteBeautyYearCardConfig(int pkid)
        {
            const string sql = @"UPDATE  [Tuhu_groupon].[dbo].[ShopBeautyYearCardConfig] With(Rowlock) 
SET [IsDeleted]=1 ,[UpdateTime]=GetDate() 
Where PKID=@PKID";
            using (var conn = new SqlConnection(strConn))
            {
                var result = SqlHelper.ExecuteNonQuery(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@PKID", pkid) });

                return result > 0;
            }
        }
        public static BeautyOprLog[] GetBeautyYearCardConfigLog(int cardId)
        {
            const string sql = @"SELECT  OperateUser,CreateDateTime as CreateTime,OldValue,NewValue 
From [Tuhu_log].[dbo].[BeautyOprLog] with(nolock) 
Where IdentityID=@CardId 
AND  (LogType='CreateBeautyYearCardConfig' 
OR LogType='UpdateBeautyYearCardConfig') 
Order by  CreateDateTime desc";

            using (var conn = new SqlConnection(strConn_tuhulog))
            {
                var dt = SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@CardId", cardId.ToString()) });
                var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyOprLog
                {
                    OperateUser = row["OperateUser"].ToString(),
                    CreateTime = Convert.ToDateTime(row["CreateTime"]),
                    OldValue = row["OldValue"].ToString(),
                    NewValue = row["NewValue"].ToString()

                });
                return result?.ToArray();
            }
        }
        /// <summary>
        /// 查询美容操作日志
        /// </summary>
        /// <param name="identityId"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public static IEnumerable<BeautyOprLog> GetBeautyOprLog(string identityId,string logType)
        {
            const string sql = @"SELECT  OperateUser ,
        CreateDateTime AS CreateTime ,
        OldValue ,
        NewValue
FROM    [Tuhu_log].[dbo].[BeautyOprLog] WITH ( NOLOCK )
WHERE   IdentityID = @IdentityID
        AND LogType = @LogType
ORDER BY CreateDateTime DESC;";

            using (var conn = new SqlConnection(strConn_tuhulog))
            {
                return SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[]
                   {
                        new SqlParameter("@IdentityID", identityId.ToString()) ,
                        new SqlParameter("@LogType", logType)
                   }).ConvertTo<BeautyOprLog>();
            }
        }

        public static Tuple<string, string, string> GetParentProuductInfo(string productId)
        {
            const string sql = @"SELECT  
      [CatalogName]
     ,[PrimaryParentCategory]
     ,[DefinitionName]
  FROM [Tuhu_productcatalog].[dbo].[CarPAR_CatalogProducts] WITH(NOLOCK)
  where  productid=@productid  AND VariantID=''";
            using (var conn = new SqlConnection(strConn_Tuhu_productcatalog))
            {
                var dt = SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@productid", productId) });
                var result = dt.Rows.Cast<DataRow>().Select(row => Tuple.Create(row["CatalogName"].ToString(),
                    row["PrimaryParentCategory"].ToString(),
                    row["DefinitionName"].ToString()));
                return result?.FirstOrDefault();
            }
        }
        public static ProductSimpleModel GetSubProuductInfo(string pid)
        {
            const string sql = @"SELECT [DisplayName]
      ,[Description] 
      ,PID
  FROM [Tuhu_productcatalog].[dbo].[vw_Products] WITH(NOLOCK)
  where PID=@PID";
            using (var conn = new SqlConnection(strConn_Tuhu_productcatalog))
            {
                var dt = SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@PID", pid) });
                var result = dt.Rows.Cast<DataRow>().Select(row => new ProductSimpleModel
                {
                    Description = row["Description"].ToString(),
                    DisplayName = row["DisplayName"].ToString(),
                    PID = row["PID"].ToString(),
                });
                return result?.FirstOrDefault();
            }
        }
        /// <summary>
        /// 查询用户年卡记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public static Tuple<IEnumerable<ShopBeautyUserYearCard>, int> SelectShopBeautyUserYearCards(string userId, string orderId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT  UC.PKID ,
        UC.Phone ,
        UC.UserId ,
        UC.UserCardId ,
        UC.CardId ,
        UC.CardPrice ,
        UC.ShopId ,
        UC.AdaptVehicle ,
        UC.BeginTime ,
        UC.EndTime ,
        UC.Status ,
        UC.CreateTime ,
        UC.UpdateTime ,
        UC.CardType
FROM    [Tuhu_groupon].[dbo].[ShopBeautyUserYearCard] (NOLOCK) AS UC
WHERE   ( UC.UserId = @UserId
          OR @UserId = ''
          OR @UserId IS NULL
        )
        AND ( UC.UserCardId = @OrderId
              OR @OrderId = ''
              OR @OrderId IS NULL
            )
ORDER BY UC.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;
SELECT  @TotalCount = COUNT(1)
FROM    [Tuhu_groupon].[dbo].[ShopBeautyUserYearCard] (NOLOCK) AS UC
WHERE   ( UC.UserId = @UserId
          OR @UserId = ''
          OR @UserId IS NULL
        )
        AND ( UC.UserCardId = @OrderId
              OR @OrderId = ''
              OR @OrderId IS NULL
            );";

            using (var conn = new SqlConnection(strConnOnRead))
            {
                var parameters = new SqlParameter[]
                   {
                       new SqlParameter("@UserId", userId),
                       new SqlParameter("@OrderId", orderId) ,
                       new SqlParameter("@PageIndex", pageIndex),
                       new SqlParameter("@PageSize", pageSize),
                       new SqlParameter("@TotalCount", SqlDbType.Int) {Direction = ParameterDirection.Output}
                   };
                var list = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<ShopBeautyUserYearCard>();
                var count = Convert.ToInt32(parameters.Last().Value);
                return new Tuple<IEnumerable<ShopBeautyUserYearCard>, int>(list, count);
            }
        }
        /// <summary>
        /// 批量查询年卡配置
        /// </summary>
        /// <param name="cardIds"></param>
        /// <returns></returns>
        public static IEnumerable<BeautyYearCardModel> SelectBeautyYearCardModelsByCardIds(IEnumerable<int> cardIds)
        {
            var sql = @"
SELECT  A.PKID ,
        A.CardName ,
        A.CardPrice ,
        A.PID ,
        A.CardImageUrl ,
        A.AdaptVehicle ,
        A.Remark ,
        A.[Rule] ,
        A.SalesBeginTime ,
        A.SalesEndTime ,
        A.CardValidDays ,
        A.IsDeleted ,
        A.CreateTime ,
        A.UpdateTime ,
        A.CardType
FROM    [Tuhu_groupon].[dbo].[ShopBeautyYearCardConfig] AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@CardIds, ',', 1) ON A.PKID = Item;";
            using (var conn = new SqlConnection(strConnOnRead))
            {
                return SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@CardIds", string.Join(",", cardIds)) }).ConvertTo<BeautyYearCardModel>();
            }
        }
        /// <summary>
        /// 查询用户年卡详情
        /// </summary>
        /// <param name="userYearCardId"></param>
        /// <returns></returns>
        public static IEnumerable<ShopBeautyUserYearCardDetail> SelectShopBeautyUserYearCardDetails(long userYearCardId)
        {
            var sql = @"SELECT  A.PKID ,
        A.UserYearCardId ,
        A.UserCardId ,
        A.ProductId ,
        A.ProductPrice ,
        A.ProductNumber ,
        A.Commission ,
        A.UseCycle ,
        A.CycleType ,
        A.FwCode ,
        A.UsedShopId ,
        A.UsedTime ,
        A.CreateTime ,
        A.UpdateTime ,
        A.ProductName ,
        A.ProductDescription
FROM    Tuhu_groupon..ShopBeautyUserYearCardDetail AS A WITH ( NOLOCK )
WHERE   A.UserYearCardId = @UserYearCardId;";
            using (var conn = new SqlConnection(strConnOnRead))
            {
                return SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text, sql,
                   new SqlParameter[] { new SqlParameter("@UserYearCardId", userYearCardId) }).ConvertTo<ShopBeautyUserYearCardDetail>();
            }
        }
        /// <summary>
        /// 批量查询用户年卡详情
        /// </summary>
        /// <param name="userYearCardId"></param>
        /// <returns></returns>
        public static IEnumerable<ShopBeautyUserYearCardDetail> SelectShopBeautyUserYearCardDetails(IEnumerable<int> userYearCardIds)
        {
            var sql = @"SELECT  A.PKID ,
        A.UserYearCardId ,
        A.UserCardId ,
        A.ProductId ,
        A.ProductPrice ,
        A.ProductNumber ,
        A.Commission ,
        A.UseCycle ,
        A.CycleType ,
        A.FwCode ,
        A.UsedShopId ,
        A.UsedTime ,
        A.CreateTime ,
        A.UpdateTime ,
        A.ProductName ,
        A.ProductDescription
FROM    Tuhu_groupon..ShopBeautyUserYearCardDetail AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@UserYearCardIds, ',', 1) ON A.UserYearCardId = Item;";
            using (var conn = new SqlConnection(strConnOnRead))
            {
                return SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text, sql,
                   new SqlParameter[] { new SqlParameter("@UserYearCardIds", string.Join(",", userYearCardIds)) }).ConvertTo<ShopBeautyUserYearCardDetail>();
            }
        }
        /// <summary>
        /// 作废美容年卡
        /// </summary>
        /// <param name="userCardId"></param>
        /// <returns></returns>
        public static  bool InvalidUserBeautyYearCard(long userCardId)
        {
            const string sql = @"  UPDATE  [Tuhu_groupon].[dbo].[ShopBeautyUserYearCard] WITH ( ROWLOCK )
SET     [Status] = 2 ,
        UpdateTime = GETDATE()
WHERE   PKID = @UserCardId;";
            using (var conn = new SqlConnection(strConn))
            {
                return SqlHelper.ExecuteNonQuery(conn,
                   CommandType.Text, sql,
                   new SqlParameter[]
                   {
                       new SqlParameter("@UserCardId", userCardId)
                   }) > 0;
            }
        }
    }
}
