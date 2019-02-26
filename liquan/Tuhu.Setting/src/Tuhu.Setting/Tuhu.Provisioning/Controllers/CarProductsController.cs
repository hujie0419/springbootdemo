using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Common.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Tuhu.Service.Product;
using Tuhu.Service.ConfigLog;
using Tuhu.Provisioning.Common;
using Tuhu.Service.CarProducts;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.CarProducts;

namespace Tuhu.Provisioning.Controllers
{
    public class CarProductsController : Controller
    {
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        readonly CarProductsManager _manager = new CarProductsManager();
        private static readonly ILog logger = LogManager.GetLogger<CarProductsController>();

        #region banner
        /// <summary>
        /// banner列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[PowerManage(IwSystem = "OperateSys")]
        public ActionResult SelectBannerList(int type)
        {
            var result = _manager.GetCarProductsBannerList(type);
            return Content(JsonConvert.SerializeObject(result, DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 保存banner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SaveBanner(CarProductsBanner model)
        {
            // 车品坑位 启用中的不能存在相同顺序
            if ((model.Type == 1 || model.Type == 2) && model.IsEnabled == true)
            {
                if (_manager.IsExistEqualSort(model))
                {
                    return Json(new { Success = false, Msg = "系统已经存在此排序，请勿重复添加" });
                }
            }

            // 车品坑位 启用中的不能大于10个
            if (model.Type == 2 && model.IsEnabled == true)
            {
                if (_manager.SelectCarProductsBannerCount(model) >= 10)
                {
                    return Json(new { Success = false, Msg = "已经存在10个启用数据，请勿重复添加" });
                }
            }

            // 弹层 启用中的只能有一个
            if (model.Type == 8 && model.IsEnabled == true)
            {
                if (_manager.SelectCarProductsBannerCount(model) >= 1)
                {
                    return Json(new { Success = false, Msg = "只能存在一条启用状态的弹层，请勿重复添加" });
                }
            }

            if (model.PKID <= 0)
            {
                var result = _manager.InsertCarProductsBanner(model);
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = (int?) result,
                        ObjectType = "CarProductsBanner",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "新建",
                        Creator = User.Identity.Name,
                    }));
                }
                return Json(new { Success = result > 0 });
            }
            else
            {
                var beforeModel = _manager.GetCarProductsBannerEntity(model.PKID ?? 0);
                var result = _manager.UpdateCarProductsBanner(model);
                if (result)
                {
                    using (var client = new ConfigLogClient())
                    {
                        client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = model.PKID,
                            ObjectType = "CarProductsBanner",
                            BeforeValue = JsonConvert.SerializeObject(beforeModel),
                            AfterValue = JsonConvert.SerializeObject(model),
                            Remark = "更新",
                            Creator = User.Identity.Name,
                        }));
                    }
                }
                return Json(new { Success = result });
            }
        }

        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteBanner(int id)
        {
            if (id > 0)
            {
                var banner = _manager.GetCarProductsBannerEntity(id);
                if (_manager.DeleteCarProductsBanner(id))
                {
                    using (var client = new ConfigLogClient())
                    {
                        client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = id,
                            ObjectType = "CarProductsBanner",
                            BeforeValue = JsonConvert.SerializeObject(banner),
                            AfterValue = "",
                            Remark = "删除",
                            Creator = User.Identity.Name,
                        }));
                    }
                    return Json(true);
                }
            }
            return Json(false);
        }
        #endregion

        #region floor
        /// <summary>
        /// 楼层列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[PowerManage(IwSystem = "OperateSys")]
        public ActionResult FloorList()
        {
            var result = _manager.GetFloorList();
            return Content(JsonConvert.SerializeObject(result, DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 楼层配置列表
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        [HttpGet]
        //[PowerManage(IwSystem = "OperateSys")]
        public ActionResult FloorConfigList(int floorId)
        {
            var result = _manager.GetFloorConfigList(floorId);
            return Content(JsonConvert.SerializeObject(result, DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 读取车品类目
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCatalogs()
        {
            using (var client = new ProductClient())
            {
                //车品分类查询
                var res = client.GetCategoryDetailLevelsByCategory("AutoProduct");

                if (!res.Success)
                    return Json(new { Success = false });

                var categorys = res.Result.ChildCategorys;
                var list = new List<Categorys>();

                foreach (var item in categorys)
                {
                    if (item.Level == 2)
                    {
                        list.Add(new Categorys
                        {
                            Id = item.Id.ToString(),
                            DisplayName = item.DisplayName,
                            CategoryName = item.CategoryName
                        });
                    }
                }
                return Json(new { Success = true, Data = list });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult GetChildCatalogs(string code)
        {
            using (var client = new ProductClient())
            {
                //车品分类查询
                var res = client.GetCategoryDetailLevelsByCategory("AutoProduct");

                if (!res.Success)
                    return Json(new { Success = false });

                var categorys = res.Result.ChildCategorys;
                var parentId = categorys.FirstOrDefault(p => p.Level == 2 && p.CategoryName == code).Id;
                var list = categorys.Where(p => p.Level == 3 && p.ParentId == parentId)?.ToList();
                return Json(new { Success = true, Data = list });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public ActionResult GetCarProductsFloorContent(int floorId)
        {
            var model = _manager.GetCarProductsFloorEntity(floorId);
            var list = _manager.GetCarProductsFloorConfigList(floorId);
            return Json(new { Floor = model, Config = list });
        }

        /// <summary>
        /// 获取楼层banner信息
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public ActionResult GetFloorBanner(int floorId)
        {
            var model = _manager.GetCarProductsBannerEntityByFloorId(floorId);

            return Content(JsonConvert.SerializeObject(model, DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RefreshFloor()
        {
            using (var client = new RefreshClient())
            {
                var result = await client.RefreshSelectCarProductsDataAsync();
                result.ThrowIfException(true);

                result = await client.RefreshSelectHomePageConfigAsync();
                result.ThrowIfException(true);

                return Json(result.Result);
            }
        }

        /// <summary>
        /// 保存楼层配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public ActionResult SaveFloor(string model, string config)
        {
            // 反序列成车品对象
            var floor = JsonConvert.DeserializeObject<CarProductsFloor>(model);
            var configModel = JsonConvert.DeserializeObject<List<CarProductsFloorConfig>>(config);

            // 车品坑位 启用中的不能存在相同顺序
            if (_manager.IsExistEqualFloorSort(floor))
            {
                return Json(new { Success = false, Msg = "系统已经存在此排序，请勿重复添加" });
            }
            // 车品坑位 启用中的不能存在相同楼层
            if (_manager.IsExistEqualFloorName(floor))
            {
                return Json(new { Success = false, Msg = "系统已经存在此楼层，请勿重复添加" });
            }

            if (floor.PKID <= 0)
            {
                var result = _manager.InsertCarProductsFloor(floor);
                if (result > 0)
                {
                    foreach (var item in configModel)
                    {
                        item.FKFloorID = result;
                        _manager.InsertCarProductsFloorConfig(item);
                    }
                    using (var client = new ConfigLogClient())
                    {
                        client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = result,
                            ObjectType = "CarProductsFloor",
                            BeforeValue = "",
                            AfterValue = JsonConvert.SerializeObject(model),
                            Remark = "新建",
                            Creator = User.Identity.Name,
                        }));
                    }
                }
                return Json(new { Success = result > 0, Id = result });
            }
            else
            {
                var oldFloor = _manager.GetCarProductsFloorEntity(floor.PKID ?? 0);
                if (_manager.UpdateCarProductsFloor(floor))
                {
                    _manager.DeleteCarProductsFloorConfigByFloorId(floor.PKID ?? 0);
                    foreach (var item in configModel)
                    {
                        item.FKFloorID = floor.PKID ?? 0;
                        _manager.InsertCarProductsFloorConfig(item);
                    }
                    using (var client = new ConfigLogClient())
                    {
                        client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = floor.PKID ?? 0,
                            ObjectType = "CarProductsFloor",
                            BeforeValue = JsonConvert.SerializeObject(oldFloor),
                            AfterValue = JsonConvert.SerializeObject(model),
                            Remark = "修改",
                            Creator = User.Identity.Name,
                        }));
                    }
                    return Json(new { Success = true, Id = floor.PKID ?? 0 });
                }
            }

            return Json(new { Success = false, Id = 0 });
        }

        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteFloor(int id)
        {
            if (id > 0)
            {
                var model = _manager.GetCarProductsFloorEntity(id);
                if (_manager.DeleteCarProductsFloor(id))
                {
                    using (var client = new ConfigLogClient())
                    {
                        client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = id,
                            ObjectType = "CarProductsFloor",
                            BeforeValue = JsonConvert.SerializeObject(model),
                            AfterValue = "",
                            Remark = "删除",
                            Creator = User.Identity.Name,
                        }));
                    }
                    return Json(true);
                }
            }
            return Json(false);
        }

        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteFloorConfig(int id)
        {
            if (id > 0)
            {
                if (_manager.DeleteCarProductsFloorConfig(id))
                    return Json(true);
            }
            return Json(false);
        }
        #endregion

        #region common
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RefreshHomePageConfig()
        {
            using (var client = new RefreshClient())
            {
                var result = await client.RefreshSelectHomePageConfigAsync();
                result.ThrowIfException(true);
                return Json(result.Result);
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult UploadImage(string type)
        {
            var result = false;
            var msg = string.Empty;
            var imageUrl = string.Empty;

            if (Request.Files.Count > 0)
            {
                var requestFile = Request.Files[0];
                // 文件扩展名
                string[] allowExtension = { ".jpg", ".gif", ".png", ".jpeg" };

                if (requestFile == null)
                    return Json(new { Status = false, ImageUrl = imageUrl, Msg = msg, Type = type });

                var fileExtension = System.IO.Path.GetExtension(requestFile.FileName);

                if (fileExtension != null && allowExtension.Contains(fileExtension.ToLower()))
                {
                    var buffers = new byte[requestFile.ContentLength];
                    requestFile.InputStream.Read(buffers, 0, requestFile.ContentLength);
                    var upLoadResult = buffers.UpdateLoadImage();

                    if (!upLoadResult.Item1)
                        return Json(new { Status = false, ImageUrl = imageUrl, Msg = msg, Type = type });

                    result = true;
                    imageUrl = upLoadResult.Item2;
                }
                else
                {
                    msg = "图片格式不对";
                }
            }
            else
            {
                msg = "请选择文件";
            }

            return Json(new { Status = result, ImageUrl = imageUrl, Msg = msg, Type = type });
        }

        /// <summary>
        /// PID校验
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult CheckPid(string pid)
        {
            var product = new ProductModel();
            using (var client = new ProductClient())
            {
                var sku = client.FetchProduct(pid);
                sku.ThrowIfException(true);
                if (sku.Success && sku.Result != null)
                {
                    product.PID = pid;
                    product.OnSale = sku.Result.Onsale;
                }
            };
            if (!string.IsNullOrEmpty(product.PID) && product.PID != pid)
            {
                product.CaseSensitive = true;
            }

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //[PowerManage(IwSystem = "OperateSys")]
        public ActionResult GetModuleList(int type)
        {
            var result = _manager.GetHomePageModuleConfigs(type);
            return Content(JsonConvert.SerializeObject(result, DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 更新模块信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateModule(CarProductsHomePageModuleConfig model)
        {
            if (model.PKID <= 0)
            {
                return Json(false);
            }
            var result = _manager.UpdateHomePageModuleConfig(model);
            if (result)
            {
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "CarProductsModule",
                        BeforeValue = JsonConvert.SerializeObject(model),
                        AfterValue = "",
                        Remark = "更新",
                        Creator = User.Identity.Name,
                    }));
                }
                using (var client = new RefreshClient())
                {
                    var refResult = await client.RefreshSelectHomePageConfigAsync();
                    refResult.ThrowIfException(true);
                    return Json(refResult.Result);
                }
            }

            return Json(result);
        }

    }
}