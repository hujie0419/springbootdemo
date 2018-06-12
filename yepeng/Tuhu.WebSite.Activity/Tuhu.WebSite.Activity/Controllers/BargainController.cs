//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using Tuhu.WebSite.Component.DataAccess;
//using Tuhu.WebSite.Component.Interface.WeChat;
//using Tuhu.WebSite.Component.SystemFramework.Log;
//using Tuhu.WebSite.Web.Activity.BusinessFacade;

//namespace Tuhu.WebSite.Web.Activity.Controllers
//{
//    public class BargainController : Controller
//    {
//        /// <summary>
//        /// Switch between test view and prod view
//        /// </summary>
//        private static string vprefix = ""; //Debugger.IsAttached ? "Test" : ""
//        private static string redirurl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx21f3300bbe61dc90&redirect_uri=http%3A//wx.tuhu.cn/wx/getwxinfo%3Frui%3D1%26cburl%3Dhttp%253A//faxian.tuhu.cn/bargain/prejoin.html%253Faid%253D1%2526wxinfo%253D&response_type=code&scope=snsapi_userinfo&state=fromentry";

//        /// <summary>
//        /// 确定是否已经参加，跳转立即参加 / 来一发页面
//        /// </summary>
//        /// <param name="aid">活动id</param>
//        /// <param name="oid">链接中的朋友微信oid</param>
//        public ActionResult PreJoin(int aid, string oid, string hdsrc)
//        {
//            var px = WxProxy.Create(this);
//            Log.Session("PreJoin aid={0}, oid={1}, hdsrc={2}", Session.SessionID, aid, oid, hdsrc);

//            if (px.IsReady())
//            {
//                var md = SqlAdapter.Create("select top 1 wuid from wxuser with(nolock) where wxoid=@oid")
//                    .Par("@oid", px.CachedUserInfo.openid, SqlDbType.NVarChar, 32)
//                    .ExecuteModel();
//                if (md.IsEmpty)
//                {
//                    return Redirect(redirurl);
//                }
//            }
            
//            if (string.IsNullOrWhiteSpace(hdsrc))
//            {

//                // 微信点立即参加
//                if (!px.IsReady())
//                {
//                    return Redirect(redirurl);
//                }
//                else if (!HasCookie(pkey))
//                {
//                    // 缓存的oid和url oid不相同
//                    return Join(aid);
//                }
//                else
//                {
//                    return MyTurn(aid);
//                }
//            }
//            else
//            {
//                // 帮砍点我也来一发
//                if (!px.IsReady(pkey))
//                {
//                    if (!string.IsNullOrEmpty(Request.QueryString[WxProxy.USERINFO_WX]))
//                    {
//                        return Join(aid);
//                    }
//                    else
//                    {
//                        return Redirect(redirurl);
//                    }
//                }
//                else
//                {
//                    return MyTurn(aid);
//                }
//            }
//        }

//        /// <summary>
//        /// 帮砍页面跳转立即参加页面
//        /// </summary>
//        /// <param name="aid">活动编号</param>
//        /// <param name="foid">朋友oid</param>
//        public ActionResult CutJoin(int aid, string foid)
//        {
//            Log.Session("CutJoin aid={0}, foid={1}", Session.SessionID, aid, foid);
//            var px = WxProxy.Create(this);
//            if (!px.IsReady() || !string.Equals(foid, px.CachedUserInfo.openid))
//            {
//                return Join(aid);
//            }
//            else
//            {
//                return MyTurn(aid);
//            }
//        }

//        [OutputCache(Duration = 600)]
//        public ActionResult Join(int aid)
//        {
//            return View(vprefix + "JoinBargain");
//        }

