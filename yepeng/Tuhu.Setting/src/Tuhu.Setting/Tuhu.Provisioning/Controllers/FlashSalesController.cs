using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThBiz.Business.OprLogManagement;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.ProductInfomationManagement;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{

    public class FlashSalesController : Controller
    {
        private readonly IFlashSalesManager manager = new FlashSalesManager();
        private readonly IAutoSuppliesManager ASmanager = new AutoSuppliesManager();
        IProductInfomationManager _PImanager = new ProductInfomationManager();

        #region 限时抢购活动管理
        [PowerManage]
        public ActionResult FlashSales()
        {
            List<FlashSales> _AllFlashSales = manager.GetAllFlashSales();
            ViewBag.Android001List = _AllFlashSales.Where(p => p.EnID == "android001").OrderByDescending(p => p.StartTime).ThenBy(p => p.Position).ToList();
            ViewBag.IOS001List = _AllFlashSales.Where(p => p.EnID == "ios001").OrderByDescending(p => p.StartTime).ThenBy(p => p.Position).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult DeleteFlashSales(int id)
        {
            try
            {
                manager.DeleteFlashSales(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ResetFlashSales(int id)
        {
            try
            {
                manager.ResetFlashSales(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddFlashSales(int? id, string EnID)
        {
            if (id.HasValue && id.Value != 0)
            {
                ViewBag.Title = "修改抢购活动";
                return View(manager.GetFlashSalesByID(id.Value));
            }
            else
            {
                if (!string.IsNullOrEmpty(EnID))
                {
                    ViewBag.Title = "新增抢购活动";
                    FlashSales _Model = new FlashSales()
                    {
                        PKID = 0,
                        StartTime = DateTime.Now,
                        CountDown = 24,
                        Position = 0,
                        DisplayColumn = 1,
                        IsBanner = false,
                        Status = 0,
                        IsTomorrowTextActive = false
                    };
                    return View(_Model);
                }
                else { return Content("请选择要添加抢购活动的类型"); }
            }
        }
        [HttpPost]
        public ActionResult AddFlashSales(FlashSales flashSales)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(flashSales.Name))
            {
                kstr = "模块名称不能为空";
                IsSuccess = false;
            }
            if (IsSuccess)
            {
                if (flashSales.PKID == 0)
                {
                    manager.AddFlashSales(flashSales);
                }
                else
                {
                    manager.UpdateFlashSales(flashSales);
                }
                return RedirectToAction("FlashSales");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='FlashSales';</script>";
                return Content(js);
            }
        }
        #endregion

        #region 限时抢购活动产品
        public ActionResult FlashSalesProduct(int? FlashSalesID)
        {
            if (FlashSalesID.HasValue)
            {
                int _FlashSalesID = FlashSalesID.Value;
                FlashSales _FlashSales = manager.GetFlashSalesByID(_FlashSalesID);
                if (_FlashSales != null)
                {
                    List<FlashSalesProduct> _ProList = manager.GetProListByFlashSalesID(_FlashSalesID);
                    ViewBag.FlashSales = _FlashSales;
                    ViewBag.ProList = _ProList;
                    return View();
                }
                else
                {
                    return Content("<script>alert('该活动不存在，请确认该活动是否被删除');location='/FlashSales/FlashSales';</script>");
                }
            }
            else
            {
                return Content("<script>alert('该活动不存在，请从正常来源进入');location='/FlashSales/FlashSales'</script>");
            }
        }

        [HttpPost]
        public ActionResult OperateProduct(int? FlashSalesID, int? PKID, string PID, byte? Position, byte? Status, int? Cate, decimal? PromotionPrice, int? PromotionNum, decimal? MarketPrice, int? MaxNum, int? NumLeft, bool? IsHotSale)
        {
            try
            {
                if (!FlashSalesID.HasValue || !Cate.HasValue)
                {
                    return Json("请确保活动编号和操作类型不为空", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int _PKID = PKID.GetValueOrDefault(0);
                    //删除产品
                    if (Cate == 1)
                    {
                        manager.DeleteFlashSalesProduct(_PKID);
                    }
                    //设置为已禁止
                    else if (Cate == 2)
                    {
                        manager.ChangeStatus(_PKID, Status.GetValueOrDefault(0));
                    }
                    //设置为已禁止
                    else if (Cate == 5)
                    {
                        manager.ChangeIsHotSale(_PKID, IsHotSale ?? false);
                    }
                    //保存产品
                    else if (Cate == 3 || Cate == 4)
                    {
                        int _FlashSalesID = FlashSalesID.GetValueOrDefault(0);
                        FlashSalesProduct _FlashSalesProduct = new FlashSalesProduct
                        {
                            PKID = _PKID,
                            FlashSalesID = _FlashSalesID,
                            PID = PID,
                            Position = Position.GetValueOrDefault(0),
                            Status = Status.GetValueOrDefault(0),
                            PromotionPrice = PromotionPrice.GetValueOrDefault(0),
                            PromotionNum = PromotionNum.GetValueOrDefault(0),
                            MarketPrice = MarketPrice.GetValueOrDefault(0),
                            MaxNum = MaxNum.GetValueOrDefault(0),
                            NumLeft = NumLeft.GetValueOrDefault(0),
                            FlashSalesProductPara = _PImanager.GetFlashSalesProductParaByPID(PID)
                        };
                        if (_FlashSalesProduct.FlashSalesProductPara != null)
                        {
                            if (Cate == 3)
                            {
                                //修改产品
                                manager.UpdateFlashSalesProduct(_FlashSalesProduct);
                            }
                            else
                            {
                                //新增产品
                                if (!Position.HasValue || !Status.HasValue)
                                {
                                    return Json("排序顺序和状态不能为空", JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    manager.AddFlashSalesProduct(_FlashSalesProduct);
                                }
                            }
                        }
                        else
                        {
                            return Json("该产品不存在，请重新选择", JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("保存失败" + (Cate.HasValue && (Cate.Value == 3 || Cate.Value == 4) ? "请确认产品编号不要重复和顺序号为两位内整数" : "") + ex.Message.Replace("'", ""), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult AllOperate(string opstr, int? Cate)
        {
            try
            {
                if (string.IsNullOrEmpty(opstr) || !Cate.HasValue)
                {
                    return Json(new { IsSuccess = false, ReturnStr = "输入的参数不能为空" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string[] _Oparr = opstr.Split('*');
                    foreach (string Paras in _Oparr)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(Paras))
                            {
                                string[] _Para = Paras.Split(',');
                                if (_Para.Length == 8)
                                {
                                    int _PKID = int.Parse(_Para[0]);
                                    string _PID = _Para[1];
                                    byte _Position = byte.Parse(_Para[2]);
                                    decimal _PromotionPrice = decimal.Parse(_Para[3]);
                                    decimal _MarketPrice = decimal.Parse(_Para[4]);
                                    int _PromotionNum = int.Parse(_Para[5]);
                                    int _MaxNum = int.Parse(_Para[6]);
                                    int _NumLeft = int.Parse(_Para[7]);
                                    if (!string.IsNullOrEmpty(_PID))
                                    {
                                        //删除
                                        if (Cate == 1)
                                        {
                                            manager.DeleteFlashSalesProduct(_PKID);
                                        }
                                        //保存
                                        else if (Cate == 2)
                                        {
                                            FlashSalesProduct _FlashSalesProduct = new FlashSalesProduct
                                            {
                                                PKID = _PKID,
                                                PID = _PID,
                                                Position = _Position,
                                                PromotionPrice = _PromotionPrice,
                                                MarketPrice = _MarketPrice,
                                                PromotionNum = _PromotionNum,
                                                MaxNum = _MaxNum,
                                                NumLeft = _NumLeft,
                                                FlashSalesProductPara = _PImanager.GetFlashSalesProductParaByPID(_PID)
                                            };
                                            manager.UpdateFlashSalesProduct(_FlashSalesProduct);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                return Json(new { IsSuccess = true, ReturnStr = Cate.Value == 1 ? "批量删除成功" : "批量保存成功" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { IsSuccess = false, ReturnStr = "保存失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        [PowerManage]
        public ActionResult Index()
        {
            return View();
        }
        #region 网站限时抢购配置


        public ActionResult CreateOrUpdateActivityProduct(Guid? ActivityID, ActivityModel model, string Products)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            model.Products = Serializer.Deserialize<List<ActivityProduct>>(Products);
            //model.Products == JsonConvert.DeserializeObject<List<ActivityProduct>>(Products);
            #region 优惠券ID的处理
            var pcodeids = model.PCodeIDS.Split(';');
            model.PCodeIDS = string.Empty;
            var validateIDS = string.Empty;
            var flag = true;
            foreach (var id in pcodeids)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    Guid gid;
                    if (Guid.TryParse(id, out gid))
                    {
                        model.PCodeIDS += id + ";";
                        if (!validateIDS.Contains(id))
                        {
                            validateIDS += id + ";";
                        }
                    }
                    else
                        flag = false;
                }
            }
            if (!flag)
                return Json(-101);//优惠券ID配置有误
            if (validateIDS != model.PCodeIDS)
                return Json(-102);
            if (model.PCodeIDS.Length > 0)
                model.PCodeIDS = model.PCodeIDS.Substring(0, model.PCodeIDS.Length - 1);
            #endregion
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new SqlDbHelper(conn))
            {

                if (ActivityID == null)
                {
                    //添加活动
                    var cmd = new SqlCommand("Activity..Action_CreateActivity");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                    cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                    cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                    cmd.Parameters.AddWithValue("@Author", User.Identity.Name);
                    cmd.Parameters.AddWithValue("@WebBanner", model.WebBanner);
                    cmd.Parameters.AddWithValue("@WebOtherPart", model.WebOtherPart);
                    cmd.Parameters.AddWithValue("@WebBackground", model.WebBackground);
                    cmd.Parameters.AddWithValue("@ActiveType", model.ActiveType);
                    cmd.Parameters.AddWithValue("@PlaceQuantity", model.PlaceQuantity);
                    cmd.Parameters.AddWithValue("@PCodeIDS", string.IsNullOrEmpty(model.PCodeIDS) ? (object)null : model.PCodeIDS);
                    cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@ActivityID",
                        Direction = ParameterDirection.Output,
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 50
                    });
                    dbHelper.ExecuteNonQuery(cmd);
                    int res = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                    model.ActivityID = Guid.Parse(cmd.Parameters["@ActivityID"].Value.ToString());

                    if (res > 0)
                    {
                        var oprLog = new ThBiz.DataAccess.Entity.OprLog();
                        oprLog.Author = ThreadIdentity.Operator.Name;
                        oprLog.ChangeDatetime = DateTime.Now;
                        oprLog.AfterValue = model.ActivityID.ToString();
                        oprLog.ObjectType = "FlashSale";
                        oprLog.Operation = "创建活动";
                        new OprLogManager().AddOprLog(oprLog);

                        foreach (var item in model.Products)
                        {
                            //添加产品
                            using (var cmd2 = new SqlCommand(@"IF NOT EXISTS(SELECT 1 FROM Activity..tbl_FlashSaleProducts AS FSP WHERE FSP.PID=@PID AND FSP.ActivityID=@ActivityID) BEGIN INSERT INTO Activity..tbl_FlashSaleProducts ( PID ,Position ,ActivityID ,Price ,TotalQuantity ,MaxQuantity ,SaleOutQuantity,InstallAndPay,Level,ImgUrl,IsUsePCode,Channel,IsJoinPlace,FalseOriginalPrice,CreateDateTime ,LastUpdateDateTime) VALUES  ( @PID , @Position ,@ActivityID , @Price , @TotalQuantity , @MaxQuantity ,0,@InstallAndPay,@Level,@ImgUrl,@IsUsePCode,@Channel,@IsJoinPlace,@FalseOriginalPrice, GETDATE() , GETDATE()) END"))
                            {
                                cmd2.CommandType = CommandType.Text;

                                #region AddParameters
                                cmd2.Parameters.AddWithValue("@PID", item.PID);
                                cmd2.Parameters.AddWithValue("@Position", item.Position);
                                cmd2.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                                cmd2.Parameters.AddWithValue("@Price", item.Price);
                                cmd2.Parameters.AddWithValue("@FalseOriginalPrice", item.FalseOriginalPrice == 0 ? (object)null : item.FalseOriginalPrice);
                                cmd2.Parameters.AddWithValue("@TotalQuantity", item.TotalQuantity == 0 ? (object)null : item.TotalQuantity);
                                cmd2.Parameters.AddWithValue("@MaxQuantity", item.MaxQuantity == 0 ? (object)null : item.MaxQuantity);
                                cmd2.Parameters.AddWithValue("@Level", item.Level);
                                cmd2.Parameters.AddWithValue("@InstallAndPay", item.InstallAndPay == "不限" ? (object)null : item.InstallAndPay);
                                cmd2.Parameters.AddWithValue("@ImgUrl", item.ImgUrl);
                                cmd2.Parameters.AddWithValue("@IsUsePCode", item.IsUsePCode);
                                cmd2.Parameters.AddWithValue("@IsJoinPlace", item.IsJoinPlace);
                                cmd2.Parameters.AddWithValue("@Channel", item.Channel);
                                #endregion

                                res = dbHelper.ExecuteNonQuery(cmd2);

                            }
                        }
                    }
                    //-99已存在该活动名称,
                    return Json(res);
                }
                else
                {
                    //修改活动
                    var cmd = new SqlCommand("Activity..Action_UpdateActivityByActivityID");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                    cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                    cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                    cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                    cmd.Parameters.AddWithValue("@Author", User.Identity.Name);
                    cmd.Parameters.AddWithValue("@WebBanner", model.WebBanner);
                    cmd.Parameters.AddWithValue("@WebOtherPart", model.WebOtherPart);
                    cmd.Parameters.AddWithValue("@WebBackground", model.WebBackground);
                    cmd.Parameters.AddWithValue("@ActiveType", model.ActiveType);
                    cmd.Parameters.AddWithValue("@PlaceQuantity", model.PlaceQuantity);
                    cmd.Parameters.AddWithValue("@PCodeIDS", string.IsNullOrEmpty(model.PCodeIDS) ? (object)null : model.PCodeIDS);
                    cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                    dbHelper.ExecuteNonQuery(cmd);
                    int res = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                    if (res > 0)
                    {
                        var oprLog = new ThBiz.DataAccess.Entity.OprLog();
                        oprLog.Author = ThreadIdentity.Operator.Name;
                        oprLog.ChangeDatetime = DateTime.Now;
                        oprLog.AfterValue = model.ActivityID.ToString();
                        oprLog.ObjectType = "FlashSale";
                        oprLog.Operation = "修改活动";
                        new OprLogManager().AddOprLog(oprLog);
                        foreach (var item in model.Products)
                        {
                            if (item.PKID == 0)//新增产品
                            {
                                using (var cmd2 = new SqlCommand(@"IF NOT EXISTS(SELECT 1 FROM Activity..tbl_FlashSaleProducts AS FSP WHERE FSP.PID=@PID AND FSP.ActivityID=@ActivityID) BEGIN INSERT INTO Activity..tbl_FlashSaleProducts ( PID ,Position ,ActivityID ,Price ,TotalQuantity ,MaxQuantity ,SaleOutQuantity,Level,InstallAndPay,ImgUrl,IsUsePCode,Channel,IsJoinPlace,FalseOriginalPrice,CreateDateTime ,LastUpdateDateTime) VALUES  ( @PID , @Position ,@ActivityID , @Price , @TotalQuantity , @MaxQuantity ,0,@Level,@InstallAndPay,@ImgUrl,@IsUsePCode,@Channel,@IsJoinPlace,@FalseOriginalPrice, GETDATE() , GETDATE()) END"))
                                {
                                    cmd2.CommandType = CommandType.Text;
                                    #region AddParameters
                                    cmd2.Parameters.AddWithValue("@PID", item.PID);
                                    cmd2.Parameters.AddWithValue("@Position", item.Position);
                                    cmd2.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                                    cmd2.Parameters.AddWithValue("@Price", item.Price);
                                    cmd2.Parameters.AddWithValue("@FalseOriginalPrice", item.FalseOriginalPrice == 0 ? (object)null : item.FalseOriginalPrice);
                                    cmd2.Parameters.AddWithValue("@TotalQuantity", item.TotalQuantity == 0 ? (object)null : item.TotalQuantity);
                                    cmd2.Parameters.AddWithValue("@MaxQuantity", item.MaxQuantity == 0 ? (object)null : item.MaxQuantity);
                                    cmd2.Parameters.AddWithValue("@Level", item.Level);
                                    cmd2.Parameters.AddWithValue("@InstallAndPay", item.InstallAndPay == "不限" ? (object)null : item.InstallAndPay);
                                    cmd2.Parameters.AddWithValue("@ImgUrl", item.ImgUrl);
                                    cmd2.Parameters.AddWithValue("@IsUsePCode", item.IsUsePCode);
                                    cmd2.Parameters.AddWithValue("@IsJoinPlace", item.IsJoinPlace);
                                    cmd2.Parameters.AddWithValue("@Channel", item.Channel);
                                    #endregion
                                    dbHelper.ExecuteNonQuery(cmd2);//不用res接收.否则最后一条PID重复的话res就会是-1
                                }
                            }
                            else
                            {
                                //修改产品
                                var cmd2 = new SqlCommand(@"UPDATE  Activity..tbl_FlashSaleProducts SET PID=@PID, Position=@Position, Price=@Price, TotalQuantity=@TotalQuantity, MaxQuantity=@MaxQuantity,Level=@Level,InstallAndPay=@InstallAndPay,ImgUrl=@ImgUrl,IsUsePCode=@IsUsePCode,Channel=@Channel,IsJoinPlace=@IsJoinPlace,FalseOriginalPrice=@FalseOriginalPrice, LastUpdateDateTime=GETDATE() WHERE PKID=@PKID");
                                cmd2.CommandType = CommandType.Text;
                                #region AddParameters
                                cmd2.Parameters.AddWithValue("@PID", item.PID);
                                cmd2.Parameters.AddWithValue("@Position", item.Position);
                                cmd2.Parameters.AddWithValue("@Price", item.Price);
                                cmd2.Parameters.AddWithValue("@FalseOriginalPrice", item.FalseOriginalPrice == 0 ? (object)null : item.FalseOriginalPrice);
                                cmd2.Parameters.AddWithValue("@TotalQuantity", item.TotalQuantity == 0 ? (object)null : item.TotalQuantity);
                                cmd2.Parameters.AddWithValue("@MaxQuantity", item.MaxQuantity == 0 ? (object)null : item.MaxQuantity);
                                cmd2.Parameters.AddWithValue("@PKID", item.PKID);
                                cmd2.Parameters.AddWithValue("@Level", item.Level);
                                cmd2.Parameters.AddWithValue("@InstallAndPay", item.InstallAndPay == "不限" ? (object)null : item.InstallAndPay);
                                cmd2.Parameters.AddWithValue("@ImgUrl", item.ImgUrl);
                                cmd2.Parameters.AddWithValue("@IsUsePCode", item.IsUsePCode);
                                cmd2.Parameters.AddWithValue("@IsJoinPlace", item.IsJoinPlace);
                                cmd2.Parameters.AddWithValue("@Channel", item.Channel);
                                #endregion
                                res = dbHelper.ExecuteNonQuery(cmd2);
                            }
                        }
                    }
                    if (res == 0 && model.ActiveType == 1)
                        res = -98;
                    //-1已存在该活动名称,-2增加活动失败
                    return Json(res);
                }
            }
        }

        public ActionResult DelActivityProduct(string pkids)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"DELETE Activity..tbl_FlashSaleProducts WHERE PKID IN (" + pkids + ")");
                cmd.CommandType = CommandType.Text;
                return Json(dbHelper.ExecuteNonQuery(cmd));
            }
        }
        public ActionResult DelActivity(string activityID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"Activity..Action_DeleteActivityByActivityID");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActivityID", activityID);
                cmd.Parameters.AddWithValue("@Author", User.Identity.Name);
                return Json(dbHelper.ExecuteNonQuery(cmd));

            }
        }
        /// <summary>
        /// 校验配置的产品是否在其他活动中存在不同价格
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public ActionResult CheckPIDSamePrice(string p)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<ActivityProduct> model = Serializer.Deserialize<List<ActivityProduct>>(p);
            if (model == null || model.Count() == 0)
                return Content("没有产品");
            var content = "";
            foreach (var item in model)
            {
                var msg = CheckPIDSamePriceInOtherActivity(item);
                if (msg != null)
                    content += msg;
            }
            if (content == "")
                content = "与其他活动相同产品价格不冲突。";
            return Content(content);

        }
        private string CheckPIDSamePriceInOtherActivity(ActivityProduct product)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT	FS.ActivityID,FSP.Price,FSP.PID
                                            FROM	Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
                                            JOIN	Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK )
		                                            ON FSP.ActivityID = FS.ActivityID
                                            WHERE	FS.EndDateTime > GETDATE()
		                                            AND FSP.PID = @PID
		                                            AND FSP.Price <> @Price
		                                            AND FS.ActivityID<>@ActivityID
		                                            AND ActiveType IN ( 0, 1 )");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PID", product.PID);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@ActivityID", product.ActivityID);
                var dt = dbHelper.ExecuteDataTable(cmd);
                if (dt == null || dt.Rows.Count == 0)
                    return null;
                var wrongMsg = "";
                foreach (DataRow item in dt.Rows)
                {
                    wrongMsg += "PID:" + item["PID"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;活动ID:" + item["ActivityID"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;价格:" + Convert.ToDouble(item["Price"]).ToString("C") + "<br/>";
                }
                return wrongMsg;
            }
        }
        #endregion
    }
}