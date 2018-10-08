using K.DLL.Common;
using K.Domain;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace K.DLL.DAL
{
    public static class DALOrderToInsuranceTyre
    {
        public static DOrderToInsuranceTyre GetOrderToInsuranceTyre(int orderId, int type, int status)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@OrderId",orderId),
                new SqlParameter("@Type",type),
                new SqlParameter("@Status", status)
            };
            return DataOp.GetDataSet(
                @"SELECT PKID,
	                OrderId,
	                InsuranceType,
	                InsuranceStatus,
	                CreatedBy,
	                CreateTime,
	                LastUpdateTime 
                FROM tbl_OrderToInsuranceTyre WITH(NOLOCK)
                WHERE orderId = @OrderId
                AND InsuranceType = @Type
                AND InsuranceStatus = @Status", sqlParamters).ToIList<DOrderToInsuranceTyre>().FirstOrDefault();
        }

        public static int InsertOrderToInsuranceTyre(DOrderToInsuranceTyre orderToInsuranceTyre)
        {
            var result = string.Empty;
            var sqlParamters = new[]
            {
                new SqlParameter("@OrderId",orderToInsuranceTyre.OrderId),
                new SqlParameter("@InsuranceType",orderToInsuranceTyre.InsuranceType),
                new SqlParameter("@InsuranceStatus",orderToInsuranceTyre.InsuranceStatus),
                new SqlParameter("@CreatedBy",orderToInsuranceTyre.CreatedBy ?? string.Empty),
                new SqlParameter("@CreateTime",orderToInsuranceTyre.CreateTime != default(DateTime)? orderToInsuranceTyre.CreateTime : DateTime.Now),
                new SqlParameter("@LastUpdateTime",orderToInsuranceTyre.LastUpdateTime != default(DateTime)? orderToInsuranceTyre.LastUpdateTime : DateTime.Now)
            };
            result = DataOp.GetPara(@"insert into tbl_OrderToInsuranceTyre(OrderId,InsuranceType,InsuranceStatus,CreatedBy,CreateTime,LastUpdateTime)
            values(@OrderId,@InsuranceType,@InsuranceStatus,@CreatedBy,@CreateTime,@LastUpdateTime);
          SELECT @@IDENTITY", sqlParamters);
            return Int32.Parse(result);
        }

        public static void UpdateOrderToInsuranceTyreStatus(int pkid, int status)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",pkid),
                new SqlParameter("@Status",status)
            };
            DataOp.SqlCom("update tbl_OrderToInsuranceTyre set InsuranceStatus=@Status,LastUpdateTime=GETDATE() where PKID=@PKID", sqlParamters);
        }
    }
}
