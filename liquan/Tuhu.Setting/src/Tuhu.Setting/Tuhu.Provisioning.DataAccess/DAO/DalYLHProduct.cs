using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Data.SqlClient;
using System.Data;


namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalYLHProduct
    {
        /// <summary>
        ///将数据导入到永隆行的商品表中
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public int AddProducts(YLHProductModel product)
        {
            #region sql

            string sql = @"
                    INSERT Tuhu_productcatalog..YLH_Product
                            ( oid ,
                              StoreName ,
                              [1stProductType] ,
                              [2ndProductType] ,
                              [3rdProductType] ,
                              [4thProductType] ,
                              [5thProductType] ,
                              counter_id ,
                              ProductNunber ,
                              ProductName ,
                              Specification ,
                              Price ,
                              SystemQuantity ,
                              SyetemSettlement ,
                              RealQuantity ,
                              RealSettlement ,
                              QuantityDiff ,
                              SettlementDiff ,
                              DiffReason ,
                              LastPurchaseDate ,
                              YearInWareHouse ,
                              DayInWareHouse ,
                              DistributionAmount ,
                              BuyoutAmount ,
                              MonthlySales ,
                              QualityClassification ,
                              Remark ,
                              CreatedTime ,
                              UpdatedTime ,
                              cy_list_price ,
                              MonthInWareHouse
                            )
                    VALUES  ( @oid , -- oid - int
                              @StoreName , -- StoreName - nvarchar(50)
                              @1stProductType , -- 1stProductType - nvarchar(50)
                              @2ndProductType, -- 2ndProductType - nvarchar(50)
                              @3rdProductType, -- 3rdProductType - nvarchar(50)
                              @4thProdictType, -- 4thProductType - nvarchar(50)
                              @5thProductType, -- 5thProductType - nvarchar(50)
                              @counter_id , -- counter_id - int
                              @ProductNunber, -- ProductNunber - nvarchar(256)
                              @ProductName, -- ProductName - nvarchar(128)
                              @Specification, -- Specification - nvarchar(128)
                              @Price , -- Price - money
                              @SystemQuantity , -- SystemQuantity - int
                              @SyetemSettlement , -- SyetemSettlement - money
                              @RealQuantity , -- RealQuantity - int
                              @RealSettlement , -- RealSettlement - money
                              @QuantityDiff , -- QuantityDiff - int
                              @SettlementDiff , -- SettlementDiff - money
                              @DiffReason , -- DiffReason - nvarchar(256)
                              @LastPurchaseDate , -- LastPurchaseDate - datetime
                              @YearInWareHouse , -- YearInWareHouse - int
                              @DayInWareHouse , -- DayInWareHouse - int
                              @DistributionAmount , -- DistributionAmount - money
                              @BuyoutAmount , -- BuyoutAmount - money
                              @MonthlySales , -- MonthlySales - int
                              @QualityClassificat , -- QualityClassification - nvarchar(50)
                              @Remark , -- Remark - nvarchar(50)
                              GETDATE() , -- CreatedTime - datetime
                              GETDATE() , -- UpdatedTime - datetime
                              @cy_list_price , -- cy_list_price - money
                              @MonthInWareHouse  -- MonthInWareHouse - int
                            )";

            #endregion

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);

                #region sqlParameters

                cmd.Parameters.AddWithValue("@oid", product.oid);
                cmd.Parameters.AddWithValue("@StoreName", product.StoreName);
                cmd.Parameters.AddWithValue("@1stProductType", product.ProductType1St);
                cmd.Parameters.AddWithValue("@2ndProductType", product.ProductType2Nd);
                cmd.Parameters.AddWithValue("@3rdProductType", product.ProductType3Rd);
                cmd.Parameters.AddWithValue("@4thProdictType", product.ProductType4Th);
                cmd.Parameters.AddWithValue("@5thProductType", product.ProductType5Th);
                cmd.Parameters.AddWithValue("@counter_id", product.counter_id);


                cmd.Parameters.AddWithValue("@ProductNunber", product.ProductNunber);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@Specification", product.Specification);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@SystemQuantity", product.SystemQuantity);
                cmd.Parameters.AddWithValue("@SyetemSettlement", product.SyetemSettlement);
                cmd.Parameters.AddWithValue("@RealQuantity", product.RealQuantity);

                cmd.Parameters.AddWithValue("@RealSettlement", product.RealSettlement);
                cmd.Parameters.AddWithValue("@QuantityDiff", product.QuantityDiff);
                cmd.Parameters.AddWithValue("@SettlementDiff", product.SettlementDiff);
                cmd.Parameters.AddWithValue("@DiffReason", product.DiffReason);
                cmd.Parameters.AddWithValue("@LastPurchaseDate", product.LastPurchaseDate);
                cmd.Parameters.AddWithValue("@YearInWareHouse", product.YearInWareHouse);
                cmd.Parameters.AddWithValue("@DayInWareHouse", product.DayInWareHouse);

                cmd.Parameters.AddWithValue("@DistributionAmount", product.DistributionAmount);
                cmd.Parameters.AddWithValue("@BuyoutAmount", product.BuyoutAmount);
                cmd.Parameters.AddWithValue("@MonthlySales", product.MonthlySales);
                cmd.Parameters.AddWithValue("@QualityClassificat", product.QualityClassification);
                cmd.Parameters.AddWithValue("@Remark", product.Remark);
                cmd.Parameters.AddWithValue("@cy_list_price", product.cy_list_price);
                cmd.Parameters.AddWithValue("@MonthInWareHouse", product.MonthInWareHouse);

                #endregion

                var result = db.ExecuteNonQuery(cmd);
                return result;
            }
        }

        /// <summary>
        ///查询catalogProduct中产品的Oid，做为永隆行的外键
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variantId"></param>
        /// <returns></returns>
        public int GetOid(string productId, string variantId)
        {
            using (
                var cmd =
                    new SqlCommand(
                        @"SELECT oid FROM Tuhu_productcatalog..CarPAR_CatalogProducts WHERE ProductID = @ProductID AND VariantID = @VariantID ")
                )
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("ProductID", productId);
                cmd.Parameters.AddWithValue("VariantID", variantId);
                var result = DbHelper.ExecuteScalar(cmd);
                return Convert.ToInt32(result ?? 0);
            }
        }

        ///  <summary>
        /// 查询catalogProduct中产品的Oid，做为永隆行的外键
        ///  </summary>
        ///  <param name="user"></param>
        /// <param name="operateMethod"></param>
        /// <param name="operateDetail"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int InsertLog(string user, string operateMethod, string operateDetail, string pid)
        {
            using (
                var cmd =
                    new SqlCommand(
                        @"INSERT INTO SystemLog..tbl_ProductManageLog
                                ( [User] ,
                                  OperateMethod ,
                                  OperateDateTime ,
                                  OperateDetail ,
                                  PID
                                )
                        VALUES  ( @User ,
                                  @OperateMethod ,
                                  GETDATE() ,
                                  @OperateDetail ,
                                  @PID 
                                )")
                )
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@User", user);
                cmd.Parameters.AddWithValue("@OperateMethod", operateMethod);
                cmd.Parameters.AddWithValue("@OperateDetail", operateDetail);
                cmd.Parameters.AddWithValue("@PID", pid);
                return DbHelper.ExecuteNonQuery(cmd);

            }
        }

        public int CheckoutProduct(string productNunber, int counterId)
        {
            using (
                var cmd =
                    new SqlCommand(
                        @"SELECT COUNT(0) FROM Tuhu_productcatalog..YLH_Product WITH(NOLOCK) WHERE ProductNunber= @ProductNunber AND counter_id= @counter_id ")
                )
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ProductNunber", productNunber);
                cmd.Parameters.AddWithValue("@counter_id", counterId);
                var result = DbHelper.ExecuteScalar(cmd);
                return Convert.ToInt32(result ?? 0);
            }
        }
    }
}
