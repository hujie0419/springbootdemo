using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace BaoYangRefreshCacheService.Common
{
    public class Retry
    {
        public const int RetryTimes = 3;

        public const int DangerBoundary = 3*60*1000;

        public const int WaitUnit = 5000;

        public int waitTime = 0;

        public ILog logger;

        public Retry(ILog logger)
        {
            this.logger = logger;
        }

        public T2 RetryFunction<T1, T2>(Func<T1, T2> func, Func<T2, bool> verify, T1 param1)
        {
            int i = 0;
            T2 result = default(T2);

            while (i < RetryTimes)
            {
                result = func(param1);

                if (verify(result))
                {
                    break;
                }
                else
                {
                    i++;
                    if (i == RetryTimes)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public T2 RetryFunctionWithWait<T1, T2>(Func<T1, T2> func, Func<T2, bool> verify, T1 param1)
        {
            int i = 0;
            T2 result = default(T2);

            while (i < RetryTimes)
            {
                result = func(param1);

                if (verify(result))
                {
                    break;
                }
                else
                {
                    waitTime += WaitUnit;
                    Thread.Sleep(WaitUnit);

                    if (waitTime > DangerBoundary)
                    {
                        logger.Error($"服务重试次数过多, 每次等待时间{WaitUnit}ms, 当前总共已等待时间{waitTime}ms");
                    }

                    i++;
                    if (i == RetryTimes)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public T1 RetryFunctionWithWait<T1>(Func<T1> func, Func<T1, bool> verify)
        {
            int i = 0;
            T1 result = default(T1);

            while (i < RetryTimes)
            {
                result = func();

                if (verify(result))
                {
                    break;
                }
                else
                {
                    waitTime += WaitUnit;
                    Thread.Sleep(WaitUnit);

                    if (waitTime > DangerBoundary)
                    {
                        logger.Error($"服务重试次数过多, 每次等待时间{WaitUnit}ms, 当前总共已等待时间{waitTime}ms");
                    }

                    i++;
                    if (i == RetryTimes)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
