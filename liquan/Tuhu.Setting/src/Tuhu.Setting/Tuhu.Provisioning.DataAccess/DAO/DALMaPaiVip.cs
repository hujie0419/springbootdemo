using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALMaPaiVip
    {
        /// <summary>
        /// 获取券码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<MaPaiVipModel> SelectContinentalActivityList(SqlConnection conn, string keyword, int pageIndex, int pageSize)
        {
            return conn.Query<MaPaiVipModel>(@"SELECT  * ,COUNT(1) OVER() AS Total
            FROM    Activity..MaPaiVip WITH ( NOLOCK )
            WHERE   ( @Keyword = ''
                      OR UniquePrivilegeCode = @Keyword
                      OR UserPhone = @Keyword
                    )
            ORDER BY PKID DESC
                    OFFSET @PageSize * ( @PageIndex - 1 ) ROWS FETCH NEXT @PageSize ROWS
                    ONLY",
            new
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 根据券码查询信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public static MaPaiVipModel SelectContinentalConfigInfoByCouponCode(SqlConnection conn, string couponCode)
        {
            return conn.Query<MaPaiVipModel>(@"SELECT  * FROM    Activity..MaPaiVip WITH(NOLOCK) WHERE   UniquePrivilegeCode = @UniquePrivilegeCode", new { UniquePrivilegeCode = couponCode }, commandType: CommandType.Text).SingleOrDefault();
        }

        /// <summary>
        /// 增加券码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="couponCode"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static int InsertContinentalConfig(SqlConnection conn, string couponCode, bool isDeleted)
        {
            return conn.Execute(@"INSERT INTO Activity..MaPaiVip
                ( UniquePrivilegeCode ,
                  IsDeleted ,
                  CreatedTime 
                )
        OUTPUT Inserted.PKID
        VALUES  ( @UniquePrivilegeCode ,
                  @IsDeleted , 
                  GETDATE() 
        )", new { UniquePrivilegeCode = couponCode, IsDeleted = isDeleted }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 删除券码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkidStr"></param>
        /// <returns></returns>
        public static int DeletedContinentalConfig(SqlConnection conn, string pkidStr)
        {
            return conn.Execute(@"WITH    pkidList
          AS ( SELECT   *
               FROM     Gungnir..SplitString(@PKIDStr, ',', 1)
             )
            UPDATE Activity..MaPaiVip
            SET     IsDeleted = 1 ,
                    UpdatedTime = GETDATE()
            WHERE   EXISTS ( SELECT 1
                     FROM   pkidList AS p
                     WHERE  PKID = p.Item );", new { PKIDStr = pkidStr }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 更新券码状态
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static int UpdateContinentalConfigStatus(SqlConnection conn, long pkid, int status)
        {
            return conn.Execute(@"UPDATE Activity..MaPaiVip SET IsDeleted=@IsDeleted,UpdatedTime=GETDATE() WHERE PKID=@PKID", new { PKID = pkid, IsDeleted = status }, commandType: CommandType.Text);
        }

        public static int UpdateConfigByCouponCode(SqlConnection conn, string couponCode, bool isDeleted)
        {
            return conn.Execute(@"UPDATE Activity..MaPaiVip SET IsDeleted=@IsDeleted ,UpdatedTime=GETDATE() WHERE UniquePrivilegeCode=@CouponCode", new { CouponCode = couponCode, IsDeleted = isDeleted }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 批量添加券码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        //public static void BatchInsertContinentalConfig(SqlConnection conn, List<MaPaiVipModel> data)
        //{
        //    using (var sbc = new SqlBulkCopy(conn))
        //    {

        //        sbc.BatchSize = 1000;
        //        sbc.BulkCopyTimeout = 0;
        //        sbc.DestinationTableName = "Activity..MaPaiVip";
        //        DataTable table = new DataTable();
        //        table.Columns.Add("UniquePrivilegeCode");
        //        table.Columns.Add("IsDeleted");
        //        foreach (DataColumn col in table.Columns)
        //        {
        //            sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
        //        }
        //        foreach (var item in data)
        //        {
        //            var row = table.NewRow();
        //            row["UniquePrivilegeCode"] = item.UniquePrivilegeCode.Trim();
        //            row["IsDeleted"] = item.IsDeleted;
        //            table.Rows.Add(row);
        //        }
        //        sbc.WriteToServer(table);
        //    };
        //}
    }
}
