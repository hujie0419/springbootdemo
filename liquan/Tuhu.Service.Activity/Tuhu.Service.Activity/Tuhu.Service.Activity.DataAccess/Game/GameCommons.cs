using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    public class GameCommons
    {

        //奖品类型
        public const string PrizeTypeCarModel = "CARMODEL";//车模
        public const string PrizeTypeEngineOil = "ENGINEILOIL";//机油
        public const string PrizeTypeCinemaTicket = "CINEMATICKET";//电影票
        public const string PrizeTypeFreeCoupon = "FREECOUPON";//免费券
        public const string PrizeTypeUTTestCoupon = "UTTESTCOUPON";//免费券

        //奖品属性
        public const string PrizeEntityTypeEntity = "Entity";// 实物
        public const string PrizeEntityTypeCOUPON = "COUPON";// 券

        //获取积分类型
        public const string PointTypeDailyShare = "每日分享";//  
        public const string PointTypeBuyProduct = "商品购买";//   
        public const string PointTypeUserSupport = "好友助力";//  
    }
}
