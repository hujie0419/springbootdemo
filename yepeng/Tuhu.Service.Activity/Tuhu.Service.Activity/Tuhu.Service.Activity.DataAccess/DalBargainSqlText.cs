using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalBargainSqlText
    {
        #region GetAllBargainProduct
        internal const string sql4GetAllBargainProduct = @"
SELECT  PKID AS ActivityProductId ,
        PID ,ProductType,ShowBeginTime, 
        productName AS ProductName ,
        pagename ,
        SuccessfulHint ,
        OriginalPrice ,
        FinalPrice ,
        Image1 ,
        Times ,
        Sequence ,
        BeginDateTime ,
        EndDateTime ,
        WXShareTitle ,
        APPShareId ,
        CASE WHEN CurrentStockCount > 0 THEN 1
             ELSE 0
        END AS HasStock
FROM    Configuration..BargainProduct WITH ( NOLOCK )
WHERE  ISNULL(ShowBeginTime,BeginDateTime)<=GETDATE();";

        internal const string sql4GetAllBargainProductCount = @"SELECT  COUNT(1)
FROM    Configuration..BargainProduct WITH ( NOLOCK )
WHERE   BeginDateTime < GETDATE()
        AND EndDateTime > GETDATE();";
        #endregion
        #region GetBargainProductByUser
        internal const string sql4GetBargainProductByUser = @"SELECT  BP.PKID AS ActivityProductId ,
        BP.PID ,
        BP.productName AS ProductName,
        BP.pageName ,
        BP.SuccessfulHint ,
        BP.OriginalPrice ,
        BP.FinalPrice ,
        BP.Image1 ,
        BP.Times ,
        BP.Sequence ,
        BP.BeginDateTime ,
        BP.EndDateTime ,
        BP.APPShareId ,
        BP.WXShareTitle ,
        CASE WHEN CurrentStockCount > 0 THEN 1
             ELSE 0
        END AS HasStock ,
        BO.IsOver ,
        IsPurchased,CurrentStockCount 
FROM    Activity..BargainOwnerAction AS BO WITH ( NOLOCK )
        LEFT JOIN Configuration..BargainProduct AS BP WITH ( NOLOCK ) ON BO.ActivityProductId = BP.PKID
WHERE   BO.OwnerId = @userid
        AND BO.IsOver = 1
        OR DATEADD(HOUR, 1, BP.BeginDateTime) > GETDATE();
";
        #endregion
        #region FetchBargainProductHistory
        internal const string sql4FetchBargainHistory = @"
select BU.UserId,
       BU.Reduce,
       BO.TotalReduce,
       BU.CreateDateTime as BargainTime,
       CONVERT(DECIMAL(18, 2), BU.Reduce / IIF(BO.Average > 0, BO.Average, BU.Reduce)) as Rate
from Activity..BargainOwnerAction as BO with (nolock)
    left join Activity..BargainShareAction as BU with (nolock)
        on BO.PKID = BU.ParentId
where BO.OwnerId = @userId
      and BO.ActivityProductId = @apId
      and BO.PID = @pid
      and BO.Status = 1
      and BU.PKID is not null;";
        #endregion

        #region FetchCurrentBargainData
        internal const string sql4FetchCurrentBargainData = @"select top 1
       BP.EndDateTime,
       BP.PID,
       BP.productName as ProductName,
       BO.PKID,
       BP.OriginalPrice,
       BP.SuccessfulHint,
       BO.OwnerId,
       BO.Average,
       BP.FinalPrice,
       BO.IsOver,
       BO.TotalReduce as CurrentRedece,
       BP.Times as TotalCount,
       BO.CurrentCount,
       BP.TotalStockCount,
       BP.CurrentStockCount,
       BP.ProductType,
       BP.SimpleDisplayName
from Activity..BargainOwnerAction as BO with (nolock)
    join Configuration..BargainProduct as BP with (nolock)
        on BO.ActivityProductId = BP.PKID
where BO.idKey = @idKey;";
        #endregion
        #region CheckBargainShare
        internal const string sql4checkBargainShare = @"
select BO.idKey,
       BS.Reduce
from Activity..BargainShareAction as BS with (nolock)
    left join Activity..BargainOwnerAction as BO with (nolock)
        on BO.PKID = BS.ParentId
where BS.UserId = @userId
      and BO.idKey = @idkey;";
        #endregion
        #region AddShareBargainAction
        internal const string sql4insertBargainShareIdAction = @"INSERT  INTO Activity..BargainShareAction
        ( ActivityProductId ,
          ParentId ,
          UserId ,
          Step ,
          Reduce
        )
VALUES  ( @apId ,
          @parentId ,
          @userId ,
          @step ,
          @reduce
        );";
        internal const string sql4updateBargainOwnerData = @"UPDATE  Activity..BargainOwnerAction WITH ( ROWLOCK )
SET     IsOver = @isover ,
        CurrentCount = CurrentCount+1 ,
        TotalReduce = @totalreduce ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @pkid;";
        internal const string sql4updateBargainConfigure = @"UPDATE  Configuration..BargainProduct WITH ( ROWLOCK )
SET     CurrentStockCount = IIF(CurrentStockCount > 0, CurrentStockCount - 1, 0) ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @apId;";
        #endregion
        #region CheckBargainProductStatus
        internal const string sql4checekBargainShareStatus = @"select case
           when BP.EndDateTime < GETDATE() then
               3
           when BP.CurrentStockCount < 1 then
               4
           when BP.BeginDateTime > GETDATE() then
               5
           else
               1
       end as Code
from Configuration..BargainProduct as BP with (nolock)
where BP.PKID = @apId
      and BP.PID = @pid;";
        internal const string sql4CheckUserShareBargain = @"SELECT TOP 1  idKey AS IdKey,1 AS Code
FROM    Activity..BargainOwnerAction WITH ( NOLOCK )
WHERE   ActivityProductId = @apid
        AND OwnerId = @ownerid";

        internal const string sql4checkBargainProductStatus = @"select case
           when DATEADD(day, 1, BP.EndDateTime) < GETDATE() then
               2
           when BO.IsPurchased = 1 then
               3
           else
               1
       end as Code,
       BO.idKey
from Activity..BargainOwnerAction as BO with (nolock)
    left join Configuration..BargainProduct as BP with (nolock)
        on BO.ActivityProductId = BP.PKID
where BP.PKID = @apId
      and BO.OwnerId = @ownerId
      and BO.IsOver = 1
      and BP.PID = @pid;";

        internal const string sql4checkBargainProductStatusByPid = @"SELECT  COUNT(1)
FROM    Activity..BargainOwnerAction AS BO WITH ( NOLOCK )
        LEFT JOIN Configuration..BargainProduct AS BP WITH ( NOLOCK ) ON BO.ActivityProductId = BP.PKID
WHERE   DATEADD(DAY, 1, BP.EndDateTime) > GETDATE()
        AND BP.BeginDateTime < GETDATE()
        AND BP.PID = @pid
        AND BO.OwnerId = @ownerId
        AND BO.IsOver = 1
        AND BO.IsPurchased = 0;";
        #endregion

        #region AddShareBargain

        internal const string sql4insertShareBargain = @"insert into Activity..BargainOwnerAction
(
    idKey,
    ActivityProductId,
    OwnerId,
    IsOver,
    IsPurchased,
    CurrentCount,
    TotalReduce,
    FinalPrice,
    PID,
    Average,
    Status
)
select @idkey,
       @apId,
       @ownerId,
       0,
       0,
       0,
       0,
       FinalPrice,
       PID,
       CONVERT(DECIMAL(18, 2), (OriginalPrice - FinalPrice) / Times),
       @status
from Configuration..BargainProduct with (nolock)
where PKID = @apId
      and PID = @pid;";
        #endregion
        #region FetchShareBargainInfo
        internal const string sql4fetchShareBargainInfo = @"SELECT  CASE WHEN BP.CurrentStockCount < 1 THEN 2
             WHEN BO.IsOver = 1 THEN 3
             WHEN BP.EndDateTime < GETDATE() THEN 4
             ELSE 1
        END AS Code ,
        BP.PageName ,
        BP.PKID AS ActivityProductId ,
        BO.OwnerId ,
        BO.idKey AS IdKey,
        BO.IsPurchased ,
        BP.PID ,
        BP.productName AS ProductName,
        BP.OriginalPrice ,
        BP.FinalPrice ,
        BP.Image1 ,
        BP.Times ,
        BP.BeginDateTime ,
        BP.EndDateTime
FROM    Activity..BargainOwnerAction AS BO WITH ( NOLOCK )
        LEFT JOIN Configuration..BargainProduct AS BP WITH ( NOLOCK ) ON BO.ActivityProductId = BP.PKID
WHERE   BO.idKey = @idkey;";
        #endregion
        #region BuyBargainProductAsync
        internal const string sql4updateBargainOwnerAction = @"update T
set T.IsPurchased = 1,
    OrderId = @OrderId
output Inserted.ActivityProductId
from Activity..BargainOwnerAction as T
    left join Configuration..BargainProduct as BP with (nolock)
        on T.ActivityProductId = BP.PKID
where T.OwnerId = @owner
      and BP.PID = @pid
      and BP.BeginDateTime < GETDATE()
      and DATEADD(day, 1, BP.EndDateTime) > GETDATE();";
        #endregion

        #region GetBackgroundStyle
        internal const string sql4fetchbackgroundstyle = @"select top 1
       BackgroundTheme as Style,
       BackgroundImage as ImgUrl,
       Title,
       QACount,
       QAData,
       WXAPPListShareText,
       WXAPPListShareImg,
       WXAPPDetailShareText,
       APPListShareTag,
       AppDetailShareTag,
       SliceShowText
from Configuration..BargainGlobalConfig with (nolock)
order by LastUpdateDateTime desc;";
        #endregion

        #region FetchBargainPrice
        internal const string sql4fetchBargainPrice = @"SELECT  TOP 1
        BO.FinalPrice
FROM    Configuration..BargainProduct AS BP WITH ( NOLOCK )
        JOIN Activity..BargainOwnerAction AS BO WITH ( NOLOCK ) ON BP.PKID = BO.ActivityProductId
WHERE   BP.BeginDateTime < GETDATE()
        AND BP.EndDateTime > DATEADD(DAY, -1, GETDATE())
        AND BP.PID = @pid
        AND BO.OwnerId = @ownerid;";
        #endregion
        #region SelectProductActivityPrice
        internal const string sql4selectProductActivityPrice = @"SELECT  1 AS Code ,
        FSP.PID AS PID ,
        FSP.Price AS ActivityPrice ,
        FSP.IsUsePCode AS ApplyCoupon
FROM    ( SELECT    Item
          FROM      Tuhu_productcatalog..SplitString(@pids, ',', 1)
        ) AS T
        LEFT JOIN Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FSP.PID = T.Item
WHERE   FSP.ActivityID = @activityId;";
        #endregion

        #region FetchBargainProductItemByIdKey
        internal const string sql4FetchBargainProductItemByIdKey = @"
select BO.PID,
       BO.ActivityProductId,
       BO.OwnerId
from Activity..BargainOwnerAction as BO with (nolock)
where idKey = @idkey";
        #endregion

        #region SelectBargainProductItems
        internal const string sql4SelectBargainProductItems = @"SELECT  PKID AS ActivityProductId ,
        PID ,
        CASE WHEN CurrentStockCount < 1 THEN 0
             ELSE 1
        END AS HasStock ,
		Sequence,
        BeginDateTime ,
        EndDateTime,
		ISNULL(ShowBeginTime,BeginDateTime) ShowBeginTime,
		ProductType 
FROM    Configuration..BargainProduct WITH ( NOLOCK )
WHERE   ISNULL(ShowBeginTime,BeginDateTime) < GETDATE()";

        internal const string sql4SelectBargainProductItemsByUserId = @"SELECT DISTINCT
		BP.PKID AS ActivityProductId ,
		BP.PID ,
		BP.Sequence ,
		BP.BeginDateTime ,
		BP.EndDateTime ,
        ISNULL(BP.ShowBeginTime,BP.BeginDateTime) ShowBeginTime,
		BP.ProductType,  
		CASE	WHEN BP.CurrentStockCount < 1 THEN 0
				ELSE 1
		END AS HasStock, 
		ISNULL(BOA.IsOver, 0) AS IsOver ,
		ISNULL(BOA.IsPurchased, 0) AS IsPurchased ,
		CASE	WHEN BOA.PKID IS NULL THEN 0
				ELSE 1
		END AS IsShare ,
		CASE	WHEN ISNULL(BOA.CurrentCount, 0) > 0 THEN 1
				ELSE 0
		END AS HasBargainHistory
FROM	Configuration..BargainProduct AS BP WITH ( NOLOCK )
		LEFT JOIN Activity..BargainOwnerAction AS BOA WITH ( NOLOCK ) ON BOA.ActivityProductId = BP.PKID
																			AND BOA.OwnerId = @ownerid
WHERE	( BOA.PKID IS NOT NULL ) OR ( ISNULL( BP.ShowBeginTime,BP.BeginDateTime) > DATEADD(HOUR, -1, GETDATE())) ;";
        #endregion

        #region CheckUserBargained
        internal const string sql4CheckUserBargained = @"SELECT  COUNT(1)
FROM    Activity..BargainOwnerAction AS BOA WITH ( NOLOCK )
        LEFT JOIN Activity..BargainShareAction AS BSA WITH ( NOLOCK ) ON BOA.PKID = BSA.ParentId
WHERE   BOA.ActivityProductId = @apid
        AND BOA.OwnerId = @ownerid
        AND BSA.UserId = @userid;
";
        #endregion

        #region CheckUserBargainedTimes
        internal const string sql4CheckUserBargainedCount = @"
select COUNT(1)
from Activity..BargainShareAction as T with (nolock)
    inner join Activity..BargainOwnerAction as S with (nolock)
        on T.ParentId = S.PKID
where T.UserId = @UserId
      and S.OwnerId <> T.UserId
      and T.CreateDateTime >= @todayDate;";

        #endregion

        #region CheckUserBargainOwnerActionCount 

        internal const string sql4CheckUserBargainOwnerActionCount = "SELECT COUNT(1) FROM Activity..BargainOwnerAction WITH ( NOLOCK )WHERE IsPurchased=1 and OwnerId = @ownerid AND CreateDateTime>=@beginTime AND CreateDateTime<=@endTime";
        #endregion

        #region MarkUserReceiveCoupon
        internal static string sql4MarkUserReceiveCoupon = "UPDATE activity..BargainOwnerAction SET IsPurchased=1,LastUpdateDateTime=GETDATE() WHERE OwnerId=@OwnerId AND ActivityProductId=@ActivityProductId ";
        #endregion

        #region SelectBargainProductInfo 
        internal static string sql4SelectBargainProductInfo = "SELECT TOP 1 PKID, PID, productName, OriginalPrice, FinalPrice, Sequence, Image1, WXShareTitle, APPShareId, Times, BeginDateTime, EndDateTime, TotalStockCount, CurrentStockCount, Operator, CreateDateTime, LastUpdateDateTime, PageName, SuccessfulHint, ProductType, ShowBeginTime FROM Configuration..BargainProduct WHERE PKID=@PKID ";

        #endregion

        #region SelectUserBargainCountAtTimeRange 
        internal static string sql4SelectUserBargainCountAtTimeRange = @"
select COUNT(1)
from Activity..BargainShareAction
where UserId = @UserId
      and (   CreateDateTime >= @beginTime
              and CreateDateTime <= @endTime)";
        #endregion
    }
}
