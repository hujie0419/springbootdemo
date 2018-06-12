using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALWXMenuMaterialMapping
    {
        /// <summary>
        /// 新增微信菜单和素材关系信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddWXMenuMaterialMapping(SqlConnection conn, WXMenuMaterialMappingModel model)
        {
            #region SQL
            string sql = @"INSERT INTO [Configuration].[dbo].[WXMenuMaterialMapping]
                                       ([ObjectID]
                                       ,[ObjectType]
                                       ,[OriginalID]
                                       ,[ButtonKey]
                                       ,[MaterialID]
                                       ,[MediaType]
                                       ,[MediaID]
                                       ,[Title]
                                       ,[ImageUrl]
                                       ,[ReplyWay]
                                       ,[IsDelaySend]
                                       ,[IntervalHours]
                                       ,[IntervalMinutes])
                                 VALUES
                                       (@ObjectID
                                       ,@ObjectType
                                       ,@OriginalID
                                       ,@ButtonKey
                                       ,@MaterialID
                                       ,@MediaType
                                       ,ISNULL(@MediaID,0)
                                       ,ISNULL(@Title,'')
                                       ,ISNULL(@ImageUrl,'')
                                       ,@ReplyWay
                                       ,@IsDelaySend
                                       ,ISNULL(@IntervalHours,0)
                                       ,ISNULL(@IntervalMinutes,0));
                           SELECT CAST(SCOPE_IDENTITY() as int);";
            #endregion
            model.PKID = conn.Query<int>(sql, model).FirstOrDefault();
            if (model.PKID > 0)
                return true;
            else return false;
        }


        /// <summary>
        /// 更新 素材关系表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateWXMenuMaterialMapping(SqlConnection conn, WXMenuMaterialMappingModel model)
        {
            #region SQL
            string sql = @"Update [Configuration].[dbo].[WXMenuMaterialMapping]
                        set [ObjectID]=@ObjectID,
                            [ObjectType]=@ObjectType,
                            [OriginalID]=@OriginalID,
                            [ButtonKey]=@ButtonKey,
                            [MaterialID]=@MaterialID,
                            [MediaType]=@MediaType,
                            [MediaID]=ISNULL(@MediaID,0),
                            [Title]=ISNULL(@Title,''),
                            [ImageUrl]=ISNULL(@ImageUrl,''),
                            [ReplyWay]=@ReplyWay,
                            [IsDelaySend]=@IsDelaySend,
                            [IntervalHours]=ISNULL(@IntervalHours,0),
                            [IntervalMinutes]=ISNULL(@IntervalMinutes,0)
                        where PKID =@PKID
                        ";
            #endregion
            model.PKID = conn.Query<int>(sql, model).FirstOrDefault();
            if (model.PKID > 0)
                return true;
            else return false;
        }

        /// <summary>
        /// 获取微信菜单和素材关系列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<WXMenuMaterialMappingModel> GetWXMenuMaterialMappingList(SqlConnection conn, long objectId, string objectType)
        {
            #region SQL
            string sql = @"SELECT [PKID]
                          ,[ObjectID]
                          ,[ObjectType]
                          ,[OriginalID]
                          ,[ButtonKey]
                          ,[MaterialID]
                          ,[MediaType]
                          ,[MediaID]
                          ,[Title]
                          ,[ImageUrl]
                          ,[ReplyWay]
                          ,[IsDelaySend]
                          ,[IntervalHours]
                          ,[IntervalMinutes]
                          ,[CreateDateTime]
                          ,[LastUpdateDateTime]
                          ,[IsDeleted]
                      FROM [Configuration].[dbo].[WXMenuMaterialMapping] WITH(NOLOCK)
                      WHERE IsDeleted=0 AND ObjectID=@ObjectID and ObjectType=@ObjectType ";
            #endregion
            var dp = new DynamicParameters();
            dp.Add("@ObjectID", objectId);
            dp.Add("@ObjectType", objectType);
            return conn.Query<WXMenuMaterialMappingModel>(sql, dp).ToList();
        }
        /// <summary>
        /// 获取微信菜单和素材关系详情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public WXMenuMaterialMappingModel GetWXMenuMaterialMappingInfo(SqlConnection conn, long pkid)
        {
            #region SQL
            string sql = @"SELECT [PKID]
                                  ,[ObjectID]
                                  ,[ObjectType]
                                  ,[OriginalID]
                                  ,[ButtonKey]
                                  ,[MaterialID]
                                  ,[MediaType]
                                  ,[MediaID]
                                  ,[Title]
                                  ,[ImageUrl]
                                  ,[ReplyWay]
                                  ,[IsDelaySend]
                                  ,[IntervalHours]
                                  ,[IntervalMinutes]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                              FROM [Configuration].[dbo].[WXMenuMaterialMapping] WITH(NOLOCK)
                              WHERE PKID=@PKID";
            #endregion
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Query<WXMenuMaterialMappingModel>(sql, dp).FirstOrDefault();
        }
        /// <summary>
        /// 删除微信菜单和素材关系
        /// </summary>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMenuMaterialMapping(SqlConnection conn, long pkid)
        {
            string sql = @"UPDATE [Configuration].[dbo].[WXMenuMaterialMapping] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Execute(sql, dp) > 0;
        }
        /// <summary>
        /// 删除微信菜单和素材关系
        /// </summary>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMenuMaterialMapping(SqlConnection conn, long objectId, string objectType)
        {
            string sql = @"UPDATE [Configuration].[dbo].[WXMenuMaterialMapping] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE()
                            WHERE IsDeleted=0 AND ObjectID=@ObjectID AND ObjectType=@ObjectType";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ObjectID", objectId);
            dp.Add("@ObjectType", objectType);

            return conn.Execute(sql, dp) > 0;
        }
    }
}
