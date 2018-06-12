using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALAliPayBaoYang
    {
        public static int SaveAliPayBaoYangItem(AliPayBaoYangItem model)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"INSERT INTO [dbo].[AliPayBaoYangItem]
           ([KeyWord] ,[IsDisabled],[CreateTime],[UpdateTime]) values
            (@keyword,@isdisabled,@createtime,@updatetime)");
                cmd.CommandType = CommandType.Text;
                DateTime datetime = DateTime.Now;
                cmd.Parameters.AddWithValue("@keyword", model.KeyWord);
                cmd.Parameters.AddWithValue("@isdisabled", model.IsDisabled);
                cmd.Parameters.AddWithValue("@createtime", datetime);
                cmd.Parameters.AddWithValue("@updatetime", datetime);
                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static int UpdateAliPayBaoYangItem(AliPayBaoYangItem model)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"update [dbo].[AliPayBaoYangItem]
              set KeyWord=@keyword,
              IsDisabled=@isdisabled,
              UpdateTime=@updatetime where PKID=@pkid");
                cmd.CommandType = CommandType.Text;
                DateTime datetime = DateTime.Now;
                cmd.Parameters.AddWithValue("@pkid", model.PKID);
                cmd.Parameters.AddWithValue("@keyword", model.KeyWord);
                cmd.Parameters.AddWithValue("@isdisabled", model.IsDisabled);               
                cmd.Parameters.AddWithValue("@updatetime", datetime);
                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static int DeleteAliPayBaoYangItem(int pkid)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"delete  [dbo].[AliPayBaoYangItem]
               where PKID=@pkid");
                cmd.CommandType = CommandType.Text;
               
                cmd.Parameters.AddWithValue("@pkid", pkid);
              
                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }

        public static List<AliPayBaoYangItem> GetAliPayBaoYangItem(int pkid)
        {
            // SupplierInfo result = null;
            List<AliPayBaoYangItem> items;
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                string sqlstr = string.Empty;
                if (pkid > 0)
                {
                    sqlstr = @"SELECT [PKID],[KeyWord] ,[IsDisabled],[CreateTime],[UpdateTime] FROM AliPayBaoYangItem WHERE PKID=@PKID";
                }
                else
                {
                    sqlstr = @"SELECT [PKID],[KeyWord] ,[IsDisabled],[CreateTime],[UpdateTime] FROM AliPayBaoYangItem";
                }
                var cmd = new SqlCommand(sqlstr);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@PKID", pkid);

                items = dbhelper.ExecuteDataTable(cmd).ConvertTo<AliPayBaoYangItem>().ToList();

            }
            return items;
        }


        public static List<string> GetModelIdList()
        {
            List<string> modelIdList = new List<string>();
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                string sqlstr = string.Empty;

                sqlstr = @"SELECT ModelID FROM tbl_ThirdPartyVehicle";

                var cmd = new SqlCommand(sqlstr);
                cmd.CommandType = CommandType.Text;

                var dt = dbhelper.ExecuteDataTable(sqlstr);
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        modelIdList.Add(dr["ModelID"].ToString());
                    }

                }
            }
            return modelIdList;
        }

        public static AliPayBaoYangActivity GetAliPayBaoYangActivity()
        {
            // SupplierInfo result = null;
            AliPayBaoYangActivity item = null;
            List<AliPayBaoYangActivity> items;
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                string sqlstr = string.Empty;
             
                sqlstr = @"SELECT [PKID],[Name],[BeginTime],[EndTime],[CreateTime],[UpdateTime] FROM [dbo].[AliPayBaoYangActivity]";
                
                var cmd = new SqlCommand(sqlstr);
                cmd.CommandType = CommandType.Text;

                items = dbhelper.ExecuteDataTable(cmd).ConvertTo<AliPayBaoYangActivity>().ToList();
                if (items != null && items.FirstOrDefault() != null)
                {
                    item = items.First();
                }
            }
            return item;
        }


        public static int UpdateAliPayBaoYangActivity(AliPayBaoYangActivity model)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var cmd = new SqlCommand(@"update [dbo].[AliPayBaoYangActivity]
              set BeginTime=@BeginTime,
              EndTime=@EndTime,
              UpdateTime=@updatetime");
                cmd.CommandType = CommandType.Text;
                DateTime datetime = DateTime.Now;                    
                cmd.Parameters.AddWithValue("@BeginTime", model.BeginTime);
                cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                cmd.Parameters.AddWithValue("@UpdateTime", datetime);
                return Convert.ToInt32(dbhelper.ExecuteNonQuery(cmd));
            }
        }
    }
}
