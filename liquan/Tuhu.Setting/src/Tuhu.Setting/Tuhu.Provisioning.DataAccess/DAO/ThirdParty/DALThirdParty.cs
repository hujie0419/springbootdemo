using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.ThirdParty;

namespace Tuhu.Provisioning.DataAccess.DAO.ThirdParty
{
    public class DALThirdParty
    {
        /// <summary>
        /// 获取三方码配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<ServiceCodeSourceConfig> GetServiceCodeSourceConfig(SqlConnection conn, int pageIndex, int pageSize)
        {
            const string sql = @"
            SELECT  sc.PKID ,
                    sc.SourceName ,
                    sc.Source ,
                    sc.SourceRegex ,
                    sc.Remarks ,
                    sc.CreatedTime ,
                    sc.UpdatedTime ,
                    COUNT(1) OVER() AS Total
            FROM    Tuhu_thirdparty..ServiceCodeSourceConfig AS sc WITH(NOLOCK)
            ORDER BY sc.PKID DESC
                    OFFSET(@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                 ONLY; ";
            return conn.Query<ServiceCodeSourceConfig>(sql, new
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 根据Source获取配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<ServiceCodeSourceConfig> GetServiceCodeSourceConfigBySource(SqlConnection conn, string source)
        {
            const string sql = @"
            SELECT  sc.PKID ,
                    sc.SourceName ,
                    sc.Source ,
                    sc.SourceRegex ,
                    sc.Remarks ,
                    sc.CreatedTime ,
                    sc.UpdatedTime
            FROM    Tuhu_thirdparty..ServiceCodeSourceConfig AS sc
                    WITH ( NOLOCK )
            WHERE   sc.Source = @Source ";
            return conn.Query<ServiceCodeSourceConfig>(sql, new
            {
                Source = source
            }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool InsertServiceCodeSourceConfig(SqlConnection conn, ServiceCodeSourceConfig config)
        {
            const string sql = @"
			INSERT INTO Tuhu_thirdparty..ServiceCodeSourceConfig
					( SourceName ,
					    Source ,
					    SourceRegex ,
					    Remarks ,
					    CreatedTime ,
					    UpdatedTime
					)
			VALUES  ( @SourceName ,
					    @Source ,
					    @SourceRegex ,
					    @Remarks ,
					    GETDATE() , 
					    GETDATE() 
					)";
            return conn.Execute(sql, new
            {
                config.Source,
                config.SourceName,
                config.SourceRegex,
                config.Remarks
            }, commandType: CommandType.Text) > 0;
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateServiceCodeSourceConfig(SqlConnection conn, ServiceCodeSourceConfig config)
        {
            const string sql = @"
            UPDATE  Tuhu_thirdparty..ServiceCodeSourceConfig
            SET     SourceName = @SourceName ,
                    SourceRegex = @SourceRegex ,
                    Remarks = @Remarks ,
                    UpdatedTime = GETDATE()
            WHERE   PKID = @PKID";
            return conn.Execute(sql, new
            {
                config.PKID,
                config.SourceName,
                config.SourceRegex,
                config.Remarks
            }, commandType: CommandType.Text) > 0;
        }
    }
}
