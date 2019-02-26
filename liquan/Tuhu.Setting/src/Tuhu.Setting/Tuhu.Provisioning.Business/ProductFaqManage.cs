using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
   public  class ProductFaqManage
    {        /// <summary>
             /// 查询所有可选配置
             /// </summary>
        public static IEnumerable<SimpleProductModel> SelectSimpleProductModels(List<CBrandModel> models,  string pids,Pagination pagination=null)
        {
            return DalFaqManage.SelectSimpleProductModels(models , pids, pagination);
        }

        public static IEnumerable<ProductFaqConfigModel> SelectProductFaqConfigModelsPagination(Pagination pagination)
        {
            try
            {
                return DalFaqManage.SelectProductFaqConfigModelsPagination(pagination);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public static IEnumerable<ProductFaqConfigModel> SelectProductFaqConfigModels()
        {
            return DalFaqManage.SelectProductFaqConfigModels();
        }
        public static IEnumerable<ProductFaqConfigDetailModel> SelectProductFaqConfigDetailModels(string pid)
        {
            return DalFaqManage.SelectProductFaqConfigDetailModels(pid);
        }
        public static bool UpdateProductFaqConfigAndDetailDetailModels(List<ProductFaqConfigDetailModel> models,List<string> pids)
        {
            try
            {
                var lisPids = pids;
                var questionDetais = string.Join(",",models.Select(r => r.Question).ToArray());
                //var aa = , questionDetais);
                bool result = true;
                //1,插入问题表，一次只会插入一条，但是这个会对应多个问答列表，这个问题也会应用到多个产品信息里
                var faqModel = new ProductFaqConfigModel
                {
                    QuestionDetail = questionDetais
                };
                var listpidModes = lisPids.Select(pid => new ProductFaqConfigModel
                {
                    Pid = pid,
                }).ToList();
                var returnId = DalFaqManage.InsertProductFaqConfigModel(faqModel);
                //2，不去管老的问题，新增一个问题关系
                foreach (var model in models)
                {
                    result = result && DalFaqManage.InsertProductFaqConfigDetailModels(model, returnId);
                }
                //3，对于产品的话，可以更新已经存在的，新增新选中过来的，或者直接删掉所有，重新新增一遍，新增的数据可能会很多，可以选择批量新增，这个看下性能问题再说
                foreach (var patchs in lisPids.Split(100).Select(_ => _.ToList()))
                {
                    result = result && DalFaqManage.DeleteProductFaqPidDetailForPatch(patchs);

                }
                result = result && DalFaqManage.InsertProductFaqPidDetailPatch(lisPids, returnId);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static IEnumerable<ZTreeModel> SelectProductCategories()
        {
            try
            {
                return DalFaqManage.SelectProductCategories();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static bool PatchDelFaqConfigPid(List<string> pids)
        {
            var result = true;
            foreach (var patchs in pids.Split(100).Select(_ => _.ToList()))
            {
                result = result && DalFaqManage.DeleteProductFaqPidDetailForPatch(patchs);

            }
            return result;
        }

        public  static int SelectNotSameFaqCount(List<string> pids, int fkId)
        {
            int result = 0;
            foreach (var patchs in pids.Split(100).Select(_ => _.ToList()))
            {
                 result= DalFaqManage.SelectNotSameFaqCount(patchs, fkId);
                if (result > 0)
                    break;
            }
            return result;
        }
    }
}
