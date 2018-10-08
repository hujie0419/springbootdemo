using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PromotionModel : BaseModel
    {
        public PromotionModel() { }
        public PromotionModel(DataRow row) : base(row) { }
        /// <summary>
        /// 主键  且为优惠券的类型
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// Type 
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// 优惠券备注 内部可见
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 优惠券应用的类型
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 优惠券应用的产品
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 优惠券应用的品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 所属的父类
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 是否可使用
        /// </summary>
        public int IsActive { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastDateTime { get; set; }
        /// <summary>
        /// 安装方式
        /// </summary>
        public int InstallType { get; set; }

        /// <summary>
        /// pid类型  0 false 排除PID  1true 增加PID
        /// </summary>
        public bool PIDType { get; set; }
        public int HrefType { get; set; }

        /// <summary>
        ///优惠券类型 0 满减  1购物返券  2 固定价格
        /// </summary>
        public int PromotionType { get; set; }

        /// <summary>
        /// 门店类型
        /// </summary>
        public int ShopType { get; set; }

        public int ShopID { get; set; }
        public string ShopName { get; set; }

        public string CustomSkipPage { get; set; }

        public string WxSkipPage { get; set; }

        public string H5SkipPage { get; set; }
        
        /// <summary>
        /// 配置项类型（1=Category;2=Brand;4=PID；8=门店类型;16=ShopId）
        /// </summary>
        public int ConfigType { get; set; }
        /// <summary>
        /// 优惠券分类（0||null=普通券；1=门店券）
        /// </summary>
        public int CouponType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public OrderPayMethodEnum OrderPayMethod { get; set; }

        /// <summary>
        /// 商品配置
        /// </summary>
        public List<CouponRulesConfigProduct> ProductsConfig { get; set; }
        /// <summary>
        /// 门店配置
        /// </summary>
        public List<CouponRulesConfigShop> ShopConfig { get; set; }
        /// <summary>
        /// 拼团购是否可用
        /// </summary>
        public bool EnabledGroupBuy { get; set; }
        /// <summary>
        /// 使用规则说明
        /// </summary>
        public string RuleDescription { get; set; }
    }

    public enum OrderPayMethodEnum
    {
        不限,
        在线支付,
        到店支付
    }

    public class GetPCodeModel : PromotionModel
    {

        public GetPCodeModel(DataRow row) : base(row) { }
        public GetPCodeModel() { }

        /// <summary>
        /// ID
        /// </summary>
        public int GETPKID { get; set; }

        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 发放的优惠券的名字
        /// </summary>
        public string PromtionName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? Discount { get; set; }
        /// <summary>
        /// 满多少元减
        /// </summary>
        public decimal? Minmoney { get; set; }
        /// <summary>
        /// 使用有效期开始时间
        /// </summary>
        public DateTime? ValiStartDate { get; set; }
        /// <summary>
        /// 使用有效期结束时间
        /// </summary>
        public DateTime? ValiEndDate { get; set; }
        /// <summary>
        /// 允许发放优惠券的渠道  1允许电销发放  0不允许电销方法
        /// </summary>
        public int AllowChanel { get; set; }
        /// <summary>
        /// 发放量
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// 提醒快抢光的警戒线
        /// </summary>
        public int? RemindQuantity { get; set; }
        /// <summary>
        /// 需要提醒的邮箱
        /// </summary>
        public string RemindEmails { get; set; }
        /// <summary>
        /// 已领取量
        /// </summary>
        public int GetQuantity { get; set; }

        /// <summary>
        /// 有效期(自领取时间向后推算)
        /// </summary>
        public int? Term { get; set; }
        /// <summary>
        /// 每人限领数量
        /// </summary>
        public int SingleQuantity { get; set; }

        public int RuleID { get; set; }
        /// <summary>
        /// 电销可用的开始时间
        /// </summary>
        public DateTime? DXStartDate { get; set; }
        /// <summary>
        /// 电销可用的结束时间
        /// </summary>
        public DateTime? DXEndDate { get; set; }
        /// <summary>
        /// 0全部 1新用户 2老用户
        /// </summary>
        public int SupportUserRange { get; set; }
        /// <summary>
        /// 在详情页显示的开始时间
        /// </summary>
        public DateTime? DetailShowStartDate { get; set; }
        /// <summary>
        /// 在详情页显示的结束时间
        /// </summary>
        public DateTime? DetailShowEndDate { get; set; }

        /// <summary>
        /// 发放渠道
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 用途id
        /// </summary>
        public int IntentionId { get; set; }
        /// <summary>
        /// 用途名称
        /// </summary>
        public string IntentionName { get; set; }
        /// <summary>
        /// 业务线id
        /// </summary>
        public int BusinessLineId { get; set; }
        /// <summary>
        /// 业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 最终有效期
        /// </summary>
        public DateTime? DeadLineDate { get; set; }
        /// <summary>
        /// 是否到期提醒
        /// </summary>
        public int IsPush { get; set; }
        /// <summary>
        /// 多少天前提醒 逗号隔开
        /// </summary>
        public string PushSetting { get; set; }
    }
    public class Category : BaseModel
    {
        public Category() { }
        public Category(DataRow row) : base(row) { }
        public string CategoryName { get; set; }
        public string DisplayName { get; set; }
        public int oid { get; set; }
        public int ParaentOid { get; set; }
        public string Description { get; set; }
        public string NodeNo { get; set; }
        public IEnumerable<Category> ParentCategory { get; set; }
        public IEnumerable<Category> ChildrenCategory { get; set; }
        public int DescendantProductCount { get; set; }



        public static IEnumerable<Category> Parse(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return new Category[0];
            var model = dt.Rows.Cast<DataRow>().Select(row => new Category()
            {
                CategoryName = Convert.ToString(row["CategoryName"]),
                DisplayName = Convert.ToString(row["DisplayName"]),
                oid = Convert.ToInt32(row["oid"]),
                ParaentOid = Convert.ToInt32(row["ParaentOid"]),
                Description = Convert.ToString(row["Description"]),
                NodeNo = Convert.ToString(row["NodeNo"]),
                ChildrenCategory = new List<Category>()
            }).ToArray();
            foreach (var category in model)
            {
                category.ParentCategory = model.Where(c => category.ParaentOid == c.oid);
                category.ChildrenCategory = model.Where(c => c.ParaentOid == category.oid);
            }
            return model;
        }
    }

    public class ProductBrand : BaseModel
    {
        public ProductBrand() { }
        public ProductBrand(DataRow row) : base(row) { }
        public string PrimaryParentCategory { get; set; }
        public string Cp_Brand { get; set; }
        public string ProductID { get; set; }
    }
    public class PromotionFilterConditionModel
    {
        public int PageIndex{ get; set; }=1;
        public int PageSize { get; set; } = 100;
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public int? RuleID { get; set; }

        /// <summary>
        /// 优惠券备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public int? OrderType { get; set; }
        /// <summary>
        /// 优惠券类型
        /// </summary>
        public int? PromotionType { get; set; }
        /// <summary>
        /// 领券规则ID
        /// </summary>
        public int? GetRuleID { get; set; }
        /// <summary>
        /// 领券规则GUID
        /// </summary>
        public Guid? GetRuleGUID { get; set; }
        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string PromotionName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        public decimal? Minmoney_Min { get; set; }
        public decimal? Minmoney_Max { get; set; }
        public decimal? Discount_Min { get; set; }
        public decimal? Discount_Max { get; set; }
        /// <summary>
        /// 是否电销可用
        /// </summary>
        public int? AllowChanel { get; set; }
        /// <summary>
        /// 支持用户范围
        /// </summary>
        public int? SupportUserRange { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 用途id
        /// </summary>
        public string IntentionId { get; set; }
    }

    public class DepartmentAndUseModel : BaseModel
    {
        /// <summary>
        /// 配置id
        /// </summary>
        public int SettingId { get; set; }
        /// <summary>
        /// 展示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 父id
        /// </summary>
        public int ParentSettingId { get; set; }
        /// <summary>
        /// 类型id 0=部门   1=用途
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string ParentName { get; set; }

        public IEnumerable<DepartmentAndUseModel> SunItems { get; set; }
    }

    public class CouponRulesConfigProduct : BaseModel
    {
        public CouponRulesConfigProduct(DataRow dr)
        {
            base.Parse(dr);
        }
        public int PKID { get; set; }
        public int RuleID { get; set; }
        /// <summary>
        /// 1=Category;2=Brand;4=PID
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue { get; set; }
        /// <summary>
        /// 单独从业务逻辑赋值
        /// </summary>
        public string ProductName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
    public class CouponRulesConfigShop : BaseModel
    {
        public CouponRulesConfigShop(DataRow dr)
        {
            base.Parse(dr);
        }
        public int PKID { get; set; }
        public int RuleID { get; set; }
        /// <summary>
        /// 8=门店类型;16=ShopId
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }

        public string ShopName { get; set; }
    }
}
