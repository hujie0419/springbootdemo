using Dapper;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage;

namespace Tuhu.Provisioning.DataAccess.DAO.VipBaoYangPackage
{
    public class DALVipBaoYangPackage
    {
        public static List<VipBaoYangPackageModel> SelectVipBaoYangPackage(SqlConnection conn, string pid, Guid vipUserId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT  t.PKID ,
        t.PID ,
        t.PackageName ,
        t.VipUserId ,
        t.Brands ,
        t.Price ,
        t.Volume ,
        t.CreateUser ,
        t.CreateDateTime ,
        t.SettlementMethod ,
        t.SettlementVipUserId ,
        t.GetRuleGUID ,
        t.Source ,
        COUNT(*) OVER ( ) AS Total
FROM    BaoYang..VipBaoYangPackageConfig AS t WITH ( NOLOCK )
WHERE   ( @PID = ''
          OR t.PID = @PID
        )
        AND ( @VipUserId = '00000000-0000-0000-0000-000000000000'
              OR t.VipUserId = @VipUserId
            )
ORDER BY t.CreateDateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            return conn.Query<VipBaoYangPackageModel>(sql, new { PID = pid, VipUserId = vipUserId.ToString(), PageIndex = pageIndex, PageSize = pageSize }, commandType: CommandType.Text).ToList();
        }
        /// <summary>
        /// 获取所有的保养套餐
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<VipBaoYangPackageModel> SelectAllVipBaoYangPackage(SqlConnection conn)
        {
            const string sql = @"
            SELECT  t.PKID ,
                    t.PID ,
                    t.PackageName ,
                    t.VipUserId ,
                    t.Brands ,
                    t.Price ,
                    t.Volume ,
                    t.CreateUser ,
                    t.CreateDateTime ,
                    t.SettlementMethod ,
                    t.SettlementVipUserId ,
                    t.GetRuleGUID ,
                    t.Source 
            FROM    BaoYang..VipBaoYangPackageConfig AS t WITH ( NOLOCK )
            ORDER BY t.PKID DESC";
            return conn.Query<VipBaoYangPackageModel>(sql, commandType: CommandType.Text).ToList();
        }

        public static VipBaoYangPackageDbModel SelectVipBaoYangPackageByPkid(SqlConnection conn, int pkid)
        {
            const string sql = @"SELECT  t.PKID ,
        t.PID ,
        t.PackageName ,
        t.VipUserId ,
        t.Brands ,
        t.Price ,
        t.Volume ,
        t.CreateUser ,
        t.SettlementMethod ,
        t.SettlementVipUserId ,
        t.GetRuleGUID ,
        t.Source 
FROM    BaoYang..VipBaoYangPackageConfig AS t WITH ( NOLOCK )
WHERE   PKID = @PKID;";
            return conn.Query<VipBaoYangPackageDbModel>(sql, new { PKID = pkid }, commandType: CommandType.Text).SingleOrDefault();
        }

        public static int IsExistsPackageName(SqlConnection conn, string packageName, int pkid)
        {
            const string sql = @"SELECT  COUNT(1)
FROM    BaoYang..VipBaoYangPackageConfig (NOLOCK)
WHERE   PKID <> @PKID
        AND PackageName = @PackageName;";
            return (int)conn.ExecuteScalar(sql, new { PackageName = packageName, PKID = pkid }, commandType: CommandType.Text);
        }

        public static BaoYangPackagePromotionRecord SelectPromotionRecordByBatchCode(SqlConnection conn, string batchCode)
        {
            const string sql = @"SELECT * FROM BaoYang..VipBaoYangPackagePromotionRecord (NOLOCK) WHERE BatchCode=@BatchCode";
            return conn.Query<BaoYangPackagePromotionRecord>(sql, new { BatchCode = batchCode }, commandType: CommandType.Text).SingleOrDefault();
        }

        public static List<BaoYangPackagePromotionRecord> SelectPromotionRecordByPackageId(SqlConnection conn, int packageId)
        {
            const string sql = @"SELECT * FROM BaoYang..VipBaoYangPackagePromotionRecord (NOLOCK) WHERE PackageId=@PackageId";
            return conn.Query<BaoYangPackagePromotionRecord>(sql, new { PackageId = packageId }, commandType: CommandType.Text).ToList();
        }

