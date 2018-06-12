using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.inventory.adjust.external
    /// </summary>
    public class InventoryAdjustExternalRequest : BaseTopRequest<Top.Api.Response.InventoryAdjustExternalResponse>
    {
        /// <summary>
        /// biz_type
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 商家外部定单号
        /// </summary>
        public string BizUniqueCode { get; set; }

        /// <summary>
        /// 商品初始库存信息： [{"scItemId":"商品后端ID，如果有传scItemCode,参数可以为0","scItemCode":"商品商家编码","inventoryType":"库存类型 1：正常,”direction”: 1: 盘盈 -1: 盘亏,参数可选,"quantity":"数量(正数)"}]
        /// </summary>
        public string Items { get; set; }

        /// <summary>
        /// 库存占用返回的操作码.operate_type 为OUTBOUND时，如果是确认事先进行过的库存占用，需要传入当时返回的操作码，并且明细必须与申请时保持一致
        /// </summary>
        public string OccupyOperateCode { get; set; }

        /// <summary>
        /// 业务操作时间
        /// </summary>
        public Nullable<DateTime> OperateTime { get; set; }

        /// <summary>
        /// operate_type
        /// </summary>
        public string OperateType { get; set; }

        /// <summary>
        /// reduce_type
        /// </summary>
        public string ReduceType { get; set; }

        /// <summary>
        /// 商家仓库编码
        /// </summary>
        public string StoreCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.inventory.adjust.external";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("biz_unique_code", this.BizUniqueCode);
            parameters.Add("items", this.Items);
            parameters.Add("occupy_operate_code", this.OccupyOperateCode);
            parameters.Add("operate_time", this.OperateTime);
            parameters.Add("operate_type", this.OperateType);
            parameters.Add("reduce_type", this.ReduceType);
            parameters.Add("store_code", this.StoreCode);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("biz_unique_code", this.BizUniqueCode);
            RequestValidator.ValidateRequired("items", this.Items);
            RequestValidator.ValidateRequired("operate_time", this.OperateTime);
            RequestValidator.ValidateRequired("store_code", this.StoreCode);
        }

        #endregion
    }
}
