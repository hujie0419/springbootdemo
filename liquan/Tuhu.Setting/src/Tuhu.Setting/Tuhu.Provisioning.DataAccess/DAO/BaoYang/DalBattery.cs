using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Battery;
using Tuhu.Provisioning.DataAccess.Request;

namespace Tuhu.Provisioning.DataAccess.DAO.BaoYang
{
    public class DalBattery
    {
        #region 公共的

        /// <summary>
        /// 获取蓄电池全部品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectBatteryBrands(SqlConnection conn)
        {
            const string sql = @"SELECT DISTINCT
        vp.CP_Brand
FROM    Tuhu_productcatalog..vw_Products AS vp WITH ( NOLOCK )
WHERE   vp.Category = N'battery'";
            var result = conn.Query<string>(sql, commandType: CommandType.Text);
            return result;
        }

        #endregion

        #region 保养蓄电池

        /// <summary>
        /// 获取全部数据, 导入导出用
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        public IEnumerable<BaoYangBatteryCoverArea> SelectAllBaoYangBatteryCoverAreas(SqlConnection conn)
        {
            const string sql = @"
SELECT  t.PKID,
        t.Brand ,
        t.ProvinceId ,
        t.CityId ,
        t.Channels ,
        t.IsEnabled
FROM    BaoYang..BaoYangBatteryCoverArea AS t WITH ( NOLOCK )";
            var result = conn.Query<BaoYangBatteryCoverArea>(sql, commandType: CommandType.Text);
            return result;
        }

        /// <summary>
        /// 根据品牌,地区获取蓄电池数据
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsExistsBaoYangBatteryCoverArea(SqlConnection conn, BaoYangBatteryCoverArea item)
        {
            const string sql = @"SELECT  COUNT(1)
FROM    BaoYang.dbo.BaoYangBatteryCoverArea AS t WITH ( NOLOCK )
WHERE   t.Brand = @Brand
        AND t.ProvinceId = @ProvinceId
        AND t.CityId = @CityId
        AND t.PKID <> @PKID";
            var value = conn.ExecuteScalar(sql, new
            {
                item.CityId,
                item.ProvinceId,
                item.PKID,
                item.Brand,
            }, commandType: CommandType.Text);
            return Convert.ToInt32(value) > 0;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<BaoYangBatteryCoverArea> SelectBaoYangBatteryCoverAreas(SqlConnection conn,
            SearchBaoYangBatteryCoverAreaRequest request)
        {
            const string sql = @"SELECT  t.PKID ,
        t.Brand ,
        t.ProvinceId ,
        t.CityId ,
        t.Channels ,
        t.IsEnabled ,
        t.CreateDatetime ,
        t.LastUpdateDateTime ,
        COUNT(1) OVER ( ) AS Total
FROM    BaoYang.dbo.BaoYangBatteryCoverArea AS t WITH ( NOLOCK )
WHERE   ( @Brand IS NULL
          OR @Brand = N''
          OR t.Brand = @Brand
        )
        AND ( @ProvinceId <= 0
              OR t.ProvinceId = @ProvinceId
            )
        AND ( @CityId <= 0
              OR t.CityId = @CityId
            )
ORDER BY t.ProvinceId ,
        t.CityId
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
            var take = request.PageSize;
            var skip = (request.PageIndex - 1) * take;
            var result = conn.Query<BaoYangBatteryCoverArea>(sql, new
            {
                Take = take,
                Skip = skip,
                ProvinceId = request.City,
                CityId = request.Province,
                request.Brand
            }, commandType: CommandType.Text);
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteBaoYangBatteryCoverArea(SqlConnection conn, long pkid)
        {
            const string sql = @"DELETE  FROM BaoYang.dbo.BaoYangBatteryCoverArea WHERE   PKID = @PKID";
            var rows = conn.Execute(sql, new
            {
                PKID = pkid
            }, commandType: CommandType.Text);
            return rows > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddBaoYangBatteryCoverArea(SqlConnection conn, BaoYangBatteryCoverArea item)
        {
            const string sql = @"INSERT  INTO BaoYang.dbo.BaoYangBatteryCoverArea
        ( Brand ,
          ProvinceId ,
          CityId ,
          Channels ,
          IsEnabled ,
          CreateDatetime ,
          LastUpdateDateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @Brand ,
          @ProvinceId ,
          @CityId ,
          @Channels ,
          @IsEnabled ,
          GETDATE() ,
          GETDATE()
        )";
            var value = conn.ExecuteScalar(sql, new
            {
                item.ProvinceId,
                item.CityId,
                item.Channels,
                item.Brand,
                item.IsEnabled,
            }, commandType: CommandType.Text);
            return value != null ? Convert.ToInt32(value) : 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateBaoYangBatteryCoverArea(SqlConnection conn, BaoYangBatteryCoverArea item)
        {
            const string sql = @"UPDATE  BaoYang.dbo.BaoYangBatteryCoverArea
SET     CityId = @CityId ,
        ProvinceId = @ProvinceId ,
        Channels = @Channels ,
        IsEnabled = @IsEnabled
WHERE   PKID = @PKID";

            var rows = conn.Execute(sql, new
            {
                item.PKID,
                item.ProvinceId,
                item.CityId,
                item.Channels,
                item.Brand,
                item.IsEnabled,
            }, commandType: CommandType.Text);
            return rows > 0;
        }

        /// <summary>
        /// 根据ID查询
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public BaoYangBatteryCoverArea SelectBaoYangBatteryCoverAreaById(SqlConnection conn, long pkid)
        {
            const string sql = @"SELECT  t.PKID ,
        t.Brand ,
        t.ProvinceId ,
        t.CityId ,
        t.Channels ,
        t.IsEnabled ,
        t.CreateDatetime ,
        t.LastUpdateDateTime
FROM    BaoYang..BaoYangBatteryCoverArea AS t WITH ( NOLOCK )
WHERE   t.PKID = @PKID";
            var result = conn.QueryFirst<BaoYangBatteryCoverArea>(sql, new
            {
                PKID = pkid,
            }, commandType: CommandType.Text);
            return result;
        }

        #endregion
    }
}
