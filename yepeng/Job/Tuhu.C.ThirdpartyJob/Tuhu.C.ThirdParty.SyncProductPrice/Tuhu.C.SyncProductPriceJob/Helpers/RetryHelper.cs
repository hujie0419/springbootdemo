using System;
using System.Threading.Tasks;

namespace Tuhu.C.SyncProductPriceJob.Helpers
{
    internal class RetryHelper
    {
        public static TResult TryInvoke<TResult>(Func<TResult> func, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = func();
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                Task.WaitAll(Task.Delay(1000 * time));
                result = func();
            }
            return result;
        }

        public static async Task<TResult> TryInvokeAsync<TResult>(Func<Task<TResult>> func, Func<TResult, bool> validFunc, int maxRetryTimes = 3)
        {
            var result = await func();
            var time = 1;
            while (!validFunc(result) && time++ < maxRetryTimes)
            {
                await Task.Delay(1000 * time);
                result = await func();
            }
            return result;
        }
    }
}
