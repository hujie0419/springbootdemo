using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALShipping
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);


        public static List<GradeDeliveryFeeRule> GetGradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            const string sql = @"SELECT  PKID ,
                                        Grade ,--会员等级
                                        ProductType ,--商品类别，0 轮胎 / 轮毂 1 车品 / 保养
                                        IsShopInstall ,--是否到店，0 到家 ；1到店
                                        Price ,--包邮金额
                                        IsFreeInstall ,--是否包安装
                                        Content ,--包邮的话语
                                        CreateDateTime ,
                                        LastUpdateDateTime ,
                                        [ContainInstall] ,
                                        [NoContainInstall] ,
                                        CASE WHEN UserType = 2 THEN UserType
                                             ELSE 1
                                        END AS UserType
                                FROM    Configuration..tbl_GradeDeliveryFeeRule WITH ( NOLOCK ) 
                                WHERE IsShopInstall=@IsShopInstall AND ProductType=@ProductType";

            var sqlParameter = new SqlParameter[]
              {
                    new SqlParameter("@ProductType",model.ProductType),
                    new SqlParameter("@IsShopInstall",model.IsShopInstall)
              };
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameter).ConvertTo<GradeDeliveryFeeRule>().ToList();
        }

        public static bool InsertGradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            const string sql = @"INSERT INTO Configuration..tbl_GradeDeliveryFeeRule
                                        (       Grade ,--会员等级
                                                ProductType ,--商品类别，0 轮胎 / 轮毂 1 车品 / 保养
                                                IsShopInstall ,--是否到店，0 到家 ；1到店
                                                Price ,--包邮金额
                                                IsFreeInstall ,--是否包安装
                                                Content ,--包邮的话语
                                                CreateDateTime ,
                                                LastUpdateDateTime,
                                                [ContainInstall],
                                                [NoContainInstall],
                                                UserType
                                        )
                                VALUES  (       @Grade ,
                                                @ProductType ,
                                                @IsShopInstall ,
                                                @Price ,
                                                @IsFreeInstall ,
                                                @Content ,
                                                GETDATE() ,
                                                GETDATE(),
                                                @ContainInstall,
                                                @NoContainInstall,
                                                @UserType
                                        )
                                ";
            var sqlParameters = new SqlParameter[]
                 {
                    new SqlParameter("@Content",model.Content),
                    new SqlParameter("@Grade",model.Grade),
                    new SqlParameter("@IsFreeInstall",model.IsFreeInstall),
                    new SqlParameter("@IsShopInstall",model.IsShopInstall),
                    new SqlParameter("@Price",model.Price),
                    new SqlParameter("@ProductType",model.ProductType),
                    new SqlParameter("@ContainInstall",model.ContainInstall),
                    new SqlParameter("@NoContainInstall",model.NoContainInstall),
                    new SqlParameter("@UserType",model.UserType)
                  };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        public static bool UpdateGradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            const string sql = @"UPDATE  Configuration.dbo.tbl_GradeDeliveryFeeRule
                                 SET     Price = @Price ,
                                         IsFreeInstall = @IsFreeInstall ,
                                         Content = @Content ,
                                         ContainInstall=@ContainInstall ,    
                                         NoContainInstall=@NoContainInstall,
                                         LastUpdateDateTime = GETDATE(),
                                         UserType = @UserType
                                 WHERE   PKID = @PKID                                             
                                ";
            var sqlParameters = new SqlParameter[]
                 {
                    new SqlParameter("@Content",model.Content),
                    new SqlParameter("@Grade",model.Grade),
                    new SqlParameter("@IsFreeInstall",model.IsFreeInstall),
                    new SqlParameter("@IsShopInstall",model.IsShopInstall),
                    new SqlParameter("@Price",model.Price),
                    new SqlParameter("@ProductType",model.ProductType),
                    new SqlParameter("@PKID",model.PKID),
                    new SqlParameter("@ContainInstall",model.ContainInstall),
                    new SqlParameter("@NoContainInstall",model.NoContainInstall),
                    new SqlParameter("@UserType",model.UserType)
                  };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        public static bool DeleteGradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            const string sql = @"DELETE FROM [Configuration].[dbo].[tbl_GradeDeliveryFeeRule] 
                                 WHERE IsShopInstall=@IsShopInstall AND ProductType=@ProductType
                                 AND Grade IS NOT NULL AND Grade <> ''                                       
                                ";
            var sqlParameters = new SqlParameter[]
                 {
                    new SqlParameter("@IsShopInstall",model.IsShopInstall),
                    new SqlParameter("@ProductType",model.ProductType),
                  };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }


        public static DataTable SelectShippingRule()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT  DF.PKID ,
                DF.Types ,
                DF.Value ,
		        DF.UserType,
                DF.CreateDateTime ,
                DF.LastUpdateDateTime ,
                DFC.CityID
                FROM  Configuration.[dbo].[tbl_DeliveryFee] AS [DF]
                LEFT JOIN Configuration.[dbo].[tbl_DeliveryFeeCity] AS [DFC] ON DF.PKID = DFC.DeliveryFeeID;");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static DataTable SelectCityNameByCityIDs(List<int> cityid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT R.RegionName FROM  Gungnir..tbl_region AS R WHERE R.PKID IN (SELECT	*
												 FROM	Gungnir..SplitString(@CityIDS, ',', 1) )");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@CityIDS", string.Join(",", cityid));
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        public static int InsertAndUpdateaRule(IEnumerable<ShippingModel> list)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                try
                {
                    foreach (var model in list)
                    {
                        if (model.PKID == 0)//新增规则
                        {
                            var result = InsertRule(dbhelper, model);
                            if (result > 0)//新增规则成功,返回PKID
                            {
                                if (!string.IsNullOrWhiteSpace(model.StrCityIDs))//该规则有城市
                                {
                                    var cityids = model.StrCityIDs.Split('|');
                                    foreach (var cityid in cityids)
                                    {
                                        var res = InsertCityID(dbhelper, result, Convert.ToInt32(cityid));//新增城市
                                        if (res <= 0)//新增城市失败 回滚 返回
                                        {
                                            dbhelper.Rollback();
                                            return res;
                                        }
                                    }
                                }
                            }
                            else//新增规则失败  回滚 返回
                            {
                                dbhelper.Rollback();
                                return result;

                            }
                        }
                        else//修改规则
                        {

                            var result = UpdateRule(dbhelper, model);
                            if (result > 0)//修改规则成功
                            {
                                if (!string.IsNullOrWhiteSpace(model.StrCityIDs))//该规则有城市
                                {
                                    var cityids = model.StrCityIDs.Split('|');
                                    if (DelCityID(dbhelper, model.PKID) <= 0)//删除该规则原来城市 删除失败则回滚
                                    {
                                        dbhelper.Rollback();
                                        return -99;
                                    }
                                    foreach (var cityid in cityids)
                                    {
                                        var res = InsertCityID(dbhelper, model.PKID, Convert.ToInt32(cityid));//新增城市
                                        if (res <= 0)//新增城市失败 回滚 返回
                                        {
                                            dbhelper.Rollback();
                                            return res;
                                        }
                                    }
                                }
                            }
                            else//修改规则失败  回滚 返回
                            {
                                dbhelper.Rollback();
                                return result;
                            }
                        }

                    }
                }
                catch(Exception ex)
                {
                    dbhelper.Rollback();
                    return -11;
                }
                dbhelper.Commit();
            }

            return 99;
        }


        public static int InsertRule(SqlDbHelper dbhelper, ShippingModel model)
        {
            int result = 0;
            using (var cmd = new SqlCommand(@"
                   INSERT INTO  Configuration.[dbo].[tbl_DeliveryFee]( [Types] ,[Value], [UserType] ) VALUES  ( @Types,@Value,@UserType)
                   IF @@ROWCOUNT>0
                   SELECT @@IDENTITY"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Types", model.Types);
                cmd.Parameters.AddWithValue("@Value", model.Value);
                cmd.Parameters.AddWithValue("@UserType", model.UserType);
                var ns = dbhelper.ExecuteScalar(cmd);
                if (ns != null)
                {
                    result = Convert.ToInt32(ns);
                }
                return Convert.ToInt32(result);
            }
        }

        public static int InsertCityID(SqlDbHelper dbhelper, int deliveryFeeID, int cityid)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO  Configuration.[dbo].[tbl_DeliveryFeeCity] ( [DeliveryFeeID] ,[CityID] ) VALUES  ( @DeliveryFeeID, @CityID)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DeliveryFeeID", deliveryFeeID);
                cmd.Parameters.AddWithValue("@CityID", cityid);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        public static int UpdateRule(SqlDbHelper dbhelper, ShippingModel model)
        {
            using (var cmd = new SqlCommand(@"UPDATE  Configuration.[dbo].[tbl_DeliveryFee] SET Types=@Types,Value=@Value, UserType = @UserType WHERE  PKID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Types", model.Types);
                cmd.Parameters.AddWithValue("@Value", model.Value);
                cmd.Parameters.AddWithValue("@UserType", model.UserType);
                cmd.Parameters.AddWithValue("@PKID", model.PKID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        public static int DelCityID(SqlDbHelper dbhelper, int deliveryFeeID)
        {
            using (var cmd = new SqlCommand(@"DELETE FROM  Configuration.[dbo].[tbl_DeliveryFeeCity]  WHERE  DeliveryFeeID=@DeliveryFeeID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DeliveryFeeID", deliveryFeeID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }


        public static int DelRule(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand("Configuration..Shipping_DelRule"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                dbhelper.ExecuteNonQuery(cmd);
                return Convert.ToInt32(cmd.Parameters["@Result"].Value);
            }
        }

    }
}

