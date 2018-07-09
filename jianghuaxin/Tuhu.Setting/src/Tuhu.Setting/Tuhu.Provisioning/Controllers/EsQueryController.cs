using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.Business.Push;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity.Push;
using Tuhu.Provisioning.Models.Push;

namespace Tuhu.Provisioning.Controllers
{
    public class EsQueryController : Controller
    {
        private static string EsUrl = WebConfigurationManager.AppSettings["EsUrl"];
        private readonly string Get;

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SubmitEsQuery(string method,string esIndex, string esJson)
        {
            var methodhttp = (Method)Enum.Parse(typeof(Method),method);
            try
            {
                var request = new RestRequest(methodhttp);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", esJson, ParameterType.RequestBody);
                var requestClient = new RestClient(EsUrl + esIndex);
                requestClient.Encoding = Encoding.UTF8;
                var resultResponse = requestClient.ExecuteAsPost(request, method);
                return Json(new
                {
                    isSuccess = (resultResponse.StatusCode == System.Net.HttpStatusCode.OK),
                    result = resultResponse.Content
                }, "text/html");
            }
            catch (Exception)
            {

                return Json(new
                {
                    isSuccess = false,
                    result = "异常"
                }, "text/html");
            }
        }
    }
}