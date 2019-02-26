using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary> 
    /// SE_QRCodeManageConfig:实体类 
    /// </summary>  
    [Serializable]
    public class QRCodeManageModel
    {

        public string appid { get; set; }
        public string appsecret { get; set; }

        #region SE_QRCodeManageConfig

        public int Id { get; set; }

        public string ChannelName { get; set; }

        public int QRCodeType { get; set; }

        public string QRCodeUrl { get; set; }

        public string ValidityTime { get; set; }

        public int TraceId { get; set; }

        public DateTime CreateTime { get; set; }

        public int IsShow { get; set; }

        public string ResponseContent { get; set; }

        #endregion
    }
}
