using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.WeiXinCard;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class WeixinCardController : Controller
    {
        [HttpGet]
        public ActionResult Supplier()
        {
            SupplierInfo model = new SupplierInfo();

            List<SupplierInfo> supplierlist = WeiXinCardManager.GetSupplierInfo(-1);
            supplierlist = supplierlist.OrderBy(q => q.pkid).ToList();
            return View(Tuple.Create(supplierlist, model));
        }


        [HttpPost]
        public ActionResult Supplier([Bind(Prefix = "Item2")]SupplierInfo model)
        {
            string resultstr = string.Empty;
            int result = WeiXinCardManager.SaveWeiXinCardSupplier(model);
            if (result > 0)
            {
                resultstr = "保存成功";
            }
            return Json(resultstr);
        }

        //[HttpPost]
        //public ActionResult UpdateSupplier(SupplierInfo model)
        //{
        //    string resultstr = string.Empty;
        //    int result = WeiXinCardManager.SaveWeiXinCardSupplier(model);
        //    if (result > 0)
        //    {
        //        resultstr = "保存成功";
        //    }
        //    return Json(resultstr);
        //}

        public ActionResult UpdateSupplier(int pkid)
        {
            var model = WeiXinCardManager.GetSupplierInfo(pkid);
            WebClient wc = new WebClient();
            byte[] imagebytes = wc.DownloadData(model.First().logo_url);
            model.First().Imgbase64string = Convert.ToBase64String(imagebytes);

            return View(model.First());
        }

        [HttpPost]
        public ActionResult UpdateSupplier(SupplierInfo model)
        {
            int i = WeiXinCardManager.UpdateWeiXinCardSupplier(model);

            return Json(i);
            //if (i > 0)
            //{
            //    return Json("更新成功");
            //}
            //else
            //{
            //    return Json("更新失败");
            //}
        }


        [HttpPost]
        public ActionResult DeleteSupplier(int pkid)
        {
            bool flag = false;
            try
            {
                WeiXinCardManager.DeleteWeiXinCardSupplier(pkid);
                flag = true;
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteWeiXinCard(int pkid)
        {
            bool flag = false;
            try
            {
                WeiXinCardManager.DeleteWeiXinCard(pkid);
                flag = true;
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        // GET: WeixinCard
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]       
        public ActionResult CreateCard(int pkId=0, int supplierId = 1)
        {
            WeixinCardModel cardmodelInfo = new WeixinCardModel();
           
          
            WeixinCardTotalModel totalModel = new WeixinCardTotalModel();
            if (pkId == 0)
            {
                SupplierInfo supplier = new SupplierInfo();
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
                TimeSpan ts1 = DateTime.Now.AddDays(30) - new DateTime(1970, 1, 1);
                List<SupplierInfo> supplierInfoList = WeiXinCardManager.GetSupplierInfo(supplierId);
                if (supplierInfoList != null && supplierInfoList.FirstOrDefault() != null)
                {
                    supplier = supplierInfoList.First();
                }
                 totalModel = new WeixinCardTotalModel()
                {
                    base_info = new WeixinCardBaseInfo()
                    {
                        logo_url = supplier.logo_url,
                        brand_name = supplier.brand_name,



                        colorid = 2,
                       
                        date_info = new DateInfo()
                        {
                            begin_time = DateTime.Now,
                            end_time = DateTime.Now.AddDays(10),
                            begin_timestamp = (UInt64)ts.Seconds,
                            end_timestamp = (UInt64)ts.Seconds,
                            // (DateTime.Now.AddDays(30) - new DateTime(1970, 1, 1)
                            type = "DATE_TYPE_FIX_TERM"
                            // end_timestamp =Convert.ToUInt32(ts.TotalSeconds),
                        },
                        sku = new SKUQuantity()
                        {
                            quantity = 0
                        },
                     //   center_title = "途虎养车网",
                      //  center_url = "https://www.tuhu.cn",
                        service_phone = "4001118868"
                      
                    },
                    advanced_info = new WeixinCardAdvancedInfo()
                    {
                        abstractinfo = new AbstractInfo(),
                        use_condition = new ConditionalUse()
                    }

                };
                cardmodelInfo.total_info = totalModel;
            }
            else
            {
                WeixinCardModel model = null;
                var list = WeiXinCardManager.GetWeixinCardModelList(pkId);
                if (list != null && list.FirstOrDefault() != null)
                {
                    model = list.First();
                }

                cardmodelInfo = model;
            }
           
            return View(cardmodelInfo);
        }


        public static string HttpUploadFile(string url, string file, string paramName, string contentType,byte[]buffers, NameValueCollection nvc=null)
        {
            string result = string.Empty;
            // log.Debug(string.Format("Uploading {0} to {1}", file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            if (nvc != null)
            {
                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            //FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

            //byte[] buffer = new byte[5000000];
            //int bytesRead = 0;
            //while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            //{
            //    rs.Write(buffer, 0, bytesRead);
            //}
            //fileStream.Close();

            rs.Write(buffers, 0, buffers.Length);
            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                result = reader2.ReadToEnd();
                // log.Debug(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
            }
            catch (Exception ex)
            {
                //  log.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            return result;
        }

        public ActionResult UploadImage(string imagePath)
        {
            string strimg = string.Empty;
            bool flag = false;
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    var buffers = new byte[file.ContentLength];
                    file.InputStream.Read(buffers, 0, file.ContentLength);
                  
                    AccessTokenModel model = GetAccess_tokenAsCache();
                    string url1 = "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + model.Access_token;
                    var str=HttpUploadFile(url1, file.FileName, "buffer", "image/jpeg",buffers, null);
                    JObject imgobj = JsonConvert.DeserializeObject(str) as JObject;
                    if (imgobj != null && imgobj["url"] != null && !string.IsNullOrWhiteSpace(imgobj["url"].ToString()))
                    {
                        flag = true;
                        strimg = imgobj["url"].ToString();
                    }


                    #region TempCode
                    /*

                    //string imgPath = "https://img4.tuhu.org/Home/Image/" + imagePath;
                    string imgPath = @"D:\" + imagePath;
                 WebClient client = new WebClient();
                 AccessTokenModel model = GetAccess_tokenAsCache();
                 string url = "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + model.Access_token;
                byte[] bytes= client.UploadFile(url, imagePath);
                string str1 = Encoding.UTF8.GetString(bytes);
                JObject imgobj = JsonConvert.DeserializeObject(str1) as JObject;
                if (imgobj != null && imgobj["url"] != null && !string.IsNullOrWhiteSpace(imgobj["url"].ToString()))
                {
                    strimg = imgobj["url"].ToString();
                }
                /*
                string strimg = string.Empty;
                if (Request.Files.Count > 0 && Request.Files[0].ContentLength < 100 * 1024)
                {
                    var file = Request.Files[0];
                    var buffers = new byte[file.ContentLength];
                    file.InputStream.Read(buffers, 0, file.ContentLength);
                    WebClient client = new WebClient();
                    AccessTokenModel model = GetAccess_tokenAsCache();
                    string url1 = "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + model.Access_token;
                    byte[] bytes = client.UploadData(url1, buffers);
                    string str1 = Encoding.UTF8.GetString(bytes);


                    ImageBuffer obj = new ImageBuffer()
                    {

                        buffer = buffers
                    };
                    string objstr = JsonConvert.SerializeObject(obj);
                    string str = GetWeixinCardResponse(url1, model.Access_token, objstr);

                    JObject imgobj = JsonConvert.DeserializeObject(str) as JObject;
                    if (imgobj != null && imgobj["url"] != null && !string.IsNullOrWhiteSpace(imgobj["url"].ToString()))
                    {
                        strimg = imgobj["url"].ToString();
                    }

                }
                */
                    #endregion
                }
            }
            catch (Exception ex)
            {
                flag = false;
                strimg = ex.Message;
                WebLog.LogException(ex);
            }
            return Json(new {flag=flag,result=strimg}, JsonRequestBehavior.AllowGet);
        }

        public static string GetJsonStr(WeixinCardModel model)
        {
            // model.total_info.base_info.date_info.type = 0;
            model.total_info.base_info.sku = new SKUQuantity() { quantity = 0 };
            model.total_info.base_info.use_custom_code = true;
            model.total_info.base_info.bind_openid = false;
            model.total_info.base_info.center_app_brand_user_name = "gh_513038890d99@app";
            model.total_info.base_info.custom_app_brand_user_name = "gh_513038890d99@app";
            model.total_info.base_info.promotion_app_brand_user_name = "gh_513038890d99@app";
           // model.total_info.base_info.center_url = "https://wx.tuhu.cn/";
           // model.total_info.base_info.custom_url = "https://wx.tuhu.cn/";
           // model.total_info.base_info.promotion_url = "https://wx.tuhu.cn/";
            model.total_info.base_info.get_custom_code_mode = "GET_CUSTOM_CODE_MODE_DEPOSIT";


            model.total_info.base_info.color = AccessTokenModel.GetOutColors(model.total_info.base_info.colorid).Where(q => q.Value == model.total_info.base_info.colorid.Value.ToString()).First().Text;
            // model.total_info.base_info.logo_url = "http://mmbiz.qpic.cn/mmbiz/iaL1LJM1mF9aRKPZJkmG8xXhiaHqkKSVMMWeN3hLut7X7hicFNjakmxibMLGWpXrEXB33367o7zHN0CwngnQY7zb7g/0";
            WeixinCardInfo weixincard = new WeixinCardInfo()
            {
                card = model
            };
            var advancedInfo = model.total_info.advanced_info;
            var abstractInfo = advancedInfo.abstractinfo;
            model.total_info.advanced_info.abstractinfo.icon_url_list = new List<string>();
            model.total_info.advanced_info.text_image_list = new List<ImageText>();
            List<string> iconlist = new List<string> { abstractInfo.icon1, abstractInfo.icon2, abstractInfo.icon3, abstractInfo.icon4, abstractInfo.icon5 };
            iconlist = iconlist.Where(q => !string.IsNullOrWhiteSpace(q)).ToList();

            model.total_info.advanced_info.abstractinfo.icon_url_list.AddRange(iconlist);
            List<ImageText> imageTextList = new List<ImageText> { new ImageText() {
                        image_url=abstractInfo.imageText1.image_url,
                        text=abstractInfo.imageText1.text
                    },
                    new ImageText() {
                        image_url=abstractInfo.imageText2.image_url,
                        text=abstractInfo.imageText2.text
                    },
                    new ImageText() {
                        image_url=abstractInfo.imageText3.image_url,
                        text=abstractInfo.imageText3.text
                    },
                    new ImageText() {
                        image_url=abstractInfo.imageText4.image_url,
                        text=abstractInfo.imageText4.text
                    },
                    new ImageText() {
                        image_url=abstractInfo.imageText5.image_url,
                        text=abstractInfo.imageText5.text
                    } };

            imageTextList = imageTextList.Where(q => (!string.IsNullOrWhiteSpace(q.image_url) && !string.IsNullOrWhiteSpace(q.text))).ToList();

            model.total_info.advanced_info.text_image_list.AddRange(imageTextList);

            string requestmodelstr = JsonConvert.SerializeObject(weixincard, Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                        //DefaultValueHandling = DefaultValueHandling.Ignore,
                                        NullValueHandling = NullValueHandling.Ignore
                            });
            var jsonrequststr = requestmodelstr.Replace("total_info", model.card_type.ToString().ToLower());
            return jsonrequststr;
        }
        [HttpPost]
        public ActionResult CreateCard(WeixinCardModel model, string command)
        {
            string resultstr = string.Empty;
            try
            {
                if (Request.QueryString["supplierId"] != null)
                {
                    List<SupplierInfo> supplierInfoList = WeiXinCardManager.GetSupplierInfo(Convert.ToInt32(Request.QueryString["supplierId"]));
                    if (supplierInfoList != null && supplierInfoList.FirstOrDefault() != null)
                    {
                        var supplier = supplierInfoList.First();
                        model.total_info.base_info.brand_name = supplier.brand_name;
                        model.total_info.base_info.logo_url = supplier.logo_url;
                        model.total_info.base_info.supplierId = supplier.pkid;
                    }

                }

                
             
                    TimeSpan startts = model.total_info.base_info.date_info.begin_time - new DateTime(1970, 1, 1);
                    TimeSpan endts = model.total_info.base_info.date_info.end_time - new DateTime(1970, 1, 1);
                    model.total_info.base_info.date_info.begin_timestamp = Convert.ToUInt64(startts.TotalSeconds);

                   
                        model.total_info.base_info.date_info.end_timestamp = Convert.ToUInt64(endts.TotalSeconds);
                   


                if (command.Equals("Save/Update"))
                {                 
                    int result = 0;
                    string requestmodelstr = JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                                //DefaultValueHandling = DefaultValueHandling.Ignore,
                                NullValueHandling = NullValueHandling.Ignore
                           });
                    var log = requestmodelstr.Replace("{", "").Replace("}", "");
                    WebLog.LogInfo(log);
                    if (string.IsNullOrWhiteSpace(model.card_id))
                    {
                         result = WeiXinCardManager.SaveWeiXinCard(model);
                    }
                    else
                    {
                         result = WeiXinCardManager.UpdateWeiXinCard(model);
                    }
                    if (result > 0)
                    {
                        resultstr = "操作成功";
                    }
                }
                else if (command.Equals("Submit"))
                {
                    string jsonrequststr = GetJsonStr(model);                  
                    var loginfo = jsonrequststr.Replace("{", "").Replace("}", "");
                      WebLog.LogInfo(loginfo);
                    AccessTokenModel tokenobj = GetAccess_tokenAsCache();
                    string token = string.Empty;
                    if (tokenobj != null && !string.IsNullOrWhiteSpace(tokenobj.Access_token))
                    {
                        token = tokenobj.Access_token;
                    }


                    var result = GetWeixinCardResponse("https://api.weixin.qq.com/card/create", token, jsonrequststr);
                    JObject weixincardobj = JsonConvert.DeserializeObject(result) as JObject;
                    if (weixincardobj != null && weixincardobj["errcode"] != null)
                    //&&Convert.ToInt32(weixincardobj["errcode"])==0 && weixincardobj["card_id"] != null)
                    {
                        if (Convert.ToInt32(weixincardobj["errcode"]) == 0 && weixincardobj["card_id"] != null)
                        {
                            model.card_id = weixincardobj["card_id"].ToString();
                            WeiXinCardManager.SaveWeiXinCard(model);
                            resultstr = "提交成功";
                        }
                        else if (weixincardobj["errmsg"] != null)
                        {
                            resultstr = "提交失败,失败原因:" + weixincardobj["errmsg"].ToString();
                        }
                    }
                }
                else if (command.Equals("Switch to CardList"))
                {
                    return RedirectToAction("CardList");                   
                }
            }
            catch (Exception ex)
            {
                resultstr = ex.Message;
            }
            return Json(resultstr, JsonRequestBehavior.AllowGet);
        }

        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 按字符读取并写入字符串缓冲
                int ch = -1;
                while ((ch = reader.Read()) > -1)
                {
                    // 过滤结束符
                    char c = (char)ch;
                    if (c != '\0')
                    {
                        result.Append(c);
                    }
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }


        private static AccessTokenModel GetWxAccess_token()
        {
            using (var client = CacheHelper.CreateCacheClient("WeixinCard"))
            {
                var cacheResult = client.GetOrSet("Access_Token", GetAccess_tokenAsCache, TimeSpan.FromHours(2));
                return cacheResult.Success ? cacheResult.Value : GetAccess_tokenAsCache();
            }
        }


        private static AccessTokenModel GetAccess_tokenAsCache()
        {


            var req = (HttpWebRequest)WebRequest.Create("https://wx.tuhu.cn/packet/test");
            req.Headers.Set("access_token", "access_token");
            req.Method = "GET";
            try
            {
                AccessTokenModel model;
                using (WebResponse wr = req.GetResponse())
                {
                    var reader = new StreamReader(wr.GetResponseStream(), Encoding.UTF8);
                    var content = reader.ReadToEnd();
                    model = JsonConvert.DeserializeObject<AccessTokenModel>(content);
                }
                return model;
            }
            catch (Exception e)
            {
                return new AccessTokenModel
                {
                    Errcode = "-1",
                    Errmsg = "调用第三方微信获取token接口失败"
                };
            }
        }

        public ActionResult CreateCode(string cardid, int count = 100)
        {
            bool issuccess = true;
            string resultstr = "生成券码成功";
            int trycount = count / 100;

            for (int i = 1; i <= trycount; i++)
            {
                if (!CreateCodeBatch(cardid, 100, ref resultstr))
                {
                    issuccess = false;
                    break;
                }
            }
            if (issuccess)
            {
                int remainingcount = count - trycount * 100;
                if (remainingcount > 0)
                {
                    issuccess = CreateCodeBatch(cardid, remainingcount, ref resultstr);
                }
            }

            if (issuccess)
            {
                int result=WeiXinCardManager.UpdateWeiXinCardPushCount(cardid, count);
                if (result > 0)
                {
                    resultstr = "生成券码成功";
                }
            }
            return Json(resultstr, JsonRequestBehavior.AllowGet);
        }


        public bool CreateCodeBatch(string cardid, int count, ref string errormsg)
        {
            
            bool issuccessfully = false;
           // errormsg = string.Empty;
            try
            {

                int result = WeiXinCardManager.InsertPromotionCodeToWeixinCard(cardid, count);
                int pkid = WeiXinCardManager.GetPKIDByWeiXinCard(cardid);


                if (result > 0)
                {
                    List<string> cardcodes = WeiXinCardManager.GetWeixinCardCode(cardid, pkid,count);
                    WeixinCardCode obj = new WeixinCardCode()
                    {
                        card_id = cardid,
                        code = cardcodes
                    };
                    var jsonrequststr = JsonConvert.SerializeObject(obj);
                    AccessTokenModel tokenobj = GetAccess_tokenAsCache();
                    string token = string.Empty;
                    if (tokenobj != null && !string.IsNullOrWhiteSpace(tokenobj.Access_token))
                    {
                        token = tokenobj.Access_token;
                    }


                    var resultc = GetWeixinCardResponse("http://api.weixin.qq.com/card/code/deposit", token, jsonrequststr);
                    JObject resultobj = JsonConvert.DeserializeObject(resultc) as JObject;
                    if (resultobj != null && resultobj["errcode"] != null)
                    {
                        if (Convert.ToInt32(resultobj["errcode"]) == 0)
                        {
                          //  resultstr = "生成券码成功";
                            WeixinCardCodeQuantity quantityobj = new WeixinCardCodeQuantity()
                            {
                                card_id = cardid,
                                increase_stock_value = count
                            };
                            var jsonquantityrequststr = JsonConvert.SerializeObject(quantityobj);
                            var quantityresult = GetWeixinCardResponse("https://api.weixin.qq.com/card/modifystock", token, jsonquantityrequststr);
                            JObject quantityresultobj = JsonConvert.DeserializeObject(quantityresult) as JObject;
                            if (quantityresultobj != null && quantityresultobj["errcode"] != null)
                            {
                                if (Convert.ToInt32(quantityresultobj["errcode"]) == 0)
                                {
                                    issuccessfully = true;
                                   
                                }
                                else
                                {
                                    errormsg = quantityresultobj["errcode"].ToString();
                                }
                            }

                        }
                        else
                        {
                            errormsg = resultobj["errcode"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }
            return issuccessfully;
        }

        [HttpPost]
        public ActionResult Submit(WeixinCardModel model)
        {
            //model.total_info.base_info.color = "Color010";
            model.card_type = CardTypeEnum.GROUPON;

            string requestmodelstr = JsonConvert.SerializeObject(model);
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("https://api.weixin.qq.com/card/create?access_token=ACCESS_TOKEN");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = requestmodelstr.Length;

            var streamWriter = new StreamWriter(httpRequest.GetRequestStream());
            streamWriter.Write(requestmodelstr);
            streamWriter.Close();

            var response = httpRequest.GetResponse();
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public string GetWeixinCardResponse(string url, string token, string requestbody)
        {
            string charset = "utf-8";
            string uri = string.Format("{0}?access_token={1}", url, token);
            HttpWebRequest req = GetWebRequest(uri, "POST");
            req.ContentType = "application/x-www-form-urlencoded;charset=" + charset;

            byte[] postData = Encoding.GetEncoding(charset).GetBytes(requestbody);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = Encoding.UTF8;
            return GetResponseAsString(rsp, encoding);
        }

        public string GetWeixinCardImageResponse(string url, string token, byte[] postData)
        {

            string uri = string.Format("{0}?access_token={1}", url, token);
            HttpWebRequest req = GetWebRequest(uri, "POST");
            req.ContentType = "image/jpeg";


            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = Encoding.UTF8;
            return GetResponseAsString(rsp, encoding);
        }

        public static string BuildQuery(IDictionary<string, string> parameters, string charset)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");

                    // string encodedValue = HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset));
                    postData.Append(value);
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        public static HttpWebRequest GetWebRequest(string url, string method)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.KeepAlive = true;
            //req.UserAgent = "Aop4Net";
            //req.Timeout = this._timeout;
            return req;
        }

        [HttpPost]
        public ActionResult Save(WeixinCardModel model)
        {
            return View(model);
        }


        public ActionResult CardList()
        {
            List<WeixinCardModel> list = WeiXinCardManager.GetWeixinCardModelList();
            list=list.OrderBy(q => q.PKID).ToList();
            return View(list);
        }


        public ActionResult UpdateCard(int pkid,int supplierid)
        {
            //WeixinCardModel model = null;
            //var list = WeiXinCardManager.GetWeixinCardModelList(pkid);
            //if (list != null && list.FirstOrDefault() != null)
            //{
            //    model = list.First();
            //}
           return RedirectToAction("CreateCard", new { pkId = pkid, supplierId = supplierid });
       //     return View("../WeixinCard/CreateCard", model);


        }
      
    }


}