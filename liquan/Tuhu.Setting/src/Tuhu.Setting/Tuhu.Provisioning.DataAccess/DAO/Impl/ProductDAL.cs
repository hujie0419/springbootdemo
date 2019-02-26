using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.DAO.Interface;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.Impl
{
    /// <summary>
    /// 产品表 sqlserver Dao 层具体实现
    /// </summary>
    public class ProductDAL : IProductDAL
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private static readonly string connstring = ConnectionHelper.Gungnir;

        /// <summary>
        /// 通过appSetDataId 获取产品
        /// </summary>
        /// <param name="appSetDataId"></param>
        /// <returns></returns>
        public IEnumerable<TblAdProductV2> GetProductByNewsAppSetDataId(int appSetDataId)
        {
            using (SqlConnection SqlServerConn = new SqlConnection(connstring))
            {
                string sql = @"SELECT  *
                                FROM Gungnir..[tbl_AdProduct_v2]
                                        WHERE newappsetdataId = @appsetdataId";
                SqlParameter parm = new SqlParameter("@appsetdataId", appSetDataId);
                return SqlHelper.ExecuteDataTable(SqlServerConn, CommandType.Text, sql, parm).ConvertTo<TblAdProductV2>().ToList();
            }
        }

        /// <summary>
        /// 添加一个产品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(TblAdProductV2 entity)
        {
            string sql = @"
                            INSERT  INTO Gungnir..[tbl_AdProduct_v2]
                                    ( PID ,
                                      Position ,
                                      State ,
                                      CreateDateTime ,
                                      PromotionPrice ,
                                      PromotionNum ,
                                      newappsetdataId ,
                                      ActivityId
                                    )
                            VALUES  ( @PID ,
                                      @Position ,
                                      @State ,
                                      GETDATE() ,
                                      @PromotionPrice ,
                                      @PromotionNum ,
                                      @newappsetdataId ,
                                      @ActivityId     
                                    )";

            SqlParameter[] sqlPrams = new SqlParameter[]
            {
               new SqlParameter("@PID",entity.PId),
               new SqlParameter("@Position",entity.Position),
               new SqlParameter("@State",entity.State),
               new SqlParameter("@PromotionPrice",entity.PromotionPrice),
               new SqlParameter("@PromotionNum",entity.PromotionNum),
               new SqlParameter("@newappsetdataId",entity.NewAppSetDataId),
               new SqlParameter("@ActivityId",entity.ActivityId)
            };

            //释放 connection资源
            using (SqlConnection SqlServerConn = new SqlConnection(connstring))
            {
                return SqlHelper.ExecuteNonQuery(SqlServerConn, CommandType.Text, sql, sqlPrams);
            }
        }

        /// <summary>
        /// 删除一个产品 (暂未实现，以后有需要再实现.)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Remove(TblAdProductV2 entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新一个产品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(TblAdProductV2 entity)
        {
            string sql = @" UPDATE  Gungnir..[tbl_AdProduct_v2]
                            SET     PID = @PID ,
                                    Position = @Position ,
                                    State = @State ,
                                    LastUpdateDateTime = GETDATE() ,
                                    PromotionPrice = @PromotionPrice ,
                                    PromotionNum = @PromotionNum ,
                                    newappsetdataId = @newappsetdataId ,
                                    ActivityId = @ActivityId
                            WHERE   ID = @ID";
            SqlParameter[] sqlPrams = new SqlParameter[]{
                new SqlParameter("@PID",entity.PId),
                new SqlParameter("@Position",entity.Position),
                new SqlParameter("@State",entity.State),
                new SqlParameter("@PromotionPrice",entity.PromotionPrice),
                new SqlParameter("@PromotionNum",entity.PromotionNum),
                new SqlParameter("@newappsetdataId",entity.NewAppSetDataId),
                new SqlParameter("@ID",entity.Id),
                new SqlParameter("@ActivityId",entity.ActivityId)
            };

            using (SqlConnection SqlServerConn = new SqlConnection(connstring))
            {
                return SqlHelper.ExecuteNonQuery(SqlServerConn, CommandType.Text, sql, sqlPrams);
            }
        }



        /// <summary>
        /// 获取单一的产品
        /// </summary>
        /// <param name="PrimaryKey"></param>
        /// <returns></returns>
        public TblAdProductV2 SelectSingle(int PrimaryKey)
        {
            string sql = "select * from Gungnir..[tbl_AdProduct_v2] where ID = @ID";

            SqlParameter param = new SqlParameter("@ID", PrimaryKey);
            using (SqlConnection SqlServerConn = new SqlConnection(connstring))
            {
                return SqlHelper.ExecuteDataTable(SqlServerConn, CommandType.Text, sql, param).ConvertTo<TblAdProductV2>().ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// 通过产品id获取产品数量
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public int GetProductCountById(int productId)
        {
            string sql = "select count(*) from Gungnir..[tbl_AdProduct_v2] where ID = @ID";
            SqlParameter parm = new SqlParameter("@ID", productId);

            using (SqlConnection SqlServerConn = new SqlConnection(connstring))
            {
                return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlServerConn, CommandType.Text, sql, parm));
            }
        }
    }
}
