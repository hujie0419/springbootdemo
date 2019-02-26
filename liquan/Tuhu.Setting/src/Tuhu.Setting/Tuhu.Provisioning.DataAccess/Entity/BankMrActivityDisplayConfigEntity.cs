using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BankMrActivityDisplayConfigEntity
    {
        public int PKID { get; set; }
        public Guid? ActivityId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AppJumpUrl { get; set; }
        public string H5JumpUrl { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? DisplayBeginTime { get; set; }
        public string DisplayBeginTimeString
        {
            get
            {
                return DisplayBeginTime?.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public DateTime? DisplayEndTime { get; set; }
        public string DisplayEndTimeString
        {
            get
            {
                return DisplayEndTime?.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public int Sort { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public string CreatedUser { get; set; }

        public IEnumerable<BankMRActivityDisplayRegionEntity> Region { get; set; }
    }
}
