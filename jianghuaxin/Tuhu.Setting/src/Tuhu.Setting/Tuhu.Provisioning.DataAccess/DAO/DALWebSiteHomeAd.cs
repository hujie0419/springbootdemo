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
    public class DALWebSiteHomeAd
    {
        /// <summary>
        /// 查找相应ID的广告位及广告位下广告的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable SelectAdDetailByID(string id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("Gungnir..[Advertise_SelectAdDetailByID]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }
        // <summary>
        /// 查找相应ID的广告下产品的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable SelectAdProductByID(string id)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("Gungnir..[Advertise_SelectAdProductByID]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// 查找所有广告位及广告位下广告的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable SelectAllAdDetail(string idstart)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("Gungnir..[Advertise_SelectAllAdDetail]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IDStart", idstart);
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// 查找所有广告位及广告位下广告的信息
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllAdProducts()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("Gungnir..[Advertise_SelectAllAdProducts]");
                cmd.CommandType = CommandType.StoredProcedure;
                return dbhelper.ExecuteDataTable(cmd);
            }
        }

        /// <summary>
        /// 新增广告位
        /// </summary>
        /// <param name="AdcModel"></param>
        /// <returns></returns>
        public static int InsertAdDetail(AdColumnModel AdcModel)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                var cmd = new SqlCommand("Gungnir..[Advertise_InsertAdcolumnDetail]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", AdcModel.ID);
                cmd.Parameters.AddWithValue("@Name", AdcModel.ADCName);
                cmd.Parameters.AddWithValue("@DefaultImage", AdcModel.DefaultImage);
                cmd.Parameters.AddWithValue("@DefaultUrl", AdcModel.DefaultUrl);
                cmd.Parameters.AddWithValue("@DefaultbgColor", AdcModel.DefaultbgColor);
                cmd.Parameters.AddWithValue("@Remark", AdcModel.Remark);
                var result = dbhelper.ExecuteNonQuery(cmd);
                if (result > 0)
                {
                    if (AdcModel.Products != null && AdcModel.Products.Count() > 0)
                    {
                        foreach (var p in AdcModel.Products)
                        {
                            var res = InsertProducts(dbhelper, p);

                            if (res <= 0)
                            {
                                dbhelper.Rollback();
                                if (res == -99)
                                    return -99;
                                else
                                    return -2;
                            }
                        }
                    }
                }
                else
                {
                    dbhelper.Rollback();
                    return -1;
                }
                dbhelper.Commit();
                return result;
            }
        }
        /// <summary>
        /// 新增广告信息
        /// </summary>
        /// <param name="ad"></param>
        /// <returns></returns>
        public static int InsertAdvertiseDetail(AdvertiseModel ad, IEnumerable<AdProductModel> products)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            var AdvertiseID = 0;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                var cmd = new SqlCommand("Gungnir..[Advertise_InsertAdvertiseDetail]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AdColumnID", ad.AdColumnID);
                cmd.Parameters.AddWithValue("@AdName", ad.AdName);
                cmd.Parameters.AddWithValue("@BeginDateTime", ad.BeginDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", ad.EndDateTime);
                cmd.Parameters.AddWithValue("@Position", ad.Position);
                cmd.Parameters.AddWithValue("@Image", ad.Image);
                cmd.Parameters.AddWithValue("@State", ad.State);
                cmd.Parameters.AddWithValue("@Url", ad.Url);
                cmd.Parameters.AddWithValue("@bgColor", ad.bgColor);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@Result",
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                });
                dbhelper.ExecuteNonQuery(cmd);
                AdvertiseID = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                if (AdvertiseID > 0)
                {
                    if (products != null && products.Count() > 0)
                    {
                        foreach (var p in products)
                        {
                            p.AdvertiseID = AdvertiseID;
                            var result = InsertProducts(dbhelper, p);
                            if (result < 0)
                            {
                                dbhelper.Rollback();
                                return -2;
                            }
                        }
                    }
                }
                else
                {
                    dbhelper.Rollback();
                    return -1;
                }
                dbhelper.Commit();
                return AdvertiseID;
            }
        }

        /// <summary>
        /// 插入广告产品
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        public static int InsertProducts(SqlDbHelper dbhelper, AdProductModel products)
        {
            using (var cmd = new SqlCommand("Gungnir..[Advertise_InsertProducts]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AdvertiseID", products.AdvertiseID);
                cmd.Parameters.AddWithValue("@AdColumnID", products.AdColumnID);
                cmd.Parameters.AddWithValue("@PID", products.PID);
                cmd.Parameters.AddWithValue("@Position", products.Position);
                cmd.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);

                try
                {
                    return dbhelper.ExecuteNonQuery(cmd);
                }
                catch
                {
                    return -99;
                }

            }
        }


        /// <summary>
        /// 更新广告产品
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        //public static int UpdateProducts(SqlDbHelper dbhelper, AdProductModel products)
        //{
        //    using (var cmd = new SqlCommand("Gungnir..[Advertise_UpdateProducts]"))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@AdvertiseID", products.AdvertiseID);
        //        cmd.Parameters.AddWithValue("@AdColumnID", products.AdColumnID);
        //        cmd.Parameters.AddWithValue("@PID", products.PID);
        //        cmd.Parameters.AddWithValue("@Position", products.Position);
        //        cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
        //        return dbhelper.ExecuteNonQuery(cmd);
        //    }
        //}
        /// <summary>
        /// 更新广告位信息   （默认图片和链接）
        /// </summary>
        /// <param name="AdcModel"></param>
        /// <returns></returns>
        public static int UpdateAdColumn(AdColumnModel AdcModel)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                var cmd = new SqlCommand("Gungnir..[Advertise_UpdateAdColumn]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", AdcModel.ID);
                cmd.Parameters.AddWithValue("@DefaultImage", AdcModel.DefaultImage);
                cmd.Parameters.AddWithValue("@Name", AdcModel.ADCName);
                cmd.Parameters.AddWithValue("@DefaultbgColor", AdcModel.DefaultbgColor);
                cmd.Parameters.AddWithValue("@DefaultUrl", AdcModel.DefaultUrl);
                cmd.Parameters.AddWithValue("@Remark", AdcModel.Remark);
                var result = dbhelper.ExecuteNonQuery(cmd);
                if (result > 0)
                {
                    if (AdcModel.Products != null && AdcModel.Products.Count() > 0)
                    {
                        if (DeleteProducts(AdcModel.ID, "0") > 0)
                        {
                            foreach (var p in AdcModel.Products)
                            {
                                if (InsertProducts(dbhelper, p) <= 0)
                                {
                                    dbhelper.Rollback();
                                    return -2;
                                }
                            }
                        }
                        else
                        {
                            dbhelper.Rollback();
                            return -3;
                        }
                    }
                    else if (AdcModel.Remark == "product")
                    {
                        dbhelper.Rollback();
                        return -98;//一个产品都没有无需保存
                    }
                }
                else
                {
                    dbhelper.Rollback();
                    return -1;
                }
                dbhelper.Commit();
                return result;
            }
        }
        /// <summary>
        /// 更新广告信息
        /// </summary>
        /// <param name="AdvertiseModel"></param>
        /// <returns></returns>
        public static int UpdateAdvertise(AdvertiseModel AdvertiseModel, IEnumerable<AdProductModel> products, out int result)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                dbhelper.BeginTransaction();
                var cmd = new SqlCommand("Gungnir..[Advertise_UpdateAdvertise]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AdColumnID", AdvertiseModel.AdColumnID);
                cmd.Parameters.AddWithValue("@AdName", AdvertiseModel.AdName);
                cmd.Parameters.AddWithValue("@PKID", AdvertiseModel.PKID);
                cmd.Parameters.AddWithValue("@BeginDateTime", AdvertiseModel.BeginDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", AdvertiseModel.EndDateTime);
                cmd.Parameters.AddWithValue("@Position", AdvertiseModel.Position);
                cmd.Parameters.AddWithValue("@Image", AdvertiseModel.Image);
                cmd.Parameters.AddWithValue("@State", AdvertiseModel.State);
                cmd.Parameters.AddWithValue("@Url", AdvertiseModel.Url);
                cmd.Parameters.AddWithValue("@bgColor", AdvertiseModel.bgColor);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@Result",
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                });
                dbhelper.ExecuteNonQuery(cmd);
                result = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                if (result > 0)
                {
                    if (products != null && products.Count() > 0)
                    {
                        if (DeleteProducts(AdvertiseModel.AdColumnID, AdvertiseModel.PKID.ToString()) > 0)
                        {
                            foreach (var p in products)
                            {
                                p.AdvertiseID = AdvertiseModel.PKID;
                                if (InsertProducts(dbhelper, p) <= 0)
                                {
                                    dbhelper.Rollback();
                                    return -2;
                                }
                            }
                        }
                        else
                        {
                            dbhelper.Rollback();
                            return -3;
                        }
                    }
                    else if (AdvertiseModel.AdColumnID.Contains("F_11"))
                    {
                        dbhelper.Rollback();
                        result = -98;
                    }
                }
                else
                {
                    dbhelper.Rollback();
                    return -1;
                }
                dbhelper.Commit();
                return result;
            }
        }
        /// <summary>
        /// 删除广告
        /// </summary>
        /// <param name="AdColumnID"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static int DeleteAdvertise(int PKID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand("DELETE FROM Gungnir..tbl_Advertise WHERE  PKID=@PKID");
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 删除单独广告产品
        /// </summary>
        /// <param name="AdColumnID"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static int DeleteProducts(string AdColumnID, string AdvertiseID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var sql = "DELETE FROM Gungnir..tbl_AdProduct WHERE  AdColumnID=@AdColumnID AND AdvertiseID = @AdvertiseID";
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@AdColumnID", AdColumnID);
                cmd.Parameters.AddWithValue("@AdvertiseID", AdvertiseID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

    }
}
