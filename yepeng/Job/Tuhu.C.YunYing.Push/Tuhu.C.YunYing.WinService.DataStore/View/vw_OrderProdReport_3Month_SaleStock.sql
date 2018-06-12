USE [Gungnir]
GO

Alter VIEW [dbo].[vw_OrderProdReport_3Month_SaleStock]
AS
SELECT	P.DisplayName,
		P.price,
		P.Pattern,
		P.Size,
		P.Brand,
		CASE WHEN (OPR_1Month.sum < OPR_3Month.sum / 3) THEN N'↓'
			 WHEN (OPR_1Month.sum > OPR_3Month.sum / 2.5) THEN N'↑↑'
		END AS [1MTrend],
		CASE WHEN (OPR_1Month.Web < OPR_3Month.Web / 3) THEN N'↓'
			 WHEN (OPR_1Month.Web > OPR_3Month.Web / 2.5) THEN N'↑↑'
		END AS [1MTrendWeb],
		CASE WHEN (OPR_1Month.Taobao < OPR_3Month.Taobao / 3) THEN N'↓'
			 WHEN (OPR_1Month.Taobao > OPR_3Month.Taobao / 2.5) THEN N'↑↑'
		END AS [1MTrendTaobao],
		CASE WHEN (OPR_1Week.sum < OPR_1Month.sum / 4) THEN N'↓'
			 WHEN (OPR_1Week.sum > OPR_1Month.sum / 3) THEN N'↑↑'
		END AS [1WTrend],
		CASE WHEN (OPR_1Week.Web < OPR_1Month.Web / 4) THEN N'↓'
			 WHEN (OPR_1Week.Web > OPR_1Month.Web / 3) THEN N'↑↑'
		END AS [1WTrendWeb],
		CASE WHEN (OPR_1Week.Taobao < OPR_1Month.Taobao / 4) THEN N'↓'
			 WHEN (OPR_1Week.Taobao > OPR_1Month.Taobao / 3) THEN N'↑↑'
		END AS [1WTrendTaobao],
		CASE WHEN (StockNum < ROUND(OPR_1Month.sum * 4 / 30, 0)
				   OR StockNum < ROUND(OPR_3Month.sum * 4 / 90, 0)) THEN N'--'
			 WHEN (StockNum > ROUND(OPR_1Month.sum * 4 * 2 / 30, 0)
				   OR (OPR_1Month.sum IS NULL
					   AND StockNum > 0)) THEN N'+'
			 ELSE N''
		END AS StockBench,
		SLN.PID,
		SLN.AllNum,
		SLN.AvbCost,
		CASE WHEN (OPR_1Month.sum > 15
				   AND OPR_1Month.sum < 30) THEN 4
			 WHEN (OPR_1Month.sum IS NULL) THEN 0
			 ELSE ROUND(OPR_1Month.sum * 4 / 30, 0)
		END AS SecurityStock,
		CONVERT(INT, SLN.StockNum) AS 库存数量,
		SLN.SHStockNum,
		SLN.BJStockNum,
		SLN.GZStockNum,
		SLN.SYStockNum,
		SLN.CDStockNum,
		SLN.WHStockNum,
		OPR_3Month.sum AS [3月销售数量],
		OPR_3Month.Web,
		OPR_3Month.Taobao,
		OPR_3Month.JingDong,
		OPR_3Month.PIFA,
		OPR_3Month.SHSum AS SHSum3M,
		OPR_3Month.BJSum AS BJSum3M,
		OPR_3Month.GZSum AS GZSum3M,
		OPR_3Month.SYSum AS SYSum3M,
		OPR_3Month.CDSum AS CDSum3M,
		OPR_3Month.WHSum AS WHSum3M,
		OPR_1Month.sum AS [1月销售数量],
		OPR_1Month.Web AS [1MWeb],
		OPR_1Month.Taobao AS [1MTaobao],
		OPR_1Month.JingDong AS [1MJingDong],
		OPR_1Month.PIFA AS [1MPIFA],
		OPR_1Month.SHSum AS SHSum1M,
		OPR_1Month.BJSum AS BJSum1M,
		OPR_1Month.GZSum AS GZSum1M,
		OPR_1Month.SYSum AS SYSum1M,
		OPR_1Month.CDSum AS CDSum1M,
		OPR_1Month.WHSum AS WHSum1M,
		OPR_1Week.sum AS [1WSum],
		OPR_1Week.Web AS [1WWeb],
		OPR_1Week.Taobao AS [1WTaobao],
		OPR_1Week.JingDong AS [1WJingDong],
		OPR_1Week.PIFA AS [1WPIFA],
		OPR_1Week.SHSum,
		OPR_1Week.BJSum,
		OPR_1Week.GZSum,
		OPR_1Week.SYSum,
		OPR_1Week.CDSum,
		OPR_1Week.WHSum,
		ORP_3Month_C.sum AS [3MCanceled],
		LVPID.vendorname,
		0 AS LastPurCost,
		LVPID.minPrice,
		LVPID.weekyear,
		OCR_1Month.cost,
		OCR_1Month.Rate,
		Taobao.PID AS TaobaoID,
		Taobao.PName AS TaobaoName,
		Taobao.PPrice AS TaobaoPrice,
		Taobao.Status AS TaobaoStatus,
		Taobao.Promotion AS TaobaoPromotion,
		Tmall.PID AS TMPID,
		Tmall.PName AS TMName,
		Tmall.PPrice AS TMPrice,
		Tmall.Status AS TMStatus,
		Tmall.Promotion AS TMPromotion,
		Tmall2.PID AS TM2PID,
		Tmall2.PName AS TM2Name,
		Tmall2.PPrice AS TM2Price,
		Tmall2.Status AS TM2Status,
		Tmall2.Promotion AS TM2Promotion,
		Tmall3.PID AS TM3PID,
		Tmall3.PName AS TM3Name,
		Tmall3.PPrice AS TM3Price,
		Tmall3.Status AS TM3Status,
		Tmall3.Promotion AS TM3Promotion,
		JDTuhu.PID AS JDTuhuPID,
		JDTuhu.PName AS JDTuhuName,
		JDTuhu.PPrice AS JDTuhuPrice,
		JDTuhu.Status AS JDTuhuStatus,
		JDTuhu.Promotion AS JDTuhuPromotion,
		JD.PID AS JDPID,
		JD.PName,
		JD.PPrice AS JDPrice,
		JD.Status AS JDStatus,
		MaiLunTai.PID AS MLTPID,
		MaiLunTai.PName AS MLTName,
		MaiLunTai.PPrice AS MLTPrice,
		MaiLunTai.Status AS MLTStatus,
		MaiLunTaiTmall.PID AS MLTTPID,
		MaiLunTaiTmall.PName AS MLTTName,
		MaiLunTaiTmall.PPrice AS MLTTPrice,
		MaiLunTaiTmall.Status AS MLTTStatus,
		TeWeiLun.PID AS TWLPID,
		TeWeiLun.PName AS TWLName,
		TeWeiLun.PPrice AS TWLPrice,
		TeWeiLun.Status AS TWLStatus,
		
		JDQTuhu.PID AS JDQTuhuPID,
		JDQTuhu.PName AS JDQTuhuName,
		JDQTuhu.PPrice AS JDQTuhuPrice,
		JDQTuhu.Status AS JDQTuhuStatus,
		JDQTuhu.Promotion AS JDQTuhuPromotion
