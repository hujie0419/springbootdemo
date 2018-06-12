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
    public class DALWXMaterialText
    {
        /// <summary>
        /// 新增文字素材
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddWXMaterialText(SqlConnection conn, WXMaterialTextModel model)
        {
            #region SQL
            string sql = @"
                        INSERT  INTO [Configuration].[dbo].[WXMaterialText]
                                ( [ObjectID],[ObjectType],[Content],[OriginalID] )
                        VALUES  ( @ObjectID,@ObjectType,@Content, @OriginalID );
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            #endregion
            model.PKID = conn.Query<int>(sql, model).FirstOrDefault();
            if (model.PKID > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 获取文字素材列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public List<WXMaterialTextModel> GetWXMaterialTextList(SqlConnection conn, long objectId, string objectType)
        {
            #region SQL
            string sql = @"SELECT  [PKID]
                                  ,[ObjectID]
                                  ,[ObjectType]
                                  ,[Content]
                                  ,[OriginalID]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                      FROM [Configuration].[dbo].[WXMaterialText] WITH(NOLOCK)
					  WHERE IsDeleted=0 
							AND PKID IN(SELECT
						MaterialID FROM Configuration..WXMenuMaterialMapping
						WITH(NOLOCK) WHERE ObjectID=@ObjectID AND ObjectType=@ObjectType)
							";
            #endregion
            var dp = new DynamicParameters();
            dp.Add("@ObjectID", objectId);
            dp.Add("@ObjectType", objectType);

            return conn.Query<WXMaterialTextModel>(sql, dp).ToList(); ;
        }
        /// <summary>
        /// 获取文字素材列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public WXMaterialTextModel GetWXMaterialTextInfo(SqlConnection conn, long pkid)
        {
            #region SQL
            string sql = @"SELECT [PKID]
                                  ,[ObjectID]
                                  ,[ObjectType]
                                  ,[Content]
                                  ,[OriginalID]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                      FROM [Configuration].[dbo].[WXMaterialText] WITH(NOLOCK)
                      WHERE IsDeleted=0 AND PKID=@PKID";
            #endregion
            var dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Query<WXMaterialTextModel>(sql, dp).FirstOrDefault();
        }
        /// <summary>
        /// 删除文字素材
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMaterialText(SqlConnection conn, long pkid)
        {
            string sql = @"UPDATE [Configuration].[dbo].[WXMaterialText] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", pkid);
            return conn.Execute(sql, dp) > 0;
        }
        /// <summary>
        /// 删除文字素材
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteWXMaterialText(SqlConnection conn, long objectId, string objectType)
        {
            string sql = @"UPDATE [Configuration].[dbo].[WXMaterialText] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() 
                            WHERE IsDeleted=0 
							AND PKID IN(SELECT
						MaterialID FROM Configuration..WXMenuMaterialMapping
						WITH(NOLOCK) WHERE ObjectID=@ObjectID AND ObjectType=@ObjectType)";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ObjectID", objectId);
            dp.Add("@ObjectType", objectType);

            return conn.Execute(sql, dp) > 0;
        }
    }
}
