using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;

namespace Tuhu.Provisioning.DataAccess.DAO.RegionPackageMap
{
    public class DALRegionPackageMap
    {
        /// <summary>
        /// 获取地区和包的对应关系
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <param name="byPackagePID"></param>
        /// <param name="businessId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<PingAnRegionPackageMap> GetPingAnRegionPackageMapList(SqlConnection conn, Guid? packageId, string byPackagePID,
            string businessId, int pageIndex, int pageSize)
        {
            const string sql = @"
            SELECT  pm.PKID ,
                    pm.RegionId ,
                    pm.PackageId ,
                    pm.BYPackagePID ,
                    pm.BusinessId ,
                    pm.CreatedDateTime ,
                    pm.UpdatedDateTime ,
                    COUNT(1) OVER ( ) AS Total
             FROM   Tuhu_thirdparty..PingAnRegionPackageMap AS pm WITH ( NOLOCK )
             WHERE  ( @BYPackagePID = ''
                      OR @BYPackagePID IS NULL
                      OR pm.BYPackagePID = @BYPackagePID
                    )
                    AND ( @PackageId = '00000000-0000-0000-0000-000000000000'
                          OR @PackageId IS NULL
                          OR pm.PackageId = @PackageId
                        )
                    AND ( @BusinessId = ''
                          OR @BusinessId IS NULL
                          OR pm.BusinessId = @BusinessId
                        )
             ORDER BY pm.PKID DESC
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                    ONLY;";
            return conn.Query<PingAnRegionPackageMap>(sql, new
            {
                PackageId = packageId ?? Guid.Empty,
                BYPackagePID = byPackagePID,
                BusinessId = businessId,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }
        /// <summary>
        /// 获取所有的配置
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<PingAnRegionPackageMap> GetAllPingAnRegionPackageMapList(SqlConnection conn)
        {
            const string sql = @"
            SELECT  pm.PKID ,
                    pm.RegionId ,
                    pm.PackageId ,
                    pm.BYPackagePID ,
                    pm.BusinessId ,
                    pm.CreatedDateTime ,
                    pm.UpdatedDateTime 
             FROM   Tuhu_thirdparty..PingAnRegionPackageMap AS pm WITH ( NOLOCK )
             ORDER BY pm.PKID DESC";
            return conn.Query<PingAnRegionPackageMap>(sql, commandType: CommandType.Text).ToList();
        }
        /// <summary>
        /// 新增对应关系
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool InsertPingAnRegionPackageMap(SqlConnection conn, PingAnRegionPackageMap data)
        {
            const string sql = @"
            INSERT INTO Tuhu_thirdparty..PingAnRegionPackageMap
                    ( RegionId ,
                      PackageId ,
                      BYPackagePID ,
                      BusinessId ,
                      CreatedDateTime ,
                      UpdatedDateTime
                    )
             VALUES ( @RegionId ,
                      @PackageId ,
                      @BYPackagePID ,
                      @BusinessId ,
                      GETDATE() ,
                      GETDATE()
                    );";
            return conn.Execute(sql, new
            {
                PackageId = data.PackageId,
                BYPackagePID = data.BYPackagePID,
                BusinessId = data.BusinessId,
                RegionId = data.RegionId
            }, commandType: CommandType.Text) > 0;
        }
        /// <summary>
        /// 更新对应关系
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdatePingAnRegionPackageMap(SqlConnection conn, PingAnRegionPackageMap data)
        {
            const string sql = @"
             UPDATE Tuhu_thirdparty..PingAnRegionPackageMap
             SET    PackageId = @PackageId ,
                    BYPackagePID = @BYPackagePID ,
                    UpdatedDateTime = GETDATE()
             WHERE  PKID = @PKID
                    AND BusinessId = @BusinessId; ";
            return conn.Execute(sql, new
            {
                PackageId = data.PackageId != null && data.PackageId != Guid.Empty ? data.PackageId : null,
                BYPackagePID = !string.IsNullOrEmpty(data.BYPackagePID) ? data.BYPackagePID : null,
                BusinessId = data.BusinessId,
                RegionId = data.RegionId,
                PKID = data.PKID
            }, commandType: CommandType.Text) > 0;
        }
        /// <summary>
        /// 删除对应关系
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeletePingAnRegionPackageMap(SqlConnection conn, int pkid)
        {
            const string sql = @" DELETE FROM Tuhu_thirdparty..PingAnRegionPackageMap WHERE  PKID = @PKID";
            return conn.Execute(sql, new { PKID = pkid }, commandType: CommandType.Text) > 0;
        }
    }
}
