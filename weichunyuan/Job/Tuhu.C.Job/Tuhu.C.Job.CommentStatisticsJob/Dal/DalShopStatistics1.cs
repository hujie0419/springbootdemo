﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using Tuhu.Service.Config;

namespace Tuhu.C.Job.CommentStatisticsJob.Dal
{
    class DalShopStatistics1
    {
        private static string DBName_ShopStatistics = "tbl_ShopStatistics";
        public static async Task<IEnumerable<ShopStatisticsModel>> GetShopStatisticsByTR()
        {
            #region sql
            const string sql = @"SELECT 	OS.ShopID,
		OS.Type,
		OS.InstallQuantity,
		OS.CommentTimes,
        OS.FavourableCount,
		(OS.CommentR2 * 0.4 + OS.CommentR3 * 0.3 + OS.CommentR4 * 0.3) AS CommentR1,
		OS.CommentR2,
		OS.CommentR3,
		OS.CommentR4,
		OS.ApplyCommentTimes,
		(OS.ApplyCommentRate2 * 0.4 + OS.ApplyCommentRate3 * 0.3 + OS.ApplyCommentRate4 * 0.3) AS ApplyCommentRate1,
		OS.ApplyCommentRate2,
		OS.ApplyCommentRate3,
		OS.ApplyCommentRate4
FROM	(SELECT	O.InstallShopID AS ShopID,
				'TR' AS Type,
				COUNT(1) AS InstallQuantity,
				NULLIF(COUNT(C.CommentId), 0) AS CommentTimes,
                sum(case when C.CommentR1>4.0 then 1 else 0 end )  as FavourableCount,
				SUM(ISNULL(C.CommentR2, C.CommentR1)) AS CommentR2,
				SUM(ISNULL(C.CommentR3, C.CommentR1)) AS CommentR3,
				SUM(ISNULL(C.CommentR4, C.CommentR1)) AS CommentR4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR2, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR2, C.CommentR1)*0.6 END ) AS ApplyCommentRate2,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR3, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR3, C.CommentR1)*0.6 END ) AS ApplyCommentRate3,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR4, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR4, C.CommentR1)*0.6 END ) AS ApplyCommentRate4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0  THEN  0.4  WHEN c.CreateTime>=DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0 THEN 0.6 ELSE  0 END ) AS ApplyCommentTimes
		 FROM	Gungnir..tbl_Order AS O WITH (NOLOCK)
		 LEFT JOIN Tuhu_comment..tbl_ShopComment AS C WITH (NOLOCK)
				ON C.CommentOrderId = O.PKID
				   AND C.InstallShopID = O.InstallShopID
				   AND C.CommentType = 1
				   AND C.CommentStatus IN (2, 3)
		             AND C.ShopTypeBU ='TR'
		 GROUP BY O.InstallShopID		 
		) AS OS
