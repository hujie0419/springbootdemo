using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Vin;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{

    public class VinController : Controller
    {
        private Lazy<VinManager> lazyVinManager = new Lazy<VinManager>();

        private VinManager VinManager
        {
            get { return lazyVinManager.Value; }
        }
        [PowerManage]
        public ActionResult VinEdit()
        {
            return View();
        }
        public ActionResult VinView(int? page)
        {
            var rlt = new ObjectResult();
            GetExistingResult(page, rlt);
            return Content(rlt.ToJson());
        }
        public ActionResult VinRemove(string id)
        {

            id = HttpUtility.UrlDecode(id);
            var r = new ResultBase();
            if (string.IsNullOrEmpty(id))
            {
                r.Error("标识不正确，无法操作");
            }
            else
            {
                if (VinManager.DeleteVin_recordById(id) < 1)
                {
                    r.Error("删除失败");
                }
            }
            return Content(r.ToJson());
        }
        public ActionResult VinUpdate(string id, string phone, string vin)
        {
            id = HttpUtility.UrlDecode(id);
            phone = HttpUtility.UrlDecode(phone);
            vin = HttpUtility.UrlDecode(vin);
            var r = new ResultBase();
            if (DataValidator.Create()
                .NotWhiteSpace(id, phone, vin)
                .Len(17, vin)
                .Len(11, 14, phone)
                .Type<long>(phone)
                .Pass(r, "参数错误，无法操作"))
            {

                if (VinManager.UpdateVin_record(phone, vin, id) < 1)
                {
                    r.Error("更新失败");
                }
            }
            return Content(r.ToJson());
        }

        public ActionResult VinAdd(string phone, string vin, int? page)
        {
            phone = HttpUtility.UrlDecode(phone);
            vin = HttpUtility.UrlDecode(vin);
            var rlt = new ResultBase();

            if (VinManager.IsRepeatVin_record(phone, vin))
            {
                var id = Guid.NewGuid().ToString();
                var u = (User != null && User.Identity != null) ? User.Identity.Name : string.Empty;

                if (!VinManager.InsertVin_record(id, phone, vin, u))
                {
                    rlt.Error("数据录入失败，请联系管理员 " + SqlAdapter.LastError.Message + " " + SqlAdapter.ConnStr());
                }
            }
            else
            {
                rlt.Error("数据已存在");
            }
            return Content(rlt.ToJson());
        }

        private static void GetExistingResult(int? page, ObjectResult rlt)
        {
            int size = 100, ps = 0, pe = size;
            if (page.HasValue)
            {
                ps = (page.Value - 1) * size;
                pe = page.Value * size;
            }

            var vins = new VinManager().GetVin_record(ps, pe);
            var s = vins.Serialize();
            rlt.Object = HttpUtility.UrlEncode(s);
        }

        //选择区域view
        public ActionResult VinSelectArea()
        {
            return View();
        }

        //获取选择区域页面的数据
        public ActionResult GetVin()
        {
            var rlt = new ObjectResult();

            if (VinManager.GetVIN_REGION())
            {
                return Content("{\"error\":true, \"Msg\":\"No Region\"}");
            }
            else
            {
                var s = VinManager.GetVIN_REGIONDyModel().Json(false);
                return Content(s);
            }
        }

        //选择区域的保存事件
        public ActionResult SaveVin(string vinstringObjectArray, string addsProvinceValue)
        {
            var ResultBase = new ResultBase();
            var jss = new JavaScriptSerializer();
            List<VinRegion> VinArraylist = ScriptDeserialize<List<VinRegion>>(vinstringObjectArray);
            if (VinArraylist.Count < 1 && string.IsNullOrEmpty(addsProvinceValue))//数据不为空
            {
                ResultBase.Code = 1;
                ResultBase.Error("数据不能为空");
                return Content(ResultBase.ToJson());
            }

            for (int j = 0; j < VinArraylist.Count; j++)//添加的省和当前有的进行遍历判断
            {
                if (VinArraylist[j].Region == addsProvinceValue)//当有的时候就不需要添加了
                {

                    ResultBase.Code = 1;
                    ResultBase.Error("已有此省的数据，请重新输入");
                    return Content(ResultBase.ToJson());
                }
            }
            //执行改变状态操作
            for (int i = 0; i < VinArraylist.Count; i++)
            {
                VinManager.Operate(VinArraylist[i].IsEnable, VinArraylist[i].ID, VinArraylist[i].Region);
            }
            //执行添加操作
            if (!string.IsNullOrEmpty(addsProvinceValue))
            {
                VinManager.InsertVIN_REGION(addsProvinceValue);
            }
            ResultBase.Code = 0;
            return Content(ResultBase.ToJson());
        }

        //反序列话
        public static T ScriptDeserialize<T>(string input)
        {
            T rtn = default(T);
            JavaScriptSerializer js = new JavaScriptSerializer();
            rtn = js.Deserialize<T>(input);
            return rtn;
        }

        public JsonResult GetTidsByVIN(string vin)
        {
            var result = VehicleService.GetTidsByVIN(vin);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
    }
}