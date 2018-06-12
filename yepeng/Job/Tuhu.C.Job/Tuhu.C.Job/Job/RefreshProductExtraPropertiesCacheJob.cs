using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class RefreshProductExtraPropertiesCacheJob : IJob
    {
        private int totalcount = 0;
        private static readonly ILog logger = LogManager.GetLogger(typeof(RefreshProductExtraPropertiesCacheJob));
        public void Execute(IJobExecutionContext context)
        {
            int pagesize = 10;
            try
            {
                var results = SelectProductExtraPropertiesPids(pagesize);
                if (results != null && results.Any())
                {
                    logger.Info($"开始刷新,一共{totalcount / pagesize}批次");
                    int count = 0;
                    foreach (var pids in results)
                    {
                        try
                        {
                            count++;
                            logger.Info($"开始刷新{count}批次,一共{totalcount / pagesize}批次");
                            if (pids != null && pids.Any() && pids.Any(x => !string.IsNullOrEmpty(x)))
                            {
                                using (var client = new Tuhu.Service.Product.CacheClient())
                                {
                                    var cacheresult = client.RefreshProductExtraPropertiesCache(pids.Where(x => !string.IsNullOrEmpty(x)));
                                    logger.Info($"结束刷新{count}批次,一共{totalcount / pagesize}批次.success:{cacheresult.Result}");
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            logger.Warn($"刷新产品扩展信息缓存,ex:{ex}");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Warn($"刷新扩展信息缓存:{ex}");
            }
        }

        private IEnumerable<IEnumerable<string>> SelectProductExtraPropertiesPids(int pagesize)
        {
            string sql =
              $"select count(1) from Tuhu_productcatalog..tbl_ProductExtraProperties  WITH(NOLOCK)  ";
            using (var dbhelper = DbHelper.CreateDbHelper(true))
            {
                var countresult = dbhelper.ExecuteScalar(sql);
                int.TryParse(countresult?.ToString(), out totalcount);
            }
            if (totalcount > 0)
            {
                int pagecount = totalcount / pagesize;
                sql = @"  SELECT  pid
    FROM    Tuhu_productcatalog..tbl_ProductExtraProperties
    ORDER BY PKID
            OFFSET ( @pagesize * ( @pageindex - 1 ) ) ROWS
    FETCH NEXT @pagesize ROWS ONLY;";
                for (int i = 1; i <= pagecount + 1; i++)
                {
                    using (var dbhelper = DbHelper.CreateDbHelper(true))
                    {
                        using (var cmd = new SqlCommand(sql))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add(new SqlParameter("@pagesize", pagesize));
                            cmd.Parameters.Add(new SqlParameter("@pageindex", i));
                            DataTable result = dbhelper.ExecuteQuery(cmd, dt => dt);
                            if (result != null && result.Rows.Count > 0)
                            {
                                var datas = result.AsEnumerable().Where(x => !string.IsNullOrEmpty(x["pid"]?.ToString())).Select(x => x["pid"].ToString());
                                yield return datas;
                            }

                        }
                    }
                }
            }

        }
    }
}
