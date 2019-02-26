using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.DataAccess.DAO.CompetingProductsMonitor;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.CompetingProductsMonitor;
using Tuhu.Provisioning.DataAccess.Mapping;

namespace Tuhu.Provisioning.Business.CompetingProductsMonitorManager
{
    public class CompetingProductsMonitorManager
    {
        private CompetingProductsMonitorDAL dal = null;
        public CompetingProductsMonitorManager()
        {
            dal = new CompetingProductsMonitorDAL();
        }

        /// <summary>
        /// 查询产品列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public IEnumerable<CompetingProductsMonitorModel> GetProductList(PriceSelectModel selectModel, PagerModel pager)
        {
            //获取商品列表
            var list = PriceManager.SelectPriceProductList(selectModel, pager);
            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TireListModel, CompetingProductsMonitorModel>());
            var mapper = config.CreateMapper();
            //集合类型转换
            var productMonitorlist = mapper.Map<IEnumerable<CompetingProductsMonitorModel>>(list);
            var pids = list.Select(g => g.PID).ToList();
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                //根据Pids查询竞品中最低价商品
                var monitorList = dal.GetProductsMonitorbyPids(conn, pids);
                foreach (var pid in pids)
                {
                    var rel = productMonitorlist.Where(g => g.PID == pid).First();
                    if (monitorList.Count() > 0)
                    {
                        var monitorModel = monitorList.Where(g => g.Pid == pid).FirstOrDefault();
                        if (monitorModel != null)
                        {
                            rel.MonitorCount = monitorModel.MonitorCount;
                            rel.ItemID = monitorModel.ItemID;
                            rel.SkuID = monitorModel.SkuID;
                            rel.ItemCode = monitorModel.ItemCode;
                            rel.Title = monitorModel.Title;
                            rel.ShopCode = monitorModel.ShopCode;
                            rel.MinPrice = monitorModel.MinPrice;
                            rel.LastUpdateDateTime = monitorModel.LastUpdateDateTime;
                        }
                    }
                }
                return productMonitorlist;
            }
        }

        /// <summary>
        /// 根据pid获取所有竞品信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public IEnumerable<CompetingProductsMonitorEntity> GetAllProductsMonitorbyPid(string pid)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return dal.GetAllProductsMonitorbyPid(conn, pid);
            }
        }

        /// <summary>
        /// 新增产品监控信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(CompetingProductsMonitorEntity model)
        {
            return dal.Insert(model);
        }

        /// <summary>
        /// 根据pkid删除竞品监控信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public int Delete(string pid, string shopCode, string itemId, long pkid = 0)
        {
            if (pkid > 0)
                return dal.Delete(pkid);
            else
                return dal.Delete(pid, shopCode, itemId);
        }
    }
}