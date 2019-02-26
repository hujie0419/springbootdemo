using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.CarFriendsGroup;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO.CarFriendsGroup
{
    public class DalCarFriendsGroup
    {

        #region 车友群

        /// <summary>
        /// 获取车友群列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<CarFriendsGroupModel> GetCarFriendsGroupList(SqlConnection conn, out int recordCount, int pageSize, int pageIndex)
        {
            string sql = @"
                           SELECT   [PKID] ,
                                    [GroupName] ,
                                    [GroupDesc] ,
                                    [BindBrand],
                                    [BindVehicleType] ,
                                    [BindVehicleTypeID] ,
                                    [GroupHeadPortrait] ,
                                    [GroupQRCode] ,
                                    [GroupCategory] ,
                                    [GroupWeight] ,
                                    [IsRecommend] ,
                                    [Is_Deleted] ,
                                    [GroupCreateTime] ,
                                    [GroupOverdueTime] ,
                                    [CreateDatetime] ,
                                    [LastUpdateDateTime] ,
                                    [CreateBy] ,
                                    [LastUpdateBy]
                            FROM    Activity.[dbo].[CarFriendsWeChatGroup] WITH ( NOLOCK )
                            WHERE   Is_Deleted = 0
                            ORDER BY CreateDatetime DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                    ONLY;";
            string sqlCount = @"
                                SELECT  COUNT(*)
                                FROM    Activity.dbo.CarFriendsWeChatGroup WITH ( NOLOCK )
                                WHERE   Is_Deleted = 0;";
            var parameters = new[]
           {
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@PageIndex", pageIndex)
            };
            recordCount = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount));
            if (recordCount > 0)
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<CarFriendsGroupModel>().ToList();
            return new List<CarFriendsGroupModel>();
        }

        /// <summary>
        /// 新建车友群
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddCarFriendsGroup(SqlConnection conn,CarFriendsGroupModel model)
        {
            string sql = @"
                            INSERT  INTO Activity.[dbo].[CarFriendsWeChatGroup]
                                    ( [GroupName] ,
                                      [GroupDesc] ,
                                      [BindBrand] ,
                                      [BindVehicleType] ,
                                      [BindVehicleTypeID] ,
                                      [GroupHeadPortrait] ,
                                      [GroupQRCode] ,
                                      [GroupCategory] ,
                                      [GroupWeight] ,
                                      [IsRecommend] ,
                                      [Is_Deleted] ,
                                      [GroupCreateTime] ,
                                      [GroupOverdueTime] ,
                                      [CreateDatetime] ,
                                      [LastUpdateDateTime] ,
                                      [CreateBy] ,
                                      [LastUpdateBy]
                                    )
                            VALUES  ( @GroupName ,
                                      @GroupDesc ,
                                      @BindBrand ,
                                      @BindVehicleType ,
                                      @BindVehicleTypeID ,
                                      @GroupHeadPortrait ,
                                      @GroupQRCode ,
                                      @GroupCategory ,
                                      @GroupWeight ,
                                      @IsRecommend ,
                                      @Is_Deleted ,
                                      @GroupCreateTime ,
                                      @GroupOverdueTime ,
                                      GETDATE() ,
                                      GETDATE() ,
                                      @CreateBy ,
                                      @LastUpdateBy
                                    ); 
                            SELECT  SCOPE_IDENTITY();";
            var parameters = new[]
            {
                new SqlParameter("@GroupName",model.GroupName?? ""),
                new SqlParameter("@GroupDesc",model.GroupDesc?? ""),
                new SqlParameter("@BindBrand",model.BindBrand),
                new SqlParameter("@BindVehicleType",model.BindVehicleType),
                new SqlParameter("@BindVehicleTypeID",model.BindVehicleTypeID),
                new SqlParameter("@GroupHeadPortrait",model.GroupHeadPortrait?? ""),
                new SqlParameter("@GroupQRCode",model.GroupQRCode?? ""),
                new SqlParameter("@GroupCategory",model.GroupCategory),
                new SqlParameter("@GroupWeight",model.GroupWeight),
                new SqlParameter("@IsRecommend",model.IsRecommend),
                new SqlParameter("@Is_Deleted",model.Is_Deleted),
                new SqlParameter("@GroupCreateTime",model.GroupCreateTime),
                new SqlParameter("@GroupOverdueTime",model.GroupOverdueTime),
                new SqlParameter("@CreateBy",model.CreateBy?? ""),
                new SqlParameter("@LastUpdateBy",model.LastUpdateBy?? "")
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters))>0;
        }

        /// <summary>
        /// 编辑车友群
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCarFriendsGroup(SqlConnection conn,CarFriendsGroupModel model)
        {
            string sql = @"
                            UPDATE  Activity.[dbo].[CarFriendsWeChatGroup] WITH ( ROWLOCK )
                            SET     [GroupName] = @GroupName ,
                                    [GroupDesc] = @GroupDesc ,
                                    [BindBrand] = @BindBrand ,
                                    [BindVehicleType] = @BindVehicleType ,
                                    [BindVehicleTypeID] = @BindVehicleTypeID ,
                                    [GroupHeadPortrait] = @GroupHeadPortrait ,
                                    [GroupQRCode] = @GroupQRCode ,
                                    [GroupCategory] = @GroupCategory ,
                                    [GroupWeight] = @GroupWeight ,
                                    [IsRecommend] = @IsRecommend ,
                                    [GroupCreateTime] = @GroupCreateTime ,
                                    [GroupOverdueTime] = @GroupOverdueTime ,
                                    [LastUpdateDateTime] = GETDATE() ,
                                    [LastUpdateBy] = @LastUpdateBy
                            WHERE   PKID = @PKID;";
            var parameters = new[]
           {
                new SqlParameter("@GroupName",model.GroupName?? ""),
                new SqlParameter("@GroupDesc",model.GroupDesc?? ""),
                new SqlParameter("@BindBrand",model.BindBrand),
                new SqlParameter("@BindVehicleType",model.BindVehicleType),
                new SqlParameter("@BindVehicleTypeID",model.BindVehicleTypeID),
                new SqlParameter("@GroupHeadPortrait",model.GroupHeadPortrait?? ""),
                new SqlParameter("@GroupQRCode",model.GroupQRCode?? ""),
                new SqlParameter("@GroupCategory",model.GroupCategory),
                new SqlParameter("@GroupWeight",model.GroupWeight),
                new SqlParameter("@IsRecommend",model.IsRecommend),
                new SqlParameter("@GroupCreateTime",model.GroupCreateTime),
                new SqlParameter("@GroupOverdueTime",model.GroupOverdueTime),
                new SqlParameter("@LastUpdateBy",model.LastUpdateBy?? ""),
                new SqlParameter("@PKID",model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters)>0;
        }

        /// <summary>
        /// 逻辑删除车友群
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public bool DeleteCarFriendsGroup(SqlConnection conn, int pkid,string lastUpdateBy)
        {
            string sql = @"
                            UPDATE  Activity.[dbo].[CarFriendsWeChatGroup] WITH ( ROWLOCK )
                            SET     [Is_Deleted] = 1 ,
                                    [LastUpdateDateTime] = GETDATE() ,
                                    [LastUpdateBy] = @LastUpdateBy
                            WHERE   PKID = @PKID;";
            var parameters = new[]
          {
                new SqlParameter("@LastUpdateBy",lastUpdateBy?? ""),
                new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        #endregion

        #region 途虎管理员

        /// <summary>
        /// 获取途虎管理员列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<CarFriendsAdministratorsModel> GetCarFriendsAdministratorList(SqlConnection conn, out int recordCount, int pageSize, int pageIndex)
        {
            string sql = @"
                            SELECT  [PKID] ,
                                    [WeChatNickName] ,
                                    [WeChatNumber] ,
                                    [WeChatHeadPortrait] ,
                                    [WeChatQRCode] ,
                                    [Is_Deleted] ,
                                    [CreateDatetime] ,
                                    [LastUpdateDateTime] ,
                                    [CreateBy] ,
                                    [LastUpdateBy]
                            FROM    Activity.[dbo].[CarFriendsAdministrators] WITH ( NOLOCK )
                            WHERE   Is_Deleted = 0
                            ORDER BY CreateDatetime DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                    ONLY;";
            string sqlCount = @"
                                SELECT  COUNT(*)
                                FROM    Activity.[dbo].[CarFriendsAdministrators] WITH ( NOLOCK )
                                WHERE   Is_Deleted = 0;
                                ";
            var parameters = new[]
          {
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@PageIndex", pageIndex)
            };
            recordCount = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount));
            if (recordCount > 0)
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<CarFriendsAdministratorsModel>().ToList();
            return new List<CarFriendsAdministratorsModel>();
        }

        /// <summary>
        /// 新增途虎管理员
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddCarFriendsAdministrator(SqlConnection conn,CarFriendsAdministratorsModel model)
        {
            string sql = @"
                            INSERT  INTO Activity.[dbo].[CarFriendsAdministrators]
                                    ( [WeChatNickName] ,
                                      [WeChatNumber] ,
                                      [WeChatHeadPortrait] ,
                                      [WeChatQRCode] ,
                                      [Is_Deleted] ,
                                      [CreateDatetime] ,
                                      [LastUpdateDateTime] ,
                                      [CreateBy] ,
                                      [LastUpdateBy]
                                    )
                            VALUES  ( @WeChatNickName ,
                                      @WeChatNumber ,
                                      @WeChatHeadPortrait ,
                                      @WeChatQRCode ,
                                      @Is_Deleted ,
                                      GETDATE() ,
                                      GETDATE() ,
                                      @CreateBy ,
                                      @LastUpdateBy
                                    ); 
                            SELECT  SCOPE_IDENTITY();";
            var parameters = new[]
            {
                new SqlParameter("@WeChatNickName",model.WeChatNickName?? ""),
                new SqlParameter("@WeChatNumber",model.WeChatNumber?? ""),
                new SqlParameter("@WeChatHeadPortrait",model.WeChatHeadPortrait?? ""),
                new SqlParameter("@WeChatQRCode",model.WeChatQRCode?? ""),
                new SqlParameter("@Is_Deleted",model.Is_Deleted),
                new SqlParameter("@CreateBy",model.CreateBy?? ""),
                new SqlParameter("@LastUpdateBy",model.LastUpdateBy?? "")
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 编辑途虎管理员信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCarFriendsAdministrator(SqlConnection conn, CarFriendsAdministratorsModel model)
        {
            string sql = @"
                            UPDATE  Activity.[dbo].[CarFriendsAdministrators] WITH ( ROWLOCK )
                            SET     [WeChatNickName] = @WeChatNickName ,
                                    [WeChatNumber] = @WeChatNumber ,
                                    [WeChatHeadPortrait] = @WeChatHeadPortrait ,
                                    [WeChatQRCode] = @WeChatQRCode ,
                                    [LastUpdateDateTime] = GETDATE() ,
                                    [LastUpdateBy] = @LastUpdateBy
                            WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@WeChatNickName",model.WeChatNickName?? ""),
                new SqlParameter("@WeChatNumber",model.WeChatNumber?? ""),
                new SqlParameter("@WeChatHeadPortrait",model.WeChatHeadPortrait?? ""),
                new SqlParameter("@WeChatQRCode",model.WeChatQRCode?? ""),
                new SqlParameter("@LastUpdateBy",model.LastUpdateBy?? ""),
                new SqlParameter("@PKID",model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 逻辑删除途虎管理员信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public bool DeleteCarFriendsAdministrator(SqlConnection conn, int pkid,string lastUpdateBy)
        {
            string sql = @"
                            UPDATE  Activity.[dbo].[CarFriendsAdministrators] WITH ( ROWLOCK )
                            SET     [Is_Deleted] = 1 ,
                                    [LastUpdateDateTime] = GETDATE() ,
                                    [LastUpdateBy] = @LastUpdateBy
                            WHERE   PKID = @PKID;";
            var parameters = new[]
         {
                new SqlParameter("@LastUpdateBy",lastUpdateBy?? ""),
                new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
        #endregion
    }
}
