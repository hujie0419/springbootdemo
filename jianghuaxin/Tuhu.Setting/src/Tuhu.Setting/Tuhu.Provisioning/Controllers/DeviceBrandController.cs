using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.Business.ActivityBoard;
using Tuhu.Provisioning.Business.DeviceConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;
using Tuhu.Provisioning.DataAccess.Entity.DeviceConfig;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class DeviceBrandController : Controller
    { 
        private readonly DeviceBrandManager _deviceBrandManager = new DeviceBrandManager();
         

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult List()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.BrandList = GetAllBrandList();

            return View();
        }



        public ActionResult GetGridJson(int brandId = 0, string keyword = "",  Pagination pagination = null)
        { 
            var allList = _deviceBrandManager.GetAllByBrandKeys(brandId, keyword);
             
            var data = JsonConvert.SerializeObject(new
            {
                rows = allList,
                total = pagination?.total ?? allList.Count,
                page = pagination?.page ??1,
                records = pagination?.records?? allList.Count
            });

            return Content(data); 
        }

         


        private List<Tuple<int,string>> GetAllBrandList()
        { 
            var result = new List<Tuple<int, string>>();

            var allList = _deviceBrandManager.GetAllBrandList(); 

            var deviceBrandEntities = allList as DeviceBrandEntity[] ?? allList.ToArray();
            if (deviceBrandEntities.Any())
            {
                deviceBrandEntities.ForEach(m=> result.Add( Tuple.Create(m.PKID,m.DeviceBrand)));
            }
            return result;
        }



        public ActionResult ListForm()
        {
            ViewBag.BrandList = GetAllBrandList();
            return View();
        }

         

        [ValidateInput(false)]
        public ActionResult SubmitForm(DeviceModelFormList data, int keyValue = 0)
        { 
            try
            {
                var result = _deviceBrandManager.BatchHandleDeviceModel(data, User.Identity.Name);


                AsyncHelper.RunSync(RefreshDeviceCache);

                var msg = result ? "操作成功" : "操作失败";
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = msg,//"操作成功",
                    data = data,
                    title = string.Empty
                }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = ex.Message, //"操作成功",
                    data = data,
                    title = string.Empty
                }));
            } 
        } 


        public ActionResult GetListFormJson(string keyValue)
        { 
            var list = _deviceBrandManager.GetAllByBrandKeys(); 
            DeviceModelInfo data = null; 
            var flag = keyValue.Substring(0, 1); 
            int.TryParse(keyValue.Substring(1), out int pkid);
            switch (flag)
            {
                case "B":
                    data = list.FirstOrDefault(m => m.BrandID== pkid);
                    break;
                case "T":
                    data = list.FirstOrDefault(m => m.TypeID == pkid);
                    break;
                case "M":
                    data = list.FirstOrDefault(m => m.ModelID == pkid);
                    break;
            } 
            return Content(JsonConvert.SerializeObject(data));
        }
         

        public ActionResult DeleteForm(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue)||keyValue?.Trim().Length<=1)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败,参数不能为空"
                }));

            bool result = false;

            var flag = keyValue.Substring(0, 1);
            //var pkid = 0;
            int.TryParse(keyValue.Substring(1), out int pkid);
            switch (flag)
            {
                case "B":
                    result= _deviceBrandManager.DeleteBrand(pkid);
                    break;
                case "T":
                    result= _deviceBrandManager.DeleteType(pkid);
                    break;
                case "M":
                    result= _deviceBrandManager.DeleteModel(pkid);
                    break; 
            } 

            AsyncHelper.RunSync(RefreshDeviceCache);

            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = result ? "删除成功"  : "删除失败"
            })); 
        }

        private async Task<bool> RefreshDeviceCache()
        {
            using (var client = new Service.Config.ConfigClient())
            {
               var result=  await client.RefreshDeviceModelCacheAsync();
                return result.Result;
            }
        }


        public async Task<ActionResult> RefreshSource()
        { 
            try
            {

                using (var client = new Tuhu.Service.Config.ConfigClient())
                {

                    var result =  await client.RefreshDeviceModelCacheAsync();

                    var code = result.Result ? 1 : -1;
                    var msg = result.Result ? "刷新成功" : "刷新失败";


                    return Content(JsonConvert.SerializeObject(new
                    {
                        Code = code,
                        Msg = msg
                    }));
                } 
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    Msg = "刷新失败",
                    Code = -1,
                    Error = ex.Message
                }));
            }
        }
         
        

    } 
}