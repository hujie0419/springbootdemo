using System;
using Tuhu.Component.ExportImport;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    [TableExportImport(ExportImportName = "门店列表")]
    public class ShopsList
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int OrderNo { get; set; }
        [ColumnExportImport(ExportImportName = "门店简单名")]
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string SimpleName { get; set; }



        public string CarparName { get; set; }
        public string CompanyName { get; set; }
        public string FullName { get; set; }
        [ColumnExportImport(ExportImportName = "省")]
        public string Province { get; set; }
        [ColumnExportImport(ExportImportName = "市")]
        public string City { get; set; }
        [ColumnExportImport(ExportImportName = "区")]
        public string District { get; set; }
        [ColumnExportImport(ExportImportName = "门店地址")]
        public string Address { get; set; }
        public string Post { get; set; }
        [ColumnExportImport(ExportImportName = "联系人")]
        public string Contact { get; set; }
        [ColumnExportImport(ExportImportName = "手机")]
        public string Telephone { get; set; }
        [ColumnExportImport(ExportImportName = "电话")]
        public string Mobile { get; set; }
        public string Brand { get; set; }
        public string Service { get; set; }
        public bool IsActive { get; set; }
        [ColumnExportImport(ExportImportName = "门店类型")]
        public string ShopTypeName { get; set; }
        public string AddressBrief { get; set; }
        public string Submitor { get; set; }
        [ColumnExportImport(ExportImportName = "责任人")]
        public string ResponsiblePerson { get; set; }
        public string Examiner { get; set; }
        [ColumnExportImport(ExportImportName = "上下架状态")]
        public string IsActiveStr { get; set; }

        public string WorkTime { get; set; }
        public string Description { get; set; }
        public int ShopType { get; set; }
        public string Cover { get; set; }
        public string Position { get; set; }
        public string Pos { get; set; }
        public decimal FeePerTire { get; set; }
        public decimal FeePerFrontBrake { get; set; }
        public decimal FeePerRearBrake { get; set; }
        public decimal FeePerFrontDisc { get; set; }
        public decimal FeePerRearDisc { get; set; }
        public decimal FeePerMaintain { get; set; }
        public decimal FeePer4Wheel { get; set; }
        public int LakalaNo { get; set; }
        public decimal FeePmInstallation { get; set; }

        public int DailyOrderQuantity { get; set; }
        public string Images { get; set; }
        public string EmailAddress { get; set; }
        public string SubmitorTel { get; set; }
        public string FailedExaminedReason { get; set; }
        public int DailyOrderUpperLimit { get; set; }
        public string ResponsiblePersonTel { get; set; }
        public string ExaminerTel { get; set; }
        public string Category { get; set; }
        public DateTime? SuspendStartDateTime { get; set; }
        public string SuspendStartDate { get; set; }
        public DateTime? SuspendEndDateTime { get; set; }
        public string SuspendEndDate { get; set; }
        public DateTime? GetThroughDateTime { get; set; }
        public DateTime? LastStartBusinessDateTime { get; set; }
        public DateTime? LasePauseBusinessDateTime { get; set; }
        public int FirstPriority { get; set; }

        public string ExpCompany { get; set; } //非到店快递
        public string ArriveShopExpCo { get; set; } //到店快递
        public string LogisticCo { get; set; }  //物流公司
        [ColumnExportImport(ExportImportName = "对账人人")]
        public string AccountingPerson { get; set; }//门店对账人
        public string AccountingPeriod { get; set; }

        public int TotalCount { get; set; } //对账分页总数

        public int RegionID { get; set; }

        public int Num { get; set; }

        /// <summary>
        /// 门店营业类型
        /// </summary>
        public string ShopBusinessType { get; set; }

        #region 2016/1/21添加
        /// <summary>
        /// 配置费用
        /// </summary>
        public double SettlementFee { get; set; }
        /// <summary>
        /// 配置类型
        /// </summary>
        public string SettlementType { get; set; }
        #endregion


    }
}