using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zhongan.DI.Util;
using Newtonsoft.Json;
using K.Domain;

namespace DataInteraction
{
    public static class ZAOP
    {
        private static String appkey = System.Configuration.ConfigurationManager.AppSettings["ZhongAnKey"];
        private static String appservicename = System.Configuration.ConfigurationManager.AppSettings["ZhongAnServiceName"];
        private static String appurl = System.Configuration.ConfigurationManager.AppSettings["ZhongAnUrl"];

        public static IDictionary<String, String> SendData(DInsuranceTyre dInsuranceTyre)
        {
            string rpsBizContent = string.Empty;
            Dictionary<String, String> newDict = new Dictionary<String, String>();

            //系统级别参数
            newDict.Add("appKey", appkey);
            newDict.Add("charset", "UTF-8");
            newDict.Add("serviceName", appservicename);

            newDict.Add("format", "json");
            newDict.Add("version", "1.0.0");
            newDict.Add("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            newDict.Add("signType", "RSA");
            //应用级别参数
            newDict.Add("orderNo", dInsuranceTyre.orderNo ?? string.Empty);
            newDict.Add("orderDate", dInsuranceTyre.orderDate ?? string.Empty);
            newDict.Add("customerName", dInsuranceTyre.customerName ?? string.Empty);
            newDict.Add("customerPhoneNo", dInsuranceTyre.customerPhoneNo ?? string.Empty);
            newDict.Add("plateNumber", dInsuranceTyre.plateNumber ?? string.Empty);
            newDict.Add("storeAddress", dInsuranceTyre.storeAddress ?? string.Empty);
            newDict.Add("storeName", dInsuranceTyre.storeName ?? string.Empty);
            newDict.Add("tyreType", dInsuranceTyre.tyreType ?? string.Empty);
            newDict.Add("tyrePrice", dInsuranceTyre.tyrePrice ?? string.Empty);
            newDict.Add("tyreBatchNo", dInsuranceTyre.tyreBatchNo ?? string.Empty);
            newDict.Add("tyreId", dInsuranceTyre.tyreId ?? string.Empty);
            newDict.Add("idType", dInsuranceTyre.idType ?? string.Empty);
            newDict.Add("idNo", dInsuranceTyre.idNo ?? string.Empty);
            newDict.Add("type", dInsuranceTyre.type ?? string.Empty);
            //公钥私钥
            string publicKeyPem = AppDomain.CurrentDomain.BaseDirectory + "Keys\\public-key-zhongan.pem";
            string privateKeyPem = AppDomain.CurrentDomain.BaseDirectory + "Keys\\rsa_private_key.pem";

            //使用 util方法 加密 加签
            IDictionary<String, String> dic = SignatureUtils.encryptAndSign(newDict, publicKeyPem, privateKeyPem, "utf-8", true, true);
            //string dsdsd = SignatureUtils.CheckSignAndDecrypt(dic, publicKeyPem, privateKeyPem, true, true);
            WebUtils webUtil = new WebUtils();
            string response = webUtil.DoPost(appurl, dic, "utf-8");

            //使用 util方法 验签，解密
            IDictionary<String, String> resp = JsonConvert.DeserializeObject<Dictionary<String, String>>(response);//泛型反序列化
            if (resp.ContainsKey("bizContent"))
            {// 验签，解密
                rpsBizContent = SignatureUtils.CheckSignAndDecrypt(resp, publicKeyPem, privateKeyPem, true, true);
            }
            else
            {//如果为空则不用解密
                resp.Add("bizContent", "");
                rpsBizContent = SignatureUtils.CheckSignAndDecrypt(resp, publicKeyPem, privateKeyPem, true, false);
            }

            //更新bizContent
            resp.Remove("bizContent");
            resp.Add("bizContent", rpsBizContent);

            return resp;


            //显示
            //System.Console.WriteLine("Response:{0}", JsonConvert.SerializeObject(resp));
            //System.Console.WriteLine("bizContent:{0}", resp["bizContent"]);
            //IDictionary<String, String> _BizContent = JsonConvert.DeserializeObject<Dictionary<String, String>>(resp["bizContent"]);
            //System.Console.WriteLine("endDate:{0}", _BizContent["endDate"]);
            //System.Console.Read();
        }
    }
}
