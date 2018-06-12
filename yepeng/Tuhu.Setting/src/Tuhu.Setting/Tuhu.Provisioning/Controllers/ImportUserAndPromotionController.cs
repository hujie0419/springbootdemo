using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;
using Tuhu.Provisioning.Business.YLHUser;
using Tuhu.Provisioning.DataAccess;

namespace Tuhu.Provisioning.Controllers
{
    public class ImportUserAndPromotionController : Controller
    {
        // GET: ImportUserAndPromotion
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ImportGrade()
        {
            try
            {
                //初始化优惠项目
                var promotionItems = PromotionItemInitialization();

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件！" }, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("sheet1", true);

                    #region 初始化失败数据表结构
                    DataTable failData = new DataTable();
                    failData.Columns.Add(new DataColumn("会员卡号", typeof(string)));
                    failData.Columns.Add(new DataColumn("会员姓名", typeof(string)));
                    failData.Columns.Add(new DataColumn("会员手机", typeof(string)));
                    failData.Columns.Add(new DataColumn("数量", typeof(string)));
                    failData.Columns.Add(new DataColumn("服务项目", typeof(string)));
                    #endregion

                    #region 插入成功数据表结构
                    DataTable finishData = new DataTable();
                    finishData.Columns.Add(new DataColumn("会员卡号", typeof(string)));
                    finishData.Columns.Add(new DataColumn("会员姓名", typeof(string)));
                    finishData.Columns.Add(new DataColumn("会员手机", typeof(string)));
                    finishData.Columns.Add(new DataColumn("数量", typeof(string)));
                    finishData.Columns.Add(new DataColumn("服务项目", typeof(string)));
                    #endregion

                    Dictionary<string, Guid> insertUsers = new Dictionary<string, Guid>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow failRow = failData.NewRow();
                        DataRow finishRow = finishData.NewRow();
                        bool sendPromotionCodeFail = false;

                        #region step 1 清理出表格中手机号字段为空的数据
                        if (null == dr["会员手机"] || string.IsNullOrWhiteSpace(dr["会员手机"].ToString().Trim()))
                        {
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                failRow[i] = dr[i];
                            }
                            failData.Rows.Add(failData);
                            continue;
                        }
                        #endregion

                        #region step 1: 获取UserID或者添加用户到UserObject
                        var tempUserId = YLHUserManager.GetUserId(dr["会员手机"].ToString().Trim());  //获取是否已经是tuhu用户
                        var dtExistedMobile = insertUsers.ContainsKey(dr["会员手机"].ToString().Trim()); //是否是已经插入的用户

                        //主表不存在手机用户
                        //插入主表用户数据、YLH表用户数据
                        if (string.IsNullOrEmpty(tempUserId) && dtExistedMobile == false)
                        {
                            tbl_UserObjectModel userObject = new tbl_UserObjectModel();
                            Guid newGuid = Guid.NewGuid();

                            #region generate userObjectModel
                            //u_user_id
                            userObject.u_user_id = newGuid.ToString("B");
                            //会员手机号
                            userObject.u_mobile_number = dr["会员手机"].ToString().Trim();
                            //会员姓名
                            userObject.u_last_name =
                                (null == dr["会员姓名"] || string.IsNullOrWhiteSpace(dr["会员姓名"].ToString().Trim()) || "NULL" == dr["会员姓名"].ToString().Trim().ToUpper())
                                ? dr["会员手机"].ToString().Trim() : dr["会员姓名"].ToString().Trim();
                            //会员邮箱
                            userObject.u_email_address = dr["会员手机"].ToString().Trim() + "@whguanggu.com";
                            userObject.u_application_name = "Import";
                            userObject.Category = "武汉光谷一路店";
                            #endregion
                            #region 插入到数据库，如果插入失败，加入脏数据表，否则加入正常数据表
                            try
                            {
                                YLHUserManager.InsertUserObject(userObject);
                            }
                            catch (Exception ex)
                            {
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                {
                                    failRow[i] = dr[i];
                                }
                                failData.Rows.Add(failRow);
                                WebLog.LogException(ex);
                                break;
                            }
                            #endregion

                            YLHUserInfoModel ylhUserInfo = new YLHUserInfoModel();

                            #region generate ylh_UserInfo
                            ylhUserInfo.u_user_id = newGuid.ToString("B");
                            ylhUserInfo.MemberName = (null == dr["会员姓名"] || string.IsNullOrWhiteSpace(dr["会员姓名"].ToString().Trim()) || "NULL" == dr["会员姓名"].ToString().Trim().ToUpper())
                                ? dr["会员手机"].ToString().Trim() : dr["会员姓名"].ToString().Trim();
                            ylhUserInfo.MemberNumber = dr["会员卡号"].ToString().Trim();
                            ylhUserInfo.MemberPhone = dr["会员手机"].ToString().Trim();
                            ylhUserInfo.Tag = "武汉光谷一路店";
                            ylhUserInfo.CreatedTime = DateTime.Now;
                            ylhUserInfo.UpdatedTime = DateTime.Now;
                            ylhUserInfo.MemberBirthday = DateTime.Now;
                            ylhUserInfo.MemberAddress = string.Empty;
                            ylhUserInfo.Integration = 0;
                            #endregion
                            #region 插入到数据库，如果插入失败，加入脏数据表，否则加入正常数据表
                            try
                            {
                                YLHUserManager.InsertYLHUserInfo(ylhUserInfo);
                            }
                            catch (Exception ex)
                            {
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                {
                                    failRow[i] = dr[i];
                                }
                                failData.Rows.Add(failRow);
                                WebLog.LogException(ex);
                                break;
                            }
                            #endregion

                            //插入的用户记录字典
                            if (!insertUsers.ContainsKey(dr["会员手机"].ToString().Trim()))
                                insertUsers.Add(dr["会员手机"].ToString().Trim(), newGuid);
                        }
                        //主表存在手机用户数据
                        //插入YLH表用户数据
                        else
                        {
                            var tempPKID = YLHUserManager.GetYLHUserInfoPKID(tempUserId);
                            if (tempPKID < 0)
                            {
                                YLHUserInfoModel ylhUserInfo = new YLHUserInfoModel();

                                #region generate ylh_UserInfo
                                ylhUserInfo.u_user_id = tempUserId;
                                ylhUserInfo.MemberName = (null == dr["会员姓名"] || string.IsNullOrWhiteSpace(dr["会员姓名"].ToString().Trim()) || "NULL" == dr["会员姓名"].ToString().Trim().ToUpper())
                                    ? dr["会员手机"].ToString().Trim() : dr["会员姓名"].ToString().Trim();
                                ylhUserInfo.MemberNumber = dr["会员卡号"].ToString().Trim();
                                ylhUserInfo.MemberPhone = dr["会员手机"].ToString().Trim();
                                ylhUserInfo.Tag = "武汉光谷一路店";
                                ylhUserInfo.CreatedTime = DateTime.Now;
                                ylhUserInfo.UpdatedTime = DateTime.Now;
                                ylhUserInfo.MemberBirthday = DateTime.Now;
                                ylhUserInfo.MemberAddress = string.Empty;
                                ylhUserInfo.Integration = 0;
                                #endregion
                                #region 插入到数据库，如果插入失败，加入脏数据表，否则加入正常数据表
                                try
                                {
                                    YLHUserManager.InsertYLHUserInfo(ylhUserInfo);
                                }
                                catch (Exception ex)
                                {
                                    for (int i = 0; i < dr.ItemArray.Length; i++)
                                    {
                                        failRow[i] = dr[i];
                                    }
                                    failData.Rows.Add(failRow);
                                    WebLog.LogException(ex);
                                    break;
                                }
                                #endregion
                            }

                            //插入的用户记录字典
                            if (!insertUsers.ContainsKey(dr["会员手机"].ToString().Trim()))
                                insertUsers.Add(dr["会员手机"].ToString().Trim(), new Guid(tempUserId));
                        }
                        #endregion

                        #region step 2: 插入优惠券
                        if (!string.IsNullOrWhiteSpace(dr["服务项目"].ToString().Trim()) && insertUsers.ContainsKey(dr["会员手机"].ToString().Trim()))
                        {
                            var promotions = promotionItems.Where(_item => _item.ProductName == dr["服务项目"].ToString().Trim()).ToList();
                            if (promotions != null && promotions.Any() && promotions.Count > 0)
                            {
                                foreach (var promotion in promotions)
                                {
                                    int count = Convert.ToInt32(dr["数量"].ToString().Trim());
                                    for (int i = 0; i < count; i++)
                                    {
                                        #region generation promotionCode
                                        PromotionCode code = new PromotionCode();
                                        code.UserID = insertUsers[dr["会员手机"].ToString().Trim()];
                                        code.StartTime = DateTime.Now;
                                        code.EndTime = Convert.ToDateTime("2018-12-31 00:00:00");
                                        code.PromotionName = promotion.PromotionName;
                                        code.Description = promotion.Description;
                                        code.RuleID = promotion.RuleID;
                                        code.Discount = promotion.Discount;
                                        code.MinMoney = promotion.MinMoney;
                                        code.CodeChannel = promotion.CodeChannel;
                                        code.Creater = "zhangchen3@tuhu.cn";
                                        code.Issuer = "zhangchen3@tuhu.cn";
                                        #endregion

                                        #region 塞券
                                        try
                                        {
                                            YLHUserManager.CreatePromotionCode(code);
                                        }
                                        catch (Exception ex)
                                        {
                                            sendPromotionCodeFail = true;
                                            WebLog.LogException(ex);
                                            break;
                                        }
                                        #endregion
                                    }

                                    if (sendPromotionCodeFail) break;
                                }
                            }

                            if (sendPromotionCodeFail)
                            {
                                for (int j = 0; j < dr.ItemArray.Length; j++)
                                {
                                    failRow[j] = dr[j];
                                }
                                failData.Rows.Add(failRow);
                            }
                            else
                            {
                                for (int j = 0; j < dr.ItemArray.Length; j++)
                                {
                                    finishRow[j] = dr[j];
                                }
                                finishData.Rows.Add(finishRow);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < dr.ItemArray.Length; j++)
                            {
                                finishRow[j] = dr[j];
                            }
                            finishData.Rows.Add(finishRow);
                            break;
                        }
                        #endregion
                    }

