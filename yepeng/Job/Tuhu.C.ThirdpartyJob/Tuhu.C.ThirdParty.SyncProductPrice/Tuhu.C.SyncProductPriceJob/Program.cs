using System;
using Tuhu.C.SyncProductPriceJob.Job;

#if !DEBUG

using Topshelf;

#endif

namespace Tuhu.C.SyncProductPriceJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            new TestEntry().Test();
#else
            // topshelf configuration
            // https://topshelf.readthedocs.io/en/latest/configuration/quickstart.html
            // 部署 Topshelf 注意事项
            // https://gitlab.tuhu.cn/share/wiki/blob/master/%E9%83%A8%E7%BD%B2%E7%B3%BB%E7%BB%9F%E4%BD%BF%E7%94%A8%E6%96%87%E6%A1%A3(%E4%BF%AE%E8%AE%A2%E7%89%88).md#%E6%B3%A8%E6%84%8F%E4%BA%8B%E9%A1%B9
            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.EnablePauseAndContinue();
                x.Service<QuartzService>();
                x.UseLog4Net();
            });
#endif
        }
    }

#if DEBUG

    public class TestEntry
    {
        private readonly QuartzService _service;

        public TestEntry() => _service = new QuartzService();

        public void Test()
        {
            new SyncRemainingsPriceJob().Execute(null);
            // SyncProductPrice
            //Task.WaitAll(new JingDongPriceManage("5京东服务店", "255654ed-28ac-40fa-a219-37001b3714b8").SyncProductPrice());

            // QueryProductMappings
            //Task.WaitAll(Task.Run(async () =>
            //{
            //    var mappings = await Products.QueryProductMappings("5京东服务店");
            //    Console.WriteLine(mappings.Count());
            //}));

            // mapping
            //Task.WaitAll(new JingDongPriceManage("5京东服务店", "255654ed-28ac-40fa-a219-37001b3714b8").SyncProductMapping());

            // SyncProductsPrice
            //Task.WaitAll(new JingDongPriceManage("5京东服务店", "255654ed-28ac-40fa-a219-37001b3714b8").SyncProductsPrice(new[]
            //{
            //    new ProductPriceMappingModel
            //    {
            //        ItemId = 11410294724,
            //        SkuId = 25674851812,
            //        Pid = "TR-CP-C1|6",
            //        Title = "固铂汽车轮胎途虎品质包安装 C1",
            //        ShopCode = "5京东服务店",
            //        Properties = "1000019681:1799696303"
            //    },
            //    new ProductPriceMappingModel
            //    {
            //        ItemId = 11410294724,
            //        SkuId = 25674851806,
            //        Pid = "TR-CP-C1|9",
            //        Title = "固铂汽车轮胎途虎品质包安装 C1",
            //        ShopCode = "5京东服务店",
            //        Properties = "1000019681:1799690613"
            //    },
            //    new ProductPriceMappingModel
            //    {
            //        ItemId = 11410192759,
            //        SkuId = 25673766766,
            //        Pid = "TR-GY-EfficientGrip|24",
            //        Title = "固特异汽车轮胎途虎品质包安装 御乘",
            //        ShopCode = "5京东服务店",
            //        Properties = "1000009927:1799690612"
            //    }
            //}));

            Console.WriteLine("finish");
            Console.ReadLine();
        }

        public void StartService() => _service.StartIt();

        public void StopService() => _service.StopIt();
    }

#endif
}