//        const string pkey = "selfphone";
//        public ActionResult MyTurn(int aid, string phone = null)
//        {
//            if (string.IsNullOrWhiteSpace(phone) && HasCookie(pkey))
//            {
//                phone = GetCookie(pkey);
//            }
//            if (!string.IsNullOrWhiteSpace(phone))
//            {
//                SetCookie(phone, pkey);
//            }
//            WxProxy px = WxProxy.Create(this);
//            if (px.IsReady(pkey))
//            {
//                if (!VerifyUser(aid, phone, px.CachedUserInfo.openid))
//                {
//                    //return View(vprefix + "EndBargain");
//                    return Redirect(redirurl);
//                }
//                ViewBag.wxinfo = px.CachedUserInfo.ToJson();
//                ViewBag.soid = px.CachedUserInfo.openid;
//                ViewBag.simg = px.CachedUserInfo.headimgurl;
//                ViewBag.nick = px.CachedUserInfo.nickname;
//                UTF8Encoding ec = new UTF8Encoding();
//                var json = GetSelfDetail(aid, px.CachedUserInfo.openid).ToJson();
//                var s = ec.GetBytes(json);
//                ViewBag.detail = Convert.ToBase64String(s);
//                var test = ec.GetString(Convert.FromBase64String(ViewBag.detail));
//                return View(vprefix + "ShareBargain");
//            }
//            Log.Info("[*]MyTurn获取用户信息失败");
//            return View(vprefix + "EndBargain");
//        }

//        private static bool VerifyUser(int aid, string phone, string oid)
//        {
//            //if (!string.IsNullOrEmpty(phone))
//            //{
//                var md = SqlAdapter.Create("hdJoinUserNew9", CommandType.StoredProcedure)
//                    .Par("@aid", aid, SqlDbType.Int)
//                    .Par("@oid", oid, SqlDbType.NVarChar, 32)
//                    .Par("@phone", phone, SqlDbType.NVarChar, 20)
//                    .ExecuteModel();
//                if (md.IsEmpty)
//                {
//                    Log.Info("[*]更新手机号失败");
//                    return false;
//                }
//            //}
//            return true;
//        }

//        [OutputCache(Duration=600)]
//        public ActionResult HelpCut(int aid, string foid)
//        {
//            return View(vprefix + "HelpCutBargain");
//        }

//        public ActionResult Play(string aid, string foid)
//        {
//            WxProxy px = WxProxy.Create(this);
//            if (px.IsReady())
//            {
//                return View(vprefix + "PlayBargain");
//            }
//            else
//            {
//                Log.Info("[*]用户信息无法确定");
//                return View(vprefix + "EndBargain");
//            }
//        }

