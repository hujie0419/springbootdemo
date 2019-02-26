using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 会员权益奖励
    /// </summary>
    public class DALUserPromotionCode
    {
        #region 会员权益奖励
        /// <summary>
        /// 获取权益奖励列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<UserPromotionCodeModel> QueryPageList(UserPromotionCodeModel search, int pageIndex, int pageSize)
        {
            var strSql = new StringBuilder();
            var strCondition = new StringBuilder();
            var sqlParamerts = new List<SqlParameter>();
            #region 查询条件
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    strCondition.AppendFormat(" And Name like @Name");
                    sqlParamerts.Add(new SqlParameter("@Name", "%" + search.Name + "%"));
                }
                if (!string.IsNullOrWhiteSpace(search.CouponName))
                {
                    strCondition.AppendFormat(" And CouponName like @CouponName");
                    sqlParamerts.Add(new SqlParameter("@CouponName", "%" + search.CouponName + "%"));
                }
                if (!string.IsNullOrWhiteSpace(search.CouponDescription))
                {
                    strCondition.AppendFormat(" And CouponDescription like @CouponDescription");
                    sqlParamerts.Add(new SqlParameter("@CouponDescription", "%" + search.CouponDescription + "%"));
                }
                if (search.MembershipsGradeId > 0)
                {
                    strCondition.AppendFormat(" And MembershipsGradeId = @MembershipsGradeId");
                    sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", search.MembershipsGradeId));
                }
            }
            #endregion

            #region sql语句
            strSql.AppendFormat(@"Select [ID] ,
       [SImage] ,
       [BImage] ,
       [RuleID] ,
       [UserRank] ,
       [CouponName] ,
       [CouponDescription] ,
       [MembershipsGradeId] ,
       [Name] ,
       [UserPermissionId] ,
       [PermissionType] ,
       [RewardType] ,
       [RewardId] ,
       [RewardValue] ,
       [SortIndex] ,
       [CreateDatetime] ,
       [CreateBy] ,
       [LastUpdateDateTime] ,
       [LastUpdateBy] ,
       [IsDeleted]
FROM   [Gungnir].[dbo].[tbl_UserPromotionCode] WITH ( NOLOCK ) WHERE IsDeleted=0 {0} ORDER BY ID DESC  OFFSET(@PageIndex-1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY", strCondition.ToString());
            #endregion
            
            sqlParamerts.Add(new SqlParameter("@PageIndex", pageIndex));
            sqlParamerts.Add(new SqlParameter("@PageSize", pageSize));

            using (var dataCmd = new SqlCommand(strSql.ToString()))
            {
                dataCmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteDataTable(dataCmd)?.ConvertTo<UserPromotionCodeModel>().ToList();
            }
        }

        /// <summary>
        /// 获取权益奖励数量
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public static int QueryPageCount(UserPromotionCodeModel search)
        {
            var strCount = new StringBuilder();
            var strCondition = new StringBuilder();
            var sqlParamerts = new List<SqlParameter>();
            #region  获取数量
            strCondition.Append(@"
   SELECT ISNULL(COUNT(*), 0) AS totalCount
FROM   [Gungnir].[dbo].[tbl_UserPromotionCode] WITH ( NOLOCK )
WHERE  IsDeleted = 0
");
            #endregion
            #region 查询条件
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    strCondition.AppendFormat(" And Name like @Name");
                    sqlParamerts.Add(new SqlParameter("@Name", "%" + search.Name + "%"));
                }
                if (!string.IsNullOrWhiteSpace(search.CouponName))
                {
                    strCondition.AppendFormat(" And CouponName like @CouponName");
                    sqlParamerts.Add(new SqlParameter("@CouponName", "%" + search.CouponName + "%"));
                }
                if (!string.IsNullOrWhiteSpace(search.CouponDescription))
                {
                    strCondition.AppendFormat(" And CouponDescription like @CouponDescription");
                    sqlParamerts.Add(new SqlParameter("@CouponDescription", "%" + search.CouponDescription + "%"));
                }
                if (search.MembershipsGradeId > 0)
                {
                    strCondition.AppendFormat(" And MembershipsGradeId = @MembershipsGradeId");
                    sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", search.MembershipsGradeId));
                }
            }
            #endregion
            using (var countCmd = new SqlCommand(strCondition.ToString()))
            {
                countCmd.Parameters.AddRange(sqlParamerts.ToArray());
                var count = DbHelper.ExecuteScalar(countCmd);
                return Convert.ToInt32(count);
            }
        }

        /// <summary>
        /// 添加权益奖励
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add(UserPromotionCodeModel model)
        {
            var sqlParamerts = new List<SqlParameter>();
            #region sql脚本
            var strSql = @" INSERT INTO [Gungnir].[dbo].[tbl_UserPromotionCode] (   [SImage] ,
                                                        [BImage] ,
                                                        [RuleID] ,
                                                        [UserRank] ,
                                                        [CouponName] ,
                                                        [CouponDescription] ,
                                                        [MembershipsGradeId] ,
                                                        [Name] ,
                                                        [UserPermissionId] ,
                                                        [PermissionType] ,
                                                        [RewardType] ,
                                                        [RewardId] ,
                                                        [RewardValue] ,
                                                        [SortIndex] ,
                                                        [CreateDatetime] ,
                                                        [CreateBy] ,
                                                        [LastUpdateDateTime] ,
                                                        [LastUpdateBy] ,
                                                        [IsDeleted]
                                                    )
