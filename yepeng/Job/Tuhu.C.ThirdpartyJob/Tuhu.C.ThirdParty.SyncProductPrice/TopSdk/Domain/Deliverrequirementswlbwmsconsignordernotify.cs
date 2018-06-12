using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Deliverrequirementswlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Deliverrequirementswlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 配送类型： CWPS-常温配送 LLPS-冷链配送
        /// </summary>
        [XmlElement("delivery_type")]
        public string DeliveryType { get; set; }

        /// <summary>
        /// 送达日期
        /// </summary>
        [XmlElement("schedule_day")]
        public string ScheduleDay { get; set; }

        /// <summary>
        /// 送达结束时间
        /// </summary>
        [XmlElement("schedule_end")]
        public string ScheduleEnd { get; set; }

        /// <summary>
        /// 送达开始时间
        /// </summary>
        [XmlElement("schedule_start")]
        public string ScheduleStart { get; set; }

        /// <summary>
        /// 投递时延要求:  1-工作日 2-节假日 101,当日达102次晨达103次日达 111 活动标  104 预约达
        /// </summary>
        [XmlElement("schedule_type")]
        public long ScheduleType { get; set; }
    }
}
