using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 保养关联项目
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class BaoYangPackageTypeRelationViewModel : BaoYangPackageTypeRelationModel
    {
        /// <summary>
        /// 主项目名称
        /// </summary>
        [JsonIgnore]
        public string MainPackageName { get; set; }

        /// <summary>
        /// 辅项目列表
        /// </summary>
        [JsonIgnore]
        public List<string> RelatedPackageTypeList
        {
            get
            {
                return string.IsNullOrWhiteSpace(RelatedPackageTypes) ? new List<string>() :
                    RelatedPackageTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        /// <summary>
        /// 辅项目名称
        /// </summary>
        [JsonIgnore]
        public string RelatedPackageNames { get; set; }
    }


    public class BaoYangPackageTypeRelationModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 主项目
        /// </summary>
        public string MainPackageType { get; set; }

        /// <summary>
        /// 辅项目
        /// </summary>
        public string RelatedPackageTypes { get; set; }

        /// <summary>
        /// 文案
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 文案高亮部分
        /// </summary>
        public string Highlights { get; set; }
        /// <summary>
        /// 是否强制关联
        /// </summary>
        public bool IsStrongRelated { get; set; }
        /// <summary>
        /// 取消时文案
        /// </summary>
        public string CancelContent { get; set; }
    }
}