FROM	(SELECT	PID,
				SUM( ISNULL(AvailableNum, 0) ) AS StockNum,
				SUM(CASE LocationId
					  WHEN 0 THEN AvailableNum
					  ELSE 0
					END) AS SHStockNum,
				SUM(CASE LocationId
					  WHEN 207 THEN AvailableNum
					  ELSE 0
					END) AS BJStockNum,
				SUM(CASE LocationId
					  WHEN 330 THEN AvailableNum
					  ELSE 0
					END) AS GZStockNum,
				SUM(CASE LocationId
					  WHEN 566 THEN AvailableNum
					  ELSE 0
					END) AS SYStockNum,
				SUM(CASE LocationId
					  WHEN 568 THEN AvailableNum
					  ELSE 0
					END) AS CDStockNum,
				SUM(CASE LocationId
					  WHEN 1937 THEN AvailableNum
					  ELSE 0
					END) AS WHStockNum,
				ROUND(SUM(ISNULL(AvailableNum, 0) * ISNULL(CostPrice,0)) / SUM(ISNULL(AvailableNum, 0) + 0.0001), 0) AS AvbCost,
				SUM(ISNULL(Num, 0)) AS AllNum
		 FROM	vw_Linked_StockLocation WITH(NOLOCK)
		 WHERE	LocationId IN (SELECT PKID FROM Gungnir..shops WITH(NOLOCK) WHERE ShopType&4=4 AND SimpleName LIKE N'%仓库' AND SimpleName NOT LIKE N'%大客户仓库')
		 GROUP BY PID
		) AS SLN
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS Taobao WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = Taobao.TuhuPID
		   AND Taobao.ShopCode = N'2淘宝'
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS Tmall WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = Tmall.TuhuPID
		   AND Tmall.ShopCode = N'4天猫'
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS Tmall2 WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = Tmall2.TuhuPID
		   AND Tmall2.ShopCode = N'g天猫2店'
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS Tmall3 WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = Tmall3.TuhuPID
		   AND Tmall3.ShopCode = N'g天猫3店'
