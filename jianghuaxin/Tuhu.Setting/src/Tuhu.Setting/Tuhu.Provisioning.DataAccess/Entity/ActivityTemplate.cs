using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class NewAppData
    {
        public int id { get; set; }

        public int apptype { get; set; }

        public string modelname { get; set; }

        public int modelfloor { get; set; }

        public int showstatic { get; set; }

        public int showorder { get; set; }

        public string appname { get; set; }

    }
    public class CategoryDic
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }
    }

    public class ActivityBuild
    {
        public int id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ActivityUrl { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string BgImageUrl { get; set; }

        /// <summary>
        /// 小图地址
        /// </summary>
        public string SBgImageUrl { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public string BgColor { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string TireBrand { get; set; }

        /// <summary>
        /// 活动类型 0 APP 1微信 3轮胎
        /// </summary>
        public int ActivityType { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 是否显示倒计时
        /// </summary>
        public int? IsShowDate { get; set; }

        /// <summary>
        /// 是否开启轮胎尺寸适配
        /// </summary>
        public int? IsTireSize { get; set; }

        /// <summary>
        ///  轮胎尺寸适配 配置信息
        /// </summary>
        public string TireSizeConfig { get; set; }


        /// <summary>
        /// 菜单
        /// </summary>
        public string ActivityMenu { get; set; }

        /// <summary>
        /// 活动会场
        /// </summary>
        public string ActivityHome { get; set; }

        /// <summary>
        /// 分享参数
        /// </summary>
        public string DataParames { get; set; }

        /// <summary>
        /// 大型活动会场
        /// </summary>
        public string BigActivityHome { get; set; }

        /// <summary>
        /// 菜单类型 -1没有菜单 0滚动菜单 1滑动菜单
        /// </summary>
        public int MenuType { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string SelKeyName { get; set; }


        /// <summary>
        /// 关键词图片
        /// </summary>
        public string SelKeyImage { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatetorUser { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        ///  0:综合 1：车品 2：保养  3：轮胎  4：美容改装 5:外部投放活动
        /// </summary>
        public int ActivityConfigType { get; set; }


        public DateTime? StartDT { get; set; }


        public string PersonWheel { get; set; }

        public bool IsNew { get; set; }

        public string HashKey { get; set; }

    }

    public class BigFHomeDeatil
    {

        public Int64 BigFHomeGroup { get; set; }

        public string BigFHomeName { get; set; }

        public string ImgFHomePic { get; set; }
        public string HidBigFHomePic { get; set; }

        public string BigFHomeMobileUrl { get; set; }

        public string BigFHomeWwwUrl { get; set; }

        public int BigFHomeOrder { get; set; }

    }


    public class BigFHome
    {
        public string    FHomeName { get; set; }

        public string ImageFHome { get; set; }

        public string HidImageFHome { get; set; }


        public Int64 BigFHomeGroup { get; set; }

        public List<BigFHomeDeatil> Items { get; set; }
    }


    public class BigActivityHome
    {
        public string BigHomeName { get; set; }

        public string ImgBigHomePic { get; set; }

        public string HidBigHomePic { get; set; }

        public string ImgBigHomePicWww { get; set; }

        public string HidBigHomePicWww { get; set; }

        public string BigHomeUrl { get; set; }

        public string BigHomeUrlWww { get; set; }


        public List<BigFHome> Rows { get; set; }

    }

    public class TireSizeConfig
    {
        /// <summary>
        /// 可换车 1可换 
        /// </summary>
        public int IsChangeTire { get; set; }

        /// <summary>
        /// 可换轮胎规格 1可换
        /// </summary>
        public int IsChangeTireSize { get; set; }

        /// <summary>
        /// 实现适配标签 1：显示
        /// </summary>
        public int IsShowTag { get; set; }

        /// <summary>
        /// 有边框 1:有
        /// </summary>
        public int IsMargin { get; set; }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string MarginColor { get; set; }

        /// <summary>
        /// 填充颜色
        /// </summary>
        public string FillColor { get; set; }

        /// <summary>
        /// 提示语颜色
        /// </summary>

        public string PromptColor { get; set; }

        /// <summary>
        /// 提示语字号
        /// </summary>
        public string PromptFontSize { get; set; }

        /// <summary>
        /// 车主信息颜色
        /// </summary>

        public string CarInfoColor { get; set; }

        /// <summary>
        /// 车主信息字号
        /// </summary>

        public string CarInfoFontSize { get; set; }


        /// <summary>
        /// 无车型提示语
        /// </summary>
        public string NoCarTypePrompt { get; set; }

        /// <summary>
        /// 无规格提示语
        /// </summary>
        public string NoFormatPrompt { get; set; }



    }



}
