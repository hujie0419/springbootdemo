using BaoYangRefreshCacheService.DAL;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.VehiclePart;

namespace BaoYangRefreshCacheService.BLL
{
    public class VehiclePartBll
    {
        private ILog logger;

        public VehiclePartBll(ILog logger)
        {
            this.logger = logger;
        }

        public void RefreshEsData()
        {
            var beginTime = DateTime.Now;
            var count = ProductDal.GetProductEsCount();

            const int batchsize = 500;

            var pageCount = Math.Ceiling((double)count / batchsize);

            bool updateSuccess = true;

            bool indexSuccess = false;
            using(var client = new EpcClient())
            {
                var indexResult = client.CreateIndex();
                indexSuccess = indexResult.Success && indexResult.Result;
            }
            if (indexSuccess)
            {
                for (var index = 1; index <= pageCount; index++)
                {
                    var pids = ProductDal.GetProductEsPids(batchsize, index);
                    using (var client = new EpcClient())
                    {
                        var result = client.RefreshEsDataByPidsWithUpdateTime(pids, beginTime.AddSeconds(1));
                        if (!result.Success)
                        {
                            using (var client2 = new EpcClient())
                            {
                                Thread.Sleep(200);
                                result = client2.RefreshEsDataByPids(pids);
                                if (!result.Success)
                                {
                                    logger.Error($"RefreshEsDataByPids:{index}失败", result.Exception);
                                    updateSuccess = false;
                                    continue;
                                }
                            }
                        }
                    }

                    logger.Info($"RefreshEsDataByPids:{index}成功!");
                }
            }
            else
            {
                updateSuccess = false;
            }


            if (updateSuccess)
            {
                Thread.Sleep(30 * 1000);
                using (var client = new EpcClient())
                {
                    var deleteResult = client.DeleteEsDataByTime(beginTime);
                    if (!deleteResult.Success)
                    {
                        logger.Error($"DeleteEsDataByTime:删除老数据出错", deleteResult.Exception);
                    }
                }

                logger.Info($"全部成功");
            }            
        }
    }
}
