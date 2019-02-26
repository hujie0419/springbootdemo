using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;

namespace Tuhu.Provisioning.Business.BeautyCode
{
    internal class BeautyCodeHandler
    {
        internal IEnumerable<BeautyServicePackageSimpleModel> GetPackagesByPackageType(SqlConnection conn, string packageType)
            => DalBeautyCode.SelectPackagesByPackageType(conn, packageType);

        internal IEnumerable<BeautyServicePackageDetailSimpleModel> GetProductsByPackageId(SqlConnection conn, int packageId)
            => DalBeautyCode.SelectProductsByPackageId(conn, packageId);

        internal bool BatchAddBeautyCode(SqlConnection conn, List<CreateBeautyCodeTaskModel> list)
        {
            var dt = new DataTable { TableName = "Tuhu_groupon.dbo.CreateBeautyCodeTask" };
            dt.Columns.Add(new DataColumn("MobileNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
            dt.Columns.Add(new DataColumn("StartTime", typeof(string)));
            dt.Columns.Add(new DataColumn("EndTime", typeof(string)));
            dt.Columns.Add(new DataColumn("Type", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("BatchCode", typeof(string)));
            dt.Columns.Add(new DataColumn("CreateUser", typeof(string)));
            dt.Columns.Add(new DataColumn("MappingId", typeof(int)));
            list.ForEach(item =>
            {
                var row = dt.NewRow();
                row["MobileNumber"] = item.MobileNumber;
                row["Quantity"] = item.Quantity;
                row["StartTime"] = item.StartTime;
                row["EndTime"] = item.EndTime;
                row["Type"] = item.Type;
                row["Status"] = item.Status;
                row["BatchCode"] = item.BatchCode;
                row["CreateUser"] = item.CreateUser;
                row["MappingId"] = item.MappingId;
                dt.Rows.Add(row);
            });
            return DalBeautyCode.BatchAddBeautyCode(conn, dt);
        }

        internal void UpdateBeautyCodeStatus(SqlConnection conn, string batchCode, string status)
            => DalBeautyCode.UpdateBeautyCodeStatus(conn, batchCode, status);
        /// <summary>
        /// 分页查询上传用户批次号
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="mappingIds"></param>
        /// <returns></returns>
        internal Tuple<List<string>, int> SelectBatchCodesByMappingIds(SqlConnection conn, int pageIndex, int pageSize, IEnumerable<int> mappingIds)
        {
            Tuple<List<string>, int> result = null;

            if (mappingIds != null && mappingIds.Any())
            {
                result = DalBeautyCode.SelectBatchCodesByMappingIds(conn, pageIndex, pageSize, mappingIds);
            }
            return result;
        }
        /// <summary>
        /// 根据批次号获取上传用户进度统计信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCodes"></param>
        /// <returns></returns>
        internal List<BeautyCodeStatistics> GetBeautyCodeStatistics(SqlConnection conn, IEnumerable<string> batchCodes)
        {
            var totals = DalBeautyCode.SelectBeautyCodeStatisticsTotalCount(conn, batchCodes);
            var counts = DalBeautyCode.SelectBeautyCodeStatisticsCount(conn, batchCodes);
            var batchStatus = DalBeautyCode.SelectBeautyCodeBatchTaskStatus(conn, batchCodes);
            var result = (from x in totals
                          join y in counts on new { x.BatchCode, x.Type, x.CreateUser, x.MappingId } equals new { y.BatchCode, y.Type, y.CreateUser, y.MappingId } into temp
                          from z in temp.DefaultIfEmpty()
                          select new BeautyCodeStatistics
                          {
                              BatchCode = x.BatchCode,
                              MappingId = x.MappingId,
                              CreateUser = x.CreateUser,
                              Count = (z?.Count).GetValueOrDefault(),
                              TotalCount = x.TotalCount,
                              Type = x.Type,
                              BuyoutOrderId = x.BuyoutOrderId,
                              Status = x.Status
                          }).ToList();
            result.ForEach(x => x.Status = batchStatus?.FirstOrDefault(s => s.BatchCode.Equals(x.BatchCode))?.Status);
            return result.OrderByDescending(x => x.BatchCode).ToList();
        }

        internal IEnumerable<CompanyUserSmsRecord> SelectCompanyUserSmsRecordByBatchCode(SqlConnection conn, string batchCode)
        {
            return DalBeautyCode.SelectCompanyUserSmsRecordByBatchCode(conn, batchCode);
        }

        internal bool InsertCompanyUserSmsRecord(SqlConnection conn, CompanyUserSmsRecord record)
        {
            return DalBeautyCode.InsertCompanyUserSmsRecord(conn, record);
        }

    }
}
