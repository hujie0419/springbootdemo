using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Pay;
using Tuhu.Service.Pay.Models;
using Tuhu.Service.Pay.Models.Enum;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class BestPayStatusJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BestPayStatusJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var data = SelectNotPayedSerialNumbers();//获取正在支付中和待撤销的所有的订单信息
                logger.Info($"开始查找翼支付未支付成功的订单，共{data.Count()}条");
                Dictionary<int, List<Task>> taskDic = new Dictionary<int, List<Task>>();
                List<Task[]> taskList = new List<Task[]>();
                for (int index = 0; index < data.Count; index++)
                {
                    var task = CreateTaskQueryOrder(data[index]);
                    var num = index / 10;
                    if (taskDic.ContainsKey(num) && taskDic[num] != null)
                    {
                        taskDic[num].Add(task);
                    }
                    else
                    {
                        taskDic[num] = new List<Task>() { task };
                    }
                }
                foreach (var tasks in taskDic.Values)
                {
                    foreach (var item in tasks)
                    {
                        item.Start();
                    }
                    Task.WaitAll(tasks.ToArray());
                }
            }
            catch (Exception ex)
            {
                logger.Error("翼支付状态更新Job异常", ex);
            }
        }


        public List<BestPaySerialNumber> SelectNotPayedSerialNumbers()
        {
            List<BestPaySerialNumber> result = new List<BestPaySerialNumber>();

            try
            {
                using (var client = new PayClient())
                {
                    result = client.SelectNotPayedSerialNumbers().Result.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public Task CreateTaskQueryOrder(BestPaySerialNumber data)
        {
            return new Task(() => QueryOrder(data));
        }

        public void QueryOrder(BestPaySerialNumber data)
        {

            try
            {
                if (data != null)
                {
                    var isReverse = (DateTime.Now - data.CreateTime).TotalSeconds > 40 || data.PayStatus == TuhuBestPayStatusType.WaitingReverse;

                    using (var client = new PayClient())
                    {
                        if (isReverse)
                        {
                            var result = client.Reverse(data.OrderId, data.SerialNumber);
                        }
                        else
                        {
                            var result = client.Query(data.OrderId, data.SerialNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
