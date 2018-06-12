USE Tuhu_productcatalog;

GO
IF EXISTS ( SELECT	*
			FROM	dbo.sysobjects
			WHERE	id = OBJECT_ID(N'[dbo].[Job_GetSyncProductDataToES]')
					AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
   DROP PROCEDURE [dbo].[Job_GetSyncProductDataToES];
GO
/*=============================================================
--FuncDes: 获取需同步到ES的产品数据
--Author	ModifyDate	Reason
--liuchao   2016-01-04	从代码中迁出到SP

EXEC Tuhu_productcatalog.dbo.Job_GetSyncProductDataToES 'TR-NX-CP672|25,TR-NX-CP661|9'
=============================================================*/
CREATE PROCEDURE [dbo].[Job_GetSyncProductDataToES]
	   @Pids NVARCHAR(MAX) = NULL
AS
WITH	C AS ( SELECT	CH.child_oid AS oid,
						CH.oid AS ParentOid,
						CH.CategoryName,
						C.DisplayName,
						NodeNo
			   FROM		Tuhu_productcatalog.[dbo].[CarPAR_CatalogHierarchy] AS CH WITH ( NOLOCK )
			   JOIN		Tuhu_productcatalog.[dbo].[CarPAR_zh-CN_Catalog] AS C WITH ( NOLOCK )
						ON C.#Catalog_Lang_Oid = CH.child_oid
			   WHERE	CH.CategoryName IS NOT NULL
			 ),
		CN
		  AS ( SELECT	C.oid,
						CAST(C.DisplayName AS NVARCHAR(MAX)) AS CategoryName,
						CAST(C.CategoryName AS NVARCHAR(MAX)) AS Category
			   FROM		C
			   WHERE	C.ParentOid < 0
						AND C.NodeNo NOT LIKE '4309%'
						AND C.NodeNo NOT LIKE '13479%'
						AND C.NodeNo NOT LIKE '35267%'
						AND C.NodeNo NOT LIKE '35266%'
			   UNION ALL
			   SELECT	VPC.oid,
						CN.CategoryName + ' ' + VPC.DisplayName,
						CN.Category + ' ' + VPC.CategoryName
			   FROM		C AS VPC
			   JOIN		CN
						ON CN.oid = VPC.ParentOid
			 ),
		PC
		  AS ( SELECT	CP.ProductID,
						CN.CategoryName,
						CN.Category
			   FROM		CarPAR_CatalogProducts AS CP
			   JOIN		CarPAR_CatalogHierarchy AS CH
						ON CP.oid = CH.child_oid
			   JOIN		CN
						ON CN.oid = CH.oid
			   WHERE	CP.i_ClassType IN ( 4, 8 )
			 )
	 SELECT	CP.ProductID + '|' + CP.VariantID AS PID,
			PC.CategoryName,
			PC.Category,
			C.CP_Brand,
			C.CP_Tire_Width,
			C.CP_Tire_AspectRatio,
			C.CP_Tire_Rim,
			C.CP_Tire_SpeedRating,
			CP.CP_Tire_ROF,
			C.CP_Filter_Type,
			CP.CP_Wiper_Size,
			C.CP_Brake_Position,
			C.CP_Brake_Type,
			C.CP_Unit,
			CP.CP_ShuXing1,
			CP.CP_ShuXing3,
			( CASE WHEN CP.OnSale > 0
						AND CP.cy_list_price > 0 THEN 1
				   ELSE 0
			  END ) AS OnSale,
			CP.stockout,
			CP.DefinitionName,
			C.DisplayName,
			CP.CP_ShuXing5,
			CP.ProductRefer
	 INTO	#T
	 FROM	Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH ( NOLOCK )
	 JOIN	Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH ( NOLOCK )
			ON CP.oid = C.#Catalog_Lang_Oid
	 JOIN	PC
			ON PC.ProductID = CP.ProductID
	 WHERE	CP.i_ClassType IN ( 2, 4 )
			AND CP.IsShow = 1
			AND CP.PrimaryParentCategory <> 'Fuelcard'
			AND CP.PrimaryParentCategory <> '3MOILPAKAGE'
			AND CP.PrimaryParentCategory <> 'Temporary'
			AND CP.ProductID <> 'OL-3MQT-JZ-SN'
			AND CP.ProductID <> 'OL-3MQT-YZ-SN';

DELETE	#T
WHERE	PID IN ( 'OL-3M-YZ-SN|1', 'OL-3M-YZ-SN|2', 'OL-3M-JZ-SN|1', 'OL-3M-JZ-SN|2', 'OL-3M-JZ-SN|3', 'OL-3M-JZ-SN|4' );

IF @Pids IS NULL
   SELECT	P.*,
			PS.SalesQuantity,
			PS.CommentRate
   FROM		#T AS P
   LEFT JOIN tbl_ProductStatistics AS PS WITH ( NOLOCK )
			ON P.PID = PS.ProductID;
ELSE
   SELECT	P.*,
			PS.SalesQuantity,
			PS.CommentRate
   FROM		#T AS P
   LEFT JOIN tbl_ProductStatistics AS PS WITH ( NOLOCK )
			ON P.PID = PS.ProductID
   JOIN		Gungnir..SplitString(@Pids, ',', 1) AS SS
			ON P.PID = SS.Item  COLLATE Chinese_PRC_CI_AS;

DROP TABLE #T;
GO