//        public ActionResult End()
//        {
//            return View("EndBargain");
//        }
///// API //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//        public ActionResult Verify(int aid, string soid)
//        {
//            var rlt = new ResultBase();
//            dynamic md = SqlAdapter.Create("bgverify", CommandType.StoredProcedure)
//                .Par("@aid", aid, SqlDbType.Int)
//                .Par("@soid", soid, SqlDbType.NVarChar, 32)
//                .ExecuteModel();
//            if (!md.IsEmpty && md.IsNotNull("code") && md.code == 10)
//            {
//                rlt.Code = 0;
//            }
//            else
//            {
//                rlt.Code = 1;
//            }
//            return Json(rlt);
//        }
//        public ActionResult Preorder(int aid, string soid)
//        {
//            var rlt = new PreOrderResult();
//            var px = WxProxy.Create(this);
//            if (!px.IsReady())
//            {
//                rlt.complete = false;
//                rlt.act = 2;
//            }
//            else
//            {
//                dynamic md = SqlAdapter.Create("bgpreorder", CommandType.StoredProcedure)
//                    .Par("@aid", aid, SqlDbType.Int)
//                    .Par("@soid", soid, SqlDbType.NVarChar, 32)
//                    .ExecuteModel();
//                if (md.IsEmpty)
//                {
//                    rlt.complete = false;
//                    rlt.act = 2;
//                }
//                else
//                {
//                    if (md.hext == 1 && md.hact == 1)
//                    {
//                        if (md.code == 0)
//                        {
//                            var arg = string.Join("&", "phone=" + md.phone, "wxoid=" + px.CachedUserInfo.openid, "aid=" + aid, "nick=" + px.CachedUserInfo.nickname, "avatar=" + px.CachedUserInfo.headimgurl);
//                            var cburl = "http://wx.tuhu.cn/bargain/preorder";// +"?" + arg;
//                            //string wxarg = string.Format("appid=wx21f3300bbe61dc90&redirect_uri={0}&response_type=code&scope={1}&state=fromentry#wechat_redirect", cburl, WxAdapter.SNSAPI_BASE);
//                            //string wxurl = "https://open.weixin.qq.com/connect/oauth2/authorize";
//                            string json = WeChatUtil.HttpSend(cburl, arg, "GET");
//                            ResultBase r = json.FromJson<ResultBase>();
//                            if (r != null && r.IsSuccess)
//                            {
//                                rlt.complete = true;
//                                rlt.act = 0;
//                            }
//                            else if (r.Code == 3)
//                            {
//                                rlt.complete = false;
//                                rlt.already = false;
//                                rlt.act = 2;
//                            }
//                            else
//                            {
//                                rlt.complete = false;
//                                rlt.already = true;
//                                rlt.act = 1;
//                            }
//                        }
//                        else if (md.code == 1)
//                        {
//                            rlt.complete = false;
//                            rlt.already = true;
//                            rlt.act = 1;
//                        }
//                        else
//                        {
//                            // Limit reached
//                            rlt.complete = false;
//                            rlt.act = 2;
//                        }
//                    }
//                    else
//                    {
//                        rlt.complete = false;
//                        rlt.act = 2;
//                    }
//                }
//            }
//            return Json(rlt);
//        }
//        public ActionResult Cut(int aid, string foid, string msg)
//        {
//            var rlt = new CutDetails();
//            WxProxy px = WxProxy.Create(this);
//            if (px.IsReady())
//            {
//                rlt.Code = 0;
//                rlt.info = msg;
//                CalcCutPrice(aid, foid, px.CachedUserInfo.openid, rlt);
//                if (!rlt.already && rlt.uready && !rlt.full)
//                {
//                    if (SqlAdapter.Create("insert into hdbargaincut(soid, foid, aid, pricedown, priceleft, text) values(@soid, @foid, @aid, @pdown, @pleft, @text)")
//                        .Par("@soid", px.CachedUserInfo.openid, SqlDbType.NVarChar, 32)
//                        .Par("@foid", foid, SqlDbType.NVarChar, 32)
//                        .Par("@aid", aid, SqlDbType.Int)
//                        .Par("@pdown", rlt.cprice, SqlDbType.Money)
//                        .Par("@pleft", rlt.rprice, SqlDbType.Money)
//                        .Par("@text", msg, SqlDbType.NVarChar, 128)
//                        .ExecuteNonQuery() < 1)
//                    {
//                        rlt.Error("哎呀，砍偏了！");
//                        rlt.Code = 2;
//                        rlt.cprice = 0;
//                    }
//                }
//                rlt.simg = px.CachedUserInfo.headimgurl;
//                rlt.soid = px.CachedUserInfo.openid;
//                rlt.nick = px.CachedUserInfo.nickname;
//            }
//            else
//            {
//                rlt.Code = 1;
//                rlt.Error("哎呀，偏了！");
//            }
//            return Json(rlt);
//            //return Redirect("http://faxian.tuhu.cn/bargain/helpcut.html?aid={0}&foid={1}&status={2}".Fmt(aid, foid, rlt.ToJson()));
//        }
//        public ActionResult SelfDetail(int aid, string soid)
//        {
//            var rlt = GetSelfDetail(aid, soid);
//            return Json(rlt);
//        }
///// Biz //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//        private string GetCookie(string key)
//        {
//            var c = Request.Cookies[key];
//            if (c != null && !string.IsNullOrWhiteSpace(c.Value))
//            {
//                return c.Value;
//            }
//            return string.Empty;
//        }
//        private bool HasCookie(string key)
//        {
//            HttpCookie c = Request.Cookies[key];
//            if (c != null && !string.IsNullOrWhiteSpace(c.Value))
//            {
//                return true;
//            }
//            return false;
//        }
//        private void SetCookie(string phone, string pkey)
//        {
//            var c = new HttpCookie(pkey, HttpUtility.HtmlEncode(phone));
//            c.Expires = DateTime.Now + TimeSpan.FromDays(300);
//            Response.SetCookie(c);
//        }
//        private static Random rnd = new Random(DateTime.Now.Second + DateTime.Now.Minute * 60);
//        private decimal CalcCutPrice(int aid, string foid, string soid, CutDetails rlt)
//        {
//            try
//            {
//                dynamic md = SqlAdapter.Create("hdcurtprice", CommandType.StoredProcedure)
//                    .Par("@aid", aid, SqlDbType.Int)
//                    .Par("@foid", foid, SqlDbType.NVarChar, 32)
//                    .Par("@soid", soid, SqlDbType.NVarChar, 32)
//                    .ExecuteModel();
//                if (md.IsEmpty)
//                {
//                    return 0;
//                }
//                if (md.already > 0 && !Debugger.IsAttached)
//                {
//                    rlt.already = true;
//                    return 0;
//                }
//                else
//                {
//                    rlt.already = false;
//                }
//                rlt.uready = md.uext > 0;
//                if (!rlt.uready)
//                {
//                    return 0;
//                }
//                if (md.Status < 1 || md.EndTime <= DateTime.Now)
//                {
//                    rlt.isexpired = true;
//                }
//                var num = md.num;
//                var total = md.tprice;
//                var count = md.fcount;
//                var curtprice = md.cprice;
//                var offset = (decimal)(rnd.Next(1, Math.Min(1200, (int)(curtprice - (count - 1 - num)) * 100))) / 100;
//                var delta = offset;
//                if (num >= count - 1)
//                {
//                    delta = curtprice;
//                }
//                rlt.dprice = md.dprice;
//                rlt.cprice = delta;
//                rlt.rprice = curtprice - delta;
//                if (md.num >= md.fcount)
//                {
//                    rlt.full = true;
//                }
//                return delta;
//            }
//            catch (Exception ex)
//            {
//                Log.Error(ex);
//                return 0;
//            }
//        }
//        private static SelfDetails GetSelfDetail(int aid, string soid)
//        {
//            var rlt = new SelfDetails();
//            var mds = SqlAdapter.Create("hdBargainGetSelfDetail", CommandType.StoredProcedure)
//                .Par("@aid", aid, SqlDbType.Int)
//                .Par("@oid", soid, SqlDbType.NVarChar, 32)
//                .ExecuteMds();
//            if (mds.IsEmpty || mds.Count < 1 || mds[0].IsEmpty)
//            {
//                rlt.isexpired = true;
//            }
//            else
//            {
//                dynamic mflag = mds[0];
//                if (mflag.sext == 1 && mflag.uext == 1)
//                {
//                    dynamic msetting = mds[1];
//                    dynamic mself = mds[2];
//                    rlt.simg = mself.GetValue<string>("avatar", "http://resource.tuhu.cn/Image/Product/jinlaohu.png");
//                    rlt.soid = mself.wxoid;
//                    rlt.nick = mself.GetValue<string>("nick", "亲");
//                    rlt.cprice = mself.cprice;
//                    rlt.tprice = msetting.totalprice;
//                    rlt.dprice = msetting.dealmoney;
//                    if (mflag.cext == 1)
//                    {
//                        if (rlt.cprice < msetting.dealmoney)
//                        {
//                            rlt.cprice = msetting.dealmoney;
//                        }
//                        var mcut = mds[3];
//                        rlt.players = new List<Player>();
//                        mcut.Enum((m) =>
//                        {
//                            var avatar = m.GetValue<string>("avatar", "http://resource.tuhu.cn/Image/Product/jinlaohu.png");
//                            var nick = m.GetValue<string>("nick", "亲");
//                            var p = new Player { delta = m.pricedown, img = avatar, nick = nick, oid = m.wxoid, text = HttpUtility.HtmlEncode(m.text) };
//                            rlt.players.Add(p);
//                        });
//                    }
//                    else
//                    {
//                        rlt.players = null;
//                    }
//                }
//                else
//                {
//                    rlt.isexpired = true;
//                }
//            }
//            return rlt;
//        }
//        #region Methods
//        public ActionResult Json(object o)
//        {
//            if (o != null)
//            {
//                return Content(o.ToJson());
//            }
//            return Content("{}");
//        }
//        public ActionResult Oid()
//        {
//            var wa = WxAdapter.Create(this);
//            if (!wa.IsBasicReady)
//            {
//                return wa.Redirect("http://faxian.tuhu.cn/bargain/oid.html");
//            }
//            return Content("Oid => " + wa.OpenId);
//        }
//        public ActionResult v(string p)
//        {
//            return View(p);
//        }
//        #endregion
//    }                    
//}