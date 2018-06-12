using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaoYangRefreshCacheService.Model;
using Tuhu;

namespace BaoYangRefreshCacheService.DAL
{
    public class KpiDal
    {
        private readonly static string connectionString = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

        const int timeout = 300;

        /// <summary>
        /// 获取所有适配人员姓名与品牌
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetAllPersonWithBrand()
        {
            var sql = @"SELECT  t1.Brand ,
        t2.Name
FROM    ( SELECT    tv.Brand
          FROM      Gungnir..tbl_Vehicle_Type AS tv WITH ( NOLOCK )
          GROUP BY  tv.Brand
        ) AS t1
        LEFT JOIN ( SELECT  Name ,
                            Item AS Brand
                    FROM    BaoYang..VehicleBrandsResponsibility AS vbr WITH ( NOLOCK )
                            CROSS APPLY Gungnir.dbo.SplitString(VehicleBrands,
                                                              N', ', 1)
                  ) AS t2 ON t1.Brand = t2.Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        #region 适配覆盖率

        /// <summary>
        /// 获取每个品牌的TID TotalCount
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetAllTidCountWithBrand()
        {
            var sql = @"SELECT  tv.Brand ,
        COUNT(DISTINCT Ti.TID) AS TotalCount
FROM    Gungnir.dbo.tbl_Vehicle_Type_Timing Ti WITH ( NOLOCK )
        INNER JOIN Gungnir.dbo.tbl_Vehicle_Type tv WITH ( NOLOCK ) ON tv.ProductID = Ti.VehicleID
GROUP BY tv.Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        /// <summary>
        /// 获取前num热门车型总的Tid数量
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetHotVehicleTidCountWithBrand(int num)
        {
            var parameter = new SqlParameter("@Num", num);
            var sql = @"SELECT  tv.Brand ,
        COUNT(DISTINCT Ti.TID) AS TotalCount
FROM    Gungnir..tbl_Vehicle_Type_Timing AS Ti WITH ( NOLOCK )
        INNER JOIN Gungnir.dbo.tbl_Vehicle_Type tv WITH ( NOLOCK ) ON tv.ProductID = Ti.VehicleID
        INNER JOIN SystemLog..tbl_ComVehicleRef AS Hot WITH ( NOLOCK ) ON Hot.VehicleID = Ti.VehicleID
                                                                                                    COLLATE Chinese_PRC_CI_AS
                                                              AND Hot.Rank <= @Num
GROUP BY tv.Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            cmd.Parameters.Add(parameter);
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        /// <summary>
        /// 获取车系总数 按品牌分类
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetVehicleSeriesCountWithBrand()
        {
            var sql = @"SELECT  Brand ,
        COUNT(DISTINCT ProductID) AS TotalCount
FROM    Gungnir..tbl_Vehicle_Type WITH ( NOLOCK )
GROUP BY Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        #endregion

        #region KpiReport
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        public void SqlBulkCopyByDatatable(SqlTransaction tran, DataTable table)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(tran.Connection, SqlBulkCopyOptions.Default, tran))
            {
                bulkCopy.DestinationTableName = table.TableName;
                foreach (DataColumn column in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                bulkCopy.WriteToServer(table);
            }
        }
        /// <summary>
        /// List转换为DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <param name="reportID"></param>
        /// <returns></returns>
        private DataTable ConvertListToDataTable(List<KpiReportDetail> list, int reportID)
        {
            const string tableName = "BaoYang..KpiReport_Detail";
            DataTable dt = new DataTable(tableName);

            dt.Columns.Add("ReportID", typeof(int));
            dt.Columns.Add("TypeName", typeof(string));
            dt.Columns.Add("ParameterName", typeof(string));
            dt.Columns.Add("VehicleAdaptCount", typeof(int));
            dt.Columns.Add("VehicleTotalCount", typeof(int));
            dt.Columns.Add("CategoryName", typeof(string));
            dt.Columns.Add("HotVehicleType", typeof(string));
            dt.Columns.Add("VehicleBrand", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            list.ForEach(x =>
            {
                DataRow row = dt.NewRow();
                row["ReportID"] = reportID;
                row["TypeName"] = x.TypeName;
                row["ParameterName"] = x.ParameterName;
                row["VehicleAdaptCount"] = x.VehicleAdaptCount;
                row["VehicleTotalCount"] = x.VehicleTotalCount;
                row["CategoryName"] = x.CategoryName;
                row["HotVehicleType"] = x.HotVehicleType;
                row["VehicleBrand"] = x.VehicleBrand;
                row["Name"] = x.Name;
                dt.Rows.Add(row);
            });
            return dt;
        }
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="list"></param>
        public void ExcuteTranfacantion(List<KpiReportDetail> list, string reportName, int reportTypeID, string version, string createUser)
        {
            var currentTime = DateTime.Now;
            var paras = new[]
            {
                new SqlParameter("@ReportTypeID", reportTypeID),
                new SqlParameter("@CreateTime", currentTime),
                new SqlParameter("@CreateUser", createUser),
                new SqlParameter("@Version", version),
                new SqlParameter("@ReportNo", $"Report-{reportName}-{version}-{currentTime.ToString("yyyyMMddHHmmss")}"),
                new SqlParameter("@ReportID", SqlDbType.Int) {Direction = ParameterDirection.Output },
            };
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                var tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                try
                {
                    cmd.CommandText = @"INSERT  INTO BaoYang..KpiReport( ReportTypeID , CreateTime , CreateUser , Version , ReportNo ) VALUES  ( @ReportTypeID , @CreateTime , @CreateUser , @Version , @ReportNo );
                                        SELECT @ReportID = @@IDENTITY;";
                    cmd.Parameters.AddRange(paras);
                    cmd.ExecuteNonQuery();
                    var reportID = (int)paras.LastOrDefault().Value;
                    var dt = ConvertListToDataTable(list, reportID);
                    SqlBulkCopyByDatatable(tran, dt);
                    tran.Commit();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    conn.Close();
                    throw ex;
                }
            }
        }
        public int GetKpiReportTypeIDByReportName(string reportName)
        {
            var parameter = new SqlParameter("@ReportName", reportName);
            var sql = @"SELECT  ID
                        FROM    BaoYang..KpiReport_Type WITH ( NOLOCK )
                        WHERE   ReportName = @ReportName;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            cmd.Parameters.Add(parameter);
            return (int)DbHelper.ExecuteScalar(true, cmd);
        }

        #endregion

        #region 已审核数据占总体数据比例

        /// <summary>
        /// 获取patNames是否审核的总数
        /// </summary>
        /// <param name="partNames"></param>
        /// <param name="isValidated"></param>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetValidatedBrandCount(string partNames)
        {
            var paras = new[] { new SqlParameter("@PartNames", partNames) };
            var sql = @"WITH    temp
          AS ( SELECT   tv.Brand ,
                        Ti.TID ,
                        Parts.IsValidated
               FROM     BaoYang..Tuhu_BaoYangParts AS Parts WITH ( NOLOCK )
                        INNER JOIN Gungnir.dbo.SplitString(@PartNames, N', ',
                                                           1) AS PartNames ON PartNames.Item = Parts.PartName COLLATE Chinese_PRC_CI_AS
                                                              AND Parts.IsDeleted = 0
                        INNER JOIN Gungnir..tbl_Vehicle_Type_Timing AS Ti WITH ( NOLOCK ) ON Ti.TID = Parts.TID
                        INNER JOIN Gungnir..tbl_Vehicle_Type AS tv WITH ( NOLOCK ) ON Ti.VehicleID = tv.ProductID
             )
    SELECT  t2.Brand ,
            ISNULL(t1.Count, 0) Count ,
            t2.Count AS TotalCount
    FROM    ( SELECT    temp.Brand ,
                        COUNT(DISTINCT temp.TID) AS Count
              FROM      temp
              WHERE     IsValidated = 1
              GROUP BY  temp.Brand
            ) t1
            RIGHT JOIN ( SELECT temp.Brand ,
                                COUNT(DISTINCT temp.TID) AS Count
                         FROM   temp
                         GROUP BY temp.Brand
                       ) t2 ON t2.Brand = t1.Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            cmd.Parameters.AddRange(paras);
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        #endregion

        #region 适配准确度

        /// <summary>
        /// 获取不适配数据原因订单数
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetOrederFeedBackAuditDataNoAdaptedCount()
        {
            var sql = @"SELECT  t.Brand ,
        COUNT(1) AS Count
FROM    ( SELECT    ISNULL(cars.Brand, CO.Brand) AS Brand ,
                    AuditReason
          FROM      ( SELECT    o.InstallShop ,
                                o.CarID ,
                                f.OrderID ,
                                f.OrderNO ,
                                f.ExamineStatus ,
                                f.Operator ,
                                f.CreateDate ,
                                o.OrderDatetime ,
                                f.ExaminePerson ,
                                f.ExamineDateTime ,
                                f.IsExamined ,
                                ( SELECT    ','
                                            + CAST(ts.PKID AS NVARCHAR(50))
                                            + ''
                                  FROM      Gungnir.dbo.Tousu ts WITH ( NOLOCK )
                                  WHERE     ts.OrderID = f.OrderID
                                FOR
                                  XML PATH('')
                                ) AS tousuIds ,
                                ROW_NUMBER() OVER ( ORDER BY f.CreateDate DESC ) AS num ,
                                f.OrderFieldResult ,-----新添加内容
                                t.AuditReason-----新添加内容
                      FROM      Gungnir.dbo.OrderFeedBack f WITH ( NOLOCK )
                                LEFT JOIN Gungnir.dbo.tbl_Order o WITH ( NOLOCK ) ON f.OrderID = o.PKID
                                LEFT JOIN Gungnir.dbo.tbl_OrderList ol WITH ( NOLOCK ) ON o.PKID = ol.OrderID
                                LEFT JOIN BaoYang..BaoYangOrderFeedBackAuditResult t
                                WITH ( NOLOCK ) ON t.OrderID = ol.OrderID
                                LEFT JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] crr
                                WITH ( NOLOCK ) ON crr.PID = ol.PID COLLATE Chinese_PRC_CI_AS
                      WHERE     o.InstallType = '1ShopInstall'
                                AND t.AuditReason LIKE N'%途虎适配数据问题%'
                                AND EXISTS ( SELECT 1
                                             FROM   Gungnir..tbl_Dictionaries
                                                    AS D ( NOLOCK )
                                             WHERE  D.DicType = 'BaoYangProducts'
                                                    AND D.DicKey = crr.PrimaryParentCategory COLLATE Chinese_PRC_CI_AS )
                      GROUP BY  f.CreateDate ,
                                o.InstallShop ,
                                o.CarID ,
                                f.OrderNO ,
                                f.OrderID ,
                                f.ExamineStatus ,
                                f.OrderFieldResult ,--新添加
                                f.Operator ,
                                f.ExaminePerson ,
                                f.ExamineDateTime ,
                                o.OrderDatetime ,
                                f.IsExamined ,
                                t.AuditReason -----新添加内容
                    ) b
                    LEFT JOIN Tuhu_profiles..CarObject AS CO WITH ( NOLOCK ) ON b.CarID = CO.CarID
                    LEFT JOIN Gungnir..tbl_OrderCarInfo AS cars WITH ( NOLOCK ) ON cars.OrderID = b.OrderID
        ) t
WHERE   t.Brand IS NOT NULL
        AND t.Brand <> N''
GROUP BY t.Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        /// <summary>
        /// 获取总的到店订单数
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetTotalOrderCount()
        {
            var sql = @"SELECT  t.Brand ,
        COUNT(1) AS TotalCount
FROM    ( SELECT    ISNULL(cars.Brand, CO.Brand) AS Brand
          FROM      Gungnir..tbl_BaoYang_ShopInstallOrderRecords AS shopOrder ( NOLOCK )
                    LEFT JOIN Gungnir..tbl_OrderCarInfo AS cars WITH ( NOLOCK ) ON cars.OrderID = shopOrder.OrderID
                    LEFT JOIN Tuhu_profiles..CarObject AS CO WITH ( NOLOCK ) ON shopOrder.CarID = CO.CarID
          WHERE     shopOrder.InstallType = '1ShopInstall'
                    AND shopOrder.OrderStatus <> '7Canceled'
                    AND shopOrder.OrderStatus <> '0New'
                    AND shopOrder.OrderChannel <> 'f8857'
        ) t
WHERE   t.Brand IS NOT NULL
        AND t.Brand <> N''
GROUP BY t.Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        /// <summary>
        /// 到店定责研发部订单数
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BrandCount> GetOrderResponseDepart()
        {
            var sql = @"SELECT  t.Brand ,
        COUNT(1) AS Count
FROM    ( SELECT    ISNULL(cars.Brand, CO.Brand) AS Brand
          FROM      Gungnir..tbl_BaoYang_ShopInstallOrderRecords AS shopOrder ( NOLOCK )
                    INNER JOIN Gungnir..Tousu AS t ( NOLOCK ) ON shopOrder.OrderID = t.OrderID
                    LEFT JOIN Gungnir..tbl_OrderCarInfo AS cars WITH ( NOLOCK ) ON cars.OrderID = shopOrder.OrderID
                    LEFT JOIN Tuhu_profiles..CarObject AS CO WITH ( NOLOCK ) ON shopOrder.CarID = CO.CarID
          WHERE     ( t.SubTousuType3 = 'bushipei'
                      OR t.SubTousuType2 = 'baoyangbushipei'
                      OR t.SubTousuType2 = 'chanpinlei-baoyang-bushipei'
                    )
                    AND t.ResponseDepart = N'研发部'
                    AND shopOrder.InstallType = '1ShopInstall'
                    AND shopOrder.OrderChannel <> 'f8857'
        ) t
WHERE   t.Brand IS NOT NULL
        AND t.Brand <> N''
GROUP BY t.Brand;";
            var cmd = new SqlCommand(sql) { CommandTimeout = timeout, CommandType = CommandType.Text };
            return DbHelper.ExecuteSelect<BrandCount>(true, cmd);
        }

        #endregion

    }
}
