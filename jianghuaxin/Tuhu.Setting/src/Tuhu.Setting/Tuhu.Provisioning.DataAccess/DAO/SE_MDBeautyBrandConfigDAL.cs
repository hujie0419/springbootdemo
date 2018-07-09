using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class SE_MDBeautyBrandConfigDAL
    {
        public static bool Insert(SqlConnection connection, SE_MDBeautyBrandConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"
                INSERT INTO [SE_MDBeautyBrandConfig]
                           ([ParentId]
                           ,[CategoryIds]
                           ,[BrandName]
                           ,[IsDisable]
                           ,[CreateTime])
                     VALUES
                           (@ParentId
                           ,@CategoryIds
                           ,@BrandName
                           ,@IsDisable
                           ,@CreateTime)";
                return conn.Execute(sql,model) > 0;
            }
        }

        public static bool BatchInsert(SqlConnection connection, SE_MDBeautyBrandConfigModel model)
        {
            int executeNum = 0;
            var _brandNameList = model?.BrandName.Split(',');
            string sql = @"
                INSERT INTO [SE_MDBeautyBrandConfig]
                           ([ParentId]
                           ,[CategoryIds]
                           ,[BrandName]
                           ,[IsDisable]
                           ,[CreateTime])
                     VALUES
                           (@ParentId
                           ,@CategoryIds
                           ,@BrandName
                           ,@IsDisable
                           ,@CreateTime)";

            using (IDbConnection conn = connection)
            {
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    for (int i = 0; i < _brandNameList.Length; i++)
                    {
                        model.BrandName = _brandNameList[i] ?? "";
                        if (!string.IsNullOrWhiteSpace(model.BrandName))
                        {
                            executeNum += conn.Execute(sql, model, transaction);
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    if (executeNum != _brandNameList.Length)
                        transaction.Rollback();
                }
            }
            return executeNum == _brandNameList.Length;
        }

        public static bool Update(SqlConnection connection, SE_MDBeautyBrandConfigModel model)
        {
            //修改状态时，若有子级则修改子级状态
            using (IDbConnection conn = connection)
            {
                //string sql = @" UPDATE [SE_MDBeautyBrandConfig]
                //                   SET [ParentId] = @ParentId
                //                      ,[CategoryIds] = @CategoryIds
                //                      ,[BrandName] = @BrandName
                //                      ,[IsDisable] = @IsDisable
                //                      ,[CreateTime] = @CreateTime
                //                WHERE Id = @Id ";
                //return conn.Execute(sql, model) > 0;
                string sql = @" 
                                BEGIN
	                                DECLARE @childCount INT
	                                with cte_child as
	                                (
		                                select *,0 as lvl from SE_MDBeautyBrandConfig WITH(NOLOCK)
		                                where Id = @Id
		                                union all
		                                select b.*,lvl + 1 from cte_child AS c 
		                                INNER join SE_MDBeautyBrandConfig AS b WITH(NOLOCK)
		                                on c.Id = b.ParentId
	                                )
	                                SELECT * INTO #ChildTable from cte_child;
	                                SELECT @childCount = COUNT(1) from #ChildTable
	                                UPDATE [SE_MDBeautyBrandConfig]
                                                                   SET [ParentId] = @ParentId
                                                                      ,[CategoryIds] = @CategoryIds
                                                                      ,[BrandName] = @BrandName
                                                                      ,[IsDisable] = @IsDisable
                                                                      ,[CreateTime] = @CreateTime
                                                                WHERE Id = @Id
                                    IF(@childCount > 1)
		                                BEGIN
			                                UPDATE [SE_MDBeautyBrandConfig] 
			                                SET [IsDisable] = @IsDisable
			                                WHERE Id IN(select id from #ChildTable)
		                                END
	                                DROP TABLE #ChildTable
                                END ";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static SE_MDBeautyBrandConfigModel Select(SqlConnection connection, int id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM SE_MDBeautyBrandConfig WITH(NOLOCK) WHERE id = @id ";
                return conn.Query<SE_MDBeautyBrandConfigModel>(sql, new { id = id })?.FirstOrDefault();
            }
        }

        public static IEnumerable<SE_MDBeautyBrandConfigModel> SelectList(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * FROM SE_MDBeautyBrandConfig WITH(NOLOCK) ";
                return conn.Query<SE_MDBeautyBrandConfigModel>(sql);
            }
        }

        public static IEnumerable<SE_MDBeautyBrandConfigModel> SelectListByBrandName(SqlConnection connection, IEnumerable<string> brandName, int id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * FROM SE_MDBeautyBrandConfig WITH(NOLOCK) WHERE BrandName IN(" + string.Join(",", brandName) + ")";
                if (id > 0)
                    sql += string.Format(" AND Id <> {0} ", id);
                return conn.Query<SE_MDBeautyBrandConfigModel>(sql);
            }
        }

        public static IEnumerable<SE_MDBeautyBrandConfigModel> SelectListForCategoryId(SqlConnection connection, string categoryId)
        {
            using (IDbConnection conn = connection)
            {
                //string sql = @" SELECT * FROM SE_MDBeautyBrandConfig WITH(NOLOCK)
                //                WHERE CategoryIds LIKE '%'+ @CategoryIds + '%'
                //                UNION 
                //                SELECT * FROM SE_MDBeautyBrandConfig WHERE ParentId IN 
                //                (
                //                    SELECT Id FROM SE_MDBeautyBrandConfig WITH(NOLOCK)
                //                    WHERE CategoryIds LIKE '%'+ @CategoryIds +'%'
                //                )";

                string sql = @" SELECT ',' + CategoryIds + ',' as 'CategoryIdsFilter',* INTO #SE_MDBeautyBrandConfig_Filter FROM SE_MDBeautyBrandConfig WITH(NOLOCK) 
                                SELECT * FROM #SE_MDBeautyBrandConfig_Filter WITH(NOLOCK)
                                WHERE CategoryIdsFilter is not null and  CategoryIdsFilter LIKE '%,'+ @CategoryIds + ',%'
                                UNION 
                                SELECT * FROM #SE_MDBeautyBrandConfig_Filter WITH(NOLOCK) WHERE ParentId IN 
                                (
                                    SELECT Id FROM #SE_MDBeautyBrandConfig_Filter WITH(NOLOCK)
                                    WHERE CategoryIdsFilter LIKE '%,'+ @CategoryIds + ',%'
                                )
                                DROP TABLE #SE_MDBeautyBrandConfig_Filter ";

                return conn.Query<SE_MDBeautyBrandConfigModel>(sql, new { CategoryIds = categoryId });
            }
        }
    }
}
