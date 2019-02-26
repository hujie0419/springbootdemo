using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using System.Data;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Provisioning.Business.Tire;
using System.Text.RegularExpressions;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Controllers
{
    public class InputTireSpecParamsConfigController : Controller
    {
        private readonly Lazy<TireSpecParamsConfigManager> lazy = new Lazy<TireSpecParamsConfigManager>();
        private TireSpecParamsConfigManager TireSpecParamsConfigManager
        {
            get { return lazy.Value; }
        }

        // GET: InputTireSpecParamsConfig
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ImportGrade()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件！" }, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("sheet1", true);
                    var pids = new List<string>();

                    #region 失败数据表结构
                    DataTable dirtyData = new DataTable();
                    dirtyData.Columns.Add(new DataColumn("PID", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("产品名称（质检）", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("产地", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("轮辋保护", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("载重", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("M+S", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("Treadwear耐磨指数", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("Traction抓地指数", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("Temperature温度指数", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("胎冠结构聚酯层数", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("胎冠结构钢丝层数", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("胎冠结构尼龙层数", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("胎侧结构聚酯层数", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("轮胎标签滚动阻力", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("轮胎标签湿滑抓地性", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("轮胎标签噪音", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("花纹对称", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("导向", typeof(string)));
                    dirtyData.Columns.Add(new DataColumn("插入数据结果", typeof(string)));
                    #endregion

                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow dirtyRow = dirtyData.NewRow();
                        if (null == dr["PID"] || string.IsNullOrWhiteSpace(dr["PID"].ToString()))
                        {
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                dirtyRow[i] = dr[i];
                            }
                            dirtyRow["插入数据结果"] = "失败,PID缺失.";
                            dirtyData.Rows.Add(dirtyRow);
                            continue;
                        }
                        else
                        {
                            var existData = TireSpecParamsConfigManager.CheckPidExist(dr["PID"].ToString().Trim());
                            if (dr["产品名称（质检）"] != null
                                && !string.IsNullOrWhiteSpace(dr["产品名称（质检）"].ToString()))
                            {
                                TireSpecParamsConfig config = new TireSpecParamsConfig();
                                #region generate model
                                config.PId = dr["PID"].ToString().Trim();
                                config.QualityInspectionName = dr["产品名称（质检）"]?.ToString();
                                config.OriginPlace = dr["产地"]?.ToString();
                                config.RimProtection = (
                                    null == dr["轮辋保护"]
                                    || string.IsNullOrWhiteSpace(dr["轮辋保护"].ToString())
                                    || !IsNum(dr["轮辋保护"].ToString())
                                    ) ? false : Convert.ToInt32(dr["轮辋保护"]) == 1 ? true : false;
                                config.TireLoad = dr["载重"]?.ToString();
                                config.MuddyAndSnow = (
                                    null == dr["M+S"]
                                    || string.IsNullOrWhiteSpace(dr["M+S"].ToString())
                                    || !IsNum(dr["M+S"].ToString())
                                    ) ? false : Convert.ToInt32(dr["M+S"]) == 1 ? true : false;
                                config.ThreeT_Treadwear = dr["Treadwear耐磨指数"]?.ToString();
                                config.ThreeT_Traction = dr["Traction抓地指数"]?.ToString();
                                config.ThreeT_Temperature = dr["Temperature温度指数"]?.ToString();
                                config.TireCrown_Polyester = (
                                    null == dr["胎冠结构聚酯层数"] 
                                    || string.IsNullOrWhiteSpace(dr["胎冠结构聚酯层数"].ToString()) 
                                    || !IsNum(dr["胎冠结构聚酯层数"].ToString())
                                    ) ? 0 : Convert.ToInt32(dr["胎冠结构聚酯层数"]);
                                config.TireCrown_Steel = (
                                    null == dr["胎冠结构钢丝层数"]
                                    || string.IsNullOrWhiteSpace(dr["胎冠结构钢丝层数"].ToString())
                                    || !IsNum(dr["胎冠结构钢丝层数"].ToString())
                                    ) ? 0 : Convert.ToInt32(dr["胎冠结构钢丝层数"]);
                                config.TireCrown_Nylon = (
                                    null == dr["胎冠结构尼龙层数"]
                                    || string.IsNullOrWhiteSpace(dr["胎冠结构尼龙层数"].ToString())
                                    || !IsNum(dr["胎冠结构尼龙层数"].ToString())
                                    ) ? 0 : Convert.ToInt32(dr["胎冠结构尼龙层数"]);
                                config.TireSideWall_Polyester = (
                                    null == dr["胎侧结构聚酯层数"]
                                    || string.IsNullOrWhiteSpace(dr["胎侧结构聚酯层数"].ToString())
                                    || !IsNum(dr["胎侧结构聚酯层数"].ToString())
                                    ) ? 0 : Convert.ToInt32(dr["胎侧结构聚酯层数"]);
                                config.TireLable_RollResistance = dr["轮胎标签滚动阻力"]?.ToString();
                                config.TireLable_WetGrip = dr["轮胎标签湿滑抓地性"]?.ToString();
                                config.TireLable_Noise = dr["轮胎标签噪音"]?.ToString();
                                config.PatternSymmetry = dr["花纹对称"]?.ToString();
                                config.TireGuideRotation = dr["导向"]?.ToString();
                                config.FactoryCode = dr["工厂编码"].ToString();
                                config.GrooveNum = (
                                    null == dr["沟槽数量"]
                                    || string.IsNullOrWhiteSpace(dr["沟槽数量"].ToString())
                                    || !IsNum(dr["沟槽数量"].ToString())
                                    ) ? 0 : Convert.ToInt32(dr["沟槽数量"]);
                                config.Remark = dr["备注"].ToString();
                                config.CreateTime = DateTime.Now;
                                config.LastUpdateDataTime = DateTime.Now;
                                #endregion

                                if (!existData)
                                {
                                    try
                                    {
                                        TireSpecParamsConfigManager.InsertTireSpecParamsConfig(config);
                                    }
                                    catch (Exception ex)
                                    {
                                        for (int i = 0; i < dr.ItemArray.Length; i++)
                                        {
                                            dirtyRow[i] = dr[i];
                                        }
                                        dirtyRow["插入数据结果"] = "数据插入失败." + ex.Message;
                                        dirtyData.Rows.Add(dirtyRow);
                                        continue;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        TireSpecParamsConfigManager.UpdateTireSpecParamsConfig(config);
                                    }
                                    catch (Exception ex)
                                    {
                                        for (int i = 0; i < dr.ItemArray.Length; i++)
                                        {
                                            dirtyRow[i] = dr[i];
                                        }
                                        dirtyRow["插入数据结果"] = "数据更新失败." + ex.Message;
                                        dirtyData.Rows.Add(dirtyRow);
                                        continue;
                                    }
                                }

                                pids.Add(config.PId);
                            }
                        }
                    }

                    //批量刷新缓存
                    using (var clientConfig = new ProductConfigClient())
                    {
                        clientConfig.RefreashTireSpecParamsConfigCache(pids);
                    }
                    //Response.Clear();
                    //Response.Charset = "UTF-8";
                    //Response.Buffer = true;
                    //Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    //Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode("轮胎3T指数导入失败记录", System.Text.Encoding.UTF8) + ".xls\"");
                    //Response.ContentType = "application/ms-excel";
                    //string colHeaders = string.Empty;
                    //string ls_item = string.Empty;
                    //DataRow[] myRow = dirtyData.Select();
                    //int cl = dirtyData.Columns.Count;
                    //foreach (DataRow row in myRow)
                    //{
                    //    for (int i = 0; i < cl; i++)
                    //    {
                    //        if (i == (cl - 1))
                    //        {
                    //            ls_item += row[i].ToString() + "\n";
                    //        }
                    //        else
                    //        {
                    //            ls_item += row[i].ToString() + "\t";
                    //        }
                    //    }
                    //}
                    //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ls_item.ToString());
                    //Response.AddHeader("Content-Length", bytes.Length.ToString());
                    //Response.BinaryWrite(bytes);
                    //Response.Flush();
                    //Response.End();

                    return Json(new { Status = 0, Result = "写入完成" });
                }
                else
                    return Json(new { Status = -1, Error = "请选择文件" });
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return Json(new { Status = -2, Error = ex });
            }
        }

        #region 验证文本框输入为数字
        public static bool IsNum(string str)
        {
            return Regex.IsMatch(str, @"^[-]?\d+[.]?\d*$");
        }
        #endregion
    }
}