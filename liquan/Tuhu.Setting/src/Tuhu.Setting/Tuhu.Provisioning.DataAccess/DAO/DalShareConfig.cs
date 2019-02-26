using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalShareConfig
    {
        public static List<ShareConfigSource> QueryShareConfigs(SqlConnection conn, ShareConfigQuery query)
        {
            string queryShareConfigSumText = @" SELECT Count(1) FROM Configuration..ShareConfigSource AS SCS
                                                WHERE(@Location IS NULL OR SCS.Location LIKE N'%' + @Location + '%') AND SCS.PKId = ISNULL(@PKId, SCS.PKId)
                                                AND(@Description IS NULL OR SCS.Description LIKE N'%' + @Description + '%')
                                                AND SCS.Status = ISNULL(@Status, SCS.Status)
                                                AND(@MinTime IS NULL OR SCS.CreateDateTime >= @MinTime)
                                                AND(@MaxTime IS NULL OR SCS.CreateDateTime <= @MaxTime)
                                                AND(@Creator IS NULL OR SCS.Creator LIKE N'%' + @Creator + '%')
                                                AND(@ShareType is NULL
                                                    OR SCS.Location in(SELECT SCS.Location
                                                    FROM Configuration..ShareConfigSource AS SCS left join Configuration..ShareSupervisionConfig AS SSC on SCS.Location=SSC.Location
                                                    WHERE SSC.ShareType = @ShareType)
                                                )";
            string queryShareConfigText = @"SELECT * FROM Configuration..ShareConfigSource AS SCS
                                            WHERE(@Location IS NULL OR SCS.Location LIKE N'%' + @Location + '%') AND SCS.PKId = ISNULL(@PKId, SCS.PKId)
                                            AND(@Description IS NULL OR SCS.Description LIKE N'%' + @Description + '%')
                                            AND SCS.Status = ISNULL(@Status, SCS.Status)
                                            AND(@MinTime IS NULL OR SCS.CreateDateTime >= @MinTime)
                                            AND(@MaxTime IS NULL OR SCS.CreateDateTime <= @MaxTime)
                                            AND(@Creator IS NULL OR SCS.Creator LIKE N'%' + @Creator + '%')
                                            AND(@ShareType is NULL
                                                OR SCS.Location in(SELECT SCS.Location
                                                FROM Configuration..ShareConfigSource AS SCS left join Configuration..ShareSupervisionConfig AS SSC on SCS.Location=SSC.Location
                                                WHERE SSC.ShareType = @ShareType)
                                            )
                                            ORDER BY SCS.CreateDateTime DESC OFFSET @Page ROWS FETCH NEXT 50 ROWS ONLY";
            var sqlParam = new[]
                {
                    new SqlParameter("@PKId", query.IdCriterion),
                    new SqlParameter("@Location", query.LocationCriterion),
                    new SqlParameter("@Description", query.DescriptionCriterion),
                    new SqlParameter("@Status", query.StatusCriterion),
                    new SqlParameter("@MinTime", query.MinTimeCriterion),
                    new SqlParameter("@MaxTime", query.MaxTimeCriterion),
                    new SqlParameter("@Creator", query.CreatorCriterion),
                    new SqlParameter("@ShareType", query.ShareTypeCriterion),
                    new SqlParameter("@Page", (query.PageIndex-1)*50)
                };
            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, queryShareConfigSumText,sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, queryShareConfigText, sqlParam).ConvertTo<ShareConfigSource>().ToList();
        }

        public static int SelectPKIdByLocation(SqlConnection conn, string location, string specialParam = null)
        {
            int pkid = 0;
            string selectPKIdByLocationText = @"SELECT SCS.PKId FROM Configuration..ShareConfigSource AS SCS WITH(NOLOCK) WHERE SCS.Location=@location AND " + (specialParam == null ? "SCS.SpecialParam is null":"SCS.SpecialParam=@SpecialParam");
            var sqlParam = new[] { new SqlParameter("@location", location), new SqlParameter("@SpecialParam", specialParam) };
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, selectPKIdByLocationText, sqlParam);
            if (obj != null)
            {
                pkid = Convert.ToInt32(obj);
            }
            return pkid;
        }

        public static string SelectLocationByPKId(SqlConnection conn, int pkid)
        {
            string location = "";
            string selectLocationByPKIdText = @"SELECT SCS.Location FROM Configuration..ShareConfigSource AS SCS WITH(NOLOCK) WHERE SCS.PKId=@pkid";
            var sqlParam = new[] { new SqlParameter("@pkid", pkid) };
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, selectLocationByPKIdText, sqlParam);
            if (obj != null)
            {
                location = Convert.ToString(obj);
            }
            return location;
        }

        public static List<ShareSupervisionConfig> QueryShareSConfigByJumpId(SqlConnection conn,int jumpId)
        {
            string queryShareSConfigText = @"SELECT * FROM Configuration..ShareSupervisionConfig AS SSC WHERE SSC.JumpId=@jumpId";
            var sqlParam = new[] {new SqlParameter("@jumpId", jumpId) };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, queryShareSConfigText, sqlParam).ConvertTo<ShareSupervisionConfig>().ToList();
        }

        public static List<ShareConfigLog> QueryShareConfigLogById(SqlConnection conn, int id)
        {
            string queryShareConfigLogText = @"SELECT SCL.PKId,SCL.ConfigId,SCL.Operator,SCL.OperateType,SCL.CreateDateTime,SCL.LastUpdateDateTime
                                               FROM Configuration..ShareConfigLog AS SCL with(nolock)
                                               WHERE SCL.ConfigId=@ConfigId";
            var sqlParam = new[]
                {
                    new SqlParameter("@ConfigId",id),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, queryShareConfigLogText, sqlParam).ConvertTo<ShareConfigLog>().ToList();
        }

        public static bool UpdateShareConfig(SqlConnection conn,ShareConfigSource scs)
        {
            string updateShareConfigText = @"
                UPDATE Configuration..ShareConfigSource
                SET Location=@location,
                Description=@description,
                Status=@status,
                UpdateDateTime=@updatedatetime,
                SpecialParam=@SpecialParam
                WHERE PKId=@pkid";
            var sqlParam = new SqlParameter[]{
                new SqlParameter("@location",scs.Location),
                new SqlParameter("@description",scs.Description),
                new SqlParameter("@status",scs.Status),
                new SqlParameter("@updatedatetime",scs.UpdateDateTime),
                new SqlParameter("@pkid",scs.PKId),
                new SqlParameter("@SpecialParam",scs.SpecialParam),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateShareConfigText, sqlParam) > 0 ? true : false;
        }
        
        public static int InsertShareConfig(SqlConnection conn,ShareConfigSource scs)
        {
            string insertShareConfigText = @"
                INSERT INTO Configuration..ShareConfigSource(Location,Description,Status,CreateDateTime,UpdateDateTime,Creator,SpecialParam)
                OUTPUT Inserted.PKId
                VALUES (@location,@description,@status,@createdatetime,@updatedateTime,@creator,@SpecialParam)";
            var sqlParam = new SqlParameter[]{
                new SqlParameter("@location",scs.Location),
                new SqlParameter("@description",scs.Description),
                new SqlParameter("@status",scs.Status),
                new SqlParameter("@createdatetime",scs.CreateDateTime),
                new SqlParameter("@updatedateTime",scs.UpdateDateTime),
                new SqlParameter("@creator",scs.Creator),
                new SqlParameter("@SpecialParam", scs.SpecialParam),
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, insertShareConfigText, sqlParam));
        }

        public static ShareSupervisionConfig QueryShareSConfigById(SqlConnection conn,int id)
        {
            string queryShareSConfigText = @"SELECT * FROM Configuration..ShareSupervisionConfig AS SSC WHERE SSC.PKId=@id";
            var sqlParam = new[]
                {
                    new SqlParameter("@id",id),
                };
            List<ShareSupervisionConfig> sscList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, queryShareSConfigText, sqlParam).ConvertTo<ShareSupervisionConfig>().ToList();
            return sscList[0];
        }

        public static bool InsertShareSConfig(SqlConnection conn,ShareSupervisionConfig ssc)
        {
            string insertShareSConfigText = @"
                INSERT INTO Configuration..ShareSupervisionConfig(Location,ShareScene,Status,ShareType,ShareUrl,ThumbPic,Title,Description,BgPic,ParameterRef,CreateDateTime,UpdateDateTime,Creator,SceneSequence,PagePath,MiniGramId,JumpId)
                VALUES (@location,@sharescene,@status,@sharetype,@shareurl,@thumbpic,@title,@description,@bgpic,@parameterref,@createdatetime,@updatedatetime,@creator,@scenesequence,@pagepath,@MiniGramId,@JumpId)";
            var sqlParam = new[]
                {
                    new SqlParameter("@location",ssc.Location),
                    new SqlParameter("@sharescene",ssc.ShareScene),
                    new SqlParameter("@status",ssc.Status),
                    new SqlParameter("@sharetype",ssc.ShareType),
                    new SqlParameter("@shareurl",ssc.ShareUrl),
                    new SqlParameter("@thumbpic",ssc.ThumbPic),
                    new SqlParameter("@title",ssc.Title),
                    new SqlParameter("@description",ssc.Description),
                    new SqlParameter("@bgpic",ssc.BgPic),
                    new SqlParameter("@parameterref",ssc.ParameterRef),
                    new SqlParameter("@createdatetime",ssc.CreateDateTime),
                    new SqlParameter("@updatedatetime",ssc.UpdateDateTime),
                    new SqlParameter("@creator",ssc.Creator),
                    new SqlParameter("@scenesequence",ssc.SceneSequence),
                    new SqlParameter("@pagepath",ssc.PagePath),
                    new SqlParameter("@MiniGramId", ssc.MiniGramId),
                    new SqlParameter("@JumpId", ssc.JumpId),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, insertShareSConfigText, sqlParam) > 0 ? true : false;
        }

        public static bool UpdateShareSConfig(SqlConnection conn,ShareSupervisionConfig ssc)
        {
            string updateShareSConfigText = @"
                UPDATE Configuration..ShareSupervisionConfig
                SET ShareScene=@sharescene,
                Status=@status,
                ShareType=@sharetype,
                ShareUrl=@shareurl,
                ThumbPic=@thumbpic,
                Title=@title,
                Description=@description,
                BgPic=@bgpic,
                ParameterRef=@parameterref,
                UpdateDateTime=@updatedatetime,
                SceneSequence=@scenesequence,
                PagePath=@pagepath,
                MiniGramId=@MiniGramId
                WHERE PKId=@pkid";
            var sqlParam = new[]
                {
                    new SqlParameter("@sharescene",ssc.ShareScene),
                    new SqlParameter("@status",ssc.Status),
                    new SqlParameter("@sharetype",ssc.ShareType),
                    new SqlParameter("@shareurl",ssc.ShareUrl),
                    new SqlParameter("@thumbpic",ssc.ThumbPic),
                    new SqlParameter("@title",ssc.Title),
                    new SqlParameter("@description",ssc.Description),
                    new SqlParameter("@bgpic",ssc.BgPic),
                    new SqlParameter("@parameterref",ssc.ParameterRef),
                    new SqlParameter("@updatedatetime",ssc.UpdateDateTime),
                    new SqlParameter("@scenesequence",ssc.SceneSequence),
                    new SqlParameter("@pagepath",ssc.PagePath),
                    new SqlParameter("@pkid",ssc.PKId),
                    new SqlParameter("@MiniGramId", ssc.MiniGramId)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateShareSConfigText, sqlParam) > 0 ? true : false;
        }

        public static bool DeleteShareSConfigById(SqlConnection conn,int id)
        {
            string deleteShareSConfigText = @"
                DELETE FROM Configuration..ShareSupervisionConfig
                WHERE PKId=@pkid";
            var sqlParam = new[]
                {
                    new SqlParameter("@pkid",id),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, deleteShareSConfigText, sqlParam) > 0 ? true : false;
        }

        public static bool DeleteShareConfigByLocation(SqlConnection conn,string location, string specialParam)
        {
            string deleteShareConfigText = @"
                DELETE FROM Configuration..ShareConfigSource
                WHERE Location=@location AND "+
                (specialParam == null ? "SpecialParam is null" : "SpecialParam=@SpecialParam");
            var sqlParam = new[]
                {
                    new SqlParameter("@location",location),
                    new SqlParameter("@SpecialParam",specialParam),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, deleteShareConfigText, sqlParam) > 0 ? true : false;
        }

        public static bool DeleteShareSConfigByJumpId(SqlConnection conn, int jumpId)
        {
            string deleteShareSConfigText = @"
                DELETE FROM Configuration..ShareSupervisionConfig
                WHERE JumpId=@jumpId";
            var sqlParam = new[]
                {
                    new SqlParameter("@jumpId",jumpId),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, deleteShareSConfigText, sqlParam) > 0 ? true : false;
        }

        public static bool InsertShareConfigLog(SqlConnection conn,ShareConfigLog scl)
        {
            string insertShareConfigLogText = @"
                INSERT INTO Configuration..ShareConfigLog(ConfigId,Operator,OperateType,CreateDateTime,LastUpdateDateTime)
                VALUES (@configid,@operator,@operatetype,@createdatetime,@lastupdatedatetime)";
            var sqlParam = new[]
                {
                    new SqlParameter("@configid",scl.ConfigId),
                    new SqlParameter("@operator",scl.Operator),
                    new SqlParameter("@operatetype",scl.OperateType),
                    new SqlParameter("@createdatetime",scl.CreateDateTime),
                    new SqlParameter("@lastupdatedatetime",scl.LastUpdateDateTime),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, insertShareConfigLogText, sqlParam) > 0 ? true : false;
        }

        public static bool DeleteShareConfigLog(SqlConnection conn, int id)
        {
            string deleteShareConfigLogText = @"
                DELETE FROM Configuration..ShareConfigLog
                WHERE ConfigId=@configid";
            var sqlParam = new[]
                {
                    new SqlParameter("@configid",id),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, deleteShareConfigLogText, sqlParam) > 0 ? true : false;
        }
    }
}
