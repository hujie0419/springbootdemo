using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalShop
    {
        public static string GetSimpleNameByShopID(SqlConnection connection, int shopID)
        {
            var sqlParameter = new SqlParameter("@PKID", shopID);
            return
                SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, "procShopsGetSimpleNameByPKID",
                    sqlParameter).ToString();
        }

        #region App 首页Banner
        public static List<ShopAppBanner> GetShopAppBannerList(SqlConnection connection)
        {
            string sql = @"SELECT  s.PKID ,
        s.Type ,
        s.Theme ,
        s.MainTitle ,
        s.Contents ,
        s.URL ,
        s.Action ,
        s.StartDate ,
        s.EndDate ,
        s.BgColor ,
        s.ImgType ,
        s.ImgUrl1 ,
        s.ImgUrl2 ,
        s.ImgUrl3 ,
        s.ImgUrl4 ,
        s.Remark ,
        e.EmployeeName AS Operator ,
        s.UpdateTime,
        s.PushType
FROM    dbo.ShopAppBanner s WITH ( NOLOCK )
LEFT JOIN dbo.HrEmployee e WITH ( NOLOCK )
        ON e.EmailAddress = s.Operator
WHERE s.IsDeleted=0";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<ShopAppBanner>().ToList();
        }

        public static ShopAppBanner GetAppShopBannerByPKID(SqlConnection connection, int _PKID)
        {
            string sql = @"SELECT  s.PKID ,
        s.Type ,
        s.Theme ,
        s.MainTitle ,
        s.Contents ,
        s.URL ,
        s.Action ,
        s.StartDate ,
        s.EndDate ,
        s.BgColor ,
        s.ImgType ,
        s.ImgUrl1 ,
        s.ImgUrl2 ,
        s.ImgUrl3 ,
        s.ImgUrl4 ,
        s.Remark ,
        s.Operator ,
        s.UpdateTime,
        s.PushType
FROM    dbo.ShopAppBanner s WITH ( NOLOCK ) where PKID={0} AND IsDeleted=0";
            sql = string.Format(sql, _PKID);
            List<ShopAppBanner> list = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<ShopAppBanner>().ToList();
            if (list.Count == 1)
            {
                return list.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public static int DeleteShopBannerByPKID(SqlConnection connection, int PKID)
        {
            string sql = "UPDATE dbo.ShopAppBanner SET IsDeleted=1,UpdateTime=GETDATE() WHERE PKID={0}";
            sql = string.Format(sql, PKID);
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql);
        }

        public static int InsertShopBanner(SqlConnection connection, ShopAppBanner banner)
        {
            /*
            string sql = @"INSERT INTO dbo.ShopAppBanner
		        (
		          Type ,
		          Theme ,
		          MainTitle ,
		          Contents ,
		          URL ,
		          Action ,
		          StartDate ,
		          EndDate ,
		          BgColor ,
		          ImgType ,
		          ImgUrl1 ,
		          ImgUrl2 ,
		          ImgUrl3 ,
		          ImgUrl4 ,
		          Remark ,
		          Operator ,
                  PushType,
		          UpdateTime
		        )
		VALUES  (
		          N'{0}', -- Type - nvarchar(50)
		          N'{1}', -- Theme - nvarchar(100)
		          N'{2}', -- MainTitle - nvarchar(100)
		          N'{3}', -- Contents - nvarchar(100)
		          N'{4}', -- URL - nvarchar(500)
		          N'{5}', -- Action - nvarchar(50)
		          '{6}', -- StartDate - datetime
		          '{7}', -- EndDate - datetime
		          N'{8}', -- BgColor - nvarchar(20)
		          N'{9}', -- ImgType - nvarchar(50)
		          N'{10}', -- ImgUrl1 - nvarchar(500)
		          N'{11}', -- ImgUrl2 - nvarchar(500)
		          N'{12}', -- ImgUrl3 - nvarchar(500)
		          N'{13}', -- ImgUrl4 - nvarchar(500)
		          N'{14}', -- Remark - nvarchar(max)
		          N'{15}', -- Operator - nvarchar(50)
                  N'{16}', -- PushType -nvchar(32)
		          GETDATE()  -- UpdateTime - datetime
		        )";
            */

            string sql = @"INSERT INTO dbo.ShopAppBanner
		        (
		          Type ,
		          Theme ,
		          MainTitle ,
		          Contents ,
		          URL ,
		          Action ,
		          StartDate ,
		          EndDate ,
		          BgColor ,
		          ImgType ,
		          ImgUrl1 ,
		          ImgUrl2 ,
		          ImgUrl3 ,
		          ImgUrl4 ,
		          Remark ,
		          Operator ,
                  PushType,
		          UpdateTime
		        )
            VALUES(
                  @Type ,
		          @Theme ,
		          @MainTitle ,
		          @Contents ,
		          @URL ,
		          @Action ,
		          @StartDate ,
		          @EndDate ,
		          @BgColor ,
		          @ImgType ,
		          @ImgUrl1 ,
		          @ImgUrl2 ,
		          @ImgUrl3 ,
		          @ImgUrl4 ,
		          @Remark ,
		          @Operator ,
                  @PushType,
                  GETDATE()
                )";
               
           // sql = string.Format(sql, banner.Type, banner.Theme, banner.MainTitle, banner.Contents, banner.URL, banner.Action, banner.StartDate.ToString("yyyy -MM-dd"), banner.EndDate.ToString("yyyy-MM-dd"),
            //    banner.BgColor, banner.ImgType, banner.ImgUrl1, banner.ImgUrl2, banner.ImgUrl3, banner.ImgUrl4, banner.Remark, banner.Operator,banner.PushType);

            var parameters = new[] { new SqlParameter("@Type",banner.Type), new SqlParameter("@Theme", banner.Theme) , new SqlParameter("@MainTitle", banner.MainTitle) ,
                                     new SqlParameter("@Contents",banner.Contents),new SqlParameter("@URL",banner.URL),new SqlParameter("@Action",banner.Action),
                                     new SqlParameter("@StartDate",banner.StartDate.ToString("yyyy -MM-dd")),new SqlParameter("@EndDate",banner.EndDate.ToString("yyyy -MM-dd")),new SqlParameter("@BgColor",banner.BgColor),
                                     new SqlParameter("@ImgType",banner.ImgType),new SqlParameter("@ImgUrl1",banner.ImgUrl1),new SqlParameter("@ImgUrl2",banner.ImgUrl2),
                                     new SqlParameter("@ImgUrl3",banner.ImgUrl3),new SqlParameter("@ImgUrl4",banner.ImgUrl4),new SqlParameter("@Remark",banner.Remark),
                                      new SqlParameter("@Operator",banner.Operator),new SqlParameter("@PushType",banner.PushType)};
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql,parameters);
        }

        public static int UpdateShopBanner(SqlConnection connection, ShopAppBanner banner)
        {
            /*
            string sql = @"UPDATE dbo.ShopAppBanner  WITH(ROWLOCK) SET 
		          Type=N'{0}',
		          Theme=N'{1}',
		          MainTitle=N'{2}',
		          Contents=N'{3}',
		          URL=N'{4}',
		          Action=N'{5}',
		          StartDate='{6}',
		          EndDate= '{7}',
		          BgColor=N'{8}',
		          ImgType=N'{9}',
		          ImgUrl1=N'{10}',
		          ImgUrl2=N'{11}',
		          ImgUrl3=N'{12}',
		          ImgUrl4=N'{13}',
		          Remark=N'{14}',
		          Operator=N'{15}',
                  PushType=N'{16}',
		          UpdateTime=GETDATE()
				  WHERE PKID={17}
		       ";
            sql = string.Format(sql, banner.Type, banner.Theme, banner.MainTitle, banner.Contents, banner.URL, banner.Action, banner.StartDate.ToString("yyyy-MM-dd"), banner.EndDate.ToString("yyyy-MM-dd"),
                banner.BgColor, banner.ImgType, banner.ImgUrl1, banner.ImgUrl2, banner.ImgUrl3, banner.ImgUrl4, banner.Remark, banner.Operator,banner.PushType, banner.PKID);
                */
            string sql = @"UPDATE dbo.ShopAppBanner  WITH(ROWLOCK) SET 
                  Type=@Type,
		          Theme=@Theme,
		          MainTitle=@MainTitle,
		          Contents=@Contents,
		          URL=@URL,
		          Action=@Action,
		          StartDate=@StartDate,
		          EndDate= @EndDate,
		          BgColor=@BgColor,
		          ImgType=@ImgType,
		          ImgUrl1=@ImgUrl1,
		          ImgUrl2=@ImgUrl2,
		          ImgUrl3=@ImgUrl3,
		          ImgUrl4=@ImgUrl4,
		          Remark=@Remark,
		          Operator=@Operator,
                  PushType=@PushType,
		          UpdateTime=GETDATE()
				  WHERE PKID=@PKID
		       ";
            var parameters = new[] { new SqlParameter("@Type",banner.Type), new SqlParameter("@Theme", banner.Theme) , new SqlParameter("@MainTitle", banner.MainTitle) ,
                                     new SqlParameter("@Contents",banner.Contents),new SqlParameter("@URL",banner.URL),new SqlParameter("@Action",banner.Action),
                                     new SqlParameter("@StartDate",banner.StartDate.ToString("yyyy -MM-dd")),new SqlParameter("@EndDate",banner.EndDate.ToString("yyyy -MM-dd")),new SqlParameter("@BgColor",banner.BgColor),
                                     new SqlParameter("@ImgType",banner.ImgType),new SqlParameter("@ImgUrl1",banner.ImgUrl1),new SqlParameter("@ImgUrl2",banner.ImgUrl2),
                                     new SqlParameter("@ImgUrl3",banner.ImgUrl3),new SqlParameter("@ImgUrl4",banner.ImgUrl4),new SqlParameter("@Remark",banner.Remark),
                                      new SqlParameter("@Operator",banner.Operator),new SqlParameter("@PushType",banner.PushType),new SqlParameter("@PKID",banner.PKID)};
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql,parameters);
        }

        public static ShopAccounting GetShopAccountingSummoneyByID(SqlConnection connection, int pkid, int shopid)
        {
            string sql = @"SELECT  SUM(SumMoney) - SUM(SumInstallFee) - SUM(SumServerFee) AS BalanceSumMoney
FROM    dbo.tbl_ShopPayOff a  WITH(NOLOCK)
WHERE   DrawStatus = 1
        AND a.ShopID = {0}
        AND a.ShopAccountingID IS NULL";
            sql = string.Format(sql, shopid);
            List<ShopAccounting> list = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<ShopAccounting>().ToList();
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        public static List<ShopAppBanner> GetAPPShopAppBannerList(SqlConnection connection, string content, int picSize)
        {
            string sql = @"SELECT  PKID ,
      	Theme ,
      	MainTitle ,
      	Contents ,
      	URL ,
      	Action ,
      	BgColor ,
      	ImgType ,
		CASE WHEN @picSize=1 THEN ImgUrl1 
		WHEN @picSize=2 THEN ImgUrl2
		WHEN @picSize=3 THEN ImgUrl3
		WHEN @picSize=4 THEN ImgUrl4
		ELSE ImgUrl1 END AS ImgUrl1
FROM    dbo.ShopAppBanner WITH ( NOLOCK )
WHERE   1 = 1
		AND StartDate <= GETDATE()
        AND EndDate >= GETDATE()-1";
            if (!string.IsNullOrEmpty(content))
            {
                sql += content;
            }
            sql += " AND IsDeleted=0 ORDER BY PKID DESC";
            var sqlparam = new[]{
                new SqlParameter("@picSize",picSize)
            };
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlparam).ConvertTo<ShopAppBanner>().ToList();
        }

        public static int GetMaxBannerId(SqlConnection connection)
        {
            string sql = "SELECT MAX(PKID) AS  PKID FROM dbo.ShopAppBanner WITH (NOLOCK) WHERE EndDate>= GETDATE()-1 AND StartDate <= GETDATE() AND IsDeleted=0";
            DataTable dt = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}