using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.TagConfig;

namespace Tuhu.Provisioning.DataAccess.DAO.TagConfig
{
    public class DalTagConfig
    {
        /// <summary>
        /// 获取标签配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleTabConfig> GetArticleTabConfig(SqlConnection conn, int pageIndex, int pageSize)
        {
            const string sql = @"
            SELECT  tc.PKID ,
                    tc.NormalImg ,
                    tc.SelectedImg ,
                    tc.Source ,
                    tc.CreateUser ,
                    tc.CreateTime ,
                    tc.UpdateTime ,
                    COUNT(1) OVER ( ) AS Total
            FROM    Configuration..ArticleTabConfig AS tc WITH ( NOLOCK )
            ORDER BY tc.PKID DESC
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                    ONLY";
            return conn.Query<ArticleTabConfig>(sql, new
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }
        /// <summary>
        /// 添加标签配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static int InsertArticleTabConfig(SqlConnection conn, ArticleTabConfig config)
        {
            const string sql = @"
            INSERT  INTO Configuration..ArticleTabConfig
                    ( NormalImg ,
                      SelectedImg ,
                      Source ,
                      CreateUser ,
                      CreateTime ,
                      UpdateTime
                    )
            VALUES  ( @NormalImg ,
                      @SelectedImg ,
                      @Source ,
                      @CreateUser ,
                      GETDATE() ,
                      GETDATE()
                    )";
            return Convert.ToInt32(conn.Execute(sql, new
            {
                NormalImg = config.NormalImg,
                SelectedImg = config.SelectedImg,
                CreateUser = config.CreateUser,
                Source = config.Source.ToString()
            }, commandType: CommandType.Text));
        }
    }
}
