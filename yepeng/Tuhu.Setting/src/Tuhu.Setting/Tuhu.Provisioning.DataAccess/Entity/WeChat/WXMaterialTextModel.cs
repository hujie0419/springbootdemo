using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class WXMaterialTextModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public long ObjectID { get; set; }
        /// <summary>
        /// 业务类型，click=点击菜单回复、keyword=关键字回复
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 微信原始ID
        /// </summary>
        public string OriginalID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
