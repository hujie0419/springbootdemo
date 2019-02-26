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
    public class SE_MDBeautyCategoryConfigDAL
    {
        public static bool Insert(SqlConnection connection, SE_MDBeautyCategoryConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"
                INSERT INTO [SE_MDBeautyCategoryConfig]
                       ([ParentId]
                       ,[CategoryName]
                       ,[ShowName]
                       ,[Describe]
                       ,[IsDisable]
                       ,[CreateTime]
                       ,[Sort])
                 VALUES
                       (@ParentId
                       ,@CategoryName
                       ,@ShowName
                       ,@DESCRIBE
                       ,@IsDisable
                       ,@CreateTime
                       ,@Sort)";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Update(SqlConnection connection, SE_MDBeautyCategoryConfigModel model)
        {
            //using (IDbConnection conn = connection)
            //{
            //    string sql = @" UPDATE [SE_MDBeautyCategoryConfig]
            //                    SET [ParentId] = @ParentId
            //                       ,[CategoryName] = @CategoryName
            //                       ,[ShowName] = @ShowName
            //                       ,[Describe] = @Describe
            //                       ,[IsDisable] = @IsDisable
            //                       ,[CreateTime] = @CreateTime
            //                    WHERE Id = @Id ";
            //    return conn.Execute(sql, model) > 0;
            //}

            //修改状态时，若有子级则修改子级状态
            using (IDbConnection conn = connection)
            {
                string sql = @" 
                                BEGIN
	                                DECLARE @childCount INT
	                                with cte_child as
	                                (
		                                select *,0 as lvl from SE_MDBeautyCategoryConfig WITH(NOLOCK)
		                                where Id = @Id
		                                union all
		                                select b.*,lvl + 1 from cte_child AS c 
		                                INNER join SE_MDBeautyCategoryConfig AS b WITH(NOLOCK)
		                                on c.Id = b.ParentId
	                                )
	                                SELECT * INTO #ChildTable from cte_child;
	                                SELECT @childCount = COUNT(1) from #ChildTable
	                                UPDATE [SE_MDBeautyCategoryConfig]
                                                                 SET [ParentId]     = @ParentId
                                                                    ,[CategoryName] = @CategoryName
                                                                    ,[ShowName]     = @ShowName
                                                                    ,[Describe]     = @Describe
                                                                    ,[IsDisable]    = @IsDisable
                                                                    ,[CreateTime]   = @CreateTime
                                                                    ,[Sort]         = @Sort
                                                                 WHERE Id = @Id
	                                IF(@childCount > 1)
		                                BEGIN
			                                UPDATE [SE_MDBeautyCategoryConfig] 
			                                SET [IsDisable] = @IsDisable
			                                WHERE Id IN(select id from #ChildTable)
		                                END
	                                DROP TABLE #ChildTable
                                END ";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static SE_MDBeautyCategoryConfigModel Select(SqlConnection connection, int id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM SE_MDBeautyCategoryConfig WITH(NOLOCK) WHERE id = @id ";
                return conn.Query<SE_MDBeautyCategoryConfigModel>(sql, new { id = id })?.FirstOrDefault();
            }
        }

        public static IEnumerable<SE_MDBeautyCategoryConfigModel> SelectList(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                //SELECT * FROM SE_MDBeautyCategoryConfig WITH(NOLOCK) 
                string sql = @" SELECT *
                                ,'Childs'= (SELECT Childs FROM SE_MDBeautyCategoryConfigForChilds(Id)) 
                                ,'Parents' = (SELECT Parents FROM SE_MDBeautyCategoryConfigForParents(Id)) 
                                FROM SE_MDBeautyCategoryConfig WITH(NOLOCK) ";
                return conn.Query<SE_MDBeautyCategoryConfigModel>(sql);
            }
        }

        public static IEnumerable<SE_MDBeautyCategoryConfigForPartModel> SelectListForPart(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                //SELECT * FROM SE_MDBeautyCategoryConfig WITH(NOLOCK) 
                string sql = @" with a as
 (SELECT   Id=Convert(nvarchar,Id),ParentId,CategoryName,ShowName,Describe,IsDisable,CreateTime,Sort
                                ,'Childs'= (SELECT Childs FROM Tuhu_groupon..SE_MDBeautyCategoryConfigForChilds(Id)) 
                                ,'Parents' = (SELECT Parents FROM Tuhu_groupon..SE_MDBeautyCategoryConfigForParents(Id)) 
                                FROM Tuhu_groupon..SE_MDBeautyCategoryConfig WITH(NOLOCK) where isdisable=0)
select *from a WITH(NOLOCK)
union all
select  ProdcutId Id,CategoryIds,ProdcutName,ProdcutName,Describe,0,CreateTime,0,Convert(nvarchar,PId),Convert(nvarchar,CategoryIds)  from [Tuhu_groupon].[dbo].[SE_MDBeautyCategoryProductConfig] with(nolock)
where  IsDisable=0 and exists(select top 1 1 from a WITH(NOLOCK) where id=CategoryIds)";
                return conn.Query<SE_MDBeautyCategoryConfigForPartModel>(sql);
            }
        }

        public static IEnumerable<string> GetPidsFromMDBeautyCategoryProductConfigByCategoryIds(SqlConnection connection, IEnumerable<string> categoryIds)
        {
            using (IDbConnection conn = connection)
            {
                //SELECT * FROM SE_MDBeautyCategoryConfig WITH(NOLOCK) 
                string sql = @"SELECT Distinct  ProdcutId From [Tuhu_groupon].[dbo].[SE_MDBeautyCategoryProductConfig] with(nolock) where CategoryIds in (SELECT *FROM [Tuhu_groupon].[dbo].SplitString(@categotyIds,',',1)) and IsDisable=0";

                return conn.Query<string>(sql, new { categotyIds = string.Join(",", categoryIds) });
            }
        }

        /// <summary>
        /// 获取指定节点下所有子节点
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<SE_MDBeautyCategoryConfigModel> SelectChilds(SqlConnection connection, int id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" with cte_child as
                                (
                                    select *,0 as Level from SE_MDBeautyCategoryConfig WITH(NOLOCK)
                                    where Id = @Id
                                    union all
                                    select b.*,Level + 1 from cte_child AS c 
	                                INNER join SE_MDBeautyCategoryConfig AS b WITH(NOLOCK)
                                    on c.Id = b.ParentId
                                )
                                select * from cte_child ";
                return conn.Query<SE_MDBeautyCategoryConfigModel>(sql, new { Id = id });
            }
        }
    }
}
