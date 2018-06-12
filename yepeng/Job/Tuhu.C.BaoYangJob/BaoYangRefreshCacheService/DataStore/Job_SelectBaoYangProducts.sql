USE Tuhu_productcatalog;
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[Job_SelectBaoYangProducts]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].Job_SelectBaoYangProducts;
GO
/*=============================================================
--FuncDes:	获取保养产品信息
--Author		ModifyDate		Reason
--liuchao		2016-5-12		Create

EXEC Tuhu_productcatalog..Job_SelectBaoYangProducts
=============================================================*/
CREATE PROCEDURE [dbo].[Job_SelectBaoYangProducts]
AS
    SELECT  cp.oid ,
            cp.ProductID ,
            cp.VariantID ,
            cp.PID ,
            cp.DisplayName ,
            cp.CatalogName ,
            cp.PrimaryParentCategory ,
            ISNULL(Image_filename,
                   ISNULL(Image_filename_2,
                          ISNULL(Image_filename_3,
                                 ISNULL(Image_filename_4,
                                        ISNULL(Image_filename_5,
                                               ISNULL(Image_filename_Big,
                                                      ISNULL(Variant_Image_filename_1,
                                                             ISNULL(Variant_Image_filename_2,
                                                              Variant_Image_filename_3)))))))) AS Image_filename ,
            cp.cy_list_price ,
            cp.CP_Remark ,
            cp.CP_ShuXing1 ,
            cp.CP_ShuXing2 ,
            cp.CP_ShuXing3 ,
            cp.CP_ShuXing4 ,
            cp.CP_ShuXing5 ,
            cp.CP_ShuXing6 ,
            cp.CP_RateNumber ,
            cp.PartNo ,
            cp.CP_Brand ,
            cp.CP_Unit ,
            cp.Color ,
            cp.CP_Brake_Position ,
            ps.AvailableStockQuantity ,
            cp.stockout ,
            cp.CP_Wiper_Series ,
            cp.isOE
    FROM    Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) cp
            JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy (NOLOCK) ch ON cp.oid = ch.child_oid
            LEFT JOIN ( SELECT  PID ,
                                SUM(AvailableStockQuantity) AS AvailableStockQuantity
                        FROM    Gungnir..DataForStock (NOLOCK) opi
                                INNER JOIN BaoYang..BaoYang_WareHouse (NOLOCK) wh ON opi.WareHouseId = wh.WareHouseId
                                                              AND wh.IsDeleted = 0
                        GROUP BY PID
                      ) AS ps ON cp.PID = ps.PID COLLATE Chinese_PRC_CI_AS
    WHERE   ( ch.NodeNo = N'28656'
              OR ch.NodeNo LIKE N'28656.%'
            )
            AND cp.OnSale = 1
            AND cp.stockout = 0
            AND cp.IsUsedInAdaptation = 1; 

GO