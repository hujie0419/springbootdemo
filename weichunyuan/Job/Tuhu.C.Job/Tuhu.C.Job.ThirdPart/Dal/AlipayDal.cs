using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ThirdParty.Models;

namespace Tuhu.C.Job.ThirdPart.Dal
{
    public class AlipayDal
    {
        public static IEnumerable<AliPayOrderLog> GetFailedlogs()
        {
            using (var dbhelper = DbHelper.CreateDbHelper(ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString))
            {
                using (var cmd = new SqlCommand("select [APOrderID],[THOrderID],[BillNo],[OrderStatus] FROM  [AliPayOrderLog](NOLOCK) l WHERE  l.OrderType='BaoYang' AND l.IsUpdatedSuccess=0"))
                {
                    return dbhelper.ExecuteSelect<AliPayOrderLog>(cmd);                  
                }
            }
           
            /*
            List<AliPayOrderLog> logs = new List<AliPayOrderLog>();
            string sql = string.Format(@"SELECT  * FROM  [AliPayOrderLog](NOLOCK) l WHERE  l.OrderType='BaoYang' AND l.IsUpdatedSuccess=false");
            sql = "select [APOrderID],[BillNo],[OrderStatus] FROM  [AliPayOrderLog](NOLOCK) l WHERE  l.OrderType='BaoYang' AND l.IsUpdatedSuccess=0";
            DataTable table=ExecuteDt(sql, new List<SqlParameter>(), ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString);
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    logs.Add(new AliPayOrderLog()
                    {
                        APOrderID = row["APOrderID"].ToString(),
                        billNo = row["BillNo"].ToString(),
                        OrderStatus=(AliPayUserPayOrderStatus)Enum.Parse(typeof(AliPayUserPayOrderStatus),row["OrderStatus"].ToString()),

                    });
                }

            }

            return logs;
            */


        }


        public static DataTable ExecuteDt(string sql, List<SqlParameter> list,string conn)
        {
            DataTable data = new DataTable();
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                if (list != null && list.Count > 0)
                {
                    cmd.Parameters.AddRange(list.ToArray());
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(data);
            }
            return data;
        }
    }
}
