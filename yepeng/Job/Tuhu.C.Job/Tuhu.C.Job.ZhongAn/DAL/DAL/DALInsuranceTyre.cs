using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using K.Domain;
using K.DLL.Common;

namespace K.DLL.DAL
{
    public static class DALInsuranceTyre
    {
        public static bool IsExisted(string tyreId, string type)
        {
            var sqlParamters = new[] 
            {
                new SqlParameter("@tyreId",tyreId),
                new SqlParameter("@type",type)
            };
            string _IsExisted = DataOp.GetPara("select top 1 1 from tbl_InsuranceTyre WITH(NOLOCK) where tyreId=@tyreId and type=@type", sqlParamters);
            if (string.IsNullOrEmpty(_IsExisted))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static DInsuranceTyre GetInsuranceType(string tyreId, string type, byte state, string orderNo)
        {
            var sqlParamters = new[] 
            {
                new SqlParameter("@tyreId",tyreId),
                new SqlParameter("@type",type),
                new SqlParameter("@state", state), 
                new SqlParameter("@orderNo", orderNo)
            };
            return DataOp.GetDataSet(@"
            SELECT  PKID,
                    PID,
                    orderNo,
                    orderDate,
                    customerName,
                    customerPhoneNo,
                    plateNumber,
                    storeAddress,
                    storeName,
                    tyreType,
                    tyrePrice,
                    idType,
                    idNo,
                    tyreBatchNo,
                    tyreId,
                    SentTime,
                    OrderListPkid
            FROM tbl_InsuranceTyre WITH ( NOLOCK )
            WHERE state = @state
            AND type = @type
            AND orderNo = @orderNo
            AND tyreId=@tyreId", sqlParamters).ToIList<DInsuranceTyre>().FirstOrDefault();
        }  

        public static string AddInsuranceTyre(DInsuranceTyre dInsuranceTyre)
        {
            var sqlParamters = new[] 
            { 
                new SqlParameter("@PID",dInsuranceTyre.PID),
                new SqlParameter("@orderNo",dInsuranceTyre.orderNo),
                new SqlParameter("@orderDate",dInsuranceTyre.orderDate),
                new SqlParameter("@customerName",dInsuranceTyre.customerName??string.Empty),
                new SqlParameter("@customerPhoneNo",dInsuranceTyre.customerPhoneNo??string.Empty),
                new SqlParameter("@plateNumber",dInsuranceTyre.plateNumber??string.Empty),
                new SqlParameter("@storeAddress",dInsuranceTyre.storeAddress??string.Empty),
                new SqlParameter("@storeName",dInsuranceTyre.storeName??string.Empty),
                new SqlParameter("@tyreType",dInsuranceTyre.tyreType??string.Empty),
                new SqlParameter("@tyrePrice",dInsuranceTyre.tyrePrice??string.Empty),
                new SqlParameter("@idType",dInsuranceTyre.idType??string.Empty),
                new SqlParameter("@idNo",dInsuranceTyre.idNo??string.Empty),
                new SqlParameter("@type",dInsuranceTyre.type??string.Empty),
                new SqlParameter("@state",dInsuranceTyre.state),
                new SqlParameter("@tyreBatchNo",dInsuranceTyre.tyreBatchNo??string.Empty),
                new SqlParameter("@tyreId",dInsuranceTyre.tyreId),
                new SqlParameter("@OrderListPkid",dInsuranceTyre.OrderListPkid??string.Empty)
            };
            return DataOp.GetPara(@"insert into tbl_InsuranceTyre(PID,orderNo,orderDate,customerName,customerPhoneNo,plateNumber,storeAddress,storeName,tyreType,tyrePrice,idType,idNo,type,state,tyreBatchNo,tyreId,SentTime,OrderListPkid)
            values(@PID,@orderNo,@orderDate,@customerName,@customerPhoneNo,@plateNumber,@storeAddress,@storeName,@tyreType,@tyrePrice,@idType,@idNo,@type,@state,@tyreBatchNo,@tyreId,GETDATE(),@OrderListPkid);
          SELECT @@IDENTITY", sqlParamters);
        }
        public static void UpdateState(byte state, int PKID)
        {
            var sqlParamters = new[] 
            {
                new SqlParameter("@state",state),
                new SqlParameter("@PKID",PKID),
            };
            DataOp.SqlCom("update tbl_InsuranceTyre set state=@state,SentTime=GETDATE() where PKID=@PKID", sqlParamters);
        }
        public static List<DInsuranceTyre> GetFirstTypeListFromOrder()
        {
            return DataOp.GetDataSet(@"SELECT
        a.PID ,
        CAST(a.PKID AS NVARCHAR(100)) AS OrderListPkid,
        CAST(b.PKID AS NVARCHAR(100)) AS orderNo ,
        CONVERT(varchar(100), b.DeliveryDatetime, 120) AS orderDate ,
        b.UserName AS customerName ,
        b.UserTel AS customerPhoneNo ,
        b.CarPlate AS plateNumber ,
        s.[Address] AS storeAddress ,
        b.InstallShop AS storeName ,
        a.Name AS tyreType ,
        CAST(a.Price AS NVARCHAR(100)) AS tyrePrice ,
        a.Num
FROM    dbo.tbl_OrderList a WITH(NOLOCK)
        INNER JOIN dbo.tbl_Order b WITH(NOLOCK) ON a.OrderID = b.PKID AND (a.ProductType IS NULL OR a.ProductType & 32 <> 32)
        INNER JOIN dbo.Shops s WITH(NOLOCK) ON b.InstallShopID = s.PKID
WHERE   
        isnull(a.Deleted,0)=0
        AND b.Status<>'7Canceled'
        AND b.InstallType <> '3NoInstall'
        AND a.PID LIKE 'TR-%'
        AND b.DeliveryDatetime <= GETDATE()
		AND ( EXISTS ( SELECT   1
                       FROM     tbl_OrderList AS OL WITH ( NOLOCK )
                       WHERE    OL.PID LIKE 'BX-TUHU-LTX%'
                                AND OL.Deleted = 0
                                AND OL.OrderID = a.OrderID )
              OR ( EXISTS ( SELECT  1
                                FROM    tbl_OrderList AS OL2 WITH ( NOLOCK )
								INNER JOIN dbo.tbl_OrderRelationship ors WITH(NOLOCK)
								ON OL2.OrderID = ors.ChildrenOrderId
                                AND OL2.PID LIKE 'BX-TUHU-LTX%'
                                AND OL2.Deleted = 0
								AND ors.RelationshipType = 13 
                                AND ors.ParentOrderId = a.OrderID
								)
                 )
            )
        AND a.PKID NOT IN (SELECT i.OrderListPkid FROM dbo.tbl_InsuranceTyre i WITH(NOLOCK) WHERE i.[state] = 1 AND i.[type] = 1)
	    ORDER BY b.DeliveryDatetime DESC", null).ToIList<DInsuranceTyre>().ToList();
        }
        public static List<DInsuranceTyre> GetSecondTypeListFromOrder()
        {
            return DataOp.GetDataSet(@"SELECT	a.PID,
		CAST(a.PKID AS NVARCHAR(100)) AS OrderListPkid,
		CAST(b.PKID AS NVARCHAR(100)) AS orderNo,
		CONVERT(VARCHAR(100), b.InstallDatetime, 120) AS orderDate,
		b.UserName AS customerName,
		b.UserTel AS customerPhoneNo,
		b.CarPlate AS plateNumber,
		s.[Address] AS storeAddress,
		b.InstallShop AS storeName,
		a.Name AS tyreType,
		CAST(a.Price AS NVARCHAR(100)) AS tyrePrice,
		a.Num
FROM tbl_OrderList AS a WITH(NOLOCK)
INNER JOIN dbo.tbl_Order b WITH (NOLOCK)
		ON a.OrderID = b.PKID AND (a.ProductType IS NULL OR a.ProductType & 32 <> 32)
INNER JOIN dbo.Shops s WITH (NOLOCK)
		ON b.InstallShopID = s.PKID
WHERE	ISNULL(a.Deleted, 0) = 0
		AND b.Status <> '7Canceled'
        AND b.InstallType <> '3NoInstall'
		AND b.InstallDatetime <= GETDATE()
		AND ( EXISTS ( SELECT   1
                       FROM     tbl_OrderList AS OL WITH ( NOLOCK )
                       WHERE    OL.PID LIKE 'BX-TUHU-LTX%'
                                AND OL.Deleted = 0
                                AND OL.OrderID = a.OrderID )
              OR ( EXISTS ( SELECT  1
                                FROM    tbl_OrderList AS OL2 WITH ( NOLOCK )
								INNER JOIN dbo.tbl_OrderRelationship ors WITH(NOLOCK)
								ON OL2.OrderID = ors.ChildrenOrderId
                                AND OL2.PID LIKE 'BX-TUHU-LTX%'
                                AND OL2.Deleted = 0
								AND ors.RelationshipType = 13 
                                AND ors.ParentOrderId = a.OrderID
								)
                 )
            )
		AND A.PID LIKE 'TR-%'
        AND a.PKID NOT IN (SELECT i.OrderListPkid FROM dbo.tbl_InsuranceTyre i WITH(NOLOCK) WHERE i.[state] = 1 AND i.[type] = 2)
ORDER BY b.InstallDatetime DESC", null).ToIList<DInsuranceTyre>().ToList();
        }

        public static List<DInsuranceTyre> GetThirdTypeList()
        {
            return DataOp.GetDataSet(@"select 
PKID,OrderListPkid,orderNo,orderDate,customerName,customerPhoneNo,plateNumber,storeAddress,storeName,tyreType,tyrePrice,idType,idNo,tyreBatchNo,tyreId
from tbl_InsuranceTyre WITH(NOLOCK) where state<>1", null).ToIList<DInsuranceTyre>().ToList();
        }

        public static DataSet GetTyreID(int OrderId, string PID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@OrderId",OrderId),
                new SqlParameter("@PID",PID),
            };
            return WMSDataOp.GetDataSet(@"
SELECT ISNULL(TI.InsuranceCode, T.TireCode) AS InsuranceCode, B.WeekYear
FROM LogisticTask AS LT WITH (NOLOCK)
INNER JOIN LogisticTaskTire AS T WITH (NOLOCK)
ON LT.PKID = T.LogisticTaskId
AND T.IsDelete = 0
INNER JOIN Batch AS B WITH (NOLOCK)
ON T.BatchId = B.PKID
LEFT JOIN TireInsurance AS TI WITH (NOLOCK)
ON T.TireCode = TI.TireCode
WHERE LT.OrderId = @OrderId AND T.PID =@PID
AND LT.OrderType = '3Sent'
AND LT.TaskStatus <> '5Canceled'", sqlParamters);
        }

        public static IList<DInsuranceTyre> SelectCancelingInsuranceTypes()
        {
            return DataOp.GetDataSet(@"
SELECT  PKID,
        PID,
        orderNo,
        orderDate,
        customerName,
        customerPhoneNo,
        plateNumber,
        storeAddress,
        storeName,
        tyreType,
        tyrePrice,
        idType,
        idNo,
        tyreBatchNo,
        tyreId,
        SentTime,
        OrderListPkid
FROM tbl_InsuranceTyre WITH ( NOLOCK )
WHERE state = 0
  AND type = 3").ToIList<DInsuranceTyre>();
        }

        public static List<DInsuranceTyre> GetFirstTypeListFromOrder(int orderId)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@OrderId",orderId)
            };
            return DataOp.GetDataSet(@"SELECT  a.PID ,
        CAST(a.PKID AS NVARCHAR(100)) AS OrderListPkid,
        a.Name AS tyreType ,
        CAST(cast((a.Price - ISNULL(a.PromotionMoney,0) * 1.0/ a.Num) as decimal(18,2)) AS NVARCHAR(100)) AS tyrePrice ,
        a.Num
FROM    dbo.tbl_OrderList a WITH(NOLOCK)
WHERE   
        isnull(a.Deleted,0)=0
        AND a.PID LIKE 'TR-%'
		AND (a.ProductType IS NULL OR a.ProductType & 32 <> 32)
        AND a.OrderID = @OrderId", sqlParamters).ToIList<DInsuranceTyre>().ToList();
        }

        public static List<DInsuranceTyre> SelectInsuranceTypes(int type, byte state, int orderNo)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@type",type),
                new SqlParameter("@state", state),
                new SqlParameter("@orderNo", orderNo)
            };
            return DataOp.GetDataSet(@"
            SELECT  PKID,
                    PID,
                    orderNo,
                    orderDate,
                    customerName,
                    customerPhoneNo,
                    plateNumber,
                    storeAddress,
                    storeName,
                    tyreType,
                    tyrePrice,
                    idType,
                    idNo,
                    tyreBatchNo,
                    tyreId,
                    SentTime,
                    OrderListPkid
            FROM tbl_InsuranceTyre WITH ( NOLOCK )
            WHERE state = @state
            AND type = @type
            AND orderNo = @orderNo", sqlParamters).ToIList<DInsuranceTyre>().ToList();
        }
    }
}