WHERE	OS.ShopID>0";
            #endregion

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 10 * 60;  
                //cmd.Parameters.AddWithValue("@Begin", (pageIndex - 1) * pageSize);
                //cmd.Parameters.AddWithValue("@PageSize", pageIndex * pageSize);
                return await DbHelper.ExecuteSelectAsync<ShopStatisticsModel>(true, cmd);
            }
        }
        public static async Task<IEnumerable<ShopStatisticsModel>> GetShopStatisticsByBY()
        {
            #region sql
            const string sql = @"SELECT 	OS.ShopID,
		OS.Type,
		OS.InstallQuantity,
		OS.CommentTimes,
        OS.FavourableCount,
		(OS.CommentR2 * 0.4 + OS.CommentR3 * 0.3 + OS.CommentR4 * 0.3) AS CommentR1,
		OS.CommentR2,
		OS.CommentR3,
		OS.CommentR4,
		OS.ApplyCommentTimes,
		(OS.ApplyCommentRate2 * 0.4 + OS.ApplyCommentRate3 * 0.3 + OS.ApplyCommentRate4 * 0.3) AS ApplyCommentRate1,
		OS.ApplyCommentRate2,
		OS.ApplyCommentRate3,
		OS.ApplyCommentRate4
FROM	(SELECT	O.InstallShopID AS ShopID,
				'BY' AS Type,
				COUNT(1) AS InstallQuantity,
				NULLIF(COUNT(C.CommentId), 0) AS CommentTimes,
                sum(case when C.CommentR1>4.0 then 1 else 0 end )  as FavourableCount,
				SUM(ISNULL(C.CommentR2, C.CommentR1)) AS CommentR2,
				SUM(ISNULL(C.CommentR3, C.CommentR1)) AS CommentR3,
				SUM(ISNULL(C.CommentR4, C.CommentR1)) AS CommentR4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR2, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR2, C.CommentR1)*0.6 END ) AS ApplyCommentRate2,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR3, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR3, C.CommentR1)*0.6 END ) AS ApplyCommentRate3,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR4, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR4, C.CommentR1)*0.6 END ) AS ApplyCommentRate4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0  THEN  0.4  WHEN c.CreateTime>=DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0 THEN 0.6 ELSE  0 END ) AS ApplyCommentTimes
		 FROM	Gungnir..tbl_Order AS O WITH (NOLOCK)
		 LEFT JOIN Tuhu_comment..tbl_ShopComment AS C WITH (NOLOCK)
				ON C.CommentOrderId = O.PKID
				   AND C.InstallShopID = O.InstallShopID
				   AND C.CommentType = 1
				   AND C.CommentStatus IN (2, 3)
		            AND C.ShopTypeBU ='BY'
		 GROUP BY O.InstallShopID		
		) AS OS
WHERE	OS.ShopID>0";
            #endregion

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 10 * 60;
                return await DbHelper.ExecuteSelectAsync<ShopStatisticsModel>(true, cmd);
            }
        }
        public static async Task<IEnumerable<ShopStatisticsModel>> GetShopStatisticsByMR()
        {
            #region sql
            const string sql = @"SELECT 	OS.ShopID,
		OS.Type,
		OS.InstallQuantity,
		OS.CommentTimes,
        OS.FavourableCount,
		(OS.CommentR2 * 0.4 + OS.CommentR3 * 0.3 + OS.CommentR4 * 0.3) AS CommentR1,
		OS.CommentR2,
		OS.CommentR3,
		OS.CommentR4,
		OS.ApplyCommentTimes,
		(OS.ApplyCommentRate2 * 0.4 + OS.ApplyCommentRate3 * 0.3 + OS.ApplyCommentRate4 * 0.3) AS ApplyCommentRate1,
		OS.ApplyCommentRate2,
		OS.ApplyCommentRate3,
		OS.ApplyCommentRate4
FROM	(SELECT	O.InstallShopID AS ShopID,
				CAST('MR' AS CHAR(2)) AS Type,
				COUNT(1) AS InstallQuantity,
				NULLIF(COUNT(C.CommentId), 0) AS CommentTimes,
                sum(case when C.CommentR1>4.0 then 1 else 0 end )  as FavourableCount,
				SUM(ISNULL(C.CommentR2, C.CommentR1)) AS CommentR2,
				SUM(ISNULL(C.CommentR3, C.CommentR1)) AS CommentR3,
				SUM(ISNULL(C.CommentR4, C.CommentR1)) AS CommentR4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR2, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR2, C.CommentR1)*0.6 END ) AS ApplyCommentRate2,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR3, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR3, C.CommentR1)*0.6 END ) AS ApplyCommentRate3,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR4, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR4, C.CommentR1)*0.6 END ) AS ApplyCommentRate4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0  THEN  0.4  WHEN c.CreateTime>=DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0 THEN 0.6 ELSE  0 END ) AS ApplyCommentTimes
		 FROM	Gungnir..tbl_Order AS O WITH (NOLOCK)
		 LEFT JOIN Tuhu_comment..tbl_ShopComment AS C WITH (NOLOCK)
				ON C.CommentOrderId = O.PKID
				   AND C.InstallShopID = O.InstallShopID
				   AND C.CommentType = 1
				   AND C.CommentStatus IN (2, 3)
		            AND C.ShopTypeBU ='MR'
		 GROUP BY O.InstallShopID
		 ) AS OS
