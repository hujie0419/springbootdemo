using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Tuhu.C.Job.DAL;
using Tuhu.C.Job.BLL;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class BeautyGroupSoldInfoStatictisJob : BaseJob
    {
        protected override string JobName => nameof(BeautyGroupSoldInfoStatictisJob);
        public BeautyGroupSoldInfoStatictisJob()
        {
            logger = LogManager.GetLogger<BeautyGroupSoldInfoStatictisJob>();
        }
        public override void Exec()
        {
            logger.Info($"BeautyGroupSoldInfoStatictisJob:Begin");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            logger.Info($"BeautyGroupSoldInfoStatictisJob:End");
        }
        private const int pageSize = 20;
        private void DoJob()
        {
            var shopIds = AsyncHelper.RunSync(() => BeautyGroupBuyingDal.Instanse.GetAllShopIdsAsync());
            if (shopIds != null && shopIds.Any())
            {
                int index = 1;
                while (true)
                {
                    var shop_ids = shopIds.Skip((index - 1) * pageSize).Take(pageSize);
                    if (shop_ids.Any())
                    {
                        var updateResult = false;
                        try
                        {
                            updateResult = AsyncHelper.RunSync(() => BeautyGroupBuyingBLL.Instanse.UpdateSoldInfoStatictisAsync(shop_ids.Select(s => s.PKID)));
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message, ex);
                            System.Threading.Thread.Sleep(1000);
                        }
                        if (updateResult)
                            logger.Info($"UpdateSoldInfoStatictisAsync:第{index}批更新成功");
                        else
                        {
                            logger.Error($"UpdateSoldInfoStatictisAsync:第{index}批更新失败");
                        }
                        System.Threading.Thread.Sleep(30);
                    }
                    else
                    {
                        break;
                    }
                    index++;
                }
            }
            else
            {
                logger.Error($"BeautyGroupSoldInfoStatictisJob:为获取到门店数据");
            }
        }
    }
}
