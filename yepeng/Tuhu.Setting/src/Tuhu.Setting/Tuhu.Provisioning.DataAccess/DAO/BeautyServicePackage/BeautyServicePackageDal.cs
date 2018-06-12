using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using System;

namespace Tuhu.Provisioning.DataAccess.DAO.BeautyServicePackageDal
{
    public class BeautyServicePackageDal
    {
        private static string writeStrConfig = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string writeConnStr = SecurityHelp.IsBase64Formatted(writeStrConfig) ? SecurityHelp.DecryptAES(writeStrConfig) : writeStrConfig;

        private static string readStrConfig = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;
        private static string readConnStr = SecurityHelp.IsBase64Formatted(readStrConfig) ? SecurityHelp.DecryptAES(readStrConfig) : readStrConfig;

        public static string GetTuhuGrouponWriteConnstr()
        {
            return writeConnStr;
        }

        public static Tuple<IEnumerable<BeautyServicePackage>, int> SelectBeautyServicePackage(int pageIndex, int pageSize, string packageType,
            string packageName, string vipCompanyName, string settlementMethod, int cooperateId)
        {
            string sql = @" 
SELECT  @TotalCount = COUNT(1)
FROM    Tuhu_groupon..BeautyServicePackage WITH ( NOLOCK )
WHERE   ( PackageType = @PackageType
          OR @PackageType = ''
        )
        AND ( PackageName LIKE N'%' + @PackageName + '%'
              OR @PackageName = ''
            )
        AND ( VipCompanyName = @VipCompanyName
              OR @VipCompanyName = ''
            )
        AND ( SettlementMethod = @SettlementMethod
              OR @SettlementMethod = ''
            )
         AND ( CooperateId = @CooperateId
              OR @CooperateId <= 0
            ); 
SELECT  PKID ,
        VipUserId ,
        VipUserName ,
        VipCompanyId ,
        VipCompanyName ,
        PackageName ,
        Department ,
        Purpose ,
        PackageCodeNum ,
        SettlementMethod ,
        Description ,
        PackageType ,
        PackageCodeStartTime ,
        PackageCodeEndTime ,
        IsPackageCodeGenerated ,
        IsActive ,
        CreateUser ,
        UpdateUser ,
        BuyoutOrderId ,
        CooperateId
FROM    Tuhu_groupon..BeautyServicePackage WITH ( NOLOCK )
WHERE   ( PackageType = @PackageType
          OR @PackageType = ''
        )
        AND ( PackageName LIKE N'%' + @PackageName + '%'
              OR @PackageName = ''
            )
        AND ( VipCompanyName = @VipCompanyName
              OR @VipCompanyName = ''
            )
        AND ( SettlementMethod = @SettlementMethod
              OR @SettlementMethod = ''
            )
        AND ( CooperateId = @CooperateId
              OR @CooperateId <= 0
            )
ORDER BY PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            var sqlParameters = new SqlParameter[]
            {
                 new SqlParameter("@PageSize",pageSize),
                 new SqlParameter("@PageIndex",pageIndex),
                 new SqlParameter("@PackageType",packageType),
                 new SqlParameter("@PackageName",packageName),
                 new SqlParameter("@VipCompanyName",vipCompanyName),
                 new SqlParameter("@SettlementMethod",settlementMethod),
                 new SqlParameter("@CooperateId", cooperateId),
                 new SqlParameter("@TotalCount",SqlDbType.Int){ Direction=ParameterDirection.Output}
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                var packages = dbhelper.ExecuteDataTable(sql, CommandType.Text, sqlParameters).ConvertTo<BeautyServicePackage>().ToList();
                var totalCount = Convert.ToInt32(sqlParameters.Last().Value);
                return new Tuple<IEnumerable<BeautyServicePackage>, int>(packages, totalCount);
            }
        }

