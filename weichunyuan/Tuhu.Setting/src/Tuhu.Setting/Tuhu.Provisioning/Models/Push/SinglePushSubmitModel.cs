using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.Provisioning.Models.Push
{
    public class SinglePushSubmitModel
    {
        public SinglePushSubmitModel()
        {
            ExtraDict = new List<ExtraKey>();
            ContentDesc = new List<ExtraKey>();
            KeyValueDesc = new List<ExtraKey>();
            SoundType = "";
        }

        public int Sound { get; set; }
        public int Shake { get; set; }
        public int Light { get; set; }
        public int Front { get; set; }
        public int Wifi { get; set; }
        public int PKID { get; set; }
        public int BatchID { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string TemplateTitle { get; set; }
        public int? Bdage { get; set; }
        public string Content { get; set; }
        public IEnumerable<ExtraKey> ExtraDict { get; set; }
        public string AppActivity { get; set; }
        public string AppExpireTime { get; set; }
        public string SoundType { get; set; }
        public string ImageUrl { get; set; }
        public string PlanName { get; set; }
        public IEnumerable<ExtraKey> ContentDesc { get; set; }
        public IEnumerable<ExtraKey> KeyValueDesc { get; set; }
        public IEnumerable<ExtraKey> WxTemplateColors { get; set; }
        public string ExpireDay { get; set; }
        public string ExpireHour { get; set; }
        public string ExpireMinute { get; set; }
        public MessageType? MessageType { get; set; }
        public int ShowBdage { get; set; }
        public bool IsPreview { get; set; }
        public DeviceType DeviceType { get; set; }
        public string WxTemplateID { get; set; }
        public string IsEnable { get; set; }
        public int? BoxID { get; set; }
        public string WxAppEmKeyWord { get; set; }
        /// <summary>
        /// 导航类型id(新消息类型)
        /// </summary>
        public int? MessageNavigationTypeId { get; set; }
        /// <summary>
        /// 消息盒子推送形式
        /// </summary>
        public MessageBoxShowType MessageBoxShowType { get; set; }

        public TemplateType? TemplateType { get; set; }

        /// <summary>
        /// 是否勾选近期激活用户选项
        /// </summary>
        public bool IsUseCurrentActiveUser { get; set; }
        /// <summary>
        /// 是否勾选其余用户
        /// </summary>
        public bool IsUseOtherUser { get; set; }
        /// <summary>
        ///模板消息=1,
        ///优先图文消息，模板次之=2
        ///图文消息=3
        /// </summary>
        public int CurrentActiveUserOptionItem { get; set; }

        /// <summary>
        /// 微信图文消息ID
        /// </summary>
        public string WeixinMediaId { get;  set; }
    }
}