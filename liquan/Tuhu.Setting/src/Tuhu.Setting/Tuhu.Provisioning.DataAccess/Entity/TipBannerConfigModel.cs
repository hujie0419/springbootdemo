using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class TipBannerTypeConfigModel
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
    public class TipBannerConfigDetailModel : TipBannerTypeConfigModel
    {
        public int PKID { get; set; }
        public bool IsEnabled { get; set; }
        public string Icon { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string BackgroundColor { get; set; }
        public double BgTransparent { get; set; }
        public string ContentColor { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}
