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
    public static class DALInsuranceNO
    {
        public static bool IsTyreExisted(string tyreId)
        {
            var sqlParamters = new[] 
            {
                new SqlParameter("@tyreId",tyreId),
            };
            string _IsExisted = DataOp.GetPara("select top 1 1 from tbl_InsuranceNO where tyreId=@tyreId", sqlParamters);
            if (string.IsNullOrEmpty(_IsExisted))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string AddInsuranceNO(DInsuranceNO dInsuranceNO)
        {
            var sqlParamters = new[] 
            { 
                new SqlParameter("@tyreId",dInsuranceNO.tyreId??string.Empty),
                new SqlParameter("@policyNo",dInsuranceNO.policyNo??string.Empty),
                new SqlParameter("@effectiveDate",dInsuranceNO.effectiveDate??string.Empty),
                new SqlParameter("@endDate",dInsuranceNO.endDate??string.Empty),
                new SqlParameter("@issueDate",dInsuranceNO.issueDate??string.Empty)
            };
            return DataOp.GetPara(@"insert into tbl_InsuranceNO(tyreId,policyNo,effectiveDate,endDate,issueDate)
            values(@tyreId,@policyNo,@effectiveDate,@endDate,@issueDate);
          SELECT @@IDENTITY", sqlParamters);
        }
    }
}
