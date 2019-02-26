using Microsoft.ApplicationBlocks.Data;
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
    public class DALOrderAreaProduct
    {
        private static readonly string gungnirConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly string connectionString = SecurityHelp.IsBase64Formatted(gungnirConn) ? SecurityHelp.DecryptAES(gungnirConn) : gungnirConn;

        #region  订单完成区域配置
        /// <summary>
        /// OrderAreaId 获取产品
        /// </summary>
        /// <param name="OrderAreaId"></param>
        /// <returns></returns>
        public static IEnumerable<OrderAreaProduct> GetProductByOrderAreaId(int OrderAreaId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                const string sql = @"SELECT * FROM Gungnir..[OrderFinishPageSettingProduct]  WITH (NOLOCK) WHERE OrderAreaId = @OrderAreaId";
                SqlParameter parm = new SqlParameter("@OrderAreaId", OrderAreaId);
                return SqlHelper.ExecuteDataTable(sqlConnection, CommandType.Text, sql, parm).ConvertTo<OrderAreaProduct>().ToList();
            }
        }

        /// <summary>
        /// 添加一个产品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int AddOrderAreaProduct(OrderAreaProduct entity)
        {
            const string sql = @"INSERT  INTO Gungnir..[OrderFinishPageSettingProduct]
                                    ( PID ,
                                      Position ,
                                      State ,
                                      CreateDateTime ,
                                      PromotionPrice ,
                                      PromotionNum ,
                                      OrderAreaId ,
                                      ActivityId
                                    )
                            VALUES  ( @PID ,
                                      @Position ,
                                      @State ,
                                      GETDATE() ,
                                      @PromotionPrice ,
                                      @PromotionNum ,
                                      @OrderAreaId  ,
                                      @ActivityId     
                                    )";

            SqlParameter[] sqlPrams = new SqlParameter[]
            {
               new SqlParameter("@PID",entity.PId),
               new SqlParameter("@Position",entity.Position),
               new SqlParameter("@State",entity.State),
               new SqlParameter("@PromotionPrice",entity.PromotionPrice),
               new SqlParameter("@PromotionNum",entity.PromotionNum),
               new SqlParameter("@OrderAreaId",entity.OrderAreaId),
               new SqlParameter("@ActivityId",entity.ActivityId)
            };
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                return SqlHelper.ExecuteNonQuery(sqlConnection, CommandType.Text, sql, sqlPrams);
            }
        }


        /// <summary>
        /// 更新一个产品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int UpdateOrderAreaProduct(OrderAreaProduct entity)
        {
            const string sql = @"UPDATE  Gungnir..[OrderFinishPageSettingProduct]
                        SET     PID = @PID ,
                                Position = @Position ,
                                State = @State ,
                                LastUpdateDateTime = GETDATE() ,
                                PromotionPrice = @PromotionPrice ,
                                PromotionNum = @PromotionNum ,
                                OrderAreaId = @OrderAreaId ,
                                ActivityId = @ActivityId
                        WHERE   ID = @ID";
            SqlParameter[] sqlPrams = new SqlParameter[]{
                new SqlParameter("@PID",entity.PId),
                new SqlParameter("@Position",entity.Position),
                new SqlParameter("@State",entity.State),
                new SqlParameter("@PromotionPrice",entity.PromotionPrice),
                new SqlParameter("@PromotionNum",entity.PromotionNum),
                new SqlParameter("@OrderAreaId",entity.OrderAreaId),
                new SqlParameter("@ID",entity.Id),
                new SqlParameter("@ActivityId",entity.ActivityId)
            };

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                return SqlHelper.ExecuteNonQuery(sqlConnection, CommandType.Text, sql, sqlPrams);
            }
        }

        /// <summary>
        /// 获取单一的产品
        /// </summary>
        /// <param name="PrimaryKey"></param>
        /// <returns></returns>
        public static OrderAreaProduct SelectSingleOrderAreaProduct(int PrimaryKey)
        {
            const string sql = @"SELECT * FROM Gungnir..[OrderFinishPageSettingProduct]  WITH (NOLOCK) WHERE ID = @ID";

            SqlParameter param = new SqlParameter("@ID", PrimaryKey);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                return SqlHelper.ExecuteDataTable(sqlConnection, CommandType.Text, sql, param).ConvertTo<OrderAreaProduct>().ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// 通过产品id获取产品数量
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static int GetOrderAreaProductCountById(int productId)
        {
            const string sql = @"SELECT COUNT(*) FROM Gungnir..[OrderFinishPageSettingProduct]  WITH (NOLOCK) WHERE ID = @ID";
            SqlParameter parm = new SqlParameter("@ID", productId);

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                return Convert.ToInt32(SqlHelper.ExecuteScalar(sqlConnection, CommandType.Text, sql, parm));
            }
        }
        #endregion
    }
}
