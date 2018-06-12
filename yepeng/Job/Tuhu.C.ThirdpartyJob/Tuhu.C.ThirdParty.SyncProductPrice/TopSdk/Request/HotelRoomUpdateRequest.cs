using System;
using System.Collections.Generic;
using Top.Api.Response;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.hotel.room.update
    /// </summary>
    public class HotelRoomUpdateRequest : BaseTopRequest<HotelRoomUpdateResponse>, ITopUploadRequest<HotelRoomUpdateResponse>
    {
        /// <summary>
        /// 已废弃
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string Bbn { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string BedType { get; set; }

        /// <summary>
        /// 早餐。A,B,C,D,E。分别代表：A：无早，B：单早，C：双早，D：三早，E：多早
        /// </summary>
        public string Breakfast { get; set; }

        /// <summary>
        /// 销售渠道。如需开通，需要申请权限。如果有多个用","分。目前制定四种渠道：A-集团协议B-双11促销价C-普通促销价M-无线专享价
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public Nullable<long> Deposit { get; set; }

        /// <summary>
        /// 宝贝描述。 宝贝描述长度不能超过50000字节
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 产品每日结束销售时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public Nullable<long> Fee { get; set; }

        /// <summary>
        /// 酒店房间商品gid。必须为数字。
        /// </summary>
        public Nullable<long> Gid { get; set; }

        /// <summary>
        /// 生效截止时间
        /// </summary>
        public Nullable<DateTime> GmtDeadline { get; set; }

        /// <summary>
        /// 生效开始时间，用来控制此rateplan生效的开始时间，配合字段deadline_time一起限定rp的有效期
        /// </summary>
        public Nullable<DateTime> GmtEffective { get; set; }

        /// <summary>
        /// 购买须知。 购买须知长度不能超过600字节
        /// </summary>
        public string Guide { get; set; }

        /// <summary>
        /// 酒店商品是否提供发票
        /// </summary>
        public Nullable<bool> HasReceipt { get; set; }

        /// <summary>
        /// 最大提前预定小时数
        /// </summary>
        public Nullable<long> MaxAdvHours { get; set; }

        /// <summary>
        /// 最大入住天数
        /// </summary>
        public Nullable<long> MaxDays { get; set; }

        /// <summary>
        /// 会员等级。如需开通，需要申请权限，取值为：1,2,3,none。
        /// </summary>
        public string MemberLevel { get; set; }

        /// <summary>
        /// 最小提前预定小时数
        /// </summary>
        public Nullable<long> MinAdvHours { get; set; }

        /// <summary>
        /// 最小入住天数
        /// </summary>
        public Nullable<long> MinDays { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string MultiRoomQuotas { get; set; }

        /// <summary>
        /// 卖家房型id
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 支付类型。可选值：A,E,H。分别代表：A：全额预付，E：前台面付，H：信用住。如需使用E,H 需要申请权限
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public FileItem Pic { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string PicPath { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string PriceType { get; set; }

        /// <summary>
        /// 发票说明，不能超过100个字
        /// </summary>
        public string ReceiptInfo { get; set; }

        /// <summary>
        /// 发票类型为其他时的发票描述,不能超过30个字
        /// </summary>
        public string ReceiptOtherTypeDesc { get; set; }

        /// <summary>
        /// 发票类型。A,B。分别代表： A:酒店住宿发票,B:其他。注意：B卖家必填该字段，C卖家可选
        /// </summary>
        public string ReceiptType { get; set; }

        /// <summary>
        /// 退订政策字段，是个json串，参考示例值设置改字段的值。。1.表示任意退{"cancelPolicyType":1};2.表示不能退{"cancelPolicyType":2};3.从入住当天24点往前推X小时前取消收取Y%手续费，否则不可取消{"t":3,"p":[{"d":15,"r":30},{"d":10,"r":40}]}表示，从入住日24点往前推提前至少15天取消，收取30%的手续费，从入住日24点往前推提前至少10天取消，收取40%的手续费;
        /// </summary>
        public string RefundPolicyInfo { get; set; }

        /// <summary>
        /// 淘宝房型主键
        /// </summary>
        public Nullable<long> Rid { get; set; }

        /// <summary>
        /// 房态房价信息。可以传今天开始90天内的房态信息。JSON格式传递。date：代表房态房价日期，格式为YYYY-MM-DD，price：代表房价，0～99999999，存储的单位是分，num：代表当天可售间数，0～999。
        /// </summary>
        public string RoomQuotas { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string SiteParam { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 产品每日开始销售时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 状态。可选值1，2。 1：上架。2：下架。传入相应状态代表去执行相应的操作。
        /// </summary>
        public Nullable<long> Status { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        public string Storey { get; set; }

        /// <summary>
        /// 宝贝名称。 宝贝名称长度不能超过60字节（30汉字）
        /// </summary>
        public string Title { get; set; }

        #region BaseTopRequest Members

        public override string GetApiName()
        {
            return "taobao.hotel.room.update";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("area", this.Area);
            parameters.Add("bbn", this.Bbn);
            parameters.Add("bed_type", this.BedType);
            parameters.Add("breakfast", this.Breakfast);
            parameters.Add("channel", this.Channel);
            parameters.Add("deposit", this.Deposit);
            parameters.Add("desc", this.Desc);
            parameters.Add("end_time", this.EndTime);
            parameters.Add("fee", this.Fee);
            parameters.Add("gid", this.Gid);
            parameters.Add("gmt_deadline", this.GmtDeadline);
            parameters.Add("gmt_effective", this.GmtEffective);
            parameters.Add("guide", this.Guide);
            parameters.Add("has_receipt", this.HasReceipt);
            parameters.Add("max_adv_hours", this.MaxAdvHours);
            parameters.Add("max_days", this.MaxDays);
            parameters.Add("member_level", this.MemberLevel);
            parameters.Add("min_adv_hours", this.MinAdvHours);
            parameters.Add("min_days", this.MinDays);
            parameters.Add("multi_room_quotas", this.MultiRoomQuotas);
            parameters.Add("outer_id", this.OuterId);
            parameters.Add("payment_type", this.PaymentType);
            parameters.Add("pic_path", this.PicPath);
            parameters.Add("price_type", this.PriceType);
            parameters.Add("receipt_info", this.ReceiptInfo);
            parameters.Add("receipt_other_type_desc", this.ReceiptOtherTypeDesc);
            parameters.Add("receipt_type", this.ReceiptType);
            parameters.Add("refund_policy_info", this.RefundPolicyInfo);
            parameters.Add("rid", this.Rid);
            parameters.Add("room_quotas", this.RoomQuotas);
            parameters.Add("service", this.Service);
            parameters.Add("site_param", this.SiteParam);
            parameters.Add("size", this.Size);
            parameters.Add("start_time", this.StartTime);
            parameters.Add("status", this.Status);
            parameters.Add("storey", this.Storey);
            parameters.Add("title", this.Title);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateMaxLength("area", this.Area, 1);
            RequestValidator.ValidateMaxLength("bbn", this.Bbn, 1);
            RequestValidator.ValidateMaxLength("bed_type", this.BedType, 1);
            RequestValidator.ValidateMaxLength("breakfast", this.Breakfast, 1);
            RequestValidator.ValidateMaxValue("deposit", this.Deposit, 99999900);
            RequestValidator.ValidateMinValue("deposit", this.Deposit, 0);
            RequestValidator.ValidateMaxLength("desc", this.Desc, 50000);
            RequestValidator.ValidateMaxValue("fee", this.Fee, 99999900);
            RequestValidator.ValidateMinValue("fee", this.Fee, 0);
            RequestValidator.ValidateMaxLength("guide", this.Guide, 300);
            RequestValidator.ValidateMaxLength("payment_type", this.PaymentType, 1);
            RequestValidator.ValidateMaxLength("pic", this.Pic, 512000);
            RequestValidator.ValidateMaxLength("price_type", this.PriceType, 1);
            RequestValidator.ValidateMaxLength("size", this.Size, 1);
            RequestValidator.ValidateMaxLength("storey", this.Storey, 8);
            RequestValidator.ValidateMaxLength("title", this.Title, 90);
        }

        #endregion

        #region ITopUploadRequest Members

        public IDictionary<string, FileItem> GetFileParameters()
        {
            IDictionary<string, FileItem> parameters = new Dictionary<string, FileItem>();
            parameters.Add("pic", this.Pic);
            return parameters;
        }

        #endregion
    }
}
