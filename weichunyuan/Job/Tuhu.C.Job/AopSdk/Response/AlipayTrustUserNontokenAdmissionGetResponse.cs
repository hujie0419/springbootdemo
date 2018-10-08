using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayTrustUserNontokenAdmissionGetResponse.
    /// </summary>
    public class AlipayTrustUserNontokenAdmissionGetResponse : AopResponse
    {
        /// <summary>
        /// admissionStatus：1//0代表非准入，1代表准入
        /// </summary>
        [XmlElement("admission_info")]
        public string AdmissionInfo { get; set; }
    }
}