WHERE	OS.ShopID>0";
            #endregion

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 10 * 60;
                //cmd.Parameters.AddWithValue("@Begin", (pageIndex - 1) * pageSize);
                //cmd.Parameters.AddWithValue("@PageSize", pageIndex * pageSize);
                return await DbHelper.ExecuteSelectAsync<ShopStatisticsModel>(true, cmd);
            }
        }
        public static async Task<IEnumerable<ShopStatisticsModel>> GetShopStatisticsByPQ()
        {
            #region sql
            const string sql = @"SELECT 	OS.ShopID,
		OS.Type,
		OS.InstallQuantity,
		OS.CommentTimes,
        OS.FavourableCount,
		(OS.CommentR2 * 0.4 + OS.CommentR3 * 0.3 + OS.CommentR4 * 0.3) AS CommentR1,
		OS.CommentR2,
		OS.CommentR3,
		OS.CommentR4,
		OS.ApplyCommentTimes,
		(OS.ApplyCommentRate2 * 0.4 + OS.ApplyCommentRate3 * 0.3 + OS.ApplyCommentRate4 * 0.3) AS ApplyCommentRate1,
		OS.ApplyCommentRate2,
		OS.ApplyCommentRate3,
		OS.ApplyCommentRate4
FROM	(SELECT	O.InstallShopID AS ShopID,
				'PQ' AS Type,
				COUNT(1) AS InstallQuantity,
				NULLIF(COUNT(C.CommentId), 0) AS CommentTimes,
                sum(case when C.CommentR1>4.0 then 1 else 0 end )  as FavourableCount,
				SUM(ISNULL(C.CommentR2, C.CommentR1)) AS CommentR2,
				SUM(ISNULL(C.CommentR3, C.CommentR1)) AS CommentR3,
				SUM(ISNULL(C.CommentR4, C.CommentR1)) AS CommentR4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR2, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR2, C.CommentR1)*0.6 END ) AS ApplyCommentRate2,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR3, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR3, C.CommentR1)*0.6 END ) AS ApplyCommentRate3,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR4, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR4, C.CommentR1)*0.6 END ) AS ApplyCommentRate4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0  THEN  0.4  WHEN c.CreateTime>=DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0 THEN 0.6 ELSE  0 END ) AS ApplyCommentTimes
		 FROM	Gungnir..tbl_Order AS O WITH (NOLOCK)
		 LEFT JOIN Tuhu_comment..tbl_ShopComment AS C WITH (NOLOCK)
				ON C.CommentOrderId = O.PKID
				   AND C.InstallShopID = O.InstallShopID
				   AND C.CommentType = 1
				   AND C.CommentStatus IN (2, 3)
                     AND C.ShopTypeBU ='PQ'
		 GROUP BY O.InstallShopID
		 ) AS OS
