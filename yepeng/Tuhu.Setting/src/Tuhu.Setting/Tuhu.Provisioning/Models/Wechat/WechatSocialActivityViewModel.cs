namespace Tuhu.Provisioning.Models.Wechat
{
    public class WechatSocialActivityRequest
    {
        public Info info { get; set; }
    }

    public class Info
    {
        public Basic_Info basic_info { get; set; }
        public Card_Info_List[] card_info_list { get; set; }
        //public Custom_Info custom_info { get; set; }
    }

    public class Basic_Info
    {
        public string activity_bg_color { get; set; }
        public string activity_tinyappid { get; set; }
        public uint begin_time { get; set; }
        public uint end_time { get; set; }
        public int gift_num { get; set; }
        public int max_partic_times_act { get; set; }
        public int max_partic_times_one_day { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_code { get; set; }
    }

    public class Custom_Info
    {
        public string type { get; set; }
    }

    public class Card_Info_List
    {
        public string card_id { get; set; }

        public int min_amt { get; set; }

        public bool? new_tinyapp_user { get; set; }

        public bool? total_user { get; set; }
    }
}