using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalLiveWorkShopConfig
    {
        /// <summary>
        /// 获取所有透明工场配置类型
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetAllLiveWorkShopConfigType(SqlConnection conn)
        {
            #region SQL
            var result = new List<string>();
            var sql = @"SELECT  DISTINCT
                                t.TypeName
                        FROM    Configuration..LiveWorkShopConfig AS t WITH ( NOLOCK )
                        ORDER BY t.TypeName;";
            #endregion
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            if (dt != null && dt.Rows != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var item = row.IsNull("TypeName") ? string.Empty : row["TypeName"].ToString();
                    if (!string.IsNullOrEmpty(item))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 透明工场配置展示
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="typeName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<LiveWorkShopConfigModel> SelectLiveWorkShopConfig(SqlConnection conn,
            string typeName, int pageIndex, int pageSize, out int totalCount)
        {
            #region SQL
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..LiveWorkShopConfig AS b WITH ( NOLOCK )
                        WHERE  @TypeName IS NULL
                                OR @TypeName = N''
                                OR b.TypeName = @TypeName;
                        SELECT  b.PKID ,
                                b.TypeName ,
                                b.Picture ,
                                b.Content ,
                                b.H5Url ,
                                b.PcUrl ,
                                b.Gif ,
                                b.SortNumber ,
                                b.ShopId ,
                                b.ChannelName ,
                                b.CreateDateTime ,
                                b.LastUpdateDateTime
                        FROM    Configuration..LiveWorkShopConfig AS b WITH ( NOLOCK )
                        WHERE   @TypeName IS NULL
                                OR @TypeName = N''
                                OR b.TypeName = @TypeName
                        ORDER BY b.TypeName ,
                                b.SortNumber
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            totalCount = 0;
            var parameters = new[]
            {
                new SqlParameter("@TypeName", typeName),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<LiveWorkShopConfigModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 现有表的所有配置
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<LiveWorkShopConfigModel> GetExistLiveWorkShopConfig(SqlConnection conn)
        {
            #region SQL
            var sql = @"SELECT  b.PKID ,
                                b.TypeName ,
                                b.SortNumber 
                        FROM    Configuration..LiveWorkShopConfig AS b WITH ( NOLOCK )
                        ORDER BY b.TypeName ,
                                b.SortNumber;";
            #endregion
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<LiveWorkShopConfigModel>().ToList();
            return result;
        }

        /// <summary>
        /// 删除现有数据库中存在而导入的Excel中不存在的记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteLiveWorkShopConfig(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"DELETE  FROM Configuration..LiveWorkShopConfig
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            var result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return result > 0;
        }

        /// <summary>
        /// 导入透明工场配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool ImportLiveWorkShopConfig(SqlConnection conn, LiveWorkShopConfigModel model)
        {
            #region SQL
            var sql = @"IF EXISTS ( SELECT  1
                                    FROM    Configuration..LiveWorkShopConfig WITH ( NOLOCK )
                                    WHERE   TypeName = @TypeName
                                            AND SortNumber = @SortNumber )
                           BEGIN
                            UPDATE  Configuration..LiveWorkShopConfig
                            SET     Picture = @Picture ,
                                    Content = @Content ,
                                    H5Url = @H5Url ,
                                    PcUrl = @PcUrl ,
                                    Gif = @Gif ,
                                    ShopId = @ShopId ,
                                    ChannelName = @ChannelName,
                                    LastUpdateDateTime = GETDATE()
                            WHERE   TypeName = @TypeName
                                    AND SortNumber = @SortNumber;
                           END;
                        ELSE
                           BEGIN
                            INSERT  INTO Configuration..LiveWorkShopConfig
                                    ( TypeName ,
                                    Picture ,
                                    Content ,
                                    H5Url ,
                                    PcUrl ,
                                    Gif ,
                                    SortNumber ,
                                    ShopId ,
                                    ChannelName
                                    )
                            VALUES  ( @TypeName ,
                                      @Picture ,
                                      @Content ,
                                      @H5Url ,
                                      @PcUrl ,
                                      @Gif ,
                                      @SortNumber ,
                                      @ShopId ,
                                      @ChannelName 
                                    );  
                        END;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@TypeName", model.TypeName),
                new SqlParameter("@Picture", model.Picture),
                new SqlParameter("@Content", model.Content),
                new SqlParameter("@H5Url", model.H5Url),
                new SqlParameter("@PcUrl", model.PcUrl),
                new SqlParameter("@Gif", model.Gif),
                new SqlParameter("@SortNumber", model.SortNumber),
                new SqlParameter("@ShopId", model.ShopId),
                new SqlParameter("@ChannelName", model.ChannelName)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
    }
}
