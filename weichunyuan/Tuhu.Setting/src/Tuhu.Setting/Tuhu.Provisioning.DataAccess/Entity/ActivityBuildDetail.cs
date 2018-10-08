namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ActivityBuildDetail
    {
        /// <summary>
        /// 组号
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 活动ID限时抢购
        /// </summary>
        public string VID { get; set; }

        /// <summary>
        /// 优惠券规则ID
        /// </summary>
        public string CID { get; set; }

        public string SmallImage { get; set; }

        /// <summary>
        /// 显示的图片链接
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 一行列数 0一行一列 1一行两列 2一行三列 4一行四列
        /// </summary>
        public int BigImg { get; set; }

        /// <summary>
        /// 活动类型 0轮胎 1车品 2链接 3图片 4优惠卷 5保养 6限购轮胎 7限购车品 8活动规则 9 秒杀
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }

        public string HandlerIOS { get; set; }

        public string HandlerAndroid { get; set; }

        public string SOAPIOS { get; set; }

        public string SOAPAndroid { get; set; }

        /// <summary>
        /// 0使用模板 1上传图片 
        /// </summary>
        public int IsUploading { get; set; }

        /// <summary>
        /// 描述 在20个字之间
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 显示方式 1横向 0竖向
        /// </summary>
        public int DisplayWay { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 图片二（小）
        /// </summary>
        public string TwoSImage { get; set; }

        /// <summary>
        /// 图片二 (大)
        /// </summary>
        public string TwoBImage { get; set; }

        /// <summary>
        /// 微信链接
        /// </summary>
        public string WXUrl { get; set; }

        /// <summary>
        /// 官网Url
        /// </summary>
        public string PCUrl { get; set; }
     
        /// <summary>
        /// 渠道 all：全部 wap：移动端 www：网站
        /// </summary>

        public string Channel { get; set; }

        /// <summary>
        /// 产品库原价
        /// </summary>
        public decimal ActivityPrice { get; set; }

        /// <summary>
        /// 是否新用户 1是 0否
        /// </summary>
        public int IsNewUser { get; set; }

        /// <summary>
        /// 用户等级 0所有用户 1：LV1 2 LV2 3：LV3 4:LV4 
        /// </summary>
        public int UserRank { get; set; }


        public string TireSize { get; set; }

    }
}
