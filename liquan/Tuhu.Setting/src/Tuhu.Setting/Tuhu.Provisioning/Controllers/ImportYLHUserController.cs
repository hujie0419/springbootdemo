using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using Tuhu.Provisioning.Business.YLHUser;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft;
using System.IO;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Controllers
{
    public class ImportYLHUserController : Controller
    {
        //
        // GET: /ImportYLHUser/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ImportGrade()
        {
            try {
                if(Request.Files.Count>0)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件！" }, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("sheet1", true);

                    #region 初始化脏数据表结构
                    DataTable dirtyData = new DataTable();
                    dirtyData.Columns.Add(new DataColumn("开卡门店", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("会员编号", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("开卡日期", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("会员姓名", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("会员生日", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("会员地址", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("会员手机", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("车牌号", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("车型", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("车厂", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("车龄", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("卡号", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("条码", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("卡别", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("卡状态", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("开卡电话", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("UserID", typeof(string)));
                    #endregion

                    #region 最终插入完成数据表定义
                    DataTable finishData = new DataTable();
                    finishData.Columns.Add(new DataColumn("开卡门店", typeof(string)));
                    finishData.Columns.Add(new DataColumn("会员编号", typeof(string)));
                    finishData.Columns.Add(new DataColumn("开卡日期", typeof(string)));
                    finishData.Columns.Add(new DataColumn("会员姓名", typeof(string)));
                    finishData.Columns.Add(new DataColumn("会员生日", typeof(string)));
                    finishData.Columns.Add(new DataColumn("会员地址", typeof(string)));
                    finishData.Columns.Add(new DataColumn("会员手机", typeof(string)));
                    finishData.Columns.Add(new DataColumn("车牌号", typeof(string)));
                    finishData.Columns.Add(new DataColumn("车型", typeof(string)));
                    finishData.Columns.Add(new DataColumn("车厂", typeof(string)));
                    finishData.Columns.Add(new DataColumn("车龄", typeof(string)));
                    finishData.Columns.Add(new DataColumn("卡号", typeof(string)));
                    finishData.Columns.Add(new DataColumn("条码", typeof(string)));
                    finishData.Columns.Add(new DataColumn("卡别", typeof(string)));
                    finishData.Columns.Add(new DataColumn("卡状态", typeof(string)));
                    finishData.Columns.Add(new DataColumn("开卡电话", typeof(string)));
                    finishData.Columns.Add(new DataColumn("UserID", typeof(string)));
                    #endregion

                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow dirtyRow = dirtyData.NewRow();
                        DataRow finishRow = finishData.NewRow();
                        if (null == dr["会员手机"] ||
                            string.IsNullOrWhiteSpace(dr["会员手机"].ToString()) ||
                            //dr["会员手机"].ToString().Count() != 11 ||              //v2.0无视手机位数11
                            null == dr["会员编号"] ||
                            string.IsNullOrWhiteSpace(dr["会员编号"].ToString()) ||
                            null == dr["卡号"] ||
                            string.IsNullOrWhiteSpace(dr["卡号"].ToString()) ||
                            null == dr["条码"] ||
                            string.IsNullOrWhiteSpace(dr["条码"].ToString())) // 不正常手机号\空会员编号\空卡号\空条形码,存储脏数据表
                        {
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                dirtyRow[i] = dr[i];
                            }
                            dirtyData.Rows.Add(dirtyRow);
                            continue;
                        }

                        #region step 1: 获取UserID或者添加用户到UserObject
                        var tempUserId = YLHUserManager.GetUserId(dr["会员手机"].ToString());  //获取是否已经是tuhu用户
                        //var dtExistedMobile = finishData.Select("会员手机='" + dr["会员手机"].ToString() + "'");

                        if (string.IsNullOrEmpty(tempUserId))
                        //if (string.IsNullOrEmpty(tempUserId) && dtExistedMobile.Count() == 0)   //不是tuhu用户，插入userobject，生成新的userid
                        {
                            tbl_UserObjectModel userObject = new tbl_UserObjectModel();
                            #region generate userObjectModel
                            //UserID
                            userObject.u_user_id = Guid.NewGuid().ToString("B");
                            //会员手机号
                            userObject.u_mobile_number = dr["会员手机"].ToString();
                            //会员姓名
                            userObject.u_last_name = null == dr["会员姓名"] || 
                                string.IsNullOrWhiteSpace(dr["会员姓名"].ToString()) ||
                                "NULL"== dr["会员姓名"].ToString().ToUpper() ?
                                dr["会员手机"].ToString() :
                                dr["会员姓名"].ToString().Substring(0, dr["会员姓名"].ToString().Length > 11 ? 11 : dr["会员姓名"].ToString().Length);
                            //会员生日
                            DateTime birthday_userObject = new DateTime();
                            if (dr["会员生日"] == null || string.IsNullOrWhiteSpace(dr["会员生日"].ToString()) ||
                                "NULL" == dr["会员生日"].ToString().ToUpper())
                                birthday_userObject = DateTime.Now;
                            else
                            {
                                if (DateTime.TryParseExact(dr["会员生日"].ToString(), "yyyyMMdd",
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out birthday_userObject)) { }
                                else { birthday_userObject = DateTime.Now; }
                                if(birthday_userObject< System.Data.SqlTypes.SqlDateTime.MinValue.Value) birthday_userObject = DateTime.Now; //v2.0忽略时间不能插入数据库错误
                            }
                            userObject.dt_birthday = birthday_userObject;
                            //会员邮箱
                            userObject.u_email_address = dr["会员手机"].ToString() + "@ylh.com";
                            #endregion

                            #region 插入到数据库，如果插入失败，加入脏数据表，否则加入正常数据表
                            try
                            {
                                YLHUserManager.InsertUserToObjectTable(userObject);
                            }
                            catch(Exception ex)
                            {
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                {
                                    dirtyRow[i] = dr[i];
                                }
                                dirtyData.Rows.Add(dirtyRow);
                                WebLog.LogException(ex);
                                continue;
                            }
                            finishRow["UserID"] = userObject.u_user_id;
                            for(int i=0; i<dr.ItemArray.Length; i++)
                            {
                                finishRow[i] = dr[i];
                            }
                            #endregion
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(tempUserId))
                            {
                                finishRow["UserID"] = tempUserId;
                            }
                            //else
                            //{
                            //    finishRow["UserID"] = dtExistedMobile[0]["UserID"];
                            //}
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                finishRow[i] = dr[i];
                            }
                        }
                        finishData.Rows.Add(finishRow);
                        #endregion

                        #region step 2: 把有UserID的项加到YLH_UserInfo表
                        var tempPKID = YLHUserManager.GetYLHUserInfoPKID(finishRow["UserID"].ToString());
                        YLHUserInfoModel ylhUserInfo = new YLHUserInfoModel();
                        //if (string.IsNullOrEmpty(tempUserId) && dtExistedMobile.Count() == 0)
                        if (tempPKID < 0)
                        {
                            #region generate ylh_UserInfo
                            //UserID
                            ylhUserInfo.u_user_id = finishRow["UserID"].ToString();
                            //会员姓名
                            ylhUserInfo.MemberName = null == dr["会员姓名"] ||
                                string.IsNullOrWhiteSpace(dr["会员姓名"].ToString()) ||
                                "NULL" == dr["会员姓名"].ToString().ToUpper() ?
                                dr["会员手机"].ToString() :
                                dr["会员姓名"].ToString().Substring(0, dr["会员姓名"].ToString().Length > 11 ? 11 : dr["会员姓名"].ToString().Length);
                            //会员编号
                            ylhUserInfo.MemberNumber = dr["会员编号"].ToString();
                            //会员地址
                            ylhUserInfo.MemberAddress = null == dr["会员地址"] ||
                                string.IsNullOrWhiteSpace(dr["会员地址"].ToString()) ||
                                "NULL" == dr["会员地址"].ToString().ToUpper() ?
                                string.Empty :
                                dr["会员地址"].ToString();
                            //会员手机号
                            ylhUserInfo.MemberPhone = dr["会员手机"].ToString();
                            //会员生日
                            DateTime birthday = new DateTime();
                            if (dr["会员生日"] == null ||
                                string.IsNullOrWhiteSpace(dr["会员生日"].ToString()) ||
                                "NULL" == dr["会员生日"].ToString().ToUpper())
                                birthday = DateTime.Now;
                            else
                            {
                                if (DateTime.TryParseExact(dr["会员生日"].ToString(), "yyyyMMdd",
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out birthday)) { }
                                else { birthday = DateTime.Now; }
                                if (birthday < System.Data.SqlTypes.SqlDateTime.MinValue.Value) birthday = DateTime.Now; //v2.0忽略时间不能插入数据库错误
                            }
                            ylhUserInfo.MemberBirthday = birthday;
                            //会员积分
                            ylhUserInfo.Integration = 0;

                            ylhUserInfo.CreatedTime = DateTime.Now;
                            ylhUserInfo.UpdatedTime = DateTime.Now;
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
                                    dirtyRow[i] = dr[i];
                                }
                                dirtyData.Rows.Add(dirtyRow);
                                WebLog.LogException(ex);
                                continue;
                            }
                            #endregion
                            #endregion
                        }

                        #region step 3: 把有UserID的项加到YLH_UserVipCardInfo表
                        YLHUserVipCardInfoModel ylhUserVipCardInfo = new YLHUserVipCardInfoModel();
                        #region generate ylh_UserVipCardInfo
                        //UserID
                        ylhUserVipCardInfo.u_user_id= finishRow["UserID"].ToString();
                        //车牌号
                        ylhUserVipCardInfo.CarNumber = null == dr["车牌号"] ||
                            string.IsNullOrWhiteSpace(dr["车牌号"].ToString()) ||
                            "NULL" == dr["车牌号"].ToString().ToUpper() ?
                            string.Empty : dr["车牌号"].ToString();
                        //车厂信息
                        ylhUserVipCardInfo.CarFactory= null == dr["车厂牌"] || 
                            string.IsNullOrWhiteSpace(dr["车厂牌"].ToString()) ||
                            "NULL" == dr["车厂牌"].ToString().ToUpper() ?
                            string.Empty : dr["车厂牌"].ToString();
                        //车型信息
                        ylhUserVipCardInfo.CarType= null == dr["车型"] || 
                            string.IsNullOrWhiteSpace(dr["车型"].ToString()) ||
                            "NULL" == dr["车型"].ToString().ToUpper() ?
                            string.Empty : dr["车型"].ToString();
                        //车龄
                        ylhUserVipCardInfo.VehicleAge= null == dr["车龄"] || 
                            string.IsNullOrWhiteSpace(dr["车龄"].ToString()) ||
                            "NULL" == dr["车龄"].ToString().ToUpper() ?
                            0 :Convert.ToDouble( dr["车龄"]);
                        //会员Vip卡号
                        ylhUserVipCardInfo.VipCardNumber = dr["卡号"].ToString();
                        //会员会员卡条形码
                        ylhUserVipCardInfo.Display_Card_NBR = dr["条码"].ToString();
                        //会员卡状态
                        ylhUserVipCardInfo.VipCardStatus = null == dr["卡状态"] || 
                            string.IsNullOrWhiteSpace(dr["卡状态"].ToString()) ||
                            "NULL" == dr["卡状态"].ToString().ToUpper() ?
                            false : "有效" == dr["卡状态"].ToString();
                        //会员卡类别
                        ylhUserVipCardInfo.VipCardType = null == dr["卡别"] ||
                            string.IsNullOrWhiteSpace(dr["卡别"].ToString()) ||
                            "NULL" == dr["卡别"].ToString().ToUpper() ?
                            string.Empty : dr["卡别"].ToString();
                        //会员卡开卡手机号
                        ylhUserVipCardInfo.RegisterPhone= null == dr["会员开卡电话"] || 
                            string.IsNullOrWhiteSpace(dr["会员开卡电话"].ToString()) ||
                            "NULL" == dr["会员开卡电话"].ToString().ToUpper() ?
                            string.Empty : dr["会员开卡电话"].ToString();
                        //会员开卡门店
                        ylhUserVipCardInfo.RegisterAddress= null == dr["开卡门店"] || 
                            string.IsNullOrWhiteSpace(dr["开卡门店"].ToString()) ||
                            "NULL" == dr["开卡门店"].ToString().ToUpper() ?
                            string.Empty : dr["开卡门店"].ToString();
                        //会员开卡日期
                        DateTime registerDate = new DateTime();
                        if (dr["开卡日期"] == null || string.IsNullOrWhiteSpace(dr["开卡日期"].ToString()) ||
                            "NULL" == dr["开卡日期"].ToString().ToUpper())
                            registerDate = DateTime.Now;
                        else
                        {
                            if (DateTime.TryParseExact(dr["开卡日期"].ToString(), "yyyyMMdd",
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out registerDate)) { }
                            else { registerDate = DateTime.Now; }
                            if (registerDate < System.Data.SqlTypes.SqlDateTime.MinValue.Value) registerDate = DateTime.Now; //v2.0忽略时间不能插入数据库错误
                        }
                        ylhUserVipCardInfo.RegisterDate = registerDate;
                        
                        ylhUserVipCardInfo.CreatedTime = DateTime.Now;
                        ylhUserVipCardInfo.UpdatedTime = DateTime.Now;
                        #endregion

                        #region 插入到数据库，如果插入失败，加入脏数据表，否则加入正常数据表
                        try
                        {
                            YLHUserManager.InsertYLHVipCardInfo(ylhUserVipCardInfo);
                        }
                        catch(Exception ex)
                        {
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                dirtyRow[i] = dr[i];
                            }
                            dirtyData.Rows.Add(dirtyRow);
                            WebLog.LogException(ex);
                            continue;
                        }
                        #endregion
                        #endregion
                    }

                    //YLHUserManager.ExportDataTableToHtml(dirtyData, @"C:\Users\zhangchen3\Desktop\永隆行\技术文档\导用户数据\脏数据.HTML");
                    //YLHUserManager.ExportDataTableToHtml(finishData, @"C:\Users\zhangchen3\Desktop\永隆行\技术文档\导用户数据\完成数据.HTML");

                    Response.Clear();
                    Response.Charset = "UTF-8";
                    Response.Buffer = true;
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode("永隆行用户导入失败记录", System.Text.Encoding.UTF8) + ".xls\"");
                    Response.ContentType = "application/ms-excel";
                    string colHeaders = string.Empty;
                    string ls_item = string.Empty;
                    DataRow[] myRow = dirtyData.Select();
                    int cl = dirtyData.Columns.Count;
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
            catch(Exception ex) {
                WebLog.LogException(ex);
                return Json(new { Status = -2, Error = ex }, "text/html");
            }
        }
    }
}
