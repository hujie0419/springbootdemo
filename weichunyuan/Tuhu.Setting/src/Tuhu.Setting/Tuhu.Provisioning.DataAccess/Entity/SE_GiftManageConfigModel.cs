using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 实体-SE_GiftManageConfigModel 
    /// </summary>
    public class SE_GiftManageConfigModel : PageBase
    {
        /// <summary>
        /// 主键标识
        /// </summary>		
        public int Id { get; set; }
        public string Group { get; set; }
        /// <summary>
        /// 标识名称
        /// </summary>		
        public string Name { get; set; }

        /// <summary>
        /// 状态 true 启用 false 禁用
        /// </summary>		
        public bool State { get; set; } = true;

        /// <summary>
        /// 每人限购
        /// </summary>		
        public int Limit { get; set; }

        /// <summary>
        /// 赠送方式 1 不随单 2 随单
        /// </summary>		
        public int DonateWay { get; set; } = 1;

        /// <summary>
        /// 套餐描述
        /// </summary>		
        public string Describe { get; set; }

        /// <summary>
        /// 可见条件: true 所有人  false 电销可见 
        /// </summary>		
        public bool Visible { get; set; } = true;

        /// <summary>
        /// 订单安装方式：1 到店安装  2 上门安装 3 无需安装
        /// </summary>		
        public string OrdersWay { get; set; }

        /// <summary>
        /// 开始有效时间
        /// </summary>		
        public DateTime ValidTimeBegin { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束有效时间
        /// </summary>		
        public DateTime ValidTimeEnd { get; set; } = DateTime.Now;

        /// <summary>
        /// 渠道
        /// </summary>		
        public string Channel { get; set; }

        /// <summary>
        /// 类型：1 按轮胎尺寸 2 按轮毂尺寸 3 按品类 4 按PID 5 按轮胎类型
        /// </summary>		
        public int Type { get; set; } = 1;

        /// <summary>
        /// 轮胎or轮毂 尺寸条件 : 1 大于 2 大于等于 3 等于 4 小于 5 小于等于
        /// </summary>		
        public int ConditionSize { get; set; } = 1;

        /// <summary>
        /// 轮胎类型 1 所有轮胎 2 防爆胎 3 非防爆胎
        /// </summary>
        public int TireType { get; set; } = 1;

        /// <summary>
        /// 轮胎 尺寸条件 : 1 大于 2 大于等于 3 等于 4 小于 5 小于等于
        /// </summary>
        public int TireSizeCondition { get; set; }

        /// <summary>
        /// 轮胎尺寸 
        /// </summary>
        public string TireSize { get; set; }

        /// <summary>
        /// 轮胎or轮毂 尺寸
        /// </summary>		
        public string Size { get; set; }

        /// <summary>
        /// 品类 : 按类别
        /// </summary>		
        public string B_Categorys { get; set; }

        /// <summary>
        /// 品类 :  品牌
        /// </summary>		
        public string B_Brands { get; set; }

        /// <summary>
        /// 品类 : PID
        /// </summary>		
        public string B_PID { get; set; }

        /// <summary>
        /// 品类 : True 增加PID  False 排除PID
        /// </summary>		
        public bool B_PID_Type { get; set; }

        /// <summary>
        /// 按PID
        /// </summary>		
        public string P_PID { get; set; }

        /// <summary>
        /// 赠品优惠 条件 ： 1 满 2 大于
        /// </summary>		
        public int GiftCondition { get; set; }

        /// <summary>
        /// 赠品优惠 数量
        /// </summary>		
        public int GiftNum { get; set; }

        /// <summary>
        /// 赠品优惠  优惠金额
        /// </summary>		
        public decimal GiftMoney { get; set; } = 0.00M;

        /// <summary>
        /// 赠品优惠 单位: 1 件 2 元
        /// </summary>		
        public int GiftUnit { get; set; }

        /// <summary>
        /// 赠品优惠 类型 : 1 减xxx元  2 送赠品
        /// </summary>		
        public string GiftType { get; set; }

        /// <summary>
        /// 送赠品
        /// </summary>		
        public string GiftProducts { get; set; }

        /// <summary>
        /// 优惠说明
        /// </summary>		
        public string GiftDescribe { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>		
        public string Creater { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>		
        public string Mender { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>		
        public DateTime? UpdateTime { get; set; }

        public string Category { get; set; }

        /// <summary>
        /// 是否套装产品
        /// </summary>
        public bool IsPackage { get; set; }

        /// <summary>
        /// 1:赠品；2:打折
        /// </summary>
        public int ActivityType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否标签显示
        /// </summary>
        public bool TagDisplay { get; set; }

        /// <summary>
        /// 是否买三送一 1是 0否
        /// </summary>
        public bool GiveAway { get; set; }

    }
}