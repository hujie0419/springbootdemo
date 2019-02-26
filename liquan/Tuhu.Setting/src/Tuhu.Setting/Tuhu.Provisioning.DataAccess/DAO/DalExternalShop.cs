using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalExternalShop
    {
        public static ExternalShop GetExternalShop(SqlConnection connection, int shopId)
        {
            ExternalShop externalShop = null;

            var parameters = new[]
            {
                new SqlParameter("@ShopId", shopId)
            };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "Shop_GetExternalShopByShopId", parameters))
            {
                if (reader.Read())
                {
                    externalShop = new ExternalShop();
                    externalShop.PKID = reader.GetTuhuValue<int>(0);
                    externalShop.ShopId = reader.GetTuhuValue<int>(1);
                    externalShop.ShopCode = reader.GetTuhuString(2);
                    externalShop.ShopType = reader.GetTuhuValue<int>(3);
                    externalShop.Creator = reader.GetTuhuString(4);
                    externalShop.LastUpdatetime = reader.GetTuhuValue<DateTime>(5);
                    externalShop.EmailAddress = reader.GetTuhuString(6);
                    externalShop.CPShopId = reader.GetTuhuNullableValue<int>(7);
                    externalShop.IsExpressSelfHosted = reader.GetTuhuValue<bool>(8);
                }
            }

            return externalShop;
        }

        public static List<ExternalShop> SelectExternalShopsByShopType(SqlConnection connection, ExternalShopType shopType)
        {
            var externalShops = new List<ExternalShop>();

            var parameters = new[]
            {
                new SqlParameter("@ShopType", shopType)
            };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "Shop_SelectExternalShopsByShopType", parameters))
            {
                while (reader.Read())
                {
                    var externalShop = new ExternalShop();
                    externalShop.PKID = reader.GetTuhuValue<int>(0);
                    externalShop.ShopId = reader.GetTuhuValue<int>(1);
                    externalShop.ShopCode = reader.GetTuhuString(2);
                    externalShop.ShopType = reader.GetTuhuValue<int>(3);
                    externalShop.Creator = reader.GetTuhuString(4);
                    externalShop.LastUpdatetime = reader.GetTuhuValue<DateTime>(5);
                    externalShop.EmailAddress = reader.GetTuhuString(6);
                    externalShop.CPShopId = reader.GetTuhuNullableValue<int>(7);
                    externalShop.IsExpressSelfHosted = reader.GetTuhuValue<bool>(8);

                    externalShops.Add(externalShop);
                }
            }

            return externalShops;
        }

        public static int Insert(SqlConnection connection, ExternalShop externalShop)
        {
            var parameters = new[]
            {
                 new SqlParameter("@ShopId", externalShop.ShopId),
                 new SqlParameter("@ShopCode", externalShop.ShopCode ?? string.Empty),
                 new SqlParameter("@ShopType", externalShop.ShopType),
                 new SqlParameter("@Creator", externalShop.Creator?? string.Empty),
                 new SqlParameter("@LastUpdatetime", externalShop.LastUpdatetime),
                 new SqlParameter("@EmailAddress",externalShop.EmailAddress),
                 new SqlParameter("@CPShopId", externalShop.CPShopId.HasValue? (object)externalShop.CPShopId.Value : DBNull.Value),
                 new SqlParameter("@IsExpressSelfHosted", externalShop.IsExpressSelfHosted)
            };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, "Shop_InsertExternalShop", parameters));
        }

        public static void DeleteExternalShopByShopId(SqlConnection conn, int shopId)
        {
            var parameters = new[]
            {
                 new SqlParameter("@ShopId", shopId)
            };
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "Shop_DeleteExternalShopByShopId",
                parameters);
        }

        public static void UpadateExternalShopById(SqlConnection conn, int shopId, string shopCode, string emailAddress, int? cpShopId, bool isExpressSelfHosted)
        {
            var parameters = new[]
            {
                 new SqlParameter("@ShopId", shopId),
                 new SqlParameter("@ShopCode",shopCode),
                 new SqlParameter("@EmailAddress",emailAddress),
                 new SqlParameter("@CPShopId",cpShopId ?? (object)DBNull.Value),
                 new SqlParameter("@IsExpressSelfHosted",isExpressSelfHosted),
            };
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "Shop_UpdateExternalShopCodeByShopId",
                parameters);
        }
    }
}
