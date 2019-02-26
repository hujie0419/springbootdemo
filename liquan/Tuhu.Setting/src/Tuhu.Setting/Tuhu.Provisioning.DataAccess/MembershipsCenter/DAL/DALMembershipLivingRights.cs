using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 会员生活权益
    /// </summary>
   public class DALMembershipLivingRights
    {
        #region 会员生活权益
        /// <summary>
        /// 获取生活权益列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<MemberShipLivingRightsModel> QueryPageList(MemberShipLivingRightsModel search, int pageIndex, int pageSize)
        {
            var strSql = new StringBuilder();
            var strCondition = new StringBuilder();
            var sqlParamerts = new List<SqlParameter>();
            #region 查询条件
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.ChannelName))
                {
                    strCondition.AppendFormat(" And ChannelName like @ChannelName");
                    sqlParamerts.Add(new SqlParameter("@ChannelName", "%" + search.ChannelName + "%"));
                }
                if (!string.IsNullOrWhiteSpace(search.WelfareContent))
                {
                    strCondition.AppendFormat(" And WelfareContent like @WelfareContent");
                    sqlParamerts.Add(new SqlParameter("@WelfareContent", "%" + search.WelfareContent + "%"));
                }
            }
            #endregion

            #region sql语句
            strSql.AppendFormat(@"SELECT [PKID]
      ,[ChannelId]
      ,[ChannelName]
      ,[WelfareContent]
      ,[RightsValue]
      ,[ReceivingType]
      ,[ReceivingDescription]
      ,[CouponId]
      ,[CouponDescription]
      ,[LinkUrl]
      ,[SortIndex]
      ,[CreateDatetime]
      ,[CreateBy]
      ,[LastUpdateDateTime]
      ,[LastUpdateBy]
      ,[IsDeleted]
  FROM [Tuhu_profiles].[dbo].[MembershipLivingRights] WITH ( NOLOCK ) WHERE IsDeleted=0 {0} ORDER BY PKID DESC  OFFSET(@PageIndex-1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY",strCondition.ToString());
            #endregion
            
            sqlParamerts.Add(new SqlParameter("@PageIndex", pageIndex));
            sqlParamerts.Add(new SqlParameter("@PageSize", pageSize));

            using (var dataCmd = new SqlCommand(strSql.ToString()))
            {
                dataCmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteDataTable(dataCmd)?.ConvertTo<MemberShipLivingRightsModel>().ToList();
            }
        }

        /// <summary>
        /// 获取生活权益数量
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public static int QueryPageCount(MemberShipLivingRightsModel search)
        {
            var strCount = new StringBuilder();
            var strCondition = new StringBuilder();
            var sqlParamerts = new List<SqlParameter>();
            #region  获取数量
            strCondition.Append(@"
   SELECT ISNULL(COUNT(*), 0) AS totalCount
FROM   [Tuhu_profiles].[dbo].[MembershipLivingRights] WITH ( NOLOCK )
WHERE  IsDeleted = 0
");
            #endregion
            #region 查询条件
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.ChannelName))
                {
                    strCondition.AppendFormat(" And ChannelName like @ChannelName");
                    sqlParamerts.Add(new SqlParameter("@ChannelName", "%" + search.ChannelName + "%"));
                }
                if (!string.IsNullOrWhiteSpace(search.WelfareContent))
                {
                    strCondition.AppendFormat(" And WelfareContent like @WelfareContent");
                    sqlParamerts.Add(new SqlParameter("@WelfareContent", "%" + search.WelfareContent + "%"));
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
        /// 添加生活权益
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add(MemberShipLivingRightsModel model)
        {
            var sqlParamerts = new List<SqlParameter>();
            #region sql脚本
            var strSql = @" INSERT INTO [Tuhu_profiles].[dbo].[MembershipLivingRights] (   [ChannelId] ,
                                                               [ChannelName] ,
                                                               [WelfareContent] ,
                                                               [RightsValue] ,
                                                               [ReceivingType] ,
                                                               [ReceivingDescription] ,
                                                               [CouponId] ,
                                                               [CouponDescription] ,
                                                               [LinkUrl] ,
                                                               [SortIndex] ,
                                                               [CreateDatetime] ,
                                                               [CreateBy] ,
                                                               [LastUpdateDateTime] ,
                                                               [LastUpdateBy] ,
                                                               [IsDeleted]
                                                           )
VALUES ( @ChannelId ,
         @ChannelName ,
         @WelfareContent ,
         @RightsValue ,
         @ReceivingType ,
         @ReceivingDescription ,
         @CouponId ,
         @CouponDescription ,
         @LinkUrl ,
         @SortIndex ,
         GetDate(),
        @LastUpdateBy,
        GetDate(),
         @LastUpdateBy,
         0
       )";
            #endregion
            #region 参数化赋值
            sqlParamerts.Add(new SqlParameter("@ChannelId", model.ChannelId));
            sqlParamerts.Add(new SqlParameter("@ChannelName", model.ChannelName));
            sqlParamerts.Add(new SqlParameter("@WelfareContent", model.WelfareContent));
            sqlParamerts.Add(new SqlParameter("@RightsValue", model.RightsValue));
            sqlParamerts.Add(new SqlParameter("@ReceivingType", model.ReceivingType));
            sqlParamerts.Add(new SqlParameter("@ReceivingDescription", model.ReceivingDescription));
            sqlParamerts.Add(new SqlParameter("@CouponId", model.CouponId));
            sqlParamerts.Add(new SqlParameter("@CouponDescription", model.CouponDescription));
            sqlParamerts.Add(new SqlParameter("@LinkUrl", model.LinkUrl));
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
        /// 更新生活权益
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(MemberShipLivingRightsModel model)
        {
            var sqlParamerts = new List<SqlParameter>();
            #region sql脚本
            var strSql = @"
UPDATE [Tuhu_profiles].[dbo].[MembershipLivingRights] WITH ( ROWLOCK )
SET    [ChannelId] = @ChannelId,
       [ChannelName] = @ChannelName ,
       [WelfareContent] = @WelfareContent ,
       [RightsValue] = @RightsValue ,
       [ReceivingType] = @ReceivingType ,
       [ReceivingDescription] =@ReceivingDescription ,
       [CouponId] = @CouponId ,
       [CouponDescription] =@CouponDescription ,
       [LinkUrl] = @LinkUrl ,
       [SortIndex] =@SortIndex ,
       [LastUpdateDateTime] = GETDATE() ,
       [LastUpdateBy] =@LastUpdateBy
WHERE  PKID = @PKID
";
            #endregion
            #region 参数化赋值
            sqlParamerts.Add(new SqlParameter("@ChannelId", model.ChannelId));
            sqlParamerts.Add(new SqlParameter("@ChannelName", model.ChannelName));
            sqlParamerts.Add(new SqlParameter("@WelfareContent", model.WelfareContent));
            sqlParamerts.Add(new SqlParameter("@RightsValue", model.RightsValue));
            sqlParamerts.Add(new SqlParameter("@ReceivingType", model.ReceivingType));
            sqlParamerts.Add(new SqlParameter("@ReceivingDescription", model.ReceivingDescription));
            sqlParamerts.Add(new SqlParameter("@CouponDescription", model.CouponDescription));
            sqlParamerts.Add(new SqlParameter("@LinkUrl", model.LinkUrl));
            sqlParamerts.Add(new SqlParameter("@CouponId", model.CouponId));
            sqlParamerts.Add(new SqlParameter("@SortIndex", model.SortIndex));
            sqlParamerts.Add(new SqlParameter("@LastUpdateBy", model.LastUpdateBy));
            sqlParamerts.Add(new SqlParameter("@PKID", model.PKID));
            #endregion
            using (var cmd = new SqlCommand(strSql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(sqlParamerts.ToArray());
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 删除生活权益
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id)
        {
            string sql = "update [Tuhu_profiles].[dbo].[MembershipLivingRights] WITH ( ROWLOCK ) SET IsDeleted=1 where PKID=@PKID";
            using (var cmd = new SqlCommand(sql.ToString()))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", id);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 获取生活权益明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MemberShipLivingRightsModel GetModelById(int id)
        {
            #region 获取数据脚本
            var strSql = @"
 SELECT TOP 1 [PKID]
      ,[ChannelId]
      ,[ChannelName]
      ,[WelfareContent]
      ,[RightsValue]
      ,[ReceivingType]
      ,[ReceivingDescription]
      ,[CouponId]
      ,[CouponDescription]
      ,[LinkUrl]
      ,[SortIndex]
      ,[CreateDatetime]
      ,[CreateBy]
      ,[LastUpdateDateTime]
      ,[LastUpdateBy]
      ,[IsDeleted]
FROM   [Tuhu_profiles].[dbo].[MembershipLivingRights] WITH ( NOLOCK )
WHERE  PKID = @PKID";
            #endregion
            using (var cmd = new SqlCommand(strSql.ToString()))
            {
                cmd.Parameters.AddWithValue("@PKID", id);
                var retsult = DbHelper.ExecuteDataTable(cmd)?.ConvertTo<MemberShipLivingRightsModel>().ToList();
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
