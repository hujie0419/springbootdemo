const apis = {
    // 资源上传接口，图片、视频上传
    FileUpload: '/ActivitySetting/Activity/FileUpload',

    // 用于输入PID时校验PID 是否有效，同时返回商品价格
    GetProductPrice: '/ActivitySetting/Activity/GetProductPrice',

    // 当PromotionType为Activity,验证活动id，当PromotionType为Maintenance，验证保养id
    GetPromotionProductPrice: '/ActivitySetting/Activity/GetPromotionProductPrice',

    // 根据日志来源id查询该id下的操作
    GetOperationLogList: '/ActivitySetting/Activity/GetOperationLogList',

    // 根据操作日志PKID 获取该条操纵下所以操作详情
    GetOperationLogDetailList: '/ActivitySetting/Activity/GetOperationLogDetailList'
};

export default apis;
