using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    public class DalInstallNow
    {
        public static IEnumerable<InstallNowModel> SelectList(InstallNowConditionModel request, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                pager.TotalItem = GetListCount(request, dbHelper);
                return dbHelper.ExecuteDataTable(@"SELECT  TINR.* ,
                                                           VP.TireSize ,
                                                           R1.RegionName AS City ,
                                                           ISNULL(R2.RegionName, R1.RegionName) AS Province
                                                   FROM    Configuration.dbo.tbl_TireInstallNow_Region AS TINR WITH ( NOLOCK )
                                                           JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON TINR.PID = VP.PID
                                                           JOIN Gungnir.dbo.tbl_region AS R1 WITH ( NOLOCK ) ON TINR.CityId = R1.PKID
                                                           LEFT JOIN Gungnir.dbo.tbl_region AS R2 WITH ( NOLOCK ) ON R1.ParentID = R2.PKID
                                                   WHERE   (TINR.PID LIKE '%'+ @PID+'%'
                                                           OR @PID IS NULL)
                                                           AND (VP.TireSize = @TireSize
                                                           OR @TireSize IS NULL)
                                                           AND (TINR.Status = @Status
                                                           OR @Status IS NULL)
                                                           AND (@CityIds IS NULL
                                                           OR TINR.CityId IN ( SELECT  *
                                                                               FROM    Gungnir.dbo.Split(@CityIds, ';') ))
                                                   ORDER BY TINR.PKID
                                                           OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                                                   	  FETCH NEXT @PageSize ROWS ONLY;", CommandType.Text, new SqlParameter[] {
                                                     new SqlParameter("@PID",request.PID),
                                                     new SqlParameter("@TireSize",request.TireSize),
                                                     new SqlParameter("@Status",request.Status),
                                                     new SqlParameter("@CityIds",request.CityIds),
                                                     new SqlParameter("@PageSize",pager.PageSize),
                                                     new SqlParameter("@PageIndex",pager.CurrentPage)
                }).ConvertTo<InstallNowModel>();
            }
        }

        public static int BitchOn(IEnumerable<InstallNowModel> list)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                dbHelper.BeginTransaction();
                foreach (var model in list)
                {
                    var countSame = SelectSameCitySameTireSizeStatusOnCount(dbHelper, model.CityId, model.PID);
                    if (countSame > 4)
                    {
                        dbHelper.Rollback();
                        return -99;
                    }
                    else
                    {
                        var rowCount = SingleOn(dbHelper, model.PKID);
                        if (rowCount <= 0)
                        {
                            dbHelper.Rollback();

                            return -101;
                        }
                    }
                }
                dbHelper.Commit();
                return 1;
            }
        }


        public static int BitchOff(string pkids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE Configuration.dbo.tbl_TireInstallNow_Region SET Status=0 WHERE  
                                            PKID IN ( SELECT    * FROM      Gungnir.dbo.Split(@PKIDS, ','))", CommandType.Text,
                      new SqlParameter("@PKIDS", pkids));
            }
        }
        public static int SingleOn(SqlDbHelper dbHelper, int pkid)
        {

            return dbHelper.ExecuteNonQuery(@"UPDATE Configuration.dbo.tbl_TireInstallNow_Region SET Status=1 WHERE  
                                            PKID =@PKID", CommandType.Text,
                  new SqlParameter("@PKID", pkid));
        }

        public static int DeleteInstallNow(int pkid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE Configuration.dbo.tbl_TireInstallNow_Region 
                                             WHERE PKID=@PKID;", CommandType.Text, new SqlParameter[] {
                                      new SqlParameter("@PKID",pkid)
           });
            }
        }
        public static IEnumerable<InstallNowModel> SelectInstallNowByPKIDS(string PKIDS)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT    *
                                                   FROM      Configuration.dbo.tbl_TireInstallNow_Region WITH ( NOLOCK )
                                                   WHERE     PKID IN ( SELECT    *
                                                                       FROM      Gungnir.dbo.Split(@PKIDS, ',') )", CommandType.Text, new SqlParameter[] {
                                      new SqlParameter("@PKIDS",PKIDS)
           }).ConvertTo<InstallNowModel>();
            }
        }

        public static ResultModel SaveInstallNow(string cityIds, IEnumerable<PidModel> pIDS)
        {
            var listExsit = new List<InstallNowModel>();
            var listSuccess = new List<InstallNowModel>();
            var result = new ResultModel();
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                dbHelper.BeginTransaction();
                foreach (var city in cityIds.Split(';'))
                {
                    var cityId = Convert.ToInt32(city);
                    foreach (var item in pIDS)
                    {
                        if (!IsExsitSame(dbHelper, cityId, item.PID))
                        {
                            var countSame = SelectSameCitySameTireSizeStatusOnCount(dbHelper, cityId, item.PID);
                            if (item.Status && countSame > 4)
                            {
                                dbHelper.Rollback();
                                result.ReturnCode = -1;
                                result.ReturnMessage = "已达到该城市相同规格上线!";
                                return result;
                            }
                            else
                            {
                                var rowCount = InsertInstallNow(dbHelper, cityId, item);
                                if (rowCount == 1)
                                    listSuccess.Add(new InstallNowModel()
                                    {
                                        PID = item.PID,
                                        Status = item.Status,
                                        CityId = cityId
                                    });
                                else
                                {
                                    dbHelper.Rollback();
                                    result.ReturnCode = -1;
                                    result.ReturnMessage = "保存失败!";
                                    return result;
                                }
                            }
                        }
                        else
                            listExsit.Add(new InstallNowModel()
                            {
                                PID = item.PID,
                                Status = item.Status,
                                CityId = cityId
                            });
                    }
                }
                if (listExsit.Any() && !listSuccess.Any())
                {
                    result.ReturnCode = -3;
                    result.ReturnMessage = "全部已存在,不允许重复添加！";
                }
                else
                {
                    result.ReturnCode = 1;
                    result.ReturnMessage = "保存成功!";
                    result.IsSuccess = true;
                    result.ExsitItem = listExsit;
                    result.SuccessItem = listSuccess;
                }
                dbHelper.Commit();
                return result;
            }
        }

        private static int InsertInstallNow(SqlDbHelper dbHelper, int cityId, PidModel item)
        {
            return dbHelper.ExecuteNonQuery(@"INSERT  INTO Configuration.dbo.tbl_TireInstallNow_Region
                                             ( PID, Status, CityId )
                                     VALUES  ( @PID, @Status, @CityId );", CommandType.Text, new SqlParameter[] {
                                      new SqlParameter("@CityId",cityId),
                                      new SqlParameter("@PID",item.PID),
                                       new SqlParameter("@Status",item.Status),
           });
        }

        private static int SelectSameCitySameTireSizeStatusOnCount(SqlDbHelper dbHelper, int cityId, string pID)
        {
            var result = dbHelper.ExecuteScalar(@" SELECT  COUNT(1)
                                                   FROM    Configuration.dbo.tbl_TireInstallNow_Region AS TINR WITH ( NOLOCK )
                                                           JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON TINR.PID = VP.PID
                                                   WHERE   VP.TireSize IN (
                                                           SELECT  VP2.TireSize
                                                           FROM    Tuhu_productcatalog.dbo.vw_Products AS VP2 WITH ( NOLOCK )
                                                           WHERE   VP2.PID = @PID )
                                                           AND TINR.CityId = @CityId
                                                           AND TINR.Status = 1", CommandType.Text, new SqlParameter[] {
                                      new SqlParameter("@CityId",cityId),
                                      new SqlParameter("@PID",pID)
           });
            return Convert.ToInt32(result);
        }

        private static bool IsExsitSame(SqlDbHelper dbHelper, int cityId, string pid)
        {
            var result = dbHelper.ExecuteScalar(@"SELECT  1
                                    FROM    Configuration.dbo.tbl_TireInstallNow_Region AS TINR WITH ( NOLOCK )
                                    WHERE   TINR.CityId = @CityId
                                            AND TINR.PID = @PID", CommandType.Text, new SqlParameter[] {
                                      new SqlParameter("@CityId",cityId),
                                      new SqlParameter("@PID",pid)
           });
            return result != null && result != DBNull.Value;
        }

        public static string FetchDisPlayNameByPID(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var result = dbHelper.ExecuteScalar("SELECT VP.DisplayName FROM  Tuhu_productcatalog.dbo.vw_Products AS VP WITH(NOLOCK) WHERE PID=@PID", CommandType.Text, new SqlParameter("@PID", pid));
                return result == null || result == DBNull.Value ? null : result.ToString();
            }
        }

        private static int GetListCount(InstallNowConditionModel request, SqlDbHelper dbHelper)
        {
            return Convert.ToInt32(dbHelper.ExecuteScalar(@"SELECT  COUNT(1)
                                                   FROM    Configuration.dbo.tbl_TireInstallNow_Region AS TINR WITH ( NOLOCK )
                                                           JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON TINR.PID = VP.PID
                                                           JOIN Gungnir.dbo.tbl_region AS R1 WITH ( NOLOCK ) ON TINR.CityId = R1.PKID
                                                           LEFT JOIN Gungnir.dbo.tbl_region AS R2 WITH ( NOLOCK ) ON R1.ParentID = R2.PKID
                                                   WHERE   (TINR.PID LIKE '%'+ @PID+'%'
                                                           OR @PID IS NULL)
                                                           AND (VP.TireSize = @TireSize
                                                           OR @TireSize IS NULL)
                                                           AND (TINR.Status = @Status
                                                           OR @Status IS NULL)
                                                           AND (@CityIds IS NULL
                                                           OR TINR.CityId IN ( SELECT  *
                                                                               FROM    Gungnir.dbo.Split(@CityIds, ';') ))", CommandType.Text, new SqlParameter[] {
                                                     new SqlParameter("@PID",request.PID),
                                                     new SqlParameter("@TireSize",request.TireSize),
                                                     new SqlParameter("@Status",request.Status),
                                                     new SqlParameter("@CityIds",request.CityIds)
                }));
        }
    }
}
