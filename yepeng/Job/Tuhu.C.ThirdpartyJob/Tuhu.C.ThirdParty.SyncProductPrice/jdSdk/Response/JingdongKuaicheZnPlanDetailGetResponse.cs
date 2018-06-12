#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-10-26 10:25:43.86007 +08:00
*/
#endregion

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;
using JdSdk.Domain;

namespace JdSdk.Response
{
    /// <summary>
    /// 获取投放计划详细信息 Response
    /// </summary>
    public class JingdongKuaicheZnPlanDetailGetResponse : JdResponse
    {
        /// <summary>
        /// 计划详细信息
        /// </summary>
        [XmlElement("plan_detail_info")]
        [JsonProperty("plan_detail_info")]
        public PlanDetailInfo PlanDetailInfo
        {
            get;
            set;
        }

    }
}
