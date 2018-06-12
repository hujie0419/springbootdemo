using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALNuomiService
    {
        public static List<NuomiServicesConfig> SelectNuomiServicesConfig(SqlConnection conn, string nuomiTitle, long nuomiId, string serviceId, int isValid, int pageIndex, int pageSize)
        {
            return conn.Query<NuomiServicesConfig>(@" SELECT config.* ,
                    COUNT(*) OVER() AS Total
             FROM   Configuration..NuomiServicesConfig AS config WITH(NOLOCK)
             WHERE(@NuomiTitle = ''
                      OR config.NuomiTitle LIKE '%' + @NuomiTitle + '%'
                    )
                    AND(@NuomiId = 0
                          OR config.NuomiId = @NuomiId
                        )
                    AND(@ServiceId = ''
                          OR config.ServiceId = @ServiceId
                        )
                    AND(@IsValid = -1
                          OR config.IsValid = @IsValid
                        )
             ORDER BY config.PKID DESC
                    OFFSET(@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                    ONLY",
                new
                {
                    NuomiTitle = nuomiTitle,
                    NuomiId = nuomiId,
                    ServiceId = serviceId,
                    IsValid = isValid,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                },
                commandType: CommandType.Text).ToList();
        }

        public static int InsertNuomiServiceConfig(SqlConnection conn, string nuomiTitle, long nuomiId, string serviceId, string userEmail, string remarks)
        {
            return conn.Execute(@" 	INSERT  INTO Configuration..NuomiServicesConfig
                    (NuomiId,
                      NuomiTitle,
                      ServiceId,
                      Email,
                      Remarks,
                      IsValid,
                      CreatedTime
                    )

            VALUES(
                      @NuomiId,
                      @NuomiTitle,
                      @ServiceId,
                      @UserEmail,
                      @Remarks,
                      1,
                      GETDATE()
                    )",
            new
            {
                NuomiTitle = nuomiTitle,
                NuomiId = nuomiId,
                ServiceId = serviceId,
                UserEmail = userEmail,
                Remarks = remarks
            },
            commandType: CommandType.Text);
        }

        public static int UpdateNuomiServiceConfig(SqlConnection conn, int pkid, string nuomiTitle, string serviceId, string remarks)
        {
            return conn.Execute(@"    UPDATE    Configuration..NuomiServicesConfig
                  SET       NuomiTitle = @NuomiTitle ,
                            ServiceId = @ServiceId ,
                            Remarks = @Remarks ,
			                UpdatedTime = GETDATE()
                  WHERE     PKID = @PKID",
            new
            {
                PKID = pkid,
                NuomiTitle = nuomiTitle,
                ServiceId = serviceId,
                Remarks = remarks
            },
            commandType: CommandType.Text);
        }

        public static int DelNuomiServiceConfig(SqlConnection conn, int pkid)
        {
            return conn.Execute(@" DELETE FROM Configuration..NuomiServicesConfig WHERE PKID=@PKID",
            new
            {
                PKID = pkid
            },
            commandType: CommandType.Text);
        }

        public static NuomiServicesConfig SelectConfigByPKID(SqlConnection conn, int pkid)
        {
            return conn.Query<NuomiServicesConfig>("SELECT * FROM Configuration..NuomiServicesConfig WITH ( NOLOCK ) WHERE PKID=@PKID",
                new
                {
                    PKID = pkid
                },
                commandType: CommandType.Text).SingleOrDefault();
        }

        public static NuomiServicesConfig SelectConfigByNuomiIdAndEmail(SqlConnection conn, long nuomiId, string email)
        {
            return conn.Query<NuomiServicesConfig>("SELECT * FROM Configuration..NuomiServicesConfig WITH ( NOLOCK ) WHERE NuomiId=@NuomiId AND Email=@Email",
            new
            {
                NuomiId = nuomiId,
                Email = email
            },
            commandType: CommandType.Text).SingleOrDefault();
        }

        public static NuomiServicesConfig SelectConfigByNuomiId(SqlConnection conn, long nuomiId)
        {
            return conn.Query<NuomiServicesConfig>("SELECT * FROM Configuration..NuomiServicesConfig WITH ( NOLOCK ) WHERE NuomiId=@NuomiId",
            new
            {
                NuomiId = nuomiId
            },
            commandType: CommandType.Text).SingleOrDefault();
        }


        public static void BatchInsertNuomiConfig(SqlConnection conn, List<NuomiServicesConfig> info, string email)
        {
            using (var sbc = new SqlBulkCopy(conn))
            {
                sbc.BatchSize = 1000;
                sbc.BulkCopyTimeout = 0;
                //将DataTable表名作为待导入库中的目标表名
                sbc.DestinationTableName = "Configuration..NuomiServicesConfig";
                //将数据集合和目标服务器库表中的字段对应
                DataTable table = new DataTable();
                table.Columns.Add("NuomiId");
                table.Columns.Add("NuomiTitle");
                table.Columns.Add("ServiceId");
                table.Columns.Add("Email");
                table.Columns.Add("Remarks");
                table.Columns.Add("IsValid");
                foreach (DataColumn col in table.Columns)
                {
                    //列映射定义数据源中的列和目标表中的列之间的关系
                    sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                foreach (var code in info)
                {
                    var row = table.NewRow();
                    row["NuomiId"] = code.NuomiId;
                    row["NuomiTitle"] = code.NuomiTitle;
                    row["ServiceId"] = code.ServiceId;
                    row["Email"] = email;
                    row["Remarks"] = code.Remarks;
                    row["IsValid"] = true;
                    table.Rows.Add(row);
                }
                sbc.WriteToServer(table);
            }
        }

        public static int BatchSoleteConfig(SqlConnection conn, string pkidStr)
        {
            return conn.Execute(@"WITH    pkidlist
                  AS(SELECT *
                       FROM     dbo.SplitString(@PKIDStr, ',', 1)
                     )
            UPDATE  Configuration..NuomiServicesConfig
            SET     IsValid = 0,
                    UpdatedTime = GETDATE()
            WHERE   EXISTS(SELECT 1
                             FROM   pkidlist AS pl WITH(NOLOCK)
                             WHERE  pl.Item = PKID)", new { PKIDStr = pkidStr }, commandType: CommandType.Text);
        }
    }
}
