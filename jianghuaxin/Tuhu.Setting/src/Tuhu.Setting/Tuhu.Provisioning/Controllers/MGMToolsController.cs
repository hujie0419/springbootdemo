using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web;
using Tuhu.Service;
using System.Web.Mvc;
using Tuhu.Service.Activity;
using NPOI.SS.Formula.Functions;
using System.Web.Compilation;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Tuhu.Service.Comment;
using Tuhu.Provisioning.Business.MGMT;
using Tuhu.Service.Utility;

namespace Tuhu.Provisioning.Controllers
{
    public class MGMToolsController : Controller
    {
        static List<Type> clientList =new List<Type>();

        //[PowerManage]
        public ActionResult Index()
        {
            return View();
        }

        [PowerManage]
        public ActionResult GetClientInfo()
        {
            if (!clientList.Any())
                LoadAssembly();

            return Json(new
            {
                result = JsonConvert.SerializeObject(new
                {
                    clientInfo = clientList
                    .GroupBy(p => p.Assembly.FullName)
                    .ToDictionary(
                        p => p.Key,
                        v => v.Select(vs => vs.FullName).ToList()).OrderBy(p=>p.Key)
                })
            });
        }
        
        private void LoadAssembly()
        {
            clientList =new List<Type>();
            var assemblies = BuildManager.GetReferencedAssemblies()
                .OfType<Assembly>()
                .Where(ass => ass.FullName.StartsWith("Tuhu.Service."))
                .ToArray();

            assemblies.ForEach(one =>
            {
                clientList.AddRange(one.GetTypes().Where(
                        pt => pt.GetInterfaces().Contains(typeof(ITuhuServiceClient))
                        && !pt.IsInterface).ToArray());
            });
        }


        [PowerManage]
        public ActionResult SubmitServiceRequest(SubmitModel sub)
        {
            if (sub != null && !string.IsNullOrWhiteSpace(sub.MethodName))
            {
                var result = SubmitServiceRequest1(sub.ClientList.Split(new char[] { '|' })[0], sub.ClientList.Split(new char[] { '|' })[1], sub.MethodName?.Trim(), sub.ParaJson, sub.Host, Convert.ToInt16(sub.Point));
                ViewBag.Result = result.ToString();
            }
            else
                ViewBag.Result = "";

            if (!clientList.Any())
                LoadAssembly();
            var model = clientList
                    .GroupBy(p => p.Assembly.FullName)
                    .ToDictionary(
                        p => p.Key,
                        v => v.Select(vs => vs.FullName).ToList());

            //TODO:联动
            //if (ViewBag.MethodList == null)
            //{
            //    ViewBag.MethodList = clientList.ToDictionary(p => p.FullName, p => p.GetMethods().Select(pm => pm.Name).ToList());
            //}

            return View(model);
        }

