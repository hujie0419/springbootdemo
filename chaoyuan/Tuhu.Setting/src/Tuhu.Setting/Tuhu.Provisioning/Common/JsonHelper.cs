using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Common
{
    public static class JsonHelper
    {
        //获取json串内的指定值
        public static string GetJsonValue_JObject(string jsonStr, string key)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(jsonStr))
            {
                JObject jb = JsonConvert.DeserializeObject(jsonStr) as JObject;
                result = (jb[key] ?? "").ToString();
            }
            return result;
        }
    }
}