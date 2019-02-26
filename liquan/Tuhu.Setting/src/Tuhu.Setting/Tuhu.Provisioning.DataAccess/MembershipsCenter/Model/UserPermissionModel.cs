using System;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class UserPermissionModel : BaseModel
    {
        public UserPermissionModel() { }
        public UserPermissionModel(DataRow row) : base(row) { }

        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 显示的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 点亮图标
        /// </summary>
        public string LightImage { get; set; }

        /// <summary>
        /// 未点亮图标
        /// </summary>
        public string DarkImage { get; set; }

        /// <summary>
        /// 头图
        /// </summary>
        public string TopImage { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 是否为头图
        /// </summary>
        public bool IsTopImage { get; set; }

        /// <summary>
        /// 用户等级(旧新版本不可用该字段）
        /// </summary>
        public int UseUserLevel { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否启用:1启用 0禁用

        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否高亮:1启用 0禁用
        /// </summary>
        public int IsLight { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string FootTile { get; set; }

        /// <summary>
        /// 版本（旧字段后期版本固定可删）
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 说明标题
        /// </summary>
        public string DescriptionTitle { get; set; }

        /// <summary>
        /// 可领取状态按钮文案
        /// </summary>
        public string LightText { get; set; }

        /// <summary>
        /// 不可领取状态按钮文案
        /// </summary>
        public string DarkText { get; set; }

        /// <summary>
        /// 会员等级外键
        /// </summary>
        public int MembershipsGradeId { get; set; }

        /// <summary>
        ///权益类型
        /// </summary>
        public int PermissionType { get; set; }

        /// <summary>
        /// 启用版本
        /// </summary>
        public string EnabledVersion { get; set; }

        /// <summary>
        /// 结束版本
        /// </summary>
        public string EndVersion { get; set; }

        /// <summary>
        /// 校验周期[n_d(多少天),n_M(多少月),n_Y(多少年)],为空表示存在即校验
        /// </summary>
        public string CheckCycle { get; set; }

        /// <summary>
        /// 是否跳转URL
        /// </summary>
        public bool IsLinkUrl { get; set; }

        /// <summary>
        /// 可领取状态跳转URL（整体）
        /// </summary>
        public string LightUrl { get; set; }

        /// <summary>
        /// 可领取状态按钮跳转链接
        /// </summary>
        public string LightButtonUrl { get; set; }

        /// <summary>
        /// 权益说明正文
        /// </summary>
        public string DescriptionDetail { get; set; }

        /// <summary>
        /// 卡片图片地址
        /// </summary>
        public string CardImage { get; set; }

        /// <summary>
        /// 安卓URL
        /// </summary>
        public string AndroidUrl { get; set; }

        /// <summary>
        /// IOSURL
        /// </summary>
        public string IOSUrl { get; set; }

        /// <summary>
        /// 是否点击领取
        /// </summary>
        public bool IsClickReceive { get; set; }

        /// <summary>
        /// 角标(提示)文案
        /// </summary>
        public string PromptTag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public string LastUpdateBy { get; set; }

     

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
