using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;
using Tuhu.MessageQueue;
using Tuhu;

namespace Tuhu.C.Job.UserAccountCombine.Job
{
    public class UserAuthMultiBindMonitorJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(UserAuthMultiBindMonitorJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info("UserAuthMultiBindMonitorJob开始执行：" + DateTime.Now.ToString());
                var result_GetCount_UnionIdBindMultiUserIds = UACManager.GetCount_UnionIdBindMultiUserIds();
                var result_GetCount_UserIdBindMultiUnionIds = UACManager.GetCount_UserIdBindMultiUnionIds();
                if (result_GetCount_UnionIdBindMultiUserIds == 0 && result_GetCount_UserIdBindMultiUnionIds == 0)
                {
                    _logger.Info("UserAuthMultiBindMonitorJob今天没有出现绑定异常：" + DateTime.Now.ToString());
                }
                else
                {
                    var body = new StringBuilder(10000);
                    string issue = string.Empty;
                    if (result_GetCount_UnionIdBindMultiUserIds > 0)
                        issue += string.Format(EmailTrBase, "UnionId绑定多UserId监控", result_GetCount_UnionIdBindMultiUserIds.ToString(), Sql_UnionIdBindMultiUserIds);
                    if (result_GetCount_UserIdBindMultiUnionIds > 0)
                        issue += string.Format(EmailTrBase, "UserId绑定多UnionId监控", result_GetCount_UserIdBindMultiUnionIds.ToString(), Sql_UserIdBindMultiUnionIds);
                    body.Append(string.Format(EmailBaseStyle, issue));

                    var message = body.ToString();
                    Tuhu.TuhuMessage.SendEmail("【Warning】出现用户授权绑定多账号问题EOM",
                        "zhangchen3@tuhu.cn;liuchao1@tuhu.cn;liuyangyang@tuhu.cn",
                        message);
                }
                _logger.Info("UserAuthMultiBindMonitorJob执行结束：" + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                _logger.Info($"UserAuthMultiBindMonitor：运行异常=》{ex}");
            }
        }

        #region Template
        private const string EmailBaseStyle = @"<!DOCTYPE html>
<html style='font-weight: 300'>
 <head style='font-weight: 300'> 
  <meta charset='utf-8' style='font-weight: 300' /> 
  <meta name='viewport' content='width=device-width, initial-scale=1' style='font-weight: 300' /> 
 </head> 
 <body> 
  <table class='main' style='border-radius: 4px; font-size: 16px; color: #2f2936; border-collapse: separate; box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1); border-spacing: 0; max-width: 700px; font-family: &quot;Lato&quot;, &quot;Helvetica Neue&quot;, helvetica, sans-serif; border: 1px solid #c7d0d4; padding: 0; -webkit-font-smoothing: antialiased; width: 100%; font-weight: 300; margin: 15px auto; background-color: #fff'> 
   <tbody>
    <tr style='font-weight: 300'> 
     <td style='padding: 0; font-weight: 300; margin: 0; text-align: center'> 
      <div class='header' style='padding: 23px 0; font-size: 14px; font-weight: 300; border-bottom: 1px solid #dee7eb'> 
       <div class='container' style='padding: 0 20px; max-width: 600px; font-weight: 300; margin: 0 auto; text-align: left'> 
        <h1 style='font-size: 38px; color: #000; letter-spacing: -1px; padding: 0; font-weight: normal; margin: 0; line-height: 42px'> <a href='https://yewu.tuhu.cn/Select/ShowAllResult' style='display:block; mar:13px;overflow:hidden;'><img src='https://img3.tuhu.org/Home/Image/1542A950674B44BD199996EEE823D31E.png?imageView2/2/format/png/q/100' /></a> </h1> 
       </div> 
      </div> </td> 
    </tr> 
    {0}
   </tbody>
  </table>  
 </body>
</html>";

        private const string EmailTrBase = @"<tr style='font-weight: 300'> 
     <td style='padding: 0; font-weight: 300; margin: 0; text-align: center'> 
      <div class='container' style='padding: 0 20px; max-width: 600px; font-weight: 300; margin: 0 auto; text-align: left'> 
       <div class='inner' style='padding: 30px 0 20px; font-weight: 300; background-color: #fff'> 
        <h2 style='font-size: 26px; font-weight: 700; margin: 0 0 20px'>{0}</h2> 
        <p style='font-weight: 300; margin: 0 0 15px; font-size: 16px; line-height: 24px'>出现 <strong style='font-weight: bold'>{1}</strong> 例这种数据，请下列查询语句检查。</p> 
        <p class='via' style='border-radius: 3px; font-size: 14px; padding: 15px; font-weight: 300; margin: 15px 0; line-height: 24px; background-color: #f7f8f9; text-align: left'> 
		{2}
		</p> 
       </div> 
      </div> </td> 
    </tr> ";

        private const string Sql_UnionIdBindMultiUserIds = @"
WITH UnionIdBindMultiUserIds<br /> 
AS ( SELECT   DISTINCT UnionId<br /> 
     FROM     Tuhu_profiles..UserAuth WITH ( NOLOCK )<br /> 
     WHERE    AuthSource = 'Weixin'<br /> 
              AND BindingStatus = 'Bound'<br /> 
              AND UnionId IS NOT NULL<br /> 
              AND UnionId <> ''<br /> 
     GROUP BY UnionId<br /> 
     HAVING   COUNT(DISTINCT UserId) > 1<br /> 
   )<br /> 
SELECT   UnionIdBindMultiUserIds.UnionId ,<br /> 
         Auth.CreatedTime<br /> 
FROM     UnionIdBindMultiUserIds<br /> 
         JOIN Tuhu_profiles..UserAuth AS Auth WITH ( NOLOCK ) ON Auth.UnionId = UnionIdBindMultiUserIds.UnionId<br /> 
ORDER BY Auth.CreatedTime DESC;<br /> ";
        private const string Sql_UserIdBindMultiUnionIds = @"
WITH UserIdBindMultiUnionIds<br />
AS ( SELECT   DISTINCT UserId<br />
     FROM     Tuhu_profiles..UserAuth WITH ( NOLOCK )<br />
     WHERE    AuthSource = 'Weixin'<br />
              AND BindingStatus = 'Bound'<br />
              AND UnionId IS NOT NULL<br />
              AND UnionId <> ''<br />
     GROUP BY UserId<br />
     HAVING   COUNT(DISTINCT UnionId) > 1<br />
   )<br />
SELECT   UserIdBindMultiUnionIds.UserId ,<br />
         Auth.CreatedTime<br />
FROM     UserIdBindMultiUnionIds<br />
         JOIN Tuhu_profiles..UserAuth AS Auth WITH ( NOLOCK ) ON Auth.UserId = UserIdBindMultiUnionIds.UserId<br />
ORDER BY Auth.CreatedTime DESC;<br />";
        #endregion
    }
}
