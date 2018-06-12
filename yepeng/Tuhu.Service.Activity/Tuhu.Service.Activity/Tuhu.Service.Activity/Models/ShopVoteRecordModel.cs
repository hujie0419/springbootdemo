using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class ShopVoteRecordModel:BaseModel
    {
        public long PKID { get; set; }
        public Guid UserId { get; set; }
        public long ShopId { get; set; }
        public bool Share { get; set; }
        public DateTime CreateTime { get; set; }
    }
}