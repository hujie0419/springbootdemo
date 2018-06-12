using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class OilBrandPriorityModel
    {
        public class OilBrandPhonePriorityModel
        {
            public OilBrandPhonePriorityModel()
            { }
            public int PKID { get; set; }
            public string PhoneNumber { get; set; }
            public string Brand { get; set; }
            public DateTime? CreateDateTime { get; set; }
            public DateTime? LastUpdateDateTime { get; set; }

            public OilBrandPhonePriorityModel(DataRow row)
            {
                var cloumns = row.Table.Columns;
                if (cloumns.Contains(nameof(PKID)))
                {
                    PKID = (int)row[nameof(PKID)];
                }
                if (cloumns.Contains(nameof(PhoneNumber)))
                {
                    PhoneNumber = row[nameof(PhoneNumber)].ToString();
                }
                if (cloumns.Contains(nameof(Brand)))
                {
                    Brand = row[nameof(Brand)].ToString();
                }
                if (cloumns.Contains(nameof(CreateDateTime)))
                {
                    CreateDateTime = (DateTime)row[nameof(CreateDateTime)];
                }
                if (cloumns.Contains(nameof(LastUpdateDateTime)))
                {
                    LastUpdateDateTime = row[nameof(LastUpdateDateTime)] as DateTime?;
                }
            }
        }


        public class OilBrandRegionPriorityModel
        {
            public OilBrandRegionPriorityModel()
            { }
            public int PKID { get; set; }
            public int ProvinceId { get; set; }
            public string ProvinceName { get; set; }
            public int RegionId { get; set; }
            public string CityName { get; set; }
            public string Brand { get; set; }
            public DateTime? CreateDateTime { get; set; }
            public DateTime? LastUpdateDateTime { get; set; }

            public OilBrandRegionPriorityModel(DataRow row)
            {
                var cloumns = row.Table.Columns;
                if (cloumns.Contains(nameof(PKID)))
                {
                    PKID = (int)row[nameof(PKID)];
                }
                if (cloumns.Contains(nameof(ProvinceId)))
                {
                    ProvinceId = (int)row[nameof(ProvinceId)];
                }
                if (cloumns.Contains(nameof(ProvinceName)))
                {
                    ProvinceName = row[nameof(ProvinceName)].ToString();
                }
                if (cloumns.Contains(nameof(RegionId)))
                {
                    RegionId = (int)row[nameof(RegionId)];
                }
                if (cloumns.Contains(nameof(CityName)))
                {
                    CityName = row[nameof(CityName)].ToString();
                }
                if (cloumns.Contains(nameof(Brand)))
                {
                    Brand = row[nameof(Brand)].ToString();
                }
                if (cloumns.Contains(nameof(CreateDateTime)))
                {
                    CreateDateTime = (DateTime)row[nameof(CreateDateTime)];
                }
                if (cloumns.Contains(nameof(LastUpdateDateTime)))
                {
                    LastUpdateDateTime = row[nameof(LastUpdateDateTime)] as DateTime?;
                }
            }
        }

        /// <summary>
        /// 用户购买过的机油
        /// </summary>
        public class OilBrandUserOrderViewModel : VehicleInfoSimpleInfo
        {
            /// <summary>
            /// 用户Guid
            /// </summary>
            public Guid UserId { get; set; }
            /// <summary>
            /// 用户手机号
            /// </summary>
            public string PhoneNumber { get; set; }
            /// <summary>
            /// 车型排量
            /// </summary>
            public string PaiLiang { get; set; }
            /// <summary>
            /// 车型年份
            /// </summary>
            public string Nian { get; set; }
            /// <summary>
            /// 机油Pid
            /// </summary>
            public string Pid { get; set; }
            /// <summary>
            /// 机油Pid
            /// </summary>
            public List<string> Pids
            {
                get
                {
                    return string.IsNullOrWhiteSpace(Pid) ? new List<string>()
                        : Pid.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                }
            }
            /// <summary>
            /// 订单号
            /// </summary>
            public long OrderId { get; set; }
        }
    }
}
