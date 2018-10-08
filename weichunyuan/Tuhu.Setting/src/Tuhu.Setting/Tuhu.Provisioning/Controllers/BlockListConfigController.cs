using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ConfigLog;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class BlockListConfigController : Controller
    {
        #region utility

        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        #endregion utility

        /// <summary>
        /// ConfigLogManager
        /// </summary>
        private static readonly CommonConfigLogManager ConfigLogManager = new CommonConfigLogManager();

        [HttpGet]
        [PowerManage(IwSystem = "OperateSys")]
        public ActionResult List(string blockSystem, int blockType, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize <= 0 || pageSize > 500)
            {
                pageSize = 20;
            }
            using (var client = new BlockListConfigClient())
            {
                var result = client.SelectPagedBlockList(blockSystem, blockType, pageIndex, pageSize);
                if (result.Success)
                {
                    return Content(JsonConvert.SerializeObject(new PagedDataModel<BlockListItem>
                    {
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalSize = result.Result.Item1,
                        Data = result.Result.Item2.ToArray(),
                        Status = 1
                    }, DefaultJsonSerializerSettings), "application/json", Encoding.UTF8);
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        ErrorMsg = result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult Add(BlockListItem item)
        {
            using (var client = new BlockListConfigClient())
            {
                item.UpdateBy = User.Identity.Name;

                var result = client.AddBlockListItem(item);
                if (result.Success)
                {
                    if (result.Result)
                    {
                        ConfigLogManager.AddCommonConfigLogInfo(new CommonConfigLogModel
                        {
                            ObjectId = $"{item.BlockType}-{item.BlockValue}",
                            AfterValue = JsonConvert.SerializeObject(item),
                            ObjectType = $"{item.BlockSystem}BlockListLog",
                            Creator = User.Identity.Name,
                            Remark = $"新增黑名单{(BlockListType)item.BlockType}-{item.BlockValue}"
                        });
                    }
                    return Json(new
                    {
                        Status = 1,
                        Success = result.Result
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        ErrorMsg = result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(BlockListItem item)
        {
            using (var client = new BlockListConfigClient())
            {
                item.UpdateBy = User.Identity.Name;

                var result = client.DeleteBlockListItem(item);
                if (result.Success)
                {
                    if (result.Result)
                    {
                        ConfigLogManager.AddCommonConfigLogInfo(new CommonConfigLogModel
                        {
                            ObjectId = $"{item.BlockType}-{item.BlockValue}",
                            BeforeValue = JsonConvert.SerializeObject(item),
                            ObjectType = $"{item.BlockSystem}BlockListLog",
                            Creator = User.Identity.Name,
                            Remark = $"删除黑名单{(BlockListType)item.BlockType}-{item.BlockValue}"
                        });
                    }
                    return Json(new
                    {
                        Status = 1,
                        Success = result.Result
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        ErrorMsg = result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}