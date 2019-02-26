using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.CityAging;
using Tuhu.Provisioning.Business.LimitAreaSaleManager;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class CityAgingController : Controller
    {
        #region 查询参数

        private int _pageIndex;

        private int PageIndex
        {
            get
            {
                if (_pageIndex <= 0)
                {
                    if (!int.TryParse(Request["PageIndex"], out _pageIndex))
                        _pageIndex = 1;
                }

                return _pageIndex;
            }
        }

        private int _pageSize;

        private int PageSize
        {
            get
            {
                if (_pageSize <= 0)
                {
                    if (!int.TryParse(Request["PageSize"], out _pageSize) || _pageSize >= 200)
                        _pageSize = 20;
                }

                return _pageSize;
            }
        }

        private string _keyWord;

        private string KeyWord
        {
            get
            {
                if (string.IsNullOrEmpty(_keyWord))
                {
                    _keyWord = Request["KeyWord"];
                }

                return _keyWord;
            }
        }

        private int _isLImit;

        private int IsLimit
        {
            get
            {
                _isLImit = Convert.ToInt32(Request["isLImit"]);
                return _isLImit;
            }
        }

        private int _isAllowSale;

        private int IsAllowSale
        {
            get
            {
                _isAllowSale = Convert.ToInt32(Request["isallowSale"]);
                return _isAllowSale;
            }
        }

        private int _cityId;

        private int CityId
        {
            get
            {
                _cityId = Convert.ToInt32(Request["City"]);
                return _cityId;
            }
        }

        private int _provinceId;

        private int ProvinceId
        {
            get
            {
                _provinceId = Convert.ToInt32(Request["Province"]);
                return _provinceId;
            }
        }

        #endregion


        public ActionResult Index()
        {
            return View();
        }


        public JsonResult CityAgingSearch()
        {
            Tuple<int, List<CityAreaAgingModel>> result2;
            var municipalit = new[] { "上海市", "北京市", "天津市", "重庆市" }; //4个直辖市
            var dbresult = LimitAreaSaleManager.SelectRegions();
            var result = new List<LimitSaleRegionModel>();
            foreach (var province in dbresult)
            {
                if (municipalit.Contains(province.RegionName))
                {
                    var regionModel = new LimitSaleRegionModel()
                    {
                        ProvinceId = province.PKID,
                        ProvinceName = province.RegionName,
                        CityId = province.PKID,
                        CityName = province.RegionName
                    };
                    result.Add(regionModel);
                }
                else
                {
                    foreach (var city in province.ChildrenRegion)
                    {
                        var regionModel = new LimitSaleRegionModel()
                        {
                            ProvinceId = province.PKID,
                            ProvinceName = province.RegionName,
                            CityId = city.PKID,
                            CityName = city.RegionName
                        };
                        result.Add(regionModel);
                    }
                }
            }

            var tableData = CityAgingManage.SelectCityAgingInfo();
            var tempdata = (from a in result
                            join b in tableData on a.CityId equals b.CityId into temp
                            from b in temp.DefaultIfEmpty()
                            select new CityAreaAgingModel
                            {
                                PKid = b?.PKid ?? -1, //PKid
                                ProvinceId = a.ProvinceId,
                                ProvinceName = a.ProvinceName,
                                CityId = a.CityId,
                                CityName = a.CityName,
                                IsShow = b?.IsShow ?? 1, // 1 时效性 默认打开
                                Title = b?.Title ?? "",
                                Content = b?.Content ?? ""
                            }).Where(
                    r => (r.ProvinceId == ProvinceId || ProvinceId == 0) && (r.CityId == CityId || CityId == 0))
                .OrderBy(r => r.CityId).ToList();
            result2 = new Tuple<int, List<CityAreaAgingModel>>(tempdata.Count,
                tempdata.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList());
            return Json(result2);

        }

        /// <summary>
        /// 保存时效信息
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveCityAging(CityAgingModel model)
        {
            var result = new MJsonResult() { Status = true };
            try
            {
                var returnValue = 0;
                if (model == null)
                {
                    result.Status = false;
                    result.Msg = "model 不能为空！";
                    return Json(result);
                }

                var operatorsUserName = ThreadIdentity.Operator.Name;

                var beforvalue = new CityAgingModel();
                if (model.PKid != -1)
                {
                    var tableData = CityAgingManage.SelectCityAgingInfoByIds(new List<int>() { model.PKid });
                    if (tableData != null && tableData.Any())
                    {
                        beforvalue = tableData.FirstOrDefault();
                    }
                    else
                    {
                        beforvalue = null;
                    }
                }
                else
                {
                    beforvalue = null;
                }

                if (model.PKid == -1) 
                {
                    returnValue = CityAgingManage.CreateSelectCityAging(model.CityId, model.CityName, model.IsShow,
                        model.Title, model.Content, operatorsUserName);
                }
                else
                {
                    model.UpdateUser = operatorsUserName;
                    returnValue = CityAgingManage.UpdateSelectCityAging(model.PKid, model.IsShow, model.Title,
                        model.Content, operatorsUserName);
                }

                if (returnValue > 0)
                {
                    SaveLog(beforvalue, model);
                    result.Status = true;
                    result.Msg = "保存成功！";
                }
                else
                {
                    result.Status = false;
                    result.Msg = "保存失败！";
                }

            }
            catch (Exception e)
            {
                result.Status = false;
                result.Msg = "异常：" + e.Message;
            }

            return Json(result);
        }



        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public JsonResult SaveCityAgingBulk(List<CityAgingModel> models)
        {
            var result = new MJsonResult() { Status = true };
            try
            {
                if (models == null)
                {
                    result.Status = false;
                    result.Msg = "数据不能为空！";
                    return Json(result);
                }
                var returnValue = 0;
                var operatorsUserName = ThreadIdentity.Operator.Name;
                //批量查询修改前的历史数据
                var tableDatas = CityAgingManage.SelectCityAgingInfoByIds(models.Select(t => t.PKid).ToList());
                foreach (CityAgingModel model in models)
                {
                    if (model.PKid == -1) //还没有修改过时效性
                    {
                        returnValue = CityAgingManage.CreateSelectCityAging(model.CityId, model.CityName, model.IsShow,
                            model.Title, model.Content, operatorsUserName);
                    }
                    else
                    {
                        model.UpdateUser = operatorsUserName;
                        returnValue = CityAgingManage.UpdateSelectCityAging(model.PKid, model.IsShow, model.Title,
                            model.Content, operatorsUserName);
                    }
                    var beforvalue = new CityAgingModel();
                    if (tableDatas != null && tableDatas.Any())
                    {
                        beforvalue = tableDatas.Where(t => t.PKid == model.PKid).FirstOrDefault();
                    }
                    else
                    {
                        beforvalue = null;
                    }
                
                    if (returnValue > 0)
                    {
                        SaveLog(beforvalue, model);
                    }
                }
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Msg = "异常：" + e.Message;
            }
            return Json(result);
        }

        /// <summary>
        /// 记录日志查询
        /// </summary>
        /// <param name="beforvalue"></param>
        /// <param name="newvalue"></param>
        private void SaveLog(CityAgingModel beforvalue, CityAgingModel newvalue)
        {
            var model = new CityAgingModel()
            {
                CityId = newvalue.CityId,
                CityName = newvalue.CityName,
                CreateUser = newvalue.CreateUser,
                UpdateUser = newvalue.UpdateUser
            };
            if (newvalue.PKid != -1)
            {
                model.PKid = newvalue.PKid;
            }
            if (newvalue.IsShow != -1)
            {
                model.IsShow = newvalue.IsShow;
            }
            if (newvalue.Title != "-1")
            {
                model.Title = newvalue.Title;
            }
            if (newvalue.Content != "-1")
            {
                model.Content = newvalue.Content;
            }
            var oprLog = new FlashSaleProductOprLog
            {
                OperateUser = ThreadIdentity.Operator.Name,
                CreateDateTime = DateTime.Now,
                BeforeValue = JsonConvert.SerializeObject(beforvalue),
                AfterValue = JsonConvert.SerializeObject(model),
                LogType = "CityAging",
                LogId = $"0|{newvalue.CityId}",
                Operation = "修改城市时效开关"
            };

            var result = Tuhu.Provisioning.Business.Logger.LoggerManager.InsertLog("CityAgingOprLog", oprLog);
        }




        public JsonResult GetCityAgingConfigLog(string logId)
        {
            if (string.IsNullOrEmpty(logId))
                return Json(null);
            var result = LoggerManager.SelectCityAgingHistoryByLogId($"0|{logId}", "CityAging");
            return Json(result);
        }
        public ActionResult CityAgingConfigLog(string logId)
        {
            return View((object)logId);
        }


        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateCache()
        {
            var result = new MJsonResult() { Status = true };
            try
            {

                // 更新缓存
                using (var client = new ConfigClient())
                {
                    var reVauel = client.RefreshCityAgingCacheAsync();
                    if (!reVauel.Success)
                    {
                        result.Status = false;
                        result.Msg = "更新缓存失败！";
                    }

                }
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Msg = "异常：" + e.Message;
            }
            return Json(result);

        }

    }
}