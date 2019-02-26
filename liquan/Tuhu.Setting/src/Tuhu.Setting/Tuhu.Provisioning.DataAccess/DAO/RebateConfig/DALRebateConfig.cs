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
using Tuhu.Provisioning.DataAccess.Entity.RebateConfig;

namespace Tuhu.Provisioning.DataAccess.DAO.RebateConfig
{
    public static class DALRebateConfig
    {
        public static List<RebateConfigModel> GetRebateConfig(SqlConnection conn, Status status, string orderId, string phone,
            string wxId, string remarks, string timeType, DateTime? startTime, DateTime? endTime, string wxName,
            string principalPerson, string rebateMoney, string source, int? installShopId, int pageIndex, int pageSize)
        {
            const string sql = @"
            SELECT  rac.PKID ,
            rac.OrderId ,
            rac.UserPhone ,
            rac.Status ,
            rac.WXId ,
            rac.WXName ,
            rac.QRCodeImg ,
            rac.UserName ,
            rac.CarNumber ,
            rac.Source ,
            rac.ContentUrl ,
            rac.BaiDuId ,
            rac.BaiDuName ,
            rac.PrincipalPerson ,
            rac.RebateMoney ,
            rac.RebateTime ,
            rac.Remarks ,
            rac.CreateTime ,
            rac.UpdateTime ,
            rac.RefusalReason ,
            rac.InstallShopId ,
            rac.UnionId ,
            rac.OpenId ,
            rac.CheckTime ,
            COUNT(1) OVER ( ) AS Total
    FROM    Activity..RebateApplyConfig AS rac WITH ( NOLOCK )
    WHERE   IsDelete = 0
            AND ( ( @Source = N'Rebate25'
                    AND rac.Source = N'Rebate25'
                  )
                  OR ( @Source = N'Rebate58'
                       AND ( rac.Source = N'爱卡'
                             OR rac.Source = N'汽车之家'
                           )
                     )
                )
            AND ( @Status = 'None'
                  OR @Status IS NULL
                  OR rac.Status = @Status
                )
            AND ( @OrderId IS NULL
                  OR @OrderId = ''
                  OR rac.OrderId = @OrderId
                )
            AND ( @WXId IS NULL
                  OR @WXId = ''
                  OR rac.WXId = @WXId
                )
            AND ( @UserPhone IS NULL
                  OR @UserPhone = ''
                  OR rac.UserPhone = @UserPhone
                )
            AND ( @Remarks IS NULL
                  OR @Remarks = ''
                  OR rac.Remarks LIKE N'%' + @Remarks + '%'
                )
            AND ( @TimeType = 'None'
                  OR ( @TimeType = 'CreateTime'
                       AND rac.CreateTime >= @StartTime
                       AND rac.CreateTime <= @EndTime
                     )
                  OR ( @TimeType = 'CheckTime'
                       AND rac.CheckTime >= @StartTime
                       AND rac.CheckTime <= @EndTime
                     )
                  OR ( @TimeType = 'RebateTime'
                       AND rac.RebateTime >= @StartTime
                       AND rac.RebateTime <= @EndTime
                     )
                )
            AND ( @WxName IS NULL
                  OR @WxName = ''
                  OR rac.WxName LIKE N'%' + @WxName + '%'
                )
            AND ( @PrincipalPerson IS NULL
                  OR @PrincipalPerson = ''
                  OR rac.PrincipalPerson LIKE N'%' + @PrincipalPerson + '%'
                )
            AND ( @InstallShopId IS NULL
                  OR @InstallShopId = 0
                  OR rac.InstallShopId = @InstallShopId
                )
            AND ( @RebateMoney IS NULL
                  OR @RebateMoney = 0
                  OR rac.RebateMoney = @RebateMoney
                )
    ORDER BY rac.PKID DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
            ONLY;";
            return conn.Query<RebateConfigModel>(sql, new
            {
                Status = status.ToString(),
                OrderId = orderId.ToLower().Replace("th", ""),
                WXId = wxId,
                UserPhone = phone,
                Remarks = remarks,
                WxName = wxName,
                TimeType = timeType,
                StartTime = startTime,
                EndTime = endTime != null ? endTime.Value.AddDays(1).AddSeconds(-1) : endTime,
                PrincipalPerson = principalPerson,
                RebateMoney = !string.IsNullOrEmpty(rebateMoney) ? decimal.Parse(rebateMoney) : decimal.Parse("0.0"),
                Source = source,
                InstallShopId = installShopId ?? 0,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public static int InsertRebateImgConfig(SqlConnection conn, int parentId, string imgUrl, ImgSource source, string remarks)
        {
            const string sql = @"
            INSERT  INTO Activity..RebateApplyImageConfig
                    ( ParentId ,
                      ImgUrl ,
                      Source ,
                      Remarks ,
                      CreateTime ,
                      UpdateTime
                    )
            VALUES  ( @ParentId ,
                      @ImgUrl ,
                      @Source ,
                      @Remarks ,
                      GETDATE() ,
                      GETDATE()
                    );";
            return conn.Execute(sql, new
            {
                ParentId = parentId,
                ImgUrl = imgUrl,
                Source = source.ToString(),
                Remarks = remarks
            }, commandType: CommandType.Text);
        }

        public static RebateConfigModel SelectRebateConfigByPKID(SqlConnection conn, int pkid)
        {
            const string sql = @"
            SELECT  rac.PKID ,
                        rac.OrderId ,
                        rac.UserPhone ,
                        rac.Status ,
                        rac.WXId ,
                        rac.WXName ,
                        rac.QRCodeImg ,
                        rac.UserName ,
                        rac.CarNumber ,
                        rac.Source ,
                        rac.ContentUrl ,
                        rac.BaiDuId ,
                        rac.BaiDuName ,
                        rac.PrincipalPerson ,
                        rac.RebateMoney ,
                        rac.RebateTime ,
                        rac.Remarks ,
                        rac.Source ,
                        rac.CreateTime ,
                        rac.RefusalReason ,
                        rac.UnionId ,
                        rac.OpenId ,
                        rac.CheckTime ,
                        rac.UpdateTime 
                FROM    Activity..RebateApplyConfig AS rac WITH ( NOLOCK )
                WHERE   rac.PKID = @PKID";
            return conn.Query<RebateConfigModel>(sql, new
            {
                PKID = pkid
            }, commandType: CommandType.Text).SingleOrDefault();
        }

        public static List<RebateApplyImageConfig> GetRebateApplyImageConfig(SqlConnection conn, int parentId, ImgSource source)
        {
            const string sql = @"
           SELECT raic.ParentId ,
                raic.ImgUrl ,
                raic.Source ,
                raic.Remarks
         FROM   Activity..RebateApplyImageConfig AS raic WITH ( NOLOCK )
         WHERE  raic.ParentId = @ParentId
                AND ( ( @Source = N'UserImg'
                        AND ( raic.Source IS NULL
                              OR raic.Source = @Source
                            )
                      )
                      OR ( @Source = N'PageImg'
                           AND raic.Source = @Source
                         )
                    )";
            return conn.Query<RebateApplyImageConfig>(sql, new
            {
                ParentId = parentId,
                Source = source.ToString()
            }, commandType: CommandType.Text).ToList();
        }

        public static List<RebateConfigModel> SelectRebateApplyConfigByParam(SqlConnection conn, RebateConfigModel data)
        {
            const string sql = @"
                SELECT  rac.OrderId ,
                        rac.UserPhone ,
                        rac.PKID ,
                        rac.Status ,
                        rac.WXId ,
                        rac.WXName ,
                        rac.BaiDuId ,
                        rac.RebateMoney ,
                        rac.PrincipalPerson ,
                        rac.Remarks ,
                        rac.BaiDuName ,
                        rac.Source 
                FROM    Activity..RebateApplyConfig AS rac WITH ( NOLOCK )
                WHERE   rac.IsDelete = 0
                        AND rac.Status IN ( 'Applying', 'Approved', 'Complete' )
                        AND PKID <> @PKID 
                        AND ( rac.OrderId = @OrderId
                              OR rac.UserPhone = @UserPhone
                            );";
            return conn.Query<RebateConfigModel>(sql, new
            {
                PKID = data.PKID,
                OrderId = data.OrderId,
                UserPhone = data.UserPhone
            }, commandType: CommandType.Text).ToList();
        }

        public static int UpdateRemarks(SqlConnection conn,int pkid, string remarks)
        {
            const string sql = @"
            UPDATE  Activity..RebateApplyConfig
            SET     Remarks = @Remarks ,
                    UpdateTime = GETDATE()
            WHERE   PKID = @PKID;";
            return conn.Execute(sql, new
            {
                PKID = pkid,
                Remarks = remarks
            }, commandType: CommandType.Text);
        }

        public static int UpdateStatusForComplete(SqlConnection conn, int pkid)
        {
            const string sql = @"    
            UPDATE  Activity..RebateApplyConfig
            SET     Status = 'Complete' ,
                    UpdateTime = GETDATE(),
                    RebateTime = GETDATE()
            WHERE   PKID = @PKID;";
            return conn.Execute(sql, new { PKID = pkid }, commandType: CommandType.Text);
        }

        public static int UpdateStatusForCompleteV2(SqlConnection conn, int pkid)
        {
            const string sql = @"    
            UPDATE  Activity..RebateApplyConfig
            SET     Status = 'Complete' ,
                    UpdateTime = GETDATE(),
                    CheckTime = GETDATE(),
                    RebateTime = GETDATE()
            WHERE   PKID = @PKID;";
            return conn.Execute(sql, new { PKID = pkid }, commandType: CommandType.Text);
        }

        public static int UpdateStatus(SqlConnection conn, int pkid, Status status, string refusalReason = "")
        {
            const string sql = @"    
            UPDATE  Activity..RebateApplyConfig
            SET     Status = @Status ,
                    RefusalReason = @RefusalReason,
                    CheckTime = GETDATE(),
                    UpdateTime = GETDATE()
            WHERE   PKID = @PKID;";
            return conn.Execute(sql, new
            {
                PKID = pkid,
                Status = status.ToString(),
                RefusalReason = refusalReason
            }, commandType: CommandType.Text);
        }

        public static int DeleteRebateApplyConfig(SqlConnection conn, List<string> pkids, bool isDelete)
        {
            const string sql = @"    
            WITH    pkidlist
                          AS ( SELECT   *
                               FROM     SystemLog..SplitString(@PKIDStr, ',',
                                                              1)
                             )
            UPDATE  Activity..RebateApplyConfig
            SET     IsDelete = @IsDelete
            WHERE   EXISTS ( SELECT 1
                                FROM   pkidlist AS pl
                                WHERE  PKID = pl.Item );";
            return conn.Execute(sql, new
            {
                PKIDStr = String.Join(",", pkids),
                IsDelete = isDelete
            }, commandType: CommandType.Text);
        }

        public static List<RebateConfigLog> SearchRebateConfigLog(string pkid)
        {
            const string sql = @"
            SELECT  rcl.PKID ,
                    rcl.IdentityId ,
                    rcl.Type ,
                    rcl.Msg ,
                    rcl.Remark ,
                    rcl.IPAddress ,
                    rcl.HostName ,
                    rcl.OperateUser ,
                    rcl.CreateDateTime ,
                    rcl.UpdateDateTime
            FROM    Tuhu_log..RebateConfigLog AS rcl WITH ( NOLOCK )
            WHERE   rcl.IdentityId = @IdentityId
            Order BY rcl.PKID DESC; ";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@IdentityId", pkid);
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<RebateConfigLog>().ToList();
            }
        }

        public static List<RebateApplyPageConfig> GetRebateApplyPageConfig(SqlConnection conn, int pageIndex, int pageSize)
        {
            const string sql = @"
            SELECT  PKID ,
                    BackgroundImg ,
                    ActivityRules ,
                    RebateSuccessMsg ,
                    RedBagRemark ,
                    CreateTime ,
                    CreateUser ,
                    UpdateTime ,
                    COUNT(1) OVER ( ) AS Total
            FROM    Activity..RebateApplyPageConfig WITH ( NOLOCK )
            ORDER BY PKID DESC
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
                    ROWS ONLY";
            return conn.Query<RebateApplyPageConfig>(sql, new
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public static int InsertRebateApplyPageConfig(SqlConnection conn, RebateApplyPageConfig config, string user)
        {
            const string sql = @"
            INSERT  INTO Activity..RebateApplyPageConfig
                        (   BackgroundImg ,
                            CreateUser ,
                            ActivityRules ,
                            RebateSuccessMsg ,
                            RedBagRemark ,
                            CreateTime ,
                            UpdateTime
					    )
                OUTPUT  Inserted.PKID
                VALUES  (   @BackgroundImg ,
                            @CreateUser ,
                            @ActivityRules ,
                            @RebateSuccessMsg ,
                            @RedBagRemark ,
                            GETDATE() ,
                            GETDATE()
                        )";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new
            {
                BackgroundImg = config.BackgroundImg,
                ActivityRules = config.ActivityRules,
                CreateUser = user,
                RebateSuccessMsg = config.RebateSuccessMsg,
                RedBagRemark = config.RedBagRemark
            }, commandType: CommandType.Text));
        }

        public static int InsertRebateConfig(SqlConnection conn, RebateConfigModel data)
        {
            const string sql = @"
            INSERT  INTO Activity..RebateApplyConfig
                    ( OrderId ,
                      UserPhone ,
                      Status ,
                      WXId ,
                      WXName ,
                      QRCodeImg ,
                      BaiDuId ,
                      BaiDuName ,
                      PrincipalPerson ,
                      RebateMoney ,
                      RebateTime ,
                      InstallShopId ,
                      Remarks ,
                      UserName ,
                      CarNumber ,
                      Source ,
                      ContentUrl ,
                      CreateTime ,
                      UpdateTime
                    )
            OUTPUT  Inserted.PKID
            VALUES  ( @OrderId ,
                      @UserPhone ,
                      @Status ,
                      @WXId ,
                      @WXName ,
                      @QRCodeImg ,
                      @BaiDuId ,
                      @BaiDuName ,
                      @PrincipalPerson ,
                      @RebateMoney ,
                      @RebateTime ,
                      @InstallShopId ,
                      @Remarks ,
                      @UserName ,
                      @CarNumber ,
                      @Source ,
                      @ContentUrl ,
                      GETDATE() ,
                      GETDATE()
                    );";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new
            {
                OrderId = data.OrderId,
                UserPhone = data.UserPhone,
                WXId = data.WXId,
                WXName = data.WXName,
                QRCodeImg = data.QRCodeImg,
                BaiDuId = data.BaiDuId,
                Status = data.Status.ToString(),
                BaiDuName = data.BaiDuName,
                PrincipalPerson = data.PrincipalPerson,
                RebateMoney = data.RebateMoney,
                RebateTime = data.RebateTime,
                Remarks = data.Remarks,
                UserName = data.UserName,
                CarNumber = data.CarNumber,
                Source = data.Source,
                ContentUrl = data.ContentUrl,
                InstallShopId = data.InstallShopId
            }, commandType: CommandType.Text));
        }
        /// <summary>
        /// 获取没有安装门店的申请记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndx"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<RebateConfigModel> GetNotInstallShopIdRebateRecrd(SqlConnection conn, int pageIndx, int pageSize)
        {
            const string sql = @"
            SELECT  PKID ,
                    OrderId
            FROM    Activity..RebateApplyConfig WITH ( NOLOCK )
            WHERE   InstallShopId IS NULL
                    OR InstallShopId = 0
            ORDER BY PKID DESC
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                    ONLY;";
            return conn.Query<RebateConfigModel>(sql, new
            {
                PageIndex = pageIndx,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }
        /// <summary>
        /// 更新安装门店
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="installShopId"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int UpdateInstallShopIdByPKID(SqlConnection conn, int installShopId, int pkid)
        {
            const string sql = @"UPDATE Activity..RebateApplyConfig SET InstallShopId=@InstallShopId WHERE PKID=@PKID";
            return conn.Execute(sql, new
            {
                InstallShopId = installShopId,
                PKID = pkid
            }, commandType: CommandType.Text);
        }
    }
}
