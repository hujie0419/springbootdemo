using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ActivityBoard
{
    public class BIActivityPageModel
    {
        /// <summary>
        /// 渠道  h5_android_app   weixin  tuhu_wap h5_ios_app
        /// </summary>
        public string APP_ID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string ClickTime_Varchar { get; set; }
        /// <summary>
        /// PV
        /// </summary>
        public int PV { get; set; }
        /// <summary>
        /// UV
        /// </summary>
        public int UV { get; set; }
        /// <summary>
        /// 点击UV
        /// </summary>
        public int ClickUV { get; set; }
        /// <summary>
        /// 注册用户数
        /// </summary>
        public int RegistrationNum { get; set; }
        /// <summary>
        /// 领券人数
        /// </summary>
        public int PromotionUV { get; set; }
        /// <summary>
        /// 下单UV
        /// </summary>
        public int OrederUV { get; set; }
        /// <summary>
        /// 订单数
        /// </summary>
        public int Ordernum { get; set; }
        /// <summary>
        /// 保养下单UV
        /// </summary>
        public int OrderUV_BY { get; set; }
        /// <summary>
        /// 保养下单数
        /// </summary>
        public int OrderNum_BY { get; set; }
        /// <summary>
        /// 保养下单转换率
        /// </summary>
        public decimal OrderPercent_BY { get; set; }
        /// <summary>
        /// 轮胎订单数
        /// </summary>
        public int Ordernum_Tire { get; set; }
        /// <summary>
        /// 保养订单数
        /// </summary>
        public int Ordernum_Maintenance { get; set; }
        /// <summary>
        /// 车品订单数
        /// </summary>
        public int Ordernum_CarProduct { get; set; }
        /// <summary>
        /// 美容轮毂订单数
        /// </summary>
        public int Ordernum_Hubbeauty { get; set; }
        /// <summary>
        /// 其他订单数
        /// </summary>
        public int Ordernum_Others { get; set; }
        /// <summary>
        /// 轮胎销售额
        /// </summary>

        public decimal SalesAmount1_Tire { get; set; }
        /// <summary>
        /// 保养销售额（保养废弃）
        /// </summary>
        public decimal SalesAmount1_Maintenance { get; set; }
        /// <summary>
        /// 保养销售额
        /// </summary>
        public decimal SalesAmount1_BY { get; set; }
        /// <summary>
        /// 车品销售额
        /// </summary>
        public decimal SalesAmount1_CarProduct { get; set; }
        /// <summary>
        /// 美容轮毂销售额
        /// </summary>
        public decimal SalesAmount1_Hubbeauty { get; set; }
        /// <summary>
        /// 其他销售额
        /// </summary>
        public decimal SalesAmount1_Others { get; set; }
        /// <summary>
        /// 轮胎去券销售额
        /// </summary>
        public decimal SalesAmount2_Tire { get; set; }
        /// <summary>
        /// 保养去券销售额（保养废弃）
        /// </summary>
        public decimal SalesAmount2_Maintenance { get; set; }
        /// <summary>
        /// 保养去券销售额
        /// </summary>
        public decimal SalesAmount2_BY { get; set; }
        /// <summary>
        /// 车品去券销售额
        /// </summary>
        public decimal SalesAmount2_CarProduct { get; set; }
        /// <summary>
        /// 美容轮毂去券销售额
        /// </summary>
        public decimal SalesAmount2_Hubbeauty { get; set; }
        /// <summary>
        /// 其他去券销售额
        /// </summary>
        public decimal SalesAmount2_Others { get; set; }
        /// <summary>
        /// 点击转化率
        /// </summary>
        public decimal ClickPercent { get; set; }
        /// <summary>
        /// 下单转化率
        /// </summary>
        public decimal OrderPercent { get; set; }
        /// <summary>
        /// 轮胎毛利率
        /// </summary>
        public decimal SalesPercent1_Tire { get; set; }
        /// <summary>
        /// 保养毛利率（保养废弃）
        /// </summary>
        public decimal SalesPercent1_Maintenance { get; set; }
        /// <summary>
        /// 保养毛利率
        /// </summary>
        public decimal SalesPercent1_BY { get; set; }
        /// <summary>
        /// 车品毛利率
        /// </summary>
        public decimal SalesPercent1_CarProduct { get; set; }
        /// <summary>
        /// 美容轮毂毛利率
        /// </summary>
        public decimal SalesPercent1_Hubbeauty { get; set; }
        /// <summary>
        /// 其他毛利率
        /// </summary>
        public decimal SalesPercent1_Others { get; set; }
        /// <summary>
        /// 轮胎去券毛利率
        /// </summary>
        public decimal SalesPercent2_Tire { get; set; }
        /// <summary>
        /// 保养去券毛利率（保养废弃）
        /// </summary>
        public decimal SalesPercent2_Maintenance { get; set; }
        /// <summary>
        /// 保养去券毛利率
        /// </summary>
        public decimal SalesPercent2_BY { get; set; }
        /// <summary>
        /// 车品去券毛利率
        /// </summary>
        public decimal SalesPercent2_CarProduct { get; set; }
        /// <summary>
        /// 美容轮毂去券毛利率
        /// </summary>
        public decimal SalesPercent2_Hubbeauty { get; set; }
        /// <summary>
        /// 其他去券毛利率
        /// </summary>
        public decimal SalesPercent2_Others { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? LoadDate { get; set; }
        /// <summary>
        /// 轮胎成本
        /// </summary>
        public decimal Cost_Tire { get; set; }
        /// <summary>
        /// 保养成本（保养废弃）
        /// </summary>
        public decimal Cost_Maintenance { get; set; }
        /// <summary>
        /// 保养成本
        /// </summary>
        public decimal Cost_BY { get; set; }
        /// <summary>
        /// 车品成本
        /// </summary>
        public decimal Cost_CarProduct { get; set; }
        /// <summary>
        /// 美容轮毂成本
        /// </summary>
        public decimal Cost_Hubbeauty { get; set; }
        /// <summary>
        /// 其他成本
        /// </summary>
        public decimal Cost_Others { get; set; }
    }
}
