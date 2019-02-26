using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.SqlServer.Server;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Extensions;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 优惠券任务 产品类别关系表
    /// </summary>
  public  class DalPromotionTaskProductCategory
    {
        /// <summary>
        /// 添加优惠券产品类别
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Insert(PromotionTaskProductCategory model)
        {
            if (model == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(model.CreateBy))
            {
                model.CreateBy = "";
            }
            if (string.IsNullOrWhiteSpace(model.LastUpdateBy))
            {
                model.LastUpdateBy = "";
            }
            #region sql 脚本
            var strSql = @"
                        INSERT INTO [Activity].[dbo].[tbl_PromotionTaskProductCategory]
                        ([PromotionTaskId]
                        ,[ProductCategoryId]
                        ,[ParentCategoryId]
                        ,[NodeNo]
                        ,[IsCheckAll]
                        ,[CategoryName]
                        ,[CreateBy]
                        ,[LastUpdateBy]
                        )
                 VALUES
                    (
                     @PromotionTaskId
                    ,@ProductCategoryId
                    ,@ParentCategoryId
                    ,@NodeNo
                    ,@IsCheckAll
                    ,@CategoryName
                    ,@CreateBy
                    ,@LastUpdateBy
                    )";
            #endregion

            using (var cmd = new SqlCommand(strSql))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", model.PromotionTaskId);
                cmd.Parameters.AddWithValue("@ProductCategoryId", model.ProductCategoryId);
                cmd.Parameters.AddWithValue("@ParentCategoryId", model.ParentCategoryId);
                cmd.Parameters.AddWithValue("@NodeNo", model.NodeNo);
                cmd.Parameters.AddWithValue("@IsCheckAll", model.IsCheckAll);
                cmd.Parameters.AddWithValue("@CategoryName", model.CategoryName);
                cmd.Parameters.AddWithValue("@CreateBy", model.CreateBy);
                cmd.Parameters.AddWithValue("@LastUpdateBy", model.LastUpdateBy);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        /// 根据业务对象获取优惠券任务集合
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<PromotionTaskProductCategory> GetList(PromotionTaskProductCategory model)
        {
            var strbSql = new StringBuilder();
            #region   sql脚本
            strbSql.Append(@"
            SELECT [PKID] ,
                   [PromotionTaskId] ,
                   [ProductCategoryId] ,
                   [ParentCategoryId] ,
                   [NodeNo] ,
                   [IsCheckAll] ,
                   [CategoryName] ,
                   [CreateBy] ,
                   [CreateTime] ,
                   [LastUpdateBy] ,
                   [LastUpdateDateTime] ,
                   [IsDeleted]
            FROM   [Activity].[dbo].[tbl_PromotionTaskProductCategory]  WITH(Nolock)
            WHERE  IsDeleted = 0
");
            #endregion
            var parmentList = new List<SqlParameter>();
            if (model.PromotionTaskId > 0)
            {
                strbSql.Append(" AND PromotionTaskId =@PromotionTaskId");
                parmentList.Add(new SqlParameter("@PromotionTaskId", model.PromotionTaskId));
            }
            using (var cmd = new SqlCommand(strbSql.ToString()))
            {
                if (parmentList.Count > 0)
                {
                    cmd.Parameters.AddRange(parmentList.ToArray());
                }
                var dt = DbHelper.ExecuteDataTable(cmd);
                if (dt != null)
                {
                    return dt.ConvertTo<PromotionTaskProductCategory>().ToList();
                }
                return null;
            }
        }

        /// <summary>
        /// 更加优惠任务物理删除产品类别
        /// </summary>
        /// <param name="PromotionTaskId">优惠券任务表外键</param>
        /// <returns></returns>
        public static bool DeletePromotionTaskProductCategory(int PromotionTaskId)
        {
            var strSql = "DELETE  FROM [Activity].[dbo].[tbl_PromotionTaskProductCategory] WHERE PromotionTaskId=@PromotionTaskId";
            using (var cmd = new SqlCommand(strSql))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", PromotionTaskId);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
    }
}