VALUES ( @SImage ,
         @BImage ,
         @RuleID ,
         @UserRank ,
         @CouponName ,
         @CouponDescription ,
         @MembershipsGradeId ,
         @Name ,
         @UserPermissionId ,
         @PermissionType ,
         @RewardType ,
         @RewardId ,
         @RewardValue ,
         @SortIndex ,
         GETDATE(),
         @LastUpdateBy ,
         GETDATE(),
         @LastUpdateBy ,
         0
       )";
            #endregion
            #region 参数化赋值
            sqlParamerts.Add(new SqlParameter("@SImage", model.SImage));
            sqlParamerts.Add(new SqlParameter("@BImage", model.BImage));
            sqlParamerts.Add(new SqlParameter("@RuleID", model.RuleID));
            sqlParamerts.Add(new SqlParameter("@UserRank", model.UserRank));
            sqlParamerts.Add(new SqlParameter("@CouponName", model.CouponName));
            sqlParamerts.Add(new SqlParameter("@CouponDescription", model.CouponDescription));
            sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", model.MembershipsGradeId));
            sqlParamerts.Add(new SqlParameter("@Name", model.Name));
            sqlParamerts.Add(new SqlParameter("@UserPermissionId", model.UserPermissionId));
            sqlParamerts.Add(new SqlParameter("@PermissionType", model.PermissionType));
            sqlParamerts.Add(new SqlParameter("@RewardType", model.RewardType));
            sqlParamerts.Add(new SqlParameter("@RewardId", model.RewardId));
            sqlParamerts.Add(new SqlParameter("@RewardValue", model.RewardValue));
            sqlParamerts.Add(new SqlParameter("@SortIndex", model.SortIndex));
            sqlParamerts.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
            #endregion
            using (var cmd = new SqlCommand(strSql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 更新权益奖励
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(UserPromotionCodeModel model)
        {
            var sqlParamerts = new List<SqlParameter>();
            #region sql脚本
            var strSql = @"
UPDATE [Gungnir].[dbo].[tbl_UserPromotionCode] WITH(ROWLOCK)
SET    [SImage] = @SImage ,
       [BImage] = @BImage ,
       [RuleID] = @RuleID ,
       [UserRank] = @UserRank ,
       [CouponName] = @CouponName ,
       [CouponDescription] = @CouponDescription ,
       [MembershipsGradeId] = @MembershipsGradeId ,
       [Name] = @Name ,
       [UserPermissionId] = @UserPermissionId ,
       [PermissionType] = @PermissionType ,
       [RewardType] = @RewardType ,
       [RewardId] = @RewardId ,
       [RewardValue] = @RewardValue ,
       [SortIndex] = @SortIndex ,
       [LastUpdateDateTime] = @LastUpdateDateTime ,
       [LastUpdateBy] = @LastUpdateBy
WHERE  ID = @ID 
";
            #endregion
            #region 参数化赋值
            sqlParamerts.Add(new SqlParameter("@SImage", model.SImage));
            sqlParamerts.Add(new SqlParameter("@BImage", model.BImage));
            sqlParamerts.Add(new SqlParameter("@RuleID", model.RuleID));
            sqlParamerts.Add(new SqlParameter("@UserRank", model.UserRank));
            sqlParamerts.Add(new SqlParameter("@CouponName", model.CouponName));
            sqlParamerts.Add(new SqlParameter("@CouponDescription", model.CouponDescription));
            sqlParamerts.Add(new SqlParameter("@MembershipsGradeId", model.MembershipsGradeId));
            sqlParamerts.Add(new SqlParameter("@Name", model.Name));
            sqlParamerts.Add(new SqlParameter("@UserPermissionId", model.UserPermissionId));
            sqlParamerts.Add(new SqlParameter("@PermissionType", model.PermissionType));
            sqlParamerts.Add(new SqlParameter("@RewardType", model.RewardType));
            sqlParamerts.Add(new SqlParameter("@RewardId", model.RewardId));
            sqlParamerts.Add(new SqlParameter("@RewardValue", model.RewardValue));
            sqlParamerts.Add(new SqlParameter("@SortIndex", model.SortIndex));
            sqlParamerts.Add(new SqlParameter("@LastUpdateDateTime", model.LastUpdateDateTime));
            sqlParamerts.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
            sqlParamerts.Add(new SqlParameter("@ID", model.Id));
            #endregion
            using (var cmd = new SqlCommand(strSql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 删除权益奖励
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id)
        {
            string sql = "update [Gungnir].[dbo].[tbl_UserPromotionCode] WITH ( ROWLOCK ) SET IsDeleted=1 where ID=@ID";
            using (var cmd = new SqlCommand(sql.ToString()))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 获取权益奖励明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserPromotionCodeModel GetModelById(int id)
        {
            #region 获取数据脚本
            var strSql = @" Select 
 TOP 1 [ID] ,
       [SImage] ,
       [BImage] ,
       [RuleID] ,
       [UserRank] ,
       [CouponName] ,
       [CouponDescription] ,
       [MembershipsGradeId] ,
       [Name] ,
       [UserPermissionId] ,
       [PermissionType] ,
       [RewardType] ,
       [RewardId] ,
       [RewardValue] ,
       [SortIndex] ,
       [CreateDatetime] ,
       [CreateBy] ,
       [LastUpdateDateTime] ,
       [LastUpdateBy] ,
       [IsDeleted]
FROM   [Gungnir].[dbo].[tbl_UserPromotionCode] WITH ( NOLOCK )
WHERE  ID = @ID";
            #endregion
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                var retsult = DbHelper.ExecuteDataTable(cmd)?.ConvertTo<UserPromotionCodeModel>().ToList();
                if (retsult != null && retsult.Count > 0)
                {
                    return retsult[0];
                }
                return null;
            }
        }

        #endregion
    }
}
