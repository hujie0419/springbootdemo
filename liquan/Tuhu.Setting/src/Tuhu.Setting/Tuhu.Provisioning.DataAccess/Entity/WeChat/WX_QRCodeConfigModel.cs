using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Mapping;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 微信二维码 model
    /// </summary>
    public class WX_QRCodeConfigModel
    {
        public int PKID { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }
        public string Scene { get; set; }
        public string action_name { get; set; }
        public long expire_seconds { get; set; }

        private DateTime expire_date;
        public DateTime Expire_Date
        {
            get { return expire_date; }
            set { this.expire_date = value; }
        }

        public string Expire_DateFormate
        {
            get
            {
                if (this.action_name== "QR_STR_SCENE")
                {
                    return Expire_Date.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    return "";
                }
                
            }
        }

        public string URL { get; set; }
        public string Userid { get; set; }

        public string OriginalID { get; set; }


        private DateTime _createDateTime;
        public DateTime CreateDateTime
        {
            get { return this._createDateTime; }
            set { this._createDateTime = value; }
        }

        public string CreateDateTimeFormat { get { return this._createDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        private DateTime _lastUpdateDateTime ;
        public DateTime LastUpdateDateTime
        {
            get { return this._lastUpdateDateTime; }
            set { this._lastUpdateDateTime = value; }
        }

        /// <summary>
        /// 文字回复内容
        /// </summary>
        public List<WXMaterialTextModel> MaterialTextList { get; set; }
        /// <summary>
        /// 回复素材信息
        /// </summary>
        public List<MaterialModel> MaterialList { get; set; }
    }
}
