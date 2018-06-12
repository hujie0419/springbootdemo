using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShareConfigQuery
    {
        /// <summary>
        /// 标签
        /// </summary>
        public string LocationCriterion { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public int? IdCriterion { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DescriptionCriterion { get; set; }
        /// <summary>
        /// 状态 （1：启用2：禁用）
        /// </summary>
        public int? StatusCriterion { get; set; }
        /// <summary>
        /// 创建时间下限
        /// </summary>
        public DateTime? MinTimeCriterion { get; set; }
        /// <summary>
        /// 创建时间上限
        /// </summary>
        public DateTime? MaxTimeCriterion { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorCriterion { get; set; }
        /// <summary>
        /// 分享类型（0：链接1:图片2:文字3：小程序）
        /// </summary>
        public int? ShareTypeCriterion { get; set; }
        /// <summary>
        /// 页面序号（1开始）
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 查询出的总条数
        /// </summary>
        public int TotalCount { get; set; }
    }

    public class ShareConfigModel
    {
        /// <summary>
        /// 主键 
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 分享位置
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 分享场景(1微信好友,2朋友圈,8QQ好友,16QQ空间,32复制链接,64短信)
        /// </summary>
        public int ShareScene { get; set; }
        /// <summary>
        /// 分享状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 分享类型(0链接， 1图片，2文案，3小程序)
        /// </summary>
        public int ShareType { get; set; }
        /// <summary>
        /// 分享链接
        /// </summary>
        public string ShareUrl { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string ThumbPic { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 背景图
        /// </summary>
        public string BgPic { get; set; }

        /// <summary>
        /// 参数参考
        /// </summary>
        public string ParameterRef { get; set; }

        /// <summary>
        /// 分享顺序
        /// </summary>
        public int SceneSequence { get; set; }

        /// <summary>
        /// 页面路径
        /// </summary>
        public string PagePath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 细节描述
        /// </summary>
        public string DetailDspt { get; set; }
    }

    public class ShareConfigSource
    {
        /// <summary>
        /// 主键 
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 分享位置
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 分享状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 特殊参数
        /// </summary>
        public string SpecialParam { get; set; }
    }

    public class ShareSupervisionConfig
    {
        /// <summary>
        /// 主键 
        /// </summary>
        public int PKId { get; set; }
        /// <summary>
        /// 分享位置
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 分享场景(1微信好友,2朋友圈,8QQ好友,16QQ空间,32复制链接,64短信)
        /// </summary>
        public int ShareScene { get; set; }
        /// <summary>
        /// 分享状态(1启用，2禁用)
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 分享类型(0链接， 1图片，2文案，3小程序)
        /// </summary>
        public int ShareType { get; set; }
        /// <summary>
        /// 分享链接
        /// </summary>
        public string ShareUrl { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string ThumbPic { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 背景图
        /// </summary>
        public string BgPic { get; set; }
        /// <summary>
        /// 参数参考
        /// </summary>
        public string ParameterRef { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 分享顺序
        /// </summary>
        public int SceneSequence { get; set; }
        /// <summary>
        /// 页面路径
        /// </summary>
        public string PagePath { get; set; }

        public int JumpId { get; set; }

        /// <summary>
        /// 小程序Id
        /// </summary>
        public string MiniGramId { get; set; }
    }

    public class ShareConfigLog
    {
        /// <summary>
        /// 主键 
        /// </summary>
        public int PKId { get; set; }

        /// <summary>
        /// 对应记录主键
        /// </summary>
        public int ConfigId { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string Operator { get; set; }
        
        /// <summary>
        /// 操作类型（0创建，1修改）
        /// </summary>
        public int? OperateType { get; set; }

        /// <summary>
        /// 创建时间,没用
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }
}
