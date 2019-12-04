using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ZeusActivity;

namespace Tuhu.C.ActivityJob.Job.MoveCar
{
    /// <summary>
    /// 描述：途虎挪车 - 批量生成途虎挪车二维码JOB
    /// 创建人：姜华鑫
    /// 创建时间：2019-06-18
    /// </summary>
    public class MoveCarGenerateQRCodeJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<MoveCarGenerateQRCodeJob>();

        public async void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("扫码挪车 - 批量生成挪车二维码JOB 开始执行");
            await RunAsync();
            stopwatch.Stop();
            Logger.Info($"扫码挪车 - 批量生成挪车二维码JOB 结束执行,用时{stopwatch.Elapsed}");
        }

        private async Task RunAsync()
        {
            try
            {
                //生成途虎挪车二维码
                using (var client = new MoveCarClient())
                {
                    ///获取所有待生成的生成挪车二维码记录
                    var generationResult = await client.GetMoveCarGeneratedRecordsAsync();
                    if (generationResult.Success && generationResult.Result != null && generationResult.Result.Count != 0)
                    {
                        var generateRecordList = generationResult.Result;
                        foreach (var item in generateRecordList)
                        {
                            ///将待生成的生成记录的生成状态改为生成中
                            await client.UpdateGeneratedRecordsStatusAsync(item.PKID);
                            int batchID = item.PKID;
                            int storeShowStatus = item.storeShowStatus;
                            var time = Stopwatch.StartNew();
                            await BatchGenerateMoveCarQRCode(item.GeneratedNum, batchID, storeShowStatus);
                            time.Stop();
                            Logger.Info($"扫码挪车 - 批量生成挪车二维码,用时{time.Elapsed}");
                            ///将生成记录的生成状态更改为已生成
                            await client.UpdateGeneratingRecordsStatusAsync(item.PKID);
                            ///添加或修改途虎挪车二维码总生成记录
                            await client.AddOrUpdateMoveCarTotalRecordAsync(item.GeneratedNum, storeShowStatus);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"MoveCarGenerateQRCodeJob -> Run -> error ", e.InnerException ?? e);
            }
        }

        private async Task BatchGenerateMoveCarQRCode(int generatedNum, int batchID, int storeShowStatus)
        {
            var tasks = new List<Task>(4);
            for (var i = 1; i <= generatedNum; i++)
            {
                tasks.Add(GenerateMoveCarQRCodeAsync(i, batchID, storeShowStatus));
                if (tasks.Count >= 4)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }
            await Task.Yield();
        }

        private async Task GenerateMoveCarQRCodeAsync(int i, int batchID, int storeShowStatus)
        {
            using (var client = new MoveCarClient())
            {
                long qrCodeID = long.Parse(batchID.ToString("0000") + i.ToString("000000"));
                for (int x = 0; x < 2; x++)
                {
                    var result = await client.GenerationMoveCarQRCodeAsync(qrCodeID, batchID, storeShowStatus);
                    if (result.Success && result.Result)
                    {
                        break;
                    }
                    else
                    {
                        Logger.Info($"批量生成挪车二维码JOB-> {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
                    }
                }
            }

        }
    }
}
