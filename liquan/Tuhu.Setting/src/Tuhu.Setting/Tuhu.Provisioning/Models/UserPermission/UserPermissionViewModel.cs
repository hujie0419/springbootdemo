using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class UserPermissionViewModel
    {
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
        /// 显示顺序
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否启用:1启用 0禁用

        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否启用:1启用 0禁用
        /// </summary>
        public string StrIsEnable { get { return IsEnable ? "启用" : "禁用"; } }

        /// <summary>
        /// 是否高亮:1启用 0禁用
        /// </summary>
        public int IsLight { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string FootTile { get; set; }

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
        /// 会员等级名称
        /// </summary>
        public string MembershipsGradeName { get; set; }

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
        /// 创建时间
        /// </summary>
        public string CreateDatetime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastUpdateDateTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public string LastUpdateBy { get; set; }
 
    }
}