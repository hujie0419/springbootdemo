using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Tuhu.Provisioning.DataAccess.DAO.ShareBargain
{
    public class ShareBargainSqlText
    {
        #region GetBackgroundTheme
        internal const string sql4getBackgroundTheme = @"SELECT  PKID AS ThemeNum ,
        ThemeName
FROM    Configuration..BackgroundStyle WITH ( NOLOCK )
WHERE   IsDelete = 0
        AND Lable = N'Bargain';";
        #endregion
        #region SelectBargainProductList

      internal const string sql4selectBargainProductList = @"SELECT  PKID ,
        PID ,
        productName AS ProductName ,
        CurrentStockCount ,
        FinalPrice ,
        BeginDateTime ,
        EndDateTime ,
        Operator,ShowBeginTime,ProductType 
FROM    Configuration..BargainProduct WITH ( NOLOCK )
WHERE   ( @Operator IS NULL
          OR Operator = @Operator
        )
        AND ( @PID IS NULL
              OR PID LIKE '%' + @PID + '%'
            )
        AND ( @ProductName IS NULL
              OR ProductName Like '%' + @ProductName + '%'
            )
        AND ( @OnSale = 0
              OR ( @OnSale = 1
                   AND BeginDateTime < GETDATE()
                   AND EndDateTime > GETDATE()
                 )
              OR ( @OnSale = 2
                   AND EndDateTime < GETDATE()
                 )
              OR ( @OnSale = 3
                   AND BeginDateTime > GETDATE()
                 )
            )
ORDER BY CreateDateTime DESC
        OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY;
";
      internal const string sql4selectBargainProductCount = @"SELECT  COUNT(1)
FROM    Configuration..BargainProduct WITH ( NOLOCK )
WHERE   ( @Operator IS NULL
          OR Operator = @Operator
        )
        AND ( @PID IS NULL
              OR PID LIKE '%' + @PID + '%'
            )
        AND ( @ProductName IS NULL
              OR ProductName Like '%' + @ProductName + '%'
            )
        AND ( @OnSale = 0
              OR ( @OnSale = 1
                   AND BeginDateTime < GETDATE()
                   AND EndDateTime > GETDATE()
                 )
              OR ( @OnSale = 2
                   AND EndDateTime < GETDATE()
                 )
              OR ( @OnSale = 3
                   AND BeginDateTime > GETDATE()
                 )
            );
";
    #endregion

    #region FetchBargainProductGlobalConfig
      internal const string Sql4FetchBargainProductGlobalConfig = @"
select top 1
       BackgroundImage,
       BackgroundTheme,
       QACount as RulesCount,
       QAData,
       Title,
       WXAPPListShareText,
       WXAPPListShareImg,
       WXAPPDetailShareText,
       APPListShareTag,
       AppDetailShareTag,
       SliceShowText
from Configuration..BargainGlobalConfig with (nolock)
order by LastUpdateDateTime desc;";
    #endregion

    #region UpdateGlobalConfig

      internal const string Sql4UpdateGlobalConfig = @"
update Configuration..BargainGlobalConfig with (rowlock)
set BackgroundImage = @backgroundimage,
    BackgroundTheme = @backgroundtheme,
    QACount = @qacount,
    QAData = @qadata,
    LastUpdateDateTime = GETDATE(),
    Title = @title,
    WXAPPListShareText = @wxapplistsharetext,
    WXAPPListShareImg = @wxapplistshareimg,
    WXAPPDetailShareText = @wxappdetailsharetext,
    APPListShareTag = @applistsharetag,
    AppDetailShareTag = @appdetailsharetag,
    SliceShowText = @sliceshowtext;";

    #endregion

    #region FetchBargainProductById

      internal const string Sql4FetchBargainProductById = @"
select PID,
       productName as ProductName,
       CurrentStockCount,
       FinalPrice,
       BeginDateTime,
       EndDateTime,
       OriginalPrice,
       Sequence,
       Image1,
       TotalStockCount,
       Times,
       PageName,
       SuccessfulHint,
       WXShareTitle,
       APPShareId,
       ShowBeginTime,
       SimpleDisplayName,
       HelpCutPriceTimes,
       CutPricePersonLimit
from Configuration..BargainProduct with (nolock)
where PKID = @pkid;";

    #endregion

    #region AddSharBargainProduct

      internal const string Sql4AddSharBargainProduct = @"
insert Configuration..BargainProduct
(
    PID,
    productName,
    OriginalPrice,
    FinalPrice,
    Sequence,
    Image1,
    WXShareTitle,
    APPShareId,
    Times,
    BeginDateTime,
    EndDateTime,
    TotalStockCount,
    CurrentStockCount,
    Operator,
    CreateDateTime,
    LastUpdateDateTime,
    PageName,
    SuccessfulHint,
    ProductType,
    ShowBeginTime,
    SimpleDisplayName,
    HelpCutPriceTimes,
    CutPricePersonLimit
)
values
(   @pid,             -- PID - nvarchar(50)
    @productName,     -- productName - nvarchar(50)
    @OriginalPrice,   -- OriginalPrice - decimal(18, 8)
    @FinalPrice,      -- FinalPrice - decimal(18, 8)
    @Sequence,        -- Sequence - int
    @Image1,          -- Image1 - nvarchar(256)
    @WXShareTitle,    -- WXShareTitle - nvarchar(512)
    @APPShareId,      -- APPShareId - nvarchar(64)
    @Times,           -- Times - int
    @BeginDateTime,   -- BeginDateTime - datetime
    @EndDateTime,     -- EndDateTime - datetime
    @TotalStockCount, -- TotalStockCount - int
    @TotalStockCount, -- CurrentStockCount - int
    @Operator,        -- Operator - nvarchar(50)
    GETDATE(),        -- CreateDateTime - datetime
    GETDATE(),        -- LastUpdateDateTime - datetime
    @PageName,        -- PageName - nvarchar(50)
    @SuccessfulHint, @ProductType, @ShowBeginTime, @simpleDisplayName,@HelpCutPriceTimes,@CutPricePersonLimit)";

        #endregion

        #region AddSharBargainCoupon

        internal const string Sql4AddSharBargainCoupon = @"INSERT Configuration..BargainProduct
        ( PID ,
          productName ,
          OriginalPrice ,
          FinalPrice ,
          Sequence ,
          Image1 ,
          WXShareTitle ,
          APPShareId ,
          Times ,
          BeginDateTime ,
          EndDateTime ,
          TotalStockCount ,
          CurrentStockCount ,
          Operator ,
          CreateDateTime ,
          LastUpdateDateTime ,
          PageName ,
          SuccessfulHint,
		  ShowBeginTime,
		  ProductType,SimpleDisplayName,
          HelpCutPriceTimes,
          CutPricePersonLimit
        )
VALUES  ( @pid , -- PID - nvarchar(50)
          @productName , -- productName - nvarchar(50)
          @OriginalPrice , -- OriginalPrice - decimal(18, 8)
          @FinalPrice , -- FinalPrice - decimal(18, 8)
          @Sequence , -- Sequence - int
          @Image1 , -- Image1 - nvarchar(256)
          @WXShareTitle , -- WXShareTitle - nvarchar(512)
          @APPShareId , -- APPShareId - nvarchar(64)
          @Times , -- Times - int
          @BeginDateTime , -- BeginDateTime - datetime
          @EndDateTime , -- EndDateTime - datetime
          @TotalStockCount , -- TotalStockCount - int
          @TotalStockCount , -- CurrentStockCount - int
          @Operator , -- Operator - nvarchar(50)
          GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- LastUpdateDateTime - datetime
          @PageName , -- PageName - nvarchar(50)
          @SuccessfulHint , -- SuccessfulHint - nvarchar(50)
		  @ShowBeginTime,
		  @ProductType,@simpleDisplayName,@HelpCutPriceTimes,@CutPricePersonLimit
        )";

        #endregion

        #region CheckBargainProductByPid

        internal const string Sql4CheckBargainProductByPid = @"SELECT  COUNT(1)
FROM    Configuration..BargainProduct WITH ( NOLOCK )
WHERE   PID = @pid
        AND ( ( BeginDateTime > DATEADD(DAY, -1, @begin)
                AND BeginDateTime < DATEADD(DAY, 1, @end)
              )
              OR ( EndDateTime > DATEADD(DAY, -1, @begin)
                   AND EndDateTime < DATEADD(DAY, 1, @end)
                 )
            );";

    #endregion

    #region CheckProductByPid

      internal const string Sql4CheckProductByPid = @"SELECT TOP 1
        DisplayName AS Info ,
        1 AS Code
FROM    Tuhu_productcatalog..vw_Products WITH ( NOLOCK )
WHERE   PID = @pid
        AND OnSale = 1;";

    #endregion

    #region UpdateBargainProductById

      internal const string Sql4UpdateBargainProductById = @"
update Configuration..BargainProduct with (rowlock)
set BeginDateTime = @begindate,
    EndDateTime = @enddate,
    TotalStockCount = @totalstockcount,
    CurrentStockCount = @currentstockcount,
    PageName = @pagename,
    Sequence = @sequence,
    Image1 = @image,
    SuccessfulHint = @successfulhint,
    WXShareTitle = @wxshretitle,
    APPShareId = @appshareid,
    ShowBeginTime = @ShowBeginTime,
    SimpleDisplayName = @simpleDisplayName
where PKID = @pkid;";

        #endregion

        #region UpdateBargainCouponById

        internal const string Sql4UpdateBargainCouponById = @"UPDATE  Configuration..BargainProduct WITH ( ROWLOCK )
SET     BeginDateTime = @begindate ,
        EndDateTime = @enddate ,
        TotalStockCount=@totalstockcount ,
        CurrentStockCount = @currentstockcount ,
        PageName = @pagename ,
        Sequence = @sequence ,
        Image1 = @image ,
        SuccessfulHint = @successfulhint ,
        WXShareTitle = @wxshretitle ,
        APPShareId = @appshareid,ShowBeginTime=@ShowBeginTime,SimpleDisplayName=@simpleDisplayName
WHERE   PKID = @pkid;";

        #endregion

        #region DeleteBargainProductById

        internal const string Sql4DeleteBargainProductById =
        @"DELETE FROM Configuration..BargainProduct WHERE PKID=@pkid";

      #endregion


    }
}
