using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.PromotionTask.Model;

namespace Tuhu.C.Job.PromotionTask.Dal
{
    class DalCouponRules
    {
        public static IEnumerable<CouponRulesModel> GetCouponRulesChild()
        {
            const string sql = "select Category,ProductId,Brand,ParentId,CreateDateTime,ShopId,ShopType,PIDType From [Activity].[dbo].[tbl_CouponRules] with(nolock) where ParentID>0";
            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteSelect<CouponRulesModel>(true, cmd);
            }
        }

        public static bool CreateCouponRuleProductConfigsData(IEnumerable<CouponRulesConfig> datas, string DestinationTableName)
        {
            DataTable dt = new DataTable(DestinationTableName);
            DataColumn dc0 = new DataColumn("PKID", Type.GetType("System.Int32"));
            DataColumn dc1 = new DataColumn("RuleID", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("Type", Type.GetType("System.Int32"));
            DataColumn dc3 = new DataColumn("ConfigValue", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("CreateDateTime", Type.GetType("System.DateTime"));
            DataColumn dc5 = new DataColumn("LastUpdateDateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            foreach (var item in datas)
            {
                var dr = dt.NewRow();
                dr["PKID"] = DBNull.Value;
                dr["RuleID"] = item.RuleID;
                dr["Type"] = item.Type;
                dr["ConfigValue"] = item.ConfigValue;
                dr["CreateDateTime"] = DateTime.Now;
                dr["LastUpdateDateTime"] = DateTime.Now;
                dt.Rows.Add(dr);
            }
            using (var db = DbHelper.CreateDbHelper())
            {
                try
                {
                    using (var cmd = new SqlBulkCopy(db.Connection.ConnectionString))
                    {
                        cmd.BatchSize = datas.Count();
                        cmd.BulkCopyTimeout = 30;
                        cmd.DestinationTableName = $"[Activity].[dbo].[{DestinationTableName}]";
                        cmd.WriteToServer(dt);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static bool CleanCouponRulesConfig()
        {
            const string sql = "Delete  From [Activity].[dbo].[tbl_CouponRules_ConfigProduct];Delete  From [Activity].[dbo].[tbl_CouponRules_ConfigShop]";
            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool UpdateCouponRuleConfigType(int pkid,int configType,bool pidType)
        {
            const string sql = "Update   [Activity].[dbo].[tbl_CouponRules] with(ROWLOCK) SET ConfigType=@ConfigType,pidType=@pidType  where pkid=@pkid";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ConfigType", configType);
                cmd.Parameters.AddWithValue("@pkid", pkid);
                cmd.Parameters.AddWithValue("@pidType", pidType);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
    }
}
