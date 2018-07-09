using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zhongan.DI.Templet
{
    public class BizParamTemp
    {

        private String[] _propertys = new String[] { "appKey", "bizContent", "timestamp", "serviceName", "format", "signType", "charset", "version", "request_info", "response_info", "sign" };
        public String[] GetPropertys
        {
            get { return _propertys; }
        }

    }
}
