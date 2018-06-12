USE Gungnir;
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[BaoYangJob_SelectIncrementOrderHistory]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].BaoYangJob_SelectIncrementOrderHistory;
GO
/*=============================================================
--FuncDes:	获取保养
--Author			ModifyDate		Reason
--zhangjianfeng		2016-06-16		Create

EXEC Gungnir..BaoYangJob_SelectIncrementOrderHistory '2016-05-10',0, 2000
=============================================================*/
CREATE PROCEDURE [dbo].BaoYangJob_SelectIncrementOrderHistory
    @Time DATETIME ,
    @Offset INT ,
    @PageSize INT
AS
    SELECT  record.UserID ,
            record.VechileID ,
            record.RelatedOrderID ,
            record.IsDeleted ,
            ol.PID
    FROM    BaoYang..UserBaoYangRecords (NOLOCK) AS record
            JOIN Gungnir..tbl_OrderList (NOLOCK) AS ol ON ol.OrderID = record.RelatedOrderID
            JOIN Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) AS carpar ON carpar.PID = ol.PID COLLATE Chinese_PRC_CI_AS
    WHERE   record.RelatedOrderID IS NOT NULL
            AND ( BaoYangType = 'xby'
                  OR BaoYangType = 'dby'
                )
            AND ( record.CreatedDateTime >= @Time
                  OR record.UpdatedDateTime >= @Time
                )
            AND ol.Deleted = 0
            AND carpar.PrimaryParentCategory = 'Oil'
    ORDER BY record.UserID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

GO