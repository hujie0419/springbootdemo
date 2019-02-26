using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALCategory
    {
        public static bool InsertCategory(SqlConnection conn, CategoryDic model)
        {

            string sqlCategory = @"  INSERT INTO Gungnir.dbo.Category
                                                ( 
                                                  [ParentId] ,
                                                  [Name] ,
                                                  [Value] ,
                                                  [Type]
                                                )
                                      VALUES    ( 
                                                  @ParentId ,
                                                  @Name ,
                                                  @Value,
                                                  @Type
                                                );SELECT  @@Identity";

            string sqlNewApp = @" INSERT INTO Gungnir.dbo.tal_newappsetdata
                                             ( apptype ,
                                               modelfloor                                 
                                             )
                                     VALUES  ( @apptype ,
                                               @modelfloor                        
                                             )";


            var sqlParams= new SqlParameter[]
            {
                new SqlParameter("@ParentId",model.ParentId),
                  new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@Value",model.Value??string.Empty),
                      new SqlParameter("@Type",model.Type??string.Empty)
            
            };


            SqlTransaction tran;
            if (conn.State == ConnectionState.Closed) conn.Open();
            tran = conn.BeginTransaction();

            try
            {
                int apptype = Convert.ToInt32(SqlHelper.ExecuteScalar(tran, CommandType.Text, sqlCategory, sqlParams));

                var sqlParamsApp = new SqlParameter[]
                    {
                        new SqlParameter("@apptype",apptype),
                          new SqlParameter("@modelfloor",1)                  
                    };

                //添加大类后，默认添加1楼，两个模块！
                SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sqlNewApp, sqlParamsApp);
                SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sqlNewApp, sqlParamsApp);
                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return false;
                throw;
            }
            finally
            {
                tran.Dispose();            
            }
        }

        public static int GetMaxId(SqlConnection conn)
        {
            string sql = @"SELECT MAX(Id) FROM [Gungnir].[dbo].[Category]  WITH(NOLOCK)";

            return (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
        }

        public static List<CategoryDic> GetCategoryDic(SqlConnection conn)
        {
            string sql = @"SELECT [Id]
                                ,[ParentId]
                                ,[Name]
                                ,[Value]
                                ,[Type]
                            FROM [Gungnir].[dbo].[Category]  WITH(NOLOCK) WHERE  Type = 'app'";

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<CategoryDic>().ToList();
        }

        public static List<NewAppData> GetNewAppData(SqlConnection conn)
        {
            string sql = @"  SELECT    [apptype] ,
                                        modelfloor ,
			                            showorder,
			                            showstatic,
                                        modelname,
                                        B.Name AS appname
                              FROM      [Gungnir].[dbo].[tal_newappsetdata] AS A WITH(NOLOCK)
                                        INNER JOIN [Gungnir].[dbo].[Category] AS B WITH(NOLOCK) ON A.apptype = B.id
                              GROUP BY  [apptype] ,
                                        modelfloor ,
			                            showorder,
			                            showstatic,
                                        modelname,
                                        B.Name
 
                              ";

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<NewAppData>().ToList();
        }

        public static bool InsertNewAppData(SqlConnection conn, NewAppData model)
        {
            string sql = @" INSERT INTO Gungnir.dbo.tal_newappsetdata
                                     ( apptype ,
                                       modelfloor 
                                      -- showorder ,     
                                      -- showstatic 

                                     )
                             VALUES  ( @apptype ,
                                       @modelfloor 
                                      -- @showorder ,     
                                      -- @showstatic 
                                     )";

            var sqlprams = new SqlParameter[]
            {
                new SqlParameter("@apptype",model.apptype),
                  new SqlParameter("@modelfloor",model.modelfloor)        
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlprams) > 0;
        }

        public static int GetMaxModelFloor(SqlConnection conn, NewAppData model)
        {

            string sql = @" SELECT MAX(modelfloor) FROM [Gungnir].[dbo].[tal_newappsetdata]  WITH(NOLOCK) WHERE apptype=@apptype";

            var sqlprams = new SqlParameter[]
            {
                new SqlParameter("@apptype",model.apptype)                         
            
            };
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlprams);
            return Convert.ToInt32(obj);
        }

        public static List<CategoryDic> GetCategoryIdName(SqlConnection conn)
        {
            string sql = @" SELECT [Id] , [Name]  FROM   [Gungnir].[dbo].[Category]   WITH(NOLOCK)";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<CategoryDic>().ToList();
        }
    }
}