                    Response.Clear();
                    Response.Charset = "UTF-8";
                    Response.Buffer = true;
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode("永隆行用户导入失败记录", System.Text.Encoding.UTF8) + ".xls\"");
                    Response.ContentType = "application/ms-excel";
                    string colHeaders = string.Empty;
                    string ls_item = string.Empty;
                    DataRow[] myRow = failData.Select();
                    int cl = failData.Columns.Count;
                    foreach (DataRow row in myRow)
                    {
                        for (int i = 0; i < cl; i++)
                        {
                            if (i == (cl - 1))
                            {
                                ls_item += row[i].ToString() + "\n";
                            }
                            else
                            {
                                ls_item += row[i].ToString() + "\t";
                            }
                        }
                        Response.Output.Write(ls_item);
                        ls_item = string.Empty;
                    }
                    Response.Output.Flush();
                    Response.End();

                    return Json(new { Status = -1, Error = "全部写入完成" }, "text/html");
                }
                return Json(new { Status = -1, Error = "请选择文件" }, "text/html");
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return Json(new { Status = -2, Error = ex }, "text/html");
            }
        }

        public List<PromotionItem> PromotionItemInitialization()
        {
            var result = new List<PromotionItem>();

            #region 配置数据
            #region 12月-免费1平方油漆
            result.Add(new PromotionItem
            {
                ProductName = "12月-免费1平方油漆",
                RuleID = 123575,
                PromotionName = "12月-免费1平方油漆（服务）",
                Description = "12月-免费1平方油漆（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 12月-免费更换防冻液
            result.Add(new PromotionItem
            {
                ProductName = "12月-免费更换防冻液",
                RuleID = 123576,
                PromotionName = "12月-免费更换防冻液",
                Description = "12月-免费更换防冻液",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });

            result.Add(new PromotionItem
            {
                ProductName = "12月-免费更换防冻液",
                RuleID = 123577,
                PromotionName = "12月-免费更换防冻液（服务）",
                Description = "12月-免费更换防冻液（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 12月-免费空气格
            result.Add(new PromotionItem
            {
                ProductName = "12月-免费空气格",
                RuleID = 123620,
                PromotionName = "12月-免费空气格",
                Description = "12月-免费空气格",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });

            result.Add(new PromotionItem
            {
                ProductName = "12月-免费空气格",
                RuleID = 123621,
                PromotionName = "12月-免费空气格（服务）",
                Description = "12月-免费空气格（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 12月-室内臭氧杀菌
            result.Add(new PromotionItem
            {
                ProductName = "12月-室内臭氧杀菌",
                RuleID = 123622,
                PromotionName = "12月-室内臭氧杀菌（服务）",
                Description = "12月-室内臭氧杀菌（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 12月-四轮交叉换位
            result.Add(new PromotionItem
            {
                ProductName = "12月-四轮交叉换位",
                RuleID = 123623,
                PromotionName = "12月-四轮交叉换位（服务）",
                Description = "12月-四轮交叉换位（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 12月-油漆抵用券200
            result.Add(new PromotionItem
            {
                ProductName = "12月-油漆抵用券200",
                RuleID = 123672,
                PromotionName = "12月-油漆抵用券200",
                Description = "12月-油漆抵用券200",
                Discount = 200,
                MinMoney = 200,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 1月-发动机舱清洗
            result.Add(new PromotionItem
            {
                ProductName = "1月-发动机舱清洗",
                RuleID = 123624,
                PromotionName = "1月-发动机舱清洗（服务）",
                Description = "1月-发动机舱清洗（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 1月-防冻玻璃水
            result.Add(new PromotionItem
            {
                ProductName = "1月-防冻玻璃水",
                RuleID = 123625,
                PromotionName = "1月-防冻玻璃水",
                Description = "1月-防冻玻璃水",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 1月-防冻玻璃水1瓶
            result.Add(new PromotionItem
            {
                ProductName = "1月-防冻玻璃水1瓶",
                RuleID = 123626,
                PromotionName = "1月-防冻玻璃水1瓶",
                Description = "1月-防冻玻璃水1瓶",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 1月-免费室内美容
            result.Add(new PromotionItem
            {
                ProductName = "1月-免费室内美容",
                RuleID = 123627,
                PromotionName = "1月-免费室内美容（服务）",
                Description = "1月-免费室内美容（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 1月-全车去污打蜡
            result.Add(new PromotionItem
            {
                ProductName = "1月-全车去污打蜡",
                RuleID = 123628,
                PromotionName = "1月-全车去污打蜡（服务）",
                Description = "1月-全车去污打蜡（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 1月-四轮钢圈清洗
            result.Add(new PromotionItem
            {
                ProductName = "1月-四轮钢圈清洗",
                RuleID = 123629,
                PromotionName = "1月-四轮钢圈清洗（服务）",
                Description = "1月-四轮钢圈清洗（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region SUV洗车
            result.Add(new PromotionItem
            {
                ProductName = "SUV洗车",
                RuleID = 123630,
                PromotionName = "SUV洗车（服务）",
                Description = "SUV洗车（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 保养工时费
            result.Add(new PromotionItem
            {
                ProductName = "保养工时费",
                RuleID = 123631,
                PromotionName = "保养工时费",
                Description = "保养工时费",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 标准洗车
            result.Add(new PromotionItem
            {
                ProductName = "标准洗车",
                RuleID = 123632,
                PromotionName = "标准洗车（服务）",
                Description = "标准洗车（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 磁护机油
            result.Add(new PromotionItem
            {
                ProductName = "磁护机油",
                RuleID = 123651,
                PromotionName = "磁护机油",
                Description = "磁护机油",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 磁护机油4L
            result.Add(new PromotionItem
            {
                ProductName = "磁护机油4L",
                RuleID = 123652,
                PromotionName = "磁护机油4L",
                Description = "磁护机油4L",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 磁护机油5L
            result.Add(new PromotionItem
            {
                ProductName = "磁护机油5L",
                RuleID = 123653,
                PromotionName = "磁护机油5L",
                Description = "磁护机油5L",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 发动机燃烧室积炭清洗
            result.Add(new PromotionItem
            {
                ProductName = "发动机燃烧室积炭清洗",
                RuleID = 123678,
                PromotionName = "发动机燃烧室积炭清洗",
                Description = "发动机燃烧室积炭清洗",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            result.Add(new PromotionItem
            {
                ProductName = "发动机燃烧室积炭清洗",
                RuleID = 123633,
                PromotionName = "发动机燃烧室积炭清洗（服务）",
                Description = "发动机燃烧室积炭清洗（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 发动机润滑系统清洗
            result.Add(new PromotionItem
            {
                ProductName = "发动机润滑系统清洗",
                RuleID = 123658,
                PromotionName = "发动机润滑系统清洗",
                Description = "发动机润滑系统清洗",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            result.Add(new PromotionItem
            {
                ProductName = "发动机润滑系统清洗",
                RuleID = 123671,
                PromotionName = "发动机润滑系统清洗（服务）",
                Description = "发动机润滑系统清洗（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 更换空气格
            result.Add(new PromotionItem
            {
                ProductName = "更换空气格",
                RuleID = 123635,
                PromotionName = "更换空气格",
                Description = "更换空气格",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            result.Add(new PromotionItem
            {
                ProductName = "更换空气格",
                RuleID = 123636,
                PromotionName = "更换空气格（服务）",
                Description = "更换空气格（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 更换空调格
            result.Add(new PromotionItem
            {
                ProductName = "更换空调格",
                RuleID = 123637,
                PromotionName = "更换空调格",
                Description = "更换空调格",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            result.Add(new PromotionItem
            {
                ProductName = "更换空调格",
                RuleID = 123638,
                PromotionName = "更换空调格（服务）",
                Description = "更换空调格（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 换油工时
            result.Add(new PromotionItem
            {
                ProductName = "换油工时",
                RuleID = 123640,
                PromotionName = "换油工时",
                Description = "换油工时",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 机油格
            result.Add(new PromotionItem
            {
                ProductName = "机油格",
                RuleID = 123634,
                PromotionName = "机油格",
                Description = "机油格",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 极护机油4L
            result.Add(new PromotionItem
            {
                ProductName = "极护机油4L",
                RuleID = 123654,
                PromotionName = "极护机油4L",
                Description = "极护机油4L",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 极护机油5L
            result.Add(new PromotionItem
            {
                ProductName = "极护机油5L",
                RuleID = 123655,
                PromotionName = "极护机油5L",
                Description = "极护机油5L",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 金嘉护机油4L
            result.Add(new PromotionItem
            {
                ProductName = "金嘉护机油4L",
                RuleID = 123656,
                PromotionName = "金嘉护机油4L",
                Description = "金嘉护机油4L",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 免费1平方油漆
            result.Add(new PromotionItem
            {
                ProductName = "免费1平方油漆",
                RuleID = 123641,
                PromotionName = "免费1平方油漆（服务）",
                Description = "免费1平方油漆（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 免费洗车
            result.Add(new PromotionItem
            {
                ProductName = "免费洗车",
                RuleID = 123642,
                PromotionName = "免费洗车（服务）",
                Description = "免费洗车（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 喷油嘴清洗
            result.Add(new PromotionItem
            {
                ProductName = "喷油嘴清洗",
                RuleID = 123639,
                PromotionName = "喷油嘴清洗",
                Description = "喷油嘴清洗",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });

            result.Add(new PromotionItem
            {
                ProductName = "喷油嘴清洗",
                RuleID = 123650,
                PromotionName = "喷油嘴清洗（服务）",
                Description = "喷油嘴清洗（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 漆面去污打蜡
            result.Add(new PromotionItem
            {
                ProductName = "漆面去污打蜡",
                RuleID = 123643,
                PromotionName = "漆面去污打蜡（服务）",
                Description = "漆面去污打蜡（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 清洗空调和空气格
            result.Add(new PromotionItem
            {
                ProductName = "清洗空调和空气格",
                RuleID = 123644,
                PromotionName = "清洗空调和空气格（服务）",
                Description = "清洗空调和空气格（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 室内美容
            result.Add(new PromotionItem
            {
                ProductName = "室内美容",
                RuleID = 123645,
                PromotionName = "室内美容（服务）",
                Description = "室内美容（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 四轮定位
            result.Add(new PromotionItem
            {
                ProductName = "四轮定位",
                RuleID = 123646,
                PromotionName = "四轮定位（服务）",
                Description = "四轮定位（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 四轮换位
            result.Add(new PromotionItem
            {
                ProductName = "四轮换位",
                RuleID = 123647,
                PromotionName = "四轮换位（服务）",
                Description = "四轮换位（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 养护项目工时费
            result.Add(new PromotionItem
            {
                ProductName = "养护项目工时费",
                RuleID = 123648,
                PromotionName = "养护项目工时费",
                Description = "养护项目工时费",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #region 油漆
            result.Add(new PromotionItem
            {
                ProductName = "油漆",
                RuleID = 123649,
                PromotionName = "油漆（服务）",
                Description = "油漆（服务）",
                Discount = 0,
                MinMoney = 0,
                CodeChannel = "武汉光谷一路店"
            });
            #endregion
            #endregion

            return result;
        }
    }
}