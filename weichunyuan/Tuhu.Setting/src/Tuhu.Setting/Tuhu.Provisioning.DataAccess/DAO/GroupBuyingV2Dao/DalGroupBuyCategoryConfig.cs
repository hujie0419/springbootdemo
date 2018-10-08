using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO.GroupBuyingV2Dao
{
    public static class DalGroupBuyCategoryConfig
    {
        public static IEnumerable<OperationCategoryModel> Select(SqlConnection connection, int? id, int? parentId, int? type)
        {
            if (id == null && parentId == null && type == null)
            {
                var sql = @"SELECT [Id],
       [ParentId],
       [CategoryCode],
       [DisplayName],
       [Icon],
       [Description],
       [AppKeyForIOS],
       [AppValueForIOS],
       [AppKeyForAndroid],
       [AppValueForAndroid],
       [H5Url],
       [Sort],
       [IsShow],
       [Type],
       [CreateTime],
       [WebsiteImage],
       [IsNeedAdpter],
       [IsNeedVehicle]
FROM [Configuration].[dbo].[tbl_OperationCategory] WITH (NOLOCK)
ORDER BY Sort";
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, null).ConvertTo<OperationCategoryModel>().ToList();
            }
            else if (id == null && type == null && parentId != null)
            {
                var sql = "SELECT * FROM [Configuration].[dbo].[tbl_OperationCategory] WITH(NOLOCK) WHERE ParentId = @parentId ORDER BY sort";
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@parentId", Value = parentId }).ConvertTo<OperationCategoryModel>().ToList();
            }
            else if (id == null && parentId == null && type != null)
            {
                var sql = "SELECT * FROM [Configuration].[dbo].[tbl_OperationCategory] WITH(NOLOCK) WHERE Type = @type ORDER BY sort";
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@type", Value = type }).ConvertTo<OperationCategoryModel>().ToList();
            }
            else
            {
                var sql = "SELECT * FROM [Configuration].[dbo].[tbl_OperationCategory] WITH(NOLOCK) WHERE Id = @id ORDER BY sort";
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@id", Value = id }).ConvertTo<OperationCategoryModel>().ToList();
            }
        }

        public static bool Insert(SqlConnection connection, OperationCategoryModel model)
        {
            var sql = @"
                INSERT INTO [Configuration].[dbo].[tbl_OperationCategory]
                           ([ParentId]
                           ,[CategoryCode]
                           ,[DisplayName]
                           ,[Icon]
                           ,[Description]
                           ,[AppKeyForIOS]
                           ,[AppValueForIOS]
                           ,[AppKeyForAndroid]
                           ,[AppValueForAndroid]
                           ,[H5Url]
                           ,[Sort]
                           ,[IsShow]
                           ,[Type]
                           ,[CreateTime]
                           ,WebsiteImage
                           ,IsNeedVehicle
                           ,IsNeedAdpter)
                     VALUES
                           (@ParentId
                           ,@CategoryCode
                           ,@DisplayName
                           ,@Icon
                           ,@Description
                           ,@AppKeyForIOS
                           ,@AppValueForIOS
                           ,@AppKeyForAndroid
                           ,@AppValueForAndroid
                           ,@H5Url
                           ,@Sort
                           ,@IsShow
                           ,@Type
                           ,@CreateTime
                           ,@WebsiteImage
                           ,@IsNeedVehicle ,0)";

            var sqlparams = new[]{
                    new SqlParameter("@ParentId",model.ParentId),
                    new SqlParameter("@CategoryCode",model.CategoryCode),
                    new SqlParameter("@DisplayName",model.DisplayName),
                    new SqlParameter("@Icon",model.Icon),
                    new SqlParameter("@Description",model.Description),
                    new SqlParameter("@AppKeyForIOS",model.AppKeyForIOS),
                    new SqlParameter("@AppValueForIOS",model.AppValueForIOS),
                    new SqlParameter("@AppKeyForAndroid",model.AppKeyForAndroid),
                    new SqlParameter("@AppValueForAndroid",model.AppValueForAndroid),
                    new SqlParameter("@H5Url",model.H5Url),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@IsShow",model.IsShow),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@CreateTime",model.CreateTime),
                    new SqlParameter("@WebsiteImage",model.WebsiteImage),
                    new SqlParameter("@IsNeedVehicle",model.IsNeedVehicle)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlparams) > 0;
        }

        public static bool Update(SqlConnection connection, OperationCategoryModel model)
        {
            var sql = @"
                    UPDATE [Configuration].[dbo].[tbl_OperationCategory]
                       SET [ParentId] = @ParentId
                          ,[CategoryCode] = @CategoryCode
                          ,[DisplayName] = @DisplayName
                          ,[Icon] = @Icon
                          ,[Description] = @Description
                          ,[AppKeyForIOS] = @AppKeyForIOS
                          ,[AppValueForIOS] = @AppValueForIOS
                          ,[AppKeyForAndroid] = @AppKeyForAndroid
                          ,[AppValueForAndroid] = @AppValueForAndroid
                          ,[H5Url] = @H5Url
                          ,[Sort] = @Sort
                          ,[IsShow] = @IsShow
                          ,[Type] = @Type
                          ,[CreateTime] = @CreateTime
                          ,WebsiteImage = @WebsiteImage
                          ,IsNeedVehicle = @IsNeedVehicle
                     WHERE Id = @Id";

            var sqlparams = new[]{
                    new SqlParameter("@ParentId",model.ParentId),
                    new SqlParameter("@CategoryCode",model.CategoryCode),
                    new SqlParameter("@DisplayName",model.DisplayName),
                    new SqlParameter("@Icon",model.Icon),
                    new SqlParameter("@Description",model.Description),
                    new SqlParameter("@AppKeyForIOS",model.AppKeyForIOS),
                    new SqlParameter("@AppValueForIOS",model.AppValueForIOS),
                    new SqlParameter("@AppKeyForAndroid",model.AppKeyForAndroid),
                    new SqlParameter("@AppValueForAndroid",model.AppValueForAndroid),
                    new SqlParameter("@H5Url",model.H5Url),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@IsShow",model.IsShow),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@CreateTime",model.CreateTime),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@WebsiteImage",model.WebsiteImage),
                    new SqlParameter("@IsNeedVehicle",model.IsNeedVehicle)
            };

            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlparams) > 0;
        }

        public static bool Delete(SqlConnection connection, int id)
        {
            var sql = @"
                DECLARE @TableCount int
                with childTable as(
                select * from [Configuration].[dbo].[tbl_OperationCategory] WITH(NOLOCK) where ParentId = @Id
                UNION ALL
                select op.* from [Configuration].[dbo].[tbl_OperationCategory] op WITH(NOLOCK) JOIN childTable on op.ParentId=childTable.id
                )SELECT * INTO #CategoryTemp FROM childTable
                SELECT @TableCount = COUNT(1) FROM #CategoryTemp
                IF(@TableCount > 0)
	                BEGIN
		                SELECT 0 AS 'result'
	                END
                ELSE
	                BEGIN
		                DELETE [Configuration].[dbo].[tbl_OperationCategory] WHERE id = @Id
		                DELETE [Configuration].[dbo].[tbl_OperationCategory_Products] WHERE OId = @Id
		                SELECT 1 AS 'result'
	                END
                DROP TABLE #CategoryTemp
            ";
            var sqlparams = new[]{
                    new SqlParameter("@Id",id)
            };
            return (int)SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, sqlparams) > 0;
        }

        /// <summary>
        /// 获取关联产品
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static IEnumerable<OperationCategoryProductsModel> SelectOperationCategoryProducts(SqlConnection connection, int oid)
        {
            var sql = " SELECT * FROM [Configuration].[dbo].[tbl_OperationCategory_Products] WITH(NOLOCK) WHERE OId = @OId ORDER BY Sort";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@OId", Value = oid }).ConvertTo<OperationCategoryProductsModel>().ToList();
        }

        /// <summary>
        /// 更新关联产品关系
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oid"></param>
        /// <param name="type"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static bool UpdateOperationCategoryProducts(SqlConnection connection, int oid, int type, List<OperationCategoryProductsModel> lists)
        {
            var sbSql = new StringBuilder();
            var listParas = new List<SqlParameter>();

            var templateForDel = "DELETE FROM [Configuration].[dbo].[tbl_OperationCategory_Products] WHERE OId = @OId";
            var templateForIns = @"INSERT INTO [Configuration].[dbo].[tbl_OperationCategory_Products]
                                     ([OId],[CorrelId],[CategoryCode],[DefinitionCode],[Sort],[IsShow],[Type],[CreateTime])
                                      VALUES({0},{1},{2},{3},{4},{5},{6},{7})";
            sbSql.AppendLine(templateForDel);
            listParas.Add(new SqlParameter("@OId", oid));

            if (lists != null)
            {
                for (var i = 0; i < lists.Count; i++)
                {
                    var item = lists[i];
                    sbSql.AppendFormat(
                        templateForIns,
                        "@OId" + i,
                        "@CorrelId" + i,
                        "@CategoryCode" + i,
                        "@DefinitionCode" + i,
                        "@Sort" + i,
                        "@IsShow" + i,
                        "@Type" + i,
                        "@CreateTime" + i);
                    listParas.Add(new SqlParameter("@OId" + i, item.OId));
                    listParas.Add(new SqlParameter("@CorrelId" + i, item.CorrelId));
                    listParas.Add(new SqlParameter("@CategoryCode" + i, item.CategoryCode));
                    listParas.Add(new SqlParameter("@DefinitionCode" + i, item.DefinitionCode));
                    listParas.Add(new SqlParameter("@Sort" + i, item.Sort));
                    listParas.Add(new SqlParameter("@IsShow" + i, item.IsShow));
                    listParas.Add(new SqlParameter("@Type" + i, item.Type));
                    listParas.Add(new SqlParameter("@CreateTime" + i, item.CreateTime));
                    sbSql.AppendLine("");
                }
            }
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sbSql.ToString(), listParas.ToArray()) > 0;
        }

        /// <summary>
        /// 获取后台类目树集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ZTreeModel> SelectProductCategories(SqlConnection connection)
        {
            var sql = " SELECT oid AS 'id',ParentOid AS 'pId',DisplayName AS 'name',0 AS 'open',0 AS 'chkDisabled',CategoryName,NodeNo FROM  [Tuhu_productcatalog].dbo.vw_ProductCategories WITH(NOLOCK) ";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, null).ConvertTo<ZTreeModel>().ToList();
        }

        /// <summary>
        /// 通过oid查询后台类目
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oids"></param>
        /// <returns></returns>
        public static IEnumerable<VW_ProductCategoriesModel> SelectProductCategoriesForOid(SqlConnection connection, IEnumerable<int> oids)
        {
            var sql =
                $"SELECT CategoryName FROM [Tuhu_productcatalog]..vw_ProductCategories WITH(NOLOCK) WHERE oid IN({string.Join(",", oids)})";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, null).ConvertTo<VW_ProductCategoriesModel>().ToList();
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<VW_ProductsModel> SelectProductCategories(SqlConnection connection, IEnumerable<string> tags, bool isOid = false)
        {
            string sql;
            if (isOid)
            {
                sql =
                    $@"SELECT oid,PID,DisplayName,OnSale,stockout,cy_list_price,cy_marketing_price,CP_Brand,CP_Place,CP_Tab,TireSize,Image
                                         FROM[Tuhu_productcatalog].[dbo].[vw_Products] WITH(NOLOCK)
                                         WHERE oid IN({string.Join(",", tags)})";
            }
            else
            {
                sql =
                    $@"SELECT oid,PID,DisplayName,OnSale,stockout,cy_list_price,cy_marketing_price,CP_Brand,CP_Place,CP_Tab,TireSize,Image
                                         FROM[Tuhu_productcatalog].[dbo].[vw_Products] WITH(NOLOCK)
                                         WHERE DefinitionName IN
                                         (
                                             SELECT CategoryName FROM[Tuhu_productcatalog]..vw_ProductCategories WITH(NOLOCK) WHERE oid IN({
                        string.Join(",", tags)
                    })
                                         ) OR Category IN
                                         (
                                             SELECT CategoryName FROM[Tuhu_productcatalog]..vw_ProductCategories WITH(NOLOCK) WHERE oid IN({
                        string.Join(",", tags)
                    })
                                         )";
            }
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, null).ConvertTo<VW_ProductsModel>().ToList();
        }

        public static IEnumerable<VW_ProductsModel> FetchPrProductsModelByProductIds(IEnumerable<string> pids)
        {
            var sql = $@"SELECT oid,PID,DisplayName,OnSale,stockout,cy_list_price,cy_marketing_price,CP_Brand,CP_Place,CP_Tab,TireSize,Image
                                            FROM [Tuhu_productcatalog].[dbo].[vw_Products] WITH(NOLOCK)
                                            WHERE PID IN ({string.Join(",", pids)})";
            var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            var dt = SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, sql, null);
            return dt.ConvertTo<VW_ProductsModel>().ToList();
        }
    }
}