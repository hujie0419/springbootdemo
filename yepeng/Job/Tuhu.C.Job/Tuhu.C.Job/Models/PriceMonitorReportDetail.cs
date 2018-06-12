using System;
using System.Data;
using Tuhu.Models;

namespace Tuhu.C.Job.Models
{
    public class PriceMonitorReportDetail : BaseModel
    {
        #region 商品基础信息
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [Column("DisplayName")]
        public string ProductName { get; set; }
        /// <summary>
        /// 安全库存
        /// </summary>
        public int SecurityStock { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 周销量
        /// </summary>
        [Column("1WSum")]
        public int WeekSales { get; set; }

        #endregion

        #region 自营店价格
        /// <summary>
        /// 进货价
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 官网价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        public decimal GrossProfits
        {
            get
            {
                if (Price == 0)
                {
                    return 0;
                }
                else
                {
                    return (Price - Cost) / Price;
                }
            }
        }
        /// <summary>
        /// 毛利显示
        /// </summary>
        public string GrossProfitsDisplay
        {
            get { return GrossProfits.ToString("0.00%"); }
        }
        public long TaobaoId { get; set; }

        public string TaobaoName { get; set; }
        /// <summary>
        /// 淘宝途虎
        /// </summary>
        [Column("TaobaoPrice")]
        public decimal? TaobaoTuhuPrice { get; set; }
        [Column("TMPID")]
        public long Tm1Pid { get; set; }
        [Column("TMName")]
        public string Tm1Name { get; set; }
        /// <summary>
        /// 天猫1途虎
        /// </summary>
        [Column("TMPrice")]
        public decimal? Mall1TuhuPrice { get; set; }
        public long Tm2Pid { get; set; }

        public string Tm2Name { get; set; }
        /// <summary>
        /// 天猫2途虎
        /// </summary>
        [Column("TM2Price")]
        public decimal? Mall2TuhuPrice { get; set; }
        public long Tm3Pid { get; set; }

        public string Tm3Name { get; set; }
        /// <summary>
        /// 天猫3途虎
        /// </summary>
        [Column("TM3Price")]
        public decimal? Mall3TuhuPrice { get; set; }
        public long JdTuhuPid { get; set; }

        public string JdTuhuName { get; set; }
        /// <summary>
        /// 京东途虎
        /// </summary>
        public decimal? JdTuhuPrice { get; set; }
        /// <summary>
        /// 国美途虎
        /// </summary>
        public decimal? GmTuhuPrice { get; set; }
        /// <summary>
        /// 苏宁途虎
        /// </summary>
        public decimal? SnTuhuPrice { get; set; }
        /// <summary>
        /// 一号店POP
        /// </summary>
        public decimal? YhdpopTuhuPrice { get; set; }
        /// <summary>
        /// 一号店自营
        /// </summary>
        public decimal? YhdTuhuPrice { get; set; }
        /// <summary>
        /// 当当
        /// </summary>
        public decimal? DdTuhuPrice { get; set; }
        /// <summary>
        /// 亚马逊自营
        /// </summary>
        public decimal? AmazonTuhuPrice { get; set; }
        /// <summary>
        /// 亚马逊POP
        /// </summary>
        public decimal? AmazonPopTuhuPrice { get; set; }

        #endregion

        #region 竞争对手价格
        public long Jdpid { get; set; }
        [Column("PName")]
        public string JdName { get; set; }
        /// <summary>
        /// 京东自营
        /// </summary>
        public decimal? JdPrice { get; set; }
        public long Twlpid { get; set; }

        public string TwlName { get; set; }
        /// <summary>
        /// 特维轮天猫
        /// </summary>
        [Column("TWLPrice")]
        public decimal? TwlPrice { get; set; }
        public long Mlttpid { get; set; }

        public string MlttName { get; set; }
        /// <summary>
        /// 麦轮胎天猫
        /// </summary>
        [Column("MLTTPrice")]
        public decimal? MlttPrice { get; set; }
        /// <summary>
        /// 汽车超人官网
        /// </summary>
        public decimal? QccrPrice { get; set; }
        public long Mltpid { get; set; }

        public string MltName { get; set; }
        /// <summary>
        /// 麦轮胎官网
        /// </summary>
        [Column("MLTPrice")]
        public decimal? MltPrice { get; set; }
        #endregion

        #region Url

        public string PriceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(Pid))
                    return "#";

                var arr = Pid.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (arr.Length > 1)
                    return string.Concat("http://item.tuhu.cn/Products/", arr[0], "/", arr[1], ".html");
                else
                    return string.Concat("http://item.tuhu.cn/Products/", Pid, ".html");
            }
        }

        public string TaobaoUrl
        {
            get { return $"http://item.taobao.com/item.htm?id={TaobaoId}"; }
        }

        public string Tm1Url
        {
            get { return $"http://detail.tmall.com/item.htm?id={Tm1Pid}"; }
        }
        public string Tm2Url
        {
            get { return $"http://detail.tmall.com/item.htm?id={Tm2Pid}"; }
        }
        public string Tm3Url
        {
            get { return $"http://detail.tmall.com/item.htm?id={Tm3Pid}"; }
        }
        public string JdTuhuUrl
        {
            get { return $"http://item.jd.com/{JdTuhuPid}.html"; }
        }
        public string JdUrl
        {
            get { return $"http://item.jd.com/{Jdpid}.html"; }
        }
        public string TwlUrl
        {
            get { return $"http://detail.tmall.com/item.htm?id={Twlpid}"; }
        }
        public string MlttUrl
        {
            get { return $"http://detail.tmall.com/item.htm?id={Mlttpid}"; }
        }
        public string MltUrl
        {
            get { return $"http://www.mailuntai.cn/product/{Mltpid}.html"; }
        }
        #endregion
    }
}
