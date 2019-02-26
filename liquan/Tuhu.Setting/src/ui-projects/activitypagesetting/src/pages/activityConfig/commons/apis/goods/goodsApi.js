const apis = {
    SaveGeneralProductSetting: '/ActivitySetting/Activity/SaveGeneralProductSetting',
    SavePushProductSetting: '/ActivitySetting/Activity/SavePushProductSetting',
    GetProductBrandsByCategory: '/ActivitySetting/Activity/GetProductBrandsByCategory', // 根据商品类目查询品牌集合接口
    GetSpikeSessions: '/ActivitySetting/Activity/GetSpikeSessions', // 秒杀商品场次查询接口
    SaveSpikeSessions: '/ActivitySetting/Activity/SaveSpikeSessions', // 秒杀商品保存接口
    GetGeneralProducts: '/ActivitySetting/Activity/GetGeneralProducts', // 普通商品数据源查询接口
    SaveGeneralProductAssociations: '/ActivitySetting/Activity/SaveGeneralProductAssociations', // 普通商品关联保存接口
    EditGeneralProductSortGroupId: '/ActivitySetting/Activity/EditGeneralProductSortGroupId', // 编辑关联商品组号接口
    GetGeneralProductAssociations: '/ActivitySetting/Activity/GetGeneralProductAssociations' // 普通商品模块关联商品查询接口
};
export default apis;
