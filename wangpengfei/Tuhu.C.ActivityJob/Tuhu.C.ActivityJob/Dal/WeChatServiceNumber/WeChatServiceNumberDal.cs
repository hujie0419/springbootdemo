using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ThirdParty.Models.WeiXin;

namespace Tuhu.C.ActivityJob.Dal.WeChatServiceNumber
{
    /// <summary>
    /// 微信服务号导购
    /// </summary>
    public class WeChatServiceNumberDal
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(WeChatServiceNumberDal));

        /// <summary>
        /// 批量导入聊天记录
        /// </summary>
        /// <param name="chatMsgList"></param>
        /// <returns></returns>
        public static bool ImportWechatServiceNumberChatLog(IEnumerable<IEnumerable<ChatMsg>> chatMsgList)
        {
            bool result = false;
            
              
                try
                {
                    var chatMsgNewList = chatMsgList.ToList();
                    Parallel.For(0, chatMsgNewList.Count, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (i) =>
                    {
                            //将数据集合和目标服务器库表中的字段对应
                            DataTable table = new DataTable();
                            table.Columns.Add("PKID", typeof(int));
                            table.Columns.Add("Worker", typeof(string));
                            table.Columns.Add("Openid", typeof(string));
                            table.Columns.Add("Opercode", typeof(string));
                            table.Columns.Add("ChatRecord", typeof(string));
                            table.Columns.Add("OperatingTime", typeof(DateTime));
                            table.Columns.Add("CreateTime", typeof(DateTime));
                            table.Columns.Add("CreateBy", typeof(string));
                            table.Columns.Add("IsDelete", typeof(bool));

                            chatMsgNewList[i].ToList().ForEach(item =>
                            {
                                var row = table.NewRow();
                                row["PKID"] = 0;
                                row["Worker"] = item.Worker??string.Empty;
                                row["Openid"] = item.Openid ?? string.Empty;
                                row["Opercode"] = item.Opercode ?? string.Empty;
                                row["ChatRecord"] = item.ChatRecord;

                                if (item.Time == null)
                                {
                                    row["OperatingTime"] = DBNull.Value;
                                }
                                else
                                {
                                    row["OperatingTime"] = item.Time;
                                }
                                row["CreateTime"] = DateTime.Now;
                                row["CreateBy"] = string.Empty;
                                row["IsDelete"] = false;
                                table.Rows.Add(row);
                            });
                            using(var dbhelper = DbHelper.CreateDbHelper("ThirdParty"))
                            {
                                dbhelper.BeginTransaction();
                                table.TableName = "Tuhu_thirdparty..WechatServiceNumberChatLog";
                                dbhelper.BulkCopy(table);
                                dbhelper.Commit();
                            }
                            result = true;
                    });
                }
                catch (Exception ex)
                {
                Logger.Error($"ImportWechatServiceNumberChatLog 接口异常：{ex.Message},堆栈信息：{ex.StackTrace}", ex);
                }
            return result;
        }
    }
}
