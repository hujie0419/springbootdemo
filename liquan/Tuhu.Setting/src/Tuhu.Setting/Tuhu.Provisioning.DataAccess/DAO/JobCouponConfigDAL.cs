using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class JobCouponConfigDAL
    {
        public static IEnumerable<JobCouponConfigModel> Select(SqlConnection sqlconn, int pageIndex, int pageSieze)
        {
            string sql = @"SELECT * FROM
                           (
	                           SELECT ROW_NUMBER() OVER(ORDER BY tab1.CreateTime DESC,tab1.ID DESC)AS RowNumver,* FROM
	                           (
		                           SELECT (SELECT COUNT(1) FROM Configuration..SE_JobCouponConfig WITH(NOLOCK)) AS 'JobCount',* FROM Configuration..SE_JobCouponConfig WITH(NOLOCK)
	                           )AS tab1
                           )AS tab2 
                           WHERE tab2.RowNumver BETWEEN @BeginPage AND @EndPage
                           ORDER BY tab2.RowNumver";

            var sqlParas = new SqlParameter[] {
                new SqlParameter("@BeginPage",(pageIndex - 1 * pageSieze)),
                new SqlParameter("@EndPage",pageIndex * pageSieze)
            };

            return SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, sql, sqlParas).ConvertTo<JobCouponConfigModel>();
        }

        public static JobCouponConfigModel SelectById(SqlConnection sqlconn, int id)
        {
            string sql = @"SELECT * FROM Configuration..SE_JobCouponConfig WITH(NOLOCK) WHERE ID = @ID";

            var sqlParas = new SqlParameter[] {
                new SqlParameter("@ID",id)
            };

            return SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, sql, sqlParas).ConvertTo<JobCouponConfigModel>().FirstOrDefault();
        }

        public static bool Insert(SqlConnection sqlconn, JobCouponConfigModel model)
        {
            string sql = @"
                            INSERT INTO Configuration..SE_JobCouponConfig
                                    ( ActivityName ,
                                      RuleID ,
                                      CouponNum ,
                                      CouponName ,
                                      CouponExplain ,
                                      ValidityTime ,
                                      ProductType ,
                                      OrderState,
                                      OrderMoney,
                                      ReturnType ,
                                      State ,
                                      CouponRules ,
                                      CreateTime
                                    )
                            VALUES  ( @ActivityName,
                                      @RuleID,
                                      @CouponNum,
                                      @CouponName,
                                      @CouponExplain,
                                      @ValidityTime,
                                      @ProductType ,
                                      @OrderState,
                                      @OrderMoney,
                                      @ReturnType,
                                      @State,
                                      @CouponRules,
                                      @CreateTime
                                    )";

            var sqlParas = new SqlParameter[] {
                new SqlParameter("@ActivityName",model.ActivityName),
                new SqlParameter("@RuleID",model.RuleID),
                new SqlParameter("@CouponNum",model.CouponNum),
                new SqlParameter("@CouponName",model.CouponName),
                new SqlParameter("@CouponExplain",model.CouponExplain),
                new SqlParameter("@ValidityTime",model.ValidityTime),
                new SqlParameter("@ProductType",model.ProductType),
                new SqlParameter("@OrderState",model.OrderState),
                new SqlParameter("@OrderMoney",model.OrderMoney),
                new SqlParameter("@ReturnType",model.ReturnType),
                new SqlParameter("@State",model.State),
                new SqlParameter("@CouponRules",model.CouponRules),
                new SqlParameter("@CreateTime",model.CreateTime)
            };

            return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, sql, sqlParas) > 0;
        }

        public static bool Update(SqlConnection sqlconn,JobCouponConfigModel model)
        {
            string sql = @" 
                            UPDATE Configuration..[SE_JobCouponConfig]
                            SET [ActivityName] = @ActivityName
                              ,[RuleID] = @RuleID
                              ,[CouponNum] = @CouponNum
                              ,[CouponName] = @CouponName
                              ,[CouponExplain] = @CouponExplain
                              ,[ValidityTime] = @ValidityTime
                              ,[ProductType] = @ProductType
                              ,[OrderState] = @OrderState
                              ,[OrderMoney] = @OrderMoney
                              ,[ReturnType] = @ReturnType
                              ,[State] = @State
                              ,[CouponRules] = @CouponRules
                              ,[CreateTime] = @CreateTime
                           WHERE ID = @ID ";

            var sqlParas = new SqlParameter[] {
                new SqlParameter("@ActivityName",model.ActivityName),
                new SqlParameter("@RuleID",model.RuleID),
                new SqlParameter("@CouponNum",model.CouponNum),
                new SqlParameter("@CouponName",model.CouponName),
                new SqlParameter("@CouponExplain",model.CouponExplain),
                new SqlParameter("@ValidityTime",model.ValidityTime),
                new SqlParameter("@ProductType",model.ProductType),
                new SqlParameter("@OrderState",model.OrderState),
                new SqlParameter("@OrderMoney",model.OrderMoney),
                new SqlParameter("@ReturnType",model.ReturnType),
                new SqlParameter("@State",model.State),
                new SqlParameter("@CouponRules",model.CouponRules),
                new SqlParameter("@CreateTime",model.CreateTime),
                new SqlParameter("@ID",model.ID)
            };

            return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, sql, sqlParas) > 0;
        }
    }
}