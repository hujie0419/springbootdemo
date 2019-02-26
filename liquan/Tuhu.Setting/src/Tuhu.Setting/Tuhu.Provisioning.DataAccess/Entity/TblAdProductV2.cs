using System;
using System.ComponentModel;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 产品表
    /// </summary>
    public class TblAdProductV2 : BasePrimaryKey
    {
        /// <summary>
        /// 产品Id
        /// </summary>
        [DisplayName("产品Id")]
        public string PId { get; set; }

        /// <summary>
        /// 产品在App上的位置
        /// </summary>
        [DisplayName("显示顺序")]
        public int? Position { get; set; }

        /// <summary>
        /// 产品状态 （0 : 禁用  1:启用）
        /// </summary>
        [DisplayName("产品状态")]
        public int? State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary
        [DisplayName("更新时间")]
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 产品价格
        /// </summary>
        [DisplayName("产品价格")]
        public decimal? PromotionPrice { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        [DisplayName("产品数量")]
        public int? PromotionNum { get; set; }

        /// <summary>
        /// 产品所对应的模块Id
        /// </summary>
        [DisplayName("产品所对应的模块Id")]
        public int? NewAppSetDataId { get; set; }

        [DisplayName("产品对应的活动Id")]
        public string ActivityId { get; set; }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(base.ToString());
            builder.AppendLine(string.Format("PId : {0}", PId));
            builder.AppendLine(string.Format("Position : {0}", Position));
            builder.AppendLine(string.Format("State : {0}", State));
            builder.AppendLine(string.Format("CreateDateTime : {0}", CreateDateTime));
            builder.AppendLine(string.Format("LastUpdateDateTime : {0}", LastUpdateDateTime));
            builder.AppendLine(string.Format("PromotionPrice : {0}", PromotionPrice));
            builder.AppendLine(string.Format("PromotionNum : {0}", PromotionNum));
            builder.AppendLine(string.Format("NewAppSetDataId : {0}", NewAppSetDataId));
            builder.AppendLine(string.Format("ActivityId : {0}", ActivityId));
            return builder.ToString();
        }

    }
}
