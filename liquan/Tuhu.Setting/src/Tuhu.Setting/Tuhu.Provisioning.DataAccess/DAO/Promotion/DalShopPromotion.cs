using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.DataAccess.DAO.Promotion
{
    public class DalShopPromotion
    {
        public static ListModel<ShopCouponRulesModel> GetCouponList(string keywords, int? discount, string startDate,
            string endDate, int status, int pageIndex, int pageSize)
        {
            string sql = "SELECT PKID AS RuleId,* FROM Activity..tbl_ShopCouponRules WITH(NOLOCK) ";
            string where = " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = $"%{keywords}%";
                where += " AND PromotionName LIKE @Keywords";
            }
            if (discount != null)
            {
                where += " AND Discount=@Discount ";
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Parse(startDate).ToString("yyyy-MM-dd");
                where += " AND CreateDateTime>=@StartDate";
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Parse(endDate).ToString("yyyy-MM-dd") + " 23:59:59";
                where += " AND CreateDateTime<=@EndDate";
            }
            if (status != -1)
            {
                where += " AND ISNULL(Status,0)=@Status";
            }
            var result = new ListModel<ShopCouponRulesModel>()
            {
                Pager = new Component.Common.Models.PagerModel()
                {
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                }
            };
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                using (var cmd =
                    db.CreateDbCommand(
                        $@"{sql} {where} ORDER BY CreateDateTime DESC OFFSET {
                                (pageIndex - 1) * pageSize
                            } ROWS FETCH NEXT {pageSize} ROWS ONLY;"))
                {
                    cmd.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@Keywords", keywords),
                        new SqlParameter("@Discount", discount),
                        new SqlParameter("@StartDate", startDate),
                        new SqlParameter("@EndDate", endDate),
                        new SqlParameter("@Status", status),
                    });
                    result.Source=db.ExecuteDataTable(cmd).ConvertTo<ShopCouponRulesModel>();
                    cmd.CommandText = $"SELECT COUNT(1) FROM Activity..tbl_ShopCouponRules WITH(NOLOCK) {where}";
                    result.Pager.TotalItem = (int) db.ExecuteScalar(cmd);
                }
            }
            return result;
        }

        public static IEnumerable<ShopCouponRuleProduct> GetCouponRuleProducts(IEnumerable<int> ruleIds)
        {
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                using (var cmd =
                    db.CreateDbCommand(
                        @"SELECT * FROM Activity..tbl_ShopCouponRules_ConfigProduct WITH(NOLOCK) WHERE RuleID IN(SELECT Item FROM Activity..SplitString(@RuleIds,',',1))")
                )
                {
                    cmd.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@RuleIds",string.Join(",",ruleIds))
                    });
                    return db.ExecuteDataTable(cmd).ConvertTo<ShopCouponRuleProduct>();
                }
            }
        }

        public static bool UpdateCouponRuleStatus(int ruleId, int status,string operater)
        {
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                using (var cmd =
                    db.CreateDbCommand(
                        @"UPDATE Activity..tbl_ShopCouponRules WITH(ROWLOCK) SET Status=@Status,LastUpdater=@Operater,LastUpdateDateTime=GETDATE() WHERE PKID=@RuleId")
                )
                {
                    cmd.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@RuleId",ruleId),
                        new SqlParameter("@Status",status),
                        new SqlParameter("@Operater",operater)
                    });
                    return db.ExecuteNonQuery(cmd) > 0;
                }
            }
        }

        public static int GetNotUserdPromotionCount(int ruleId) {
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                using (var cmd =
                    db.CreateDbCommand(
                        @"SELECT COUNT(1) FROM Activity..tbl_ShopPromotionCode WITH(NOLOCK) WHERE RuleId=@RuleId AND Status=0 AND DATEDIFF(dd, EndTime, GETDATE()) <= 7")
                )
                {
                    cmd.Parameters.AddRange(new[]
                    {
                        new SqlParameter("@RuleId",ruleId)
                    });
                    return (int) db.ExecuteScalar(cmd);
                }
            }
        }
        public static bool CopyCouponRule(int ruleId, string operater)
        {
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                try
                {
                    int result = 0;
                    db.BeginTransaction();
                    using (var cmd =
                        db.CreateDbCommand(
                            @"
INSERT INTO Activity..tbl_ShopCouponRules(PromotionType,PromotionName,Description,DisplayName,Discount,MinMoney,SupportUserRange,Status,Creater,LastUpdater,CreateDateTime,LastUpdateDateTime)
SELECT PromotionType,PromotionName,Description,DisplayName,Discount,MinMoney,SupportUserRange,0,@Operater,@Operater,GETDATE(),GETDATE() FROM Activity..tbl_ShopCouponRules WHERE PKID=@RuleId;
SELECT @@IDENTITY;")
                    )
                    {
                        cmd.Parameters.AddRange(new[]
                        {
                            new SqlParameter("@RuleId",ruleId),
                            new SqlParameter("@Operater",operater)
                        });
                        result =  (int)db.ExecuteScalar(cmd);
                        if (result > 0)
                        {
                            cmd.CommandText = $@"INSERT INTO Activity..tbl_ShopCouponRules_ConfigShop
                                                ( RuleId ,
                                                  ShopId ,
                                                  CreateDateTime ,
                                                  LastUpdateDateTime
                                                ) SELECT {result} ,
                                                  ShopId ,
                                                  GETDATE() ,
                                                  GETDATE() FROM Activity..tbl_ShopCouponRules_ConfigShop WITH(NOLOCK) WHERE RuleId=@RuleId";
                            db.ExecuteNonQuery(cmd);

                            cmd.CommandText = $@"INSERT INTO Activity..tbl_ShopCouponRules_ConfigProduct
                                                ( RuleID ,
                                                  Type ,
                                                  ConfigValue ,
                                                  CreateDateTime ,
                                                  LastUpdateDateTime
                                                )SELECT {result} ,
                                                  Type ,
                                                  ConfigValue ,
                                                  GETDATE() ,
                                                  GETDATE() FROM Activity..tbl_ShopCouponRules_ConfigProduct WITH(NOLOCK) WHERE RuleID=@RuleId";
                            db.ExecuteNonQuery(cmd);
                        }
                    }
                    db.Commit();
                    return result > 0;
                }
                catch (Exception e)
                {
                    db.Rollback();
                    return false;
                }
                
            }
        }
    }
}
