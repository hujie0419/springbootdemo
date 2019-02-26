using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   public  class WXEventMsgConfigEntity:EntityTypeConfiguration<WXEventMsgConfigEntity>
    {
        public WXEventMsgConfigEntity()
        {
            this.HasKey(_ => _.PKID);
            this.ToTable("dbo.WXEventMsgConfig");
        }

        public int PKID { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public string Title { get; set; }

        public string Descriptions { get; set; }

        public string PicUrl { get; set; }

        public string Urls { get; set; }

        public bool IsEnabled { get; set; }

        public int OrderBy { get; set; }
        /// <summary>
        /// 微信原始ID
        /// </summary>
        public string OriginalID { get; set; }

        /// <summary>
        /// 消息类型 [text,image,voice,video,music,news,mpnews,miniprogrampage]
        /// </summary>
        public string MsgType { get; set; }


        /// <summary>
        /// 微信公众号的素材id
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 小程序id
        /// </summary>
        public string Appid { get; set; }




    }
}