WHERE	OS.ShopID>0";
            #endregion

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 10 * 60;
                //cmd.Parameters.AddWithValue("@Begin", (pageIndex - 1) * pageSize);
                //cmd.Parameters.AddWithValue("@PageSize", pageIndex * pageSize);
                return await DbHelper.ExecuteSelectAsync<ShopStatisticsModel>(true, cmd);
            }
        }
        public static async Task<IEnumerable<ShopStatisticsModel>> GetShopStatisticsByFW()
        {
            #region sql
            const string sql = @"SELECT 	OS.ShopID,
		OS.Type,
		OS.InstallQuantity,
		OS.CommentTimes,
		(OS.CommentR2 * 0.4 + OS.CommentR3 * 0.3 + OS.CommentR4 * 0.3) AS CommentR1,
		OS.CommentR2,
		OS.CommentR3,
		OS.CommentR4,
		OS.ApplyCommentTimes,
		(OS.ApplyCommentRate2 * 0.4 + OS.ApplyCommentRate3 * 0.3 + OS.ApplyCommentRate4 * 0.3) AS ApplyCommentRate1,
		OS.ApplyCommentRate2,
		OS.ApplyCommentRate3,
		OS.ApplyCommentRate4
FROM	(SELECT	O.InstallShopID AS ShopID,
				CAST('FW' AS CHAR(2)) AS Type,
				COUNT(1) AS InstallQuantity,
				NULLIF(COUNT(C.CommentId), 0) AS CommentTimes,
                sum(case when C.CommentR1>4.0 then 1 else 0 end )  as FavourableCount,
				SUM(ISNULL(C.CommentR2, C.CommentR1)) AS CommentR2,
				SUM(ISNULL(C.CommentR3, C.CommentR1)) AS CommentR3,
				SUM(ISNULL(C.CommentR4, C.CommentR1)) AS CommentR4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR2, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR2, C.CommentR1)*0.6 END ) AS ApplyCommentRate2,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR3, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR3, C.CommentR1)*0.6 END ) AS ApplyCommentRate3,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) THEN  ISNULL(C.CommentR4, C.CommentR1)*0.4  ELSE ISNULL(C.CommentR4, C.CommentR1)*0.6 END ) AS ApplyCommentRate4,
				SUM(CASE  WHEN c.CreateTime<DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0  THEN  0.4  WHEN c.CreateTime>=DATEADD(MONTH,-1,GETDATE()) AND C.CommentId>0 THEN 0.6 ELSE  0 END ) AS ApplyCommentTimes
		 FROM	tbl_Order AS O WITH (NOLOCK)
		 LEFT JOIN Tuhu_comment..tbl_ShopComment AS C WITH (NOLOCK)
				ON C.CommentOrderId = O.PKID
				   AND C.InstallShopID = O.InstallShopID
				   AND C.CommentType = 1
				   AND C.CommentStatus IN (2, 3)
		   AND C.ShopTypeBU ='FW'
		 GROUP BY O.InstallShopID
		 ) AS OS
