using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{

    public interface ICouponUseRuleRepository
    {
        ValueTask<int> CreateAsync(CouponUseRuleEntity entity, CancellationToken cancellationToken);
        ValueTask<bool> UpdateAsync(CouponUseRuleEntity entity, CancellationToken cancellationToken);
        ValueTask<CouponUseRuleEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken);
        CouponUseRuleEntity GetByPKID(int PKID );
    }

    /// <summary>
    /// 仓储 - 使用规则
    /// </summary>
    public class CouponUseRuleRepository : ICouponUseRuleRepository
    {

        private string DBName = "Activity..tbl_CouponRules";
        private readonly IDbHelperFactory _factory;

        public CouponUseRuleRepository(IDbHelperFactory factory) => _factory = factory;


        #region 增删改查
        /// <summary>
        /// Create entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationTok"></param>
        /// <returns></returns>
        public async ValueTask<int> CreateAsync(CouponUseRuleEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   (
                                       [Name]
                                      ,[Category]
                                      ,[ProductID]
                                      ,[Brand]
                                      ,[ParentID]
                                      ,[IsActive]
                                      ,[CreateDateTime]
                                      ,[LastDateTime]
                                      ,[Type]
                                      ,[InstallType]
                                      ,[PIDType]
                                      ,[IOSKey]
                                      ,[IOSValue]
                                      ,[androidKey]
                                      ,[androidValue]
                                      --,[TempCate]
                                      ,[HrefType]
                                      --,[tbl_CouponRules]
                                      ,[PromotionType]
                                      ,[ShopID]
                                      ,[ShopType]
                                      ,[PKID]
                                      ,[CustomSkipPage]
                                      ,[CouponType]
                                      ,[WxSkipPage]
                                      ,[ConfigType]
                                      ,[H5SkipPage]
                                      ,[OrderPayMethod]
                                      ,[EnabledGroupBuy]
                                      ,[RuleDescription]
                                    )
                             VALUES
                                   (
                                       @Name
                                      ,@Category
                                      ,@ProductID
                                      ,@Brand
                                      ,@ParentID
                                      ,@IsActive
                                      ,@CreateDateTime
                                      ,@LastDateTime
                                      ,@Type
                                      ,@InstallType
                                      ,@PIDType
                                      ,@IOSKey
                                      ,@IOSValue
                                      ,@androidKey
                                      ,@androidValue
                                      --,@TempCate
                                      ,@HrefType
                                      --,@tbl_CouponRules
                                      ,@PromotionType
                                      ,@ShopID
                                      ,@ShopType
                                      ,@PKID
                                      ,@CustomSkipPage
                                      ,@CouponType
                                      ,@WxSkipPage
                                      ,@ConfigType
                                      ,@H5SkipPage
                                      ,@OrderPayMethod
                                      ,@EnabledGroupBuy
                                      ,@RuleDescriptiond
                                      ,@RemindEmails
                                );
                                select  @@IDENTITY ;";  //返回pkid
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Name", entity.Name));
                    cmd.Parameters.Add(new SqlParameter("@Category", entity.Category));
                    cmd.Parameters.Add(new SqlParameter("@ProductID", entity.ProductID));
                    cmd.Parameters.Add(new SqlParameter("@Brand", entity.Brand));
                    cmd.Parameters.Add(new SqlParameter("@ParentID", entity.ParentID));
                    cmd.Parameters.Add(new SqlParameter("@IsActive", entity.IsActive));
                    cmd.Parameters.Add(new SqlParameter("@CreateDateTime", entity.CreateDateTime));
                    cmd.Parameters.Add(new SqlParameter("@LastDateTime", entity.LastDateTime));
                    cmd.Parameters.Add(new SqlParameter("@Type", entity.Type));
                    cmd.Parameters.Add(new SqlParameter("@InstallType", entity.InstallType));
                    cmd.Parameters.Add(new SqlParameter("@PIDType", entity.PIDType));
                    cmd.Parameters.Add(new SqlParameter("@IOSKey", entity.IOSKey));
                    cmd.Parameters.Add(new SqlParameter("@IOSValue", entity.IOSValue));
                    cmd.Parameters.Add(new SqlParameter("@androidKey", entity.androidKey));
                    cmd.Parameters.Add(new SqlParameter("@androidValue", entity.androidValue));
                    //cmd.Parameters.Add(new SqlParameter("@TempCate", entity.TempCate));
                    cmd.Parameters.Add(new SqlParameter("@HrefType", entity.HrefType));
                    //cmd.Parameters.Add(new SqlParameter("@tbl_CouponRules", entity.tbl_CouponRules));
                    cmd.Parameters.Add(new SqlParameter("@PromotionType", entity.PromotionType));
                    cmd.Parameters.Add(new SqlParameter("@ShopID", entity.ShopID));
                    cmd.Parameters.Add(new SqlParameter("@ShopType", entity.ShopType));
                    cmd.Parameters.Add(new SqlParameter("@CustomSkipPage", entity.CustomSkipPage));
                    cmd.Parameters.Add(new SqlParameter("@CouponType", entity.CouponType));
                    cmd.Parameters.Add(new SqlParameter("@WxSkipPage", entity.WxSkipPage));
                    cmd.Parameters.Add(new SqlParameter("@ConfigType", entity.ConfigType));
                    cmd.Parameters.Add(new SqlParameter("@H5SkipPage", entity.H5SkipPage));
                    cmd.Parameters.Add(new SqlParameter("@OrderPayMethod", entity.OrderPayMethod));
                    cmd.Parameters.Add(new SqlParameter("@EnabledGroupBuy", entity.EnabledGroupBuy));
                    cmd.Parameters.Add(new SqlParameter("@RuleDescription", entity.RuleDescription));
                    var result = (await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return Convert.ToInt32(result);
                }
            }
        }



        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> UpdateAsync(CouponUseRuleEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            StringBuilder strSql = new StringBuilder();
            strSql.Append($@"update {DBName} set ");

            strSql.Append(" Name = @Name , ");
            strSql.Append(" InstallType = @InstallType , ");
            strSql.Append(" PIDType = @PIDType , ");
            strSql.Append(" IOSKey = @IOSKey , ");
            strSql.Append(" IOSValue = @IOSValue , ");
            strSql.Append(" androidKey = @androidKey , ");
            strSql.Append(" androidValue = @androidValue , ");
            //strSql.Append(" TempCate = @TempCate , ");
            strSql.Append(" HrefType = @HrefType , ");
            strSql.Append(" tbl_CouponRules = @tbl_CouponRules , ");
            strSql.Append(" PromotionType = @PromotionType , ");
            strSql.Append(" Category = @Category , ");
            strSql.Append(" ShopID = @ShopID , ");
            strSql.Append(" ShopType = @ShopType , ");
            strSql.Append(" CustomSkipPage = @CustomSkipPage , ");
            strSql.Append(" CouponType = @CouponType , ");
            strSql.Append(" WxSkipPage = @WxSkipPage , ");
            strSql.Append(" ConfigType = @ConfigType , ");
            strSql.Append(" H5SkipPage = @H5SkipPage , ");
            strSql.Append(" OrderPayMethod = @OrderPayMethod , ");
            strSql.Append(" EnabledGroupBuy = @EnabledGroupBuy , ");
            strSql.Append(" ProductID = @ProductID , ");
            strSql.Append(" RuleDescription = @RuleDescription , ");
            strSql.Append(" Brand = @Brand , ");
            strSql.Append(" ParentID = @ParentID , ");
            strSql.Append(" IsActive = @IsActive , ");
            strSql.Append(" CreateDateTime = @CreateDateTime , ");
            strSql.Append(" LastDateTime = @LastDateTime , ");
            strSql.Append(" Type = @Type  ");
            strSql.Append(" where PKID=@PKID ");
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(strSql.ToString()))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", entity.PKID));
                    cmd.Parameters.Add(new SqlParameter("@Name", entity.Name));
                    cmd.Parameters.Add(new SqlParameter("@Category", entity.Category));
                    cmd.Parameters.Add(new SqlParameter("@ProductID", entity.ProductID));
                    cmd.Parameters.Add(new SqlParameter("@Brand", entity.Brand));
                    cmd.Parameters.Add(new SqlParameter("@ParentID", entity.ParentID));
                    cmd.Parameters.Add(new SqlParameter("@IsActive", entity.IsActive));
                    cmd.Parameters.Add(new SqlParameter("@CreateDateTime", entity.CreateDateTime));
                    cmd.Parameters.Add(new SqlParameter("@LastDateTime", entity.LastDateTime));
                    cmd.Parameters.Add(new SqlParameter("@Type", entity.Type));
                    cmd.Parameters.Add(new SqlParameter("@InstallType", entity.InstallType));
                    cmd.Parameters.Add(new SqlParameter("@PIDType", entity.PIDType));
                    cmd.Parameters.Add(new SqlParameter("@IOSKey", entity.IOSKey));
                    cmd.Parameters.Add(new SqlParameter("@IOSValue", entity.IOSValue));
                    cmd.Parameters.Add(new SqlParameter("@androidKey", entity.androidKey));
                    cmd.Parameters.Add(new SqlParameter("@androidValue", entity.androidValue));
                    //cmd.Parameters.Add(new SqlParameter("@TempCate", entity.TempCate));
                    cmd.Parameters.Add(new SqlParameter("@HrefType", entity.HrefType));
                    //cmd.Parameters.Add(new SqlParameter("@tbl_CouponRules", entity.tbl_CouponRules));
                    cmd.Parameters.Add(new SqlParameter("@PromotionType", entity.PromotionType));
                    cmd.Parameters.Add(new SqlParameter("@ShopID", entity.ShopID));
                    cmd.Parameters.Add(new SqlParameter("@ShopType", entity.ShopType));
                    cmd.Parameters.Add(new SqlParameter("@CustomSkipPage", entity.CustomSkipPage));
                    cmd.Parameters.Add(new SqlParameter("@CouponType", entity.CouponType));
                    cmd.Parameters.Add(new SqlParameter("@WxSkipPage", entity.WxSkipPage));
                    cmd.Parameters.Add(new SqlParameter("@ConfigType", entity.ConfigType));
                    cmd.Parameters.Add(new SqlParameter("@H5SkipPage", entity.H5SkipPage));
                    cmd.Parameters.Add(new SqlParameter("@OrderPayMethod", entity.OrderPayMethod));
                    cmd.Parameters.Add(new SqlParameter("@EnabledGroupBuy", entity.EnabledGroupBuy));
                    cmd.Parameters.Add(new SqlParameter("@RuleDescription", entity.RuleDescription));
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result > 0;
                }
            }
        }


        /// <summary>
        /// Get entity by  pkid
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<CouponUseRuleEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT
                            PKID
                            ,Name
                            ,Category
                            ,ProductID
                            ,Brand
                            ,ParentID
                            ,IsActive
                            ,CreateDateTime
                            ,LastDateTime
                            ,Type
                            ,InstallType
                            ,PIDType
                            ,IOSKey
                            ,IOSValue
                            ,androidKey
                            ,androidValue
                            --,TempCate
                            ,HrefType
                            --,tbl_CouponRules
                            ,PromotionType
                            ,ShopID
                            ,ShopType
                            ,CustomSkipPage
                            ,CouponType
                            ,WxSkipPage
                            ,ConfigType
                            ,H5SkipPage
                            ,OrderPayMethod
                            ,EnabledGroupBuy
                            ,RuleDescription
                        FROM  {DBName} with (nolock)
                        where PKID = @PKID
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", PKID));
                    var result = (await dbHelper.ExecuteFetchAsync<CouponUseRuleEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }

        public CouponUseRuleEntity GetByPKID(int PKID)
        {
            #region sql
            string sql = $@"SELECT
                            PKID
                            ,Name
                            ,Category
                            ,ProductID
                            ,Brand
                            ,ParentID
                            ,IsActive
                            ,CreateDateTime
                            ,LastDateTime
                            ,Type
                            ,InstallType
                            ,PIDType
                            ,IOSKey
                            ,IOSValue
                            ,androidKey
                            ,androidValue
                            --,TempCate
                            ,HrefType
                            --,tbl_CouponRules
                            ,PromotionType
                            ,ShopID
                            ,ShopType
                            ,CustomSkipPage
                            ,CouponType
                            ,WxSkipPage
                            ,ConfigType
                            ,H5SkipPage
                            ,OrderPayMethod
                            ,EnabledGroupBuy
                            ,RuleDescription
                        FROM  {DBName} with (nolock)
                        where PKID = @PKID
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", PKID));
                    var result = dbHelper.ExecuteFetch<CouponUseRuleEntity>(cmd);
                    return result;
                }
            }
        }

        #endregion
    }
}
