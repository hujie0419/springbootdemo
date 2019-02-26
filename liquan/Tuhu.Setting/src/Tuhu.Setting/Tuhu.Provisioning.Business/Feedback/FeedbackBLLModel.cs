using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.Feedback
{
    public class FeedbackBLLModel
    {
        /// <summary>
        /// 反馈内容Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 问题类型
        /// </summary>
        public string TypeName { get; set; }

        public string CreateTime { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserPhone { get; set; }
        /// <summary>
        /// 反馈内容
        /// </summary>
        public string FeedbackContent { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNum { get; set; }
        /// <summary>
        /// 手机型号
        /// </summary>
        public string PhoneModels { get; set; }
        /// <summary>
        /// 网络环境
        /// </summary>
        public string NetworkEnvironment { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public List<string> FeedbackImgs { get; set; }
        /// <summary>
        /// 单个图片地址
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 客服介入
        /// </summary>
        public int IsCustomerServer { get; set; }
    }
    /// <summary>
    /// 分页意见反馈信息
    /// </summary>
    public class PageResult
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 每页意见反馈信息实体集合
        /// </summary>

        public IEnumerable<FeedbackBLLModel> FeedbackList { get; set; }
    }
}