        public static IEnumerable<string> SelectAllVipUserName()
        {
            var result = new List<string>();
            string sql = @" SELECT DISTINCT VipCompanyName FROM Tuhu_groupon..BeautyServicePackage(NOLOCK) ";
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                var dt = dbhelper.ExecuteDataTable(sql, CommandType.Text);
                if (dt != null && dt.Rows != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = row.IsNull("VipCompanyName") ? string.Empty : row["VipCompanyName"].ToString();
                        if (!string.IsNullOrEmpty(item))
                        {
                            result.Add(item);
                        }
                    }
                }
            }
            return result;
        }
        public static IEnumerable<BeautyServicePackageDetail> SelectBeautyServicePackageDetails(int packegeId)
        {
            string sql = @"SELECT  PKID ,
        PackageId ,
        PID ,
        ServiceCodeTypeId ,
        Name ,
        VipSettlementPrice ,
        ShopCommission ,
        SettlementMethod ,
        Num ,
        ServiceCodeNum ,
        ServiceCodeStartTime ,
        ServiceCodeEndTime ,
        IsActive ,
        EffectiveDayAfterExchange ,
        IsServiceCodeGenerated ,
        BuyoutOrderId ,
        IsImportUser ,
        CreateUser ,
        UpdateUser ,
        CooperateId
FROM    Tuhu_groupon..BeautyServicePackageDetail WITH ( NOLOCK )
WHERE   PackageId = @PackegeId
ORDER BY PKID DESC;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PackegeId",packegeId)
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, sqlParameters).ConvertTo<BeautyServicePackageDetail>().ToList();
            }
        }
        /// <summary>
        /// 分页查询服务码配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isImportUser"></param>
        /// <param name="settlementMethod"></param>
        /// <param name="cooperateId"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<BeautyServicePackageDetail>, int> SelectBeautyServicePackageDetails(int pageIndex, int pageSize,
            bool isImportUser, string settlementMethod, int cooperateId, string serviceId)
        {
            var sql = @"SELECT  @TotalCount = COUNT(1)
FROM    Tuhu_groupon..BeautyServicePackageDetail AS A WITH ( NOLOCK )
        LEFT JOIN Tuhu_groupon..BeautyServicePackage AS B WITH ( NOLOCK ) ON A.PackageId = B.PKID
WHERE   ( B.PackageType = 'serviceCode'
          OR A.PackageId = 0
        )
        AND A.IsImportUser = @IsImportUser
        AND ( A.SettlementMethod = @SettlementMethod
              OR @SettlementMethod = ''
            )
        AND ( A.CooperateId = @CooperateId
              OR @CooperateId <= 0
            )
        AND ( A.PID = @ServiceId
              OR @ServiceId = ''
            );

SELECT  A.PKID ,
        A.PackageId ,
        A.PID ,
        A.ServiceCodeTypeId ,
        A.Name ,
        A.VipSettlementPrice ,
        A.ShopCommission ,
        A.SettlementMethod ,
        A.Num ,
        A.ServiceCodeNum ,
        A.IsActive ,
        A.ServiceCodeStartTime ,
        A.ServiceCodeEndTime ,
        A.EffectiveDayAfterExchange ,
        A.IsServiceCodeGenerated ,
        A.IsImportUser ,
        A.BuyoutOrderId ,
        A.CooperateId ,
        A.CreateUser,
        A.UpdateUser
FROM    Tuhu_groupon..BeautyServicePackageDetail AS A WITH ( NOLOCK )
        LEFT JOIN Tuhu_groupon..BeautyServicePackage AS B WITH ( NOLOCK ) ON A.PackageId = B.PKID
WHERE   ( B.PackageType = 'serviceCode'
          OR A.PackageId = 0
        )
        AND A.IsImportUser = @IsImportUser
        AND ( A.SettlementMethod = @SettlementMethod
              OR @SettlementMethod = ''
            )
        AND ( A.CooperateId = @CooperateId
              OR @CooperateId <= 0
            )
        AND ( A.PID = @ServiceId
              OR @ServiceId = ''
            )
ORDER BY A.CreateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@IsImportUser",isImportUser),
                new SqlParameter("@SettlementMethod", settlementMethod),
                new SqlParameter("@CooperateId", cooperateId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@ServiceId", serviceId),
                new SqlParameter("@TotalCount",SqlDbType.Int){ Direction=ParameterDirection.Output}
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                var result = dbhelper.ExecuteDataTable(sql, CommandType.Text, sqlParameters).ConvertTo<BeautyServicePackageDetail>().ToList();
                var totalCount = Convert.ToInt32(sqlParameters.Last().Value);
                return new Tuple<IEnumerable<BeautyServicePackageDetail>, int>(result, totalCount);
            }
        }

        public static BeautyServicePackage SelectBeautyServicePackage(int packageId)
        {
            string sql = @"SELECT  PKID ,
        VipUserId ,
        VipUserName ,
        VipCompanyId ,
        VipCompanyName ,
        PackageName ,
        Department ,
        Purpose ,
        PackageCodeNum ,
        SettlementMethod ,
        Description ,
        PackageType ,
        PackageCodeStartTime ,
        PackageCodeEndTime ,
        IsActive ,
        BuyoutOrderId ,
        IsPackageCodeGenerated ,
        CooperateId ,
        CreateUser,
        UpdateUser
FROM    Tuhu_groupon..BeautyServicePackage WITH ( NOLOCK )
WHERE   PKID = @PackageId;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PackageId", packageId)
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, sqlParameters).ConvertTo<BeautyServicePackage>().FirstOrDefault();
            }
        }

        public static BeautyServicePackageDetail SelectBeautyServicePackageDetail(int packegeDetailId)
        {
            string sql = @"SELECT  PKID ,
        PackageId ,
        PID ,
        ServiceCodeTypeId ,
        Name ,
        VipSettlementPrice ,
        ShopCommission ,
        SettlementMethod ,
        Num ,
        ServiceCodeNum ,
        ServiceCodeStartTime ,
        ServiceCodeEndTime ,
        IsActive ,
        EffectiveDayAfterExchange ,
        IsServiceCodeGenerated ,
        BuyoutOrderId ,
        IsImportUser ,
        CooperateId ,
		CreateUser ,
        UpdateUser
FROM    Tuhu_groupon..BeautyServicePackageDetail WITH ( NOLOCK )
WHERE   PKID = @PackegeDetailId;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PackegeDetailId", packegeDetailId)
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, sqlParameters).ConvertTo<BeautyServicePackageDetail>().FirstOrDefault();
            }
        }
        /// <summary>
        /// 根据礼包详情id获取礼包详情
        /// </summary>
        /// <param name="packegeDetailIds"></param>
        /// <returns></returns>
        public static IEnumerable<BeautyServicePackageDetail> SelectBeautyServicePackageDetails(IEnumerable<int> packegeDetailIds)
        {
            string sql = @"SELECT  PKID ,
        PackageId ,
        PID ,
        ServiceCodeTypeId ,
        Name ,
        VipSettlementPrice ,
        ShopCommission ,
        SettlementMethod ,
        Num ,
        ServiceCodeNum ,
        ServiceCodeStartTime ,
        ServiceCodeEndTime ,
        IsActive ,
        EffectiveDayAfterExchange ,
        IsServiceCodeGenerated ,
        BuyoutOrderId ,
        CooperateId
FROM    Tuhu_groupon..BeautyServicePackageDetail AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@PackegeDetailIds, ',', 1) AS B ON A.PKID = B.Item;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PackegeDetailIds", string.Join(",", packegeDetailIds))
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, sqlParameters).ConvertTo<BeautyServicePackageDetail>();
            }
        }

        public static IEnumerable<BeautyServiceCodeTypeConfig> SelectBeautyServiceCodeTypeConfig()
        {
            string sql = @"
SELECT  PKID ,
        PID ,
        Name ,
        Description ,
        LogoUrl 
FROM    Tuhu_groupon..BeautyServiceCodeTypeConfig WITH ( NOLOCK )
WHERE   IsActive = 1";
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<BeautyServiceCodeTypeConfig>().ToList();
            }
        }

        public static Tuple<bool, int> InsertBeautyServicePackage(BeautyServicePackage package)
        {
            string sql = @"INSERT  INTO Tuhu_groupon..BeautyServicePackage
        ( VipUserId ,
          VipUserName ,
          VipCompanyId ,
          VipCompanyName ,
          PackageName ,
          Department ,
          Purpose ,
          PackageCodeNum ,
          SettlementMethod ,
          Description ,
          PackageType ,
          PackageCodeStartTime ,
          PackageCodeEndTime ,
          CreateUser ,
          CooperateId
        )
VALUES  ( @VipUserId ,
          @VipUserName ,
          @VipCompanyId ,
          @VipCompanyName ,
          @PackageName ,
          @Department ,
          @Purpose ,
          @PackageCodeNum ,
          @SettlementMethod ,
          @Description ,
          @PackageType ,
          @PackageCodeStartTime ,
          @PackageCodeEndTime ,
          @CreateUser ,
          @CooperateId
        );
        SELECT TOP 1
        @PackageId = A.PKID
FROM    Tuhu_groupon..BeautyServicePackage AS A WITH ( NOLOCK )
ORDER BY CreateTime DESC; 
";
            var parameters = new SqlParameter[]
           {
                new SqlParameter("@VipUserId",package.VipUserId),
                new SqlParameter("@VipUserName",package.VipUserName),
                new SqlParameter("@VipCompanyId",package.VipCompanyId),
                new SqlParameter("@VipCompanyName",package.VipCompanyName),
                new SqlParameter("@PackageName",package.PackageName),
                new SqlParameter("@Department",package.Department),
                new SqlParameter("@Purpose",package.Purpose),
                new SqlParameter("@PackageCodeNum",package.PackageCodeNum),
                new SqlParameter("@SettlementMethod",package.SettlementMethod),
                new SqlParameter("@Description",package.Description),
                new SqlParameter("@PackageType",package.PackageType),
                new SqlParameter("@PackageCodeStartTime",package.PackageCodeStartTime),
                new SqlParameter("@PackageCodeEndTime",package.PackageCodeEndTime),
                new SqlParameter("@CreateUser",package.CreateUser),
                new SqlParameter("@CooperateId",package.CooperateId),
                new SqlParameter("@PackageId", SqlDbType.Int){ Direction=ParameterDirection.Output}
           };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return new Tuple<bool, int>(dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0, 
                    Convert.ToInt32(parameters.Last().Value));
            }
        }

        public static bool UpdateBeautyServicePackage(SqlDbHelper dbHelper, BeautyServicePackage package)
        {
            string sql = @"UPDATE  Tuhu_groupon..BeautyServicePackage
SET     VipUserId = @VipUserId ,
        VipUserName = @VipUserName ,
        VipCompanyId = @VipCompanyId ,
        VipCompanyName = @VipCompanyName ,
        PackageName = @PackageName ,
        Department = @Department ,
        Purpose = @Purpose ,
        PackageCodeNum = @PackageCodeNum ,
        SettlementMethod = @SettlementMethod ,
        Description = @Description ,
        PackageType = @PackageType ,
        PackageCodeStartTime = @PackageCodeStartTime ,
        PackageCodeEndTime = @PackageCodeEndTime ,
        UpdateTime = GETDATE() ,
        UpdateUser = @UpdateUser ,
        IsActive = @IsActive ,
        CooperateId = @CooperateId
WHERE   PKID = @PKID;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKID",package.PKID),
                new SqlParameter("@VipUserId",package.VipUserId),
                new SqlParameter("@VipUserName",package.VipUserName),
                new SqlParameter("@VipCompanyId",package.VipCompanyId),
                new SqlParameter("@VipCompanyName",package.VipCompanyName),
                new SqlParameter("@PackageName",package.PackageName),
                new SqlParameter("@Department",package.Department),
                new SqlParameter("@Purpose",package.Purpose),
                new SqlParameter("@PackageCodeNum",package.PackageCodeNum),
                new SqlParameter("@SettlementMethod",package.SettlementMethod),
                new SqlParameter("@Description",package.Description),
                new SqlParameter("@PackageType",package.PackageType),
                new SqlParameter("@PackageCodeStartTime",package.PackageCodeStartTime),
                new SqlParameter("@PackageCodeEndTime",package.PackageCodeEndTime),
                new SqlParameter("@UpdateUser",package.UpdateUser),
                new SqlParameter("@IsActive", package.IsActive),
                new SqlParameter("@CooperateId", package.CooperateId)
            };

            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }
        /// <summary>
        /// 根据兑换码配置id更新服务码配置的合作ID
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="packageId"></param>
        /// <param name="cooperateId"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServicePackageDetailCooperateIdByPackageId(SqlDbHelper dbHelper, int packageId, int cooperateId)
        {
            var sql = @"UPDATE  Tuhu_groupon..BeautyServicePackageDetail
SET     CooperateId = @CooperateId
WHERE   PackageId = @PackageId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CooperateId",cooperateId),
                new SqlParameter("@PackageId",packageId),
            };

            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }
        /// <summary>
        /// 根据PackagId更新兑换码起始时间
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="packageId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServicePackageCodeTime(SqlDbHelper dbHelper, int packageId, DateTime? startTime, DateTime? endTime)
        {
            var sql = @"UPDATE  Tuhu_groupon..BeautyServicePackageCode
SET     StartTime = @StartTime ,
        EndTime = @EndTime ,
        UpdateTime = GETDATE()
WHERE   PackageId = @PackageId;";
            var parameters = new SqlParameter[] {
                new SqlParameter("@StartTime",startTime),
                new SqlParameter("@EndTime", endTime),
                new SqlParameter("@PackageId", packageId)
            };

            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }


        public static Tuple<bool, int> InsertBeautyServicePackageDetail(BeautyServicePackageDetail packageDetail)
        {
            string sql = @"INSERT  INTO Tuhu_groupon..BeautyServicePackageDetail
        ( PackageId ,
          PID ,
          ServiceCodeTypeId ,
          Name ,
          VipSettlementPrice ,
          ShopCommission ,
          SettlementMethod ,
          Num ,
          ServiceCodeNum ,
          IsActive ,
          ServiceCodeStartTime ,
          ServiceCodeEndTime ,
          EffectiveDayAfterExchange ,
          IsImportUser ,
          CreateUser ,
          CooperateId
        )
VALUES  ( @PackageId ,
          @PID ,
          @ServiceCodeTypeId ,
          @Name ,
          @VipSettlementPrice ,
          @ShopCommission ,
          @SettlementMethod ,
          @Num ,
          @ServiceCodeNum ,
          @IsActive ,
          @ServiceCodeStartTime ,
          @ServiceCodeEndTime ,
          @EffectiveDayAfterExchange ,
          @IsImportUser ,
          @CreateUser ,
          @CooperateId
        );
        SELECT TOP 1
        @PackageDetailId = A.PKID
FROM    Tuhu_groupon..BeautyServicePackageDetail AS A WITH ( NOLOCK )
ORDER BY CreateTime DESC; 
";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PackageId",packageDetail.PackageId),
                new SqlParameter("@PID",packageDetail.PID),
                new SqlParameter("@ServiceCodeTypeId",packageDetail.ServiceCodeTypeId),
                new SqlParameter("@Name",packageDetail.Name),
                new SqlParameter("@VipSettlementPrice",packageDetail.VipSettlementPrice),
                new SqlParameter("@ShopCommission",packageDetail.ShopCommission),
                new SqlParameter("@SettlementMethod",packageDetail.SettlementMethod),
                new SqlParameter("@Num",packageDetail.Num),
                new SqlParameter("@ServiceCodeNum",packageDetail.ServiceCodeNum),
                new SqlParameter("@IsActive",packageDetail.IsActive),
                new SqlParameter("@ServiceCodeStartTime",packageDetail.ServiceCodeStartTime),
                new SqlParameter("@ServiceCodeEndTime",packageDetail.ServiceCodeEndTime),
                new SqlParameter("@EffectiveDayAfterExchange",packageDetail.EffectiveDayAfterExchange),
                new SqlParameter("@IsImportUser", packageDetail.IsImportUser),
                new SqlParameter("@CreateUser", packageDetail.CreateUser),
                new SqlParameter("@CooperateId", packageDetail.CooperateId) ,
                new SqlParameter("@PackageDetailId", SqlDbType.Int){ Direction=ParameterDirection.Output}
            };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return new Tuple<bool, int>(dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0, 
                    Convert.ToInt32(parameters.Last().Value));
            }
        }

        public static bool UpdateBeautyServicePackageDetail(SqlDbHelper dbHelper, BeautyServicePackageDetail packageDetail)
        {
            string sql = @"UPDATE  Tuhu_groupon..BeautyServicePackageDetail
SET     PID = @PID ,
        ServiceCodeTypeId = @ServiceCodeTypeId ,
        Name = @Name ,
        VipSettlementPrice = @VipSettlementPrice ,
        ShopCommission = @ShopCommission ,
        SettlementMethod = @SettlementMethod ,
        Num = @Num ,
        ServiceCodeNum = @ServiceCodeNum ,
        IsActive = @IsActive ,
        ServiceCodeStartTime = @ServiceCodeStartTime ,
        ServiceCodeEndTime = @ServiceCodeEndTime ,
        EffectiveDayAfterExchange = @EffectiveDayAfterExchange ,
        IsImportUser = @IsImportUser ,
        UpdateUser = @UpdateUser,
		CooperateId=@CooperateId
WHERE   PKID = @PKID;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKID",packageDetail.PKID),
                new SqlParameter("@PID",packageDetail.PID),
                new SqlParameter("@ServiceCodeTypeId",packageDetail.ServiceCodeTypeId),
                new SqlParameter("@Name",packageDetail.Name),
                new SqlParameter("@VipSettlementPrice",packageDetail.VipSettlementPrice),
                new SqlParameter("@ShopCommission",packageDetail.ShopCommission),
                new SqlParameter("@SettlementMethod",packageDetail.SettlementMethod),
                new SqlParameter("@Num",packageDetail.Num),
                new SqlParameter("@ServiceCodeNum",packageDetail.ServiceCodeNum),
                new SqlParameter("@IsActive",packageDetail.IsActive),
                new SqlParameter("@ServiceCodeStartTime",packageDetail.ServiceCodeStartTime),
                new SqlParameter("@ServiceCodeEndTime",packageDetail.ServiceCodeEndTime),
                new SqlParameter("@EffectiveDayAfterExchange",packageDetail.EffectiveDayAfterExchange),
                new SqlParameter("@IsImportUser", packageDetail.IsImportUser) ,
                new SqlParameter("@UpdateUser", packageDetail.UpdateUser),
                new SqlParameter("@CooperateId", packageDetail.CooperateId)
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }
        /// <summary>
        /// 根据packageDetailId更新服务码起始时间
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="packageDetailId"></param>
        /// <param name="StartTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool UpdateServiceCodeTime(SqlDbHelper dbHelper, int packageDetailId, DateTime? StartTime, DateTime? endTime)
        {
            var sql = @"UPDATE  Tuhu_groupon..BeautyServicePackageDetailCode
SET     StartTime = @StartTime ,
        EndTime = @EndTime
WHERE   PackageDetailId = @PackageDetailId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PackageDetailId",packageDetailId),
                new SqlParameter("@StartTime",StartTime),
                new SqlParameter("@EndTime",endTime)
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }

        public static bool DeleteBeautyServicePackageDetail(int pkid)
        {
            var sql = @"DELETE Tuhu_groupon..BeautyServicePackageDetail WHERE PKID=@PKID";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKID",pkid)
            };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
            }
        }

        public static bool DeleteBeautyServicePackage(int pkid)
        {
            var sql = @"DELETE Tuhu_groupon..BeautyServicePackage WHERE PKID=@PKID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKID",pkid)
            };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
            }
        }

        public static IEnumerable<BeautyServiceCodeTypeConfig> SelectAllBeautyServiceCodeTypeConfig()
        {
            var sql = @"SELECT  PKID ,
        PID ,
        Name ,
        Description ,
        IsActive ,
        LogoUrl ,
        AdapterVehicle 
FROM    Tuhu_groupon.dbo.BeautyServiceCodeTypeConfig WITH ( NOLOCK )
ORDER BY CreateTime DESC";
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<BeautyServiceCodeTypeConfig>().ToList();
            }

        }

        public static bool InsertBeautyServiceCodeTypeConfig(BeautyServiceCodeTypeConfig config)
        {
            var sql = @"INSERT INTO Tuhu_groupon..BeautyServiceCodeTypeConfig
        ( PID ,
          Name ,
          Description ,
          IsActive ,
          LogoUrl ,
          AdapterVehicle 
        )
VALUES  ( @PID , 
          @Name , 
          @Description , 
          @IsActive ,
          @LogoUrl ,
          @AdapterVehicle 
        )";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PID",config.PID),
                new SqlParameter("@Name",config.Name),
                new SqlParameter("@Description",config.Description),
                new SqlParameter("@IsActive",config.IsActive),
                new SqlParameter("@LogoUrl",config.LogoUrl),
                new SqlParameter("@AdapterVehicle",config.AdapterVehicle)
            };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
            }
        }
        /// <summary>
        /// 更新服务码配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServiceCodeTypeConfig(BeautyServiceCodeTypeConfig config)
        {
            var sql = @"UPDATE  Tuhu_groupon..BeautyServiceCodeTypeConfig
SET     PID = @PID ,
        Name = @Name ,
        Description = @Description ,
        IsActive = @IsActive ,
        UpdateTime = GETDATE(),
        LogoUrl=@LogoUrl  ,
        AdapterVehicle=@AdapterVehicle  
WHERE   PKID = @PKID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKID", config.PKID),
                new SqlParameter("@PID",config.PID),
                new SqlParameter("@Name",config.Name),
                new SqlParameter("@Description",config.Description),
                new SqlParameter("@IsActive",config.IsActive),
                new SqlParameter("@LogoUrl",config.LogoUrl),
                new SqlParameter("@AdapterVehicle",config.AdapterVehicle)
            };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
            }
        }

        public static int InsertBeautyServicePackageDetailCodes(SqlDbHelper dbHelper, BeautyServicePackageDetail packageDetail, Guid vipUserId, IEnumerable<string> serviceCodes)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..BeautyServicePackageDetailCode
        ( UserId ,
          PackageDetailId ,
          ServiceCode ,
          IsActive ,
          StartTime ,
          EndTime ,
          PackageCode ,
          VipUserId
        )
        SELECT  @UserId ,
                @PackageDetailId ,
                C.Item ,
                1 ,
                @StartTime ,
                @EndTime ,
                @PackageCode ,
                @VipUserId
        FROM    Tuhu_groupon.dbo.SplitString(@ServiceCodes, ',' , 1) AS C;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId",null),
                new SqlParameter("@PackageDetailId",packageDetail.PKID),
                new SqlParameter("@StartTime",packageDetail.ServiceCodeStartTime),
                new SqlParameter("@EndTime",packageDetail.ServiceCodeEndTime),
                new SqlParameter("@PackageCode",null),
                new SqlParameter("@VipUserId",vipUserId),
                new SqlParameter("@ServiceCodes",string.Join(",", serviceCodes))
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }
        public static int InsertBeautyServicePackageCodes(SqlDbHelper dbHelper, BeautyServicePackage package, int count)
        {
            var sql = @"
DECLARE @BatchSize INT= 1000;
DECLARE @InsertedCount INT;
DECLARE @CDKeys TABLE
    (
      PKID INT IDENTITY(1, 1)
               PRIMARY KEY ,
      CDKey NVARCHAR(16)
    ); 
SET @GenerateCount=0;
DECLARE @Inserted TABLE ( CDKey NVARCHAR(16) ); 
DECLARE @i INT;           
DECLARE @cdKey VARCHAR(16)= '';
WHILE @Count > 0
    BEGIN TRY 
        DECLARE @Index INT; 
        SET @Index = 1;
			  
        WHILE @Index <= @Count
            AND @Index <= @BatchSize
            BEGIN
                SET @i = 1;
                SET @cdKey = '';
                WHILE @i < 6
                    BEGIN
                        BEGIN
                            SELECT  @cdKey = @cdKey + CHAR(65 + CEILING(RAND()
                                                              * 25));
                        END;
                        SET @i = @i + 1;
                    END;  
                SET @i = 1;

                WHILE @i < 6
                    BEGIN

                        SELECT  @cdKey = @cdKey
                                + CAST(CEILING(RAND() * 9) AS VARCHAR(1));
                        SET @i = @i + 1;
                    END;	 
                INSERT  INTO @CDKeys
                        ( CDKey )
                VALUES  ( @cdKey );
                SET @Index += 1;
            END;
        INSERT  INTO Tuhu_groupon..BeautyServicePackageCode
                ( UserId ,
                  PackageId ,
                  PackageCode ,
                  IsActive ,
                  StartTime ,
                  EndTime ,
                  CreateTime ,
                  UpdateTime ,
                  IsExchange ,
                  OrderId ,
                  VipUserId
                )
        OUTPUT  Inserted.PackageCode
                INTO @Inserted
                SELECT  @UserId ,
                        @PackageId ,
                        v.CDKey ,
                        1 ,
                        @StartTime ,
                        @EndTime ,
                        GETDATE() ,
                        NULL ,
                        0 ,
                        @OrderId ,
                        @VipUserId
                FROM    @CDKeys AS v;

        SELECT  @InsertedCount = COUNT(1)
        FROM    @Inserted;
        SET @GenerateCount+=@InsertedCount;

        DELETE  @CDKeys;
        DELETE  @Inserted;
        SET @Count -= @InsertedCount;
    END TRY
    BEGIN CATCH
        SELECT  @InsertedCount = COUNT(1)
        FROM    @Inserted;
        SET @GenerateCount+=@InsertedCount;
        DELETE  @CDKeys;
        DELETE  @Inserted;
        SET @Count -= @InsertedCount;
    END CATCH;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId",null),
                new SqlParameter("@PackageId",package.PKID),
                new SqlParameter("@StartTime",package.PackageCodeStartTime),
                new SqlParameter("@EndTime",package.PackageCodeEndTime),
                new SqlParameter("@OrderId",null),
                new SqlParameter("@VipUserId",package.VipUserId),
                new SqlParameter("@Count",count),
                new SqlParameter("@GenerateCount",SqlDbType.Int){ Direction=ParameterDirection.Output}
            };
            var result = dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters);
            return result > 0 ? Convert.ToInt32(parameters[7].Value) : 0;
        }

        public static IEnumerable<BeautyServicePackageCode> SelectBeautyServicePackageCodesByPackageId(int packageId)
        {
            var sql = @"SELECT  UserId ,
        PackageId ,
        PackageCode ,
        IsActive ,
        StartTime ,
        EndTime ,
        IsExchange ,
        ExchangeTime ,
        OrderId ,
        VipUserId
FROM    Tuhu_groupon..BeautyServicePackageCode (NOLOCK)
WHERE   PackageId = @PackageId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PackageId",packageId),
            };

            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BeautyServicePackageCode>().ToList();
            }
        }

        public static IEnumerable<BeautyServicePackageDetailCode> SelectBeautyServicePackageDetailCodesByPackageDetailId(int packageDetailId)
        {
            var sql = @"SELECT  UserId ,
        PackageDetailId ,
        ServiceCode ,
        IsActive ,
        StartTime ,
        EndTime ,
        PackageCode ,
        VipUserId
FROM    Tuhu_groupon..BeautyServicePackageDetailCode (NOLOCK)
WHERE   PackageDetailId = @PackageDetailId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PackageDetailId",packageDetailId),
            };

            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BeautyServicePackageDetailCode>().ToList();
            }
        }
        /// <summary>
        /// 根据用户查询服务码信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<BeautyServicePackageDetailCode>, int> SelectBeautyServicePackageDetailCodesByUserId(Guid userId, int pageIndex, int pageSize)
        {
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Tuhu_groupon..BeautyServicePackageDetailCode AS b WITH ( NOLOCK )
                        WHERE   UserId = @UserId;
                        SELECT  PKID ,
                                UserId ,
                                PackageDetailId ,
                                ServiceCode ,
                                IsActive ,
                                StartTime ,
                                EndTime ,
                                PackageCode ,
                                VipUserId
                        FROM    Tuhu_groupon..BeautyServicePackageDetailCode (NOLOCK)
                        WHERE   UserId=@UserId
                        ORDER BY PKID DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total",SqlDbType.Int){ Direction=ParameterDirection.Output}
            };

            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                var packageDetails = dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BeautyServicePackageDetailCode>().ToList();
                var totalCount = Convert.ToInt32(parameters.Last().Value);
                return new Tuple<IEnumerable<BeautyServicePackageDetailCode>, int>(packageDetails, totalCount);
            }
        }
        /// <summary>
        /// 批量根据服务码获取服务码信息
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static IEnumerable<BeautyServicePackageDetailCode> SelectBeautyServicePackageDetailCodesByCodes(IEnumerable<string> codes)
        {
            var sql = @"SELECT  PKID ,
                                UserId ,
                                PackageDetailId ,
                                ServiceCode ,
                                IsActive ,
                                StartTime ,
                                EndTime ,
                                PackageCode ,
                                VipUserId
                        FROM    Tuhu_groupon..BeautyServicePackageDetailCode AS A ( NOLOCK )
                                JOIN Tuhu_groupon..SplitString(@Codes, ',', 1) AS B ON A.ServiceCode = B.Item;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Codes", string.Join(",",codes)),
            };

            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BeautyServicePackageDetailCode>().ToList();
            }
        }

        public static bool SetPackageCodeIsGenerated(SqlDbHelper dbHelper, int packageId)
        {
            var sql = @"
UPDATE  Tuhu_groupon..BeautyServicePackage
SET     IsPackageCodeGenerated = 1
WHERE   PKID = @PackageId";
            var parameters = new SqlParameter[]
             {
                new SqlParameter("@PackageId",packageId),
             };

            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }

        public static bool SetPackageBuyoutOrderId(SqlDbHelper dbHelper, int packageId, int orderId)
        {
            var sql = @"
UPDATE  Tuhu_groupon..BeautyServicePackage
SET     BuyoutOrderId = @BuyoutOrderId
WHERE   PKID = @PackageId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PackageId", packageId),
                new SqlParameter("@BuyoutOrderId", orderId)
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }

        public static bool SetPackageDetailBuyoutOrderId(SqlDbHelper dbHelper, int packageDetailId, int orderId)
        {
            var sql = @"
UPDATE  Tuhu_groupon..BeautyServicePackageDetail
SET     BuyoutOrderId = @BuyoutOrderId
WHERE   PKID = @PackageDetailId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PackageDetailId", packageDetailId),
                new SqlParameter("@BuyoutOrderId", orderId)
            };

            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }
        public static bool SetServiceCodeIsGenerated(SqlDbHelper dbHelper, int packageDetailId)
        {
            var sql = @"UPDATE  Tuhu_groupon..BeautyServicePackageDetail
SET     IsServiceCodeGenerated = 1
WHERE   PKID = @PackageDetailId";
            var parameters = new SqlParameter[]
             {
                new SqlParameter("@PackageDetailId", packageDetailId),
             };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }
        /// <summary>
        /// 根据pid查询美容产品信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static BeautyProductModel SelectBeautyProductByPid(string pid)
        {
            var sql = @"SELECT  SP.PId AS PKID ,
        SP.ProdcutId AS PID ,
        SP.ProdcutName AS ProductName,
        SP.CategoryIds AS CategoryId ,
        SC.CategoryName ,
        SP.Describe AS Description ,
        SP.AdaptiveCar AS RestrictVehicleType ,
        SP.Commission
FROM    Tuhu_Groupon..SE_MDBeautyCategoryProductConfig AS SP WITH ( NOLOCK )
        JOIN Tuhu_Groupon..SE_MDBeautyCategoryConfig AS SC WITH ( NOLOCK ) ON SC.Id = SP.CategoryIds
WHERE SP.ProdcutId=@ProdcutId AND SP.IsDisable=0";
            var parameters = new SqlParameter[]
            {
               new SqlParameter("@ProdcutId", pid),
            };

            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BeautyProductModel>().FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取当前分类的子分类（包含自身）
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static IEnumerable<int> SelectBeautyChildAndSelfCategoryIdsByCategoryId(int categoryId)
        {
            var sql = @"SELECT Childs
                         FROM   Tuhu_Groupon..SE_MDBeautyCategoryConfigForChilds(@CategoryId)";
            var parameters = new SqlParameter[]
            {
               new SqlParameter("@CategoryId", categoryId),
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                var dt = dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters);
                List<int> categoryIds = new List<int>();
                if (dt != null && dt.AsEnumerable().Any())
                {
                    var row = dt.AsEnumerable().FirstOrDefault();
                    if (row != null)
                    {
                        var childStrs = row.IsNull("Childs") ? string.Empty : row["Childs"].ToString();
                        if (!string.IsNullOrEmpty(childStrs))
                        {
                            var categoryIdStrs = childStrs.Split(',').ToList();
                            foreach (var id in categoryIdStrs)
                            {
                                int idInt = 0;
                                if (Int32.TryParse(id, out idInt))
                                {
                                    if (idInt != categoryId)
                                    {
                                        categoryIds.Add(idInt);
                                    }
                                }
                            }
                        }
                    }

                }

                return categoryIds;
            }
        }

        /// <summary>
        /// 修改大客户美容服务码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServicePackageDetailCodes(SqlDbHelper dbhelper, BeautyServicePackageDetailCode model)
        {
            var sql = @"UPDATE  Tuhu_groupon..BeautyServicePackageDetailCode
                        SET     StartTime = @StartTime ,
                                EndTime = @EndTime ,
                                UpdateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                cmd.Parameters.AddWithValue("@PKID", model.PKID);
                return dbhelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        /// 根据PKID获取美容服务码详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static BeautyServicePackageDetailCode GetBeautyServicePackageDetailCodeByPKID(int pkid)
        {
            var sql = @"SELECT  s.PKID ,
                                s.UserId ,
                                s.PackageDetailId ,
                                s.ServiceCode ,
                                s.IsActive ,
                                s.StartTime ,
                                s.EndTime ,
                                s.PackageCode ,
                                s.VipUserId
                        FROM    Tuhu_groupon..BeautyServicePackageDetailCode AS s WITH ( NOLOCK )
                        WHERE   s.PKID = @PKID;";
            var parameters = new SqlParameter[]
            {
                 new SqlParameter("@PKID",pkid)
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BeautyServicePackageDetailCode>().FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取限购配置
        /// </summary>
        /// <param name="packageDetailId"></param>
        /// <returns></returns>
        public static BeautyServicePackageLimitConfig SelectBeautyServicePackageLimitConfigByPackageDetailId(int packageDetailId)
        {
            var sql = @"SELECT TOP 1
        A.PKID ,
        A.PackageDetailId ,
        A.CycleType ,
        A.CycleLimit ,
        A.ProvinceIds ,
        A.CityIds ,
        A.CreateTime ,
        A.UpdateTime
FROM    Tuhu_groupon..BeautyServicePackageLimitConfig AS A WITH ( NOLOCK )
WHERE   A.PackageDetailId = @PackageDetailId
ORDER BY CreateTime DESC; ";
            var parameters = new SqlParameter[]
            {
                 new SqlParameter("@PackageDetailId",packageDetailId)
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<BeautyServicePackageLimitConfig>().FirstOrDefault();
            }
        }
        /// <summary>
        /// 插入限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool InsertBeautyServicePackageLimitConfig(BeautyServicePackageLimitConfig config)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..BeautyServicePackageLimitConfig
        ( PackageDetailId ,
          CycleType ,
          CycleLimit ,
          ProvinceIds ,
          CityIds 
        )
VALUES  ( @PackageDetailId ,
          @CycleType ,
          @CycleLimit ,
          @ProvinceIds ,
          @CityIds 
        );";
            var parameters = new SqlParameter[]
            {
                 new SqlParameter("@PackageDetailId",config.PackageDetailId),
                 new SqlParameter("@CycleType", config.CycleType),
                 new SqlParameter("@CycleLimit", config.CycleLimit),
                 new SqlParameter("@ProvinceIds", config.ProvinceIds),
                 new SqlParameter("@CityIds", config.CityIds)
            };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
            }
        }
        /// <summary>
        /// 更新限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServicePackageLimitConfig(BeautyServicePackageLimitConfig config)
        {
            var sql = @" UPDATE  Tuhu_groupon..BeautyServicePackageLimitConfig
    SET     CycleType = @CycleType ,
            CycleLimit = @CycleLimit ,
            ProvinceIds = @ProvinceIds ,
            CityIds = @CityIds ,
            UpdateTime = GETDATE()
    WHERE   PackageDetailId = @PackageDetailId;";
            var parameters = new SqlParameter[]
            {
                 new SqlParameter("@PackageDetailId",config.PackageDetailId),
                 new SqlParameter("@CycleType", config.CycleType),
                 new SqlParameter("@CycleLimit", config.CycleLimit),
                 new SqlParameter("@ProvinceIds", config.ProvinceIds),
                 new SqlParameter("@CityIds", config.CityIds)
            };
            using (var dbhelper = new SqlDbHelper(writeConnStr))
            {
                return dbhelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
            }
        }
        /// <summary>
        /// 根据批次号修改服务码起始时间
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="batchCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServicePackageDetailCodeTimeByBatchCode(SqlDbHelper dbHelper, string batchCode, DateTime startTime,
            DateTime endTime)
        {
            var sql = @"UPDATE  Tuhu_groupon..BeautyServicePackageDetailCode
SET     StartTime = @StartTime ,
        EndTime = @EndTime
WHERE   ImportBatchCode = @BatchCode;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@StartTime", startTime),
                new SqlParameter("@EndTime",endTime),
                new SqlParameter("@BatchCode", batchCode)
            };

            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }

        public static IEnumerable<string> SelectServiceCodesByBatchCode(string batchCode)
        {
            var result = new List<string>();
            var sql = @"
SELECT  A.ServiceCode
FROM    Tuhu_groupon..BeautyServicePackageDetailCode AS A WITH ( NOLOCK )
WHERE   A.ImportBatchCode = @BatchCode;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BatchCode", batchCode)
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                var dt = dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = row.IsNull("ServiceCode") ? null : row["ServiceCode"].ToString();
                        if (!string.IsNullOrEmpty(item))
                            result.Add(item);
                    }
                }
            }

            return result;
        }

        public static IEnumerable<CreateBeautyCodeTaskModel> SelectCreateBeautyCodeTaskModels(string batchCode)
        {
            var sql = @"SELECT  A.PKID ,
        A.MobileNumber ,
        A.Quantity ,
        A.StartTime ,
        A.EndTime ,
        A.MappingId ,
        A.Type ,
        A.Status ,
        A.BatchCode ,
        A.CreateUser ,
        A.CreateDateTime ,
        A.LastUpdateDateTime ,
        A.Source ,
        A.BuyoutOrderId ,
        A.UserId
FROM    Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
WHERE   A.BatchCode = @BatchCode
ORDER BY PKID DESC; ";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BatchCode", batchCode)
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<CreateBeautyCodeTaskModel>();
            }
        }


        public static List<Guid> GetEnterpriseUserUserId(int pageIndex, int pageSize, Guid userId, out int total)
        {
            var result = new List<Guid>();
            var sql = @"SELECT @Total= COUNT(DISTINCT UserId)
            FROM Tuhu_groupon..EnterpriseUserBeautyConfig WITH(NOLOCK )
            WHERE(@UserId = '00000000-0000-0000-0000-000000000000'
                      OR @UserId IS NULL
                      OR UserId = @UserId
                    )

             SELECT DISTINCT UserId
            FROM    Tuhu_groupon..EnterpriseUserBeautyConfig WITH(NOLOCK)
            WHERE(@UserId = '00000000-0000-0000-0000-000000000000'
                      OR @UserId IS NULL
                      OR UserId = @UserId
                    )
            ORDER BY UserId DESC
                    OFFSET(@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize
                    ROWS ONLY; ";
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@UserId",userId),
                new SqlParameter("@Total",SqlDbType.Int){ Direction=ParameterDirection.Output}
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                var dt = dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add((Guid)row["UserId"]);
                    }
                }
                total = Convert.ToInt32(parameters.Last().Value);
                return result;
            }
        }

        public static IEnumerable<EnterpriseUserBeautyConfig> GetEnterpriseUserServiceConfig(IEnumerable<Guid> userIdList)
        {
            var sql = @"SELECT  eusc.PKID ,
            eusc.UserId ,
            eusc.PackageDetailsId ,
            eusc.Remark ,
            eusc.CreateDateTime ,
            eusc.UpdateDateTime
    FROM    Tuhu_groupon..EnterpriseUserBeautyConfig AS eusc WITH ( NOLOCK )
    WHERE   EXISTS ( SELECT 1
                     FROM   Tuhu_groupon..SplitString(@UserIdStr, ',', 1) AS t
                     WHERE  t.Item = eusc.UserId );";
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserIdStr",string.Join(",",userIdList))
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<EnterpriseUserBeautyConfig>().ToList();
            }
        }

        public static IEnumerable<EnterpriseUserModuleConifg> GetEnterpriseUserModuleConfig(IEnumerable<Guid> userIdList)
        {
            var sql = @" SELECT  PKID ,
                    UserId ,
                    ModuleType ,
                    CreateDateTime ,
                    UpdateDateTime
            FROM    Tuhu_groupon..EnterpriseUserModuleConfig WITH ( NOLOCK )
            WHERE   EXISTS ( SELECT 1
                             FROM   Tuhu_groupon..SplitString(@UserIdStr, ',', 1) AS t
                             WHERE  t.Item = UserId )";
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserIdStr",string.Join(",",userIdList))
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<EnterpriseUserModuleConifg>().ToList();
            }
        }

        public static IEnumerable<CooperateUserService> GetCooperateUserServices(List<int> packageDetailIdList)
        {
            var sql = @" SELECT  A.PKID AS PackageDetailId ,
                    a.Name ,
                    a.CooperateId ,
					c.CooperateName,
                    C.VipUserId ,
                    A.SettlementMethod ,
                    A.PID AS ServiceId,
                    D.Name AS ServiceName
            FROM    Tuhu_groupon..BeautyServicePackageDetail AS A WITH ( NOLOCK )
                    LEFT JOIN Tuhu_groupon..BeautyServicePackage AS B WITH ( NOLOCK ) ON A.PackageId = B.PKID
                    LEFT JOIN Tuhu_groupon..MrCooperateUserConfig AS C WITH ( NOLOCK ) ON a.CooperateId = C.PKID
                    LEFT JOIN Tuhu_groupon..BeautyServiceCodeTypeConfig AS D
                    WITH ( NOLOCK ) ON A.ServiceCodeTypeId=d.PKID
            WHERE   ( B.PackageType = 'serviceCode'
                      OR A.PackageId = 0
                    )
                    AND A.IsImportUser = 1
					AND C.VipUserId IS NOT NULL
                    AND ( @DetailIdStr = ''
                        OR @DetailIdStr IS NULL
                        OR EXISTS ( SELECT    1
                                    FROM      Tuhu_groupon..SplitString(@DetailIdStr,
                                                            ',', 1) AS t
                                    WHERE     t.Item = A.PKID )
                    )";
            SqlParameter[] parameters =
            {
                new SqlParameter("@DetailIdStr",string.Join(",",packageDetailIdList))
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<CooperateUserService>().ToList();
            }
        }

        public static IEnumerable<CooperateUserService> SelectCooperateUserByUserIdList(List<Guid> userIdList)
        {
            var sql = @"   SELECT  A.PKID AS PackageDetailId ,
                        A.Name ,
                        A.CooperateId ,
                        C.CooperateName ,
                        C.VipUserId ,
                        A.SettlementMethod ,
                        A.PID AS ServiceId ,
                        D.Name AS ServiceName
                FROM    Tuhu_groupon..BeautyServicePackageDetail AS A WITH ( NOLOCK )
                        LEFT JOIN Tuhu_groupon..BeautyServicePackage AS B WITH ( NOLOCK ) ON A.PackageId = B.PKID
                        LEFT JOIN Tuhu_groupon..MrCooperateUserConfig AS C WITH ( NOLOCK ) ON A.CooperateId = C.PKID
                        LEFT JOIN Tuhu_groupon..BeautyServiceCodeTypeConfig AS D WITH ( NOLOCK ) ON A.ServiceCodeTypeId = D.PKID
                WHERE   ( B.PackageType = 'serviceCode'
                          OR A.PackageId = 0
                        )
                        AND A.IsImportUser = 1
                        AND C.VipUserId IS NOT NULL
                        AND ( @UserIdStr = ''
                              OR @UserIdStr IS NULL
                              OR EXISTS ( SELECT    1
                                          FROM      Tuhu_groupon..SplitString(@UserIdStr, ',',
                                                                              1) AS t
                                          WHERE     t.Item = C.VipUserId )
                            );";
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserIdStr",string.Join(",",userIdList))
            };
            using (var dbhelper = new SqlDbHelper(readConnStr))
            {
                return dbhelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<CooperateUserService>().ToList();
            }
        }

        public static IEnumerable<EnterpriseUserBeautyConfig> SelectEnterpriseUserConfigByUserId(SqlDbHelper dbHelper, Guid userId)
        {
            var sql = @"SELECT PKID,UserId,PackageDetailsId,Remark FROM  Tuhu_groupon..EnterpriseUserBeautyConfig WITH (NOLOCK) WHERE UserId=@UserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            return dbHelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<EnterpriseUserBeautyConfig>().ToList();
        }

        public static IEnumerable<EnterpriseUserModuleConifg> SelectEnterpriseUserModuleConfigByUserId(SqlDbHelper dbHelper, Guid userId)
        {
            var sql = @"SELECT  PKID ,
                    UserId ,
                    ModuleType ,
                    CreateDateTime ,
                    UpdateDateTime
            FROM    Tuhu_groupon..EnterpriseUserModuleConfig WITH ( NOLOCK ) WHERE UserId=@UserId";
            var parameters = new SqlParameter[]
{
                new SqlParameter("@UserId", userId)
};
            return dbHelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<EnterpriseUserModuleConifg>().ToList();

        }

        public static bool DeleteEnterpriseUserConfigByUserId(SqlDbHelper dbHelper, Guid userId)
        {
            var sql = @"DELETE FROM  Tuhu_groupon..EnterpriseUserBeautyConfig WHERE UserId=@UserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }

        public static bool DeleteEnterpriseModuleConfigByUserId(SqlDbHelper dbHelper, Guid userId)
        {
            var sql = @"DELETE FROM  Tuhu_groupon..EnterpriseUserModuleConfig WHERE UserId=@UserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }

        public static bool InsertEnterpriseUserConfig(SqlDbHelper dbHelper, Guid userId, int packageDetailsId, string remark)
        {
            var sql = @"INSERT INTO Tuhu_groupon..EnterpriseUserBeautyConfig
                    ( UserId ,
                      PackageDetailsId ,
                      Remark ,
                      CreateDateTime ,
                      UpdateDateTime
                    )
            VALUES  ( @UserId ,
                      @PackageDetailsId ,
                      @Remark ,
                      GETDATE() , 
                      GETDATE()  
                    )";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@PackageDetailsId",packageDetailsId),
                new SqlParameter("@Remark",remark)
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }

        public static bool InsertEnterpriseUserModuleConfig(SqlDbHelper dbHelper, Guid userId, string moduleType)
        {
            var sql = @"INSERT INTO Tuhu_groupon..EnterpriseUserModuleConfig
                    ( UserId ,
                      ModuleType ,
                      CreateDateTime ,
                      UpdateDateTime
                    )
            VALUES  ( @UserId ,
                      @ModuleType ,
                      GETDATE() , 
                      GETDATE()  
                    )";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ModuleType", moduleType)
            };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
        }
    }
}
