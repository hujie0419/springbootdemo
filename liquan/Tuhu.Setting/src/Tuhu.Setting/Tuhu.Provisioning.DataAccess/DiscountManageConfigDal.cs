using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess
{
   public class DiscountManageConfigDal
    {
        public static bool Update(SqlConnection connection, SE_GiftManageConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" UPDATE  Configuration.dbo.SE_GiftManageConfig
                                SET	Name = @Name,
									State = @State,
									Limit = @Limit,
									DonateWay = @DonateWay,
									Describe = @Describe,
									Visible = @Visible,
									OrdersWay = @OrdersWay,
									ValidTimeBegin = @ValidTimeBegin,
									ValidTimeEnd = @ValidTimeEnd,
									Channel = @Channel,
									Type = @Type,
									ConditionSize = @ConditionSize,
                                    TireType = @TireType,
									Size = @Size,
									B_Categorys = @B_Categorys,
									B_Brands = @B_Brands,
									B_PID = @B_PID,
									B_PID_Type = @B_PID_Type,
									P_PID = @P_PID,
									GiftCondition = @GiftCondition,
									GiftNum = @GiftNum,
									GiftMoney = @GiftMoney,
									GiftUnit = @GiftUnit,
									GiftType = @GiftType,
									GiftProducts = @GiftProducts,
									GiftDescribe = @GiftDescribe,
									Creater = @Creater,
									Mender = @Mender,
									CreateTime = @CreateTime,
									UpdateTime = @UpdateTime,
                                    TireSizeCondition= @TireSizeCondition,
                                    TireSize= @TireSize,
                                    Category=@Category,
                                    IsPackage=@IsPackage,
                                    ActivityType=@ActivityType,
                                    TagDisplay=@TagDisplay
								WHERE Id = @Id ";
                return conn.Execute(sql, model) > 0;
            }
        }
        public static int Insert(SqlConnection connection, SE_GiftManageConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" 
                                INSERT INTO Configuration.dbo.SE_GiftManageConfig
								(
									Name,
									State,
									Limit,
									DonateWay,
									Describe,
									Visible,
									OrdersWay,
									ValidTimeBegin,
									ValidTimeEnd,
									Channel,
									Type,
									ConditionSize,
                                    TireType,
									Size,
									B_Categorys,
									B_Brands,
									B_PID,
									B_PID_Type,
									P_PID,
									GiftCondition,
									GiftNum,
									GiftMoney,
									GiftUnit,
									GiftType,
									GiftProducts,
									GiftDescribe,
									Creater,
									Mender,
									CreateTime,
									UpdateTime,
                                    TireSizeCondition,
                                    TireSize,
                                    CateGory,
                                    IsPackage,
                                    ActivityType,
                                    TagDisplay
								)
                                VALUES
                                (
									@Name,
									@State,
									@Limit,
									@DonateWay,
									@Describe,
									@Visible,
									@OrdersWay,
									@ValidTimeBegin,
									@ValidTimeEnd,
									@Channel,
									@Type,
									@ConditionSize,
                                    @TireType,
									@Size,
									@B_Categorys,
									@B_Brands,
									@B_PID,
									@B_PID_Type,
									@P_PID,
									@GiftCondition,
									@GiftNum,
									@GiftMoney,
									@GiftUnit,
									@GiftType,
									@GiftProducts,
									@GiftDescribe,
									@Creater,
									@Mender,
									@CreateTime,
									@UpdateTime,
                                    @TireSizeCondition,
                                    @TireSize,
                                    @Category,
                                    @IsPackage,
                                    @ActivityType,
                                    @TagDisplay
								)SELECT @@IDENTITY";
                return conn.ExecuteScalar<int>(sql, model);
            }
        }
    }
}
