using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common.Extensions;
using Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO.GeneralBeautyServerCode
{
    public class DalGeneralBeautyServerCode
    {
        private static string strConn_Write = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string strConn_OnlyRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;
        public IEnumerable<ThirdPartyBeautyPackageConfigModel> GetGeneralBeautyServerCodeTmpList(int pageIndex, int pageSize, int cooperateId, string settlementMethod, out int count)
        {
            #region SQL
            string sql_count = @"SELECT COUNT(1) 
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageConfig] WITH(NOLOCK)
  WHERE IsDeleted=0 {0} {1}";
            string sql = @"SELECT [PKID]
      ,[PackageId]
      ,[IsActive]
      ,[CreatedUser]
      ,[CreatedDateTime]
      ,[UpdatedDateTime]
      ,[CooperateId]
      ,[PackageName]
      ,[Description]
      ,[SettlementMethod]
      ,[AppId] 
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageConfig] WITH(NOLOCK)
  WHERE IsDeleted=0  {0} {1} 
  Order By PKID DESC
  OFFSET @Begin ROWS FETCH NEXT @PageSize ROWS ONLY";
            #endregion
            var cooperCon = string.Empty;
            var settleCon = string.Empty;
            if (cooperateId > 0)
            {
                cooperCon = " and CooperateId=@cooperateId ";
            }
            if (!string.IsNullOrEmpty(settlementMethod))
            {
                settleCon = " and SettlementMethod=@settlementMethod ";
            }
            sql_count = string.Format(sql_count, cooperCon, settleCon);
            sql = string.Format(sql, cooperCon, settleCon);
            List<ThirdPartyBeautyPackageConfigModel> result = new List<ThirdPartyBeautyPackageConfigModel>();
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlCountPara = new SqlParameter[]
                {
                    new SqlParameter("cooperateId",cooperateId),
                    new SqlParameter("settlementMethod",settlementMethod),
                };
                count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql_count, sqlCountPara));
                if (count > 0)
                {
                    var sqlPara = new[]
                    {
                        new SqlParameter("Begin",(pageIndex-1)*pageSize),
                        new SqlParameter("PageSize",pageSize),
                        new SqlParameter("cooperateId",cooperateId),
                        new SqlParameter("settlementMethod",settlementMethod),
                    };
                    var datetable = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPara);
                    if (datetable != null)
                        foreach (DataRow row in datetable.Rows)
                        {
                            result.Add(new ThirdPartyBeautyPackageConfigModel
                            {
                                PKID = row.GetValue<Int32>("PKID"),
                                AppId = row.GetValue<String>("AppId"),
                                PackageName = row.GetValue<String>("PackageName"),
                                PackageId = row.GetValue<Guid>("PackageId"),
                                IsActive = row.GetValue<Boolean>("IsActive"),
                                CreatedUser = row.GetValue<String>("CreatedUser"),
                                CreatedDateTime = row.GetValue<DateTime>("CreatedDateTime"),
                                UpdatedDateTime = row.GetValue<DateTime>("UpdatedDateTime"),
                                CooperateId = row.GetValue<Int32>("CooperateId"),
                                Description = row.GetValue<String>("Description"),
                                SettlementMethod = row.GetValue<String>("SettlementMethod"),
                            });
                        }
                }
            }
            return result;
        }

        public ThirdPartyBeautyPackageConfigModel GetGeneralBeautyServerCodeTmpDetail(Guid packageId)
        {
            #region SQL
            string sql = @"SELECT [PKID]
      ,[PackageId]
      ,[IsActive]
      ,[CreatedUser]
      ,[CreatedDateTime]
      ,[UpdatedDateTime]
      ,[CooperateId]
      ,[PackageName]
      ,[Description]
      ,[SettlementMethod]
      ,[AppId] 
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageConfig] WITH(NOLOCK)
  WHERE PackageId=@PackageId";
            #endregion

            ThirdPartyBeautyPackageConfigModel result = null;
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlPara = new[]
                {
                        new SqlParameter("PackageId",packageId)
                    };
                var row = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, sqlPara);
                if (row != null)
                    result = new ThirdPartyBeautyPackageConfigModel
                    {
                        PKID = row.GetValue<Int32>("PKID"),
                        AppId = row.GetValue<String>("AppId"),
                        PackageName = row.GetValue<String>("PackageName"),
                        PackageId = row.GetValue<Guid>("PackageId"),
                        IsActive = row.GetValue<Boolean>("IsActive"),
                        CreatedUser = row.GetValue<String>("CreatedUser"),
                        CreatedDateTime = row.GetValue<DateTime>("CreatedDateTime"),
                        UpdatedDateTime = row.GetValue<DateTime>("UpdatedDateTime"),
                        CooperateId = row.GetValue<Int32>("CooperateId"),
                        Description = row.GetValue<String>("Description"),
                        SettlementMethod = row.GetValue<String>("SettlementMethod"),
                    };

            }
            return result;
        }
        public async Task<bool> SaveGeneralBeautyServerCodeTmpAsync(ThirdPartyBeautyPackageConfigModel model)
        {
            #region SQL
            string sql = @"
  Insert Into [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageConfig]
([PackageId]
,[IsActive]
,[IsDeleted]
,[CreatedUser]
,[CreatedDateTime]
,[UpdatedDateTime]
,[CooperateId]
,[PackageName]
,[AppId]
,[Description]
,[SettlementMethod])
values
(@PackageId
,@IsActive
,0
,@CreatedUser
,GetDate()
,GetDate()
,@CooperateId
,@PackageName
,@AppId
,@Description
,@SettlementMethod
)";
            #endregion
            using (var conn = new SqlConnection(strConn_Write))
            {
                var sqlPara = new[]
                {
                    new SqlParameter("PackageId",model.PackageId),
                    new SqlParameter("IsActive",model.IsActive),
                    new SqlParameter("CreatedUser",model.CreatedUser),
                    new SqlParameter("CooperateId",model.CooperateId),
                    new SqlParameter("PackageName",model.PackageName),
                    new SqlParameter("Description",model.Description),
                    new SqlParameter("AppId",model.AppId),
                    new SqlParameter("SettlementMethod",model.SettlementMethod)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPara) > 0;
            }
        }

        public async Task<bool> UpdateGeneralBeautyServerCodeTmpAsync(ThirdPartyBeautyPackageConfigModel model)
        {
            #region SQL
            string sql = @"
 Update  [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageConfig] WITH(ROWLOCK)
 SET 
 [IsActive]=@IsActive
,CooperateId=@CooperateId
,[UpdatedDateTime]=GetDate()
,[PackageName]=@PackageName
,[AppId]=@AppId
,[Description]=@Description 
WHERE PackageId=@PackageId";
            #endregion
            using (var conn = new SqlConnection(strConn_Write))
            {
                var sqlPara = new[]
                {
                    new SqlParameter("PackageId",model.PackageId),
                    new SqlParameter("IsActive",model.IsActive),
                    new SqlParameter("PackageName",model.PackageName),
                    new SqlParameter("AppId",model.AppId),
                    new SqlParameter("Description",model.Description),
                    new SqlParameter("CooperateId",model.CooperateId)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPara) > 0;
            }
        }
        public async Task<bool> DeleteGeneralBeautyServerCodeTmpAsync(Guid packageId)
        {
            #region SQL
            string sql = @"
 Update  [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageConfig] WITH(ROWLOCK)
 SET 
 [IsDeleted]=1
,[UpdatedDateTime]=GetDate()
WHERE PackageId=@PackageId";
            #endregion
            using (var conn = new SqlConnection(strConn_Write))
            {
                var sqlPara = new[]
                {
                    new SqlParameter("PackageId",packageId),
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPara) > 0;
            }
        }
        public async Task<IEnumerable<ThirdPartyBeautyPackageProductConfigModel>> GetGeneralBeautyServerCodeProductsListAsync(Guid packageId)
        {
            #region SQL
            string sql = @"SELECT [PKID]
      ,[CooperateId]
      ,[CodeTypeConfigId]
      ,[Name]
      ,[SettlementMethod]
      ,[SettlementPrice]
      ,[IsActive]
      ,[CreatedUser]
      ,[CreatedDateTime]
      ,[UpdatedDateTime]
      ,[Number]
      ,[PackageId]
      ,[Description]
      ,[ValidDate]
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageProductConfig] WITH(NOLOCK) 
  WHERE  PackageId=@pageageId AND IsDeleted=0";
            #endregion

            List<ThirdPartyBeautyPackageProductConfigModel> result = new List<ThirdPartyBeautyPackageProductConfigModel>();
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlPara = new[]
                {
                        new SqlParameter("pageageId",packageId)
                };
                var datetable = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPara);
                if (datetable != null)
                    foreach (DataRow row in datetable.Rows)
                    {
                        result.Add(new ThirdPartyBeautyPackageProductConfigModel
                        {
                            PKID = row.GetValue<Int32>("PKID"),
                            CooperateId = row.GetValue<Int32>("CooperateId"),
                            CodeTypeConfigId = row.GetValue<Int32>("CodeTypeConfigId"),
                            Name = row.GetValue<String>("Name"),
                            SettlementMethod = row.GetValue<String>("SettlementMethod"),
                            SettlementPrice = row.GetValue<decimal>("SettlementPrice"),
                            IsActive = row.GetValue<Boolean>("IsActive"),
                            CreatedUser = row.GetValue<String>("CreatedUser"),
                            CreatedDateTime = row.GetValue<DateTime>("CreatedDateTime"),
                            UpdatedDateTime = row.GetValue<DateTime>("UpdatedDateTime"),
                            Number = row.GetValue<int>("Number"),
                            PackageId = row.GetValue<Guid>("PackageId"),
                            Description = row.GetValue<String>("Description"),
                            ValidDate = row.GetValue<int>("ValidDate"),
                        });
                    }
            }
            return result;
        }
        public async Task<ThirdPartyBeautyPackageProductConfigModel> GetGeneralBeautyServerCodeProductsDetailAsync(int pkid)
        {
            #region SQL
            string sql = @"SELECT [PKID]
      ,[CooperateId]
      ,[CodeTypeConfigId]
      ,[Name]
      ,[SettlementMethod]
      ,[SettlementPrice]
      ,[IsActive]
      ,[CreatedUser]
      ,[CreatedDateTime]
      ,[UpdatedDateTime]
      ,[Number]
      ,[PackageId]
      ,[Description]
      ,[ValidDate]
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageProductConfig] WITH(NOLOCK) 
  WHERE PKID=@PKID";
            #endregion

            ThirdPartyBeautyPackageProductConfigModel result = null;
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlPara = new[]
                {
                        new SqlParameter("PKID",pkid)
                };
                var row = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, sqlPara);
                if (row != null)
                    result = new ThirdPartyBeautyPackageProductConfigModel
                    {
                        PKID = row.GetValue<Int32>("PKID"),
                        CooperateId = row.GetValue<Int32>("CooperateId"),
                        CodeTypeConfigId = row.GetValue<Int32>("CodeTypeConfigId"),
                        Name = row.GetValue<String>("Name"),
                        SettlementMethod = row.GetValue<String>("SettlementMethod"),
                        SettlementPrice = row.GetValue<decimal>("SettlementPrice"),
                        IsActive = row.GetValue<Boolean>("IsActive"),
                        CreatedUser = row.GetValue<String>("CreatedUser"),
                        CreatedDateTime = row.GetValue<DateTime>("CreatedDateTime"),
                        UpdatedDateTime = row.GetValue<DateTime>("UpdatedDateTime"),
                        Number = row.GetValue<int>("Number"),
                        PackageId = row.GetValue<Guid>("PackageId"),
                        Description = row.GetValue<String>("Description"),
                        ValidDate = row.GetValue<int>("ValidDate"),
                    };
            }
            return result;
        }
        public async Task<bool> SaveGeneralBeautyServerCodeProductsAsync(ThirdPartyBeautyPackageProductConfigModel model)
        {
            #region SQL
            string sql = @"INSERT INTO  [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageProductConfig] 
( [CooperateId]
 ,[CodeTypeConfigId]
 ,[Name]
 ,[SettlementMethod]
 ,[SettlementPrice]
 ,[IsActive]
 ,[IsDeleted]
 ,[CreatedUser]
 ,[CreatedDateTime]
 ,[UpdatedDateTime]
 ,[Number]
 ,[PackageId]
 ,[Description]
 ,[ValidDate])
VALUES(
  @CooperateId
 ,@CodeTypeConfigId
 ,@Name
 ,@SettlementMethod
 ,@SettlementPrice
 ,@IsActive
 ,0
 ,@CreatedUser
 ,GetDate()
 ,GetDate()
 ,@Number
 ,@PackageId
 ,@Description
 ,@ValidDate
)";
            #endregion

            IEnumerable<ThirdPartyBeautyPackageProductConfigModel> result = new List<ThirdPartyBeautyPackageProductConfigModel>();
            using (var conn = new SqlConnection(strConn_Write))
            {
                var sqlPara = new[]
                {
                    new SqlParameter("CooperateId",model.CooperateId),
                    new SqlParameter("CodeTypeConfigId",model.CodeTypeConfigId),
                    new SqlParameter("Name",model.Name),
                    new SqlParameter("SettlementMethod",model.SettlementMethod),
                    new SqlParameter("SettlementPrice",model.SettlementPrice),
                    new SqlParameter("IsActive",model.IsActive),
                    new SqlParameter("CreatedUser",model.CreatedUser),
                    new SqlParameter("Number",model.Number),
                    new SqlParameter("PackageId",model.PackageId),
                    new SqlParameter("Description",model.Description),
                    new SqlParameter("ValidDate",model.ValidDate),
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPara) > 0;

            }
        }

        public async Task<bool> UpdateGeneralBeautyServerCodeProductsAsync(ThirdPartyBeautyPackageProductConfigModel model)
        {
            #region SQL
            string sql = @"Update [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageProductConfig] WITH(ROWLOCK) 
SET
  [Name]=@Name
 ,[CodeTypeConfigId]=@CodeTypeConfigId
 ,[IsActive]=@IsActive
 ,[UpdatedDateTime]=GetDate()
 ,[Description]=@Description 
WHERE  PKID=@PKID";
            #endregion
            using (var conn = new SqlConnection(strConn_Write))
            {
                var sqlPara = new[]
                {
                    new SqlParameter("Name",model.Name),
                    new SqlParameter("IsActive",model.IsActive),
                    new SqlParameter("Description",model.Description),
                    new SqlParameter("PKID",model.PKID),
                    new SqlParameter("CodeTypeConfigId",model.CodeTypeConfigId),
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPara) > 0;

            }
        }
        public async Task<bool> DeleteGeneralBeautyServerCodeProductsAsync(int pkid)
        {
            #region SQL
            string sql = @"Update [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageProductConfig] WITH(ROWLOCK) 
SET
 [IsDeleted]=1
,[UpdatedDateTime]=GetDate()
WHERE  PKID=@PKID";
            #endregion
            using (var conn = new SqlConnection(strConn_Write))
            {
                var sqlPara = new[]
                {
                    new SqlParameter("PKID",pkid),
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPara) > 0;

            }
        }
        public IEnumerable<ThirdPartyBeautyPackageRecordModel> GetGeneralBeautyServerCodeSendRecords(Guid packageId, int pageIndex, int pageSize, int cooperateId, string settlementMethod, out int count)
        {
            #region SQL

            string sql_count = @"SELECT  COUNT(1) 
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageRecord] AS A WITH(NOLOCK)
  LEFT JOIN [Tuhu_groupon].[dbo].ThirdPartyBeautyPackageConfig  AS B WITH(NOLOCK)
   ON A.[PackageId]=B.[PackageId] AND B.IsDeleted=0 
   WHERE A.[Status]=1 And A.IsDeleted=0 And (A.PackageId=@packageId OR @packageId='00000000-0000-0000-0000-000000000000')  
		{0} {1} ";
            string sql = @"SELECT  A.[PKID]
      ,A.[PackageId]
      ,A.[Phone]
      ,A.[SerialNumber]
      ,A.[IsDeleted]
      ,A.[CreatedDateTime]
      ,A.[UpdatedDateTime]
      ,A.[Status]
      ,A.[UserId]
      ,A.[OrderId]
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageRecord] AS A WITH(NOLOCK)
  LEFT JOIN [Tuhu_groupon].[dbo].ThirdPartyBeautyPackageConfig  AS B WITH(NOLOCK)
   ON A.[PackageId]=B.[PackageId] AND B.IsDeleted=0 
   WHERE A.[Status]=1 And A.IsDeleted=0 And (A.PackageId=@packageId OR @packageId='00000000-0000-0000-0000-000000000000')  
				{0} {1}  
   Order By PKID DESC
  OFFSET @Begin ROWS FETCH NEXT @PageSize ROWS ONLY";
            #endregion
            var cooperCon = string.Empty;
            var settleCon = string.Empty;
            if (cooperateId > 0)
            {
                cooperCon = " and B.CooperateId=@cooperateId ";
            }
            if (!string.IsNullOrEmpty(settlementMethod))
            {
                settleCon = " and B.SettlementMethod=@settlementMethod ";
            }
            sql_count = string.Format(sql_count, cooperCon, settleCon);
            sql = string.Format(sql, cooperCon, settleCon);
            List<ThirdPartyBeautyPackageRecordModel> result = new List<ThirdPartyBeautyPackageRecordModel>();
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlCountPara = new SqlParameter[]
                {
                    new SqlParameter("packageId",packageId),
                    new SqlParameter("cooperateId",cooperateId),
                    new SqlParameter("settlementMethod",settlementMethod),
                };
                count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql_count, sqlCountPara));
                if (count > 0)
                {
                    var sqlPara = new[]
                    {
                        new SqlParameter("Begin",(pageIndex-1)*pageSize),
                        new SqlParameter("PageSize",pageSize),
                        new SqlParameter("packageId",packageId),
                        new SqlParameter("cooperateId",cooperateId),
                        new SqlParameter("settlementMethod",settlementMethod),
                    };
                    var datetable = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPara);
                    if (datetable != null)
                        foreach (DataRow row in datetable.Rows)
                        {
                            result.Add(new ThirdPartyBeautyPackageRecordModel
                            {
                                PKID = row.GetValue<Int32>("PKID"),
                                PackageId = row.GetValue<Guid>("PackageId"),
                                Phone = row.GetValue<String>("Phone"),
                                SerialNumber = row.GetValue<String>("SerialNumber"),
                                CreatedDateTime = row.GetValue<DateTime>("CreatedDateTime"),
                                UpdatedDateTime = row.GetValue<DateTime>("UpdatedDateTime"),
                                UserId = row.GetValue<Guid>("UserId"),
                                OrderId = row.GetValue<Int32>("OrderId"),
                            });
                        }
                }
            }
            return result;
        }

        public async Task<IEnumerable<ThirdPartyBeautyPackageRecordDetailModel>> GetGeneralBeautyServerCodeSendRecordDetail(int packageRecordId)
        {
            #region SQL
            string sql = @"SELECT [PKID]
      ,[PackageId]
      ,[ServiceCode]
      ,[IsActive]
      ,[CreatedDateTime]
      ,[UpdatedDateTime]
      ,[PackageProductId]
      ,[PackageRecordId]
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageRecordDetail] WITH(NOLOCK) 
  WHERE PackageRecordId=@packageRecordId";
            #endregion

            List<ThirdPartyBeautyPackageRecordDetailModel> result = new List<ThirdPartyBeautyPackageRecordDetailModel>();
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlPara = new[]
                {
                    new SqlParameter("packageRecordId",packageRecordId)
                };
                var datetable = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPara);
                if (datetable != null)
                    foreach (DataRow row in datetable.Rows)
                    {
                        result.Add(new ThirdPartyBeautyPackageRecordDetailModel
                        {
                            PKID = row.GetValue<Int32>("PKID"),
                            PackageId = row.GetValue<Guid>("PackageId"),
                            ServiceCode = row.GetValue<String>("ServiceCode"),
                            PackageProductId = row.GetValue<Int32>("PackageProductId"),
                            CreatedDateTime = row.GetValue<DateTime>("CreatedDateTime"),
                            UpdatedDateTime = row.GetValue<DateTime>("UpdatedDateTime"),
                            PackageRecordId = row.GetValue<Int32>("PackageRecordId"),
                        });
                    }

            }
            return result;
        }

        public async Task<ThirdPartyBeautyPackageRecordModel> GetGeneralBeautyServerCodeSendRecordById(int pkid)
        {
            #region SQL
            string sql = @"SELECT TOP  1 [PKID]
      ,[PackageId]
      ,[Phone]
      ,[SerialNumber]
      ,[CreatedDateTime]
      ,[UpdatedDateTime]
      ,[Status]
      ,[UserId]
      ,[OrderId]
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageRecord] WITH(NOLOCK) 
  WHERE PKID=@PKID";
            #endregion
            ThirdPartyBeautyPackageRecordModel result = null;
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlPara = new[]
                {
                        new SqlParameter("PKID",pkid),
                    };
                var dateRow = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, sqlPara);
                if (dateRow != null)
                    result = new ThirdPartyBeautyPackageRecordModel
                    {
                        PKID = dateRow.GetValue<Int32>("PKID"),
                        PackageId = dateRow.GetValue<Guid>("PackageId"),
                        Phone = dateRow.GetValue<String>("Phone"),
                        SerialNumber = dateRow.GetValue<String>("SerialNumber"),
                        CreatedDateTime = dateRow.GetValue<DateTime>("CreatedDateTime"),
                        UpdatedDateTime = dateRow.GetValue<DateTime>("UpdatedDateTime"),
                        UserId = dateRow.GetValue<Guid>("UserId"),
                        OrderId = dateRow.GetValue<Int32>("OrderId"),
                    };
            }
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<ThirdPartyBeautyPackageProductRegionConfigModel>> GetGeneralBeautyServerCodeProductRegions(int pkid)
        {
            #region SQL
            string sql = @"SELECT [PKID]
      ,[PackageProductId]
      ,[ProvinceId]
      ,[CityIds]
      ,[IsAllCitys]
      ,[IsDeleted]
      ,[CreatedDateTime]
      ,[UpdatedDateTime]
  FROM [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageRegion] WITH(NOLOCK) 
  WHERE [PackageProductId]=@packageProductId AND IsDeleted=0";
            #endregion
            List<ThirdPartyBeautyPackageProductRegionConfigModel> result = new List<ThirdPartyBeautyPackageProductRegionConfigModel>();
            using (var conn = new SqlConnection(strConn_OnlyRead))
            {
                var sqlPara = new[]
                {
                        new SqlParameter("packageProductId",pkid),
                    };
                var dateTable = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPara);
                if (dateTable != null)
                    foreach (DataRow row in dateTable.Rows)
                    {
                        result.Add(new ThirdPartyBeautyPackageProductRegionConfigModel
                        {
                            PKID = row.GetValue<Int32>("PKID"),
                            CityIds = row.GetValue<String>("CityIds"),
                            IsAllCitys = row.GetValue<Boolean>("IsAllCitys"),
                            PackageProductId = row.GetValue<Int32>("PackageProductId"),
                            ProvinceId = row.GetValue<Int32>("ProvinceId")
                        });
                    }
            }
            return await Task.FromResult(result);
        }


        public async Task<bool> SaveGeneralBeautyServerCodeProductRegions(IEnumerable<ThirdPartyBeautyPackageProductRegionConfigModel> models)
        {
            #region SQL
            string del = @"Update    [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageRegion] WITH(ROWLOCK) 
   SET IsDeleted=1 ,UpdatedDateTime=GetDate() 
    WHERE PackageProductId=@PackageProductId AND IsDeleted=0";
            string sql = @"Insert into [Tuhu_groupon].[dbo].[ThirdPartyBeautyPackageRegion]
  ([PackageProductId]
   ,[ProvinceId]
   ,[CityIds]
   ,[IsAllCitys]
   ,[IsDeleted]
   ,[CreatedDateTime]
   ,[UpdatedDateTime])
   values(
    @PackageProductId
   ,@ProvinceId
   ,@CityIds
   ,@IsAllCitys
   ,0
   ,GetDate()
   ,GetDate()
   )";
            #endregion
            List<ThirdPartyBeautyPackageProductRegionConfigModel> result = new List<ThirdPartyBeautyPackageProductRegionConfigModel>();
            using (var dbhelper = new SqlDbHelper(strConn_Write))
            {
                dbhelper.BeginTransaction();
                dbhelper.ExecuteNonQuery(del,
                    CommandType.Text,
                    new[] { new SqlParameter("PackageProductId", models.FirstOrDefault()?.PackageProductId), });
                
                    bool success = true;
                    foreach (var item in models)
                    {
                        var sqlPara = new[]
                        {
                        new SqlParameter("PackageProductId",item.PackageProductId),
                        new SqlParameter("ProvinceId",item.ProvinceId),
                        new SqlParameter("CityIds",item.CityIds),
                        new SqlParameter("IsAllCitys",item.IsAllCitys),
                    };
                        if (dbhelper.ExecuteNonQuery(sql, CommandType.Text, sqlPara) <= 0)
                        {
                            success = false;
                            break;
                        }
                    }
                    if (success)
                        dbhelper.Commit();
                    else
                    {
                        dbhelper.Rollback();
                    }
                    return await Task.FromResult(success);
            }
        }
    }
}