WHERE	OS.ShopID>0";
            #endregion

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 10 * 60;
                //cmd.Parameters.AddWithValue("@Begin", (pageIndex - 1) * pageSize);
                //cmd.Parameters.AddWithValue("@PageSize", pageIndex * pageSize);
                return await DbHelper.ExecuteSelectAsync<ShopStatisticsModel>(true, cmd);
            }
        }


        /// <summary>
        /// 获取 所以门店的 默认好评数 【分组 统计】
        /// </summary>
        /// <returns></returns>
        public static List<ShopDefaultFavourableStatisticsModel> GetAllProductDefaultFavourableStatistics()
        {
            string sql = "";
            List<ShopDefaultFavourableStatisticsModel> result = new List<ShopDefaultFavourableStatisticsModel>();
            foreach (ShopStatisticsType item in Enum.GetValues(typeof(ShopStatisticsType)))
            {
                #region 根据类型 选择sql
                switch (item)
                {
                    #region TR
                    case ShopStatisticsType.TR:
                        sql = @"select  OCS.ShopId ,'TR' AS Type,COUNT(1) as DefaultFavourableCount
                    from Tuhu_comment..OrderCommentStatus OCS with(nolock) 
                    LEFT JOIN Gungnir..tbl_Order  AS O WITH (NOLOCK)
				    ON O.PKID = OCS.OrderId
		            WHERE	
				        OCS.CanComment=1 and OCS.CreateDateTime < DATEADD(month, @month, GETDATE()) and OCS.ShopId is not null
				        and O.Status <> '7Canceled'
				        AND O.Status <> '0New'
				        AND O.OrderType <> N'6美容'
				        AND O.OrderType <> N'10服务'
				        AND O.OrderType <> N'19美容团购'
				        AND EXISTS ( SELECT	1
							            FROM	Gungnir..tbl_OrderList AS OL WITH (NOLOCK)
							            WHERE	OL.OrderID = O.PKID
									        AND OL.Deleted = 0
									        AND (OL.PID LIKE 'TR-%'
										            OR OL.PID LIKE 'LG-%'
										        ) )
		            GROUP BY OCS.ShopId		 ";
                        break;
                    #endregion
                    #region BY
                    case ShopStatisticsType.BY:
                        sql = @"with tmp
                                as (select OL.OrderID
                                    from Gungnir..tbl_OrderList as OL with (nolock)
                                    where OL.Deleted = 0
                                          and OL.PID in (   select CP.ProductID + '|' + CP.VariantID collate Chinese_PRC_CI_AS
                                                            from Tuhu_productcatalog..CarPAR_CatalogHierarchy as CH with (nolock)
                                                                join Tuhu_productcatalog..CarPAR_CatalogProducts as CP with (nolock)
                                                                    on CH.child_oid = CP.oid
                                                                       and CP.i_ClassType in ( 2, 4 )
                                                            where CH.NodeNo like '28656.%' ))
                                select OCS.ShopId,
                                       'BY' as Type,
                                       COUNT(1) as DefaultFavourableCount
                                from Tuhu_comment..OrderCommentStatus OCS with (nolock)
                                    inner join Gungnir..tbl_Order as O with (nolock)
                                        on O.PKID = OCS.OrderId
                                    inner join tmp
                                        on O.PKID = tmp.OrderID
                                where OCS.CanComment = 1
                                      and OCS.CreateDateTime < DATEADD(month, @month, GETDATE())
                                      and OCS.ShopId is not null
                                      and O.Status <> '7Canceled'
                                      and O.Status <> '0New'
                                      and O.OrderType <> N'6美容'
                                      and O.OrderType <> N'10服务'
                                      and O.OrderType <> N'19美容团购'
                                group by OCS.ShopId;";
                        break;
                    #endregion
                    #region MR
                    case ShopStatisticsType.MR:
                        sql = @"select OCS.ShopId ,'MR' AS Type,COUNT(1) as DefaultFavourableCount
                            from Tuhu_comment..OrderCommentStatus OCS with(nolock) 
                            LEFT JOIN Gungnir..tbl_Order  AS O WITH (NOLOCK)
                            ON O.PKID = OCS.OrderId
                            WHERE	OCS.CanComment=1 and OCS.CreateDateTime < DATEADD(month, @month, GETDATE()) and OCS.ShopId is not null
	                            and O.Status = '3Installed'
	                            AND (O.OrderType = N'6美容' OR O.OrderType=N'19美容团购')
                            GROUP BY OCS.ShopId		 ";
                        break;
                    #endregion
                    #region PQ
                    case ShopStatisticsType.PQ:
                        sql = @"select OCS.ShopId ,'PQ' AS Type,COUNT(1) as DefaultFavourableCount
                            from Tuhu_comment..OrderCommentStatus OCS with(nolock) 
                            LEFT JOIN Gungnir..tbl_Order  AS O WITH (NOLOCK)
                            ON O.PKID = OCS.OrderId
                            WHERE	OCS.CanComment=1 and OCS.CreateDateTime < DATEADD(month, @month, GETDATE()) and OCS.ShopId is not null
	                            and O.Status <> '7Canceled'
				                            AND O.Status <> '0New'
				                            AND (O.OrderType = N'10服务' OR O.OrderType = N'32钣喷服务')
				                            AND EXISTS ( SELECT TOP 1
									                            1
							                             FROM	Gungnir..tbl_OrderList AS OL WITH (NOLOCK)
							                             WHERE	OL.OrderID = O.PKID
									                            AND OL.Deleted = 0
									                            AND OL.PID IN ('FU-PQXB-JKQ|1','FU-PQXB-GCQ|1','FU-PQXB-BZQ|1','FU-PQXB-BZQ|2',
													                            'FU-PQXB-BZQ|3','FU-PQXB-BZQ|4','FU-PQXB-BZQ|5','FU-PQXB-GDQ|1',
													                            'FU-PQXB-GDQ|2','FU-PQXB-GDQ|3','FU-PQXB-GDQ|4','FU-PQXB-GDQ|5')
							                            )
                            GROUP BY OCS.ShopId";
                        break;
                    #endregion
                    #region FW
                    case ShopStatisticsType.FW:
                        sql = @"select OCS.ShopId ,'FW' AS Type,COUNT(1) as DefaultFavourableCount
                            from Tuhu_comment..OrderCommentStatus OCS with(nolock) 
                            LEFT JOIN Gungnir..tbl_Order  AS O WITH (NOLOCK)
                            ON O.PKID = OCS.OrderId
                            WHERE	OCS.CanComment=1 and OCS.CreateDateTime < DATEADD(month,@month, GETDATE()) and OCS.ShopId is not null
	                            and O.Status = '3Installed'
	                            	AND O.OrderType = N'10服务'
                            GROUP BY OCS.ShopId";
                        break;
                    #endregion
                    default: break;
                }
                #endregion
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 10 * 60;
                    cmd.Parameters.AddWithValue("@month", -1 * CommonUtilsModel.CanComment);
                    List<ShopDefaultFavourableStatisticsModel> data = DbHelper.ExecuteSelect<ShopDefaultFavourableStatisticsModel>(cmd).ToList() ?? new List<ShopDefaultFavourableStatisticsModel>();
                    result.AddRange(data);
                }
            }
            return result;
        }

        public static IEnumerable<ShopStatisticsModel> GetShopStatisticByShopIds(int shopId)
        {
            string DBName = GetSwitchValue() ? DBName_ShopStatistics + "_temp" : DBName_ShopStatistics;
            #region sql
            string sql = @"SELECT * FROM  [Gungnir].[dbo].[" + DBName + @"] WITH(NOLOCK) WHERE ShopId=@ShopId";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                //cmd.Parameters.AddWithValue("@DBName", DBName);
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                return DbHelper.ExecuteSelect<ShopStatisticsModel>(true, cmd);
            }

        }
        public static bool UpdateShopStatistic(ShopStatisticsModel model)
        {
            string DBName = GetSwitchValue() ? DBName_ShopStatistics + "_temp" : DBName_ShopStatistics;
            #region sql
            string sql = @"UPDATE  [Gungnir].[dbo].[" + DBName + @"] WITH(ROWLOCK) 
SET InstallQuantity =@InstallQuantity,
    CommentTimes = @CommentTimes,
	CommentR1 = @CommentR1,
	CommentR2 = @CommentR2,
	CommentR3 = @CommentR3,
	CommentR4 = @CommentR4,
    ApplyCommentTimes=@ApplyCommentTimes,
    ApplyCommentRate1=@ApplyCommentRate1,
    ApplyCommentRate2=@ApplyCommentRate2,
    ApplyCommentRate3=@ApplyCommentRate3,
    ApplyCommentRate4=@ApplyCommentRate4,
    LastUpdateTime=getdate(),
FavourableCount = @FavourableCount,
DefaultFavourableCount = @DefaultFavourableCount,
Score = @Score,
CommentTimesB = @CommentTimesB,
CommentR1B = @CommentR1B,
CommentR2B = @CommentR2B,
CommentR3B = @CommentR3B,
CommentR4B = @CommentR4B
WHERE ShopId=@ShopId  and Type=@Type";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                //cmd.Parameters.AddWithValue("@DBName", DBName);

                cmd.Parameters.AddWithValue("@InstallQuantity", model.InstallQuantity);
                cmd.Parameters.AddWithValue("@CommentTimes", model.CommentTimes);
                cmd.Parameters.AddWithValue("@CommentR1", model.CommentR1);
                cmd.Parameters.AddWithValue("@CommentR2", model.CommentR2);
                cmd.Parameters.AddWithValue("@CommentR3", model.CommentR3);
                cmd.Parameters.AddWithValue("@CommentR4", model.CommentR4);
                cmd.Parameters.AddWithValue("@ApplyCommentTimes", model.ApplyCommentTimes);
                cmd.Parameters.AddWithValue("@ApplyCommentRate1", model.ApplyCommentRate1);
                cmd.Parameters.AddWithValue("@ApplyCommentRate2", model.ApplyCommentRate2);
                cmd.Parameters.AddWithValue("@ApplyCommentRate3", model.ApplyCommentRate3);
                cmd.Parameters.AddWithValue("@ApplyCommentRate4", model.ApplyCommentRate4);

                cmd.Parameters.AddWithValue("@FavourableCount", model.FavourableCount);
                cmd.Parameters.AddWithValue("@DefaultFavourableCount", model.DefaultFavourableCount);
                cmd.Parameters.AddWithValue("@Score", model.Score);
                cmd.Parameters.AddWithValue("@CommentTimesB", model.CommentTimesB);
                cmd.Parameters.AddWithValue("@CommentR1B", model.CommentR1B);
                cmd.Parameters.AddWithValue("@CommentR2B", model.CommentR2B);
                cmd.Parameters.AddWithValue("@CommentR3B", model.CommentR3B);
                cmd.Parameters.AddWithValue("@CommentR4B", model.CommentR4B);

                cmd.Parameters.AddWithValue("@ShopId", model.ShopID);
                cmd.Parameters.AddWithValue("@Type", model.Type);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }

        }
        public static bool InsertShopStatistic(ShopStatisticsModel model)
        {
            string DBName = GetSwitchValue() ? DBName_ShopStatistics + "_temp" : DBName_ShopStatistics;
            #region sql
            string sql = @"Insert into  [Gungnir].[dbo].[" + DBName + @"]
(ShopId,
Type,InstallQuantity,CommentTimes,CommentR1, CommentR2,CommentR3, CommentR4,
ApplyCommentTimes,ApplyCommentRate1,ApplyCommentRate2,ApplyCommentRate3,ApplyCommentRate4,
FavourableCount,DefaultFavourableCount,Score,CommentTimesB,CommentR1B,CommentR2B,CommentR3B,CommentR4B,CreateTime
)
values(
@ShopId,@Type,@InstallQuantity,@CommentTimes,@CommentR1, @CommentR2,@CommentR3, @CommentR4,
@ApplyCommentTimes,@ApplyCommentRate1,@ApplyCommentRate2,@ApplyCommentRate3,@ApplyCommentRate4,
@FavourableCount,@DefaultFavourableCount,@Score,@CommentTimesB,@CommentR1B,@CommentR2B,@CommentR3B,@CommentR4B,getdate()
)";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                //cmd.Parameters.AddWithValue("@DBName", DBName);

                cmd.Parameters.AddWithValue("@InstallQuantity", model.InstallQuantity);
                cmd.Parameters.AddWithValue("@CommentTimes", model.CommentTimes);
                cmd.Parameters.AddWithValue("@CommentR1", model.CommentR1);
                cmd.Parameters.AddWithValue("@CommentR2", model.CommentR2);
                cmd.Parameters.AddWithValue("@CommentR3", model.CommentR3);
                cmd.Parameters.AddWithValue("@CommentR4", model.CommentR4);
                cmd.Parameters.AddWithValue("@ApplyCommentTimes", model.ApplyCommentTimes);
                cmd.Parameters.AddWithValue("@ApplyCommentRate1", model.ApplyCommentRate1);
                cmd.Parameters.AddWithValue("@ApplyCommentRate2", model.ApplyCommentRate2);
                cmd.Parameters.AddWithValue("@ApplyCommentRate3", model.ApplyCommentRate3);
                cmd.Parameters.AddWithValue("@ApplyCommentRate4", model.ApplyCommentRate4);
                cmd.Parameters.AddWithValue("@ShopId", model.ShopID);
                cmd.Parameters.AddWithValue("@Type", model.Type);

                cmd.Parameters.AddWithValue("@FavourableCount", model.FavourableCount);
                cmd.Parameters.AddWithValue("@DefaultFavourableCount", model.DefaultFavourableCount);
                cmd.Parameters.AddWithValue("@Score", model.Score);
                cmd.Parameters.AddWithValue("@CommentTimesB", model.CommentTimesB);
                cmd.Parameters.AddWithValue("@CommentR1B", model.CommentR1B);
                cmd.Parameters.AddWithValue("@CommentR2B", model.CommentR2B);
                cmd.Parameters.AddWithValue("@CommentR3B", model.CommentR3B);
                cmd.Parameters.AddWithValue("@CommentR4B", model.CommentR4B);

                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }

        }

        public static IEnumerable<ShopsModel> GetShops()
        {
            const string sql = @" SELECT PKID FROM [Gungnir].[dbo].[Shops] WITH(nolock) ";
            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteSelect<ShopsModel>(true, cmd);
            }

        }

        public static IEnumerable<ShopsModel> GetShopsPage(int pageIndex, int pageSize)
        {
            using (var cmd = new SqlCommand($@"SELECT PKID  FROM [Gungnir].[dbo].[Shops] WITH(nolock) where IsActive=1 ORDER BY PKID ASC
                                                OFFSET {(pageIndex - 1) * pageSize} ROW
                                                FETCH NEXT {pageSize} ROWS ONLY"))
            {
                return DbHelper.ExecuteSelect<ShopsModel>(true, cmd);
            }
        }
        public static int GetShopsCount()
        {
            using (var cmd = new SqlCommand($@"SELECT COUNT(1) FROM [Gungnir].[dbo].[Shops] WITH(nolock)"))
            {
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }
        public static IEnumerable<TechModel> GetTechsPage(int pageIndex, int pageSize)
        {
            using (var cmd = new SqlCommand($@"SELECT PKID,ShopID FROM Gungnir.dbo.ShopEmployee WITH(nolock) ORDER BY PKID ASC
                                                OFFSET {(pageIndex - 1) * pageSize} ROW
                                                FETCH NEXT {pageSize} ROWS ONLY"))
            {
                return DbHelper.ExecuteSelect<TechModel>(true, cmd);
            }
        }
        public static int GetTechsCount()
        {
            using (var cmd = new SqlCommand($@"SELECT COUNT(1) FROM Gungnir.dbo.ShopEmployee WITH(nolock)"))
            {
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }


        public static int SetTotalPoints()
        {
            const string sqlStr = @"
WITH    CommentRate
          AS ( SELECT   ShopId ,
                        AVG(ApplyCommentRate1) AS rate
               FROM     [Gungnir].[dbo].[tbl_ShopStatistics] WITH ( NOLOCK )
               WHERE    Type IN ( N'BY', N'FW', N'MR', N'TR' )
                        AND ApplyCommentRate1 > 0
               GROUP BY ShopId
             )
    UPDATE  T
    SET     ApplyCommentRate1 = S.rate
    FROM    [Gungnir].[dbo].[tbl_ShopStatistics] AS T WITH ( ROWLOCK )
            INNER JOIN CommentRate AS S ON T.ShopId = S.ShopId
    WHERE   T.Type = N'ALL';";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }


        private static bool GetSwitchValue()
        {
            using (var client = new ConfigClient())
            {
                var result = client.GetOrSetRuntimeSwitch("UpdateShopStatisticsIsTest");
                return result.Success ? result.Result.Value : false;
            }

        }

        #region 门店评论 分类规则
        public async static Task<List<ShopCommentTypeRule>> GetShopCommentTypeRule()
        {
            string sql = @"SELECT  [PKID]
                                  ,[ShopType]
                                  ,[OrderStatus]
                                  ,[OrderType]
                                  ,[ProductType]
                                  ,[CreateBy]
                                  ,[CreateDateTime]
                                  ,[UpdateBy]
                                  ,[LastUpdateDateTime]
                                  ,[PIDPrefix]
                                  ,[ShopCommentTypeID]
                                    FROM Tuhu_comment.dbo.ShopCommentTypeRule with (nolock)
                            ";
            List<ShopCommentTypeRule> result = new List<ShopCommentTypeRule>();
            using (var cmdCount = new SqlCommand(sql))
            {
                var temp = await DbHelper.ExecuteSelectAsync<ShopCommentTypeRule>(true, cmdCount);
                result = temp.ToList();
                return result;
            }
        }

        #endregion
    }
}
