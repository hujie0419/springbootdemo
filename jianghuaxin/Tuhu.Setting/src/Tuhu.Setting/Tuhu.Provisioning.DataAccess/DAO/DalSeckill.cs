using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalSeckill
    {
        public static IEnumerable<QiangGouProductModel> SelectQiangGouAndProducts(DateTime dt)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @"SELECT * FROM Activity..tbl_FlashSale AS A WITH ( NOLOCK) 
		JOIN Activity..tbl_FlashSaleProducts AS B WITH(NOLOCK) ON B.ActivityID = A.ActivityID WHERE A.ActiveType=1 AND A.StartDateTime>@Dt-8 AND a.EndDateTime<@DT+8 And IsDefault=0";

                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[]
                {
                    new SqlParameter("@Dt",dt)
                }).ConvertTo<QiangGouProductModel>();
            }
        }
        public static IEnumerable<QiangGouProductModel> SelectQiangGouTempAndProducts(DateTime dt)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @"SELECT * FROM Activity..tbl_FlashSale_temp AS A WITH ( NOLOCK) 
		JOIN Activity..tbl_FlashSaleProducts_temp AS B WITH(NOLOCK) ON B.ActivityID = A.ActivityID WHERE A.ActiveType=1 AND A.StartDateTime>@Dt-8 AND a.EndDateTime<@DT+8 And IsDefault=0";

                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[]
                {
                    new SqlParameter("@Dt",dt)
                }).ConvertTo<QiangGouProductModel>();
            }
        }
        public static int SelectActivityStatusByActivityId(string activityId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @"	SELECT Status FROM Configuration..ActivityApprovalStatus WITH ( NOLOCK)  WHERE ActivityId=@ActivityId";

                return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[]
                {
                     new SqlParameter("@ActivityId",activityId)
            }));
            }
        }

        public static int SelectActivityProductsByActivityId(string activityId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @"SELECT count(*) FROM Activity..tbl_FlashSaleProducts_Temp WITH ( NOLOCK)  WHERE ActivityId=@ActivityId";

                return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[]
                {
                     new SqlParameter("@ActivityId",activityId)
            }));
            }
        }
        public static IEnumerable<QiangGouProductModel> SelectDefultActivityBySchedule(string schedule)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var whereCondition = $"where ActivityName like N'%:{schedule}'";
                var sql = $@"SELECT * FROM Activity..tbl_FlashSale AS A WITH ( NOLOCK) 
		JOIN Activity..tbl_FlashSaleProducts AS B WITH(NOLOCK) ON B.ActivityID = A.ActivityID {whereCondition}AND IsDefault=1";

                return dbHelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<QiangGouProductModel>(); ;
            }
        }
        public static IEnumerable<QiangGouProductModel> SelectDefultActivityTempBySchedule(string schedule)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var whereCondition = $"where ActivityName like N'%:{schedule}'";
                var sql = $@"SELECT * FROM Activity..tbl_FlashSale_temp AS A WITH ( NOLOCK) 
		JOIN Activity..tbl_FlashSaleProducts_temp AS B WITH(NOLOCK) ON B.ActivityID = A.ActivityID {whereCondition}AND IsDefault=1";

                return dbHelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<QiangGouProductModel>(); ;
            }
        }
        public static List<ProductModel> SelectProductCostPriceByPids(List<string> pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_BI_ReadOnly")))
            {
                using (var cmd = new SqlCommand(@"	            
                        WITH    pids
                      AS ( SELECT   Item COLLATE Chinese_PRC_CI_AS AS PID
                           FROM     Tuhu_bi..SplitString(@Pids,
                                                              ',', 1)
                         )
                      SELECT  t.pid ,t.cost AS CostPrice
                        FROM    Tuhu_bi.dbo.dm_Product_SalespredictData AS t
                                WITH ( NOLOCK )
                        JOIN pids ON pids.PID = t.PID "))
                {
                    cmd.Parameters.AddWithValue("@Pids", string.Join(",", pids));
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<ProductModel>().ToList();
                }
            }
        }
        public static DataTable FetchNeedExamQiangGouAndProducts(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	FS.ActiveType,
		FS.ActivityName,
		FS.StartDateTime,
		FS.EndDateTime,
		FS.PlaceQuantity,
        FS.IsNewUserFirstOrder,
		VP.DisplayName,
		VP.cy_list_price AS OriginalPrice,
        VP.Image,
		FSP.*
FROM	Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
JOIN	Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK )
		ON FS.ActivityID = FSP.ActivityID
JOIN	Tuhu_productcatalog..vw_Products AS VP
		ON FSP.PID  COLLATE Chinese_PRC_CI_AS = VP.PID   
WHERE	FS.ActivityID = @ActivityID ORDER BY FSP.PKID;", CommandType.Text, new SqlParameter("@ActivityID", aid));
            }
        }

        public static int UpdateSeckillToToApprove(string acid,int status=1)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                using (var cmd = new SqlCommand(@"	            
                        	MERGE INTO Configuration..ActivityApprovalStatus AS T 
	USING (select @ActivityId as ActivityId)AS S
	ON s.ActivityId=T.ActivityId 
	WHEN MATCHED 
	THEN UPDATE SET Status=@Status
	when NOT MATCHED THEN 
						INSERT (ActivityId,Status)VALUES(@ActivityId,1);"))
                {
                    cmd.Parameters.AddWithValue("@ActivityId", acid);
                    cmd.Parameters.AddWithValue("@Status", status);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static int DeleteStatusData(string acid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                using (var cmd = new SqlCommand(@"	            
             delete from Configuration..ActivityApprovalStatus where  ActivityId=@ActivityId"))
                {
                    cmd.Parameters.AddWithValue("@ActivityId", acid);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static int DeleteFlashSaleTempByAcid(string acid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                using (var cmd = new SqlCommand(@"	            
                   DELETE  Activity..tbl_FlashSale_Temp WITH(ROWLOCK)
                    WHERE   ActivityID = @ActivityID;"))
                {
                    cmd.Parameters.AddWithValue("@ActivityId", acid);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }
        public static int DeleteFlashSaleProductsTempByAcid(string acid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                using (var cmd = new SqlCommand(@"	            
                    DELETE  Activity..tbl_FlashSaleProducts_Temp WITH(ROWLOCK)
                    WHERE   ActivityID = @ActivityID;"))
                {
                    cmd.Parameters.AddWithValue("@ActivityId", acid);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }
 
        public static int InsertActivityApprovalStatusByAcid(string acid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                using (var cmd = new SqlCommand(@"	            
                    						INSERT INTO Configuration..ActivityApprovalStatus (ActivityId,Status)VALUES(@ActivityId,2)"))
                {
                    cmd.Parameters.AddWithValue("@ActivityId", acid);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static int  SelectQiangGouIsExist(string aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (var cmd = new SqlCommand(@"select count(*) from Activity..tbl_FlashSale WITH ( NOLOCK ) where ActivityID=@Aid"))
                {
                    cmd.Parameters.AddWithValue("@Aid", aid);
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(dbHelper.ExecuteScalar(cmd));
                }
            }
        }

    }
}
