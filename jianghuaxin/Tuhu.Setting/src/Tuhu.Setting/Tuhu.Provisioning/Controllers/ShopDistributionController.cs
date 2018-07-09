using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.ShopDistribution;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ShopDistribution;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Controllers
{
    public class ShopDistributionController : Controller
    {
        // GET: ShopDistribution
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        // GET: ShopDistribution/Create
        public ActionResult ListForm(Pagination pagination)
        {
            return View();
        }
        /// <summary>
        /// 获取商品的列表数据[车品类目下的商品]
        /// </summary>
        /// <param name="pagination">分页对象</param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult GetProductList(Pagination pagination, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;
            var manager = new ShopDistributionManager();
            var products = manager.GetProductList(keyword, pagination);
            return Content(JsonConvert.SerializeObject(new
            {
                rows = products,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }
        /// <summary>
        /// 获取门店铺货列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetShopDistributionList(Pagination pagination)
        {
            RepositoryManager db = new RepositoryManager();
            var list = db.GetEntityList<ShopDistributionEntity>(pagination);
            var shopDistributionModelList = list.Select(t => new ShopDistributionModel() {PKID=t.PKID,FKPID = t.FKPID, CreateTime = t.CreateTime });
            var manager = new ShopDistributionManager();
            //将商品标题组装到门店铺货配置中
            var shopDistributionlist = manager.AddProductName(shopDistributionModelList.ToList());
            return Content(JsonConvert.SerializeObject(new
            {
                rows = shopDistributionlist,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }
        /// <summary>
        /// 删除门店铺货
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult DeleteForm(int pkid)
        {
            if (pkid <= 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败,参数不能为空"
                }));
            RepositoryManager repository = new RepositoryManager();
            Expression<Func<ShopDistributionEntity, bool>> expression = _ => _.PKID == pkid;
            try
            {
                var entity = repository.GetEntity<ShopDistributionEntity>(pkid);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "删除商品"+entity.FKPID, ObjectType = "MDPHLoger" });
                repository.Delete<ShopDistributionEntity>(expression);
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"

                }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = ex.Message
                }));
            }
        }
        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public ActionResult SubmitForm(string pids)
        {
            var entity = new ShopDistributionEntity();
            RepositoryManager db = new RepositoryManager();
            entity.CreateTime = DateTime.Now;
            entity.CreateBy = User.Identity.Name;
            entity.LastUpdateBy = User.Identity.Name;
            entity.LastUpdateTime = DateTime.Now;
            string[] pidArray = pids.Split(',');
            foreach (var pid in pidArray)
            {
                entity.FKPID = pid;
                db.Add<ShopDistributionEntity>(entity);
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "添加商品" + pid, ObjectType = "MDPHLoger", ObjectID = entity.PKID.ToString() });
            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
            }));
        }

    }
}
