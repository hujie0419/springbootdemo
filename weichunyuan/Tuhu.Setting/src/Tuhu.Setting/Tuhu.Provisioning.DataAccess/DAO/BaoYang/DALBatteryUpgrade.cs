using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Battery;
using Dapper;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Provisioning.DataAccess.Request;

namespace Tuhu.Provisioning.DataAccess.DAO.BaoYang
{
    public class DALBatteryLevelUp
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Tuple<int, List<BatteryLevelUpEntity>> GetBatteryLeveUpList(SqlConnection connection, BatteryLevelUpRequest model)
        {
            int totalCount = 0;
            var parameters = new SqlParameter []
            {
                 new SqlParameter("@PageIndex",model.PageIndex),
                 new SqlParameter("@PageSize",model.PageSize),
                 new SqlParameter("@pid",model.OriginalPID),
                 new  SqlParameter("@totalCount",SqlDbType.Int){  Direction=ParameterDirection.Output}
            };
            var sql = @" SELECT @totalCount = COUNT(1)
                            FROM BaoYang..BatteryLevelUp AS a
                            WHERE (
                                      a.OriginalPID = @pid
                                      OR @pid = ''
                                      OR @pid IS NULL
                                  )
                                  OR
                                  (
                                      a.NewPID = @pid
                                      OR @pid = ''
                                      OR @pid IS NULL
                                  );
                            SELECT a.OriginalPID,
                                   a.NewPID,
                                   a.Copywriting,
                                   a.IsEnabled,
                                   a.CreateDateTime,
                                   a.LastUpdateDateTime,
                                   a.PKID
                            FROM BaoYang.dbo.BatteryLevelUp AS a WITH (NOLOCK)
                            WHERE (
                                      a.OriginalPID = @pid
                                      OR @pid = ''
                                      OR @pid IS NULL
                                  )
                                  OR
                                  (
                                      a.NewPID = @pid
                                      OR @pid = ''
                                      OR @pid IS NULL
                                  )
                            ORDER BY a.PKID DESC OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY; ";
           var dt =  SqlHelper.ExecuteDataTable2(connection, CommandType.Text, sql, parameters);
            var list = dt.AsEnumerable().Select(x => new BatteryLevelUpEntity
            {
                Copywriting = x["Copywriting"].ToString(),
                IsEnabled = Convert.ToBoolean(x["IsEnabled"]),
                NewPID = x["NewPID"].ToString(),
                OriginalPID = x["OriginalPID"].ToString(),
                CreateDateTime = Convert.ToDateTime(x["CreateDateTime"]),
                PKID = int.Parse(x["PKID"].ToString()),
                 LsatUpdateDateTime = Convert.ToDateTime(x["LastUpdateDateTime"])
            }).ToList();
            int.TryParse(parameters.Last().Value.ToString(), out totalCount);
            return new Tuple<int, List<BatteryLevelUpEntity>>(totalCount, list);
        }
        /// <summary>
        /// 添加蓄电池升级购实体
        /// </summary>
        /// <returns></returns>
        public static bool InsertBatteryLevelUp(SqlConnection connection,BatteryLevelUpEntity entity )
        {
            var sql = @"INSERT BaoYang..BatteryLevelUp
                        (
                            OriginalPID,
                            NewPID,
                            Copywriting,
                            IsEnabled
                        )
                        VALUES
                        (@OriginalPID, @NewPID, @Copywriting, @IsEnabled);";
            var parameters = new
            {
                entity.OriginalPID,
                entity.NewPID,
                entity.Copywriting,
                entity.IsEnabled
            };
            var result = connection.Execute(sql, param: parameters, commandType: CommandType.Text);
            return result > 0;
        }
        /// <summary>
        ///修改蓄电池升级购实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool UpdateBatteryLevelUp(SqlConnection connection,BatteryLevelUpEntity entity)
        {
            var sql = @"UPDATE BaoYang..BatteryLevelUp
                        SET OriginalPID = @OriginalPID,
                            NewPID = @NewPID,
                            Copywriting = @Copywriting,
                            IsEnabled = @IsEnabled,
                            LastUpdateDateTime = GETDATE()
                            WHERE PKID = @PKID;";
            var parameters = new
            {
                entity.OriginalPID,
                entity.NewPID,
                entity.Copywriting,
                entity.IsEnabled,
                entity.PKID
            };
            var result = connection.Execute(sql, param: parameters, commandType: CommandType.Text);
            return result > 0;
        }
        /// <summary>
        /// 获取蓄电池升级购实体
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static BatteryLevelUpEntity GetBatteryLevelUpEntity(SqlConnection connection,int pkid)
        {
            var sql = @"SELECT a.OriginalPID,
                       a.NewPID,
                       a.Copywriting,
                       a.PKID,
                       a.IsEnabled
                       FROM BaoYang.dbo.BatteryLevelUp AS a WITH (NOLOCK)
                       WHERE a.PKID = @PKID;";
            var parameters = new
            {
                PKID = pkid
            };

            var entity = connection.QueryFirstOrDefault<BatteryLevelUpEntity>(sql, param: parameters, commandType: CommandType.Text);
            return entity;
        }
        /// <summary>
        /// 根据原始产品PID 获取升级购配置
        /// </summary>
        /// <param name="originalPID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static BatteryLevelUpEntity GetBatteryLevelUpEntityByOriginalPid(SqlConnection connection ,string originalPID)
        {
            var sql = @"SELECT a.PKID
                        FROM BaoYang.dbo.BatteryLevelUp AS a WITH (NOLOCK)
                        WHERE a.OriginalPID = @originalPID And a.IsEnabled = 1;";
            var parameters = new
            {
                originalPID
            };
            var entity = connection.QueryFirstOrDefault<BatteryLevelUpEntity>(sql, param: parameters, commandType: CommandType.Text);
            return entity;
        }
        /// <summary>
        /// 根据升级够产品PID 获取升级购配置
        /// </summary>
        /// <param name="newPid"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static BatteryLevelUpEntity GetBatteryLevelUpEntityByNewPID(SqlConnection connection,string newPid)
        {
            var sql = @"SELECT a.PKID
                        FROM BaoYang.dbo.BatteryLevelUp AS a WITH (NOLOCK)
                        WHERE a.NewPID = @newPid And a.IsEnabled = 1;";
            var parameters = new
            {
                newPid
            };
            var entity = connection.QueryFirstOrDefault<BatteryLevelUpEntity>(sql, param: parameters, commandType: CommandType.Text);
            return entity;
        }
        /// <summary>
        /// 根据产品PID 获取升级购配置
        /// </summary>
        /// <param name="newPid"></param>
        /// <param name="originalPID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static BatteryLevelUpEntity GetBatteryLevelUpEntity(SqlConnection connection,string newPid ,string originalPID)
        {
            var sql = @"SELECT a.PKID
                        FROM BaoYang.dbo.BatteryLevelUp AS a WITH (NOLOCK)
                        WHERE a.OriginalPID = @originalPID
                        AND a.NewPID = @newPid;";
            var parameters = new
            {
                newPid,
                originalPID
            };
            var entity = connection.QueryFirstOrDefault<BatteryLevelUpEntity>(sql, param: parameters, commandType: CommandType.Text);
            return entity;
        }
        /// <summary>
        /// 删除升级购配置项
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteBatteryByPkid(SqlConnection connection,int pkid) {
            var sql = @"delete BaoYang..BatteryLevelUp 
                        where pkid =@pkid";
            var parameters = new SqlParameter[]
           {
                 new SqlParameter("@pkid",pkid) 
           };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql,parameters)>0;
        }
        /// <summary>
        /// 获取所有启用状态的PKID 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static  List<string> GetAllOriginalPID(SqlConnection connection) {
            var sql = @"select  a.OriginalPID FROM BaoYang..BatteryLevelUp as a WITH (NOLOCK)";
            var dt = SqlHelper.ExecuteDataTable2(connection, CommandType.Text, sql); 
            return dt.AsEnumerable().Select(x =>x["OriginalPID"].ToString()).ToList();
        }
    }
}
