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
    public class DALServiceTag
    {
        public static DataTable SelectServiceTag()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("[Configuration]..[ServiceTag_SelectServiceTag]");
                cmd.CommandType = CommandType.StoredProcedure;
                return dbhelper.ExecuteDataTable(cmd);
            }

        }

        public static int DeleteTag(int PKID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("[Configuration]..[ServiceTag_DeleteTag]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int InsertServiceTag(SqlDbHelper dbhelper, ServiceTagModel model)
        {
            using (var cmd = new SqlCommand("Configuration..ServiceTag_InsertServiceTag"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Type", model.Type);
                cmd.Parameters.AddWithValue("@ServiceTag", model.ServiceTag);
                cmd.Parameters.AddWithValue("@ServiceDescribe", model.ServiceDescribe);
                if (!string.IsNullOrWhiteSpace(model.ProductTag))
                {
                    cmd.Parameters.AddWithValue("@ProductTag", model.ProductTag.Trim());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ProductTag", null);
                }


                if (!string.IsNullOrWhiteSpace(model.ProductDescribe))
                {
                    cmd.Parameters.AddWithValue("@ProductDescribe", model.ProductDescribe.Trim());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ProductDescribe", null);
                }
                
                cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                dbhelper.ExecuteNonQuery(cmd);
                return Convert.ToInt32(cmd.Parameters["@Result"].Value);
            }
        }

        public static int InsertTagID(SqlDbHelper dbhelper, int TagID, int Type, string serviceid,string ServiceDescribeTID)
        {
            using (var cmd = new SqlCommand("Configuration..ServiceTag_InsertTagID"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TagID", TagID);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@ServiceID", serviceid);
                cmd.Parameters.AddWithValue("@ServiceDescribeTID", ServiceDescribeTID);
                return dbhelper.ExecuteNonQuery(cmd);

            }
        }

        public static int UpdateServiceTag(SqlDbHelper dbhelper, ServiceTagModel model)
        {
            using (var cmd = new SqlCommand(@"UPDATE  Configuration.[dbo].[tbl_ProductServiceTag] SET ServiceTag=@ServiceTag,ServiceDescribe=@ServiceDescribe,ProductTag=@ProductTag,ProductDescribe=@ProductDescribe WHERE  PKID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", model.PKID);
                cmd.Parameters.AddWithValue("@ServiceTag", model.ServiceTag);
                cmd.Parameters.AddWithValue("@ServiceDescribe", model.ServiceDescribe);

                // cmd.Parameters.AddWithValue("@ProductTag", model.ProductTag.Trim()??null);
                // cmd.Parameters.AddWithValue("@ProductDescribe", model.ProductDescribe.Trim()??null);
                if (!string.IsNullOrWhiteSpace(model.ProductTag))
                {
                    cmd.Parameters.AddWithValue("@ProductTag", model.ProductTag.Trim());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ProductTag", null);
                }

                if (!string.IsNullOrWhiteSpace(model.ProductDescribe))
                {
                    cmd.Parameters.AddWithValue("@ProductDescribe", model.ProductDescribe.Trim());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ProductDescribe", DBNull.Value);
                }

               

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        public static int DelTagID(SqlDbHelper dbhelper, int TagID)
        {
            using (var cmd = new SqlCommand(@"DELETE FROM Configuration.[dbo].[tbl_ProductServiceTagObject] WHERE TagID=@TagID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@TagID", TagID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static DataTable VaildatePID(string strPIDs)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("[Configuration]..[ServiceTag_VaildatePID]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pids", strPIDs);
                return dbhelper.ExecuteDataTable(cmd);
            }

        }

        public static int InsertAndUpdateTag(IEnumerable<ServiceTagModel> list)
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
                        if (model.PKID == 0)//新增服务标签
                        {
                            var result = InsertServiceTag(dbhelper, model);
                            if (result > 0)//新增服务标签成功,返回PKID
                            {
                                if (!string.IsNullOrWhiteSpace(model.StrServiceIDs))
                                {
                                    var serviceids = model.StrServiceIDs.Split(';').Distinct().ToArray();//去重
                                    foreach (var serviceid in serviceids)
                                    {
                                        if (serviceid != "")
                                        {
                                            var res = InsertTagID(dbhelper, result, model.Type, serviceid,model.ServiceDescribeTID);//新增TagID
                                            if (res <= 0)//新增失败 回滚 返回
                                            {
                                                dbhelper.Rollback();
                                                return res;
                                            }
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

                            var result = UpdateServiceTag(dbhelper, model);
                            if (result > 0)//修改规则成功
                            {
                                if (!string.IsNullOrWhiteSpace(model.StrServiceIDs))//该规则有城市
                                {
                                    var serviceids = model.StrServiceIDs.Split(';').Distinct().ToArray();//去重
                                    DelTagID(dbhelper, model.PKID);//删除原有TagID 删除失败则回滚
                                    foreach (var serviceid in serviceids)
                                    {
                                        if (serviceid != "")
                                        {
                                            var res = InsertTagID(dbhelper, model.PKID, model.Type, serviceid,model.ServiceDescribeTID);//新增TagID
                                            if (res <= 0)//新增城市失败 回滚 返回
                                            {
                                                dbhelper.Rollback();
                                                return res;
                                            }
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
                catch 
                {
                    dbhelper.Rollback();
                    return -11;
                }
                dbhelper.Commit();
               
                
            }
            return 99;
        }
    }
}
