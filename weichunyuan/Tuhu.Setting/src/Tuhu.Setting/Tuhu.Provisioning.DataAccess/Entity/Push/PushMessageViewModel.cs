using System;

namespace Tuhu.Provisioning.DataAccess.Entity.Push
{
    public class PushMessageViewModel
    {
        public string PKID { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int PushType { get; set; }
        public string AfterOpen { get; set; }
        public string AppActivity { get; set; }
        public string AndriodKey1 { get; set; }
        public string AndriodValue1 { get; set; }
        public string AndriodKey2 { get; set; }
        public string AndriodValue2 { get; set; }
        public string IOSKey1 { get; set; }
        public string IOSValue1 { get; set; }
        public string IOSKey2 { get; set; }
        public string IOSValue2 { get; set; }
        public string IOSKey3 { get; set; }
        public string IOSValue3 { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? ExpireTime { get; set; }
        public DateTime? APPExpireTime { get; set; }
        public string HeadImageUrl { get; set; }


        public string AppMsgType { get; set; }

        public string OperUser { get; set; }
        public string BigImagePath { get; set; }
        public string RichTextImage { get; set; }

        public bool IOSShowBadge { get; set; }

        public string Tags { get; set; }
        public string IOSMainTitle { get; set; }
        public string IOSTitle { get; set; }
    }
}
