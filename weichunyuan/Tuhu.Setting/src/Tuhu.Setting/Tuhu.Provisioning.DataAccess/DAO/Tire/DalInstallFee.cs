using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    public class DalInstallFee
    {
        public static IEnumerable<InstallFeeModel> SelectInstallFeeList(InstallFeeConditionModel condition, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                var para = new SqlParameter[] {
                    new SqlParameter("@Brands",condition.Brands),
                    new SqlParameter("@IsConfig",condition.IsConfig),
                    new SqlParameter("@OnSale",condition.OnSale),
                    new SqlParameter("@Patterns",condition.Patterns),
                    new SqlParameter("@PID",condition.PID),
                    new SqlParameter("@Rims",condition.Rims),
                    new SqlParameter("@Rof",condition.Rof),
                    new SqlParameter("@TireSizes",condition.TireSizes),
                    new SqlParameter("@Winter",condition.Winter),
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize),
                };
                var para_temp = new SqlParameter[] {
                    new SqlParameter("@Brands",condition.Brands),
                    new SqlParameter("@IsConfig",condition.IsConfig),
                    new SqlParameter("@OnSale",condition.OnSale),
                    new SqlParameter("@Patterns",condition.Patterns),
                    new SqlParameter("@PID",condition.PID),
                    new SqlParameter("@Rims",condition.Rims),
                    new SqlParameter("@Rof",condition.Rof),
                    new SqlParameter("@TireSizes",condition.TireSizes),
                    new SqlParameter("@Winter",condition.Winter)
                };
                pager.TotalItem = SelectInstallFeeList_Count(para_temp, dbHelper);
                return dbHelper.ExecuteDataTable(TireSql.InstallFee.select, CommandType.Text, para).ConvertTo<InstallFeeModel>();
            }
        }

        public static List<string> SelectPackagePIDs(IEnumerable<string> pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var dt = dbHelper.ExecuteDataTable(@"SELECT DISTINCT  PP.PackagePid
FROM    Tuhu_productcatalog..tbl_ProductPackage AS PP
WHERE   PP.Pid COLLATE Chinese_PRC_CI_AS IN (
        SELECT  *
        FROM    Gungnir..Split(@pids, ';') )", CommandType.Text, new SqlParameter("@pids", string.Join(";", pids)));
                List<string> list = new List<string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(row[0].ToString());
                    }
                }
                return list;
            }
        }

        public static int SelectInstallFeeList_Count(SqlParameter[] para, SqlDbHelper dbHelper)
        => Convert.ToInt32(dbHelper.ExecuteScalar(TireSql.InstallFee.select_count, CommandType.Text, para));

        public static int Insert(string pid, decimal addPrice)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(TireSql.InstallFee.insert, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@AddPrice",addPrice),
                    new SqlParameter("@PID",pid)
                });
            }
        }

        public static int Delete(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(TireSql.InstallFee.delete, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID",pid)
                });
            }
        }

        public static int Edit(string pid, decimal addPrice)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(TireSql.InstallFee.update, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@AddPrice",addPrice),
                    new SqlParameter("@PID",pid)
                });
            }
        }
    }
}
