using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class UserPermissionModel : BaseModel
    {
        public UserPermissionModel() { }
        public UserPermissionModel(DataRow row) : base(row) { }
        /// <summary>
        /// 自增主键
        /// </summary>
        public int? Id { get; set; }

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
        public int? Position { get; set; }

        /// <summary>
        /// 是否为头图
        /// </summary>
        public int? IsTopImage { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int? UseUserLevel { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 小标题
        /// </summary>
        public string FootTile { get; set; }

        /// <summary>
        /// 是否启用 1启用 0禁用
        /// </summary>
        public int? IsEnable {get;set;}

        /// <summary>
        /// 是否高亮
        /// </summary>
        public int? IsLight {get ;set;}


        /// <summary>
        /// 版本 1是老版本，2是新版本
        /// </summary>
        public int? Version { get; set; }

    }
}