LEFT JOIN dbo.vw_Linked_Price_Follow_ThirdTuhu AS JDTuhu WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = JDTuhu.TuhuPID  
		   AND JDTuhu.ShopCode = N'5京东2店' 
LEFT JOIN dbo.vw_Linked_Price_Follow_ThirdTuhu AS JDQTuhu WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = JDQTuhu.TuhuPID
		   AND JDQTuhu.ShopCode = N'5京东旗舰店' 
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS JD WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = JD.TuhuPID
		   AND JD.ShopCode = N'京东自营'  
--LEFT JOIN dbo.vw_Linked_Price_Follow_MaiLunTai AS MaiLunTai  
--  ON SLN.PID COLLATE Chinese_PRC_CI_AS = MaiLunTai.TuhuPID  
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS MaiLunTai WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = MaiLunTai.TuhuPID
		   AND MaiLunTai.ShopCode = N'麦轮胎官网'
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS MaiLunTaiTmall WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = MaiLunTaiTmall.TuhuPID
		   AND MaiLunTaiTmall.ShopCode = N'麦轮胎天猫'
LEFT JOIN dbo.vw_Linked_Price_Follow_Taobao AS TeWeiLun WITH(NOLOCK)
		ON SLN.PID COLLATE Chinese_PRC_CI_AS = TeWeiLun.TuhuPID
		   AND TeWeiLun.ShopCode = N'特维轮'
LEFT JOIN dbo.vw_OrderCost_Report_1Month AS OCR_1Month WITH(NOLOCK)
		ON SLN.PID = OCR_1Month.PID  COLLATE Chinese_PRC_CI_AS
LEFT JOIN dbo.vw_LastVendorforPID AS LVPID WITH(NOLOCK)
		ON SLN.PID = LVPID.PID  COLLATE Chinese_PRC_CI_AS
LEFT JOIN dbo.vw_Products AS P WITH(NOLOCK)
		ON SLN.PID = P.PID COLLATE SQL_Latin1_General_CP1_CI_AS
LEFT JOIN dbo.vw_OrderProdReport_1Month AS OPR_1Month WITH(NOLOCK)
		ON SLN.PID = OPR_1Month.PID  COLLATE Chinese_PRC_CI_AS
LEFT JOIN dbo.vw_OrderProdReport_1Week AS OPR_1Week WITH(NOLOCK)
		ON SLN.PID = OPR_1Week.PID  COLLATE Chinese_PRC_CI_AS
LEFT JOIN dbo.vw_OrderProdReport_3Month_Canceled AS ORP_3Month_C WITH(NOLOCK)
		ON SLN.PID = ORP_3Month_C.PID  COLLATE Chinese_PRC_CI_AS
FULL JOIN dbo.vw_OrderProdReport_3Month AS OPR_3Month WITH(NOLOCK)
		ON SLN.PID = OPR_3Month.PID  COLLATE Chinese_PRC_CI_AS  
  
  
  