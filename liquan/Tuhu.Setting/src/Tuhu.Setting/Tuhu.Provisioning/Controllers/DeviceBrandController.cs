using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.DeviceConfig;
using Tuhu.Provisioning.DataAccess.Entity.DeviceConfig;

namespace Tuhu.Provisioning.Controllers
{
    public class DeviceBrandController : Controller
    {
        private readonly DeviceBrandManager _deviceBrandManager = new DeviceBrandManager();

        /// <summary>
        /// 首页视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 列表视图
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.BrandList = GetAllBrandList();

            return View();
        }

        /// <summary>
        /// 列表数据
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="keyword"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetGridJson(int brandId = 0, string keyword = "", Pagination pagination = null)
        {
            var allList = _deviceBrandManager.GetAllByBrandKeys(brandId, keyword);

            var data = JsonConvert.SerializeObject(new
            {
                rows = allList,
                total = pagination?.total ?? allList.Count,
                page = pagination?.page ?? 1,
                records = pagination?.records ?? allList.Count
            });

            return Content(data);
        }

        /// <summary>
        /// 厂商列表
        /// </summary>
        /// <returns></returns>
        private List<Tuple<int, string>> GetAllBrandList()
        {
            var result = new List<Tuple<int, string>>();

            var allList = _deviceBrandManager.GetAllBrandList();

            var deviceBrandEntities = allList as DeviceBrandEntity[] ?? allList.ToArray();
            if (deviceBrandEntities.Any())
            {
                deviceBrandEntities?.ToList().ForEach(m => result.Add(Tuple.Create(m.PKID, m.DeviceBrand)));
            }
            return result;
        }

        /// <summary>
        /// 编辑视图
        /// </summary>
        /// <returns></returns>
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

                //刷新缓存
                var refResult = AsyncHelper.RunSync(RefreshDeviceCache);

                var msg = result ? "操作成功" : "操作失败";
                msg += refResult ? ",缓存已刷新" : "缓存刷新失败";

                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = msg,
                    data,
                    title = string.Empty
                }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = ex.Message,
                    data,
                    title = string.Empty
                }));
            }
        }

        /// <summary>
        /// 详情页数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetListFormJson(string keyValue)
        {
            var list = _deviceBrandManager.GetAllByBrandKeys();
            DeviceModelInfo data = null;
            var flag = keyValue.Substring(0, 1);
            int.TryParse(keyValue.Substring(1), out int pkid);
            switch (flag)
            {
                case "B":
                    data = list.FirstOrDefault(m => m.BrandID == pkid);
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

        /// <summary>
        /// 删除机型
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult DeleteForm(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue) || keyValue?.Trim().Length <= 1)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败,参数不能为空"
                }));

            bool result = false;

            var flag = keyValue.Substring(0, 1);
            int.TryParse(keyValue.Substring(1), out int pkid);
            switch (flag)
            {
                case "B":
                    result = _deviceBrandManager.DeleteBrand(pkid);
                    break;
                case "T":
                    result = _deviceBrandManager.DeleteType(pkid);
                    break;
                case "M":
                    result = _deviceBrandManager.DeleteModel(pkid);
                    break;
            }

            //刷新缓存
            var refResult = AsyncHelper.RunSync(RefreshDeviceCache);

            var msg = result ? "删除成功" : "删除失败";
            msg += refResult ? ",缓存已刷新" : "缓存刷新失败";

            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = msg
            }));
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RefreshDeviceCache()
        {
            using (var client = new Service.Config.ConfigClient())
            {
                var result = await client.RefreshDeviceModelCacheAsync();
                return result.Result;
            }
        }
    }
}