using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    public class PromotionTaskBudgetRepository : IPromotionTaskBudgetRepository
    {
        private string DBName = "Gungnir..PromotionTaskBudget";
        private readonly IDbHelperFactory _factory;
        public PromotionTaskBudgetRepository(IDbHelperFactory factory) => _factory = factory;
        public async ValueTask<int> CreateAsync(PromotionTaskBudgetEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   (PromotionTaskID
                                   ,CouponRulesId
                                   ,BusinessLineName
                                   ,OrderID
                                   ,PIDs
                                   --,PurchaseNum
                                   --,SpendMoney
                                   ,DiscountMoney
                                   ,CouponNum
                                   ,CreateDateTime
                                   ,LastUpdateDateTime
                                    )
                             VALUES
                                   (@PromotionTaskID
                                   ,@CouponRulesId
                                   ,@BusinessLineName
                                   ,@OrderID
                                   ,@PIDs
                                   --,@PurchaseNum
                                   --,@SpendMoney
                                   ,@DiscountMoney
                                   ,@CouponNum
                                   ,getdate()
                                   ,getdate()
                                    );
                                select SCOPE_IDENTITY();";  //返回pkid
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskID", entity.PromotionTaskID));
                    cmd.Parameters.Add(new SqlParameter("@CouponRulesId", entity.CouponRulesId));
                    cmd.Parameters.Add(new SqlParameter("@BusinessLineName", entity.BusinessLineName??"" ));
                    cmd.Parameters.Add(new SqlParameter("@OrderID", entity.OrderID));
                    cmd.Parameters.Add(new SqlParameter("@PIDs", entity.PIDs ??""));
                    //cmd.Parameters.Add(new SqlParameter("@PurchaseNum", entity.PurchaseNum));
                    //cmd.Parameters.Add(new SqlParameter("@SpendMoney", entity.SpendMoney));
                    cmd.Parameters.Add(new SqlParameter("@DiscountMoney", entity.DiscountMoney));
                    cmd.Parameters.Add(new SqlParameter("@CouponNum", entity.CouponNum));

                    var temp = await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false);
                    return Convert.ToInt32(temp);

                }
            }
        }
    }
}
