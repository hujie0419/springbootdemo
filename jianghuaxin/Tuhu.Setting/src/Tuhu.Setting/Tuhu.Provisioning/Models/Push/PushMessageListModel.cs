using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity.Push;

namespace Tuhu.Provisioning.Models.Push
{
    public class PushMessageListModel
    {
        public List<PushMessageViewModel> PushMessageList { get; set; }
        public int SumNum { get; set; }

        public string PagerStr { get; set; }

        public PushMessageSeachInfoModel SearchInfo { get; set; }
    }

    public class PushMessageSeachInfoModel
    {
        public int? PageNo { get; set; }
    }
}