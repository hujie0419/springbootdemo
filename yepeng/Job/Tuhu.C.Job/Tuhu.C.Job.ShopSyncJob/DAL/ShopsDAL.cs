using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Log;
using Tuhu.Service.Shop.Models;

namespace Tuhu.C.Job.ShopSyncJob.DAL
{
    public class ShopsDAL
    {
        public static readonly string DBConntionStr = ConfigurationManager.ConnectionStrings["ThirdParty"].ConnectionString;

        public static void CreateThirdPartyShop(ShopDetailModel shopDetail)
        {
            try
            {
                using (
                    var dbhelper =
                        DbHelper.CreateDbHelper(ConfigurationManager.ConnectionStrings["ThirdParty"].ConnectionString))
                {
                    using (
                        var cmd =
                            new SqlCommand(
                                "INSERT INTO ThirdPartySyncShops" +
                                " VALUES (@PKID, @SimpleName, @CarparName, @FullName, @CompanyName, @RegionID, @Province, @CityID, @City, @DistrictID," +
                                " @District, @Address, @AddressBrief, @Position, @Contact, @Telephone,@Mobile,@Cover,@POS," +
                                " @WorkTime,@Images,@ShopType,@SuspendStartDateTime,@SuspendEndDateTime,@ShopLevel,@ServiceType,@ShopAICUrl,@MetalServiceType,@Date_CreateTime,@Date_ChangeTime)")
                            { CommandTimeout = 10 * 60 }
                        )
                    {
                        DateTime time = DateTime.Now;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PKID", shopDetail.ShopId);
                        cmd.Parameters.AddWithValue("@SimpleName", shopDetail.SimpleName);
                        cmd.Parameters.AddWithValue("@CarparName", shopDetail.CarparName);
                        cmd.Parameters.AddWithValue("@FullName", shopDetail.FullName);
                        cmd.Parameters.AddWithValue("@CompanyName", shopDetail.CompanyName);
                        cmd.Parameters.AddWithValue("@RegionID", shopDetail.ProvinceId);
                        cmd.Parameters.AddWithValue("@Province", shopDetail.Province);
                        cmd.Parameters.AddWithValue("@CityID", shopDetail.CityId);
                        cmd.Parameters.AddWithValue("@City", shopDetail.City);
                        cmd.Parameters.AddWithValue("@DistrictID", shopDetail.DistrictId);
                        cmd.Parameters.AddWithValue("@District", shopDetail.District);
                        cmd.Parameters.AddWithValue("@Address", shopDetail.Address);
                        cmd.Parameters.AddWithValue("@AddressBrief", shopDetail.AddressBrief);
                        //cmd.Parameters.AddWithValue("@Description", shopDetail);
                        cmd.Parameters.AddWithValue("@Position",
                            (shopDetail.Position != null)
                                ? string.Join(",", shopDetail.Position.Where(q => q > 0).Select(q => q).ToList())
                                : string.Empty);
                        cmd.Parameters.AddWithValue("@Contact", shopDetail.Contact);
                        cmd.Parameters.AddWithValue("@Telephone", shopDetail.Telephone);
                        cmd.Parameters.AddWithValue("@Mobile", shopDetail.Mobile);
                        cmd.Parameters.AddWithValue("@Cover",
                            (shopDetail.Cover != null)
                                ? string.Join(",", shopDetail.Cover.Where(q => q != null).Select(q => q).ToList())
                                : string.Empty);
                        cmd.Parameters.AddWithValue("@POS", shopDetail.Pos);
                        cmd.Parameters.AddWithValue("@WorkTime", shopDetail.WorkTime);
                        cmd.Parameters.AddWithValue("@Images",
                            (shopDetail.Images != null && shopDetail.Images.FirstOrDefault() != null)
                                ? shopDetail.Images[0]
                                : string.Empty);
                        cmd.Parameters.AddWithValue("@ShopType", shopDetail.ShopType);
                        cmd.Parameters.AddWithValue("@SuspendStartDateTime", shopDetail.SuspendStartDateTime);
                        cmd.Parameters.AddWithValue("@SuspendEndDateTime", shopDetail.SuspendEndDateTime);
                        cmd.Parameters.AddWithValue("@ShopLevel", shopDetail.ShopLevel != null ? shopDetail.ShopLevel.ShopStarLevel : 0);
                        // cmd.Parameters.AddWithValue("@ShopBusinessType", shopDetail.shopb);
                        cmd.Parameters.AddWithValue("@ServiceType", shopDetail.ServiceType);
                        cmd.Parameters.AddWithValue("@ShopAICUrl",
                            shopDetail.ShopAICUrl != null ? shopDetail.ShopAICUrl : string.Empty);
                        cmd.Parameters.AddWithValue("@MetalServiceType", shopDetail.MetalServiceType);
                        cmd.Parameters.AddWithValue("@Date_CreateTime", time);
                        cmd.Parameters.AddWithValue("@Date_ChangeTime", time);
                        dbhelper.ExecuteNonQuery(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                TuhuShopSyncJob.Logger.Error("ShopID:" + shopDetail.ShopId + "occur error", ex);
            }

        }

        public static IEnumerable<int> GetValidThirdPartyShopIds()
        {

            using (
                var dbhelper =
                    DbHelper.CreateDbHelper(
                        ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString))
            {
                using (var cmd = new SqlCommand(ShopSqlText.SqlTextAllThirdPartyShopIds) { CommandTimeout = 10 * 60 })
                {
                    #region AddParameters

                    //  cmd.Parameters.AddWithValue("@VehicleId", vehicleId)

                    #endregion

                    return dbhelper.ExecuteQuery(cmd, dt => dt.ToList<int>("PKID"));
                }
            }
        }

        public static IEnumerable<int> GetAllThirdPartyShopIds()
        {

            using (
                var dbhelper =
                    DbHelper.CreateDbHelper(
                        ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString))
            {
                using (var cmd = new SqlCommand(ShopSqlText.SqlTextAllThirdPartyShopIds) { CommandTimeout = 10 * 60 })
                {
                    #region AddParameters

                    //  cmd.Parameters.AddWithValue("@VehicleId", vehicleId)

                    #endregion

                    return dbhelper.ExecuteQuery(cmd, dt => dt.ToList<int>("PKID"));
                }
            }
        }

        public static bool DeleteThirdPartyShops()
        {
            bool result = true;
            try
            {
                int count = 0;
                using (var dbhelper = DbHelper.CreateDbHelper(ConfigurationManager.ConnectionStrings["ThirdParty"].ConnectionString))
                using (var cmd = new SqlCommand(ShopSqlText.SqlTextDeleteThirdPartyShops) { CommandTimeout = 10 * 60 })
                {
                    #region AddParameters

                    //  cmd.Parameters.AddWithValue("@VehicleId", vehicleId)

                    #endregion

                    count = DbHelper.ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                result = false;

            }
            return result;
        }


        public static bool DeleteShopInDB(int shopid)
        {
            using (var dbhelper = DbHelper.CreateDbHelper(ConfigurationManager.ConnectionStrings["ThirdParty"].ConnectionString))
            {
                string sql = @"                    
                     delete  from Tuhu_thirdparty..ThirdPartySyncShops where PKID=@shopid                                      
                    ";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@shopid", shopid);

                return dbhelper.ExecuteNonQuery(cmd) > 0 ? true : false;
            }
        }

        public static bool ShopIsExistedInDB(int shopid)
        {
            bool isExisted = false;
            using (
             var dbhelper =
                 DbHelper.CreateDbHelper(
                     ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString))
            {

               
                int pkid = 0;
               
                string sql = @"                    
                     select top 1 PKID  from Tuhu_thirdparty..ThirdPartySyncShops where PKID=@shopid                                      
                    ";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@shopid", shopid);
                object result = dbhelper.ExecuteScalar(cmd);
                if (result!=null && Int32.TryParse(result.ToString(), out pkid) && pkid > 0)
                {
                    isExisted = true;
                }

            }
            return isExisted;
        }
    }

}
