using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VehicleAuthAuditRequest
    {
        public string Mobile { get; set; }
        public Guid CarId { get; set; }
        public string CarNo { get; set; }
        public string VinCode { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int Status { get; set; }
    }
    public class VehicleAuditInfoModel
    {
        /// <summary>车型编号</summary>
        public Guid CarId { get; set; }

        /// <summary>会员编号</summary>
        public Guid UserId { get; set; }

        /// <summary>是否默认车型</summary>
        public bool IsDefaultCar { get; set; }

        /// <summary>VIN码</summary>
        public string VinCode { get; set; }

        /// <summary> 车牌号 </summary>
        public string CarNumber { get; set; }

        /// <summary>发动机号 </summary>
        public string EngineNo { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车型名称
        /// </summary>
        public string Vehicle { get; set; }

        /// <summary>
        /// 排量
        /// </summary>
        public string PaiLiang { get; set; }

        /// <summary>
        /// 生产年份
        /// </summary>
        public string Nian { get; set; }

        /// <summary>
        /// 生产年份子款型名称（例如：“2010款 2.0T 双离合 Quattro 四驱 越野版”）
        /// </summary>
        public string SalesName { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 身份证照片
        /// </summary>
        public string IdCardUrl { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 用户手机号/用来Setting查询呈现用
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 认证渠道
        /// </summary>
        public string Channel { get; set; }
        private int? status;
        /// <summary>
        /// 车型认证状态-1:未认证，0审核中，1已审核，2，未审核
        /// </summary>
        public int? Status
        {
            get
            {
                if (!status.HasValue)
                    status = -1;
                return status;
            }
            set { status = value; }
        }
        public DateTime certified_time { get; set; }
        public DateTime LastChangedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? car_Registrationtime { get; set; }
    }

    public class VehicleTypeCertificationAuditLogModel
    {
        public long PKID { get; set; }
        /// <summary>
        /// 车型id
        /// </summary>
        public Guid CarId { get; set; }
        public string Author { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        /// <summary>
        /// 操作描述
        /// </summary>
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
    }
}