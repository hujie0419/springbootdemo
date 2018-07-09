using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2
{
    public class LotteryGroupInfo
    {
        public string ProductGroupId { get; set; }
        public int GroupType { get; set; }
        public int GroupCategory { get; set; }
        public string ProductName { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string GroupDescription { get; set; }
    }


    public class LotteryUserModel
    {
        public string ProductGroupId { get; set; }
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public Guid UserId { get; set; }
        public string UserPhone { get; set; }
        public int LotteryResult { get; set; }
    }

    public class LotteryCouponModel
    {
        public string ProductGroupId { get; set; }
        public Guid CouponId { get; set; }
        public string CouponDesc { get; set; }
        public string CouponCondition { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string UsefulLife { get; set; }
        public string Creator { get; set; }
    }


    public class LotteryLogInfo
    {
        public string Operator { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string OperateType { get; set; }
        public string Remark { get; set; }
    }

    public class LotteryUserInfo
    {
        public int PKID { get; set; }
        public int OrderId { get; set; }
        public Guid UserId { get; set; }
        public string ProductGroupId { get; set; }
    }

    public class GroupBuyingStockModel
    {
        public string PID { get; set; }
        public int SHAvailableStockQuantity { get; set; }
        public int SHZaituStockQuantity { get; set; }
        public decimal SHStockCost { get; set; }
        public int WHAvailableStockQuantity { get; set; }
        public int WHZaituStockQuantity { get; set; }
        public decimal WHStockCost { get; set; }
        public int BeiJingAvailableStockQuantity { get; set; }
        public int BeiJingZaituStockQuantity { get; set;}
        public int GuangZhouAvailableStockQuantity { get; set; }
        public int GuangZhouZaituStockQuantity { get; set; }
        public int YiWuAvailableStockQuantity { get; set; }
        public int YiWuZaituStockQuantity { get; set; }
        public int TotalAvailableStockQuantity { get; set; }
        public int TotalZaituStockQuantity { get; set; }
    }

    public class ProductStockInfo
    {
        public string PID { get; set; }
        public int WAREHOUSEID { get; set; }
        public int TotalAvailableStockQuantity { get; set; }
        public decimal StockCost { get; set; }
        public int CaigouZaitu { get; set; }
    }


    public class GroupBuyingExportInfo
    {
        public string ProductGroupId { get; set; }
        public int GroupType { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsShow { get; set; }
        public int MemberCount { get; set; }
        public int TotalGroupCount { get; set; }
        public int CurrentGroupCount { get; set; }
        public int GroupCategory { get; set; }
        public string Creator { get; set; }
        public string PID { get; set; }
        public string ProductName { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal SpecialPrice { get; set; }

        public string Label { get; set; }
    }

}