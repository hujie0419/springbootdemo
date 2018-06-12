using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.hotel.rooms.update
    /// </summary>
    public class HotelRoomsUpdateRequest : BaseTopRequest<HotelRoomsUpdateResponse>
    {
        /// <summary>
        /// 多商品房态信息。json encode。每个商品房态参考单商品更新中的room_quota字段。反序列化后入：array(( 'gid'=>1或者'rid'=>1或者'outer_id=>1', 'roomQuota'=>array(('date'=>'2011-01-29', 'price'=>100, 'num'=>1),('date'=>'2011-01-30', 'price'=>100, 'num'=>1)),( 'gid'=>2或者'rid'=>1或者'outer_id=>1', 'roomQuota'=>array(('date'=>'2011-01-29', 'price'=>100, 'num'=>1),('date'=>'2011-01-30', 'price'=>100, 'num'=>1)))
        /// </summary>
        public string GidRoomQuotaMap { get; set; }

        /// <summary>
        /// 为到店支付卖家特殊使用，可传入多种支付类型的房态信息。该参数有值时，忽略gid_room_quota_map参数；该参数无值时，使用gid_room_quota_map参数
        /// </summary>
        public string MultiRoomQuotas { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.hotel.rooms.update";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("gid_room_quota_map", this.GidRoomQuotaMap);
            parameters.Add("multi_room_quotas", this.MultiRoomQuotas);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

        #endregion
    }
}
