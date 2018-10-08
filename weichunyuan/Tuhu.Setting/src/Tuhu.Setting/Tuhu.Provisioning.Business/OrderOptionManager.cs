using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class OrderOptionManager
    {
        /// <summary>
        /// 查询所有可选配置
        /// </summary>
        public static IEnumerable<TireCreateOrderOptionsConfigModel> SelectOrderOptions()
        {
            return DalCreateOrder.SelectOrderOptions();
        }

        /// <summary>
        /// 查询可选配置通过id
        /// </summary>
        public static TireCreateOrderOptionsConfigModel SelectOrderOptionsById(int id)
        {
            return DalCreateOrder.SelectOrderOptionById(id);
        }
        /// <summary>
        /// 查询可选配置关联商品
        /// </summary>
        public static IEnumerable<OrderOptionReferProductModel> SelectOrderOptionReferProducts(int orderOptionId)
        {
            return DalCreateOrder.SelectOrderOptionReferProducts(orderOptionId);
        }

        /// <summary>
        /// 更新可选配置关联商品
        /// </summary>
        public static bool UpdateOrderOptionReferProducts(List<OrderOptionReferProductModel> refers,int orderOptionId,string user)
        {
            var result = true;
            var model= DalCreateOrder.SelectOrderOptionReferProducts(orderOptionId);
            if (model.Any())
            {
                var del = DalCreateOrder.DelOrderOptionReferProducts(orderOptionId);
                result = del >= 0;
            }
            if (refers != null && refers.Any())
            {
                foreach (var refer in refers)
                {
                    refer.OrderOptionId = orderOptionId;
                    var res = DalCreateOrder.InsertOrderOptionReferProducts(refer);
                    result = result && res > 0;
                }
            }
            var oprLog = new FlashSaleProductOprLog
            {
                OperateUser = user,
                CreateDateTime = DateTime.Now,
                BeforeValue = JsonConvert.SerializeObject(model),
                AfterValue = JsonConvert.SerializeObject(refers),
                LogType = "OOption",
                LogId = orderOptionId.ToString(),
                Operation = "编辑关联产品"
            };
            LoggerManager.InsertLog("OrderOpertionOprLog", oprLog);
            //LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(refers), AfterValue = JsonConvert.SerializeObject(model), Author = user, Operation = "编辑关联产品", ObjectType = "OrderOption" });
            return result;

        }

        public static bool InsertOrderOption(TireCreateOrderOptionsConfigModel model)
        {
            var result= DalCreateOrder.InsertOrderOption(model);
            return result > 0;
        }

        public static bool UpdateOrderOption(TireCreateOrderOptionsConfigModel model)
        {
            var result = DalCreateOrder.UpdateOrderOption(model);
            return result > 0;
        }
    }
}
