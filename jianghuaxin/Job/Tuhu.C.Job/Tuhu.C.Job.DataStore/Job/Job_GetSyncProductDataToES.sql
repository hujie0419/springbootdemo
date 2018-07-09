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
			   FROM		CarPAR_CatalogHierarchy AS CH WITH ( NOLOCK )
			   JOIN		[CarPAR_zh-CN_Catalog] AS C WITH ( NOLOCK )
						ON C.#Catalog_Lang_Oid = CH.child_oid
			   WHERE	CH.CategoryName IS NOT NULL
			 ),
		CN
		  AS ( SELECT	C.oid,
						CAST(C.DisplayName AS NVARCHAR(MAX)) AS CategoryName,
						CAST(C.CategoryName AS NVARCHAR(MAX)) AS Category
			   FROM		C
			   WHERE	C.ParentOid < 0
			   UNION ALL
			   SELECT	VPC.oid,
						CN.CategoryName + ' ' + VPC.DisplayName,
						CN.Category + '/' + VPC.CategoryName
			   FROM		C AS VPC
			   JOIN		CN
						ON CN.oid = VPC.ParentOid
			 )
	 SELECT	CP.ProductID,
			CN.CategoryName,
			CN.Category
	 INTO	#T
	 FROM	CarPAR_CatalogProducts AS CP
	 JOIN	CarPAR_CatalogHierarchy AS CH
			ON CP.oid = CH.child_oid
	 JOIN	CN
			ON CN.oid = CH.oid
	 WHERE	CP.i_ClassType IN ( 4, 8 );

IF @Pids IS NULL
BEGIN
	  SELECT	*
	  INTO		#P
	  FROM		vw_ProductService_ProductSearch WITH ( NOLOCK );

	  CREATE UNIQUE INDEX idx_ProductID ON #T(ProductID);
	  CREATE INDEX idx_ProductID ON #P(ProductID);

	  SELECT	PC.CategoryName,
				PC.Category,
				P.*
	  FROM		#P AS P
	  LEFT	  JOIN #T AS PC
				ON PC.ProductID = P.ProductID;

	  DROP TABLE #P;
END;
ELSE
	  SELECT	PC.CategoryName,
				PC.Category,
				P.*
	  FROM		vw_ProductService_ProductSearch AS P WITH ( NOLOCK )
	  LEFT  JOIN #T AS PC
				ON PC.ProductID = P.ProductID
	  JOIN		Gungnir..SplitString(@Pids, ',', 1) AS SS
				ON P.PID = SS.Item  COLLATE Chinese_PRC_CI_AS;

DROP TABLE #T;
GO
