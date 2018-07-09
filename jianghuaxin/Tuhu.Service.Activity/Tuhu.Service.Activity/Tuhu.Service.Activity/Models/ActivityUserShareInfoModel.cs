using System;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class DownloadApp
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string BottomContent { get; set; }

        public bool BottomStatus { get; set; }

        public string ImageContent { get; set; }

        public string TimerContent { get; set; }

        public bool TimerStatus { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
      
    }
    public class ActivityUserShareInfoModel : BaseModel
    {
        public Guid ShareId { get; set; }

        public Guid UserId { get; set; }

        public string PID { get; set; }

        public string Times { get; set; }

        public DateTime? CreatedTime { get; set; }

        public Guid? BatchId { get; set; }

        [Column("ConfigGuid")]
        public Guid? ConfigId { get; set; }
    }
}
