using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public enum News
    {
        途虎官方新闻 = 0, 媒体报道新闻 = 1
    }
    public class tbl_TuhuNews : BaseModel
    {
        public tbl_TuhuNews() { }
        public tbl_TuhuNews(DataRow row) : base(row) { }
        public int Id { get; set; }
        public string Title { get; set; }  //新闻标题
        public string NewsFrom { get; set; }   //新闻来源
        public DateTime IssueTime { get; set; } // 发布时间
        public int NewsType { get; set; }   //新闻类别0：途虎官方新闻 1：媒体报道新闻
        public string Content { get; set; } //新闻内容
        public DateTime CreateTime { get; set; }
        public Guid? NewsGuid { get; set; }
    }
}
