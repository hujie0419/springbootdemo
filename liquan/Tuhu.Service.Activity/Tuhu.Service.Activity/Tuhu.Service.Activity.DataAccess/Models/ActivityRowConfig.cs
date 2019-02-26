using System;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    public class ActivityCellConfig
    {
        public ActivityCellConfig()
        {
            this.Image = Image.GetImageUrl();
        }
        public int Pkid { get; set; }

        public int FkActiveId { get; set; }
        #region 通用
        /// <summary>
        /// 组号
        /// </summary>
        public string Group { get; set; }

        ///// <summary>
        ///// --渠道 
        ///// </summary>
        //public string Channel { get; set; }

        /// <summary>
        /// 行类型 
        /// </summary>
        public int RowType { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// --是否图片
        /// </summary>
        public int IsUploading { get; set; }

        /// <summary>
        /// 是否显示进度条
        /// </summary>
        public int DisplayWay { get; set; }

        /// <summary>
        /// --图片地址
        /// </summary>
        public string Image { get; set; }

        public int OrderBy { get; set; }

        public string Pid { get; set; }
        #endregion
        #region 推荐
        /// <summary>
        /// 是否替换商品
        /// </summary>
        public int IsReplace { get; set; }
        #endregion
        #region 轮胎
        #endregion
        #region 车品
        #endregion
        #region 轮毂
        #endregion
        #region 链接
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
        #endregion
        #region 图片
        public string FileUrl { get; set; }
        #endregion
        #region 优惠券
        /// <summary>
        /// --优惠券ID
        /// </summary>
        public Guid Cid { get; set; }
        #endregion
        #region 保养
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
        #endregion
        #region 活动规则
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        #endregion
        #region 秒杀
        #endregion
        #region 大翻盘
        public Guid? ActivityId { get; set; }
        #endregion
        #region 新大翻盘
        public string HashKey { get; set; }
        #endregion
        #region 答题抽奖
        #endregion
        #region 商品池
        /// <summary>
        /// 一行X列
        /// </summary>
        public int ColumnNumber { get; set; }
        #endregion
        #region 拼团
        public string ProductGroupId { get; set; }
        #endregion
        #region 文案
        /// <summary>
        /// 文案
        /// </summary>
        public string ActiveText { get; set; }
        #endregion
        #region 文字链
        /// <summary>
        /// 文字链，优惠券等配置内容json格式存储
        /// </summary>
        public string JsonContent { get; set; }
        #endregion
        #region 倒计时
        /// <summary>
        /// 倒计时样式：1,深色，2，浅色
        /// </summary>
        public int CountDownStyle { get; set; }
        #endregion
        #region 摇奖机
        #endregion
        #region 分车型头图
        public bool IsVehicle { get; set; }
        #endregion
        #region 新商品池
        /// <summary>
        /// 是否展示适配标签（适配过滤）
        /// </summary>
        public int IsAdapter { get; set; }
        #endregion
        #region 其他
        /// <summary>
        /// 选项
        /// </summary>
        public int OthersType { get; set; }
        #endregion
    }
}