        public static List<PromotionOperationRecord> SelectPromotionOperationRecord(SqlConnection conn, string pid, Guid vipUserId, int packageId, string batchCode, string mobilePhone, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT  bppr.PackageId ,
        bppr.BatchCode ,
        scu.UserName AS VipUserName ,
        bpc.PID ,
        bpc.PackageName ,
        bpc.Volume ,
        ( SELECT    COUNT(1)
          FROM      BaoYang..VipBaoYangPackagePromotionDetail (NOLOCK) AS bpr
          WHERE     bpr.BatchCode = bppr.BatchCode
        ) AS WaitCount ,
        ( SELECT    COUNT(1)
          FROM      BaoYang..VipBaoYangPackagePromotionDetail (NOLOCK) AS bpr
          WHERE     bpr.BatchCode = bppr.BatchCode
                    AND bpr.Status = 'SUCCESS'
        ) AS SuccessCount ,
        bppr.CreateUser ,
        bppr.CreateDateTime,
        bppr.IsSendSms,
        COUNT(*) OVER ( ) AS Total
FROM    BaoYang..VipBaoYangPackageConfig (NOLOCK) AS bpc
        LEFT JOIN BaoYang..VipBaoYangPackagePromotionRecord (NOLOCK) AS bppr ON bpc.PKID = bppr.PackageId
        LEFT JOIN Tuhu_profiles..SYS_CompanyUser (NOLOCK) AS scu ON bpc.VipUserId = scu.UserId
WHERE  BatchCode <> ''
        AND ( @MobilePhone = ''
            OR  EXISTS ( SELECT    1
                        FROM      BaoYang..VipBaoYangPackagePromotionDetail
                                AS vppd WITH ( NOLOCK )
                        WHERE     vppd.MobileNumber = @MobilePhone
                                AND bppr.BatchCode = vppd.BatchCode )
        )
        AND ( @PID = ''
              OR bpc.PID = @pid
            )
        AND ( @VipUserId = '00000000-0000-0000-0000-000000000000'
              OR bpc.VipUserId = @VipUserId
            )
        AND ( @PackageId = 0
              OR bppr.PackageId = @PackageId
            )
        AND ( @BatchCode = ''
              OR bppr.BatchCode = @BatchCode
            )
ORDER BY bppr.CreateDateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            return conn.Query<PromotionOperationRecord>(sql, new
            {
                PID = pid,
                VipUserId = vipUserId,
                PackageId = packageId,
                BatchCode = batchCode,
                MobilePhone = mobilePhone,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public static List<BaoYangPackagePromotionDetail> SelectNoSuccessPromotionDetails(SqlConnection conn, string batchCode)
        {
            const string sql = @"SELECT * FROM  BaoYang..VipBaoYangPackagePromotionDetail (NOLOCK)  WHERE BatchCode=@BatchCode AND Status <> 'SUCCESS'";
            return conn.Query<BaoYangPackagePromotionDetail>(sql, new { BatchCode = batchCode }, commandType: CommandType.Text).ToList();
        }

        public static List<VipBaoYangPackageModel> GetBaoYangPackageNameByVipUserId(SqlConnection conn, Guid vipUserId)
        {
            const string sql = @"SELECT * FROM BaoYang..VipBaoYangPackageConfig (NOLOCK) WHERE VipUserId=@VipUserId";
            return conn.Query<VipBaoYangPackageModel>(sql, new { VipUserId = vipUserId }, commandType: CommandType.Text).ToList();
        }

        public static int InsertVipBaoYangPackage(SqlConnection conn, VipBaoYangPackageDbModel package)
        {
            const string sql = @"INSERT  INTO BaoYang..VipBaoYangPackageConfig
        ( PackageName ,
          VipUserId ,
          PID ,
          Brands ,
          Price ,
          Volume ,
          CreateUser ,
          SettlementMethod ,
          SettlementVipUserId ,
          GetRuleGUID ,
          Source ,
          CreateDateTime ,
          LastUpdateDateTime
	    )
OUTPUT  Inserted.PKID
VALUES  ( @PackageName ,
          @VipUserId ,
          N'' ,
          @Brands ,
          @Price ,
          @Volume ,
          @CreateUser ,
          @SettlementMethod ,
          @SettlementVipUserId ,
          @GetRuleGUID ,
          @Source ,
          GETDATE() ,
          GETDATE()
        );";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new
            {
                package.VipUserId,
                package.PackageName,
                package.Price,
                package.Brands,
                package.Volume,
                package.CreateUser,
                package.SettlementMethod,
                package.SettlementVipUserId,
                package.GetRuleGUID,
                package.Source
            }, commandType: CommandType.Text));
        }

        public static int UpdateVipBaoYangPackagePID(SqlConnection conn, int pkid, string pid)
        {
            const string sql = @"UPDATE BaoYang..VipBaoYangPackageConfig SET PID=@PID WHERE PKID=@PKID";
            return conn.Execute(sql, new { PKID = pkid, PID = pid }, commandType: CommandType.Text);
        }

        public static bool UpdateVipBaoYangPackage(SqlConnection conn, VipBaoYangPackageDbModel package)
        {
            const string sql = @"UPDATE  BaoYang..VipBaoYangPackageConfig
SET     Brands = @Brands ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID;";
            return conn.Execute(sql, new { package.PKID, package.Brands }, commandType: CommandType.Text) > 0;
        }

        public static int DeleteVipBaoYanPackage(SqlConnection conn, int pkid)
        {
            const string sql = @"DELETE FROM BaoYang..VipBaoYangPackageConfig WHERE PKID=@PKID";
            return conn.Execute(sql, new { PKID = pkid }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 更新结算用户信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <param name="settlementVipUserId"></param>
        /// <returns></returns>
        public static int UpdateVipBaoYangPackage(SqlConnection conn, VipBaoYangPackageViewModel package)
        {
            const string sql = @"    
            UPDATE  BaoYang..VipBaoYangPackageConfig
            SET     SettlementVipUserId = @SettlementVipUserId ,
                    GetRuleGUID = @GetRuleGUID ,
                    Source = @Source ,
                    LastUpdateDateTime = GETDATE()
            WHERE   PKID = @PKID";
            return conn.Execute(sql, new
            {
                PKID = package.PKID,
                SettlementVipUserId = package.SettlementVipUserId == Guid.Empty ? null : package.SettlementVipUserId.ToString(),
                GetRuleGUID = package.GetRuleGUID == Guid.Empty ? null : package.GetRuleGUID.ToString(),
                Source = package.Source
            }, commandType: CommandType.Text);
        }

        public static int InsertBaoYangPackagePromotionRecord(SqlConnection conn, int packageId, string batchCode, Guid rulesGUID, bool isSendSms, string user)
        {
            const string sql = @"	INSERT BaoYang..VipBaoYangPackagePromotionRecord
	                        ( PackageId ,
	                          BatchCode ,
	                          RulesGUID ,
	                          IsSendSms ,
	                          CreateUser ,
	                          CreateDateTime ,
	                          LastUpdateDateTime
	                        )
	                VALUES  ( @PackageId ,
	                          @BatchCode ,
	                          @RulesGUID ,
	                          @IsSendSms ,
	                          @CreateUser ,
	                          GETDATE() ,
	                          GETDATE()
	                        )";
            return conn.Execute(sql, new
            {
                PackageId = packageId,
                BatchCode = batchCode,
                RulesGUID = rulesGUID,
                IsSendSms = isSendSms,
                CreateUser = user
            }, commandType: CommandType.Text);
        }

        public static bool BatchBaoYangPakckagePromotion(SqlConnection conn, List<BaoYangPackagePromotionDetail> info, string batchCode)
        {
            using (var sbc = new SqlBulkCopy(conn))
            {
                sbc.BatchSize = 1000;
                sbc.BulkCopyTimeout = 0;
                //将DataTable表名作为待导入库中的目标表名
                sbc.DestinationTableName = "BaoYang..VipBaoYangPackagePromotionDetail";
                //将数据集合和目标服务器库表中的字段对应
                DataTable table = new DataTable();
                table.Columns.Add("BatchCode");
                table.Columns.Add("MobileNumber");
                table.Columns.Add("Carno");
                table.Columns.Add("PromotionId");
                table.Columns.Add("Status");
                foreach (DataColumn col in table.Columns)
                {
                    //列映射定义数据源中的列和目标表中的列之间的关系
                    sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                foreach (var code in info)
                {
                    var row = table.NewRow();
                    row["BatchCode"] = batchCode;
                    row["MobileNumber"] = code.MobileNumber;
                    row["Carno"] = code.Carno;
                    row["PromotionId"] = code.PromotionId;
                    row["Status"] = Status.WAIT;
                    table.Rows.Add(row);
                }
                sbc.WriteToServer(table);
                return true;
            }
        }

        public static List<BaoYangPackagePromotionDetail> SelectPromotionDetailsByBatchCode(SqlConnection conn, string batchCode)
        {
            const string sql = @"SELECT * FROM BaoYang..VipBaoYangPackagePromotionDetail (NOLOCK) WHERE BatchCode=@BatchCode";
            return conn.Query<BaoYangPackagePromotionDetail>(sql, new { BatchCode = batchCode }, commandType: CommandType.Text).ToList();
        }

        public static int UpdateBaoYangPackagePromotionToSuccess(SqlConnection conn, string batchCode, string phone, int promotionId)
        {
            const string sql = @"	UPDATE BaoYang..VipBaoYangPackagePromotionDetail SET PromotionId=@PromotionId,Status=@Status,LastUpdateDateTime=GETDATE() WHERE BatchCode=@BatchCode AND MobileNumber=@MobileNumber ";
            return conn.Execute(sql, new { BatchCode = batchCode, PromotionId = promotionId, MobileNumber = phone, Status = Status.SUCCESS.ToString() }, commandType: CommandType.Text);
        }

        public static int UpdateBaoYangPackagePromotionStatus(SqlConnection conn, string batchCode, string phone, Status status)
        {
            const string sql = @"	UPDATE BaoYang..VipBaoYangPackagePromotionDetail SET Status=@Status,LastUpdateDateTime=GETDATE() WHERE BatchCode=@BatchCode AND MobileNumber=@MobileNumber AND Status <> 'SUCCESS'";
            return conn.Execute(sql, new { BatchCode = batchCode, MobileNumber = phone, Status = status.ToString() }, commandType: CommandType.Text); 
        }

        public static int InsertBaoYangPackagePromotionDetail(SqlConnection conn, string batchCode, string phone, string carNo, int promotionId)
        {
            const string sql = @"	INSERT INTO BaoYang..VipBaoYangPackagePromotionDetail
	                    ( BatchCode ,
	                      MobileNumber ,
	                      Carno ,
	                      PromotionId ,
	                      CreateDateTime ,
	                      LastUpdateDateTime
	                    )
	            VALUES  ( @BatchCode ,
	                      @MobileNumber ,
	                      @Carno ,
	                      @PromotionId ,
	                      GETDATE() ,
	                      GETDATE()
	                    )";
            return conn.Execute(sql, new { MobileNumber = phone, BatchCode = batchCode, Carno = carNo, PromotionId = promotionId }, commandType: CommandType.Text);
        }

        public static List<VipSimpleUser> GetBaoYangPackageConfigUser(SqlConnection conn)
        {
            const string sql = @"     SELECT DISTINCT scu.UserId AS VipUserId  , scu.UserName AS VipUserName
            FROM   BaoYang..VipBaoYangPackageConfig AS bpc ( NOLOCK )
            JOIN Tuhu_profiles..SYS_CompanyUser AS scu ( NOLOCK ) ON bpc.VipUserId = scu.UserId ORDER BY VipUserName ;";
            return conn.Query<VipSimpleUser>(sql, commandType: CommandType.Text).ToList();
        }

        public static List<VipSimpleUser> GetAllBaoYangPackageUser(SqlConnection conn)
        {
            const string sql = @"SELECT  u.UserId AS VipUserId ,
        u.UserName AS VipUserName
FROM    Tuhu_profiles..SYS_CompanyUser AS u WITH ( NOLOCK )
        JOIN Tuhu_profiles..SYS_CompanyInfo AS i WITH ( NOLOCK ) ON u.CompanyId = i.Id
WHERE   u.IsDeleted = 0
        AND i.IsDeleted = 0
        AND i.IsActive = 1
ORDER BY VipUserName;";
            return conn.Query<VipSimpleUser>(sql, commandType: CommandType.Text).ToList();
        }

        public static List<VipBaoYangPackageSmsConfig> SelectVipSmsConfig(SqlConnection conn, Guid vipUserId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT  scu.UserId AS VipUserId ,
        scu.UserName ,
        vps.IsSendSms ,
        COUNT(*) OVER ( ) AS Total
FROM    Tuhu_profiles..SYS_CompanyUser AS scu WITH ( NOLOCK )
        LEFT JOIN BaoYang..VipBaoYangPackageSmsConfig AS vps WITH ( NOLOCK ) ON scu.IsDeleted = 0
                                                              AND vps.VipUserId = scu.UserId
WHERE   ( @VipUserId = '00000000-0000-0000-0000-000000000000'
          OR scu.UserId = @VipUserId
        )
ORDER BY vps.UpdateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            return conn.Query<VipBaoYangPackageSmsConfig>(sql, new
            {
                VipUserId = vipUserId.ToString(),
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public static int IsExistsSmsConfig(SqlConnection conn, Guid vipUserId)
        {
            const string sql = @"  SELECT Count(1) FROM BaoYang..VipBaoYangPackageSmsConfig WITH (NOLOCK) WHERE VipUserId=@VipUserId";
            return (int)conn.ExecuteScalar(sql, new { VipUserId = vipUserId.ToString() }, commandType: CommandType.Text);
        }

        public static bool IsSendSmsByPackageId(SqlConnection conn, int packageId)
        {
            const string sql = @"SELECT vps.IsSendSms FROM BaoYang..VipBaoYangPackageConfig AS vpc WITH (NOLOCK)
	        JOIN BaoYang..VipBaoYangPackageSmsConfig AS vps WITH (NOLOCK) ON vps.VipUserId = vpc.VipUserId
	        WHERE vpc.PKID=@PKID";
            var result= conn.ExecuteScalar(sql, new { PKID = packageId }, commandType: CommandType.Text);
            return result != null ? Convert.ToBoolean(result) : false;
        }

        public static int UpdateSendSmsStatus(SqlConnection conn, Guid vipUserId, bool isSendSms)
        {
            const string sql = @"UPDATE BaoYang..VipBaoYangPackageSmsConfig SET IsSendSms=@IsSendSms,UpdateTime=GETDATE() WHERE VipUserId=@VipUserId";
            return conn.Execute(sql, new { VipUserId = vipUserId, IsSendSms = isSendSms }, commandType: CommandType.Text);
        }

        public static int InsertVipSmsCofig(SqlConnection conn, Guid vipUserId, bool isSendSms, string user)
        {
            const string sql = @"	INSERT BaoYang..VipBaoYangPackageSmsConfig
	        ( VipUserId ,
	          IsSendSms ,
	          CreateUser ,
	          CreateTime ,
	          UpdateTime
	        )
	VALUES  ( @VipUserId ,
	          @IsSendSms ,
	          @CreateUser ,
	          GETDATE() ,
	          GETDATE()  
	        )";
            return conn.Execute(sql, new { VipUserId = vipUserId, IsSendSms = isSendSms, CreateUser = user }, commandType: CommandType.Text);
        }

        public static List<BaoYangPackagePromotionDetail> SelectImportedPromotionDetail(SqlConnection conn,
            Guid rulesGUID, IEnumerable<string> mobileNumbers)
        {
            var sql = @"SELECT  b.MobileNumber ,
        COUNT(1) AS Quantity
FROM    BaoYang..VipBaoYangPackagePromotionRecord AS a WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangPackagePromotionDetail AS b WITH ( NOLOCK ) ON a.BatchCode = b.BatchCode
WHERE   a.RulesGUID = @RulesGUID
        AND EXISTS ( SELECT 1
                     FROM   Gungnir..SplitString(@MobileNumbers, N',', 1)
                     WHERE  Item = b.MobileNumber COLLATE Chinese_PRC_CI_AS )
GROUP BY b.MobileNumber;";
            return conn.Query<BaoYangPackagePromotionDetail>(sql, new
            {
                RulesGUID = rulesGUID,
                MobileNumbers = string.Join(",", mobileNumbers ?? new List<string>())
            }, commandType: CommandType.Text).ToList();
        }



        #region PromotionDetail

        public static BaoYangPackageConfigSimpleInfo SelectPackageConfigSimpleInfo(SqlConnection conn, string batchCode)
        {
            var sql = @"SELECT  re.PackageId ,
        re.BatchCode ,
        sc.UserName AS VipUserName ,
        co.PID ,
        co.PackageName ,
        co.Volume ,
        co.SettlementMethod ,
        co.Price ,
        co.Brands ,
        re.CreateUser ,
        re.IsSendSms ,
        re.RulesGUID 
FROM    BaoYang..VipBaoYangPackageConfig AS co WITH ( NOLOCK )
        LEFT JOIN BaoYang..VipBaoYangPackagePromotionRecord AS re WITH ( NOLOCK ) ON co.PKID = re.PackageId
        LEFT JOIN Tuhu_profiles..SYS_CompanyUser AS sc WITH ( NOLOCK ) ON co.VipUserId = sc.UserId
WHERE   BatchCode = @BatchCode;";
            return conn.Query<BaoYangPackageConfigSimpleInfo>(sql, new
            {
                BatchCode = batchCode,
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public static List<BaoYangPackagePromotionDetailSimpleModel> SelectPromotionDetailsByBatchCode(SqlConnection conn, 
            string batchcode, int index, int size)
        {
            var sql = @"SELECT  t.PKID ,
        t.BatchCode ,
        t.MobileNumber ,
        t.Carno ,
        t.Status ,
        t.Remarks ,
        t.PromotionId ,
        COUNT(t.PKID) OVER ( ) AS Total ,
        t.StartTime ,
		t.EndTime 
FROM    BaoYang..VipBaoYangPackagePromotionDetail AS t WITH ( NOLOCK )
WHERE   t.BatchCode = @BatchCode
ORDER BY CASE WHEN t.Status <> N'SUCCESS' THEN 1
              ELSE 0
         END DESC ,
        t.PKID
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";
            var skip = (index - 1) * size;
            return conn.Query<BaoYangPackagePromotionDetailSimpleModel>(sql, new
            {
                Skip = skip > 0 ? skip : 0,
                Take = size > 0 ? size : 1,
                BatchCode = batchcode,
            }, commandType: CommandType.Text).ToList();
        }

        public static BaoYangPackagePromotionDetailSimpleModel SelectPromotionDetailById(SqlConnection conn,
            long pkid)
        {
            var sql = @"SELECT  t.PKID ,
        t.BatchCode ,
        t.MobileNumber ,
        t.Carno ,
        t.Status ,
        t.Remarks ,
        t.PromotionId
FROM    BaoYang..VipBaoYangPackagePromotionDetail AS t WITH ( NOLOCK )
WHERE   t.PKID = @PKID;";
            return conn.Query<BaoYangPackagePromotionDetailSimpleModel>(sql, new
            {
                PKID = pkid
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public static bool UpdatePromotionDetail(SqlConnection conn,
            long pkid, string mobileNumber)
        {
            var sql = @"UPDATE  BaoYang..VipBaoYangPackagePromotionDetail
SET     MobileNumber = @MobileNumber
WHERE   PKID = @PKID
        AND PromotionId IS NULL
        AND Status <> @Status;";
            return conn.Execute(sql, new
            {
                PKID = pkid,
                Status = "SUCCESS",
                MobileNumber = mobileNumber
            }, commandType: CommandType.Text) > 0;
        }

        public static bool UpdatePromotionRemarks(SqlConnection conn, int promotionId, string remarks)
        {
            var sql = @"UPDATE BaoYang..VipBaoYangPackagePromotionDetail set Remarks = @Remarks where PromotionId = @PromotionId;";
            return conn.Execute(sql, new
            {
                PromotionId = promotionId,
                Remarks = remarks
            }, commandType: CommandType.Text) > 0;
        }


        #endregion

        #region RedemptionCode

        /// <summary>
        /// 获取套餐Id和套餐名称
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Dictionary<int, string> SelectVipBaoYangConfigSimpleInfo(SqlConnection conn)
        {
            var sql = @"SELECT  c.PKID ,
        c.PackageName
FROM    BaoYang..VipBaoYangPackageConfig AS c WITH ( NOLOCK )";
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).Rows
                .Cast<DataRow>().ToDictionary(x => Convert.ToInt32(x["PKID"]), x => x["PackageName"].ToString());
            return result;
        }

        /// <summary>
        /// 新增礼包配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static long AddVipBaoYangGiftPackConfig(SqlConnection conn, VipBaoYangGiftPackConfig config)
        {
            var sql = @"
INSERT  INTO BaoYang..VipBaoYangGiftPackConfig
        ( PackName ,
          PackageId ,
          IsValid ,
          CreateDateTime ,
          LastUpdateDateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @PackName ,
          @PackageId ,
          @IsValid ,
          GETDATE() ,
          GETDATE()
        );";
            var result = Convert.ToInt64(conn.ExecuteScalar(sql, new
            {
                PackName = config.PackName,
                PackageId = config.PackageId,
                IsValid = config.IsValid
            }, commandType: CommandType.Text));
            return result;
        }

        /// <summary>
        /// 修改礼包配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool EditVipBaoYangGiftPackConfig(SqlConnection conn, VipBaoYangGiftPackConfig config)
        {
            var sql = @"
UPDATE  BaoYang..VipBaoYangGiftPackConfig
SET     PackName = @PackName ,
        IsValid = @IsValid ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID;";
            var result = conn.Execute(sql, new
            {
                PackName = config.PackName,
                PackageId = config.PackageId,
                IsValid = config.IsValid,
                PKID = config.PKID
            }, commandType: CommandType.Text) > 0;
            return result;
        }

        /// <summary>
        /// 是否存在礼包名称
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool IsExisitsVipBaoYangGiftPackName(SqlConnection conn, VipBaoYangGiftPackConfig config)
        {
            var sql = @"SELECT  COUNT(1)
FROM    BaoYang..VipBaoYangGiftPackConfig AS c WITH ( NOLOCK )
WHERE   c.PackName = @PackName
        AND c.PKID <> @PackId;";
            var returnvalue = conn.ExecuteScalar(sql, new
            {
                PackName = config.PackName,
                PackId = config.PackId
            }, commandType: CommandType.Text);
            var count = Convert.ToInt32(returnvalue);
            return count > 0;
        }

        /// <summary>
        /// 分页获取配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static VipBaoYangPagerModel<VipBaoYangGiftPackConfig> SelectVipBaoYangGiftPackConfig(SqlConnection conn,
            int index, int size)
        {
            var sql = @"SELECT  @Total = COUNT(1)
FROM    BaoYang..VipBaoYangGiftPackConfig AS c WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangPackageConfig AS cc WITH ( NOLOCK ) ON c.PackageId = cc.PKID;
SELECT  c.PKID ,
        c.PackName ,
        c.PackageId ,
        c.IsValid ,
        cc.PackageName
FROM    BaoYang..VipBaoYangGiftPackConfig AS c WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangPackageConfig AS cc WITH ( NOLOCK ) ON c.PackageId = cc.PKID
ORDER BY c.PKID DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

            var skip = (index - 1) * size;
            skip = skip > 0 ? skip : 0;
            var param = new DynamicParameters();
            param.Add("@Skip", skip);
            param.Add("@Take", size);
            param.Add("@Total", 0, DbType.Int32, ParameterDirection.Output);
            var data = conn.Query<VipBaoYangGiftPackConfig>(sql, param)?.ToList() ?? new List<VipBaoYangGiftPackConfig>();
            int total = param.Get<int>("@Total");
            return new VipBaoYangPagerModel<VipBaoYangGiftPackConfig> { Data = data, Total = total };
        }

        /// <summary>
        /// 根据礼包Id获取优惠券配置
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public static IEnumerable<GiftPackCouponConfig> SelectGiftPackCouponConfig(SqlConnection conn, long packId)
        {
            var sql = @"SELECT  t.PKID ,
        t.PackId ,
        t.GetRuleGUID ,
        t.Quantity
FROM    BaoYang..VipBaoYangGiftPackCouponConfig AS t WITH ( NOLOCK )
WHERE   t.PackId = @PackId;";
            var result = conn.Query<GiftPackCouponConfig>(sql, new { PackId = packId });
            return result;
        }

        /// <summary>
        /// 添加礼包优惠券配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packId"></param>
        /// <param name="getRuleId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static long AddGiftPackCouponConfig(SqlConnection conn, long packId, Guid getRuleGUID, int quantity)
        {
            var sql = @"INSERT  INTO BaoYang..VipBaoYangGiftPackCouponConfig
        ( PackId ,
          GetRuleGUID ,
          Quantity ,
          CreateDateTime ,
          LastUpdateDateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @PackId ,
          @GetRuleGUID ,
          @Quantity ,
          GETDATE() ,
          GETDATE()
        );";
            return Convert.ToInt64(conn.ExecuteScalar(sql, new { PackId = packId, GetRuleGUID = getRuleGUID, Quantity = quantity }));
        }

        /// <summary>
        /// 判断是否存在相同优惠券和礼包
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packId"></param>
        /// <param name="getRuleId"></param>
        /// <returns></returns>
        public static bool IsExistsGiftPackCouponConfig(SqlConnection conn, long packId, Guid getRuleId)
        {
            var sql = @"SELECT  COUNT(1)
FROM    BaoYang..VipBaoYangGiftPackCouponConfig AS t WITH ( NOLOCK )
WHERE   PackId = @PackId
        AND GetRuleGUID = @GetRuleID;";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new { PackId = packId, GetRuleID = getRuleId })) > 0;
        }

        /// <summary>
        /// 根据Id获取礼包配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packId"></param>
        /// <returns></returns>
        public static GiftPackCouponConfig SelectVipBaoYangGiftPackConfigById(SqlConnection conn, long packId)
        {
            var sql = @"SELECT  t.PKID ,
        t.PackName ,
        t.PackageId ,
        t.IsValid
FROM    BaoYang..VipBaoYangGiftPackConfig AS t WITH ( NOLOCK )
WHERE   t.PKID = @PackId;";
            return conn.Query<GiftPackCouponConfig>(sql, new { PackId = packId }).FirstOrDefault();
        }

        /// <summary>
        /// 保存兑换码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static bool InsertBaoYangRedemptionCode(SqlConnection conn, IEnumerable<VipBaoYangRedemptionCode> codes)
        {
            var createDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var table = new DataTable { TableName = "BaoYang.dbo.VipBaoYangRedemptionCode" };
            table.Columns.Add("PackId", typeof(long));
            table.Columns.Add("BatchCode", typeof(string));
            table.Columns.Add("RedemptionCode", typeof(string));
            table.Columns.Add("CreateUser", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("StartTime", typeof(DateTime));
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("CreateDateTime", typeof(DateTime));
            codes.ToList().ForEach(item =>
            {
                var row = table.NewRow();

                row["PackId"] = item.PackId;
                row["BatchCode"] = item.BatchCode;
                row["RedemptionCode"] = item.RedemptionCode;
                row["CreateUser"] = item.CreateUser;
                row["StartTime"] = item.StartTime;
                row["EndTime"] = item.EndTime;
                row["Status"] = item.Status;
                row["CreateDateTime"] = createDateTime;
                table.Rows.Add(row);
            });
            
            using (var sbk = new SqlBulkCopy(conn))
            {
                sbk.DestinationTableName = table.TableName;
                foreach (DataColumn clomun in table.Columns)
                {
                    sbk.ColumnMappings.Add(clomun.ColumnName, clomun.ColumnName);
                }
                sbk.WriteToServer(table);
            }
            return true;
        }

        /// <summary>
        /// 查询礼包下面兑换码简要信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packId"></param>
        /// <returns></returns>
        public static List<VipBaoYangRedemptionCodeSimpleModel> SelectRedemptionCodeSimpleInfo(SqlConnection conn, long packId)
        {
            var sql = @"SELECT  DISTINCT
        t.BatchCode ,
        t.CreateUser ,
        t.CreateDateTime
FROM    BaoYang..VipBaoYangRedemptionCode AS t WITH ( NOLOCK )
WHERE   t.PackId = @PackId;";
            var result = conn.Query<VipBaoYangRedemptionCodeSimpleModel>(sql, new { PackId = packId }, commandType: CommandType.Text);
            return result.ToList();
        }

        /// <summary>
        /// 获取兑换码详情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <param name="packId"></param>
        /// <returns></returns>
        public static List<VipBaoYangRedemptionCode> SelectRedemptionCodeDetails(SqlConnection conn, string batchCode, long packId)
        {
            var sql = @"SELECT  t.PKID ,
        t.PackId ,
        t.RedemptionCode ,
        t.BatchCode ,
        t.CreateUser ,
        t.StartTime ,
        t.EndTime ,
        t.CreateDateTime
FROM    BaoYang..VipBaoYangRedemptionCode AS t WITH ( NOLOCK )
WHERE   t.PackId = @PackId
        AND t.BatchCode = @BatchCode;";
            return conn.Query<VipBaoYangRedemptionCode>(sql, new
            {
                BatchCode = batchCode,
                PackId = packId
            }, commandType: CommandType.Text).ToList();
        }

        /// <summary>
        /// 根据礼包Id获取套餐配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packId"></param>
        /// <returns></returns>
        public static VipBaoYangPackageModel SelectVipBaoYangPackageByPackId(SqlConnection conn, long packId)
        {
            var sql = @"SELECT  c.PID ,
        c.PackageName ,
        c.VipUserId ,
        c.Brands ,
        c.Price ,
        c.Volume ,
        c.CreateUser ,
        c.SettlementMethod ,
        c.SettlementVipUserId 
FROM    BaoYang..VipBaoYangPackageConfig AS c WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangGiftPackConfig AS pc WITH ( NOLOCK ) ON pc.PackageId = c.PKID
WHERE   pc.PKID = @PackId;";
            return conn.Query<VipBaoYangPackageModel>(sql, new { PackId = packId }, commandType: CommandType.Text).FirstOrDefault();
        }

        /// <summary>
        /// 更新订单号
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool UpdateRedemptionCode(SqlConnection conn, string batchCode, int orderId)
        {
            var sql = @"UPDATE  BaoYang..VipBaoYangRedemptionCode
SET     OrderId = @OrderId
WHERE   BatchCode = @BatchCode
        AND ( OrderId IS NULL
              OR OrderId < 0
            );";
            return conn.Execute(sql, new { OrderId = orderId, BatchCode = batchCode }, commandType: CommandType.Text) > 0;
        }

        #endregion

        #region Promotion

        /// <summary>
        /// 获取优惠券数量
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="getRuleId"></param>
        /// <returns></returns>
        public static BaoYangPromotionSimpleInfo SelectPromotionSimpleInfoByGetRuleGuid(SqlConnection conn, Guid getRuleId)
        {
            var sql = @"SELECT  Quantity ,
        GetQuantity ,
        SingleQuantity
FROM    Activity..tbl_GetCouponRules WITH ( NOLOCK )
WHERE   GetRuleGUID = @GetRuleGUID;";
            return conn.QueryFirstOrDefault<BaoYangPromotionSimpleInfo>(sql, new { GetRuleGUID = getRuleId });
        }

        /// <summary>
        /// 根据RuleId获取信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public static List<Tuple<int, Guid, string, string>> SelectCouponGetRules(SqlConnection conn, int ruleId)
        {
            const string sql = @"SELECT  PKID ,
        GetRuleGUID ,
        PromtionName ,
        Description
FROM    Activity..tbl_GetCouponRules WITH ( NOLOCK )
WHERE   RuleID = @RuleID;";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@RuleID", ruleId));
            var result = dt.Rows.Cast<DataRow>().Select(row => Tuple.Create(Convert.ToInt32(row["PKID"]), (Guid)row["GetRuleGUID"], row["PromtionName"].ToString(), row["Description"].ToString())).ToList();
            return result;
        }

        /// <summary>
        /// 获取优惠券
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Dictionary<int, string> SelectCouponRules(SqlConnection conn)
        {
            const string sql = @"SELECT  CR.PKID ,
        CR.Name
FROM    Activity..tbl_CouponRules AS CR WITH ( NOLOCK )
WHERE   CR.ParentID = 0
        AND EXISTS ( SELECT 1
                     FROM   Activity..tbl_GetCouponRules AS GCR WITH ( NOLOCK )
                     WHERE  GCR.RuleID = CR.PKID )
ORDER BY CR.PKID;";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            var result = dt.Rows.Cast<DataRow>().ToDictionary(row => Convert.ToInt32(row["PKID"]), row => row["Name"].ToString());
            return result;
        }

        /// <summary>
        /// 获取优惠券信息根据getRuleIds
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="getRuleGUIDs"></param>
        /// <returns></returns>
        public static IEnumerable<GiftPackCouponConfig> SelectCouponInfos(SqlConnection conn, IEnumerable<Guid> getRuleGUIDs)
        {
            var sql = @"SELECT  GCR.RuleID ,
        GCR.GetRuleGUID ,
        CR.Name ,
        GCR.Description ,
        GCR.PromtionName ,
        GCR.PKID AS GetRuleID ,
        GCR.ValiStartDate ,
        GCR.ValiEndDate ,
        GCR.Term
FROM    Activity..tbl_CouponRules AS CR WITH ( NOLOCK )
        INNER JOIN Activity..tbl_GetCouponRules AS GCR ON CR.PKID = GCR.RuleID
WHERE   CR.ParentID = 0
        AND EXISTS ( SELECT 1
                     FROM   Configuration..SplitString(@GetRuleGUIDs, N',', 1)
                     WHERE  Item = GCR.GetRuleGUID );";
            return conn.Query<GiftPackCouponConfig>(sql, new { GetRuleGUIDs = string.Join(",", getRuleGUIDs ?? new List<Guid>()) });
        }

        #endregion

        #region BaoYangOprLog

        public static List<BaoYangOprLog> SelectExportRedemptionCodeRecordDetails(SqlConnection conn, IEnumerable<string> identityIDs,
            string logType)
        {
            var sql = @"SELECT  t.PKID ,
        t.LogType ,
        t.OldValue ,
        t.NewValue ,
        t.IdentityID ,
        t.OperateUser ,
        t.Remarks ,
        t.CreateTime
FROM    Tuhu_log..BaoYangOprLog AS t WITH ( NOLOCK )
WHERE   t.LogType = @LogType
        AND EXISTS ( SELECT 1
                     FROM   Tuhu_log..SplitString(@IdentityIDs, N',', 1)
                     WHERE  Item = t.IdentityID );";
            var result = conn.Query<BaoYangOprLog>(sql, new { IdentityIDs = string.Join(",", identityIDs ?? new List<string>()), LogType = logType }, commandType: CommandType.Text);
            return result.ToList();
        }

        #endregion

        #region Oil Config

        /// <summary>
        /// 添加机油配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="oilConfig"></param>
        /// <returns></returns>
        public static long InsertBaoYangPackageOilConfig(SqlConnection conn, VipBaoYangPackageOilConfig oilConfig)
        {
            const string sql = @"INSERT  INTO BaoYang.dbo.VipBaoYangPackageOilConfig
        ( PackageId ,
          Brand ,
          Grade ,
          Series ,
          CreateDateTime ,
          LastUpdateDateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @PackageId ,
          @Brand ,
          @Grade ,
          @Series ,
          GETDATE() ,
          GETDATE()
        )";
            var result = conn.ExecuteScalar(sql, new
            {
                oilConfig.PackageId,
                oilConfig.Brand,
                oilConfig.Grade,
                oilConfig.Series,
            }, commandType: CommandType.Text);
            return Convert.ToInt64(result);
        }

        /// <summary>
        /// 同步数据检查数据是否已经存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static bool IsExistsBaoYangPackageOilConfigForSync(SqlConnection conn, int packageId)
        {
            const string sql = @"SELECT  COUNT(1)
FROM    BaoYang.dbo.VipBaoYangPackageOilConfig
WHERE   PackageId = @PackageId";
            var result = conn.ExecuteScalar(sql, new
            {
                PackageId = packageId,
            }, commandType: CommandType.Text);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// 根据套餐ID查询机油配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static IEnumerable<VipBaoYangPackageOilConfig> SelectBaoYangPackageOilConfigs(SqlConnection conn, int packageId)
        {
            const string sql = @"SELECT  t.PKID ,
        t.PackageId ,
        t.Brand ,
        t.Grade ,
        t.Series ,
        t.CreateDateTime ,
        t.LastUpdateDateTime
FROM    BaoYang.dbo.VipBaoYangPackageOilConfig AS t WITH ( NOLOCK )
WHERE   t.PackageId = @PackageId";
            var result = conn.Query<VipBaoYangPackageOilConfig>(sql, new
            {
                PackageId = packageId,
            }, commandType: CommandType.Text);
            return result;
        }

        /// <summary>
        /// 批量查询套餐配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageIds"></param>
        /// <returns>
        ///     Key:套餐ID
        ///     Value:套餐机油配置
        /// </returns>
        public static IDictionary<int, List<VipBaoYangPackageOilConfig>> SelectBaoYangPackageOilConfigs(SqlConnection conn, IEnumerable<int> packageIds)
        {
            const string sql = @"SELECT  t.PKID ,
        t.PackageId ,
        t.Brand ,
        t.Grade ,
        t.Series ,
        t.CreateDateTime ,
        t.LastUpdateDateTime
FROM    BaoYang.dbo.VipBaoYangPackageOilConfig AS t WITH ( NOLOCK )
WHERE   EXISTS ( SELECT 1
                 FROM   BaoYang.dbo.SplitString(@PackageIds, N',', 1)
                 WHERE  Item = t.PackageId )";
            var result = conn.Query<VipBaoYangPackageOilConfig>(sql, new
            {
                PackageIds = string.Join(",", packageIds ?? new List<int>()),
            }, commandType: CommandType.Text);
            var dict = result.GroupBy(x => x.PackageId).ToDictionary(g => g.Key, g => g.ToList());
            return dict;
        }

        /// <summary>
        /// 删除保养套餐机油配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public static bool DeleteBaoYangPackageOilConfigs(SqlConnection conn, int packageId)
        {
            const string sql = @"DELETE  FROM BaoYang.dbo.VipBaoYangPackageOilConfig
WHERE   PackageId = @PackageId";
            return conn.Execute(sql, new
            {
                PackageId = packageId,
            }, commandType: CommandType.Text) > 0;
        }

        #endregion

        #region Sync Data

        public static Dictionary<int, string> SelectBaoYangPackageOilOldConfig(SqlConnection conn)
        {
            const string sql = @"SELECT  t.PKID ,
        t.Brands
FROM    BaoYang.dbo.VipBaoYangPackageConfig AS t WITH ( NOLOCK )";
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            return result.Rows.OfType<DataRow>().ToDictionary(row => Convert.ToInt32(row["PKID"]), row => row["Brands"].ToString());
        }

        #endregion
    }
}
