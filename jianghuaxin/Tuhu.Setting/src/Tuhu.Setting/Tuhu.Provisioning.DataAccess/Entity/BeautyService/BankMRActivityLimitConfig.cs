﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BankMRActivityLimitConfig
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 每个月的哪些天，逗号隔开
        /// </summary>
        public string DaysOfMonth { get; set; }
        /// <summary>
        /// 每周的哪些天
        /// </summary>
        public string DaysOfWeek { get; set; }
        /// <summary>
        /// 每日总限购
        /// </summary>
        public int DayLimit { get; set; }
        /// <summary>
        /// 场次限购
        /// </summary>
        public int RoundLimit { get; set; }
        /// <summary>
        /// 限购省IDs
        /// </summary>
        public string ProvinceIds { get; set; }
        /// <summary>
        /// 限购市IDs
        /// </summary>
        public string CityIds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 每张卡限购类型（1=年，2=季，3=月，4=天）
        /// </summary>
        public int CardLimitType { get; set; }
        /// <summary>
        /// 每张卡限购次数
        /// </summary>
        public int CardLimitValue { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        public string StartTimeString
        {
            get
            {
                var result = "";
                if (StartTime != null)
                    result = StartTime?.ToString("yyyy-MM-dd HH:mm");
                return result;
            }
            set
            {
                StartTime = DateTime.Parse(value);
            }
        }
    }
}
