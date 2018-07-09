namespace Tuhu.Provisioning.DataAccess.Entity
{
    public enum MonitorModule
    {
        [EnumDescription("订单")]
        Order = 0,
        [EnumDescription("采购")]
        Purchase = 1,
        [EnumDescription("仓库")]
        WareHouse = 2,
        [EnumDescription("物流")]
        Logistic = 3,
        [EnumDescription("财务")]
        Financial = 4,
        [EnumDescription("门店")]
        Shop = 5,
        [EnumDescription("其他")]
        Other = 6,
        [EnumDescription("JobService")]
        JobService = 7,
        [EnumDescription("数据查询")]
        DataSearch = 8,
        [EnumDescription("保养数据")]
        BaoYangData = 9,
        [EnumDescription("PMS数据")]
        Pms = 10,
        [EnumDescription("地址")]
        Address = 11,
        [EnumDescription("用户")]
        UserObject = 12,
        [EnumDescription("产品")]
        Product = 13,
        [EnumDescription("订单产品")]
        OrderList = 14,
        [EnumDescription("途虎工场门店")]
        TuhuShop=15,
        [EnumDescription("优惠券")]
        PromotionCode = 16,
        [EnumDescription("快修服务码")]
        ServiceCode = 17,

        [EnumDescription("途虎工单")]
        TuhuWorkOrder = 18
    }

}
