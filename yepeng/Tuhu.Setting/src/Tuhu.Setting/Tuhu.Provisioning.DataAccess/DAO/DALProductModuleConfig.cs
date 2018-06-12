using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALProductModuleConfig
    {
        public static DataTable SelectAllProductConfig(int pageSize, int pageIndex, string pid, int isActive, int isAdvert, string moduleName, out int totalCount)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }

            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("[Gungnir]..[ProductModel_GetAllConfigInfo]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PID", pid == "" ? null : pid);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.Parameters.AddWithValue("@IsAdvert", isAdvert);
                cmd.Parameters.AddWithValue("@ModuleName", moduleName);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output,
                    ParameterName = "@TotalCount",
                });
                DataTable data = dbhelper.ExecuteDataTable(cmd);
                totalCount = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return data;
            }
        }

        public static DataTable GetProductDescModuleDetail(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("[Gungnir]..[ProductModel_GetProductConfigByID]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModuleID", id);
                return dbhelper.ExecuteDataTable(cmd);
            }

        }

        public static int GetProductModuleMaxID(SqlDbHelper dbhelper)
        {
            using (var cmd = new SqlCommand("[Gungnir]..[ProductModel_GetProductModuleMaxID]"))
            {
                var result = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output,
                    ParameterName = "@result",
                });
                dbhelper.ExecuteNonQuery(cmd);
                result = Convert.ToInt32(cmd.Parameters["@result"].Value);

                return result;
            }
        }

        public static int InsertProductConfig(ProductDescriptionModel productModuleModel, List<ProductDescriptionModel> platformList,
            List<ProductDescriptionModel> categoryList, List<ProductDescriptionModel> pidList, List<ProductDescriptionModel> brandList,
            string userName, out int moduleID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                var result = 0;
                moduleID = DALProductModuleConfig.GetProductModuleMaxID(dbhelper) + 1;
                if (platformList.Count() <= 0)
                {
                    result = DALProductModuleConfig.InsertProductModule(dbhelper, productModuleModel, moduleID, null, userName);
                }
                else
                {
                    foreach (var item in platformList)
                    {
                        var r = DALProductModuleConfig.InsertProductModule(dbhelper, productModuleModel, moduleID, item, userName);
                        if (r <= 0)
                        {
                            dbhelper.Rollback();
                            moduleID = 0;
                            return -1;
                        }
                    }
                    result = 1;
                }
                if (result > 0)
                {
                    foreach (var item in categoryList)
                    {
                        item.ModuleID = moduleID;
                        var r = InsertProductConfigCategory(dbhelper, item, productModuleModel, userName);
                        if (r <= 0)
                        {
                            dbhelper.Rollback();
                            moduleID = 0;
                            return -1;
                        }
                    }
                    foreach (var item in pidList)
                    {
                        item.ModuleID = moduleID;
                        var r = InsertProductConfigPID(dbhelper, item, productModuleModel, userName);
                        if (r <= 0)
                        {
                            dbhelper.Rollback();
                            moduleID = 0;
                            return -1;
                        }
                    }
                    foreach (var item in brandList)
                    {
                        item.ModuleID = moduleID;
                        var r = InsertProductConfigBrand(dbhelper, item, userName);
                        if (r <= 0)
                        {
                            dbhelper.Rollback();
                            moduleID = 0;
                            return -1;
                        }
                    }

                    dbhelper.Commit();

                    return moduleID;
                }
                else
                {
                    dbhelper.Rollback();
                    moduleID = 0;
                    return -1;
                }
            }
        }


        public static int UpdateProductConfig(ProductDescriptionModel productModuleModel, List<ProductDescriptionModel> platformList,
            List<ProductDescriptionModel> categoryList, List<ProductDescriptionModel> pidList, List<ProductDescriptionModel> brandList,
            int moduleID, string userName)
        {
            var result = 0;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                if (DALProductModuleConfig.DeleteProductModuleItem(dbhelper, moduleID, userName) > 0)
                {
                    if (platformList.Count() <= 0)
                    {
                        result = DALProductModuleConfig.InsertProductModule(dbhelper, productModuleModel, moduleID, null, userName);
                    }
                    else
                    {
                        foreach (var item in platformList)
                        {
                            var r = DALProductModuleConfig.InsertProductModule(dbhelper, productModuleModel, moduleID, item, userName);
                            if (r <= 0)
                            {
                                dbhelper.Rollback();
                                return -1;
                            }
                        }
                        result = 1;
                    }
                }
                else
                {
                    dbhelper.Rollback();
                    return -1;
                }

                if (result > 0)
                {
                    if (DeleteProductConfigItem(dbhelper, moduleID, userName) > 0)
                    {
                        foreach (var item in categoryList)
                        {
                            item.ModuleID = moduleID;
                            var r = InsertProductConfigCategory(dbhelper, item, productModuleModel, userName);
                            if (r <= 0)
                            {
                                dbhelper.Rollback();
                                return -1;
                            }
                        }
                        foreach (var item in pidList)
                        {
                            item.ModuleID = moduleID;
                            var r = InsertProductConfigPID(dbhelper, item, productModuleModel, userName);
                            if (r <= 0)
                            {
                                dbhelper.Rollback();
                                return -1;
                            }
                        }
                        foreach (var item in brandList)
                        {
                            item.ModuleID = moduleID;
                            var r = InsertProductConfigBrand(dbhelper, item, userName);
                            if (r <= 0)
                            {
                                dbhelper.Rollback();
                                moduleID = 0;
                                return -1;
                            }
                        }
                    }
                    else
                    {
                        dbhelper.Rollback();
                        return -1;
                    }
                }
                else
                {
                    dbhelper.Rollback();
                    return -1;
                }
                dbhelper.Commit();
                return moduleID;
            }
        }

        public static int DeleteProductConfigItem(SqlDbHelper dbhelper, int PKID, string userName)
        {
            using (var cmd = new SqlCommand("[Gungnir]..[ProductModel_DeleteProductConfigByID]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModuleID", PKID);
                cmd.Parameters.AddWithValue("@DeleteUser", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }



        public static int DeleteProductModuleItem(SqlDbHelper dbhelper, int moduleID, string userName)
        {
            using (var cmd = new SqlCommand("[Gungnir]..[ProductModel_DeleteProductModuleByID]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModuleID", moduleID);
                cmd.Parameters.AddWithValue("@DeleteUser", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int InsertProductConfigCategory(SqlDbHelper dbhelper, ProductDescriptionModel categoryList, ProductDescriptionModel model,
            string userName)
        {
            using (var cmd = new SqlCommand("[Gungnir]..[ProductModel_AddConfigInfo]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModuleID", categoryList.ModuleID);
                cmd.Parameters.AddWithValue("@CategoryName", categoryList.CategoryName);
                cmd.Parameters.AddWithValue("@CreatedUser", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int InsertProductConfigPID(SqlDbHelper dbhelper, ProductDescriptionModel pidItem, ProductDescriptionModel productModuleModel,
            string userName)
        {

            using (var cmd = new SqlCommand("[Gungnir]..[ProductModel_AddConfigPIDInfo]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModuleID", pidItem.ModuleID);
                cmd.Parameters.AddWithValue("@PID", pidItem.PID);
                cmd.Parameters.AddWithValue("@AddOrDel", productModuleModel.AddOrDel);
                cmd.Parameters.AddWithValue("@CreatedUser", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int InsertProductConfigBrand(SqlDbHelper dbhelper, ProductDescriptionModel brandItem, string userName)
        {
            using (var cmd = new SqlCommand("[Gungnir]..[ProductModel_AddConfigBrandInfo]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModuleID", brandItem.ModuleID);
                cmd.Parameters.AddWithValue("@CategoryName", brandItem.CategoryName);
                cmd.Parameters.AddWithValue("@Brand", brandItem.Brand);
                cmd.Parameters.AddWithValue("@CreatedUser", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int InsertProductModule(SqlDbHelper dbhelper, ProductDescriptionModel model, int moduleID, ProductDescriptionModel platformItem,
            string userName)
        {
            using (var cmd = new SqlCommand("[Gungnir]..[ProductModel_InsertProductModuleInfo]"))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModuleID", moduleID);
                cmd.Parameters.AddWithValue("@ModuleName", model.ModuleName);
                cmd.Parameters.AddWithValue("@ModuleOrder", model.ModuleOrder);
                cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                cmd.Parameters.AddWithValue("@IsAdvert", model.IsAdvert);
                cmd.Parameters.AddWithValue("@ShowPlatform", model.IsAdvert == false ? model.ShowPlatform : platformItem.ShowPlatform);
                cmd.Parameters.AddWithValue("@BigImageUrl", model.IsAdvert == false ? null : platformItem.BigImageUrl);
                cmd.Parameters.AddWithValue("@SmallImageUrl", model.IsAdvert == false ? null : platformItem.SmallImageUrl);
                cmd.Parameters.AddWithValue("@AppHandlekey", model.IsAdvert == false ? null : platformItem.AppHandleKey);
                cmd.Parameters.AddWithValue("@AppSpecialkey", model.IsAdvert == false ? null : platformItem.AppSpecialKey);
                cmd.Parameters.AddWithValue("@URL", model.IsAdvert == false ? null : platformItem.URL);
                cmd.Parameters.AddWithValue("@ModuleContent", model.IsAdvert == false ? (model.ModuleContent == "" ? null : model.ModuleContent) : (platformItem.ModuleContent == "" ? null : platformItem.ModuleContent));
                cmd.Parameters.AddWithValue("@CreatedUser", userName);

                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int DeleteProductAllInfo(int pkid, string userName)
        {
            var result = 0;
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                result = DeleteProductModuleItem(dbhelper, pkid, userName);
                if (result > 0)
                {
                    if (DeleteProductConfigItem(dbhelper, pkid, userName) > 0)
                    {
                        dbhelper.Commit();
                        return result;
                    }
                    else
                    {
                        dbhelper.Rollback();
                        return -1;
                    }
                }
                else
                {
                    dbhelper.Rollback();
                    return -1;
                }
            }
        }
    }
}
