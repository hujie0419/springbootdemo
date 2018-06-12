using System;

namespace Zhongan.DI.JsonModel
{
    public class RequestModel
    {


        private String _appKey;
        /// <summary>
        /// 应用接入唯一key
        /// </summary>
        public String appKey
        {
            get { return _appKey; }
            set { _appKey = value; }
        }


        private string _bizContent;
        /// <summary>
        /// 业务级参数，需加密
        /// </summary>
        public string bizContent
        {
            get { return _bizContent; }
            set { _bizContent = value; }
        }

        private String _timestamp;
        /// <summary>
        /// 时间搓
        /// </summary>
        public String timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        private String _serviceName;
        /// <summary>
        /// 服务名
        /// </summary>
        public String serviceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        private String _format;
        /// <summary>
        /// 响应格式
        /// </summary>
        public String format
        {
            get { return _format; }
            set { _format = value; }
        }


        private String _signType;
        /// <summary>
        /// 签名类型
        /// </summary>
        public String signType
        {
            get { return _signType; }
            set { _signType = value; }
        }

        private String _charset;
        /// <summary>
        /// 字符集
        /// </summary>
        public String charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        private String _version;
        /// <summary>
        /// 版本
        /// </summary>
        public String version
        {
            get { return _version; }
            set { _version = value; }
        }

        private String _request_info;
        /// <summary>
        /// 请求参数
        /// </summary>
        public String request_info
        {
            get { return _request_info; }
            set { _request_info = value; }
        }

        private String _response_info;
        /// <summary>
        /// 应答参数
        /// </summary>
        public String response_info
        {
            get { return _response_info; }
            set { _response_info = value; }
        }


        private String _sign;
        /// <summary>
        /// 签名
        /// </summary>
        public String sign
        {
            get { return _sign; }
            set { _sign = value; }
        }

    }

}
