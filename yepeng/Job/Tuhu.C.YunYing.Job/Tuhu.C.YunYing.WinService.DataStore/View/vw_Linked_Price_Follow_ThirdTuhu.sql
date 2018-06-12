USE [Gungnir]
GO

CREATE  VIEW [dbo].[vw_Linked_Price_Follow_ThirdTuhu]
AS
SELECT	'JDTuhu' AS Site,
		T.PID,
		T.PName,
		T.PPrice,
		N'²»È·¶¨' AS Status,
		T.TuhuPID,
		T.Promotion,
		T.ShopCode
FROM	(SELECT	PPM.SkuID AS PID,
				PPM.Title AS PName,
				PPM.Price AS PPrice,
				PPM.Pid AS TuhuPID,
				PPM.Promotion,
				PPM.ShopCode,
				ROW_NUMBER() OVER (PARTITION BY PPM.Pid ORDER BY Price DESC) AS RowNumber
		 FROM	Tuhu_productcatalog..ProductPriceMapping  AS PPM WITH (NOLOCK)
		) AS T
WHERE	T.RowNumber = 1
  
  
  