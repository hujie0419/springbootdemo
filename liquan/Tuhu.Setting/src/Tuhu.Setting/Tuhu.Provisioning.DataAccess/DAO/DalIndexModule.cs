using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalIndexModule
    {

        public static bool UpdateModuleIndex(SqlConnection conn, int moduleId, int displayIndex)
        {
            var sql = @"UPDATE  [Configuration].[dbo].[IndexModule]
                        SET     DisplayIndex = @DisplayIndex
                        WHERE   PKID = @ModuleId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@DisplayIndex", displayIndex),
                new SqlParameter("@ModuleId", moduleId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateModuleName(SqlConnection conn, int moduleId, string moduleName)
        {
            var sql = @"UPDATE  [Configuration].[dbo].[IndexModule]
                        SET     ModuleName = @ModuleName,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @ModuleId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ModuleId",moduleId),
                new SqlParameter("@ModuleName", moduleName)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool DeleteModule(SqlConnection conn, int moduleId)
        {
            var sql = @"DELETE  FROM [Configuration].[dbo].[IndexModule]
                        WHERE   PKID = @ModuleId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ModuleId", moduleId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool CreateModule(SqlConnection conn, string moduleName, int DisplayIndex)
        {
            var sql = @"INSERT  INTO [Configuration].[dbo].[IndexModule]
                            ( ModuleName ,
                              DisplayIndex 
                            )
                        VALUES  ( @ModuleName ,
                                  @DisplayIndex 
                                );";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ModuleName", moduleName),
                new SqlParameter("@DisplayIndex", DisplayIndex)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateModuleItemIndex(SqlConnection conn, int moduleItemId, int displayIndex)
        {
            var sql = @"UPDATE  [Configuration].[dbo].[IndexModuleItem]
                        SET     DisplayIndex = @DisplayIndex
                        WHERE   PKID = @ModuleItemId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@DisplayIndex", displayIndex),
                new SqlParameter("@ModuleItemId", moduleItemId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateModuleEntry(SqlConnection conn, int moduleItemId, string entryName, string controller, string action)
        {
            var sql = @"UPDATE  [Configuration].[dbo].[IndexModuleItem]
                        SET     EntryName = @EntryName ,
                                Controller = @Controller ,
                                Action = @Action,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @ModuleItemId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@EntryName", entryName),
                new SqlParameter("@Controller", controller),
                new SqlParameter("@Action", action),
                new SqlParameter("@ModuleItemId", moduleItemId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool DeleteModuleItem(SqlConnection conn, int moduleItemId)
        {
            var sql = @"DELETE  FROM [Configuration].[dbo].[IndexModuleItem]
                        WHERE   PKID = @ModuleItemId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ModuleItemId", moduleItemId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool CreateModuleItem(SqlConnection conn, int moduleId, string entryName, string controller, string action, int displayIndex)
        {
            var sql = @"INSERT  INTO [Configuration].[dbo].[IndexModuleItem]
                                ( ModuleId ,
                                  EntryName ,
                                  Controller ,
                                  Action ,
                                  DisplayIndex 
                                )
                        VALUES  ( @ModuleId ,
                                  @EntryName ,
                                  @Controller ,
                                  @Action ,
                                  @DisplayIndex 
                                );";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ModuleId", moduleId),
                new SqlParameter("@EntryName", entryName),
                new SqlParameter("@Controller", controller),
                new SqlParameter("@Action", action),
                new SqlParameter("@DisplayIndex", displayIndex)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static List<IndexModuleConfig> SelectAllIndexModuleConfigs(SqlConnection conn)
        {
            var sql = @"SELECT  PKID ,
                                ModuleName ,
                                DisplayIndex
                        FROM    [Configuration].[dbo].[IndexModule] WITH ( NOLOCK )
                        ORDER BY DisplayIndex;";

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            var result = dt.ConvertTo<IndexModuleConfig>().ToList();
            return result;
        }

        public static List<IndexModuleItem> SelectAllIndexModuleItems(SqlConnection conn, int moduleId)
        {
            var sql = @"SELECT  PKID ,
                                ModuleId ,
                                EntryName ,
                                Controller ,
                                Action ,
                                DisplayIndex
                        FROM    [Configuration].[dbo].[IndexModuleItem] WITH ( NOLOCK )
                        WHERE   ModuleId = @ModuleId
                        ORDER BY DisplayIndex";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ModuleId", moduleId)
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            return dt.ConvertTo<IndexModuleItem>().ToList();
        }

        public static bool DeleteModuleItemsByModuleId(SqlConnection conn, int moduleId)
        {
            var sql = @"DELETE  FROM [Configuration].[dbo].[IndexModuleItem]
                        WHERE   ModuleId = @ModuleId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ModuleId", moduleId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool IsExistModule(SqlConnection conn, string moduleName)
        {
            var sql = @"SELECT  moduleName
                        FROM    [Configuration].[dbo].[IndexModule] WITH ( NOLOCK )
                        WHERE   ModuleName = @ModuleName;";

            var paraments = new SqlParameter[]
            {
                new SqlParameter("@ModuleName", moduleName)
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paraments);
            return dt.ConvertTo<IndexModuleConfig>().ToList().Count > 0;
        }
        public static bool IsExistModuleItem(SqlConnection conn, int moduleId, string entryName, string controller, string action)
        {
            var sql = @"SELECT  moduleId
                        FROM    [Configuration].[dbo].[IndexModuleItem] WITH ( NOLOCK )
                        WHERE   ModuleId = @ModuleId
                            AND EntryName = @EntryName
                            AND Controller = @Controller
                            AND Action = @Action;";

            var paraments = new SqlParameter[]
            {
                new SqlParameter("@ModuleId", moduleId),
                new SqlParameter("@EntryName", entryName),
                new SqlParameter("@Controller", controller),
                new SqlParameter("@Action", action)
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paraments);
            return dt.ConvertTo<IndexModuleItem>().ToList().Count > 0;
        }
        public static int GetMaxModuleIndex(SqlConnection conn)
        {
            var sql = @"SELECT  MAX(DisplayIndex)
                        FROM    [Configuration].[dbo].[IndexModule] WITH ( NOLOCK );";
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (result == DBNull.Value)
                return 0;
            return (int) result;
        }

        public static int GetMaxModuleItemIndex(SqlConnection conn, int moduleId)
        {
            var sql = @"SELECT  MAX(DisplayIndex)
                        FROM    [Configuration].[dbo].[IndexModuleItem] WITH ( NOLOCK )
                        WHERE   ModuleId = @ModuleId;";
            var paraments = new SqlParameter[]
            {
                new SqlParameter("@ModuleId", moduleId)
            };
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, paraments);
            if (result == DBNull.Value)
                return 0;
            return (int)(result);
        }
    }
}
