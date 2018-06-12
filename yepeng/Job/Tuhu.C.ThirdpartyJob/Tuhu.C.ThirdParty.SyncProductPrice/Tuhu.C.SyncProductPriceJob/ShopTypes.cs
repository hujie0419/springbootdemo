using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Tuhu.C.SyncProductPriceJob
{
    [DataContract]
    public enum ShopTypes : byte
    {
        [EnumMember]
        Taobao = 1,

        [EnumMember]
        Jingdong = 2,

        [EnumMember]
        Amazon = 3,

        [EnumMember]
        Eticket = 4,

        [EnumMember]
        Suning = 5,

        [EnumMember]
        Qipeilong = 6,

        [EnumMember]
        DangDang = 7,

        [EnumMember]
        QuanCheJian = 8,

        [EnumMember]
        HubeiMaPai = 9,

        [EnumMember]
        QplQingCang = 10,

        [EnumMember]
        Pinduoduo = 11,
    }

    [DataContract]
    public class Shop
    {
        [JsonProperty]
        public string NickName { get; set; }

        [DataMember]
        public string ShopCode { get; set; }

        [DataMember]
        public ShopTypes ShopType { get; set; }

        [JsonProperty]
        public string SessionKey { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