        //[HttpGet]
        //[HttpPost]
        //[PowerManage]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemly"></param>
        /// <param name="client"></param>
        /// <param name="method"></param>
        /// <param name="parm">
        /// Josn数组
        /// [
        /// {"参数名1":"参数值"},
        /// {"参数名2":"参数值"}
        /// ]
        /// </param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private string SubmitServiceRequest1(string assemly, string client, string method, string parm, string host, int? port)
        {
            if (!clientList.Any())
                LoadAssembly();
            if (!clientList.Any())
                return  "没有程序集" ;
            var containsType = clientList.FirstOrDefault(p => p.Assembly.FullName==(assemly ) && p.FullName == client);
            if (containsType != default(Type))
            {
                var containsAssemly = containsType.Assembly;
                var containsMethod = containsType.GetMethod(method);
                if (containsMethod == null)
                    return "没有此方法";

                var instanceClient = containsAssemly.CreateInstance(client);
                if (!string.IsNullOrEmpty(host) || (port != null && port.Value != 0))
                {
                    var endPoint = containsType.GetProperty("Endpoint");
                    var oldEndPoint = endPoint.GetValue(instanceClient) as ServiceEndpoint;
                    var newAddress = new UriBuilder(oldEndPoint.Address.Uri)
                    {
                        Host = string.IsNullOrEmpty(host) ? oldEndPoint.Address.Uri.Host : host,
                        Port = (port == null || port.Value == 0) ? oldEndPoint.Address.Uri.Port : port.Value
                    };
                    oldEndPoint.Address = new EndpointAddress(
                        newAddress.Uri,
                        oldEndPoint.Address.Identity,
                        oldEndPoint.Address.Headers);
                }
                object result = null;
                if (!string.IsNullOrEmpty(parm))
                {
                    var methodPara = containsMethod.GetParameters();
                    var paraValues = new object[methodPara.Count()];
                    var jsonObj = JsonConvert.DeserializeObject<JToken>(parm);
                    for (var i = 0; i < methodPara.Count(); i++) //顺序需要一致
                    {
                        var parValueStr = jsonObj.Value<JToken>(methodPara[i].Name)?.ToString();
                        if (parValueStr == null)
                        {
                            return $"缺少参数：{methodPara[i].Name}";
                        }
                        var tType = methodPara[i].ParameterType;
                        if ((tType.IsClass && tType != typeof(Nullable)) || tType.GenericTypeArguments.Any())
                        {
                            if (tType == typeof(string) || tType.IsValueType)
                                paraValues[i] = TypeDescriptor.GetConverter(tType).ConvertFromInvariantString(parValueStr);
                            else
                                paraValues[i] = JsonConvert.DeserializeObject(parValueStr, tType);
                        }
                        else
                            paraValues[i] = TypeDescriptor.GetConverter(methodPara[i].ParameterType).ConvertFromInvariantString(parValueStr);
                    }
                    result = containsMethod.Invoke(instanceClient, paraValues);
                }
                else
                    result = containsMethod.Invoke(instanceClient, null);

                return JsonConvert.SerializeObject(result);
            }
            return "创建client失败,assemly为dll名称，client请用全类名(包含命名空间)";
        }

        private object GetServiceResult(object client, Type type, string methodName)
        {
            var method = type.GetMethod(methodName);
            if (method == null)
            {
                return Content("找不到方法");
            }
            var parameters = method.GetParameters();
            var array = new object[parameters.Length];
            parameters.ForEach(x =>
            {
                var text = Request[x.Name];
                object value = null;
                if (text != null)
                {
                    if (x.ParameterType.IsClass)
                    {
                        value = JsonConvert.DeserializeObject(text, x.ParameterType);
                    }
                    else
                    {
                        value = TypeDescriptor.GetConverter(x.ParameterType).ConvertFromInvariantString(text);
                    }
                }
                else
                {
                    value = Activator.CreateInstance(x.ParameterType);
                }
                array[x.Position] = value;
            });
            var result = method.Invoke(client, array);
            return result;
        }
        #region 重置评论状态
        public JsonResult ResetCommentStatus(int orderId)
        {
            using(var client= new ShopCommentClient())
            {
                var result = client.SetShopCommentStatus(orderId);
                if(result.Success && result.Result)
                {
                    return Json($"重置成功{result.Success},{orderId}",JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json($"失败{result.Success},{orderId}", JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region 设置拼团状态
        [HttpGet]
        public JsonResult ChangeGroupBuyingOrderStatus(int orderId)
        {
            return Json("不允许使用", JsonRequestBehavior.AllowGet);
            //var result = MGMToolsManager.ChangeGroupBuyingStatus(orderId);
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

#region 途虎长短链转换
        public ActionResult GetShortUrl()
        {
            return View();
        }
        public JsonResult Transform(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Json(new
                {
                    Code = 0,
                    Info = "参数不符合要求"
                });
            }
            using(var client=new UtilityClient())
            {
                var result = client.GetTuhuDwz(url, "Setting");
                if (result.Success && !string.IsNullOrWhiteSpace(result.Result))
                {
                    return Json(new
                    {
                        Code = 1,
                        Data = result.Result
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = 0,
                        Info = "转换失败"
                    });
                }
            }
        }
    }
#endregion
    public class SubmitModel
    {
        public string ClientList { get; set; }
        public string MethodName { get; set; }
        public string Host { get; set; }
        public string Point { get; set; }
        public string ParaJson { get; set; }
    }
}