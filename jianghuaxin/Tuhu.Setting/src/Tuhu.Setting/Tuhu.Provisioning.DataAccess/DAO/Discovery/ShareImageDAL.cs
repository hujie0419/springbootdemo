using Dapper;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    /// <summary>
    /// 晒图数据访问
    /// </summary>
    public class ShareImageDAL
    {

        public static IEnumerable<ShareImage> SelectList(SqlConnection connection,string strWhere)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"
                                    SELECT * FROM Marketing..tbl_ShareImagesInfo(NOLOCK) A
                                    WHERE 1=1 " + strWhere;
                //ShareImage imageInfo = null;

                //var query = conn.Query<ShareImage, ImagesDetail, ShareImage>(sql,
                //    (imgInfo, imgDetails) =>
                //    {
                //        //扫描第一条记录，判断非空和非重复
                //        if (imageInfo == null || imageInfo.PKID != imgInfo.PKID)
                //            imageInfo = imgInfo;
                //        if (imgDetails != null)
                //            imageInfo.images.Add(imgDetails);
                //        return imageInfo;
                //    }).Distinct();
                //return query;
                return conn.Query<ShareImage>(sql);

            }
        }
        public static int ExecuteSqlForUpdate(SqlConnection connection, string sql)
        {
            using (connection)
            {
                return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql);
            }
        }


        public static DataTable SelectShareList(SqlConnection connection, PagerModel pager,string strWhere)
        {
            using (connection)
            {
                object totalCount = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT COUNT(1) FROM Marketing..tbl_ShareImagesInfo(NOLOCK) A WHERE 1=1 " + strWhere);
                pager.TotalItem = Convert.ToInt32(totalCount);
                string sql = @"
                                    SELECT * FROM
                                    (
                                        SELECT A.*,B.likesCount,B.commentCount,B.shareCount,ROW_NUMBER() OVER(ORDER BY A.PKID DESC) AS RowNum
                                        FROM Marketing..tbl_ShareImagesInfo(NOLOCK) A
                                        INNER JOIN Marketing..tbl_ShareImagesStatistics(NOLOCK) B ON A.PKID=B.infoId
                                        WHERE 1=1  {0}
                                    ) AS T
                                    WHERE T.RowNum BETWEEN (@PageIndex-1)*@PageSize+1 AND @PageIndex*@PageSize";
                var sqlParams = new[] {
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize)
                 };
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, string.Format(sql, strWhere), sqlParams);
            }
        }

        public static DataRow SelectShareDetailByPKID(SqlConnection connection, int PKID)
        {
            using (connection)
            {
                return SqlHelper.ExecuteDataRow(connection, CommandType.Text, @"SELECT TOP 1 A.*,B.likesCount,B.commentCount,B.shareCount
                                FROM Marketing..tbl_ShareImagesInfo(NOLOCK) A
                                INNER JOIN Marketing..tbl_ShareImagesStatistics(NOLOCK) B ON A.PKID = B.infoId WHERE A.PKID=@PKID ", new SqlParameter("@PKID", PKID));
            }
        }


        public static DataTable SelectShareImages(SqlConnection connection, int PKID)
        {
            using (connection)
            {
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, "SELECT * FROM Marketing..tbl_ShareImagesDetail(NOLOCK) WHERE infoId=@PKID AND isDelete=0", new SqlParameter("@PKID", PKID));
            }
        }

        public static DataRow GetUserInfo(SqlConnection connectionStr, string userId)
        {
            using (connectionStr)
            {
                string strSql = @"
                                    SELECT TOP 1 UserID,
                                    u_Pref5,
                                    u_last_name,
                                    u_mobile_number,
                                    u_Imagefile 
                                    FROM Tuhu_profiles.dbo.UserObject(NOLOCK)   
                                    WHERE UserID=@UserID ";

                var sqlParams = new[] {
                new SqlParameter("@UserID",userId)
                };
                return SqlHelper.ExecuteDataRow(connectionStr, CommandType.Text, strSql, sqlParams);
            }
        }




        public static bool UpdateShareImages(SqlConnection connection, ShareImage model)
        {
            SqlTransaction tran;
            if (connection.State == ConnectionState.Closed) connection.Open();
            tran = connection.BeginTransaction();

            try
            {
                string shareInfoSql = "UPDATE Marketing..tbl_ShareImagesInfo SET content=@content,isActive=@isActive,[status]=@status,lastUpdateTime=@lastUpdateTime WHERE PKID=@PKID";
                var sqlParamsInfo = new SqlParameter[]
                  {
                        new SqlParameter("@content",model.content),
                        new SqlParameter("@isActive",model.isActive),
                        new SqlParameter("@status",model.isActive?"AuditPass":"AuditNotPass"),
                        new SqlParameter("@lastUpdateTime",model.lastUpdateTime),
                        new SqlParameter("@PKID",model.PKID)
                  };
                bool r = SqlHelper.ExecuteNonQuery(tran, CommandType.Text, shareInfoSql, sqlParamsInfo) > 0;

                List<bool> flag = new List<bool>();
                //更新图片的状态->是否删除
                string shareDetailSql = "UPDATE Marketing..tbl_ShareImagesDetail SET isDelete=@isDelete WHERE PKID=@PKID";
                if (model.images!=null&&model.images.Count > 0)
                {
                    foreach (var img in model.images)
                    {
                        var sqlParamsDetail = new SqlParameter[]
                        {
                            new SqlParameter("@isDelete",img.isDelete),
                            new SqlParameter("@PKID",img.PKID)
                        };
                        flag.Add(SqlHelper.ExecuteNonQuery(tran, CommandType.Text, shareDetailSql, sqlParamsDetail) > 0);
                    }
                }
                //如果状态通过->将所有图片也设为通过
                if (model.isActive)
                {
                    flag.Add(SqlHelper.ExecuteNonQuery(tran, CommandType.Text, "UPDATE Marketing..tbl_ShareImagesDetail SET isActive=1 WHERE infoId=@PKID", new SqlParameter("@PKID", model.PKID))>0);
                }

                if (r && !flag.Contains(false))
                {
                    tran.Commit();
                    return true;
                }
                else
                {
                    tran.Rollback();
                    return false;
                }
            }
            catch(Exception ex)
            {
                tran.Rollback();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return false;
                throw ex;
            }
            finally
            {
                tran.Dispose();
            }

        }


        public static bool UpdateStatus(SqlConnection conn,int PKID,bool isActive)
        {
            using (conn)
            {
                string shareInfoSql = "UPDATE Marketing..tbl_ShareImagesInfo SET isActive=@isActive,[status]=@status WHERE PKID=@PKID";
                var sqlParamsInfo = new SqlParameter[]
                  {
                        new SqlParameter("@isActive",isActive),
                        new SqlParameter("@status",isActive?"AuditPass":"AuditNotPass"),
                        new SqlParameter("@PKID",PKID)
                  };

                int res = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, shareInfoSql, sqlParamsInfo);
                if (isActive)//通过->将所有图片也设为通过
                {
                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text,"UPDATE Marketing..tbl_ShareImagesDetail SET isActive=1 WHERE infoId=@PKID",new SqlParameter("@PKID", PKID));
                }
                return res > 0;
            }
        }
    }
}
