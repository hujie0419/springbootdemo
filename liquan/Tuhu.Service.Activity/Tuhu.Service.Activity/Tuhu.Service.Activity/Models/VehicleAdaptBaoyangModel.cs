using System;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class VehicleAdaptBaoyangModel : BaseModel
    {
        ///<summary>主键</summary>
        public int PKID { get; set; }


        ///<summary> 车型Id </summary>
        public string VehicleId { get; set; }

        ///<summary>
        /// 七个自然日销量统计分数
        /// </summary>

        public int? SalesOrder { get; set; }

        /// <summary>图片</summary>
        public string Image { get; set; }

        ///<summary>最低价格</summary>

        public decimal? MinPrice { get; set; }


        /// <summary>保养项目分类</summary>
        public string BaoyangType { get; set; }

        /// <summary>
        /// 移动端链接
        /// </summary>
        public string MobileLine { get; set; }

        /// <summary>
        ///Ios处理值
        /// </summary>
        public string IProcessValue { get; set; }

        /// <summary>
        /// Android处理值
        /// </summary>
        public string AProcessValue { get; set; }

        /// <summary>
        /// Ios通信值
        /// </summary>
        public string ICommunicationValue { get; set; }

        /// <summary>
        /// Android通信值
        /// </summary>
        public string ACommunicationValue { get; set; }
    }
}
