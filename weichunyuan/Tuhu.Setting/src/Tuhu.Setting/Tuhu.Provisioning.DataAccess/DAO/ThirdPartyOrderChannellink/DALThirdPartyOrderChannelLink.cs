using System;
using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity.ThirdPartyOrderChannellink;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALThirdPartyOrderChannelLink
    {
        /// <summary>
        /// 获取三方渠道链接申请列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="orderChannel"></param>
        /// <param name="businessType"></param>
        /// <param name="status"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static List<ThirdPartyOrderChannellinkModel> GetTPOrderChannellinkList(out int recordCount, string orderChannel, string businessType,int status=0, int pageSize = 10, int pageIndex = 1)
        {
            string sql = @"SELECT b.[PKID]
              ,[OrderChanneID]
              ,[OrderChannelEngName]
              ,[BusinessType]
              ,[Status]
              ,[InitialPagelink]
              ,[FinalPagelink]
              ,[IsAggregatePage]
              ,[IsAuthorizedLogin]
              ,[IsPartnerReceivSilver]
              ,[IsOrderBack]
              ,[IsViewOrders]
              ,[IsViewCoupons]
              ,[IsContactUserService]
              ,[IsBackTop]
              ,[LastUpdateBy]
              ,b.[LastUpdateDateTime]
              ,a.[OrderChannel]
          FROM Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannel] AS a WITH(NOLOCK) INNER JOIN
                tbl_ThirdPartyOrderChannellink AS b WITH(NOLOCK) ON a.PKID = b.OrderChanneID where 1=1 ";
            string sqlCount = @"SELECT COUNT(1) FROM Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannel] AS a WITH(NOLOCK) INNER JOIN
                tbl_ThirdPartyOrderChannellink AS b WITH(NOLOCK) ON a.PKID = b.OrderChanneID where 1=1 ";
            if (!string.IsNullOrEmpty(orderChannel)&&orderChannel != "全部")
            {
                sql += "and a.OrderChannel=@OrderChannel ";
                sqlCount += "and a.OrderChannel=@OrderChannel ";
            }
            if (!string.IsNullOrEmpty(businessType) && businessType != "全部")
            {
                sql += "and b.BusinessType=@BusinessType ";
                sqlCount += "and b.BusinessType=@BusinessType ";
            }
            if (status != 0)
            {
                sql += "and b.Status=@Status ";
                sqlCount += "and b.Status=@Status ";
            }
            sql += " order by [PKID] DESC OFFSET (@PageIndex -1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY ";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PageSize", pageSize);
                dp.Add("@PageIndex", pageIndex);
                if (!string.IsNullOrEmpty(orderChannel) && orderChannel != "全部")
                {
                    dp.Add("@OrderChannel", orderChannel);
                }
                if (!string.IsNullOrEmpty(businessType) && businessType != "全部")
                {
                    dp.Add("@BusinessType", businessType);
                }
                if (status != 0)
                {
                    dp.Add("@Status", status);
                }

                SqlCommand cmd = new SqlCommand(sqlCount, conn);
                if (!string.IsNullOrEmpty(orderChannel) && orderChannel != "全部")
                {
                    cmd.Parameters.Add(new SqlParameter("@OrderChannel", orderChannel));
                }
                if (!string.IsNullOrEmpty(businessType) && businessType != "全部")
                {
                    cmd.Parameters.Add(new SqlParameter("@BusinessType", businessType));
                }
                if (status != 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                }
                recordCount = (int)cmd.ExecuteScalar();
                return conn.Query<ThirdPartyOrderChannellinkModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 从三方渠道表获取所有三方渠道key
        /// </summary>
        /// <returns></returns>
        public static List<ThirdPartyOrderChannelModel> GetTPOrderChannelList()
        {
            string sql = @"SELECT [PKID]
                          ,[OrderChannel]
                          ,[CreateDatetime]
                          ,[LastUpdateDateTime]
                      FROM Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannel] WITH(NOLOCK)";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                conn.Open();
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<ThirdPartyOrderChannelModel>().ToList();
            }
        }

        /// <summary>
        /// 三方渠道链接-状态操作（启用或禁用）
        /// </summary>
        /// <param name="status"></param>
        /// <param name="lastUpdateBy"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int UpdateTPOrderChannellinkStatus(int status,string lastUpdateBy, int PKID)
        {
            //业务层先select
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                string sql = @"UPDATE [Tuhu_thirdparty].[dbo].[tbl_ThirdPartyOrderChannellink] WITH(ROWLOCK) SET Status=@Status,LastUpdateBy=@LastUpdateBy,LastUpdateDateTime=GETDATE() where PKID=@PKID";
                var cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@LastUpdateBy", lastUpdateBy);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        #region 添加渠道链接
        /// <summary>
        /// 根据订单渠道key获取三方订单渠道集合
        /// </summary>
        /// <param name="orderChannel"></param>
        /// <returns></returns>
        public static ThirdPartyOrderChannelModel GetTPOrderChannelListByOrderChannel(string orderChannel)
        {
            string sql = @"SELECT [PKID]
                  ,[OrderChannel]
                  ,[CreateDatetime]
                  ,[LastUpdateDateTime]
              FROM Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannel] WITH(NOLOCK) where 1=1 ";
            if (!string.IsNullOrWhiteSpace(orderChannel))
            {
                sql += "and OrderChannel=@OrderChannel ";
            }
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@OrderChannel", orderChannel);
                return conn.Query<ThirdPartyOrderChannelModel>(sql, dp).ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取值与orderChannelEngName相同的简称个数 
        /// </summary>
        /// <param name="orderChannelEngName"></param>
        /// <returns></returns>
        public static int GetTPOrderChannelDiffPinYin(string orderChannelEngName)
        {
            string sql = @"SELECT COUNT(DISTINCT(equeEngName)) FROM (
                           SELECT threeEngName+'_'+twoEngName AS equeEngName ,* FROM (
                           SELECT  PARSENAME(REPLACE(OrderChannelEngName,'_','.'),2)  AS twoEngName,PARSENAME(REPLACE(OrderChannelEngName,'_','.'),3) AS threeEngName,
		                   OrderChannelEngName,PKID FROM Tuhu_thirdparty.dbo.tbl_ThirdPartyOrderChannellink WITH(NOLOCK)
                            ) a  WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace(orderChannelEngName))
            {
                sql += "and threeEngName=@orderChannelEngName ";
            }
            sql += ") b";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrWhiteSpace(orderChannelEngName))
                {
                    cmd.Parameters.Add(new SqlParameter("@orderChannelEngName", orderChannelEngName));
                }
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 获取某订单渠道key下的所有渠道链接
        /// </summary>
        /// <param name="orderChanneID"></param>
        /// <returns></returns>
        public static List<ThirdPartyOrderChannellinkModel> GetOrderChannelLinkByOrderChanneID(int orderChanneID)
        {
            string sql = @"SELECT [PKID]
                      ,[OrderChanneID]
                      ,[OrderChannelEngName]
                      ,[BusinessType]
                      ,[Status]
                      ,[InitialPagelink]
                      ,[FinalPagelink]
                      ,[IsAggregatePage]
                      ,[IsAuthorizedLogin]
                      ,[IsPartnerReceivSilver]
                      ,[IsOrderBack]
                      ,[IsViewOrders]
                      ,[IsViewCoupons]
                      ,[IsContactUserService]
                      ,[IsBackTop]
                      ,[CreateBy]
                      ,[LastUpdateBy]
                      ,[CreateDatetime]
                      ,[LastUpdateDateTime]
                  FROM Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannellink] WITH(NOLOCK) where OrderChanneID=@OrderChanneID";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@OrderChanneID", orderChanneID);
                return conn.Query<ThirdPartyOrderChannellinkModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 根据PKID获取三方渠道实体
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static ThirdPartyOrderChannelModel GetThirdPartyOrderChannelModel(int PKID)
        {
            string sql = @"SELECT [PKID]
                  ,[OrderChannel]
                  ,[CreateDatetime] 
                  ,[LastUpdateDateTime]
              FROM Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannel] WITH(NOLOCK) where PKID=@PKID";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("ThirdPartyReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PKID", PKID);
                return conn.Query<ThirdPartyOrderChannelModel>(sql, dp).ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// 插入第三方订单渠道key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddThirdPartyOrderChannel(ThirdPartyOrderChannelModel model)
        {
            var sql = @"INSERT INTO Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannel]
                                ( OrderChannel
                                 ,CreateDatetime
                                 ,LastUpdateDateTime
                                )
                        VALUES  ( @OrderChannel , 
                                  GETDATE() ,
                                  GETDATE()
                                 )
                        SELECT  SCOPE_IDENTITY();";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("ThirdParty")))
            {
                var parameters = new[]
            {
                new SqlParameter("@OrderChannel", model.OrderChannel),
            };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            }
        }

        /// <summary>
        /// 向三方订单渠道链接表添加渠道链接
        /// </summary>
        /// <param name="linkmodel"></param>
        /// <returns></returns>
        public static bool AddThirdPartyOrderChannellink(ThirdPartyOrderChannellinkModel linkmodel)
        {
            bool isSuccess = true;
            var result = 0;
            string source = string.Empty;
            string conn = ConnectionHelper.GetDecryptConn("ThirdParty");
            using (var db = new SqlDbHelper(conn))
            {
                db.BeginTransaction();
                string Sql = @"INSERT INTO Tuhu_thirdparty.[dbo].[tbl_ThirdPartyOrderChannellink]
                                        ( [OrderChanneID]
                                        ,[OrderChannelEngName]
                                        ,[BusinessType]
                                        ,[Status]
                                        ,[InitialPagelink]
                                        ,[FinalPagelink]
                                        ,[IsAggregatePage]
                                        ,[IsAuthorizedLogin]
                                        ,[IsPartnerReceivSilver]
                                        ,[IsOrderBack]      
                                        ,[IsViewOrders]
                                        ,[IsViewCoupons]
                                        ,[IsContactUserService]
                                        ,[IsBackTop]
                                        ,[CreateBy]
                                        ,[LastUpdateBy]
                                        ,[CreateDatetime]
                                        ,[LastUpdateDateTime]
                                        )
                                VALUES (    @OrderChanneID , 
                                            @OrderChannelEngName,
                                            @BusinessType ,
                                            @Status , 
                                            @InitialPagelink ,
                                            @FinalPagelink , 
                                            @IsAggregatePage ,
                                            @IsAuthorizedLogin , 
                                            @IsPartnerReceivSilver ,
                                            @IsOrderBack ,
                                            @IsViewOrders ,
                                            @IsViewCoupons ,
                                            @IsContactUserService ,
                                            @IsBackTop ,
                                            @CreateBy ,
                                            @LastUpdateBy ,
                                            GETDATE() ,
                                            GETDATE()
                                        )
                                SELECT  SCOPE_IDENTITY();";
                    var parameters = new[]
                    {
                        new SqlParameter("@OrderChanneID", linkmodel.OrderChanneID),
                        new SqlParameter("@OrderChannelEngName", linkmodel.OrderChannelEngName),
                        new SqlParameter("@BusinessType", linkmodel.BusinessType),
                        new SqlParameter("@Status", 1),
                        new SqlParameter("@InitialPagelink", linkmodel.InitialPagelink),
                        new SqlParameter("@FinalPagelink", linkmodel.FinalPagelink),
                        new SqlParameter("@IsAggregatePage", linkmodel.IsAggregatePage),
                        new SqlParameter("@IsAuthorizedLogin", linkmodel.IsAuthorizedLogin),
                        new SqlParameter("@IsPartnerReceivSilver", linkmodel.IsPartnerReceivSilver),
                        new SqlParameter("@IsOrderBack", linkmodel.IsOrderBack),
                        new SqlParameter("@IsViewOrders", linkmodel.IsViewOrders),
                        new SqlParameter("@IsViewCoupons", linkmodel.IsViewCoupons),
                        new SqlParameter("@IsContactUserService", linkmodel.IsContactUserService),
                        new SqlParameter("@IsBackTop", linkmodel.IsBackTop),
                        new SqlParameter("@CreateBy", linkmodel.CreateBy),
                        new SqlParameter("@LastUpdateBy", linkmodel.LastUpdateBy)
                    };
                    result = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, Sql, parameters));
                    if (result < 0)
                    {
                        db.Rollback();
                        isSuccess = false;
                    }
                    else
                    {
                        db.Commit();
                    }
                return isSuccess;
            }
        }
        #endregion
    }
}
