using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalPurchase
    {

        /// <summary>
        /// 获取审核人
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="auditType">审核类型</param>
        /// <returns></returns>
        public static List<string> GetBatchPurchaseAuditor(SqlConnection conn, string auditType)
        {
            var list = new List<string>();
            const string commandText =
                @"SELECT Auditor FROM dbo.AuditConfiguration  WITH (NOLOCK) WHERE IsDeleted=0 AND AuditType=@AuditType";
            using (
                var reader = SqlHelper.ExecuteReader(conn, CommandType.Text, commandText,
                    new SqlParameter("@AuditType", auditType)))
            {
                while (reader.Read())
                {
                    string auditor = reader.GetTuhuString(0);
                    list.Add(auditor);
                }
            }
            return list;
        }
    
    }
}
