using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public class ShareImage
    {
        public int PKID { get; set; }

        public string userId { get; set; }

        public string content { get; set; }

        public bool isActive { get; set; }

        public bool isDelete { get; set; }

        public DateTime createTime { get; set; }

        public DateTime lastUpdateTime { get; set; }

        public int likesCount { get; set; }

        public int commentCount { get; set; }

        public int shareCount { get; set; }

        public List<ImagesDetail> images { get; set; }

        [NotMapped]
        public UserInfo User { get; set; }
    }

    public class ImagesDetail
    {
        public int PKID { get; set; }
        public string imageUrl { get; set; }
        public string uploadTime { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }

    }

    public class UserInfo
    {
        public string UserID { get; set; }
        public string NickName { get; set; }
        public string UserHead { get; set; }
        public string UserPhone { get; set; }
    }


}
