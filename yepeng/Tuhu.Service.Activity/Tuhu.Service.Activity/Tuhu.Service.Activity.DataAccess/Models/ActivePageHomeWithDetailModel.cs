
namespace Tuhu.Service.Activity.DataAccess.Models
{
    public class ActivePageHomeWithDetailModel
    {
        public int DetailPkid { get; set; }

        public int FkActiveHome { get; set; }

        public string HomeName { get; set; }

        public string HidBigFHomePic { get; set; }

        /// <summary>
        /// --h5链接
        /// </summary>
        public string BigFHomeMobileUrl { get; set; }

        /// <summary>
        /// --网站链接
        /// </summary>
        public string BigFHomeWwwUrl { get; set; }

        /// <summary>
        /// 小程序
        /// </summary>
        public string BigFHomeWxAppUrl { get; set; }

        public int BigFHomeOrder { get; set; }

        public int Pkid { get; set; }

        public int FkActiveId { get; set; }

        /// <summary>
        /// --会场名称
        /// </summary>
        public string BigHomeName { get; set; }

        /// <summary>
        /// --图标
        /// </summary>
        public string HidBigHomePic { get; set; }

        /// <summary>
        /// --h5链接地址
        /// </summary>
        public string BigHomeUrl { get; set; }

        /// <summary>
        /// --网站图标
        /// </summary>
        public string HidBigHomePicWww { get; set; }

        /// <summary>
        /// --网站链接
        /// </summary>
        public string BigHomeUrlWww { get; set; }

        /// <summary>
        /// 小程序图标
        /// </summary>
        public string HidBigHomePicWxApp{ get; set; }

        /// <summary>
        /// --小程序链接
        /// </summary>
        public string BigHomeUrlWxApp { get; set; }

        public int Sort { get; set; }

        /// <summary>
        /// 是否主会场
        /// </summary>
        public int IsHome { get; set; }
    }
}
