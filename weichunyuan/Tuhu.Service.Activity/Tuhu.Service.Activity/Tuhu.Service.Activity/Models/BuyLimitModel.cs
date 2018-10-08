using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Service.Activity.Models
{
    public class BuyLimitModel
    {
        public string ModuleName { get; set; }
        public string ModuleProductId { get; set; }
        public string LimitObjectId { get; set; }
        public string ObjectType { get; set; }
    }
    public class BuyLimitDetailModel : BuyLimitModel
    {
        public string Reference { get; set; }
        public string Remark { get; set; }
    }

    public class BuyLimitInfoModel : BuyLimitModel
    {
        public int CurrentCount { get; set; }
    }

    public class BaseResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
    public enum LimitObjectTypeEnum
    {
        // 用户Id
        UserId = 0,
        // 设备号
        DeviceId = 1,
        // 小程序OpenId
        OpenId = 2,
        // 支付账号
        PayAccount = 3,
        // 收货人手机号
        ConsigneeNumber = 4
    }
}
