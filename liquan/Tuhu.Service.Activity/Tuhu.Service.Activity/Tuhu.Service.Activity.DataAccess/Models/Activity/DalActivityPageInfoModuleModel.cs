using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models.Activity;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    public class ActivityPageInfoModuleBase
    {
        [Column("Group")]
        public string RowNum { get; set; }

        /// <summary>
        /// 行类型 0是一行一列以此类推
        /// </summary>
        public int RowType { get; set; }

        /// <summary>
        /// 单元格号
        /// </summary>
        [Column("OrderBy")]
        public int CellNum { get; set; }
    }
    public class DalActivityPageInfoVehicleBanner : ActivityPageInfoRowBase
    {
        /// <summary>
        /// 是否区分车型，false不限，true分车型
        /// </summary>
        public bool IsVehicle { get; set; }

        /// <summary>
        /// --图片地址
        /// </summary>
        public string Image { get; set; }

        public string LinkUrl { get; set; }

        public string WxAppUrl { get; set; }
    }

    //public class DalActivityPageInfoProductPool : ActivityPageInfoRowBase
    //{
    //    public string Pid { get; set; }

    //    public Guid? ActivityId { get; set; }

    //    public string  SystemActivityId { get; set; }

    //    public string Brand { get; set; }

    //    public int ProductType { get; set; }

    //    public int VehicleLevel { get; set; }

    //    public int ActivityType { get; set; }

    //    public int RowLimit { get; set; }

    //    public int ColumnNumber { get; set; }

    //    public int DisplayWay { get; set; }

    //    public int IsAdapter { get; set; }
    //}

    public class DalActivityPageInfoImage : ActivityPageInfoRowBase
    {
        public string FileUrl { get; set; }
        /// <summary>
        /// --图片地址
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoLink : ActivityPageInfoRowBase
    {
        /// <summary>
        /// --跳转链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// --app跳转链接
        /// </summary>
        public string AppUrl { get; set; }

        /// <summary>
        /// --官网链接
        /// </summary>
        public string PcUrl { get; set; }

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }
        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }

        /// <summary>
        /// --图片地址
        /// </summary>
        public string Image { get; set; }
    }

    public class DalActivityPageInfoProduct : ActivityPageInfoRowBase
    {
        public string Pid { get; set; }

        public Guid? ActivityId { get; set; }
        /// <summary>
        /// 是否显示进度条
        /// </summary>
        public int DisplayWay { get; set; }
        /// <summary>
        /// 是否替换商品
        /// </summary>
        public int IsReplace { get; set; }
        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }

        public int Type { get; set; }
    }

    public class DalActivityPageInfoCoupon : ActivityPageInfoRowBase
    {
        public Guid Cid { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoBy : ActivityPageInfoRowBase
    {
        /// <summary>
        /// 途虎推荐
        /// </summary>
        public int? IsRecommended { get; set; }
        /// <summary>
        /// 强制登陆
        /// </summary>
        public int? IsLogin { get; set; }

        /// <summary>
        /// 轮胎规格
        /// </summary>
        public int? IsTireStandard { get; set; }

        /// <summary>
        ///  --是否开启轮胎适配 1:全部 2:轮胎 3:轮毂
        /// </summary>
        public int? IsTireSize { get; set; }

        /// <summary>
        /// 隐藏标题栏
        /// </summary>
        public int? IsHiddenTtile { get; set; }

        /// <summary>
        /// 保养服务
        /// </summary>
        public string ByService { get; set; }

        /// <summary>
        /// 保养活动id
        /// </summary>
        public string ByActivityId { get; set; }

        /// <summary>
        /// 车型要求
        /// </summary>
        public int? Vehicle { get; set; }

        /// <summary>
        /// --跳转链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// --app跳转链接
        /// </summary>
        public string AppUrl { get; set; }

        /// <summary>
        /// --官网链接
        /// </summary>
        public string PcUrl { get; set; }

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }
        public string Image { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoRule : ActivityPageInfoRowBase
    {
        public string Description { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }


    public class DalActivityPageInfoOther : ActivityPageInfoRowBase
    {
        /// <summary>
        /// --跳转链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// --app跳转链接
        /// </summary>
        public string AppUrl { get; set; }

        /// <summary>
        /// --官网链接
        /// </summary>
        public string PcUrl { get; set; }

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }

        /// <summary>
        /// 选项
        /// </summary>
        public int? OthersType { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoOtherActivity : ActivityPageInfoRowBase
    {
        public string ActivityId { get; set; }

        public string HashKey { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }

        public int Type { get; set; }
    }

    public class DalActivityPageInfoVideo : ActivityPageInfoRowBase
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// --跳转链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// --app跳转链接
        /// </summary>
        public string AppUrl { get; set; }

        /// <summary>
        /// --官网链接
        /// </summary>
        public string PcUrl { get; set; }

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 小程序跳转
        /// </summary>
        public string WxAppUrl { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoPintuan : ActivityPageInfoRowBase
    {
        public string ProductGroupId { get; set; }

        public string Pid { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoJson : ActivityPageInfoRowBase
    {
        /// <summary>
        /// 文字链，优惠券等配置内容json格式存储
        /// </summary>
        public string JsonContent { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }

        public int Type { get; set; }
    }

    public class DalActivityPageInfoActivityText : ActivityPageInfoRowBase
    {
        public string ActiveText { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoCountDown : ActivityPageInfoRowBase
    {
        public int CountDownStyle { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }
    }

    public class DalActivityPageInfoNewProductPool : ActivityPageInfoRowBase
    {
        /// <summary>
        /// 一行X列
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// 是否展示适配标签（适配过滤）
        /// </summary>
        public int IsAdapter { get; set; }

        public string Pid { get; set; }

        public string ProductName { get; set; }

        public Guid? ActivityId { get; set; }

        public string Category { get; set; }

        public int Type { get; set; }

        public int ProductType { get; set; }

        public string TireSize { get; set; }

        public int VehicleLevel { get; set; }

        public string SystemActivityId { get; set; }

        public int RowLimit { get; set; }

        public int ActivityType { get; set; }

        public int DisplayWay { get; set; }

        public string Brand { get; set; }
        /// <summary>
        /// 模板样式,1:极简版; 2:无按钮版;3:完整版 
        /// </summary>
        public int TemplateType { get; set; }

        /// <summary>
        /// 是否选择进度条
        /// </summary>
        public bool? IsProgressBar { get; set; }

        /// <summary>
        /// 是否选择品牌
        /// </summary>
        public bool? IsBrandName { get; set; }
    }

    public class DalActivityPageInfoProductPool : ActivityPageInfoRowBase
    {
        /// <summary>
        /// 一行X列
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// 是否展示适配标签（适配过滤）
        /// </summary>
        public int IsAdapter { get; set; }

        public string Pid { get; set; }

        public string ProductName { get; set; }

        public Guid? ActivityId { get; set; }

        public string Category { get; set; }

        public int Type { get; set; }

        public int ProductType { get; set; }

        public string TireSize { get; set; }

        public int VehicleLevel { get; set; }

        public string SystemActivityId { get; set; }

        public int RowLimit { get; set; }

        public int ActivityType { get; set; }

        public int DisplayWay { get; set; }

        public string Brand { get; set; }

        public string Tips { get; set; }
    }

    public class DalActivityPageInfoMenu : ActivityPageInfoRowBase
    {
        public int Pkid { get; set; }
    }
    /// <summary>
    /// --菜单内容
    /// </summary>
    public class DalActivityPageMenuModel : ActivityPageInfoRowBase
    {
        public int FKActiveContentID { get; set; }
        /// <summary>
        /// --菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }

        public int Sort { get; set; }
        /// <summary>
        /// --菜单开始值
        /// </summary>
        public string MenuValue { get; set; }

        /// <summary>
        /// --菜单结束值
        /// </summary>
        public string MenuValueEnd { get; set; }

        /// <summary>
        /// 导航菜单描述
        /// </summary>
        public string Description { get; set; }
    }

    public class DalActivityPageContent
    {
        public string Group { get; set; }
        /// <summary>
        /// 行类型 
        /// </summary>
        public int RowType { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 单元格号
        /// </summary>
        public int OrderBy { get; set; }
    }

    public class DalActivityPageInfoSeckill: ActivityPageInfoRowBase
    {
        public string Image { get; set; }
        /// <summary>
        /// 行类型 
        /// </summary>
        public int RowType { get; set; }

    }
}
