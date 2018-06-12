USE Gungnir;
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[BaoYangJob_SelectBaoYangOrdersByPid]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].BaoYangJob_SelectBaoYangOrdersByPid;
GO
/*=============================================================
--FuncDes:	获取保养
--Author			ModifyDate		Reason
--zhangjianfeng		2016-06-16		Create

EXEC Gungnir..BaoYangJob_SelectBaoYangOrdersByPid 'OL-SH-HEPLUS-5W40-1|',1000, 2000
=============================================================*/
CREATE PROCEDURE [dbo].[BaoYangJob_SelectBaoYangOrdersByPid]
    @Pid NVARCHAR(513) ,
    @Offset INT ,
    @PageSize INT
AS
    SELECT  o.UserID AS UserId ,
            car.u_cartype_pid_vid AS VehicleId ,
            o.PKID AS OrderId
    FROM    Gungnir..tbl_OrderList (NOLOCK) AS ol
            JOIN Gungnir..tbl_Order (NOLOCK) AS o ON ol.OrderID = o.PKID
            JOIN Tuhu_profiles..CarObject (NOLOCK) AS car ON o.CarID = car.CarID
    WHERE   ol.PID = @Pid
            AND o.Status <> '7Canceled'
    ORDER BY o.PKID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
GO