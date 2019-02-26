using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShareOrderConfig
    {
        public int Id { get; set; }

        public string AppLink { get; set; }

        public string IOSStartVersions { get; set; }

        public string IOSEndVersions { get; set; }

        public string AndroidStartVersions { get; set; }

        public string AndroidEndVersions { get; set; }

        public string Prompt { get; set; }

        public string ButtonValue { get; set; }

        public string PreviewIntroduce { get; set; }

        public string Image { get; set; }

        public string QRCodeIntroduce { get; set; }

        public string QRCodeLink { get; set; }

        public DateTime? CreateTime { get; set; }

        public bool Status { get; set; }
    }


    public class OrderSharedPushMessageConfig
    {
        public int Id { get; set; }

        /// <summary>
        /// system 系统消息 , activity :优惠活动
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string MessageTilte { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// 推送标题
        /// </summary>
        public string PushTitile { get; set; }

        /// <summary>
        /// 推送内容
        /// </summary>
        public string PushContent { get; set; }

        public DateTime? CreateTime { get; set; }


        /// <summary>
        /// AndriodModel
        /// </summary>
        public AndriodModel AndriodModel { get; set; }

        /// <summary>
        /// IOSModel
        /// </summary>
        public IOSModel IOSModel { get; set; }


        public string IOSCommunicationValue { get; set; }

        public string AndroidCommunicationValue { get; set; }

    }

    public class AndriodModel
    {      
        /// <summary>
        /// App打开方式
        /// </summary>
        public AfterOpenEnum AfterOpen { get; set; } = AfterOpenEnum.GoActivity;
        /// <summary>
        /// Andriod处理值
        /// </summary>
        public string AppActivity { get; set; }

        /// <summary>
        /// 附加参数1
        /// </summary>
        public string ExKey1 { get; set; }
        /// <summary>
        /// 附加参数1值
        /// </summary>
        public string ExValue1 { get; set; }
        /// <summary>
        ///  附加参数2
        /// </summary>
        public string ExKey2 { get; set; }
        /// <summary>
        /// 附加参数2值
        /// </summary>
        public string ExValue2 { get; set; }
    }
    public enum AfterOpenEnum
    {
        /// <summary>
        /// 打开App
        /// </summary>
        GoApp,
        /// <summary>
        /// 打开特定的Activity
        /// </summary>
        GoActivity,
    }
    public class IOSModel
    {
        /// <summary>
        /// 附加参数1
        /// </summary>
        public string ExKey1 { get; set; }
        /// <summary>
        /// 附加参数1值
        /// </summary>
        public string ExValue1 { get; set; }
        /// <summary>
        ///  附加参数2
        /// </summary>
        public string ExKey2 { get; set; }
        /// <summary>
        /// 附加参数2值
        /// </summary>
        public string ExValue2 { get; set; }

        /// <summary>
        ///  附加参数3
        /// </summary>
        public string ExKey3 { get; set; }
        /// <summary>
        /// 附加参数3值
        /// </summary>
        public string ExValue3 { get; set; }

        /// <summary>
        /// 是否显示角标
        /// </summary>
        public bool ShowBadge { get; set; } = true;
    }
}
