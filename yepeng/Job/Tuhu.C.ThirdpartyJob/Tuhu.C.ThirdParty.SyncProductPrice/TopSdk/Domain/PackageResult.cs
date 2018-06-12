using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// PackageResult Data Structure.
    /// </summary>
    [Serializable]
    public class PackageResult : TopObject
    {
        /// <summary>
        /// 操作结果码
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

        /// <summary>
        /// 操作结果信息
        /// </summary>
        [XmlElement("info")]
        public string Info { get; set; }

        /// <summary>
        /// 包基本信息
        /// </summary>
        [XmlElement("package_base")]
        public Top.Api.Domain.PackageBase PackageBase { get; set; }

        /// <summary>
        /// 包基本信息列表
        /// </summary>
        [XmlArray("package_base_list")]
        [XmlArrayItem("package_base")]
        public List<Top.Api.Domain.PackageBase> PackageBaseList { get; set; }

        /// <summary>
        /// 包扩展信息
        /// </summary>
        [XmlElement("package_extend")]
        public Top.Api.Domain.PackageExtendDto PackageExtend { get; set; }

        /// <summary>
        /// 包扩展信息id
        /// </summary>
        [XmlElement("package_extend_id")]
        public long PackageExtendId { get; set; }

        /// <summary>
        /// 包扩展信息列表
        /// </summary>
        [XmlArray("package_extend_list")]
        [XmlArrayItem("package_extend")]
        public List<Top.Api.Domain.PackageExtend> PackageExtendList { get; set; }

        /// <summary>
        /// 包id
        /// </summary>
        [XmlElement("package_id")]
        public long PackageId { get; set; }

        /// <summary>
        /// 操作是否成功
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }
    }
}
