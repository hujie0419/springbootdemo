using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.hotel.room.get
    /// </summary>
    public class HotelRoomGetRequest : BaseTopRequest<HotelRoomGetResponse>
    {
        /// <summary>
        /// 酒店房间商品gid。必须为数字。gid和item_id至少要传一个。
        /// </summary>
        public Nullable<long> Gid { get; set; }

        /// <summary>
        /// 酒店房间商品item_id。必须为数字。item_id和gid至少要传一个。
        /// </summary>
        public Nullable<long> ItemId { get; set; }

        /// <summary>
        /// 是否需要返回该商品的酒店信息。可选值：true，false。
        /// </summary>
        public Nullable<bool> NeedHotel { get; set; }

        /// <summary>
        /// 是否需要返回该商品的宝贝描述。可选值：true，false。
        /// </summary>
        public Nullable<bool> NeedRoomDesc { get; set; }

        /// <summary>
        /// 是否需要返回该商品的房态列表。可选值：true，false。
        /// </summary>
        public Nullable<bool> NeedRoomQuotas { get; set; }

        /// <summary>
        /// 是否需要返回该商品的房型信息。可选值：true，false。
        /// </summary>
        public Nullable<bool> NeedRoomType { get; set; }

        /// <summary>
        /// 卖家房型id
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 淘宝房型主键
        /// </summary>
        public Nullable<long> Rid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.hotel.room.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("gid", this.Gid);
            parameters.Add("item_id", this.ItemId);
            parameters.Add("need_hotel", this.NeedHotel);
            parameters.Add("need_room_desc", this.NeedRoomDesc);
            parameters.Add("need_room_quotas", this.NeedRoomQuotas);
            parameters.Add("need_room_type", this.NeedRoomType);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("rid", this.Rid);
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
