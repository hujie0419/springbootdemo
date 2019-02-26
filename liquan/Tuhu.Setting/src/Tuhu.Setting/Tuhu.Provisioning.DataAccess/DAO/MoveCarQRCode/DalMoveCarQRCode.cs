using Dapper;
using Microsoft.ApplicationBlocks.Data;
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
using Tuhu.Provisioning.DataAccess.Entity.MoveCarQRCode;

namespace Tuhu.Provisioning.DataAccess.DAO.MoveCarQRCode
{
    public class DalMoveCarQRCode
    {

        #region 新增生成

        /// <summary>
        /// 生成途虎挪车二维码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddMoveCarQRCode(SqlConnection conn,MoveCarQRCodeModel model)
        {
            #region SQL 
            const string sql = @"
                                INSERT  INTO Tuhu_profiles.[dbo].[MoveCarQRCode]
                                        ( [QRCodeUrl] ,
                                          [QRCodeID] ,
                                          [QRCodeImageUrl] ,
                                          [IsDownload] ,
                                          [IsBinding] ,
                                          [BatchID] ,
                                          [CreateDatetime] ,
                                          [LastUpdateDateTime] ,
                                          [CreateBy] ,
                                          [LastUpdateBy]
                                        )
                                VALUES  ( @QRCodeUrl ,
                                          @QRCodeID ,
                                          @QRCodeImageUrl ,
                                          @IsDownload ,
                                          @IsBinding ,
                                          @BatchID ,
                                          GETDATE() ,
                                          GETDATE() ,
                                          @CreateBy ,
                                          @LastUpdateBy
                                        );
                                SELECT  SCOPE_IDENTITY();";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@QRCodeUrl", model.QRCodeUrl??""),
                new SqlParameter("@QRCodeID", model.QRCodeID),
                new SqlParameter("@QRCodeImageUrl", model.QRCodeImageUrl??""),
                new SqlParameter("@IsDownload", model.IsDownload),
                new SqlParameter("@IsBinding", model.IsBinding),
                new SqlParameter("@BatchID", model.BatchID),
                new SqlParameter("@CreateBy", model.CreateBy??""),
                new SqlParameter("@LastUpdateBy", model.LastUpdateBy??"")
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 批量插入途虎挪车二维码
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public bool BulkSaveMoveCarQRCode(DataTable tb)
        {
            SqlConnection conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir"));
            conn.Open();
            conn.ChangeDatabase("Tuhu_profiles");
            using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
            {
                bulk.BatchSize = tb.Rows.Count;
                bulk.DestinationTableName = "MoveCarQRCode";
                bulk.WriteToServer(tb);
            }
            conn.Close();
            return true;
        }

