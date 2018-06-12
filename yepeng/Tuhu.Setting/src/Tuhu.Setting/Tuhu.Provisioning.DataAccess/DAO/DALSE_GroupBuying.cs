using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Dapper;
using System.Data.SqlClient;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
   public static class DALSE_GroupBuying
    {

        public static bool Add(SqlConnection connection, SE_GroupBuyingConfig model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_GroupBuyingConfig
                            (
                              PID ,
                              FalshSaleGuid ,
                              ActivityPrice ,
                              LimitSaleNumber ,
                              LimitGroupNumber ,
                              IsGroupBuy ,
                              OrderBy ,
                              CreateDate ,
                              UpdateDate
                            )
                    VALUES  ( 
                              @PID , -- PID - varchar(50)
                              @FalshSaleGuid , -- FalshSaleGuid - uniqueidentifier
                              @ActivityPrice , -- ActivityPrice - money
                              @LimitSaleNumber , -- LimitSaleNumber - int
                              @LimitGroupNumber , -- LimitGroupNumber - int
                              @IsGroupBuy , -- IsGroupBuy - bit
                              @OrderBy , -- OrderBy - int
                              GETDATE() , -- CreateDate - datetime
                              GETDATE()  -- UpdateDate - datetime
                            )";
            return connection.Execute(sql, model)>0;

        }

        public static bool Update(SqlConnection connection, SE_GroupBuyingConfig model)
        {
            string sql = @"UPDATE Configuration.dbo.SE_GroupBuyingConfig SET PID=@PID,FalshSaleGuid=@FalshSaleGuid,ActivityPrice=@ActivityPrice,
		LimitSaleNumber=@LimitSaleNumber,LimitGroupNumber=@LimitGroupNumber,IsGroupBuy=@IsGroupBuy,OrderBy=@OrderBy,UpdateDate=GETDATE()
		WHERE ID=@ID ";

            return connection.Execute(sql, model) > 0;
        }


        public static IEnumerable<SE_GroupBuyingConfig>  GetList(SqlConnection connection)
        {
            string sql = @"	SELECT GB.*,FS.ProductName,FS.SaleOutQuantity 
		FROM Configuration.dbo.SE_GroupBuyingConfig (NOLOCK) GB LEFT JOIN Activity.dbo.tbl_FlashSaleProducts (NOLOCK) FS ON GB.FalshSaleGuid=FS.ActivityID AND GB.PID=FS.PID
		ORDER BY GB.OrderBy ASC";

            return connection.Query<SE_GroupBuyingConfig>(sql);
        }


        public static SE_GroupBuyingConfig GetEntity(SqlConnection connection, int id)
        {
            string sql = @"SELECT GB.*,FS.ProductName,FS.SaleOutQuantity 
		FROM Configuration.dbo.SE_GroupBuyingConfig (NOLOCK) GB LEFT JOIN Activity.dbo.tbl_FlashSaleProducts (NOLOCK) FS ON GB.FalshSaleGuid=FS.ActivityID AND GB.PID=FS.PID
		WHERE GB.ID=@ID";

            var parameter = new DynamicParameters();
            parameter.Add("@ID", id);
            return connection.Query<SE_GroupBuyingConfig>(sql, parameter).FirstOrDefault();
        }


        public static bool Delete(SqlConnection connection, int id)
        {
            string sql = @"	DELETE FROM Configuration.dbo.SE_GroupBuyingConfig WHERE ID=@ID";
            var parameter = new DynamicParameters();
            parameter.Add("@ID",id);
            return connection.Execute(sql, parameter) > 0;
        }


    }
}