        /// <summary>
        /// 添加途虎挪车二维码生成记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddMoveCarGenerationRecords(SqlConnection conn, MoveCarGenerationRecordsModel model)
        {
            #region SQL 
            const string sql = @"
                                INSERT  INTO [Tuhu_profiles].[dbo].[MoveCarGenerationRecords]
                                        ( [GeneratedNum] ,
                                          [GeneratingStatus] ,
                                          [CreateDatetime] ,
                                          [LastUpdateDateTime] ,
                                          [CreateBy] ,
                                          [LastUpdateBy]
                                        )
                                VALUES  ( @GeneratedNum ,
                                          @GeneratingStatus ,
                                          GETDATE() ,
                                          GETDATE() ,
                                          @CreateBy ,
                                          @LastUpdateBy
                                        ); 
                                SELECT  SCOPE_IDENTITY()";
            #endregion

            var parameters = new[]
            {
                new SqlParameter("@GeneratedNum", model.GeneratedNum),
                new SqlParameter("@GeneratingStatus", model.GeneratingStatus),
                new SqlParameter("@CreateBy", model.CreateBy??""),
                new SqlParameter("@LastUpdateBy", model.LastUpdateBy??""),
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更改生成记录的生成状态为已生成
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool UpdateGenerationRecordsStatus(SqlConnection conn, int pkid)
        {
            #region SQL
            const string sql = @"
                                UPDATE  [Tuhu_profiles].[dbo].[MoveCarGenerationRecords] WITH ( ROWLOCK )
                                SET     GeneratingStatus = 1 ,
                                        LastUpdateDateTime = GETDATE() 
                                WHERE   PKID = @PKID;";
            #endregion
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return dbhelper.ExecuteNonQuery(cmd)>0;
            }
        }

        /// <summary>
        /// 获取总生成下载记录
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public MoveCarTotalRecordsModel GetMoveCarTotalRecord(SqlConnection conn)
        {
            string sql = @"
                        SELECT  [PKID] ,
                                [GeneratedNum] ,
                                [DownloadedNum] ,
                                [DownloadableNum] ,
                                [CreateDatetime] ,
                                [LastUpdateDateTime] 
                        FROM    [Tuhu_profiles].[dbo].[MoveCarTotalRecords] WITH ( NOLOCK );";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<MoveCarTotalRecordsModel>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 添加或修改途虎挪车二维码总生成下载记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOrUpdateMoveCarTotalRecord(SqlConnection conn, MoveCarTotalRecordsModel model,int totalRecordCount)
        {
            #region SQL
            string sqlInsert = @"
                                INSERT  INTO [Tuhu_profiles].[dbo].[MoveCarTotalRecords]
                                        ( [GeneratedNum] ,
                                          [DownloadedNum] ,
                                          [DownloadableNum] ,
                                          [CreateDatetime] ,
                                          [LastUpdateDateTime] 
                                        )
                                VALUES  ( @GeneratedNum ,
                                          @DownloadedNum ,
                                          @DownloadableNum ,
                                          GETDATE() ,
                                          GETDATE()
                                        );";
            string sqlUpdate1 = @"
                                UPDATE  [Tuhu_profiles].[dbo].[MoveCarTotalRecords] WITH ( ROWLOCK )
                                SET     GeneratedNum = GeneratedNum + @GeneratedNum ,
                                        DownloadedNum = DownloadedNum + @DownloadedNum ,
                                        LastUpdateDateTime = GETDATE() ;";
            string sqlUpdate2 = @"
                                UPDATE  [Tuhu_profiles].[dbo].[MoveCarTotalRecords] WITH ( ROWLOCK )
                                SET     DownloadableNum = GeneratedNum - DownloadedNum";
            #endregion
            var parameters = new[]
                {
                    new SqlParameter("@GeneratedNum", model.GeneratedNum),
                    new SqlParameter("@DownloadedNum", model.DownloadedNum),
                    new SqlParameter("@DownloadableNum", model.DownloadableNum)
                };
            if (totalRecordCount == 0)
            {
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlInsert, parameters)) > 0;
            }
            else
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlUpdate1, parameters);
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlUpdate2, parameters)>0;
            }
        }

        #endregion

        #region 新增下载

        /// <summary>
        /// 获取途虎挪车二维码列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="downloadNum"></param>
        /// <returns></returns>
        public List<MoveCarQRCodeModel> GetMoveCarQRCodeList(SqlConnection conn,int downloadNum)
        {
            string sql = @"
                        SELECT TOP {0}
                                [PKID] ,
                                [UserID] ,
                                [QRCodeUrl] ,
                                [QRCodeID] ,
                                [QRCodeImageUrl] ,
                                [Phone] ,
                                [LicensePlateNumber] ,
                                [OpenID] ,
                                [IsDownload] ,
                                [IsBinding] ,
                                [BatchID] ,
                                [CreateDatetime] ,
                                [LastUpdateDateTime] ,
                                [CreateBy] ,
                                [LastUpdateBy]
                        FROM    Tuhu_profiles.dbo.MoveCarQRCode WITH ( NOLOCK )
                        WHERE   IsDownload = 0  ORDER BY CreateDatetime;";
            sql = string.Format(sql, downloadNum);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<MoveCarQRCodeModel>().ToList();
        }

        /// <summary>
        /// 更新途虎挪车二维码表的下载flag为true
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="downloadNum"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public bool UpdateMoveCarQRCodeDownloadFlag(SqlConnection conn, int downloadNum,string lastUpdateBy)
        {
            string sql = @"
                        UPDATE TOP ({0})
                                [Tuhu_profiles].[dbo].MoveCarQRCode WITH ( ROWLOCK )
                        SET     IsDownload = 1,
                                LastUpdateDateTime = GETDATE() ,
                                LastUpdateBy = @LastUpdateBy
                        WHERE   IsDownload = 0;";
            sql= string.Format(sql, downloadNum);
            var parameters = new[]
            {
                new SqlParameter("@LastUpdateBy", lastUpdateBy??"")
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 更新途虎挪车二维码表的下载flag为true 并获取更新flag的列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="downloadNum"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public List<MoveCarQRCodeModel> UpdateDownloadFlagAndSelectMoveCarQRCode(SqlConnection conn, int downloadNum,string lastUpdateBy)
        {
            string sql = @"
                            UPDATE TOP ( {0} )
                                    [Tuhu_profiles].[dbo].MoveCarQRCode WITH ( ROWLOCK )
                            SET     IsDownload = 1 ,
                                    LastUpdateDateTime = GETDATE() ,
                                    LastUpdateBy = @LastUpdateBy
                            output  Inserted.PKID ,
                                    Inserted.IsBinding ,
                                    Inserted.QRCodeImageUrl ,
                                    Inserted.CreateDatetime ,
                                    Inserted.IsDownload
                            WHERE   IsDownload = 0;";
            sql = string.Format(sql, downloadNum);
            var parameters = new[]
            {
                new SqlParameter("@LastUpdateBy", lastUpdateBy??"")
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql,parameters).ConvertTo<MoveCarQRCodeModel>().ToList();
        }
        #endregion

        /// <summary>
        /// 获取途虎挪车二维码生成记录列表
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<MoveCarGenerationRecordsModel> GetMoveCarGenerationRecordsList(SqlConnection conn, out int recordCount,int pageSize, int pageIndex )
        {
            string sql = @"
                        SELECT  [PKID] ,
                                [GeneratedNum] ,
                                [GeneratingStatus] ,
                                [CreateDatetime] ,
                                [LastUpdateDateTime] ,
                                [CreateBy] ,
                                [LastUpdateBy]
                        FROM    [Tuhu_profiles].[dbo].[MoveCarGenerationRecords] WITH ( NOLOCK )
                        ORDER BY [PKID] DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                    ONLY;";
            string sqlCount = @"
                                SELECT  COUNT(*)
                                FROM    [Tuhu_profiles].[dbo].[MoveCarGenerationRecords] WITH ( NOLOCK );";
            var parameters = new[]
           {
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@PageIndex", pageIndex)
            };
            recordCount = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount));
            if(recordCount>0)
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<MoveCarGenerationRecordsModel>().ToList();
            return new List<MoveCarGenerationRecordsModel>();
        }

        /// <summary>
        /// 获取途虎挪车二维码总生成下载记录
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public MoveCarTotalRecordsModel GetMoveCarTotalRecordsModel(SqlConnection conn)
        {
            string sql = @"
                        SELECT  [PKID] ,
                                [GeneratedNum] ,
                                [DownloadedNum] ,
                                [DownloadableNum] ,
                                [CreateDatetime] ,
                                [LastUpdateDateTime] 
                        FROM    [Tuhu_profiles].[dbo].[MoveCarTotalRecords] WITH ( NOLOCK );";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<MoveCarTotalRecordsModel>().ToList().FirstOrDefault();
        }
    }
}
